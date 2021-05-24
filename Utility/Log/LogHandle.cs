using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using Utility;

namespace Utility
{

    public class LogHandle
    {

        private static string filePath;

        public static void WriteLog_Info(int Mark, string msg)
        {
            filePath = "";

            switch (Mark)
            {
                case 0:
                    filePath = "D:\\LogInfo\\System" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                case 1:
                    filePath = "D:\\LogInfo\\ScannerQR" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                case 2:
                    filePath = "D:\\LogInfo\\InterStackBuffer" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                case 3:
                    filePath = "D:\\LogInfo\\PickBoard" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                case 4:
                    filePath = "D:\\LogInfo\\SendCutPic" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                case 5:
                    filePath = "D:\\LogInfo\\LeftScanBoards" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                case 6:
                    filePath = "D:\\LogInfo\\RightScanBoards" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                case 7:
                    filePath = "D:\\LogInfo\\OnNotification" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                case 8:
                    filePath = "D:\\LogInfo\\View" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                case 9:
                    filePath = "D:\\LogInfo\\BasePlate" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                default:
                    break;
            }


            Globle.logMutex.WaitOne();

            msg = "[MESSAGE]" + msg;

            msg = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "    " + msg;
            FileHandle.WriteFile(msg, filePath);

            Globle.logMutex.ReleaseMutex();
        }


        public static void WriteLog_Error(int Mark, string msg)
        {
            filePath = "";

            switch (Mark)
            {
                case 0:
                    filePath = "D:\\LogError\\System" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                case 1:
                    filePath = "D:\\LogError\\ScannerQR" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                case 2:
                    filePath = "D:\\LogError\\InterStackBuffer" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                case 3:
                    filePath = "D:\\LogError\\PickBoard" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                case 4:
                    filePath = "D:\\LogError\\SendCutPic" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                case 5:
                    filePath = "D:\\LogError\\LeftScanBoards" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                case 6:
                    filePath = "D:\\LogError\\RightScanBoards" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                case 7:
                    filePath = "D:\\LogError\\OnNotification" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                case 8:
                    filePath = "D:\\LogError\\View" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                case 9:
                    filePath = "D:\\LogError\\BasePlate" + "-" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
                    break;
                default:
                    break;
            }


            Globle.logMutex.WaitOne();

            msg = "[MESSAGE]" + msg;

            msg = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "    " + msg;
            FileHandle.WriteFile(msg, filePath);

            Globle.logMutex.ReleaseMutex();
        }
    }
}
