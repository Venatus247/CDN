namespace Core.Utils.Response
{
    public class BasicResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        
        protected BasicResponse() {}

        public BasicResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }
        
    }
}