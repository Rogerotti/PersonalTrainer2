using System;
using System.Runtime.Serialization;

namespace Framework.Models
{
    [DataContract]
    public class RecaptchaDto
    {
        [DataMember(Name = "secret")]
        public String Secret { get; set; }
        [DataMember(Name = "response")]
        public String Response { get; set; }
    }
}
