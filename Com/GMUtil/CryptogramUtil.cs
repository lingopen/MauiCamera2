using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiCamera2.GMUtil
{
    public class CryptogramUtil
    {
        public static readonly bool StrongPassword = false; // 是否开启密码强度验证
        public static readonly string PasswordStrengthValidation = "^(?=.*\\\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[~@#$%\\\\*-\\\\+=:,\\\\\\\\?\\\\[\\\\]\\\\{}]).{6,16}$"; // 密码强度验证正则表达式
        public static readonly string PasswordStrengthValidationMsg = "密码必须包含大小写字母、数字和特殊字符的组合，长度在6-16之间"; // 密码强度验证提示
        public static readonly string CryptoType = "SM2"; // 加密类型
        public static readonly string PublicKey = "0484C7466D950E120E5ECE5DD85D0C90EAA85081A3A2BD7C57AE6DC822EFCCBD66620C67B0103FC8DD280E36C3B282977B722AAEC3C56518EDCEBAFB72C5A05312"; // 公钥
        public static readonly string PrivateKey = "8EDB615B1D48B8BE188FC0F18EC08A41DF50EA731FA28BF409E6552809E3A111"; // 私钥

        public static readonly string SM4_key = "0123456789abcdeffedcba9876543210";
        public static readonly string SM4_iv = "595298c7c6fd271f0402f804c33d3f66";

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Encrypt(string plainText)
        {
            if (CryptoType == CryptogramEnum.MD5.ToString())
            {
                return MD5Encryption.Encrypt(plainText);
            }
            else if (CryptoType == CryptogramEnum.SM2.ToString())
            {
                return SM2Encrypt(plainText);
            }
            else if (CryptoType == CryptogramEnum.SM4.ToString())
            {
                return SM4EncryptECB(plainText);
            }
            return plainText;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText)
        {
            if (CryptoType == CryptogramEnum.SM2.ToString())
            {
                return SM2Decrypt(cipherText);
            }
            else if (CryptoType == CryptogramEnum.SM4.ToString())
            {
                return SM4DecryptECB(cipherText);
            }
            return cipherText;
        }

        /// <summary>
        /// SM2加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string SM2Encrypt(string plainText)
        {
            return GMUtil.SM2Encrypt(PublicKey, plainText);
        }

        /// <summary>
        /// SM2解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string SM2Decrypt(string cipherText)
        {
            return GMUtil.SM2Decrypt(PrivateKey, cipherText);
        }

        /// <summary>
        /// SM4加密（ECB）
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string SM4EncryptECB(string plainText)
        {
            return GMUtil.SM4EncryptECB(SM4_key, plainText);
        }

        /// <summary>
        /// SM4解密（ECB）
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string SM4DecryptECB(string cipherText)
        {
            return GMUtil.SM4DecryptECB(SM4_key, cipherText);
        }

        /// <summary>
        /// SM4加密（CBC）
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string SM4EncryptCBC(string plainText)
        {
            return GMUtil.SM4EncryptCBC(SM4_key, SM4_iv, plainText);
        }

        /// <summary>
        /// SM4解密（CBC）
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string SM4DecryptCBC(string cipherText)
        {
            return GMUtil.SM4DecryptCBC(SM4_key, SM4_iv, cipherText);
        }
    }
}
