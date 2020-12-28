using System;

namespace Core.Data.Account
{
    public class MailAddress
    {
        public string Address { get; set; }
        public bool Verified { get; set; }
        public DateTime AddTime { get; set; }
        public DateTime? VerificationTime { get; set; }
        public DateTime? RemoveTime { get; set; }
    }
}