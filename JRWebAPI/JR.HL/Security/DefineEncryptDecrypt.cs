using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace JR.HL.Security
{
    /// <summary>
    /// 自定义加密解密算法
    /// </summary>
    public class DefineEncryptDecrypt
    {
        // Fields
        private const string EncryptKey = "K";

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <returns></returns>
        public static string Decrypt(string ciphertext)
        {
                if (!string.IsNullOrEmpty(ciphertext))
                {
                    if (ciphertext.Length % 2 == 1)
                    {
                        throw new Exception("非法密码");
                    }

                    try
                    {
                        ciphertext = ciphertext.Replace("Z", "A").Replace("Y", "B").Replace("K", "C").Replace("P", "D").Replace("W", "E").Replace("U", "F");
                        int length = ciphertext.Length;
                        List<char> list = new List<char>();
                        for (int i = 0; i < (length / 2); i++)
                        {
                            char item = (char)Convert.ToInt32(ciphertext.Substring(i * 2, 2), 0x10);
                            list.Add(item);
                        }
                        char[] array = list.ToArray();
                        int num3 = array.Length;
                        for (int j = 0; j < num3; j++)
                        {
                            if ((j % 2) == 0)
                            {
                                array[j] = (char)(((short)array[j]) - 7);
                            }
                            else
                            {
                                array[j] = (char)(((short)array[j]) + 9);
                            }
                        }
                        Array.Reverse(array);
                        StringBuilder builder = new StringBuilder();
                        for (int k = 0; k < num3; k++)
                        {
                            builder.Append(array[k]);
                        }
                        ciphertext = builder.ToString();
                        ciphertext = ciphertext.Replace(EncryptKey, "");
                        ciphertext = ciphertext.Substring(1, ciphertext.Length - 1);
                    }catch(Exception ex)
                    {
                        throw new Exception(string.Format("密码{1}解密失败：{0}", ex.Message, ciphertext));
                    }
                }
                return ciphertext;
            
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="plaintext"></param>
        /// <returns></returns>
        public static string Encrypt(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
            {
                return plaintext;
            }
            plaintext = "E" + plaintext;
            plaintext = plaintext + EncryptKey;
            int length = plaintext.Length;
            char[] array = plaintext.ToCharArray();
            Array.Reverse(array);
            for (int i = 0; i < length; i++)
            {
                if ((i % 2) == 0)
                {
                    array[i] = (char)(((short)array[i]) + 7);
                }
                else
                {
                    array[i] = (char)(((short)array[i]) - 9);
                }
            }
            StringBuilder builder = new StringBuilder();
            for (int j = 0; j < length; j++)
            {
                builder.Append(string.Format("{0:X}", (int)array[j]));
            }
            return builder.ToString().Replace("A", "Z").Replace("B", "Y").Replace("C", "K").Replace("D", "P").Replace("E", "W").Replace("F", "U");
        }

        /// <summary>
        /// 在MD5的基础上再次加密
        /// </summary>
        /// <param name="plaintext"></param>
        /// <returns></returns>
        public static string DefineMD5Encrypt(string plaintext)
        {
            byte[] bytes = Encoding.Default.GetBytes(plaintext.Trim());
            MD5 md = new MD5CryptoServiceProvider();
            return PwdEncrypt(BitConverter.ToString(md.ComputeHash(bytes)).Replace("-", "@"));
        }

        /// <summary>
        /// 自定义加密
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <returns></returns>
        private static string PwdEncrypt(string ciphertext)
        {
            if (string.IsNullOrEmpty(ciphertext) || (ciphertext.Length < 10))
            {
                return ciphertext;
            }
            int length = ciphertext.Length;
            char[] chArray = ciphertext.ToCharArray();
            for (int i = 0; i < length; i++)
            {
                if ((i % 2) == 0)
                {
                    chArray[i] = (char)(((short)chArray[i]) + 1);
                }
                else
                {
                    chArray[i] = (char)(((short)chArray[i]) - 1);
                }
            }
            StringBuilder builder = new StringBuilder();
            for (int j = 0; j < length; j++)
            {
                builder.Append(chArray[j]);
            }
            return builder.ToString();
        }

    }
}
