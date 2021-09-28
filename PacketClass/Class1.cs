using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;

namespace PacketClass
{
    public enum PacketType
    {
        ///////////////////////////////////// 공통 패킷부 /////////////////////////////////////
        //채팅방 내부 관련
        채팅내용 = 0,               //ChatText     패킷     
        채팅파일,                   //



        ///////////////////////////////////// 클라이언트 패킷부 /////////////////////////////////////
        //회원 정보 조작 관련
        로그인,                     //userInfo 패킷                  //Select count from [유저 table] where userID
        회원가입,                   //Join     패킷                  //Insert
        회원탈퇴,                   //userInfo 패킷                  //delete

        //채팅방 검색 및 가입 관련
        내채팅방검색,               //userID    패킷                 //Select [chatInfo data] from [chat table] where userID
        전체채팅방검색,             //userID    패킷                 //Select [chatInfo data] from [chat table]
        제목채팅방검색,             //chatTitle 패킷                 //Select [chatInfo data] from [chat table] where title
        초대코드검색,               //chatPW    패킷                 //Select [chatInfo data] from [chat table] where ~~
        채팅방접속,                 //ChatText  패킷                 //Select count From [chat table] where chatID order(??)
        채팅방가입,                 //chatText  패킷
        채팅방생성,                 //ChatInfo  패킷

        //일정 관련
        채팅방일정,                 //chatID       패킷              //Select [Calendar Info] from [Calendar table] where chatID
        일정추가,                   //CalendarInfo 패킷              //insert
        일정삭제,                   //CalendarInfo 패킷              //delete
        개인일정,                   //userID       패킷              //Select [Calendar Info] from [Calendar table] where userID



        ///////////////////////////////////// 서버 패킷부 /////////////////////////////////////
        질의결과전송,               //QueryResult  패킷              

        //일정 및 채팅방 정보
        일정전송,                   //CalendarInfo 패킷
        채팅방전송                  //ChatInfo     패킷

    }
    public enum PacketSendERROR
    {
        정상 = 0,
        에러
    }

    [Serializable]
    public class Packet
    {
        public int Length;
        public int Type;

        public Packet()
        {
            this.Length = 0;
            this.Type = 0;
        }
        public static byte[] Serialize(Object o)
        {
            MemoryStream ms = new MemoryStream(1024 * 4);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, o);
            return ms.ToArray();
        }
        //public static byte[] SerializeImage(Image img)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    img.Save(ms, ImageFormat.Jpeg);
        //    return ms.ToArray();
        //}

        public static Object Desserialize(byte[] bt)
        {
            MemoryStream ms = new MemoryStream(1024 * 4);
            foreach (byte b in bt)
            {
                ms.WriteByte(b);
            }
            ms.Flush();
            ms.Position = 0;
            BinaryFormatter bf = new BinaryFormatter();

            Object obj = bf.Deserialize(ms);
            ms.Close();
            return obj;
        }
        //public static Image DesserializeImage(byte[] bt)
        //{
        //    MemoryStream ms = new MemoryStream(bt);

        //    Image returnImage = Image.FromStream(ms);

        //    return returnImage;
        //}
    }
    [Serializable]
    public class Join : Packet                      //회원가입 전송 패킷
    {
        public string m_strID;                      /*회원가입 유저 ID*/
        public string m_strPass;                    /*회원가입 유저 PW*/
        public string m_strName;                    /*회원가입 유저 이름*/
        public Join()
        {
            this.m_strID = null;
            this.m_strPass = null;
            this.m_strName = null;
        }
    }
    [Serializable]
    public class UserInfo : Packet                  //유저 ID, PW 전송 패킷
    {
        public string m_strID;
        public string m_strPass;
        public UserInfo()
        {
            this.m_strID = null;
            this.m_strPass = null;
        }
    }
    [Serializable]
    public class UserID : Packet                      //유저 ID 전송 패킷
    {
        public string m_strID;

        public UserID()
        {
            this.m_strID = null;
        }
    }
    [Serializable]
    public class ChatID : Packet                    //채팅방 ID 전송 패킷
    {
        public string chatID;

        public ChatID()
        {
            this.chatID = null;
        }
    }
    [Serializable]
    public class CalendarInfo : Packet              //캘린더 전송 패킷
    {
        public string chatID;                       /*해당 일정 채팅방 ID*/
        public string Day;                          /*해당 일정 요일*/
        public string Time;                         /*해당 일정 시간*/
        public string Text;                         /*해당 일정 내용*/

        public CalendarInfo()
        {
            this.chatID = null;
            this.Day = null;
            this.Time = null;
            this.Text = null;
        }
        public CalendarInfo(string chatID, string Day, string Time, string Text)//DB때문에 추가함
        {
            this.chatID = chatID;
            this.Day = Day;
            this.Time = Time;
            this.Text = Text;
        }
    }
    [Serializable]
    public class ChatTitle : Packet                     //채팅방 제목 전송 패킷
    {
        public string chatTitle;

        public ChatTitle()
        {
            this.chatTitle = null;
        }
    }
    [Serializable]
    public class ChatPW : Packet                        //초대코드 전송 패킷
    {
        public string chatPW;

        public ChatPW()
        {
            this.chatPW = null;
        }
    }
    [Serializable]
    public class ChatInfo : Packet              //채팅방의 모든 정보 전송 패킷
    {
        public string chatID;                   /*채팅방ID*/
        public string chatStartDate;            /*채팅방 만든 날짜*/
        public string chatEndDate;              /*채팅방 종료 날짜*/
        public string chatIntro;                /*채팅방 소개글*/
        public string chatTitle;                /*채팅방 제목*/
        public string chatPW;                   /*채팅방 비밀번호*/
        public string leaderID;                 /*채팅방 리더 정보*/

        public ChatInfo()
        {
            this.chatID = null;
            this.chatStartDate = null;
            this.chatEndDate = null;
            this.chatIntro = null;
            this.chatTitle = null;
            this.chatPW = null;
            this.leaderID = null;
        }
        public ChatInfo(string chatID, string chatStartDate, string chatEndDate, string chatIntro, string chatTitle, string chatPW, string leaderID)//DB떄문에 추가
        {
            this.chatID = chatID;
            this.chatStartDate = chatStartDate;
            this.chatEndDate = chatEndDate;
            this.chatIntro = chatIntro;
            this.chatTitle = chatTitle;
            this.chatPW = chatPW;
            this.leaderID = leaderID;
        }
    }
    [Serializable]
    public class ChatText : Packet
    {
        public string chatID;
        public string userID;
        public DateTime chatOrder;
        public string chatText;

        public ChatText()
        {
            this.chatID = null;
            this.userID = null;
            this.chatText = null;
        }
        public ChatText(string chatID, string userID, DateTime chatOrder, string chatText)
        {
            this.chatID = chatID;
            this.userID = userID;
            this.chatText = chatText;
            this.chatOrder = chatOrder;
        }
    }
    [Serializable]
    public class QueryResult : Packet
    {
        public int result;

        public QueryResult()
        {
            this.result = 0;
        }
    }
}

