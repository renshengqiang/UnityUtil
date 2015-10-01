
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
public class FileUtil
{
    /* 文件协议 */
#if UNITY_EDITOR
    public static string STREAM_FILE_PROTOCAL = "file:///";
#elif UNITY_IPHONE
            public static string STREAM_FILE_PROTOCAL_ = "file://";
#else
            public static string STREAM_FILE_PROTOCAL_ = "";
#endif

#if UNITY_EDITOR
    public static string PERSISTENT_FILE_PROTOCAL = "file:///";
#else
            public static string PERSISTENT_FILE_PROTOCAL_ = "file://";
#endif

    /// <summary>
    /// 文件是否存在
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool FileExist(string name)
    {
        return File.Exists(name);
    }

    /// <summary>
    /// 保存文件.
    /// </summary>
    /// <param name="content">Content.</param>
    /// <param name="filename">Filename.</param>
    public static void SaveToFile(string content, string dirname, string filename)
    {
        byte[] byte_xml = System.Text.Encoding.Default.GetBytes(content);
        SaveToFile(byte_xml, dirname, filename);
    }

    /// <summary>
    /// 保存文件
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="dirname"></param>
    /// <param name="filename"></param>
    public static void SaveToFile(byte[] bytes, string dirname, string filename)
    {
        if (!Directory.Exists(dirname))
        {
            Directory.CreateDirectory(dirname);
        }

        string filepath = dirname + filename;

        using (FileStream fileStream = new FileStream(filepath, File.Exists(filepath) ? FileMode.Truncate : FileMode.OpenOrCreate))
        {
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();
            fileStream.Dispose();
        }
    }

    /// <summary>
    /// 读取文件对应的byte数组
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    public static byte[] GetFileBytes(string filepath)
    {
        if (File.Exists(filepath) == false)
        {
            return new byte[0] { };
        }
        byte[] bytes = null;
        using (FileStream fileStream = new FileStream(filepath, FileMode.Open))
        {
            bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, int.Parse(fileStream.Length + ""));

            fileStream.Close();
            fileStream.Dispose();
        }
        return bytes;
    }

    /// <summary>
    /// 读取文件对应的字符串
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    public static string GetFileString(string filepath)
    {
        byte[] bytes = GetFileBytes(filepath);
        return System.Text.Encoding.ASCII.GetString(bytes);
    }

    /// <summary>
    /// 获取文件大小.
    /// </summary>
    /// <returns>The file size.</returns>
    /// <param name="filepath">Filepath.</param>
    public static long GetFileSize(string filepath)
    {
        FileInfo bundleinfo = new FileInfo(filepath);
        long bundlesize = bundleinfo.Length;
        return bundlesize;
    }

    /// <summary>
    /// 获取文件的 md5.
    /// </summary>
    /// <returns>The file md5.</returns>
    /// <param name="fileurl">Fileurl.</param>
    public static string GetFileMd5(string fileurl)
    {
        string strmd5 = "";
        if (File.Exists(fileurl) == true)
        {
            using (FileStream fileStream = new FileStream(fileurl, FileMode.Open))
            {
                byte[] bytes = new byte[fileStream.Length];
                strmd5 = GetBytesMd5(bytes);

                fileStream.Close();
                fileStream.Dispose();
            }
        }
        else
        {
#if LOG_DETAIL
            Debug.LogError(fileurl + "文件不存在");
#endif
        }
        return strmd5;
    }

    /// <summary>
    /// 获取字节的md5
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string GetBytesMd5(byte[] bytes)
    {
        string strmd5 = "";

        if (bytes != null)
        {
            MD5 md5 = MD5.Create();
            byte[] bMd5 = md5.ComputeHash(bytes);

            md5.Clear();

            for (int i = 0; i < bMd5.Length; i++)
            {
                strmd5 += bMd5[i].ToString("x").PadLeft(2, '0');
            }
        }

        return strmd5;
    }

    /// <summary>
    /// 重命名文件
    /// </summary>
    /// <param name="source">源文件名</param>
    /// <param name="destination">目标文件名</param>
    /// <param name="bDeleteIfExit">如果存在和目标文件名相同的文件名，是否删除</param>
    public static void RenameFile(string source, string destination, bool bDeleteIfExit = false)
    {
        if (File.Exists(destination) == true)
        {
            if (bDeleteIfExit)
            {
                File.Delete(destination);
            }
            else
            {
                return;
            }
        }
        File.Move(source, destination);
    }

    /// <summary>
    /// 拷贝一个文件
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    public static void CopyFile(string source, string destination)
    {
        if (File.Exists(destination) == true)
        {
            File.Delete(destination);
        }
        try
        {
            File.Copy(source, destination);
        }
        catch
        {
#if LOG_DETAIL
            Debug.Log("拷贝文件" + source + "发生错误，请检查是否存在");
#endif
        }
    }

    /// <summary>
    /// 获取一个文件夹下面的文件列表
    /// </summary>
    /// <returns>The file list.</returns>
    /// <param name="directory_list">Directory_list.</param>
    public static List<string> getFileList(string dirpath, string cont = "*.*")
    {
        List<string> file_list = new List<string>();

        /* 遍历所有文件 枚举所有依赖 */
        DirectoryInfo directory = new DirectoryInfo(dirpath);
        FileInfo[] dirs = directory.GetFiles(cont, SearchOption.AllDirectories);

        /* 遍历所有Prefab */
        foreach (FileInfo info in dirs)
        {
            /* 把遍历到的资源路径存起来 */
            file_list.Add(info.FullName);
        }

        return file_list;
    }

    /// <summary>
    /// 清空一个文件内容
    /// </summary>
    /// <param name="fileName"></param>
    public static void Clear(string fileName)
    {
        if (File.Exists(fileName) == true)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                fileStream.SetLength(0);
                fileStream.Close();
            }
        }
    }

    /// <summary>
    /// 清空一个文件夹
    /// </summary>
    /// <param name="dir"></param>
    public static void ClearDir(string dir)
    {
        try
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            Directory.CreateDirectory(dir);
        }
        catch
        {
#if LOG_DETAIL
            Debug.LogError("清空文件夹" + dir + "出错");
#endif
        }
    }

    /// <summary>
    /// 拷贝一个文件夹
    /// </summary>
    /// <param name="sourceDirName"></param>
    /// <param name="destDirName"></param>
    /// <param name="copySubDirs"></param>
    public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);
        DirectoryInfo[] dirs = dir.GetDirectories();

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        // If the destination directory doesn't exist, create it. 
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            if (File.Exists(temppath))
                File.Delete(temppath);
            file.MoveTo(temppath);
        }

        // If copying subdirectories, copy them and their contents to new location. 
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
            }
        }
    }

    /// <summary>
    /// 创建一个目录，如果原来的目录已经存在或者创建成功则返回true
    /// 否则返回 false
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool CreateDirectory(string path)
    {
        if (Directory.Exists(path)) return true;
        else
        {
            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
