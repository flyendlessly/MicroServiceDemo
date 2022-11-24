using System;
using System.Collections.Generic;
using System.Text;

namespace Class1.Model
{
    public class JwtInfo
    {
        public string Sub { get; set; }
        public string Jti { get; set; }
        public string Iat { get; set; }
    }
}
