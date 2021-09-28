using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using PacketClass;
using System.Net.NetworkInformation;
using System.Threading;
using DB_s;

namespace Messenger_For_Team_Server
{
    public partial class serverF : Form
    {
        private byte[] _receivebuffer = new byte[1024 * 4];
        private byte[] _sendbuffer = new byte[1024 * 4];

        public List<SocketT2h> __ClientSockets { get; set; }
        List<string> _ids = new List<string>();
        private Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


        DBfunc DB = new DBfunc();

        public serverF()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            __ClientSockets = new List<SocketT2h>();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            SetupServer();
        }

        private void serverF_Load(object sender, EventArgs e)
        {
            string before = string.Empty;
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork || ip.IsDnsEligible)
                        {
                            txtIP.Text = before;
                            before = ip.Address.ToString();
                        }
                    }
                }
            }
        }

        private void SetupServer()
        {
            if (txtPort.Text == string.Empty)
                txtPort.Text = "4000";
            txtLog.Text += "Setting up server Port" + txtPort.Text + "...\r\n";

            try
            {
                _serverSocket.Bind(new IPEndPoint(IPAddress.Parse(txtIP.Text), Int32.Parse(txtPort.Text)));
                _serverSocket.Listen(10);
                _serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
                txtIP.ReadOnly = true;
                txtPort.ReadOnly = true;
                btnStart.Enabled = false;
            }
            catch
            {
                txtLog.Text += "삐빅-IP주소 이상하다\r\n";
            }
        }

        private void AppceptCallback(IAsyncResult ar)
        {
            Socket socket = _serverSocket.EndAccept(ar);
            __ClientSockets.Add(new SocketT2h(socket));
            txtLog.Text += __ClientSockets.Count.ToString() + "연결 성공적\r\n";
            socket.BeginReceive(_receivebuffer, 0, _receivebuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);  // 수정사항
            _serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {

            Socket socket = (Socket)ar.AsyncState;
            if (socket.Connected)
            {
                int received;
                try
                {
                    received = socket.EndReceive(ar);
                }
                catch (Exception)
                {
                    for (int i = 0; i < __ClientSockets.Count; i++)
                    {
                        if (__ClientSockets[i]._Socket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                        {
                            txtLog.Text += __ClientSockets[i]._ID + " 접속 해제" + "\r\n";
                            __ClientSockets.RemoveAt(i);
                        }
                    }
                    return;
                }

                /////////////////////////// 정상 작동 구현 부//////////////////////////
                if (received != 0)
                {
                    byte[] dataBuf = new byte[received];
                    Array.Copy(_receivebuffer, dataBuf, received);                     //data 받기 부분


                    Packet packet = (Packet)Packet.Desserialize(dataBuf);


                    switch (packet.Type)
                    {
                        case (int)PacketType.회원가입:
                            {
                                //회원가입
                                Join(dataBuf, socket);
                                break;
                            }
                        case (int)PacketType.로그인:
                            {

                                Login(dataBuf, socket);
                                //로그인
                                break;
                            }
                        case (int)PacketType.전체채팅방검색:
                            {
                                searchAllChatRoom(dataBuf, socket);
                                break;
                            }
                        case (int)PacketType.내채팅방검색:
                            {
                                searchMyChatRoom(dataBuf, socket);
                                break;
                            }
                        case (int)PacketType.제목채팅방검색:               ///클라이언트 내부 처리가능
                            {
                                searchChatRoomByTitle(dataBuf, socket);
                                break;
                            }
                        case (int)PacketType.채팅내용:
                            {
                                getChatText(dataBuf, socket);
                                break;
                            }
                        case (int)PacketType.채팅방생성:                     //의미없음
                            {
                                createChatRoom(dataBuf, socket);
                                break;
                            }
                        case (int)PacketType.채팅방접속:                     //
                            {
                                connectChatRoom(dataBuf, socket);
                                break;
                            }
                        case (int)PacketType.채팅방일정:
                            {
                                calender_Send(dataBuf, socket);//
                                break;
                            }
                        case (int)PacketType.채팅방가입:
                            {
                                joinChatRoom(dataBuf, socket);
                                break;
                            }
                        case (int)PacketType.일정전송://개인일정??? 밑에랑 중복
                            {
                                break;
                            }
                        case (int)PacketType.일정추가:
                            {
                                calender_Add(dataBuf, socket);
                                break;
                            }
                        case (int)PacketType.일정삭제:
                            {
                                calender_Del(dataBuf, socket);
                                break;
                            }
                        case (int)PacketType.회원탈퇴:
                            {
                                withdraw_Account(dataBuf, socket);
                                break;
                            }
                        case (int)PacketType.초대코드검색:
                            {
                                getChat_Pass(dataBuf, socket);//비밀번호
                                break;
                            }
                        case (int)PacketType.채팅파일:
                            {
                                get_Chat_File(dataBuf, socket);//파일들 미구현
                                break;
                            }
                        case (int)PacketType.개인일정:
                            {
                                get_Indi_Cal(dataBuf, socket);
                                break;
                            }
                    }
                }
                else
                {                                                           ////아직 이해 못함////
                    for (int i = 0; i < __ClientSockets.Count; i++)
                    {
                        if (__ClientSockets[i]._Socket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                        {
                            __ClientSockets.RemoveAt(i);
                            txtLog.Text = "연결안되있는데 보냄: " + __ClientSockets.Count.ToString() + "연결해제\r\n";
                        }
                    }
                }

            }
            socket.BeginReceive(_receivebuffer, 0, _receivebuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }


        void Send(Socket socket, byte[] data)
        {
            Thread.Sleep(200);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
        }
        private void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }


        public void Join(byte[] data, Socket Sender)
        {
            Join joinClass = (Join)Packet.Desserialize(data);
            QueryResult qr = new QueryResult();

            qr.result = DB.Join(joinClass);

            if (qr.result > 0)
                txtLog.Text += joinClass.m_strID + "셍성\r\n";
            else
                txtLog.Text += joinClass.m_strID + "셍성실패\r\n";

            qr.Type = (int)PacketType.회원가입;

            byte[] sendData = Packet.Serialize(qr);

            Send(Sender, sendData);

            //회원가입
        }

        public void Login(byte[] data, Socket Sender)
        {

            UserInfo uInfo = (UserInfo)Packet.Desserialize(data);
            QueryResult qr = new QueryResult();

            qr.Type = (int)PacketType.로그인;

            qr.result = DB.login(uInfo);

            txtLog.Text += uInfo.m_strID + " " + uInfo.m_strPass + "\r\n";
            txtLog.Text += qr.result.ToString() + "\r\n";

            if (qr.result == 1)
            {
                for (int i = 0; i < __ClientSockets.Count; i++)
                {
                    if (__ClientSockets[i]._Socket.RemoteEndPoint.ToString().Equals(Sender.RemoteEndPoint.ToString()))
                    {
                        __ClientSockets[i]._ID = uInfo.m_strID;
                    }
                }
            }

            byte[] sendData = Packet.Serialize(qr);

            Send(Sender, sendData);
        }

        public void searchAllChatRoom(byte[] data, Socket Sender)
        {
            UserID userid = (UserID)Packet.Desserialize(data);

            List<ChatInfo> allchat = DB.Find_All_chat();

            txtLog.Text += "전체 방 " + allchat.Count.ToString() + "개" + "\r\n";

            foreach (ChatInfo chat in allchat)
            {
                chat.Type = (int)PacketType.전체채팅방검색;

                byte[] sendData = Packet.Serialize(chat);
                Send(Sender, sendData);
            }
        }
        public void searchMyChatRoom(byte[] data, Socket Sender)
        {
            UserInfo userinfo = (UserInfo)Packet.Desserialize(data);

            List<ChatInfo> mychat = DB.Find_my_chat(userinfo);

            txtLog.Text += userinfo.m_strID + "의 방 " + mychat.Count.ToString() + "개" + "\r\n";

            foreach (ChatInfo chat in mychat)
            {
                chat.Type = (int)PacketType.내채팅방검색;

                byte[] sendData = Packet.Serialize(chat);
                Send(Sender, sendData);
            }
        }
        public void searchChatRoomByTitle(byte[] data, Socket Sender)
        {
            ChatTitle title = (ChatTitle)Packet.Desserialize(data);

            List<ChatInfo> allchat = DB.Find_All_chat();

            int i = 0;

            foreach (ChatInfo chat in allchat)
            {
                if (chat.chatTitle.Contains(title.chatTitle))
                {
                    chat.Type = (int)PacketType.제목채팅방검색;

                    i++;

                    byte[] sendData = Packet.Serialize(chat);
                    Send(Sender, sendData);
                }
            }
            txtLog.Text += title.chatTitle + "의 방 " + i.ToString() + "개" + "\r\n";
        }

        public void getChatText(byte[] data, Socket Sender)
        {
            ChatText text = (ChatText)Packet.Desserialize(data);
            ChatInfo info = new ChatInfo();

            info.chatID = text.chatID;

            if (DB.Insert_chat_txet(text) > 0)                  //접속 중인 유저중에, 해당 채팅방의 유저에게 전송
            {
                List<string> userId = DB.This_chat_userID(info);

                foreach (string id in userId)
                {
                    foreach (SocketT2h user in __ClientSockets)
                    {
                        if (id.Equals(user._ID) && !user._Socket.RemoteEndPoint.ToString().Equals(Sender.RemoteEndPoint.ToString()))
                        {
                            Send(user._Socket, data);
                        }
                    }
                }
            }
        }
        public void connectChatRoom(byte[] data, Socket Sender)
        {
            ChatText text = (ChatText)Packet.Desserialize(data);
            ChatInfo info = new ChatInfo();
            info.chatID = text.chatID;

            List<ChatText> textList = DB.Load_chat_txet(info);
            
            foreach (ChatText ch in textList)
            {
                txtLog.Text += ch.userID+"유저아이디";
                ch.Type = (int)PacketType.채팅내용;

                byte[] sendData = Packet.Serialize(ch);
                Send(Sender, sendData);
            }
        }
        public void calender_Send(byte[] data, Socket Sender)//채팅방 전체일정
        {
            ChatInfo chatinfo = (ChatInfo)Packet.Desserialize(data);

            List<CalendarInfo> allCal = DB.Find_schedule_chat(chatinfo);

            foreach (var cal in allCal)
            {
                txtLog.Text += cal.Text;
                cal.Type = (int)PacketType.채팅방일정;
                byte[] sendData = Packet.Serialize(cal);
                Send(Sender, sendData);
            }

        }//채팅방 일정보내기
        public void calender_Add(byte[] data, Socket Sender)
        {
            CalendarInfo calinfo = (CalendarInfo)Packet.Desserialize(data);

            QueryResult qr = new QueryResult();
            qr.result = DB.Add_schedule_chat(calinfo);
            qr.Type = (int)PacketType.일정추가;

            byte[] sendData = Packet.Serialize(qr);

            Send(Sender, sendData);

        }//일정 추가
        public void calender_Del(byte[] data, Socket Sender)
        {

            CalendarInfo calinfo = (CalendarInfo)Packet.Desserialize(data);

            QueryResult qr = new QueryResult();
            qr.result = DB.Del_schedule_chat(calinfo);
            qr.Type = (int)PacketType.일정삭제;

            byte[] sendData = Packet.Serialize(qr);

            Send(Sender, sendData);
        }//일정삭제
        public void withdraw_Account(byte[] data, Socket Sender)
        {
            UserInfo userinfo = (UserInfo)Packet.Desserialize(data);
            QueryResult qr = new QueryResult();

            qr.result = DB.Join_out(userinfo);

            qr.Type = (int)PacketType.회원탈퇴;

            byte[] sendData = Packet.Serialize(qr);

            Send(Sender, sendData);

        }//회원탈퇴
        public void getChat_Pass(byte[] data, Socket Sender)
        {
            ChatInfo chatinfo = (ChatInfo)Packet.Desserialize(data);
            ChatPW getPw = new ChatPW();
            getPw.chatPW = DB.get_pw_chat(chatinfo);
            getPw.Type = (int)PacketType.초대코드검색;

            byte[] sendData = Packet.Serialize(getPw);

            Send(Sender, sendData);
        }
        public void get_Chat_File(byte[] data, Socket Sender)
        {
            //
        }
        public void get_Indi_Cal(byte[] data, Socket Sender)
        {
            UserInfo userinfo = (UserInfo)Packet.Desserialize(data);

            List<CalendarInfo> allCal = DB.Find_schedule_Indi(userinfo);

            foreach (var cal in allCal)
            {
                cal.Type = (int)PacketType.개인일정;
                byte[] sendData = Packet.Serialize(cal);
                Send(Sender, sendData);
            }

        }
        public void createChatRoom(byte[] data, Socket Sender)
        {
            ChatInfo info = (ChatInfo)Packet.Desserialize(data);

            info.chatID = info.chatTitle;

            QueryResult qr = new QueryResult();
            qr.Type = (int)PacketType.채팅방생성;

            qr.result = DB.Create_chat(info);
            if (qr.result > 0)
            {
                ChatText text = new ChatText();
                text.chatID = info.chatID;
                text.userID = info.leaderID;
                DB.Add_chat_user(text);
            }
            if (qr.result > 0)
                txtLog.Text += info.chatID + " " + info.leaderID + "생성\r\n";
            else
                txtLog.Text += info.chatID + " " + info.leaderID + "생성실패\r\n";

            byte[] sendData = Packet.Serialize(qr);
            Send(Sender, sendData);
        }

        public void joinChatRoom(byte[] data, Socket Sender)
        {
            ChatInfo info = (ChatInfo)Packet.Desserialize(data);

            ChatText text = new ChatText();
            string pw = DB.get_pw_chat(info);
            QueryResult qr = new QueryResult();
            qr.Type = (int)PacketType.채팅방가입;


            if (pw.Equals(info.chatPW) || pw == string.Empty)
            {
                text.chatID = info.chatID;

                for (int i = 0; i < __ClientSockets.Count; i++)
                {
                    if (__ClientSockets[i]._Socket.RemoteEndPoint.ToString().Equals(Sender.RemoteEndPoint.ToString()))
                    {
                        text.userID = __ClientSockets[i]._ID;
                        qr.result = DB.Add_chat_user(text);
                    }
                }
            }
            else
                qr.result = 0;

           
            byte[] sendData = Packet.Serialize(qr);
            Send(Sender, sendData);
        }
    }

    public class SocketT2h
    {
        public Socket _Socket { get; set; }
        public string _ID { get; set; }
        public SocketT2h(Socket socket)
        {
            this._Socket = socket;
        }
    }
}