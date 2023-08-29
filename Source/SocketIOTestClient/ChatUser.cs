namespace SocketIOTestClient
{
    public class ChatUser
    {
        public int Id { get; set; }             // 접속시 서버에서 발급해주는 아이디 
        public string Nickname { get; set; }    // 유저의 닉네임
        public ChatUser() { }
    }
}
