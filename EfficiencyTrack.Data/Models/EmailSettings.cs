namespace EfficiencyTrack.Data.Models
{
    public class EmailSettings
    {
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }
        public string SmtpHost { get; set; } = "smtp.gmail.com"; 
        public int SmtpPort { get; set; } = 587;                 
        public bool EnableSsl { get; set; } = true;              
    }
}
