using System.Text.RegularExpressions;
using UnityEngine;
public class String
{
    public static byte[] GetStrBytes(string str)
    {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }

    public static string BytesToString(byte[] bytes)
    {
        char[] chars = new char[bytes.Length / sizeof(char)];
        System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }

    /// <summary>
    /// 将一个字符串限制在 len 个字符以下，有截取以 .. 结尾
    /// </summary>
    /// <param name="str">原来的字符串</param>
    /// <param name="len">截取的长度（英文原长，汉子减半）</param>
    /// <returns></returns>
    public static string Limit(string str, int len)
    {
        if (str == null || str.Length == 0 || len <= 0)
        {
            return string.Empty;
        }

        str = Regex.Replace(str, @"\p{Cs}", "");

        int l = str.Length;

        #region 计算长度
        int clen = 0;
        while (clen < len && clen < l)
        {
            //每遇到一个中文，则将目标长度减一。
            if ((int)str[clen] > 128) { len--; }
            clen++;
        }
        #endregion

        if (clen - len > 0) clen--;     // 如果刚好是最后遇到一个中文，则减去一个字符
        if (clen < l)
        {
            string a = str.Substring(0, clen);
            string b = a + "..";
            return b;
        }
        else
        {
            return str;
        }
    }

        /// <summary>
        /// Convert Color32 to hex string.
        /// </summary>
        public static string ColorToHexString(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        /// <summary>
        /// Convert hex string to Color32.
        /// </summary>
        public static Color32 HexStringToColor(string hex)
        {
            hex = hex.Replace("0x", "");            //in case the string is formatted 0xFFFFFF         
            hex = hex.Replace ("#", "");            //in case the string is formatted #FFFFFF         
            byte a = 255;                           //assume fully visible unless specified in hex         
            byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);         
            byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);         
            byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);         
            //Only use alpha if the string has enough characters         
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(6,2), System.Globalization.NumberStyles.HexNumber);
            }         
            return new Color32(r,g,b,a);
        }
}
