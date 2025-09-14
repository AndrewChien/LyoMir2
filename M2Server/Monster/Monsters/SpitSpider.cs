﻿using OpenMir2;
using OpenMir2.Consts;
using SystemModule;
using SystemModule.Actors;

namespace M2Server.Monster.Monsters
{
    public class SpitSpider : AtMonster
    {
        public bool UsePoison;

        public SpitSpider() : base()
        {
            SearchTime = M2Share.RandomNumber.Random(1500) + 1500;
            Animal = true;
            UsePoison = true;
        }

        private void SpitAttack(byte btDir)
        {
            Dir = btDir;
            int nDamage = HUtil32.LoByte(WAbil.DC) + M2Share.RandomNumber.Random(Math.Abs(HUtil32.HiByte(WAbil.DC) - HUtil32.LoByte(WAbil.DC)) + 1);
            if (nDamage <= 0)
            {
                return;
            }
            SendRefMsg(Messages.RM_HIT, Dir, CurrX, CurrY, 0, "");
            for (int i = 0; i < 5; i++)
            {
                for (int k = 0; k < 5; k++)
                {
                    if (SystemShare.Config.SpitMap[btDir, i, k] == 1)
                    {
                        short nX = (short)(CurrX - 2 + k);
                        short nY = (short)(CurrY - 2 + i);
                        IActor baseObject = Envir.GetMovingObject(nX, nY, true);
                        if (baseObject != null && baseObject != this && IsProperTarget(baseObject) && M2Share.RandomNumber.Random(baseObject.SpeedPoint) < HitPoint)
                        {
                            nDamage = baseObject.GetMagStruckDamage(this, nDamage);
                            if (nDamage > 0)
                            {
                                baseObject.StruckDamage(nDamage);
                                baseObject.SendStruckDelayMsg(Messages.RM_REFMESSAGE, nDamage, WAbil.HP, WAbil.MaxHP, ActorId, "", 300);
                                if (UsePoison)
                                {
                                    if (M2Share.RandomNumber.Random(AntiPoison + 20) == 0)
                                    {
                                        baseObject.MakePosion(PoisonState.DECHEALTH, 30, 1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override bool AttackTarget()
        {
            byte btDir = 0;
            if (TargetCret == null)
            {
                return false;
            }
            if (TargetInSpitRange(TargetCret, ref btDir))
            {
                if ((HUtil32.GetTickCount() - AttackTick) > NextHitTime)
                {
                    AttackTick = HUtil32.GetTickCount();
                    TargetFocusTick = HUtil32.GetTickCount();
                    SpitAttack(btDir);
                    BreakHolySeizeMode();
                }
                return true;
            }
            if (TargetCret.Envir == Envir)
            {
                SetTargetXy(TargetCret.CurrX, TargetCret.CurrY);
            }
            else
            {
                DelTargetCreat();
            }
            return false;
        }
    }
}

