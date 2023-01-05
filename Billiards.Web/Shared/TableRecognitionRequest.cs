namespace Billiards.Web.Shared
{
    public class TableRecognitionRequest
    {
        public TableRecognitionRequest(string data)
        {
            Data = data;
        }

        public string Data { get; set; }
    }
}