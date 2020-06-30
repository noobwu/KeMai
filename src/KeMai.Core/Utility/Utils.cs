using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using KeMai.Text;
namespace KeMai
{
    /// <summary>
    /// 
    /// </summary>
    public class Utils
    {


        /// <summary>
        /// Ĭ��ʱ��
        /// </summary>
        public static DateTime DefaultTime = new DateTime(1900, 1, 1);
        /// <summary>
        /// ����Ŀ¼
        /// </summary>
        /// <param name="name">����</param>
        /// <returns>�����Ƿ�ɹ�</returns>
        [DllImport("dbgHelp", SetLastError = true)]
        private static extern bool MakeSureDirectoryPathExists(string name);

        /// <summary>
        /// �����ļ���
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool CreateDir(string name)
        {
            return MakeSureDirectoryPathExists(name);
        }

        /// <summary>
        /// MD5����
        /// </summary>
        /// <param name="str">ԭʼ�ַ���</param>
        /// <returns>MD5���</returns>
        public static string MD5(string str)
        {
            return MD5(str, Encoding.Default);
        }


        /// <summary>
        /// MD5����
        /// </summary>
        /// <param name="str">ԭʼ�ַ���</param>
        /// <param name="encoding">�����ʽ</param>
        /// <returns>MD5���</returns>
        public static string MD5(string str, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            if (encoding == null) encoding = Encoding.Default;
            byte[] bytes = encoding.GetBytes(str);
            return MD5ByBytes(bytes);
        }

        /// <summary>
        /// MD5����
        /// </summary>
        /// <param name="bytes">�ֽ�����</param>
        /// <returns>MD5���</returns>
        public static string MD5ByBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return string.Empty;
            bytes = new MD5CryptoServiceProvider().ComputeHash(bytes);
            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += bytes[i].ToString("x").PadLeft(2, '0');
            }
            return ret;
        }
        /// <summary>
        /// MD5����
        /// </summary>
        /// <param name="stream">ԭʼ�ַ���</param>
        /// <returns>MD5���</returns>
        public static string MD5ByStream(Stream stream)
        {
            byte[] bytes = new MD5CryptoServiceProvider().ComputeHash(stream);
            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += bytes[i].ToString("x").PadLeft(2, '0');
            }
            return ret;
        }
        /// <summary>
        /// ��õ�ǰ����·��
        /// </summary>
        /// <param name="strPath">ָ����·��</param>
        /// <param name="serverUtility"></param>
        /// <returns>����·��</returns>
        public static string GetMapPath(string strPath)
        {
            if (string.IsNullOrEmpty(strPath))
            {
                return strPath;
            }
            else
            {
                if (strPath.IndexOf("/") != 0)
                {
                    if (Path.IsPathRooted(strPath))
                    {
                        return strPath;
                    }
                }
            }
            try
            {
                if (HttpContext.Current != null)
                {
                    strPath = strPath.Replace("\\", "/");
                    string strIndex = strPath.Substring(0, 1);
                    if (strIndex != "/" && strIndex != "~")
                    {
                        strPath = "~/" + strPath;
                    }
                    return HostingEnvironment.MapPath(strPath);
                    //return HttpContext.Current.Server.MapPath(strPath);
                }
                else //��web��������
                {
                    return GetAppPath(strPath);
                }
            }
            catch (Exception)
            {
                return GetAppPath(strPath);
            }

        }
        /// <summary>
        /// ��õ�ǰ����·��
        /// </summary>
        /// <param name="strPath">ָ����·��</param>
        /// <param name="serverUtility"></param>
        /// <returns>����·��</returns>
        public static string GetAppPath(string strPath)
        {
            if (string.IsNullOrEmpty(strPath)) return string.Empty;
            strPath = strPath.Replace("~/", "\\").Replace("/", "\\");
            if (strPath.StartsWith("\\"))
            {
                strPath = strPath.TrimStart('\\');
            }
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
        }


        /// <summary>
        /// �ж�ָ���ַ�����ָ���ַ��������е�λ��
        /// </summary>
        /// <param name="strSearch">�ַ���</param>
        /// <param name="stringArray">�ַ�������</param>
        /// <param name="caseInsensetive">�Ƿ����ִ�Сд, trueΪ������, falseΪ����</param>
        /// <returns>�ַ�����ָ���ַ��������е�λ��, �粻�����򷵻�-1</returns>
        public static int GetInArrayID(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (caseInsensetive)
                {
                    if (strSearch.ToLower() == stringArray[i].ToLower())
                    {
                        return i;
                    }
                }
                else
                {
                    if (strSearch == stringArray[i])
                    {
                        return i;
                    }
                }

            }
            return -1;
        }

        /// <summary>
        /// �ж�ָ���ַ�����ָ���ַ��������е�λ��
        /// </summary>
        /// <param name="strSearch">�ַ���</param>
        /// <param name="stringArray">�ַ�������</param>
        /// <returns>�ַ�����ָ���ַ��������е�λ��, �粻�����򷵻�-1</returns>		
        public static int GetInArrayID(string strSearch, string[] stringArray)
        {
            return GetInArrayID(strSearch, stringArray, true);
        }

        /// <summary>
        /// �ж�ָ���ַ����Ƿ�����ָ���ַ��������е�һ��Ԫ��
        /// </summary>
        /// <param name="strSearch">�ַ���</param>
        /// <param name="stringArray">�ַ�������</param>
        /// <param name="caseInsensetive">�Ƿ����ִ�Сд, trueΪ������, falseΪ����</param>
        /// <returns>�жϽ��</returns>
        public static bool InArray(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            return GetInArrayID(strSearch, stringArray, caseInsensetive) >= 0;
        }

        /// <summary>
        /// �ж�ָ���ַ����Ƿ�����ָ���ַ��������е�һ��Ԫ��
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <param name="stringarray">�ַ�������</param>
        /// <returns>�жϽ��</returns>
        public static bool InArray(string str, string[] stringarray)
        {
            return InArray(str, stringarray, false);
        }

        /// <summary>
        /// �ж�ָ���ַ����Ƿ�����ָ���ַ��������е�һ��Ԫ��
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <param name="stringarray">�ڲ��Զ��ŷָ�ʵ��ַ���</param>
        /// <returns>�жϽ��</returns>
        public static bool InArray(string str, string stringarray)
        {
            return InArray(str, SplitString(stringarray, ","), false);
        }

        /// <summary>
        /// �ж�ָ���ַ����Ƿ�����ָ���ַ��������е�һ��Ԫ��
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <param name="stringarray">�ڲ��Զ��ŷָ�ʵ��ַ���</param>
        /// <param name="strsplit">�ָ��ַ���</param>
        /// <returns>�жϽ��</returns>
        public static bool InArray(string str, string stringarray, string strsplit)
        {
            return InArray(str, SplitString(stringarray, strsplit), false);
        }

        /// <summary>
        /// �ж�ָ���ַ����Ƿ�����ָ���ַ��������е�һ��Ԫ��
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <param name="stringarray">�ڲ��Զ��ŷָ�ʵ��ַ���</param>
        /// <param name="strsplit">�ָ��ַ���</param>
        /// <param name="caseInsensetive">�Ƿ����ִ�Сд, trueΪ������, falseΪ����</param>
        /// <returns>�жϽ��</returns>
        public static bool InArray(string str, string stringarray, string strsplit, bool caseInsensetive)
        {
            return InArray(str, SplitString(stringarray, strsplit), caseInsensetive);
        }


        /// <summary>
        /// �ָ��ַ���
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!string.IsNullOrEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit) < 0)
                {
                    string[] tmp = { strContent };
                    return tmp;
                }
                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
            {
                return new string[0] { };
            }
        }

        /// <summary>
        /// �ָ��ַ���
        /// </summary>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, int count)
        {
            string[] result = new string[count];

            string[] splited = SplitString(strContent, strSplit);

            for (int i = 0; i < count; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// ����ַ���
        /// </summary>
        /// <param name="padString">����ַ���</param>
        /// <param name="count">���Ĵ���</param>
        public static string PadString(string padString, int count)
        {
            if (count <= 1) return padString;
            StringBuilder builder = new StringBuilder(1000);
            for (int i = 0; i < count; i++)
            {
                builder.Append(padString);
            }
            return builder.ToString();
        }

        /// <summary>
        /// �����ַ���������ÿ��Ԫ��Ϊ���ʵĴ�С
        /// ������С��minLengthʱ�����Ե�,-1Ϊ��������С����
        /// �����ȴ���maxLengthʱ��ȡ��ǰmaxLengthλ
        /// �����������nullԪ�أ��ᱻ���Ե�
        /// </summary>
        /// <param name="strArray"></param>
        /// <param name="minLength">����Ԫ����С����</param>
        /// <param name="maxLength">����Ԫ����󳤶�</param>
        /// <returns></returns>
        public static string[] PadStringArray(string[] strArray, int minLength, int maxLength)
        {
            if (minLength > maxLength)
            {
                int t = maxLength;
                maxLength = minLength;
                minLength = t;
            }

            int iMiniStringCount = 0;
            for (int i = 0; i < strArray.Length; i++)
            {
                if (minLength > -1 && strArray[i].Length < minLength)
                {
                    strArray[i] = null;
                    continue;
                }
                if (strArray[i].Length > maxLength)
                {
                    strArray[i] = strArray[i].Substring(0, maxLength);
                }
                iMiniStringCount++;
            }

            string[] result = new string[iMiniStringCount];
            for (int i = 0, j = 0; i < strArray.Length && j < result.Length; i++)
            {
                if (strArray[i] != null && strArray[i] != string.Empty)
                {
                    result[j] = strArray[i];
                    j++;
                }
            }


            return result;
        }

        /// <summary>
        /// ����Ƿ���SqlΣ���ַ�
        /// </summary>
        /// <param name="str">Ҫ�ж��ַ���</param>
        /// <returns>�жϽ��</returns>
        public static bool IsSafeSqlString(string str)
        {

            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        /// <summary>
        /// ����Ƿ���Σ�յĿ����������ӵ��ַ���
        /// </summary>
        /// <param name="str">Ҫ�ж��ַ���</param>
        /// <returns>�жϽ��</returns>
        public static bool IsSafeUserInfoString(string str)
        {
            return !Regex.IsMatch(str, @"^\s*$|^c:\\con\\con$|[%,\*" + "\"" + @"\s\t\<\>\&]|�ο�|^Guest");
        }

        /// <summary>
        /// �Ƿ�Ϊip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");

        }


        /// <summary>
        /// �Ƿ���Ip��Χ
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIPSect(string ip)
        {
            return Regex.IsMatch(ip,
                @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$");

        }



        /// <summary>
        /// ����ָ��IP�Ƿ���ָ����IP�������޶��ķ�Χ��, IP�����ڵ�IP��ַ����ʹ��*��ʾ��IP������, ����192.168.1.*
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="iparray"></param>
        /// <returns></returns>
        public static bool InIPArray(string ip, string[] iparray)
        {

            string[] userip = Utils.SplitString(ip, @".");
            for (int ipIndex = 0; ipIndex < iparray.Length; ipIndex++)
            {
                string[] tmpip = Utils.SplitString(iparray[ipIndex], @".");
                int r = 0;
                for (int i = 0; i < tmpip.Length; i++)
                {
                    if (tmpip[i] == "*")
                    {
                        return true;
                    }

                    if (userip.Length > i)
                    {
                        if (tmpip[i] == userip[i])
                        {
                            r++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }

                }
                if (r == 4)
                {
                    return true;
                }


            }
            return false;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetHashCode(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }
        /// <summary>
        /// string��ת��ΪDateTime��
        /// </summary>
        /// <param name="objValue">Ҫת�����ַ���</param>
        /// <returns>ת�����DateTime���ͽ��</returns>
        public static DateTime StrToDateTime(object objValue)
        {
            DateTime defValue = DefaultTime;
            if (objValue == null || string.IsNullOrEmpty(objValue.ToString())) return defValue;
            DateTime result = defValue;
            DateTime.TryParse(objValue.ToString(), out result);
            return result;
        }
        /// <summary>
        /// string��ת��ΪDateTime��
        /// </summary>
        /// <param name="objValue">Ҫת�����ַ���</param>
        /// <param name="defValue">ȱʡֵ</param>
        /// <returns>ת�����DateTime���ͽ��</returns>
        public static DateTime StrToDateTime(object objValue, DateTime defValue)
        {
            if (defValue == null || defValue == DateTime.MinValue) defValue = DefaultTime;
            if (objValue == null || string.IsNullOrEmpty(objValue.ToString())) return defValue;
            DateTime result = defValue;
            DateTime.TryParse(objValue.ToString(), out result);
            return result;
        }
        /// <summary>
        /// string��ת��ΪDateTime��
        /// </summary>
        /// <param name="strValue">Ҫת�����ַ���</param>
        /// <param name="defValue">ȱʡֵ</param>
        /// <returns>ת�����DateTime���ͽ��</returns>
        public static DateTime StrToDateTime(string strValue, DateTime defValue)
        {
            if (string.IsNullOrEmpty(strValue)) return defValue;
            DateTime result = defValue;
            DateTime.TryParse(strValue, out result);
            return result;
        }

        /// <summary>
        /// ��ȡ�Ǻ��ַ�����158****0617
        /// </summary>
        /// <param name="input">Դ�ַ���</param>
        /// <param name="leftLen">����ַ�������</param>
        /// <param name="rightLen">�ұ��ַ�������</param>
        /// <param name="asteriskLen">�Ǻ��ַ�����</param>
        /// <returns></returns>
        public static string GetAsteriskString(string input, int leftLen, int rightLen, int asteriskLen = 0)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            if ((input.Length <= (leftLen + rightLen)))
            {
                return input;
            }
            else
            {
                if (asteriskLen > 0)
                {
                    if (input.Length < (leftLen + rightLen + asteriskLen))
                    {
                        asteriskLen = (input.Length - leftLen - rightLen);
                    }
                }
                else
                {
                    asteriskLen = (input.Length - leftLen - rightLen);
                }
                return input.Substring(0, leftLen) + PadString("*", asteriskLen) + input.Substring(input.Length - asteriskLen);
            }

        }

        /// <summary>
        /// ת��ʱ��Ϊunixʱ���
        /// </summary>
        /// <returns></returns>
        public static long GetUnixTimestamp()
        {
            //
            //tick:10000000(tick)Ϊһ��
            DateTime originTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (DateTime.Now.ToUniversalTime().Ticks - originTime.Ticks) / TimeSpan.TicksPerSecond;
        }

        /// <summary>
        /// unixʱ���ת��ΪDateTime
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimestampToDateTime(long timestamp)
        {
            DateTime originTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            try
            {
                DateTime dateTime = originTime.AddSeconds(timestamp).ToLocalTime();
                return dateTime;
            }
            catch
            {
                return DataValueConstants.MIN_DATE;
            }
        }
        /// <summary>
        /// ���ַ�����ָ��λ�ý�ȡָ�����ȵ����ַ���
        /// </summary>
        /// <param name="str">ԭ�ַ���</param>
        /// <param name="startIndex">���ַ�������ʼλ��</param>
        /// <param name="length">���ַ����ĳ���</param>
        /// <returns>���ַ���</returns>
        public static string CutString(string str, int startIndex, int length)
        {
            if (startIndex >= 0)
            {
                if (length < 0)
                {
                    length = length * -1;
                    if (startIndex - length < 0)
                    {
                        length = startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        startIndex = startIndex - length;
                    }
                }


                if (startIndex > str.Length)
                {
                    return "";
                }


            }
            else
            {
                if (length < 0)
                {
                    return "";
                }
                else
                {
                    if (length + startIndex > 0)
                    {
                        length = length + startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        return "";
                    }
                }
            }

            if (str.Length - startIndex < length)
            {
                length = str.Length - startIndex;
            }

            return str.Substring(startIndex, length);
        }

        /// <summary>
        /// ���ַ�����ָ��λ�ÿ�ʼ��ȡ���ַ�����β���˷���
        /// </summary>
        /// <param name="str">ԭ�ַ���</param>
        /// <param name="startIndex">���ַ�������ʼλ��</param>
        /// <returns>���ַ���</returns>
        public static string CutString(string str, int startIndex)
        {
            return CutString(str, startIndex, str.Length);
        }



        /// <summary>
        ///  Md5ǩ��(sign = Md5(ԭ�ַ���&key=�̻���Կ).ToUpper())
        ///  Item1:ǩ����Ϣ
        ///  Item2��ǩ���Ĳ���
        /// </summary>
        /// <param name="dicSignParms"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Tuple<string, string> BuildSign(IDictionary<string, string> dicSignParms, string key)
        {
            StringBuilder signBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicSignParms)
            {
                if (FilerSignParam(temp))
                {
                    continue;
                }
                signBuilder.Append("&" + temp.Key + "=" + temp.Value);
            }
            signBuilder.Append("&key=" + key.Trim());
            string strSource = signBuilder.ToString().Trim('&');
            string strSign = MD5(strSource);
            return Tuple.Create(strSign, strSource);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool FilerSignParam(KeyValuePair<string, string> item)
        {
            if (string.IsNullOrEmpty(item.Value) || string.IsNullOrEmpty(item.Value) || item.Key.ToLower() == "charset" || item.Key.ToLower() == "sign")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// ��֤�ֻ������ʽ
        /// </summary>
        /// <param name="mobile">�ֻ�����</param>
        /// <returns></returns>
        public static bool IsValidMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return false;
            string pattern =@"^((13[0-9])|(14[5|7])|(15([0-3]|[5-9]))|(18[0,5-9]))\d{8}$"; //@"^(13|14|15|17|18|19)[0-9]\d{8}$";
            Regex regex = new Regex(pattern, RegexOptions.Compiled);
            return Regex.IsMatch(mobile, pattern);
        }

        /// <summary>
        /// Verifies that a string is in valid e-mail format
        /// </summary>
        /// <param name="email">Email to verify</param>
        /// <returns>true if the string is a valid e-mail address and false if it's not</returns>
        public static bool IsValidEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;
            email = email.Trim();
            return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");

        }
        /// <summary>
        /// ��֤�̶��绰
        /// </summary>
        /// <param name="phone">�̶��绰����</param>
        /// <returns></returns>
        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return false;
            string pattern = @"((\d{11})|(400\d{7})|(400-(\d{4}|\d{3})-(\d{3}|\d{4}))|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)";
            Regex regex = new Regex(pattern, RegexOptions.Compiled);
            return Regex.IsMatch(phone, pattern);
        }
    }
}
