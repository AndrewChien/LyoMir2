﻿using OpenMir2;

namespace M2Server.Monster.Monsters
{
    public class RunAway : MonsterObject
    {
        public RunAway() : base()
        {
            SearchTime = M2Share.RandomNumber.Random(1500) + 1500;
        }

        public override void Run()
        {
            int time1 = 0;
            short nx = 0;
            short ny = 0;
            bool borunaway = false;
            if (!Death && !Ghost)
            {
                if (TargetCret != null)
                {
                    TargetX = TargetCret.CurrX;
                    TargetY = TargetCret.CurrY;
                    if (WAbil.HP <= HUtil32.Round(WAbil.MaxHP / 2.0))
                    {
                        GetFrontPosition(ref nx, ref ny);
                        SendRefMsg(Messages.RM_SPACEMOVE_FIRE, 0, 0, 0, 0, "");
                        SpaceMove(MapName, (short)(nx - 2), (short)(ny - 2), 0);
                        borunaway = true;
                    }
                    else
                    {
                        if (WAbil.HP >= HUtil32.Round(WAbil.MaxHP / 2.0))
                        {
                            borunaway = false;
                        }
                    }
                    if (borunaway)
                    {
                        if ((HUtil32.GetTickCount() - time1) > 5000)
                        {
                            if (Math.Abs(TargetX - CurrX) == 1 && Math.Abs(TargetY - CurrY) == 1)
                            {
                                WalkTo(M2Share.RandomNumber.RandomByte(4), true, BoFearFire);
                            }
                            else
                            {
                                WalkTo(M2Share.RandomNumber.RandomByte(7), true, BoFearFire);
                            }
                        }
                        else
                        {
                            time1 = HUtil32.GetTickCount();
                        }
                    }
                }
                if ((HUtil32.GetTickCount() - SearchEnemyTick) > 8000 || (HUtil32.GetTickCount() - SearchEnemyTick) > 1000 && TargetCret == null)
                {
                    SearchEnemyTick = HUtil32.GetTickCount();
                    SearchTarget();
                }
            }
            base.Run();
        }
    }
}