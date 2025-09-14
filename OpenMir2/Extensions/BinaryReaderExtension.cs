using System.IO;

namespace OpenMir2.Extensions
{
    public static class BinaryReaderExtension
    {
        public static string ReadString(this BinaryReader binaryReader, int size)
        {
            return new string(binaryReader.ReadChars(size));
        }

        public static byte[] ReadDeCodeBytes(this BinaryReader binaryReader, int size)
        {
            int buffLen = 0;
            byte[] data = binaryReader.ReadBytes(size);
            return EncryptUtil.Decode(data, data.Length, ref buffLen);
        }

        public static string ReadPascalString(this BinaryReader binaryReader, int size)
        {
            try   //添加防错
            {
                byte packegeLen = binaryReader.ReadByte();
                if (size < packegeLen)
                {
                    size = packegeLen;
                }
                byte[] strbuff = binaryReader.ReadBytes(size);
                ////lyo：读取到的数据长度小于标定长度，报错
                //if (strbuff.Length < packegeLen)
                //{
                //    return HUtil32.GetString(strbuff, 0, strbuff.Length);
                //}
                return HUtil32.GetString(strbuff, 0, packegeLen);
            }
            catch
            {
                return null;
            }
        }
    }
}