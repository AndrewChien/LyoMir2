﻿using OpenMir2.Data;
using SystemModule.Data;

namespace SystemModule.SubSystem
{
    public interface IFrontEngine
    {
        IList<SavePlayerRcd> GetSaveRcdList();

        IList<SavePlayerRcd> GetTempSaveRcdList();

        IList<LoadDBInfo> GetLoadTempList();

        void ClearSaveList();

        void ClearSaveRcdTempList();

        void ClearLoadRcdTempList();

        void ClearLoadList();

        void AddChangeGoldList(string sGameMasterName, string sGetGoldUserName, int nGold);

        void AddToLoadRcdList(LoadDBInfo loadRcdInfo);

        /// <summary>
        /// 添加到保存队列
        /// </summary>
        /// <param name="SaveRcd"></param>
        void AddToSaveRcdList(SavePlayerRcd SaveRcd);

        void DeleteHuman(int nGateIndex, int nSocket);

        /// <summary>
        /// 是否在保存队列
        /// </summary>
        /// <param name="sChrName"></param>
        /// <returns></returns>
        bool InSaveRcdList(string sChrName);

        bool IsFull();

        bool IsIdle();

        void ProcessGameDate();

        void RemoveSaveList(int queryId);

        int SaveListCount();
    }
}
