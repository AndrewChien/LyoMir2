﻿using BotSrv.Player;

namespace BotSrv.Objects;

public class TZombiDigOut : TSkeletonOma
{
    public TZombiDigOut(RobotPlayer robotClient) : base(robotClient)
    {
    }

    public override void RunFrameAction(int frame)
    {
        //TClEvent clEvent;
        //if (this.m_nCurrentAction == Messages.SM_DIGUP)
        //{
        //    if (frame == 6)
        //    {
        //        clEvent = new TClEvent(this.m_nCurrentEvent, this.CurrX, this.CurrY, Grobal2.ET_DIGOUTZOMBI);
        //        clEvent.m_nDir = this.m_btDir;
        //        ClMain.EventMan.AddEvent(clEvent);
        //    }
        //}
    }
}