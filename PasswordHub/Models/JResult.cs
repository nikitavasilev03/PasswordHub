namespace PasswordHub.Models
{
    public class JResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public JResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
        public JResult(bool isSuccess, string message, object data) : this(isSuccess, message)
        {
            Data = data;
        }
    }
}
