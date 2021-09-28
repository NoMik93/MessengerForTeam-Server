using System;
using System.Data.SQLite;
using PacketClass;
using System.Data.SQLite.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
namespace DB_s
{
    public class DBfunc
    {
        private SQLiteConnection conn = null;
        private string location = "Data Source=c:/sqlite/db_test.db";

        public object SQLiteLinq { get; private set; }

        public int login(UserInfo userInfo)//로그인 성공하면 1 실패하면 -1
        {//로그인
            conn = new SQLiteConnection(location + ";Version=3;");
            conn.Open();
            SQLiteDataReader sqr = null;
            String sql = "SELECT count(*) as result FROM USER WHERE userID='" + userInfo.m_strID + "'" + "AND userPass ='" + userInfo.m_strPass + "'";
            //  Console.WriteLine(sql);
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            // command.ExecuteNonQueryAsync();
            int result = 0;
            try
            {
                sqr = command.ExecuteReader();//다읽었을때 reader닫아주는 행동
                sqr.Read();

                result = Int32.Parse(sqr["result"].ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "이유로 데이터베이스 명령 실패");
            }
            finally { if (sqr != null) sqr.Close(); }
            //1단계 제대로 DB연동되는지 체크
            //구현 해야될 기능 : password 출력후 값이 맞으면 1 틀리면 0을 출력하여야함
            return result;
        }
        public int Join(Join join)//회원가입//성공하면 1
        {
            conn = new SQLiteConnection(location + ";Version=3;");
            conn.Open();
            String sql = "INSERT INTO User VALUES ('" + join.m_strName + "','" + join.m_strID + "','" + join.m_strPass + "')";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            int result;
            try
            {
                result = command.ExecuteNonQuery();
            }
            catch
            {
                result = 0;
            }
            //1단계 제대로 DB연동되는지 체크
            //구현 해야될 기능 : 회원가입이 완료되었다면 1을 출력하여 출력되었다고 알려줌

            return result;//성공하면 1인값이 저장
        }
        public int Join_out(UserInfo userinfo)//회원탈퇴 성공하면 1
        {
            conn = new SQLiteConnection(location + ";Version=3;");
            conn.Open();
            String sql = "DELETE FROM User WHERE userID='" + userinfo.m_strID + "'";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            int result;
            try
            {
                result = command.ExecuteNonQuery();
            }
            catch
            {
                result = 0;
            }
            //1단계 제대로 DB연동되는지 체크
            //구현 해야될 기능 : 회원탈퇴가 완료 되었으면 되었다고 출력
            return result;
        }
        public List<ChatInfo> Find_my_chat(UserInfo user)//내 채팅방 검색
        {
            List<ChatInfo> infoList = new List<ChatInfo>();
            SQLiteDataReader sqr = null;
            conn = new SQLiteConnection(location + ";Version=3;");
            conn.Open();
            String sql = "SELECT * FROM Chat WHERE chatID IN (SELECT J_chatID FROM Join_part WHERE J_userID='" + user.m_strID + "')";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            int result = command.ExecuteNonQuery();
            try
            {
                sqr = command.ExecuteReader(CommandBehavior.CloseConnection);//다읽었을때 reader닫아주는 행동
                while (sqr.Read())
                {
                    infoList.Add(new ChatInfo(sqr["chatID"].ToString(), sqr["Start_date"].ToString(), sqr["End_Date"].ToString(), sqr["Intro"].ToString(), sqr["Title"].ToString(), sqr["pw"].ToString(), sqr["S_userID"].ToString()));//가능하려나?
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "이유로 데이터베이스 명령 실패");
            }
            finally { sqr.Close(); }
            return infoList;
            //1단계 제대로 DB연동되는지 체크
            //구현 해야될 기능 : select으로 찾은 채팅방의 정보들을 뽑아서 출력하여서 보내야함
        }
        public List<ChatInfo> Find_All_chat() //전체 채팅방 검색
        {
            List<ChatInfo> infoList = new List<ChatInfo>();
            SQLiteDataReader sqr = null;
            conn = new SQLiteConnection(location + ";Version=3;");
            conn.Open();
            String sql = "SELECT * FROM Chat ";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            int result = command.ExecuteNonQuery();
            try
            {
                sqr = command.ExecuteReader(CommandBehavior.CloseConnection);//다읽었을때 reader닫아주는 행동
                while (sqr.Read())
                {
                    infoList.Add(new ChatInfo(sqr["chatID"].ToString(), sqr["Start_date"].ToString(), sqr["End_Date"].ToString(), sqr["Intro"].ToString(), sqr["Title"].ToString(), sqr["pw"].ToString(), sqr["S_userID"].ToString()));//가능하려나?
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "이유로 데이터베이스 명령 실패");
            }
            finally { sqr.Close(); }
            return infoList;
            //1단계 제대로 DB연동되는지 체크
            //구현 해야될 기능 : 전체 채팅방의 정보를 뽑아옴으로 인해서 그에 관한 정보들 뽑아주는함수 
        }
        public String get_pw_chat(ChatInfo chatinfo) //해당 채팅방의 비밀번호 얻기
        {
            SQLiteDataReader sqr = null;
            conn = new SQLiteConnection(location + ";Version=3;");
            conn.Open();
            String sql = "SELECT pw FROM Chat WHERE chatID='" + chatinfo.chatID + "'";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            int result = command.ExecuteNonQuery();
            //1단계 제대로 DB연동되는지 체크
            //구현 해야될 기능 : password만 return해주기
            String result_pass = null;
            try
            {
                sqr = command.ExecuteReader(CommandBehavior.CloseConnection);
                while (sqr.Read())
                {
                    result_pass = sqr["pw"].ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "이유로 데이터베이스 명령 실패");
            }
            finally { sqr.Close(); }
            return result_pass;
        }
        public List<CalendarInfo> Find_schedule_chat(ChatInfo chatinfo)//채팅방 스케쥴 검색
        {
            List<CalendarInfo> Ca_List = new List<CalendarInfo>();
            SQLiteDataReader sqr = null;
            conn = new SQLiteConnection(location + ";Version=3;");
            conn.Open();
            String sql = "SELECT * FROM Calendar WHERE Cal_chatID='" + chatinfo.chatID + "'";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            int result = command.ExecuteNonQuery();
            try
            {
                sqr = command.ExecuteReader(CommandBehavior.CloseConnection);
                while (sqr.Read())
                {

                    Ca_List.Add(new CalendarInfo(sqr["Cal_chatID"].ToString(), sqr["Week"].ToString(), sqr["Time"].ToString(), sqr["calText"].ToString()));//가능하려나?

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "이유로 데이터베이스 명령 실패");
            }
            finally { sqr.Close(); }
            //1단계 제대로 DB연동되는지 체크
            //구현 해야될 기능 : 스케쥴을 전체다 출력하여 주기
            return Ca_List;

        }
        public int Add_schedule_chat(CalendarInfo calinfo)//개인일정 추가
        {
            conn = new SQLiteConnection(location + ";Version=3;");
            conn.Open();
            String sql = "INSERT INTO Calendar VALUES('" + calinfo.chatID + "','" + calinfo.Day + "','" + calinfo.Time + "','" + calinfo.Text + "')";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            int result;
            try
            {
                result = command.ExecuteNonQuery();
            }
            catch
            {
                result = 0;
            }
            //1단계 제대로 DB연동되는지 체크
            //구현 해야될 기능 : 성공여부만 체크
            return result;//성공하면 0아닌거 맞아?
        }
        public int Del_schedule_chat(CalendarInfo calinfo)//텍스트랑 비교하여 삭제
        {
            conn = new SQLiteConnection(location + ";Version=3;");
            conn.Open();
            String sql = "DELETE FROM Calendar WHERE calText='" + calinfo.Text + "'";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            int result;
            try
            {
                result = command.ExecuteNonQuery();
            }
            catch
            {
                result = 0;
            }
            //1단계 제대로 DB연동되는지 체크
            //구현 해야될 기능 : 삭제된지만 체크
            return result;//성공하면 0아닌거 맞아?
        }
        public List<CalendarInfo> Find_schedule_Indi(UserInfo userinfo) //텍스트랑 비교하여 전체 출력
        {
            List<CalendarInfo> Ca_List = new List<CalendarInfo>();
            SQLiteDataReader sqr = null;
            conn = new SQLiteConnection(location + ";Version=3;");
            conn.Open();

            String sql = "SELECT * FROM Calendar WHERE calText IN (SELECT c_calText FROM cal_indi WHERE c_userID='" + userinfo.m_strID + "')";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            int result = command.ExecuteNonQuery();
            try
            {
                sqr = command.ExecuteReader(CommandBehavior.CloseConnection);
                while (sqr.Read())
                {
                    Ca_List.Add(new CalendarInfo(sqr["Cal_chatID"].ToString(), sqr["Week"].ToString(), sqr["Time"].ToString(), sqr["calText"].ToString()));//가능하려나?
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "이유로 데이터베이스 명령 실패");
            }
            finally { sqr.Close(); }
            //1단계 제대로 DB연동되는지 체크
            //구현 해야될 기능 : 스케쥴을 전체다 출력하여 주기
            return Ca_List;
        }
        public int Insert_chat_txet(ChatText chattext)//대화추가  
        {
            conn = new SQLiteConnection(location + ";Version=3;");
            conn.Open();
            String sql = "INSERT INTO Communication Values(@param1,@param2,strftime('%Y-%m-%d %H:%M:%S', 'now'),@param3)";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.Parameters.Add(new SQLiteParameter("@param1", chattext.userID));
            command.Parameters.Add(new SQLiteParameter("@param2", chattext.chatID));
            command.Parameters.Add(new SQLiteParameter("@param3", chattext.chatText));
            int result;
            try
            {
                result = command.ExecuteNonQuery();
            }
            catch
            {
                result = 0;
            }
            //1단계 제대로 DB연동되는지 체크
            //구현 해야될 기능 : 개인 스케쥴을 전부다 뽑아주기
            return result;
        }
        public int Create_chat(ChatInfo chatinfo)//채팅방생성 
        {
            conn = new SQLiteConnection(location + ";Version=3;");
            conn.Open();
            String sql = "INSERT INTO Chat Values(@param1,@param2,@param3,@param4,@param5,@param6,@param7)";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.Parameters.Add(new SQLiteParameter("@param1", chatinfo.chatID));
            command.Parameters.Add(new SQLiteParameter("@param2", chatinfo.leaderID));
            command.Parameters.Add(new SQLiteParameter("@param3", chatinfo.chatIntro));
            command.Parameters.Add(new SQLiteParameter("@param4", chatinfo.chatStartDate));
            command.Parameters.Add(new SQLiteParameter("@param5", chatinfo.chatEndDate));
            command.Parameters.Add(new SQLiteParameter("@param6", chatinfo.chatTitle));
            command.Parameters.Add(new SQLiteParameter("@param7", chatinfo.chatPW));

            int result;
            try
            {
                result = command.ExecuteNonQuery();
            }
            catch
            {
                result = 0;
            }
            //1단계 제대로 DB연동되는지 체크
            //구현 해야될 기능 : 개인 스케쥴을 전부다 뽑아주기
            return result;
        }
        public List<ChatText> Load_chat_txet(ChatInfo chatinfo) //해당 chatinfo로 저장된 대화내용들 load
        {
            List<ChatText> Chat_Text_List = new List<ChatText>();
            SQLiteDataReader sqr = null;
            conn = new SQLiteConnection(location + ";Version=3;");
            conn.Open();
            String sql = "SELECT * FROM Communication WHERE T_chatID='" + chatinfo.chatID + "'";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            int result = command.ExecuteNonQuery();
            //1단계 제대로 DB연동되는지 체크
            //구현 해야될 기능 : 개인 스케쥴을 전부다 뽑아주기
            try
            {
                sqr = command.ExecuteReader(CommandBehavior.CloseConnection);
                while (sqr.Read())
                {
                    Chat_Text_List.Add(new ChatText(sqr["T_chatID"].ToString(), sqr["T_userID"].ToString(), Convert.ToDateTime(sqr["Text_Time"]), sqr["Text"].ToString()));//가능하려나?순서바꿔서
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "이유로 데이터베이스 명령 실패");
            }
            finally { sqr.Close(); }

            return Chat_Text_List;
        }
        public List<String> This_chat_userID(ChatInfo chatinfo) //현재 접속중인 채팅방에 있는 유저아이디들 출력
        {
            List<String> Chat_in_user_List = new List<string>();
            SQLiteDataReader sqr = null;
            conn = new SQLiteConnection(location + ";Version=3;");
            conn.Open();
            String sql = "SELECT J_UserID FROM Join_part WHERE J_ChatID='" + chatinfo.chatID + "'";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            int result = command.ExecuteNonQuery();
            //1단계 제대로 DB연동되는지 체크
            //구현 해야될 기능 : 개인 스케쥴을 전부다 뽑아주기

            try
            {
                sqr = command.ExecuteReader(CommandBehavior.CloseConnection);
                while (sqr.Read())
                {
                    Chat_in_user_List.Add(sqr["J_UserID"].ToString());//가능하려나?
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "이유로 데이터베이스 명령 실패");
            }
            finally { sqr.Close(); }

            return Chat_in_user_List;
        }
        public void Content_In(ChatInfo chatinfo) //이미지나 zip파일 넣는 함수///////////미완
        {
            conn = new SQLiteConnection(location + ";Version=3;");
            conn.Open();
            //  SQLite
            String sql = "SELECT UserID FROM Join_part WHERE J_ChatID='" + chatinfo.chatID + "'";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            int result = command.ExecuteNonQuery();
            //1단계 제대로 DB연동되는지 체크
            //구현 해야될 기능 : 개인 스케쥴을 전부다 뽑아주기
        }
        public int Add_chat_user(ChatText chattext)
        {
            conn = new SQLiteConnection(location + ";Version=3;");
            conn.Open();
            String sql = "INSERT INTO Join_part Values(@param1,@param2)";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.Parameters.Add(new SQLiteParameter("@param1", chattext.userID));
            command.Parameters.Add(new SQLiteParameter("@param2", chattext.chatID));

            int result;
            try
            {
                result = command.ExecuteNonQuery();
            }
            catch
            {
                result = 0;
            }
            return result;
        }

    }
}