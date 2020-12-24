namespace PasswordHub.Models
{
    public class JResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public JResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}
