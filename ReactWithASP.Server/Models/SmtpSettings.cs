namespace ReactWithASP.Server.Models
{
    public class SmtpSettings
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EnableSsl { get; set; }
        public string FromEmail { get; set; }
    }

}
