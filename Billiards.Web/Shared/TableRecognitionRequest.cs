namespace Billiards.Web.Shared
{
    public class TableRecognitionRequest
    {
        public TableRecognitionRequest(string data)
        {
            Data = data;
        }

/*        public TableRecognitionRequest(byte[]? bytes)
        {
            Bytes = bytes;
        }

        public byte[]? Bytes { get; set; }
*/        public string Data { get; set; }
    }

}