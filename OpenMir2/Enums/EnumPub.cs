using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMir2.Enums
{
    public static class EnumPub<T>
    {
        public static T Parse(int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }
    }
}
