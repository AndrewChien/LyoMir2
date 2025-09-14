﻿using BotSrv.Player;

namespace BotSrv.Objects
{
    public class TCastleDoor : Actor
    {
        public TCastleDoor(RobotPlayer robotClient) : base(robotClient)
        {
            m_btDir = 0;
            m_nDownDrawLevel = 1;
        }

        private void ApplyDoorState()
        {
            //bool bowalk;
            //ClMain.Map.MarkCanWalk(this.CurrX, this.CurrY - 2, true);
            //ClMain.Map.MarkCanWalk(this.CurrX + 1, this.CurrY - 1, true);
            //ClMain.Map.MarkCanWalk(this.CurrX + 1, this.CurrY - 2, true);
            //if (dstate == TDoorState.dsClose)
            //{
            //    bowalk = false;
            //}
            //else
            //{
            //    bowalk = true;
            //}
            //ClMain.Map.MarkCanWalk(this.CurrX, this.CurrY, bowalk);
            //ClMain.Map.MarkCanWalk(this.CurrX, this.CurrY - 1, bowalk);
            //ClMain.Map.MarkCanWalk(this.CurrX, this.CurrY - 2, bowalk);
            //ClMain.Map.MarkCanWalk(this.CurrX + 1, this.CurrY - 1, bowalk);
            //ClMain.Map.MarkCanWalk(this.CurrX + 1, this.CurrY - 2, bowalk);
            //ClMain.Map.MarkCanWalk(this.CurrX - 1, this.CurrY - 1, bowalk);
            //ClMain.Map.MarkCanWalk(this.CurrX - 1, this.CurrY, bowalk);
            //ClMain.Map.MarkCanWalk(this.CurrX - 1, this.CurrY + 1, bowalk);
            //ClMain.Map.MarkCanWalk(this.CurrX - 2, this.CurrY, bowalk);
            //if (dstate == TDoorState.dsOpen)
            //{
            //    ClMain.Map.MarkCanWalk(this.CurrX, this.CurrY - 2, false);
            //    ClMain.Map.MarkCanWalk(this.CurrX + 1, this.CurrY - 1, false);
            //    ClMain.Map.MarkCanWalk(this.CurrX + 1, this.CurrY - 2, false);
            //}
        }

        public override void Run()
        {
            //if ((ClMain.Map.m_nCurUnitX != oldunitx) || (ClMain.Map.m_nCurUnitY != oldunity))
            //{
            //    if (this.m_boDeath)
            //    {
            //        ApplyDoorState(TDoorState.dsBroken);
            //    }
            //    else if (BoDoorOpen)
            //    {
            //        ApplyDoorState(TDoorState.dsOpen);
            //    }
            //    else
            //    {
            //        ApplyDoorState(TDoorState.dsClose);
            //    }
            //}
            //oldunitx = ClMain.Map.m_nCurUnitX;
            //oldunity = ClMain.Map.m_nCurUnitY;
            base.Run();
        }
    }
}