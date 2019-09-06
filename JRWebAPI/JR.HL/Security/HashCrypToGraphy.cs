using System;
using System.Security.Cryptography;
using System.Text;
namespace JR.HL.Security
{
    public class HashCrypToGraphy
    {
        private HashProvider _provider;
        private bool m_IsAddSalt;
        private short m_SaltLength;
        private string m_SaltValue;
        private HashAlgorithm mhash;
        public bool IsAddSalt
        {
            get
            {
                return this.m_IsAddSalt;
            }
            set
            {
                this.m_IsAddSalt = value;
            }
        }
        public short SaltLength
        {
            get
            {
                return this.m_SaltLength;
            }
            set
            {
                this.m_SaltLength = value;
            }
        }
        public string SaltValue
        {
            get
            {
                return this.m_SaltValue;
            }
            set
            {
                this.m_SaltValue = value;
            }
        }
        public HashCrypToGraphy()
        {
            this._provider = HashProvider.SHA1;
            this.m_IsAddSalt = false;
            this.m_SaltValue = string.Empty;
            this.m_SaltLength = 8;
            this.mhash = this.SetHash();
        }
        public HashCrypToGraphy(HashProvider provider)
        {
            this._provider = HashProvider.SHA1;
            this.m_IsAddSalt = false;
            this.m_SaltValue = string.Empty;
            this.m_SaltLength = 8;
            this._provider = provider;
            this.mhash = this.SetHash();
        }
        public string CreateSalt()
        {
            byte[] array = new byte[8];
            new RNGCryptoServiceProvider().GetBytes(array);
            return Convert.ToBase64String(array);
        }
        public string HashString(string Value)
        {
            if (this.m_IsAddSalt)
            {
                if (this.m_SaltValue.Length == 0)
                {
                    this.m_SaltValue = this.CreateSalt();
                }
            }
            else
            {
                this.m_SaltValue = string.Empty;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(Value + this.m_SaltValue);
            byte[] inArray = this.mhash.ComputeHash(bytes);
            this.mhash.Clear();
            return Convert.ToBase64String(inArray);
        }
        public void Reset()
        {
            this.m_SaltValue = string.Empty;
            this._provider = HashProvider.SHA1;
            this.m_IsAddSalt = false;
            this.m_SaltLength = 8;
            this.mhash = null;
        }
        private HashAlgorithm SetHash()
        {
            HashProvider provider = this._provider;
            HashAlgorithm result;
            switch (provider)
            {
                case HashProvider.SHA1:
                    result = new SHA1CryptoServiceProvider();
                    return result;
                case HashProvider.MD5:
                    result = new MD5CryptoServiceProvider();
                    return result;
                case (HashProvider)3:
                    break;
                case HashProvider.SHA256:
                    result = new SHA256Managed();
                    return result;
                default:
                    if (provider == HashProvider.SHA384)
                    {
                        result = new SHA384Managed();
                        return result;
                    }
                    if (provider == HashProvider.SHA512)
                    {
                        result = new SHA512Managed();
                        return result;
                    }
                    break;
            }
            result = new SHA1CryptoServiceProvider();
            return result;
        }
    }

    public enum HashProvider
    {
        MD5 = 2,
        SHA1 = 1,
        SHA256 = 4,
        SHA384 = 8,
        SHA512 = 16
    }
}
