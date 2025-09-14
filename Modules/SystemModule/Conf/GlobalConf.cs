using OpenMir2.Common;

namespace SystemModule.Conf
{
    /// <summary>
    /// global.conf
    /// </summary>
    public class GlobalConf : ConfigFile
    {
        /// <summary>
        /// global.conf
        /// </summary>
        public GlobalConf(string fileName) : base(fileName)
        {
            Load();
        }
        /// <summary>
        /// º”‘ÿglobal.conf
        /// </summary>
        public void LoadConfig()
        {
            for (int i = 0; i < SystemShare.Config.GlobalVal.Length; i++)
            {
                SystemShare.Config.GlobalVal[i] = ReadWriteInteger("Integer", "GlobalVal" + i, SystemShare.Config.GlobalVal[i]);
            }
            for (int i = 0; i < SystemShare.Config.GlobalAVal.Length; i++)
            {
                SystemShare.Config.GlobalAVal[i] = ReadWriteString("String", "GlobalStrVal" + i, SystemShare.Config.GlobalAVal[i]);
            }
        }
    }
}