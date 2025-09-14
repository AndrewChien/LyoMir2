﻿using M2Server.Actor;
using OpenMir2;
using OpenMir2.Data;
using SystemModule;
using SystemModule.Actors;

namespace M2Server.Monster.Monsters
{
    public class SpiderHouseMonster : AnimalObject
    {
        private readonly IList<IActor> _bbList;

        public SpiderHouseMonster() : base()
        {
            ViewRange = 9;
            RunTime = 250;
            SearchTime = M2Share.RandomNumber.Random(1500) + 2500;
            SearchTick = 0;
            StickMode = true;
            _bbList = new List<IActor>();
        }

        private void GenBb()
        {
            if (_bbList.Count < 15)
            {
                SendRefMsg(Messages.RM_HIT, Dir, CurrX, CurrY, 0, "");
                SendSelfDelayMsg(Messages.RM_ZEN_BEE, 0, 0, 0, 0, "", 500);
            }
        }

        protected override bool Operate(ProcessMessage processMsg)
        {
            if (processMsg.wIdent == Messages.RM_ZEN_BEE)
            {
                short n08 = CurrX;
                short n0C = (short)(CurrY + 1);
                if (Envir.CanWalk(n08, n0C, true))
                {
                    IActor bb = SystemShare.WorldEngine.RegenMonsterByName(Envir.MapName, n08, n0C, SystemShare.Config.Spider);
                    if (bb != null)
                    {
                        bb.SetTargetCreat(TargetCret);
                        _bbList.Add(bb);
                    }
                }
            }
            return base.Operate(processMsg);
        }

        public override void Run()
        {
            if (CanMove())
            {
                if ((HUtil32.GetTickCount() - WalkTick) >= WalkSpeed)
                {
                    WalkTick = HUtil32.GetTickCount();
                    if ((HUtil32.GetTickCount() - AttackTick) >= NextHitTime)
                    {
                        AttackTick = HUtil32.GetTickCount();
                        SearchTarget();
                        if (TargetCret != null)
                        {
                            GenBb();
                        }
                    }
                    for (int i = _bbList.Count - 1; i >= 0; i--)
                    {
                        IActor bb = _bbList[i];
                        if (bb.Death || bb.Ghost)
                        {
                            _bbList.RemoveAt(i);
                        }
                    }
                }
            }
            base.Run();
        }
    }
}