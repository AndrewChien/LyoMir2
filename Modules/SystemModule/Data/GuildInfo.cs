﻿using SystemModule.Actors;

namespace SystemModule.Data
{
    public class GuildRank
    {
        public short RankNo;
        public string RankName;
        public IList<GuildMember> MemberList;
    }

    public record struct GuildMember
    {
        public string MemberName;
        public IPlayerActor PlayObject;
    }
}
