using System;
using System.Runtime.Serialization;

namespace Framework.Models
{
    [DataContract]
    public class RecaptchaResult
    {
        [DataMember(Name = "success")]
        public Boolean Success { get; set; }

        [DataMember(Name = "challenge_ts")]
        public String Time { get; set; }

        [DataMember(Name = "hostname")]
        public String Hostname { get; set; }

        [DataMember(Name = "error-codes")]
        public String[] Errors { get; set; }
    }
}
