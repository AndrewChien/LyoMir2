﻿using OpenMir2;
using OpenMir2.Consts;
using SystemModule.Actors;

namespace M2Server.Monster.Monsters
{
    public class GasMothMonster : GasAttackMonster
    {
        public GasMothMonster() : base()
        {
            ViewRange = 7;
        }

        protected override IActor GasAttack(byte bt05)
        {
            IActor baseObject = base.GasAttack(bt05);
            if (baseObject != null && M2Share.RandomNumber.Random(3) == 0 && baseObject.HideMode)
            {
                baseObject.StatusTimeArr[PoisonState.STATETRANSPARENT] = 1;
            }
            return baseObject;
        }

        public override void Run()
        {
            if (CanMove() && (HUtil32.GetTickCount() - WalkTick) >= WalkSpeed)
            {
                if ((HUtil32.GetTickCount() - SearchEnemyTick) > 8000 || (HUtil32.GetTickCount() - SearchEnemyTick) > 1000 && TargetCret == null)
                {
                    SearchEnemyTick = HUtil32.GetTickCount();
                    GasMothAttack();
                }
            }
            base.Run();
        }

        private void GasMothAttack()
        {
            IActor Creat = null;
            int n10 = 999;
            for (int i = 0; i < this.VisibleActors.Count; i++)
            {
                IActor BaseObject = this.VisibleActors[i].BaseObject;
                if (BaseObject.Death)
                {
                    continue;
                }
                if (!this.IsProperTarget(BaseObject))
                {
                    continue;
                }

                int nC = Math.Abs(this.CurrX - BaseObject.CurrX) + Math.Abs(this.CurrY - BaseObject.CurrY);
                if (nC >= n10)
                {
                    continue;
                }

                n10 = nC;
                Creat = BaseObject;
            }
            if (Creat != null)
            {
                this.SetTargetCreat(Creat);
            }
        }
    }
}