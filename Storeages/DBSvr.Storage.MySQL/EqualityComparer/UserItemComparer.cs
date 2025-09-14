using OpenMir2.Packets.ClientPackets;
using System.Collections.Generic;

namespace DBSrv.Storage.MySQL.EqualityComparer
{
    public class UserItemComparer : IEqualityComparer<UserItem>
    {
        public bool Equals(UserItem x, UserItem y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null)
            {
                return false;
            }

            if (y is null)
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            return x.MakeIndex == y.MakeIndex || x.Index == y.Index;
        }

        public int GetHashCode(UserItem obj)
        {
            unchecked
            {
                int hashCode = obj.MakeIndex;
                hashCode = (hashCode * 397) ^ (obj.Index != 0 ? obj.Index.GetHashCode() : 0);
                return hashCode;
            }

            //unchecked
            //{
            //    var hashCode = (obj.MakeIndex != null ? obj.MakeIndex.GetHashCode() : 0);
            //    hashCode = (hashCode * 397) ^ (obj.Index != null ? obj.Index.GetHashCode() : 0);
            //    hashCode = (hashCode * 397) ^ (obj.Dura != null ? obj.Dura.GetHashCode() : 0);
            //    hashCode = (hashCode * 397) ^ (obj.DuraMax != null ? obj.DuraMax.GetHashCode() : 0);
            //    hashCode = (hashCode * 397) ^ (obj.Desc != null ? obj.Desc.GetHashCode() : 0);
            //    hashCode = (hashCode * 397) ^ (obj.ColorR != null ? obj.ColorR.GetHashCode() : 0);
            //    hashCode = (hashCode * 397) ^ (obj.ColorG != null ? obj.ColorG.GetHashCode() : 0);
            //    hashCode = (hashCode * 397) ^ (obj.ColorB != null ? obj.ColorB.GetHashCode() : 0);
            //    hashCode = (hashCode * 397) ^ (obj.Prefix != null ? obj.Prefix.GetHashCode() : 0);
            //    return hashCode;
            //}
        }
    }
}