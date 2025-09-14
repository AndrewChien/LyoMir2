using OpenMir2;
using SystemModule;
using SystemModule.Actors;

namespace M2Server.Monster.Monsters
{
    public class DoubleCriticalMonster : AtMonster
    {
        public DoubleCriticalMonster() : base()
        {
            Animal = false;
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
                    DoubleAttack(btDir);
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

        private void DoubleAttack(byte btDir)
        {
            Dir = btDir;
            int nDamage = HUtil32.LoByte(WAbil.DC) + M2Share.RandomNumber.Random(Math.Abs(HUtil32.HiByte(WAbil.DC) - HUtil32.LoByte(WAbil.DC)) + 1);
            if (nDamage <= 0)
            {
                return;
            }
            SendRefMsg(Messages.RM_HIT, Dir, CurrX, CurrY, 0, "");
            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < 4; k++)
                {
                    if (SystemShare.Config.SpitMap[btDir, i, k] == 1)
                    {
                        short nX = (short)(CurrX - 2 + k);
                        short nY = (short)(CurrY - 2 + i);
                        IActor baseObject = Envir.GetMovingObject(nX, nY, true);
                        if (baseObject != null && baseObject != this && IsProperTarget(baseObject) && M2Share.RandomNumber.Random(baseObject.SpeedPoint) < HitPoint)
                        {
                            nDamage = baseObject.GetHitStruckDamage(this, nDamage);
                            if (nDamage > 0)
                            {
                                baseObject.StruckDamage(nDamage);
                                baseObject.SendStruckDelayMsg(Messages.RM_REFMESSAGE, nDamage, WAbil.HP, WAbil.MaxHP, ActorId, "", 300);
                            }
                        }
                    }
                }
            }
        }
    }
}