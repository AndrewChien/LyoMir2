﻿using OpenMir2;
using OpenMir2.Enums;
using System.Collections.Concurrent;
using SystemModule.Actors;
using SystemModule.Data;

namespace SystemModule
{
    /// <summary>
    /// 对象管理
    /// </summary>
    public sealed class ActorMgr
    {
        /// <summary>
        /// 精灵生成器队列
        /// </summary>
        private readonly ConcurrentQueue<int> _generateQueue = new ConcurrentQueue<int>();
        private readonly IList<int> ActorIds = new List<int>(1000);
        /// <summary>
        /// 精灵列表
        /// </summary>
        private readonly ConcurrentDictionary<int, IActor> _actorsMap = new ConcurrentDictionary<int, IActor>();
        /// <summary>
        /// 其他对象
        /// </summary>
        private readonly ConcurrentDictionary<int, object> _ohterMap = new ConcurrentDictionary<int, object>();
        private static int PlayerCount { get; set; }
        private static int NpcCount { get; set; }
        private static int MonsterCount { get; set; }
        private static int MonsterDeathCount { get; set; }
        private static int MonsterDisposeCount { get; set; }
        private static int PlayerGhostCount { get; set; }

        public int GetNextIdentity()
        {
            return _generateQueue.TryDequeue(out int sequence) ? sequence : HUtil32.Sequence();
        }

        public int GenerateQueueCount => _generateQueue.Count;

        public void AddToQueue(int sequence)
        {
            _generateQueue.Enqueue(sequence);
        }

        public bool ContainsKey(int actorId)
        {
            return _actorsMap.ContainsKey(actorId);
        }

        public void Add(IActor actor)
        {
            _actorsMap.TryAdd(actor.ActorId, actor);
        }

        public IActor Get(int actorId)
        {
            return _actorsMap.TryGetValue(actorId, out IActor actor) ? actor : null;
        }

        public T Get<T>(int actorId) where T : IActor
        {
            return _actorsMap.TryGetValue(actorId, out IActor actor) ? (T)actor : default;
        }

        private void SendMessage(int actorId, SendMessage sendMessage)
        {
            if (_actorsMap.TryGetValue(actorId, out IActor actor))
            {
                actor.AddMessage(sendMessage);
            }
        }

        public void SendMessage(int actorId, int wIdent, int wParam, int nParam1, int nParam2, int nParam3, string sMsg)
        {
            SendMessage sendMessage = new SendMessage
            {
                wIdent = wIdent,
                wParam = wParam,
                nParam1 = nParam1,
                nParam2 = nParam2,
                nParam3 = nParam3,
                DeliveryTime = 0,
                ActorId = actorId,
                LateDelivery = false,
                Buff = sMsg
            };
            SendMessage(actorId, sendMessage);
        }

        public void AddOhter(int objectId, object obj)
        {
            _ohterMap.TryAdd(objectId, obj);
        }

        public object GetOhter(int objectId)
        {
            return _ohterMap.TryGetValue(objectId, out object obj) ? obj : null;
        }

        public void RevomeOhter(int actorId)
        {
            _ohterMap.TryRemove(actorId, out object actor);
        }

        /// <summary>
        /// 清理无效或者死亡对象
        /// </summary>
        public void CleanObject()
        {
            foreach (int actorId in ActorIds)
            {
                if (_actorsMap.TryRemove(actorId, out IActor actor))
                {
                    if (actor.Race != ActorRace.Play)
                    {
                        MonsterDisposeCount++;
                    }
                    // actor.Dispose();
                }
            }
            ActorIds.Clear();
        }

        /// <summary>
        /// 统计对象数量
        /// </summary>
        public string Analytics()
        {
            ActorIds.Clear();
            PlayerCount = 0;
            MonsterCount = 0;
            NpcCount = 0;
            using IEnumerator<KeyValuePair<int, IActor>> actors = _actorsMap.GetEnumerator();
            while (actors.MoveNext())
            {
                IActor actor = actors.Current.Value;
                if (!actor.Death && !actor.Ghost)
                {
                    if (actor.Race == ActorRace.Play)
                    {
                        PlayerCount++;
                        PlayerGhostCount++;
                    }
                    else if (actor.Race == ActorRace.NPC || actor.Race == ActorRace.Merchant)
                    {
                        NpcCount++;
                    }
                    else
                    {
                        MonsterCount++;
                    }
                }
                else if (actor.Race != ActorRace.Play && actor.Death || actor.Ghost)
                {
                    MonsterDeathCount++;
                }
                if (!actor.Ghost || actor.GhostTick <= 0)
                {
                    continue;
                }

                if ((HUtil32.GetTickCount() - actor.GhostTick) <= 20000)//死亡对象清理时间
                {
                    continue;
                }
                ActorIds.Add(actors.Current.Key);
            }
            return $"对象数:[{_actorsMap.Count}] 玩家/NPC/怪物:[{PlayerCount}/{NpcCount}/{MonsterCount}] 死亡角色:[{PlayerGhostCount}] 死亡怪物:[{MonsterDeathCount}] 释放怪物:[{MonsterDisposeCount}]";
        }
    }
}