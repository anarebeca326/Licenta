using System;
using System.Text;

namespace services.utils
{
    public class Transcoder
    {
        public static string EncodePass(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(bytes);
        }

        public static string DecodePass(string password)
        {
            var decoded = Convert.FromBase64String(password);
            return Encoding.UTF8.GetString(decoded);
        }
    }
}
