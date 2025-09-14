﻿using M2Server.Actor;
using OpenMir2;
using OpenMir2.Enums;
using SystemModule;

namespace M2Server.Monster.Monsters
{
    /// <summary>
    /// 神兽普通跟随形态
    /// </summary>
    public class ElfMonster : MonsterObject
    {
        public bool BoIsFirst;

        public ElfMonster() : base()
        {
            ViewRange = 6;
            FixedHideMode = true;
            NoAttackMode = true;
            BoIsFirst = true;
            Race = ActorRace.ElfMonster;
        }

        public void AppearNow()
        {
            FixedHideMode = false;
            RecalcAbilitys();
            WalkTick = WalkTick + 800;
        }

        public override void RecalcAbilitys()
        {
            base.RecalcAbilitys();
            ResetElfMon();
        }

        private void ResetElfMon()
        {
            WalkSpeed = 500 - SlaveMakeLevel * 50;
            WalkTick = HUtil32.GetTickCount() + 2000;
        }

        public override void Run()
        {
            bool boChangeFace = false;
            if (BoIsFirst)
            {
                BoIsFirst = false;
                FixedHideMode = false;
                SendRefMsg(Messages.RM_DIGUP, Dir, CurrX, CurrY, 0, "");
                ResetElfMon();
            }
            if (Death)
            {
                if ((HUtil32.GetTickCount() - DeathTick) > (2 * 1000))
                {
                    MakeGhost();
                }
            }
            else
            {
                if (TargetCret != null)
                {
                    boChangeFace = true;
                }
                if (Master != null && (Master.TargetCret != null || Master.LastHiter != null))
                {
                    boChangeFace = true;
                }
                if (boChangeFace)
                {
                    BaseObject elfMon = MakeClone(SystemShare.Config.Dragon1, this);
                    if (elfMon != null)
                    {
                        elfMon.AutoChangeColor = AutoChangeColor;
                        if (elfMon is ElfWarriorMonster monster)
                        {
                            monster.AppearNow();
                        }
                        Master = null;
                        KickException();
                    }
                }
            }
            base.Run();
        }
    }
}

