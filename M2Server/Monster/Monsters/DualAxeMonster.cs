﻿using OpenMir2;
using SystemModule.Actors;

namespace M2Server.Monster.Monsters
{
    public class DualAxeMonster : MonsterObject
    {
        private int _mNAttackCount;
        /// <summary>
        /// 最大攻击目标数量
        /// </summary>
        protected int AttackMax;

        private void FlyAxeAttack(IActor target)
        {
            if (Envir.CanFly(CurrX, CurrY, target.CurrX, target.CurrY))
            {
                Dir = M2Share.GetNextDirection(CurrX, CurrY, target.CurrX, target.CurrY);
                int nDamage = HUtil32.LoByte(WAbil.DC) + M2Share.RandomNumber.Random(Math.Abs(HUtil32.HiByte(WAbil.DC) - HUtil32.LoByte(WAbil.DC)) + 1);
                if (nDamage > 0)
                {
                    nDamage = target.GetHitStruckDamage(this, nDamage);
                }
                if (nDamage > 0)
                {
                    target.StruckDamage(nDamage);
                    target.SendStruckDelayMsg(Messages.RM_REFMESSAGE, nDamage, target.WAbil.HP, target.WAbil.MaxHP, ActorId, "", HUtil32._MAX(Math.Abs(CurrX - target.CurrX), Math.Abs(CurrY - target.CurrY)) * 50 + 600);
                }
                SendRefMsg(Messages.RM_FLYAXE, Dir, CurrX, CurrY, target.ActorId, "");
            }
        }

        protected override bool AttackTarget()
        {
            if (TargetCret == null)
            {
                return false;
            }
            if ((HUtil32.GetTickCount() - AttackTick) > NextHitTime)
            {
                AttackTick = HUtil32.GetTickCount();
                if (Math.Abs(CurrX - TargetCret.CurrX) <= 7 && Math.Abs(CurrX - TargetCret.CurrX) <= 7)
                {
                    if (AttackMax - 1 > _mNAttackCount)
                    {
                        _mNAttackCount++;
                        TargetFocusTick = HUtil32.GetTickCount();
                        FlyAxeAttack(TargetCret);
                    }
                    else
                    {
                        if (M2Share.RandomNumber.Random(5) == 0)
                        {
                            _mNAttackCount = 0;
                        }
                    }
                    return true;
                }
                if (TargetCret.Envir == Envir)
                {
                    if (Math.Abs(CurrX - TargetCret.CurrX) <= 11 && Math.Abs(CurrX - TargetCret.CurrX) <= 11)
                    {
                        SetTargetXy(TargetCret.CurrX, TargetCret.CurrY);
                    }
                }
                else
                {
                    DelTargetCreat();
                }
            }
            return false;
        }

        public DualAxeMonster() : base()
        {
            ViewRange = 5;
            RunTime = 250;
            SearchTime = 3000;
            _mNAttackCount = 0;
            AttackMax = 2;
            SearchTick = HUtil32.GetTickCount();
        }

        public override void Run()
        {
            int nRage = 9999;
            IActor targetBaseObject = null;
            if (CanMove())
            {
                if ((HUtil32.GetTickCount() - SearchEnemyTick) >= 5000)
                {
                    SearchEnemyTick = HUtil32.GetTickCount();
                    for (int i = 0; i < VisibleActors.Count; i++)
                    {
                        IActor baseObject = VisibleActors[i].BaseObject;
                        if (baseObject.Death)
                        {
                            continue;
                        }
                        if (IsProperTarget(baseObject))
                        {
                            if (!baseObject.HideMode || CoolEye)
                            {
                                int nAbs = Math.Abs(CurrX - baseObject.CurrX) + Math.Abs(CurrY - baseObject.CurrY);
                                if (nAbs < nRage)
                                {
                                    nRage = nAbs;
                                    targetBaseObject = baseObject;
                                }
                            }
                        }
                    }
                    if (targetBaseObject != null)
                    {
                        SetTargetCreat(targetBaseObject);
                    }
                }
                if ((HUtil32.GetTickCount() - WalkTick) > WalkSpeed && TargetCret != null)
                {
                    if (Math.Abs(CurrX - TargetCret.CurrX) <= 4 && Math.Abs(CurrX - TargetCret.CurrX) <= 4)
                    {
                        if (Math.Abs(CurrX - TargetCret.CurrX) <= 2 && Math.Abs(CurrX - TargetCret.CurrX) <= 2)
                        {
                            if (M2Share.RandomNumber.Random(5) == 0)
                            {
                                GetBackPosition(ref TargetX, ref TargetY);
                            }
                        }
                        else
                        {
                            GetBackPosition(ref TargetX, ref TargetY);
                        }
                    }
                }
            }
            base.Run();
        }
    }
}