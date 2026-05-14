using System;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CR.Utils.DataUtils
{
    public static class GenerateOTP
    {
        private static Random random = new Random();
        public static string GenerateOtp(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder otp = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                otp.Append(chars[random.Next(chars.Length)]);
            }

            return otp.ToString();
        }
    }
}