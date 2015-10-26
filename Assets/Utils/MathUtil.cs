using UnityEngine;
using System.Collections;

public class MathUtil : MonoBehaviour {

    /// <summary>
    ///  获取length对应的字符串
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GetSizeString(long length)
    {
        if (length < 1024)
        {
            return length + "B";
        }
        else if (length < 1048576)
        {
            float num = (float)length / 1024;
            return num.ToString("F1") + "K";
        }
        else
        {
            float num = (float)length / 1048576;
            return num.ToString("F1") + "M";
        }
    }
}
