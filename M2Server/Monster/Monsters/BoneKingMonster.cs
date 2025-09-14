﻿using OpenMir2;
using SystemModule;
using SystemModule.Actors;

namespace M2Server.Monster.Monsters
{
    public class BoneKingMonster : MonsterObject
    {
        private short DangerLevel;
        private readonly IList<IActor> SlaveObjectList;

        public BoneKingMonster() : base()
        {
            SearchTime = M2Share.RandomNumber.Random(1500) + 1500;
            ViewRange = 8;
            Dir = 5;
            DangerLevel = 5;
            SlaveObjectList = new List<IActor>();
        }

        private void CallSlave()
        {
            string[] sMonName = { "BoneCaptain", "BoneArcher", "BoneSpearman" };
            short n10 = 0;
            short n14 = 0;
            int nC = M2Share.RandomNumber.Random(6) + 6;
            GetFrontPosition(ref n10, ref n14);
            for (int i = 0; i < nC; i++)
            {
                if (SlaveObjectList.Count >= 30)
                {
                    break;
                }
                IActor baseObject = SystemShare.WorldEngine.RegenMonsterByName(MapName, n10, n14, sMonName[M2Share.RandomNumber.Random(3)]);
                if (baseObject != null)
                {
                    SlaveObjectList.Add(baseObject);
                }
            }
        }

        protected override void Attack(IActor targetBaseObject, byte nDir)
        {
            int nPower = GetAttackPower(HUtil32.LoByte(WAbil.DC), Math.Abs(HUtil32.HiByte(WAbil.DC) - HUtil32.LoByte(WAbil.DC)));
            HitMagAttackTarget(targetBaseObject, 0, nPower, true);
        }

        public override void Run()
        {
            if (CanMove() && HUtil32.GetTickCount() - WalkTick >= WalkSpeed)
            {
                if ((HUtil32.GetTickCount() - SearchEnemyTick) > 8000 || (HUtil32.GetTickCount() - SearchEnemyTick) > 1000 && TargetCret == null)
                {
                    SearchEnemyTick = HUtil32.GetTickCount();
                    SearchTarget();
                    if (DangerLevel > WAbil.HP / WAbil.MaxHP * 5 && DangerLevel > 0)
                    {
                        DangerLevel -= 1;
                        CallSlave();
                    }
                    if (WAbil.HP == WAbil.MaxHP)
                    {
                        DangerLevel = 5;
                    }
                }
                for (int i = SlaveObjectList.Count - 1; i >= 0; i--)
                {
                    IActor baseObject = SlaveObjectList[i];
                    if (baseObject.Death || baseObject.Ghost)
                    {
                        SlaveObjectList.RemoveAt(i);
                    }
                }
            }
            base.Run();
        }
    }
}

