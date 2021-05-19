using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;


namespace BasicDriver
{

    public class LogHandle
    {

        private static string filePath;
        private static string filePathService;

        /// <summary>
        /// 写log
        /// </summary>
        /// <param name="level">1:alarm 2:warning 3:normal</param> 
        /// <param name="msg">写入的信息</param> 
        public static void WriteLog(int level, string msg)
        {
            WriteLog(level, Globle.LOG_TYPE_MESSAGE, msg);
        }

        public static void WriteLog(int level, int type, string msg)
        {
            filePath = "D:\\Log\\" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";

           Globle.logMutex.WaitOne();

            switch (type)
            {
                case Globle.LOG_TYPE_MESSAGE:
                    msg = "[MESSAGE]" + msg;
                    break;
                case Globle.LOG_TYPE_ERROR:
                    msg = "[ERROR]  " + msg;
                    break;
                default:
                     msg = "[MESSAGE]" + msg;
                    break;
            }

            if (Globle.logLevel >= level)
            {
                msg = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "    " + msg;
                FileHandle.WriteFile(msg, filePath);
            }
           Globle.logMutex.ReleaseMutex();
        }


        /// <summary>
        /// 写log
        /// </summary>
        /// <param name="level">1:alarm 2:warning 3:normal</param> 
        /// <param name="msg">写入的信息</param> 
        public static void WriteLogService(int level, string msg)
        {
            filePathService = "D:\\Log\\Service\\" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";

            Globle.logServiceMutex.WaitOne();

            if (Globle.logLevel >= level)
            {
                msg = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "    " + msg;
                FileHandle.WriteFile(msg, filePathService);
            }
            Globle.logServiceMutex.ReleaseMutex();
        }

    }
}
