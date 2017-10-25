using System;

namespace Framework.Models.Dto
{
    public class TopMenuDto
    {
        public Boolean IsLogedIn { get; set; }

        public Boolean IsAdmin { get; set; }

        public String UserName { get; set; }
    }
}
