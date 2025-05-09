namespace DTech.Library
{
    public class XMessage(string typeMsg, string msg)
    {
        public string TypeMsg { get; set; } = typeMsg;

        public string Msg { get; set; } = msg;
    }
}
