using DBSrv.Storage.Impl;
using DBSrv.Storage.Model;
using OpenMir2;
using OpenMir2.Packets.ClientPackets;
using OpenMir2.Packets.ServerPackets;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace DBSrv.Storage.MySQL
{
    public partial class PlayDataStorage : IPlayDataStorage
    {

        private readonly Dictionary<string, int> _NameQuickMap;
        private readonly Dictionary<int, int> _IndexQuickIdMap;
        private readonly PlayQuickList _mirQuickIdList;
        private readonly StorageOption _storageOption;

        public PlayDataStorage(StorageOption storageOption)
        {
            _NameQuickMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            _mirQuickIdList = new PlayQuickList();
            _IndexQuickIdMap = new Dictionary<int, int>();
            _storageOption = storageOption;
        }

        public void LoadQuickList()
        {
            const string sSqlString = "SELECT * FROM characters WHERE Deleted=0";
            _IndexQuickIdMap.Clear();
            _NameQuickMap.Clear();
            _mirQuickIdList.Clear();
            IList<PlayQuick> accountList = new List<PlayQuick>();
            IList<string> chrNameList = new List<string>();
            using StorageContext context = new StorageContext(_storageOption);
            bool success = false;
            context.Open(ref success);
            if (!success)
            {
                return;
            }
            try
            {
                MySqlConnector.MySqlCommand command = context.CreateCommand();
                command.CommandText = sSqlString;
                using MySqlConnector.MySqlDataReader dr = command.ExecuteReader();
                int nIndex = 0;
                while (dr.Read())
                {
                    nIndex = dr.GetInt32("Id");
                    bool boDeleted = dr.GetBoolean("Deleted");
                    string sAccount = dr.GetString("LoginID");
                    string sChrName = dr.GetString("ChrName");
                    if (!boDeleted && (!string.IsNullOrEmpty(sChrName)))
                    {
                        _NameQuickMap.Add(sChrName, nIndex);
                        accountList.Add(new PlayQuick()
                        {
                            Account = sAccount,
                            SelectID = 0
                        });
                        chrNameList.Add(sChrName);
                        _IndexQuickIdMap.Add(nIndex, nIndex);
                    }
                }
                dr.Close();
                dr.Dispose();
            }
            finally
            {
                context.Dispose();
            }
            for (int nIndex = 0; nIndex < accountList.Count; nIndex++)
            {
                _mirQuickIdList.AddRecord(accountList[nIndex].Account, chrNameList[nIndex], 0, accountList[nIndex].SelectID);
            }
            chrNameList.Clear();
            accountList.Clear();
            //m_MirQuickList.SortString(0, m_MirQuickList.Count - 1);
        }

        public int Index(string sName)
        {
            if (_NameQuickMap.TryGetValue(sName, out int Index))
            {
                return Index;
            }
            return -1;
        }

        public int Get(int nIndex, ref CharacterDataInfo humanRcd)
        {
            int result = -1;
            if (nIndex < 0)
            {
                return result;
            }
            if (!_IndexQuickIdMap.ContainsKey(nIndex))
            {
                return result;
            }
            if (GetRecord(nIndex, ref humanRcd))
            {
                result = nIndex;
            }
            return result;
        }

        public bool Get(string chrName, ref CharacterDataInfo humanRcd)
        {
            if (string.IsNullOrEmpty(chrName))
            {
                return false;
            }
            if (_NameQuickMap.TryGetValue(chrName, out int nIndex))
            {
                if (GetRecord(nIndex, ref humanRcd))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public CharacterData Query(int playerId)
        {
            CharacterData playData;
            using StorageContext context = new StorageContext(_storageOption);
            try
            {
                bool success = false;
                context.Open(ref success);
                if (!success)
                {
                    return null;
                }
                playData = GetChrRecord(playerId, context);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                context.Dispose();
            }
            return playData;
        }

        private bool GetRecord(int nIndex, ref CharacterDataInfo humanRcd)
        {
            int playerId = _IndexQuickIdMap[nIndex];
            if (playerId == 0)
            {
                return false;
            }
            using StorageContext context = new StorageContext(_storageOption);
            try
            {
                bool success = false;
                context.Open(ref success);
                if (!success)
                {
                    return false;
                }
                humanRcd = new CharacterDataInfo();
                humanRcd.Data = GetChrRecord(playerId, context);
                humanRcd.Header.SetName(humanRcd.Data.ChrName);
                humanRcd.Data.Abil = GetAbilGetRecord(playerId, context);
                GetBonusAbilRecord(playerId, context, ref humanRcd);
                GetMagicRecord(playerId, context, ref humanRcd);
                GetItemRecord(playerId, context, ref humanRcd);
                GetBagItemRecord(playerId, context, ref humanRcd);
                GetStorageRecord(playerId, context, ref humanRcd);
                GetPlayerStatus(playerId, context, ref humanRcd);
            }
            catch (Exception e)
            {
                LogService.Error($"获取角色[{nIndex}]数据失败." + e.StackTrace);
                return false;
            }
            finally
            {
                context.Dispose();
            }
            return true;
        }

        private CharacterData GetChrRecord(int playerId, StorageContext context)
        {
            const string sSqlString = "SELECT * FROM characters WHERE ID=@ID";
            try
            {
                CharacterData humInfoData = null;
                MySqlConnector.MySqlCommand command = context.CreateCommand();
                command.CommandText = sSqlString;
                command.Parameters.AddWithValue("@Id", playerId);
                using MySqlConnector.MySqlDataReader dr = command.ExecuteReader();
                if (dr.Read())
                {
                    humInfoData = new CharacterData();
                    humInfoData.Account = dr.GetString("LoginID");
                    humInfoData.ChrName = dr.GetString("ChrName");
                    if (!dr.IsDBNull(dr.GetOrdinal("MapName")))
                    {
                        humInfoData.CurMap = dr.GetString("MapName");
                    }
                    humInfoData.CurX = dr.GetInt16("CX");
                    humInfoData.CurY = dr.GetInt16("CY");
                    humInfoData.Dir = dr.GetByte("Dir");
                    humInfoData.Hair = dr.GetByte("Hair");
                    humInfoData.Sex = dr.GetByte("Sex");
                    humInfoData.Job = dr.GetByte("Job");
                    humInfoData.Gold = dr.GetInt32("Gold");
                    if (!dr.IsDBNull(dr.GetOrdinal("HomeMap")))
                    {
                        humInfoData.HomeMap = dr.GetString("HomeMap");
                    }
                    humInfoData.HomeX = dr.GetInt16("HomeX");
                    humInfoData.HomeY = dr.GetInt16("HomeY");
                    if (!dr.IsDBNull(dr.GetOrdinal("DearName")))
                    {
                        humInfoData.DearName = dr.GetString("DearName");
                    }
                    if (!dr.IsDBNull(dr.GetOrdinal("MasterName")))
                    {
                        humInfoData.MasterName = dr.GetString("MasterName");
                    }
                    humInfoData.IsMaster = dr.GetBoolean("IsMaster");
                    humInfoData.CreditPoint = (byte)dr.GetInt32("CreditPoint");
                    if (!dr.IsDBNull(dr.GetOrdinal("StoragePwd")))
                    {
                        humInfoData.StoragePwd = dr.GetString("StoragePwd");
                    }
                    humInfoData.ReLevel = dr.GetByte("ReLevel");
                    humInfoData.LockLogon = dr.GetBoolean("LockLogon");
                    humInfoData.BonusPoint = dr.GetInt32("BonusPoint");
                    humInfoData.GameGold = dr.GetInt32("Gold");
                    humInfoData.GamePoint = dr.GetInt32("GamePoint");
                    humInfoData.PayMentPoint = dr.GetInt32("PayMentPoint");
                    humInfoData.HungerStatus = dr.GetInt32("HungerStatus");
                    humInfoData.AllowGroup = (byte)dr.GetInt32("AllowGroup");
                    humInfoData.AttatckMode = dr.GetByte("AttatckMode");
                    humInfoData.IncHealth = dr.GetByte("IncHealth");
                    humInfoData.IncSpell = dr.GetByte("IncSpell");
                    humInfoData.IncHealing = dr.GetByte("IncHealing");
                    humInfoData.FightZoneDieCount = dr.GetByte("FightZoneDieCount");
                    humInfoData.AllowGuildReCall = dr.GetBoolean("AllowGuildReCall");
                    humInfoData.AllowGroupReCall = dr.GetBoolean("AllowGroupReCall");
                    humInfoData.GroupRcallTime = dr.GetInt16("GroupRcallTime");
                    humInfoData.BodyLuck = dr.GetDouble("BodyLuck");
                }
                dr.Close();
                dr.Dispose();
                return humInfoData;
            }
            catch (Exception ex)
            {
                LogService.Error("[Exception] PlayDataStorage.GetChrRecord");
                LogService.Error(ex.StackTrace);
                return null;
            }
        }

        private Ability GetAbilGetRecord(int playerId, StorageContext context)
        {
            Ability humanRcd = new Ability();
            try
            {
                MySqlConnector.MySqlCommand command = context.CreateCommand();
                command.CommandText = "select * from characters_ablity where playerId=@playerId";
                command.Parameters.AddWithValue("@playerId", playerId);
                using MySqlConnector.MySqlDataReader dr = command.ExecuteReader();
                if (dr.Read())
                {
                    humanRcd.Level = dr.GetByte("Level");
                    int dw = dr.GetInt32("HP");
                    humanRcd.HP = HUtil32.LoWord(dw);
                    humanRcd.AC = HUtil32.HiWord(dw);
                    dw = dr.GetInt32("MP");
                    humanRcd.MP = HUtil32.LoWord(dw);
                    humanRcd.MAC = HUtil32.HiWord(dw);
                    humanRcd.DC = dr.GetUInt16("DC");
                    humanRcd.MC = dr.GetUInt16("MC");
                    humanRcd.SC = dr.GetUInt16("SC");
                    humanRcd.Exp = dr.GetInt32("EXP");
                    humanRcd.MaxExp = dr.GetInt32("MaxExp");
                    humanRcd.Weight = dr.GetUInt16("Weight");
                    humanRcd.MaxWeight = dr.GetUInt16("MaxWeight");
                    humanRcd.WearWeight = dr.GetByte("WearWeight");
                    humanRcd.MaxWearWeight = dr.GetByte("MaxWearWeight");
                    humanRcd.HandWeight = dr.GetByte("HandWeight");
                    humanRcd.MaxHandWeight = dr.GetByte("MaxHandWeight");
                }
                dr.Close();
                dr.Dispose();
            }
            catch (Exception ex)
            {
                LogService.Error("[Exception] PlayDataStorage.GetAbilGetRecord");
                LogService.Error(ex.StackTrace);
            }
            return humanRcd;
        }

        private void GetBonusAbilRecord(int playerId, StorageContext context, ref CharacterDataInfo humanRcd)
        {
            const string sSqlString = "SELECT * FROM characters_bonusability WHERE PlayerId=@PlayerId";
            try
            {
                MySqlConnector.MySqlCommand command = context.CreateCommand();
                command.CommandText = sSqlString;
                command.Parameters.AddWithValue("@PlayerId", playerId);
                using MySqlConnector.MySqlDataReader dr = command.ExecuteReader();
                if (dr.Read())
                {
                    humanRcd.Data.BonusAbil = new NakedAbility
                    {
                        AC = dr.GetUInt16("AC"),
                        MAC = dr.GetUInt16("MAC"),
                        DC = dr.GetUInt16("DC"),
                        MC = dr.GetUInt16("MC"),
                        SC = dr.GetUInt16("SC"),
                        HP = dr.GetUInt16("HP"),
                        MP = dr.GetUInt16("MP"),
                        Hit = dr.GetByte("HIT"),
                        Speed = dr.GetInt32("SPEED"),
                        Reserved = dr.GetByte("RESERVED")
                    };
                }
                dr.Close();
                dr.Dispose();
            }
            catch (Exception)
            {
                LogService.Error("[Exception] PlayDataStorage.GetBonusAbilRecord");
            }
        }

        private void GetMagicRecord(int playerId, StorageContext context, ref CharacterDataInfo humanRcd)
        {
            const string sSqlString = "SELECT * FROM characters_magic WHERE PlayerId=@PlayerId";
            try
            {
                for (int i = 0; i < humanRcd.Data.Magic.Length; i++)
                {
                    humanRcd.Data.Magic[i] = new MagicRcd();
                }
                MySqlConnector.MySqlCommand command = context.CreateCommand();
                command.CommandText = sSqlString;
                command.Parameters.AddWithValue("@PlayerId", playerId);
                using MySqlConnector.MySqlDataReader dr = command.ExecuteReader();
                int position = 0;
                while (dr.Read())
                {
                    humanRcd.Data.Magic[position].MagIdx = dr.GetUInt16("MagicId");
                    humanRcd.Data.Magic[position].MagicKey = dr.GetChar("UseKey");
                    humanRcd.Data.Magic[position].Level = dr.GetByte("Level");
                    humanRcd.Data.Magic[position].TranPoint = dr.GetInt32("CurrTrain");
                    position++;
                }
                dr.Close();
                dr.Dispose();
            }
            catch (Exception ex)
            {
                LogService.Error($"[Exception] GetMagicRecord");
                LogService.Error(ex.StackTrace);
            }
        }

        private void GetItemRecord(int playerId, StorageContext context, ref CharacterDataInfo humanRcd)
        {
            const string sSqlString = "SELECT * FROM characters_item WHERE PlayerId=@PlayerId";
            try
            {
                MySqlConnector.MySqlCommand command = context.CreateCommand();
                command.CommandText = sSqlString;
                command.Parameters.AddWithValue("@PlayerId", playerId);
                using MySqlConnector.MySqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    int nPosition = dr.GetInt32("Position");
                    humanRcd.Data.HumItems[nPosition] = new ServerUserItem();
                    humanRcd.Data.HumItems[nPosition].MakeIndex = dr.GetInt32("MakeIndex");
                    humanRcd.Data.HumItems[nPosition].Index = dr.GetUInt16("StdIndex");
                    humanRcd.Data.HumItems[nPosition].Dura = dr.GetUInt16("Dura");
                    humanRcd.Data.HumItems[nPosition].DuraMax = dr.GetUInt16("DuraMax");
                }
                dr.Close();
                dr.Dispose();
                QueryItemAttr(context, playerId, ref humanRcd.Data.HumItems);
            }
            catch (Exception ex)
            {
                LogService.Error("[Exception] PlayDataStorage.GetItemRecord:" + ex.StackTrace);
            }
        }

        private void GetBagItemRecord(int playerId, StorageContext context, ref CharacterDataInfo humanRcd)
        {
            const string sSqlString = "SELECT * FROM characters_bagitem WHERE PlayerId=@PlayerId";
            try
            {
                MySqlConnector.MySqlCommand command = context.CreateCommand();
                command.CommandText = sSqlString;
                command.Parameters.AddWithValue("@PlayerId", playerId);
                using MySqlConnector.MySqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    int nPosition = dr.GetInt32("Position");
                    humanRcd.Data.BagItems[nPosition] = new ServerUserItem();
                    humanRcd.Data.BagItems[nPosition].MakeIndex = dr.GetInt32("MakeIndex");
                    humanRcd.Data.BagItems[nPosition].Index = dr.GetUInt16("StdIndex");
                    humanRcd.Data.BagItems[nPosition].Dura = dr.GetUInt16("Dura");
                    humanRcd.Data.BagItems[nPosition].DuraMax = dr.GetUInt16("DuraMax");
                }
                dr.Close();
                dr.Dispose();
                QueryItemAttr(context, playerId, ref humanRcd.Data.BagItems);
            }
            catch
            {
                LogService.Error("[Exception] PlayDataStorage.GetBagItemRecord");
            }
        }

        private void GetStorageRecord(int playerId, StorageContext context, ref CharacterDataInfo humanRcd)
        {
            const string sSqlString = "SELECT * FROM characters_storageitem WHERE PlayerId=@PlayerId";
            try
            {
                MySqlConnector.MySqlCommand command = context.CreateCommand();
                command.CommandText = sSqlString;
                command.Parameters.AddWithValue("@PlayerId", playerId);
                using MySqlConnector.MySqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    int nPosition = dr.GetInt32("Position");
                    humanRcd.Data.StorageItems[nPosition] = new ServerUserItem();
                    humanRcd.Data.StorageItems[nPosition].MakeIndex = dr.GetInt32("MakeIndex");
                    humanRcd.Data.StorageItems[nPosition].Index = dr.GetUInt16("StdIndex");
                    humanRcd.Data.StorageItems[nPosition].Dura = dr.GetUInt16("Dura");
                    humanRcd.Data.StorageItems[nPosition].DuraMax = dr.GetUInt16("DuraMax");
                }
                dr.Close();
                dr.Dispose();
                QueryItemAttr(context, playerId, ref humanRcd.Data.StorageItems);
            }
            catch (Exception ex)
            {
                LogService.Error("[Exception] PlayDataStorage.GetStorageRecord");
                LogService.Error(ex.StackTrace);
            }
        }

        private void GetPlayerStatus(int playerId, StorageContext context, ref CharacterDataInfo humanRcd)
        {
            const string sSqlString = "SELECT * FROM characters_status WHERE PlayerId=@PlayerId";
            try
            {
                MySqlConnector.MySqlCommand command = context.CreateCommand();
                command.CommandText = sSqlString;
                command.Parameters.AddWithValue("@PlayerId", playerId);
                using MySqlConnector.MySqlDataReader dr = command.ExecuteReader();
                if (dr.Read())
                {
                    for (int i = 0; i < 15; i++)
                    {
                        humanRcd.Data.StatusTimeArr[i] = dr.GetUInt16($"Status{i}");
                    }
                }
                dr.Close();
                dr.Dispose();
            }
            catch
            {
                LogService.Error("[Exception] PlayDataStorage.GetPlayerStatus");
            }
        }

        public int Find(string sChrName, StringDictionary list)
        {
            for (int i = 0; i < _NameQuickMap.Count; i++)
            {
                //if (HUtil32.CompareLStr(m_MirQuickList[i], sChrName, sChrName.Length))
                //{
                //    List.Add(m_MirQuickList[i], m_MirQuickList.Values[i]);
                //}
            }
            return list.Count;
        }

        public bool Delete(int nIndex)
        {
            for (int i = 0; i < _NameQuickMap.Count; i++)
            {
                //if (((int)m_MirQuickList.Values[i]) == nIndex)
                //{
                //    s14 = m_MirQuickList[i];
                //    if (DeleteRecord(nIndex))
                //    {
                //        m_MirQuickList.Remove(i);
                //        result = true;
                //        break;
                //    }
                //}
            }
            return false;
        }

        private bool DeleteRecord(int nIndex)
        {
            const string sqlString = "UPDATE characters SET DELETED=1, LastUpdate=now() WHERE ID=@Id";
            bool result = true;
            int playerId = _IndexQuickIdMap[nIndex];
            using StorageContext context = new StorageContext(_storageOption);
            bool success = false;
            context.Open(ref success);
            if (!success)
            {
                return false;
            }
            try
            {
                MySqlConnector.MySqlCommand command = context.CreateCommand();
                command.CommandText = sqlString;
                command.Parameters.AddWithValue("@Id", playerId);
                command.ExecuteNonQuery();
            }
            catch
            {
                result = false;
                LogService.Error("[Exception] PlayDataStorage.DeleteRecord");
            }
            finally
            {
                context.Dispose();
            }
            return result;
        }

        public int Count()
        {
            return _NameQuickMap.Count;
        }

        public bool Delete(string sChrName)
        {
            //int nIndex = m_MirQuickList.GetIndex(sChrName);
            //if (nIndex < 0)
            //{
            //    return result;
            //}
            //if (DeleteRecord(nIndex))
            //{
            //    m_MirQuickList.Remove(nIndex);
            //    result = true;
            //}
            return false;
        }

        public bool GetQryChar(int nIndex, ref QueryChr queryChrRcd)
        {
            const string sSql = "SELECT * FROM characters WHERE ID=@Id";
            if (nIndex < 0)
            {
                return false;
            }
            using StorageContext context = new StorageContext(_storageOption);
            bool success = false;
            context.Open(ref success);
            if (!success)
            {
                return false;
            }
            try
            {
                MySqlConnector.MySqlCommand command = context.CreateCommand();
                command.CommandText = sSql;
                command.Parameters.AddWithValue("@Id", nIndex);
                using MySqlConnector.MySqlDataReader dr = command.ExecuteReader();
                if (dr.Read())
                {
                    queryChrRcd = new QueryChr();
                    queryChrRcd.Name = dr.GetString("ChrName");
                    queryChrRcd.Job = dr.GetByte("Job");
                    queryChrRcd.Hair = dr.GetByte("Hair");
                    queryChrRcd.Sex = dr.GetByte("Sex");
                    queryChrRcd.Level = dr.GetUInt16("Level");
                }
                dr.Close();
                dr.Dispose();
            }
            catch (Exception)
            {
                LogService.Error("[Exception] PlayDataStorage.GetQryChar");
                return false;
            }
            finally
            {
                context.Dispose();
            }
            return true;
        }

        private void QueryItemAttr(StorageContext context, int playerId, ref ServerUserItem[] userItems)
        {
            List<int> makeIndexs = userItems.Where(x => x != null && x.MakeIndex > 0).Select(x => x.MakeIndex).ToList();
            if (!makeIndexs.Any())
            {
                return;
            }
            const string sSqlString = "SELECT * FROM characters_item_attr WHERE PlayerId=@PlayerId and MakeIndex in (@MakeIndex)";
            try
            {
                MySqlConnector.MySqlCommand command = context.CreateCommand();
                command.CommandText = sSqlString;
                command.Parameters.AddWithValue("@PlayerId", playerId);
                command.Parameters.AddWithValue("@MakeIndex", string.Join(",", makeIndexs));
                using MySqlConnector.MySqlDataReader dr = command.ExecuteReader();
                int nPosition = 0;
                while (dr.Read())
                {
                    userItems[nPosition].Desc[nPosition] = dr.GetByte($"VALUE{nPosition}");
                    nPosition++;
                }
                dr.Close();
                dr.Dispose();
            }
            catch (Exception ex)
            {
                LogService.Error("[Exception] PlayDataStorage.QueryItemAttr:" + ex.StackTrace);
            }
        }
    }
}