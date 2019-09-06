using System;
using System.Text.RegularExpressions;
namespace JR.HL.Security
{
    public class SecurityPolicy
    {
        private HashCrypToGraphy m_hashObj;
        public static int checkEffectiveBitSize(string PwdValue)
        {
            int num = 0;
            int length = PwdValue.Length;
            double num2 = Math.Log(Math.Pow((double)SecurityPolicy.getCharSetUsed(PwdValue), (double)length)) / Math.Log(2.0);
            int result;
            if (num2 <= 32.0)
            {
                result = 0;
            }
            else if (num2 <= 64.0)
            {
                result = 1;
            }
            else if (num2 <= 128.0)
            {
                result = 2;
            }
            else
            {
                if (num2 > 128.0)
                {
                    num = 3;
                }
                result = num;
            }
            return result;
        }
        private static bool containsLowerCaseChars(string str)
        {
            Regex regex = new Regex("[a-z]");
            return regex.IsMatch(str);
        }
        private static bool containsNumbers(string str)
        {
            Regex regex = new Regex("[\\d]");
            return regex.IsMatch(str);
        }
        private static bool containsPunctuation(string str)
        {
            Regex regex = new Regex("[\\W|_]");
            return regex.IsMatch(str);
        }
        private static bool containsUpperCaseChars(string str)
        {
            Regex regex = new Regex("[A-Z]");
            return regex.IsMatch(str);
        }
        public string EnPassWord(string PwdValue)
        {
            if (this.m_hashObj == null)
            {
                this.m_hashObj = new HashCrypToGraphy(HashProvider.SHA1);
            }
            return this.m_hashObj.HashString(PwdValue) + "\u0003";
        }
        private static int getCharSetUsed(string pass)
        {
            int num = 0;
            if (SecurityPolicy.containsNumbers(pass))
            {
                num += 10;
            }
            if (SecurityPolicy.containsLowerCaseChars(pass))
            {
                num += 26;
            }
            if (SecurityPolicy.containsUpperCaseChars(pass))
            {
                num += 26;
            }
            if (SecurityPolicy.containsPunctuation(pass))
            {
                num += 31;
            }
            return num;
        }
    }
}
