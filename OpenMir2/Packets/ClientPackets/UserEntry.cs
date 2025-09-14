using OpenMir2.Extensions;
using System.IO;

namespace OpenMir2.Packets.ClientPackets
{
    public class UserEntry : ClientPacket
    {
        public string Account;
        public string Password;
        public string UserName;
        public string SSNo;
        public string Phone;
        public string Quiz;
        public string Answer;
        public string EMail;

        public const byte Size = 140 + 8;

        protected override void ReadPacket(BinaryReader reader)
        {
            throw new System.NotImplementedException();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.WriteAsciiString(Account, 10);
            writer.WriteAsciiString(Password, 10);
            writer.WriteAsciiString(UserName, 20);
            writer.WriteAsciiString(SSNo, 14);
            writer.WriteAsciiString(Phone, 14);
            writer.WriteAsciiString(Quiz, 20);
            writer.WriteAsciiString(Answer, 12);
            writer.WriteAsciiString(EMail, 40);
        }
    }

    public class UserAccountPacket : ClientPacket
    {
        public UserEntry UserEntry;
        public UserEntryAdd UserEntryAdd;

        public UserAccountPacket()
        {
            UserEntry = new UserEntry();
            UserEntryAdd = new UserEntryAdd();
        }

        protected override void ReadPacket(BinaryReader reader)
        {
            //lyo：客户端包结构
              //TUserEntryA = packed record
              //  sAccount: string[10];
              //  sPassword: string[10];
              //  sUserName: string[20];
              //  sSSNo: string[14];
              //  sPhone: string[14];
              //  sQuiz: string[20];
              //  sAnswer: string[12];
              //  sEMail: string[40];
              //  sQuiz2: string[20];
              //  sAnswer2: string[12];
              //  sBirthDay: string[10];
              //  sMobilePhone: string[13];
              //  sMemo: string[20];
              //  sMemo2: string[20];
              //end;

            UserEntry = new UserEntry();
            UserEntry.Account = reader.ReadPascalString(10);
            UserEntry.Password = reader.ReadPascalString(10);
            UserEntry.UserName = reader.ReadPascalString(20);
            UserEntry.SSNo = reader.ReadPascalString(14);//客户端版本，客户端写死
            UserEntry.Phone = reader.ReadPascalString(14);
            UserEntry.Quiz = reader.ReadPascalString(20);
            UserEntry.Answer = reader.ReadPascalString(12);
            UserEntry.EMail = reader.ReadPascalString(40);

            UserEntryAdd = new UserEntryAdd();
            UserEntryAdd.Quiz2 = reader.ReadPascalString(20);
            UserEntryAdd.Answer2 = reader.ReadPascalString(12);
            UserEntryAdd.BirthDay = reader.ReadPascalString(10);
            UserEntryAdd.MobilePhone = reader.ReadPascalString(13);
            UserEntryAdd.Memo = reader.ReadPascalString(20);
            UserEntryAdd.Memo2 = reader.ReadPascalString(20);
            //lyo：防错，客户端发回来的数据不能读取UserEntryAdd部分（读Quiz2时第一个字节读到的数量不对）
            if (UserEntryAdd.BirthDay == null)
            {
                UserEntryAdd.BirthDay = "1922/02/02";
                UserEntryAdd.Quiz2 = "autovalue";
            }
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.WriteAsciiString(UserEntry.Account, 10);
            writer.WriteAsciiString(UserEntry.Password, 10);
            writer.WriteAsciiString(UserEntry.UserName, 20);
            writer.WriteAsciiString(UserEntry.SSNo, 14);
            writer.WriteAsciiString(UserEntry.Phone, 14);
            writer.WriteAsciiString(UserEntry.Quiz, 20);
            writer.WriteAsciiString(UserEntry.Answer, 12);
            writer.WriteAsciiString(UserEntry.EMail, 40);
            writer.WriteAsciiString(UserEntryAdd.Quiz2, 20);
            writer.WriteAsciiString(UserEntryAdd.Answer2, 12);
            writer.WriteAsciiString(UserEntryAdd.BirthDay, 10);
            writer.WriteAsciiString(UserEntryAdd.MobilePhone, 13);
            writer.WriteAsciiString(UserEntryAdd.Memo, 20);
            writer.WriteAsciiString(UserEntryAdd.Memo2, 20);
        }
    }
}