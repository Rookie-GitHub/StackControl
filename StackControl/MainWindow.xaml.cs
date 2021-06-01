using DTO.EDM;
using SerialCom.SerialLogic;
using StackControl.basic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using TwinCAT.Ads;
using Utility;

namespace StackControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region plc vars
        private AdsStream dataStream;
        private static TcAdsClient tcClient;
        private BinaryReader binRead = null;
        private int HeatBeatInt;
        private int LeftLoadInt;
        private int RightLoadInt;
        private int LeftPart_InWorkInt;
        private int RightPart_InWorkInt;
        private int ConCh1Int;
        private int ConCh2Int;
        private int ConCh3Int;
        private int ConCh4Int;
        private int ConCh5Int;
        private int PartDataReqInt;
        private int LeftScanDataReqInt;
        private int RightScanDataReqInt;
        private int PickPartFinishReqInt;
        private bool Heart = false;
        private bool HeartBroken = false;
        // plc handler
        private int PickPartFinishReqFB;

        #region HPS
        private int CH1PartDataFBInt;
        private int CH2PartDataFBInt;
        private int CH3PartDataFBInt;
        private int CH4PartDataFBInt;
        private int CH5PartDataFBInt;
        private int CH1PartDataFB;
        private int CH2PartDataFB;
        private int CH3PartDataFB;
        private int CH4PartDataFB;
        private int CH5PartDataFB;
        private int CH1PartIDFB;
        private int CH1BatchIDFB;
        private int CH1StackIDFB;
        private int CH1PatternFB;
        private int CH2PartIDFB;
        private int CH2BatchIDFB;
        private int CH2StackIDFB;
        private int CH2PatternFB;
        private int CH3PartIDFB;
        private int CH3BatchIDFB;
        private int CH3StackIDFB;
        private int CH3PatternFB;
        private int CH4PartIDFB;
        private int CH4BatchIDFB;
        private int CH4StackIDFB;
        private int CH4PatternFB;
        private int CH5PartIDFB;
        private int CH5BatchIDFB;
        private int CH5StackIDFB;
        private int CH5PatternFB;
        #endregion

        private int ReqLeftUPI;
        private int leftUPI;
        private int ReqRightUPI;
        private int RightUPI;

        private int LeftLoadReq;
        private int LeftLoadRel;
        private int RightLoadReq;
        private int RightLoadRel;
        private int LeftPartInWork;
        private int RightPartInWork;
        private int NewBatchIDReady;
        private int NewBatchID;
        private int BatchID;
        private int Length;
        private int tWidth;
        private int Thinkness;
        private int Path;
        private int PathID;
        private int ChangeBatch;
        private int PickPartChannel;
        private int StackID;
        private int Pattern;

        private int PartDataReq;
        private int CH1PartDataFBReset;
        private int CH2PartDataFBReset;
        private int CH3PartDataFBReset;
        private int CH4PartDataFBReset;
        private int CH5PartDataFBReset;

        private int LeftRedLightFB;
        private int LeftGreenLightFB;
        private int LeftYellowLightFB;

        private int RightRedLightFB;
        private int RightGreenLightFB;
        private int RightYellowLightFB;

        public int CH1PatternOKFB;
        public int CH2PatternOKFB;
        public int CH3PatternOKFB;
        public int CH4PatternOKFB;
        public int CH5PatternOKFB;

        private int ConCh1FB;
        private int ConCh2FB;
        private int ConCh3FB;
        private int ConCh4FB;
        private int ConCh5FB;
        //convery status
        private bool ConCh1Status;
        private bool ConCh2Status;
        private bool ConCh3Status;
        private bool ConCh4Status;
        private bool ConCh5Status;
        public string No1HPS;
        public string No1HPSPatrId;
        public string NO1HPSPlateID;
        public string No2HPS;
        public string No2HPSPatrId;
        public string NO2HPSPlateID;
        public string No3HPS;
        public string No3HPSPatrId;
        public string NO3HPSPlateID;
        public string No4HPS;
        public string No4HPSPatrId;
        public string NO4HPSPlateID;
        public string No5HPS;
        public string No5HPSPatrId;
        public string NO5HPSPlateID;
        #endregion

        #region logic vars
        SerialHelper SerialCom;
        public static MainWindow win;
        public List<string> CurrentLeftStackId = new List<string>();
        public List<string> CurrentRightStackId = new List<string>();
        public string CurrentFirstProcessedStack;
        public string CurrentSecProcessedStack;
        public string CurrentBatchId;
        public string NextBatchId;
        public string CurrentScannerBatchId;
        public bool ChangeBatchAlarm;
        public bool LeftChangeBatchAlarm;
        public bool RightChangeBatchAlarm;
        public string LeftChStackId;
        public string RightChStackId;
        public string tmpStackId;
        public bool ChangingBatch;
        public bool BatchChangingAtLastone;
        public bool NextStackNewBatch;
        public bool LastStackAtCurrentBatch;
        public int StackPartNums;
        public string CurrentStackId;
        public string CurrentBatch;
        public string currentBatch;
        public bool ForceChangeBatch;
        public string lprebatchid = string.Empty;
        public string rprebatchid = string.Empty;

        public ObservableCollection<Alarm> Alarmlist = new ObservableCollection<Alarm>();

        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        /// <summary>
        /// 扫描枪扫描的堆垛编号
        /// </summary>
        public string StackId;
        int _PartID = -1;
        bool StartSignal = false;
        #endregion

        #region InTimer Control
        int inTimer_PickBoard = 0;
        int inTimer_PickBoardFinish = 0;
        int inTimer_LeftScanBoards = 0;
        int inTimer_RightScanBoards = 0;
        int inTimer_SendCutPic = 0;
        #endregion

        #region windows Loaded
        public MainWindow()
        {
            InitializeComponent();
            win = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.alarm.ItemsSource = Alarmlist;
            CurrentBatch = config.AppSettings.Settings["CurrentBatchID"].Value;
            showTime();
            ScanQRCode();
        }

        private bool StartSystem()
        {
            try
            {
                ViewLoad();
                FirstConfigPLC();
                PickBoardTimer();
                PickBoardFinishTimer();
                LeftScanBoardsTimer();
                RightScanBoardsTimer();
                SendCutPicTimer();
                StartSignal = true;

                return true;
            }
            catch (Exception ex)
            {
                LogHandle.WriteLog_Error((int)LogMark.系统, ex.Message);
                return false;
            }
        }

        #endregion

        #region PLC
        #region ConnPLC
        public void ConnPLC()
        {
            tcClient = new TcAdsClient();
            try
            {
                tcClient.Connect("5.79.111.254.1.1", 801);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Cannot connect with PLC(ConnPLC),please restart the app");
            }
        }

        #endregion

        #region HeatBeatPLC
        public void HeatBeatPLC()
        {
            DispatcherTimer heartbeat = new DispatcherTimer();
            heartbeat.Tick += new EventHandler(HeartBeatCon);
            heartbeat.Interval = new TimeSpan(0, 0, 0, 1, 0);
            heartbeat.Start();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeartBeatCon(object sender, EventArgs e)
        {
            try
            {
                tcClient.WriteAny(HeatBeatInt, !Heart);
                Heart = !Heart;
                if (HeartBroken)
                {
                    HeartBroken = false;
                }
            }
            catch (Exception ex)
            {
                if (!HeartBroken)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Alarmlist.Add(new Alarm() { Message = ex.Message, Timestamp = DateTime.Now });

                    });
                    HeartBroken = true;
                }
            }
        }
        #endregion

        #region ConfigPLcTags
        /// <summary>
        /// 加载PLC Tags
        /// </summary>
        public void ConfigPLCTags()
        {
            try
            {
                #region  notification              
                LeftLoadInt = tcClient.AddDeviceNotification(".LeftLoad_Req", dataStream, 0, 1, AdsTransMode.OnChange, 100, 0, null);
                RightLoadInt = tcClient.AddDeviceNotification(".RightLoad_Req", dataStream, 1, 1, AdsTransMode.OnChange, 100, 0, null);

                LeftPart_InWorkInt = tcClient.AddDeviceNotification(".LeftPart_InWork", dataStream, 2, 1, AdsTransMode.OnChange, 100, 0, null);
                RightPart_InWorkInt = tcClient.AddDeviceNotification(".RightPart_InWork", dataStream, 3, 1, AdsTransMode.OnChange, 100, 0, null);

                //ConCh1Int = tcClient.AddDeviceNotification(".ConCh1_Status", dataStream, 4, 1, AdsTransMode.OnChange, 100, 0, null);
                //ConCh2Int = tcClient.AddDeviceNotification(".ConCh2_Status", dataStream, 5, 1, AdsTransMode.OnChange, 100, 0, null);
                //ConCh3Int = tcClient.AddDeviceNotification(".ConCh3_Status", dataStream, 6, 1, AdsTransMode.OnChange, 100, 0, null);
                //ConCh4Int = tcClient.AddDeviceNotification(".ConCh4_Status", dataStream, 7, 1, AdsTransMode.OnChange, 100, 0, null);
                //ConCh5Int = tcClient.AddDeviceNotification(".ConCh5_Status", dataStream, 8, 1, AdsTransMode.OnChange, 100, 0, null);

                //PartDataReqInt = tcClient.AddDeviceNotification(".PartData_Req", dataStream, 9, 1, AdsTransMode.OnChange, 100, 0, null);

                //CH1PartDataFBInt = tcClient.AddDeviceNotification(".CH1_PartData_FB", dataStream, 10, 1, AdsTransMode.OnChange, 100, 0, null);
                //CH2PartDataFBInt = tcClient.AddDeviceNotification(".CH2_PartData_FB", dataStream, 11, 1, AdsTransMode.OnChange, 100, 0, null);
                //CH3PartDataFBInt = tcClient.AddDeviceNotification(".CH3_PartData_FB", dataStream, 12, 1, AdsTransMode.OnChange, 100, 0, null);
                //CH4PartDataFBInt = tcClient.AddDeviceNotification(".CH4_PartData_FB", dataStream, 13, 1, AdsTransMode.OnChange, 100, 0, null);
                //CH5PartDataFBInt = tcClient.AddDeviceNotification(".CH5_PartData_FB", dataStream, 14, 1, AdsTransMode.OnChange, 100, 0, null);

                //LeftScanDataReqInt = tcClient.AddDeviceNotification(".LeftScanData_Req", dataStream, 15, 1, AdsTransMode.OnChange, 100, 0, null);
                //RightScanDataReqInt = tcClient.AddDeviceNotification(".RightScanData_Req", dataStream, 16, 1, AdsTransMode.OnChange, 100, 0, null);

                //PickPartFinishReqInt = tcClient.AddDeviceNotification(".PickPartFinish", dataStream, 17, 1, AdsTransMode.OnChange, 100, 0, null);

                tcClient.AdsNotification += new AdsNotificationEventHandler(tcClient_OnNotification);
                #endregion 

                #region handler
                HeatBeatInt = tcClient.CreateVariableHandle(".HeartBeat");
                //plc -----> stack control
                CH1PartDataFB = tcClient.CreateVariableHandle(".CH1_PartData_FB");//DataReq
                CH1PartIDFB = tcClient.CreateVariableHandle(".CH1_PartID_FB");//Path => PartID
                CH1BatchIDFB = tcClient.CreateVariableHandle(".CH1_BatchID_FB");
                CH1StackIDFB = tcClient.CreateVariableHandle(".CH1_StackID_FB");
                CH1PatternFB = tcClient.CreateVariableHandle(".CH1_Pattern_FB");
                CH2PartDataFB = tcClient.CreateVariableHandle(".CH2_PartData_FB");//DataReq
                CH2PartIDFB = tcClient.CreateVariableHandle(".CH2_PartID_FB");//Path => PartID
                CH2BatchIDFB = tcClient.CreateVariableHandle(".CH2_BatchID_FB");
                CH2StackIDFB = tcClient.CreateVariableHandle(".CH2_StackID_FB");
                CH2PatternFB = tcClient.CreateVariableHandle(".CH2_Pattern_FB");
                CH3PartDataFB = tcClient.CreateVariableHandle(".CH3_PartData_FB");//DataReq
                CH3PartIDFB = tcClient.CreateVariableHandle(".CH3_PartID_FB");//Path => PartID
                CH3BatchIDFB = tcClient.CreateVariableHandle(".CH3_BatchID_FB");
                CH3StackIDFB = tcClient.CreateVariableHandle(".CH3_StackID_FB");
                CH3PatternFB = tcClient.CreateVariableHandle(".CH3_Pattern_FB");
                CH4PartDataFB = tcClient.CreateVariableHandle(".CH4_PartData_FB");//DataReq
                CH4PartIDFB = tcClient.CreateVariableHandle(".CH4_PartID_FB");//Path => PartID
                CH4BatchIDFB = tcClient.CreateVariableHandle(".CH4_BatchID_FB");
                CH4StackIDFB = tcClient.CreateVariableHandle(".CH4_StackID_FB");
                CH4PatternFB = tcClient.CreateVariableHandle(".CH4_Pattern_FB");
                CH5PartDataFB = tcClient.CreateVariableHandle(".CH5_PartData_FB");//DataReq
                CH5PartIDFB = tcClient.CreateVariableHandle(".CH5_PartID_FB");//Path => PartID
                CH5BatchIDFB = tcClient.CreateVariableHandle(".CH5_BatchID_FB");
                CH5StackIDFB = tcClient.CreateVariableHandle(".CH5_StackID_FB");
                CH5PatternFB = tcClient.CreateVariableHandle(".CH5_Pattern_FB");
                ReqLeftUPI = tcClient.CreateVariableHandle(".LeftScanData_Req");
                leftUPI = tcClient.CreateVariableHandle(".LeftUPI");
                ReqRightUPI = tcClient.CreateVariableHandle(".RightScanData_Req");
                RightUPI = tcClient.CreateVariableHandle(".RightUPI");
                PickPartFinishReqFB = tcClient.CreateVariableHandle(".PickPartFinish");

                ConCh1FB = tcClient.CreateVariableHandle(".ConCh1_Status");
                ConCh2FB = tcClient.CreateVariableHandle(".ConCh2_Status");
                ConCh3FB = tcClient.CreateVariableHandle(".ConCh3_Status");
                ConCh4FB = tcClient.CreateVariableHandle(".ConCh4_Status");
                ConCh5FB = tcClient.CreateVariableHandle(".ConCh5_Status");
                // stack control ----> plc
                LeftLoadReq = tcClient.CreateVariableHandle(".LeftLoad_Req");
                LeftLoadRel = tcClient.CreateVariableHandle(".LeftLoad_Rel");
                RightLoadReq = tcClient.CreateVariableHandle(".RightLoad_Req");
                RightLoadRel = tcClient.CreateVariableHandle(".RightLoad_Rel");
                LeftPartInWork = tcClient.CreateVariableHandle(".LeftPart_InWork");
                RightPartInWork = tcClient.CreateVariableHandle(".RightPart_InWork");
                NewBatchIDReady = tcClient.CreateVariableHandle(".NewBatchIDReady");
                NewBatchID = tcClient.CreateVariableHandle(".NewBatchID");
                BatchID = tcClient.CreateVariableHandle(".BatchID");
                Length = tcClient.CreateVariableHandle(".Length");
                tWidth = tcClient.CreateVariableHandle(".Width");
                Thinkness = tcClient.CreateVariableHandle(".Thinkness");
                Path = tcClient.CreateVariableHandle(".Path");
                PathID = tcClient.CreateVariableHandle(".PartID");
                ChangeBatch = tcClient.CreateVariableHandle(".Change_Batch");
                PickPartChannel = tcClient.CreateVariableHandle(".PickPart_Channel");
                StackID = tcClient.CreateVariableHandle(".StackID");
                Pattern = tcClient.CreateVariableHandle(".Pattern");
                //Board Request 
                PartDataReq = tcClient.CreateVariableHandle(".PartData_Req");

                CH1PartDataFBReset = tcClient.CreateVariableHandle(".CH1_PartData_FB");
                CH2PartDataFBReset = tcClient.CreateVariableHandle(".CH2_PartData_FB");
                CH3PartDataFBReset = tcClient.CreateVariableHandle(".CH3_PartData_FB");
                CH4PartDataFBReset = tcClient.CreateVariableHandle(".CH4_PartData_FB");
                CH5PartDataFBReset = tcClient.CreateVariableHandle(".CH5_PartData_FB");

                LeftRedLightFB = tcClient.CreateVariableHandle(".LeftRedLight");
                LeftGreenLightFB = tcClient.CreateVariableHandle(".LeftGreenLight");
                LeftYellowLightFB = tcClient.CreateVariableHandle(".LeftYellowLight");

                RightRedLightFB = tcClient.CreateVariableHandle(".RightRedLight");
                RightGreenLightFB = tcClient.CreateVariableHandle(".RightGreenLight");
                RightYellowLightFB = tcClient.CreateVariableHandle(".RightYellowLight");

                CH1PatternOKFB = tcClient.CreateVariableHandle(".CH1PatternOKFB");
                CH2PatternOKFB = tcClient.CreateVariableHandle(".CH2PatternOKFB");
                CH3PatternOKFB = tcClient.CreateVariableHandle(".CH3PatternOKFB");
                CH4PatternOKFB = tcClient.CreateVariableHandle(".CH4PatternOKFB");
                CH5PatternOKFB = tcClient.CreateVariableHandle(".CH5PatternOKFB");

                #endregion

                Console.WriteLine("notification done!");
            }
            catch (AdsErrorException adsEx)
            {
                if (adsEx.ToString().Contains("Timeout"))
                {
                    //log
                }
                else
                {
                    tcClient.Dispose();
                    MessageBox.Show("ConfigPLCTags Failed , plrease restart the app");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "-notification err!");
                App.Current.Dispatcher.Invoke(() =>
                {
                    Alarmlist.Add(new Alarm() { Message = ex.Message, Timestamp = DateTime.Now });

                });
            }
        }
        #endregion

        #region FirstConfigPLC
        public void FirstConfigPLC()
        {
            dataStream = new AdsStream(18); /* stream for storing the ADS state of the PLC */
            binRead = new BinaryReader(dataStream); /* reader for reading the state */

            #region code
            try
            {
                ConnPLC();
                ConfigPLCTags();
                HeatBeatPLC();
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot connect with PLC(FirstConfigPLC)");
                App.Current.Dispatcher.Invoke(() =>
                {
                    Alarmlist.Add(new Alarm() { Message = "Cannot connect with PLC(FirstConfigPLC)", Timestamp = DateTime.Now });

                });

                FirstConfigPLC();
            }
            #endregion
        }
        #endregion

        #region tcClient_OnNotification 订阅事件,获取Plc value 变化
        public void tcClient_OnNotification(object sender, AdsNotificationEventArgs e)
        {
            try
            {
                e.DataStream.Position = e.Offset;

                #region plc请求上料
                if (e.NotificationHandle == LeftLoadInt)
                {
                    if (binRead.ReadBoolean())
                    {
                        Req_PutOnThePlates((int)PlatesChannel.左侧);
                    }
                }
                else if (e.NotificationHandle == RightLoadInt)
                {
                    if (binRead.ReadBoolean())
                    {
                        Req_PutOnThePlates((int)PlatesChannel.右侧);
                    }
                }
                #endregion

                #region 进入抓板区
                else if (e.NotificationHandle == LeftPart_InWorkInt)
                {
                    if (binRead.ReadBoolean())
                    {
                        EnterThePickPlateArea((int)PlatesChannel.左侧);
                    }
                }
                else if (e.NotificationHandle == RightPart_InWorkInt)
                {
                    if (binRead.ReadBoolean())
                    {
                        EnterThePickPlateArea((int)PlatesChannel.右侧);
                    }
                }
                #endregion
            }
            catch (AdsErrorException adsEx)
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ": tcClient_OnNotification Error!");
                App.Current.Dispatcher.Invoke(() =>
                {
                    Alarmlist.Add(new Alarm() { Message = ex.Message + ": tcClient_OnNotification Error!", Timestamp = DateTime.Now });

                });
                LogHandle.WriteLog_Error((int)LogMark.PLC订阅, ex.Message);
            }
        }
        #endregion
        #endregion

        #region 共享文件夹相关
        #region Ping
        /// <summary>
        /// 测试连接量
        /// </summary>
        /// <param name="remoteHost"></param>
        /// <returns></returns>
        public static bool Ping(string remoteHost)
        {
            bool Flag = false;
            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = @"ping -n 1 " + remoteHost;
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (proc.HasExited == false)
                {
                    proc.WaitForExit(500);
                }
                string pingResult = proc.StandardOutput.ReadToEnd();
                if (pingResult.IndexOf("(0% loss)") != -1 || pingResult.IndexOf("(0% 丢失)") != -1)
                {
                    Flag = true;
                }
                proc.StandardOutput.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "--ping " + remoteHost + "失败！！");

            }
            finally
            {
                try
                {
                    proc.Close();
                    proc.Dispose();
                }
                catch
                {
                }
            }
            return Flag;
        }
        #endregion

        #region 连接共享文件夹
        /// <summary>
        /// 连接共享文件夹
        /// </summary>
        /// <param name="remoteHost"></param>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public static bool Connect(string remoteHost, string userName, string passWord)
        {
            if (!Ping(remoteHost))
            {
                return false;
            }
            bool Flag = true;
            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = @"net use \\" + remoteHost + " " + "/DELETE"; // + ">NUL";
                proc.StandardInput.WriteLine(dosLine);
                dosLine = @"net use \\" + remoteHost + " " + passWord + " " + " /user:" + userName + ">NUL";
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (proc.HasExited == false)
                {
                    proc.WaitForExit(1000);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                if (errormsg != "")
                {
                    Flag = false;
                    Console.WriteLine(errormsg);
                }
                proc.StandardError.Close();
            }
            catch (Exception ex)
            {
                Flag = false;
                Console.WriteLine(DateTime.Now + "--远程共享文件夹失败！！" + ex.Message);
            }
            finally
            {
                try
                {
                    proc.Close();
                    proc.Dispose();
                }
                catch
                {
                }
            }
            return Flag;
        }

        #endregion

        #region 断开远程文件夹 合并在连接时先断开
        /// <summary>
        /// 断开远程共享文件夹
        /// </summary>
        /// <param name="remoteHost"></param>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public static bool Disconnect(string remoteHost, string userName, string passWord)
        {
            if (!Ping(remoteHost))
            {
                return false;
            }
            bool Flag = true;
            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = @"net use \\" + remoteHost + " " + "/DELETE" + ">NUL";
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (proc.HasExited == false)
                {
                    proc.WaitForExit(1000);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                if (errormsg != "")
                {
                    Flag = false;
                    Console.WriteLine(errormsg);
                }
                proc.StandardError.Close();
            }
            catch (Exception ex)
            {
                Flag = false;
                Console.WriteLine(DateTime.Now + "--远程共享文件夹失败！！" + ex.Message);
            }
            finally
            {
                try
                {
                    proc.Close();
                    proc.Dispose();
                }
                catch
                {
                }
            }
            return Flag;
        }
        #endregion
        #endregion

        #region ScanQRCode
        #region ScanQRCode Connect
        /// <summary>
        /// ScanQRCode
        /// </summary>
        public void ScanQRCode()
        {
            try
            {
                SerialCom = new SerialHelper("COM2"/*串口号*/, 115200/*波特率*/, 0/*校验位*/, 8/*数据位*/, 1/*停止位*/, serialPortHelper_ReceivedDataEvent);
                LogHandle.WriteLog_Info((int)LogMark.板垛扫码, "手持扫码串口连接建立成功！");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                LogHandle.WriteLog_Error((int)LogMark.板垛扫码, ex.ToString());
            }

        }
        #endregion

        #region ScanQRCode Event
        /// <summary>
        /// ScanQRCode Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void serialPortHelper_ReceivedDataEvent(object sender, SerialPortHelper.SerialPortRecvEventArgs args)
        {
            try
            {
                var Recvmessage = (Encoding.Default.GetString(args.RecvData, 0, args.RecvDataLength)).Replace('\r', ' ').Trim();
                StackId = Recvmessage;
                Console.WriteLine(Recvmessage);
                LogHandle.WriteLog_Info((int)LogMark.板垛扫码, "FeedBack Scanner value ：" + Recvmessage);
                /*记录到数据库中，ScanQrCodeRecord status = 1 （扫描完成）*/
                using (EDM Db = new EDM())
                {
                    ScanQrCodeRecord model = new ScanQrCodeRecord()
                    {
                        StackId = Recvmessage,
                        Status = (int)ScanQRCodeStatus.扫描完成,
                        ScanTime = DateTime.Now
                    };

                    Db.ScanQrCodeRecord.Add(model);
                    Db.SaveChanges();
                }

                LogHandle.WriteLog_Info((int)LogMark.板垛扫码, "Beginning Analysis Data ：" + Recvmessage);
                Analysis(Recvmessage);
                LogHandle.WriteLog_Info((int)LogMark.板垛扫码, "End Analysis Data ：" + Recvmessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(" ScanQrCode :" + ex.ToString());
                //记录日志
                LogHandle.WriteLog_Error((int)LogMark.板垛扫码, "serialPortHelper_ReceivedDataEvent ：" + ex.ToString());

            }
        }
        #endregion

        #region Analysis Data and Save
        /// <summary>
        /// 解析并存储堆垛/板料信息
        /// </summary>
        /// <param name="scanRes"></param>
        public void Analysis(string scanRes)
        {
            string Getcsv_IP = config.AppSettings.Settings["Getcsv_IP"].Value;
            string Use = config.AppSettings.Settings["Getcsv_UsePwd"].Value.Split(',')[0];
            string Pwd = config.AppSettings.Settings["Getcsv_UsePwd"].Value.Split(',')[1];
            string Path = config.AppSettings.Settings["csv_Path"].Value;
            string Goal = config.AppSettings.Settings["csv_Goal"].Value;
            string FileNme = scanRes + ".csv";
            string FileNme_Parts = scanRes.Split('_')[0] + "_parts.csv";
            string FileNme_All = scanRes.Split('_')[0] + "_all.csv";
            List<Stack_table> Stack_tables = new List<Stack_table>();
            List<Board_table> board_Tables = new List<Board_table>();
            DateTime dateTime = DateTime.Now;
            try
            {
                if (Ping(Getcsv_IP))
                {
                    if (Connect(Getcsv_IP, Use, Pwd))
                    {
                        using (EDM Db = new EDM())
                        {
                            var ChekcUnMarkStack = Db.StackInfo_table.Where(s => s.Status == (int)StackStatus.数据解析并存储完成).Count();
                            if (ChekcUnMarkStack > 0)
                            {
                                Console.WriteLine(DateTime.Now + "上料失败！上料口存在未标记左右的堆垛，StackId :" + StackId);
                                LogHandle.WriteLog_Info((int)LogMark.板垛扫码, "上料失败！上料口存在未标记左右的堆垛StackId :" + StackId + "，请完成操作后重新扫描。");
                                return;
                            }
                            var CheckComStack = Db.ScanQrCodeRecord.Where(s => s.StackId == scanRes && s.Status != (int)ScanQRCodeStatus.扫描完成).Count();
                            if (CheckComStack > 0)
                            {
                                Console.WriteLine(DateTime.Now + "该板垛已完成扫码且数据解析完成，请勿重复扫码，避免造成数据错乱 StackId :" + StackId);
                                LogHandle.WriteLog_Info((int)LogMark.板垛扫码, "该板垛已完成扫码且数据解析完成，请勿重复扫码，避免造成数据错乱 StackId: " + StackId);
                                return;
                            }
                            //存储板料信息
                            if (File.Exists(@"\\" + Getcsv_IP + @"\" + Path + @"\" + FileNme))
                            {
                                //存储堆垛信息
                                List<string> list_Stack = File.ReadAllLines(@"\\" + Getcsv_IP + @"\" + Path + @"\" + FileNme, Encoding.Default).ToList();  //(@"C:\Users\VM-TEMP\Documents\Oppein2020\Project\062210526016020-A_1.csv", Encoding.Default).ToList(); 

                                if (list_Stack.Count <= 0)
                                {
                                    MessageBox.Show("The infomation of the Stack : " + scanRes + " was Analysis failed,Please check the MDB file!");
                                    LogHandle.WriteLog_Info((int)LogMark.板垛扫码, "The infomation of the Stack: " + scanRes + " was Analysis failed, Please check the MDB file!");
                                    return;
                                }

                                //The Batch of Stack
                                var BatchOfStack = list_Stack[0].Split(',')[0].Split('_')[0];

                                Stack_tables.Clear();

                                list_Stack.ForEach(s =>
                                {
                                    Stack_table stackModel = new Stack_table
                                    {
                                        StackId = scanRes,
                                        Batch = BatchOfStack,
                                        PartID = s.Split(',')[1],
                                        Len = Convert.ToInt32(s.Split(',')[2]),
                                        Width = Convert.ToInt32(s.Split(',')[3]),
                                        Thin = Convert.ToInt32(s.Split(',')[4]),
                                        Material = s.Split(',')[5],
                                        Pos = Convert.ToInt32(s.Split(',')[7]),
                                        Pattern = Convert.ToInt32(s.Split(',')[8]),
                                        Map = s.Split(',')[9],
                                        Status = (int)StackStatus.数据解析并存储完成,
                                        About = (int)PlatesChannel.无,
                                        DateTime = dateTime,
                                        UpdateTime = dateTime
                                    };
                                    Stack_tables.Add(stackModel);
                                });

                                Db.Stack_table.AddRange(Stack_tables);

                                /*Stack_Table(DetailInfo) Inserted Finish , then Insert the StackInfo to the table of StackInfo*/
                                StackInfo_table stackInfo_Table = new StackInfo_table
                                {
                                    Batch = BatchOfStack,
                                    StackId = scanRes,
                                    Status = (int)StackStatus.数据解析并存储完成,
                                    About = (int)PlatesChannel.无,
                                    PlateAmount = list_Stack.Count(),
                                    CurrentCount = 0,
                                    StartTime = DateTime.Now,
                                    EndTime = null
                                };

                                Db.StackInfo_table.Add(stackInfo_Table);

                                /*Update ScanQrCode Status = 2 means Data Analysis and Save Complete*/
                                var Scan_table = Db.ScanQrCodeRecord.OrderByDescending(s => s.ScanTime).FirstOrDefault(s => s.StackId == scanRes);

                                Scan_table.Status = (int)ScanQRCodeStatus.数据解析并存储完成;


                                File.Move(@"\\" + Getcsv_IP + @"\" + Path + @"\" + FileNme, @"\\" + Getcsv_IP + @"\" + Goal + @"\" + FileNme);
                            }
                            //存储板料信息
                            if (File.Exists(@"\\" + Getcsv_IP + @"\" + Path + @"\" + FileNme_Parts))
                            {
                                List<string> list_Parts = File.ReadAllLines(@"\\" + Getcsv_IP + @"\" + Path + @"\" + FileNme_Parts, Encoding.Default).ToList(); //(@"E:\Homag\Project\Oppen-wuxi-2020\CSV\062210511001020-A_parts.csv", Encoding.Default).ToList(); //

                                if (list_Parts.Count <= 0)
                                {
                                    MessageBox.Show("The infomation of the Stack :" + scanRes + " Parts was analysis failed ,Please check the MDB file!");
                                    //Log
                                    LogHandle.WriteLog_Info((int)LogMark.板垛扫码, "The infomation of the Stack :" + scanRes + " Parts was analysis failed ,Please check the MDB file!");
                                    return;
                                }

                                board_Tables.Clear();

                                list_Parts.ForEach(s =>
                                {
                                    Board_table board_Table = new Board_table
                                    {
                                        BatchId = s.Split(',')[0],
                                        Upi = s.Split(',')[2],
                                        Array = Convert.ToInt32(s.Split(',')[1]),
                                        Status = (int)BoardScanStatus.未扫描,
                                    };

                                    board_Tables.Add(board_Table);
                                });

                                Db.Board_table.AddRange(board_Tables);

                                File.Move(@"\\" + Getcsv_IP + @"\" + Path + @"\" + FileNme_Parts, @"\\" + Getcsv_IP + @"\" + Goal + @"\" + FileNme_Parts);
                            }

                            //存储批次数量信息
                            if (File.Exists(@"\\" + Getcsv_IP + @"\" + Path + @"\" + FileNme_All))
                            {
                                string[] BatchInfo = File.ReadAllLines(@"\\" + Getcsv_IP + @"\" + Path + @"\" + FileNme_All, Encoding.Default);//(@"E:\Homag\Project\Oppen-wuxi-2020\CSV\062210511001020-A_all.csv", Encoding.Default); //

                                if (BatchInfo.Length <= 0)
                                {
                                    MessageBox.Show("The infomation of the Stack :" + scanRes + " Batch was analysis failed ,Please check the MDB file!");
                                    //Log
                                    LogHandle.WriteLog_Info((int)LogMark.板垛扫码, "The infomation of the Stack :" + scanRes + " Batch was analysis failed ,Please check the MDB file!");
                                    return;
                                }

                                Batch_table batch_Table = new Batch_table
                                {
                                    BatchId = BatchInfo[0].Split(',')[0],
                                    AllNum = Convert.ToInt32(BatchInfo[0].Split(',')[1]),
                                    ComNum = 0,
                                    Status = (int)BatchStatus.已上线,
                                    DateTime = dateTime
                                };

                                Db.Batch_table.Add(batch_Table);

                                File.Move(@"\\" + Getcsv_IP + @"\" + Path + @"\" + FileNme_All, @"\\" + Getcsv_IP + @"\" + Goal + @"\" + FileNme_All);
                            }
                            Db.SaveChanges();
                        }
                    }
                    else
                    {
                        //
                        Console.WriteLine("Can not connect server: " + Getcsv_IP);
                        LogHandle.WriteLog_Info((int)LogMark.板垛扫码, "Can not connect server: " + Getcsv_IP);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHandle.WriteLog_Error((int)LogMark.板垛扫码, ex.Message);

                Console.WriteLine(ex.Message);
            }
        }
        #endregion
        #endregion

        #region About View

        #region View Refresh
        /// <summary>
        /// View Refresh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void Window_ContentRendered(object sender, EventArgs e)
        //{
        //    view();
        //}
        #endregion

        #region View Load
        public void ViewLoad()
        {
            try
            {
                using (EDM Db = new EDM())
                {
                    //Get Info about StackInfo which Status != 3
                    var StackInfos = Db.StackInfo_table.Where(s => s.Status != 3).ToList();

                    var _StackId = string.Empty;

                    StackInfos.ForEach(s =>
                    {
                        _StackId = s.StackId;
                        view(_StackId);
                    });
                    LogHandle.WriteLog_Info((int)LogMark.View, "ViewLoad sucess");
                }
            }
            catch (Exception ex)
            {
                LogHandle.WriteLog_Error((int)LogMark.View, ex.Message);
            }
        }
        #endregion

        #region Load View Data
        /// <summary>
        /// Load View Data
        /// </summary>
        /// <param name="StackId"></param>
        public void view(string StackId = "")
        {
            int Pos = -1;
            int Status = -1;
            int About = -1;
            using (EDM Db = new EDM())
            {
                var stack_table = Db.Stack_table.Where(s => s.StackId == StackId).ToList();

                if (stack_table.Count <= 0)
                {
                    return;
                }

                stack_table.ForEach(s =>
                {
                    Pos = s.Pos;
                    Status = (int)s.Status;
                    About = (int)s.About;
                    upe_Background(Pos, Status, About);
                });

                switch (About)
                {
                    case 1:
                        //更新堆垛id
                        st1.Text = stack_table.FirstOrDefault().StackId;
                        //更新批次id
                        st3.Text = stack_table.FirstOrDefault().Batch;
                        //更新进料状态
                        st5.Fill = new SolidColorBrush(Colors.Green);
                        break;
                    case 2:
                        st2.Text = stack_table.FirstOrDefault().StackId;
                        st4.Text = stack_table.FirstOrDefault().Batch;
                        st6.Fill = new SolidColorBrush(Colors.Green);
                        break;
                }
            }
        }
        #endregion

        #region change the Background color
        /// <summary>
        /// change the Backgroud color
        /// </summary>
        /// <param name="Pos">序号</param>
        /// <param name="Status">状态</param>
        /// <param name="About">左右</param>
        public void upe_Background(int Pos, int Status, int About)
        {
            switch (Pos)
            {
                case 40:
                    upe_Background(About == 1 ? A1 : B1, Status);
                    break;
                case 39:
                    upe_Background(About == 1 ? A2 : B2, Status);
                    break;
                case 38:
                    upe_Background(About == 1 ? A3 : B3, Status);
                    break;
                case 37:
                    upe_Background(About == 1 ? A4 : B4, Status);
                    break;
                case 36:
                    upe_Background(About == 1 ? A5 : B5, Status);
                    break;
                case 35:
                    upe_Background(About == 1 ? A6 : B6, Status);
                    break;
                case 34:
                    upe_Background(About == 1 ? A7 : B7, Status);
                    break;
                case 33:
                    upe_Background(About == 1 ? A8 : B8, Status);
                    break;
                case 32:
                    upe_Background(About == 1 ? A9 : B9, Status);
                    break;
                case 31:
                    upe_Background(About == 1 ? A10 : B10, Status);
                    break;
                case 30:
                    upe_Background(About == 1 ? A11 : B11, Status);
                    break;
                case 29:
                    upe_Background(About == 1 ? A12 : B12, Status);
                    break;
                case 28:
                    upe_Background(About == 1 ? A13 : B13, Status);
                    break;
                case 27:
                    upe_Background(About == 1 ? A14 : B14, Status);
                    break;
                case 26:
                    upe_Background(About == 1 ? A15 : B15, Status);
                    break;
                case 25:
                    upe_Background(About == 1 ? A16 : B16, Status);
                    break;
                case 24:
                    upe_Background(About == 1 ? A17 : B17, Status);
                    break;
                case 23:
                    upe_Background(About == 1 ? A18 : B18, Status);
                    break;
                case 22:
                    upe_Background(About == 1 ? A19 : B19, Status);
                    break;
                case 21:
                    upe_Background(About == 1 ? A20 : B20, Status);
                    break;
                case 20:
                    upe_Background(About == 1 ? A21 : B21, Status);
                    break;
                case 19:
                    upe_Background(About == 1 ? A22 : B22, Status);
                    break;
                case 18:
                    upe_Background(About == 1 ? A23 : B23, Status);
                    break;
                case 17:
                    upe_Background(About == 1 ? A24 : B24, Status);
                    break;
                case 16:
                    upe_Background(About == 1 ? A25 : B25, Status);
                    break;
                case 15:
                    upe_Background(About == 1 ? A26 : B26, Status);
                    break;
                case 14:
                    upe_Background(About == 1 ? A27 : B27, Status);
                    break;
                case 13:
                    upe_Background(About == 1 ? A28 : B28, Status);
                    break;
                case 12:
                    upe_Background(About == 1 ? A29 : B29, Status);
                    break;
                case 11:
                    upe_Background(About == 1 ? A30 : B30, Status);
                    break;
                case 10:
                    upe_Background(About == 1 ? A31 : B31, Status);
                    break;
                case 9:
                    upe_Background(About == 1 ? A32 : B32, Status);
                    break;
                case 8:
                    upe_Background(About == 1 ? A33 : B33, Status);
                    break;
                case 7:
                    upe_Background(About == 1 ? A34 : B34, Status);
                    break;
                case 6:
                    upe_Background(About == 1 ? A35 : B35, Status);
                    break;
                case 5:
                    upe_Background(About == 1 ? A36 : B36, Status);
                    break;
                case 4:
                    upe_Background(About == 1 ? A37 : B37, Status);
                    break;
                case 3:
                    upe_Background(About == 1 ? A38 : B38, Status);
                    break;
                case 2:
                    upe_Background(About == 1 ? A39 : B39, Status);
                    break;
                case 1:
                    upe_Background(About == 1 ? A40 : B40, Status);
                    break;
            }
        }
        #endregion

        #region change the background color
        /// <summary>
        /// change the background color
        /// </summary>
        /// <param name="border">标签名称</param>
        /// <param name="status">状态</param>
        public void upe_Background(Border border, int status)
        {
            if (status == 1)
                border.Dispatcher.Invoke(new Action(() => { border.Background = new SolidColorBrush(Colors.Yellow); }));
            else if (status == 2)
                border.Dispatcher.Invoke(new Action(() => { border.Background = new SolidColorBrush(Colors.Green); }));
            else if (status == 3)
                border.Dispatcher.Invoke(new Action(() => { border.Background = new SolidColorBrush(Colors.White); }));

            if (border == A40 && status == 3)
            {
                //更新堆垛id
                st1.Dispatcher.Invoke(new Action(() => { st1.Text = String.Empty; }));
                //更新批次id
                st3.Dispatcher.Invoke(new Action(() => { st3.Text = String.Empty; }));
                //更新进料状态
                st5.Dispatcher.Invoke(new Action(() => { st5.Fill = new SolidColorBrush(Colors.Yellow); }));
            }
            if (border == B40 && status == 3)
            {
                //更新堆垛id
                st2.Dispatcher.Invoke(new Action(() => { st2.Text = String.Empty; }));
                //更新批次id
                st4.Dispatcher.Invoke(new Action(() => { st4.Text = String.Empty; }));
                //更新进料状态
                st6.Dispatcher.Invoke(new Action(() => { st6.Fill = new SolidColorBrush(Colors.Yellow); }));
            }
        }
        #endregion

        #region Clear Alarm
        /// <summary>
        /// Clear Alarm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void alarm_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.alarm.ItemsSource = null;
            this.alarm.Items.Clear();
        }
        #endregion

        #endregion

        #region Window_closing
        /// <summary>
        /// Closing Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            config.AppSettings.Settings["CurrentBatchID"].Value = CurrentBatch;
            config.Save();

        }
        #endregion

        #region Window_Closed
        /// <summary>
        /// Closed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                if (StartSignal)
                {
                    tcClient.DeleteDeviceNotification(LeftLoadInt);
                    tcClient.DeleteDeviceNotification(ReqLeftUPI);
                    tcClient.DeleteDeviceNotification(RightLoadInt);
                    tcClient.DeleteDeviceNotification(RightUPI);
                    tcClient.DeleteVariableHandle(LeftPartInWork);
                    tcClient.DeleteVariableHandle(RightPartInWork);
                    tcClient.DeleteVariableHandle(LeftLoadReq);
                    tcClient.DeleteVariableHandle(RightLoadReq);
                    tcClient.DeleteVariableHandle(CH1PartIDFB);
                    tcClient.DeleteVariableHandle(CH1BatchIDFB);
                    tcClient.DeleteVariableHandle(CH2PartIDFB);
                    tcClient.DeleteVariableHandle(CH2BatchIDFB);
                    tcClient.DeleteVariableHandle(CH3PartIDFB);
                    tcClient.DeleteVariableHandle(CH3BatchIDFB);
                    tcClient.DeleteVariableHandle(CH4PartIDFB);
                    tcClient.DeleteVariableHandle(CH4BatchIDFB);
                    tcClient.DeleteVariableHandle(CH5PartIDFB);
                    tcClient.DeleteVariableHandle(CH5BatchIDFB);
                    tcClient.DeleteVariableHandle(leftUPI);
                    tcClient.DeleteVariableHandle(RightUPI);
                    tcClient.DeleteVariableHandle(LeftLoadRel);
                    tcClient.DeleteVariableHandle(RightLoadRel);
                    tcClient.DeleteVariableHandle(NewBatchIDReady);
                    tcClient.DeleteVariableHandle(NewBatchID);
                    tcClient.DeleteVariableHandle(BatchID);
                    tcClient.DeleteVariableHandle(Length);
                    tcClient.DeleteVariableHandle(tWidth);
                    tcClient.DeleteVariableHandle(Thinkness);
                    tcClient.DeleteVariableHandle(Path);
                    tcClient.DeleteVariableHandle(PathID);
                    tcClient.DeleteVariableHandle(ChangeBatch);
                    tcClient.DeleteVariableHandle(PickPartChannel);
                    tcClient.DeleteVariableHandle(HeatBeatInt);
                    tcClient.DeleteVariableHandle(PartDataReq);
                    tcClient.DeleteVariableHandle(CH1PartDataFBReset);
                    tcClient.DeleteVariableHandle(CH2PartDataFBReset);
                    tcClient.DeleteVariableHandle(CH3PartDataFBReset);
                    tcClient.DeleteVariableHandle(CH4PartDataFBReset);
                    tcClient.DeleteVariableHandle(CH5PartDataFBReset);
                    tcClient.DeleteVariableHandle(PickPartFinishReqFB);
                    tcClient.DeleteVariableHandle(LeftRedLightFB);
                    tcClient.DeleteVariableHandle(LeftGreenLightFB);
                    tcClient.DeleteVariableHandle(LeftYellowLightFB);
                    tcClient.DeleteVariableHandle(RightRedLightFB);
                    tcClient.DeleteVariableHandle(RightGreenLightFB);
                    tcClient.DeleteVariableHandle(RightYellowLightFB);
                    tcClient.DeleteVariableHandle(CH1PatternOKFB);
                    tcClient.DeleteVariableHandle(CH2PatternOKFB);
                    tcClient.DeleteVariableHandle(CH3PatternOKFB);
                    tcClient.DeleteVariableHandle(CH4PatternOKFB);
                    tcClient.DeleteVariableHandle(CH5PatternOKFB);

                    tcClient.Dispose();
                }
                Environment.Exit(0);
            }
            catch (AdsErrorException ex)
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ":closing error");
                LogHandle.WriteLog_Error((int)LogMark.系统, ex.Message);
            }

        }
        #endregion

        #region show current time
        private void showTime()
        {
            //DispatcherTimer Ui线程 可以直接处理页面
            DispatcherTimer showTimer = new DispatcherTimer();
            showTimer.Tick += new EventHandler(ShowCurrentTime);
            showTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            showTimer.Start();
        }
        private void ShowCurrentTime(object sender, EventArgs e)
        {
            this.timeTb.Text = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
        }
        #endregion

        #region Timer

        #region Pick Board Timer

        #region PickBoardTimer
        /// <summary>
        /// Pick Board Timer
        /// </summary>
        private void PickBoardTimer()
        {
            // Timers 非UI线程 
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;//设置是否执行Elapsed事件
            timer.Elapsed += new ElapsedEventHandler(PickBoardEvent);//绑定Elapsed事件
            timer.Interval = 1000;//设置时间间隔
        }

        /// <summary>
        /// Pick Board Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PickBoardEvent(object sender, EventArgs e)
        {
            //Timer lock control the Repeat
            if (Interlocked.Exchange(ref inTimer_PickBoard, 1) == 0)
            {
                try
                {
                    var Req = (bool)tcClient.ReadAny(PartDataReq, typeof(bool));
                    if (!Req)
                    {
                        return;
                    }
                    LogHandle.WriteLog_Info((int)LogMark.抓板, "reveice req");
                    using (EDM Db = new EDM())
                    {
                        //
                        var BoardStatus = Db.Stack_table.Where(s => s.Status == (int)StackStatus.单板开始抓板);

                        if (BoardStatus.Count() > 0)
                        {
                            Console.WriteLine("PickBoardEvent :There is a board just picking !");
                            //无数据
                            LogHandle.WriteLog_Info((int)LogMark.抓板, "PickBoardEvent :There is a board just picking !");
                            return;
                        }

                        int About = -1;
                        string currentStackId = "";
                        int currentStackNum = -1;
                        int AmountStackNum = -1;
                        bool BasePlate = false;

                        var PickFinishStack = Db.StackInfo_table.FirstOrDefault(s => s.Status == (int)StackStatus.整垛抓板完成);

                        if (PickFinishStack != null)
                        {
                            currentStackId = PickFinishStack.StackId;
                            PickFinishStack.Status = (int)StackStatus.垫板抓取完成;
                            Db.SaveChanges();

                            tcClient.WriteAny(Path, (short)88);
                            tcClient.WriteAny(PartDataReq, false);
                            LogHandle.WriteLog_Info((int)LogMark.BasePlate, "currentStackId : " + currentStackId + " BasePlate PickCommand send sucess");
                            return;
                        }

                        var PickingStack = Db.StackInfo_table.FirstOrDefault(s => s.Status == (int)StackStatus.整垛开始抓板);

                        if (PickingStack != null)
                        {
                            currentStackId = PickingStack.StackId;
                            About = (int)PickingStack.About;
                            currentStackNum = PickingStack.CurrentCount;
                            AmountStackNum = PickingStack.PlateAmount;
                            currentBatch = PickingStack.Batch;
                        }
                        else
                        {
                            var WaitPickStack = Db.StackInfo_table.OrderBy(s => s.StartTime).FirstOrDefault(s => s.Status == (int)StackStatus.抓板区);

                            if (WaitPickStack != null)
                            {
                                currentStackId = WaitPickStack.StackId;
                                About = (int)WaitPickStack.About;
                                currentStackNum = WaitPickStack.CurrentCount;
                                AmountStackNum = WaitPickStack.PlateAmount;
                                currentBatch = WaitPickStack.Batch;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(currentStackId) && About != -1)
                        {
                            //send message about new btach 
                            if (!Equals(CurrentBatch, currentBatch) && !string.IsNullOrWhiteSpace(currentBatch) && !string.IsNullOrWhiteSpace(CurrentBatch))
                            {
                                tcClient.WriteAny(NewBatchID, currentBatch, new int[] { 30 });
                                tcClient.WriteAny(NewBatchIDReady, true);
                            }
                            //Update CurrentBatch value
                            CurrentBatch = currentBatch;

                            #region orgion
                            //if (PickPart_Channel == 99)
                            //{
                            //    SendData(About, currentBatch, currentStackId);
                            //}
                            //else if (PickPart_Channel == 1 || PickPart_Channel == 2)
                            //{
                            #region /* orgion */ according by stackstatus(3)
                            ////less than 40
                            //if (AmountStackNum < 40 && currentStackNum == AmountStackNum)
                            //{
                            //    //The last plate
                            //    BasePlate = true;
                            //}
                            ////else if (AmountStackNum > 40 && currentStackNum % 40 == 0 && currentStackNum > 0)
                            ////{
                            ////    //The last plate
                            ////    BasePlate = true;
                            ////}
                            //else if (currentStackNum == AmountStackNum)
                            //{
                            //    //The last plate
                            //    BasePlate = true;
                            //}
                            //else
                            //{
                            //    //The last plate
                            //    BasePlate = false;
                            //}
                            ////is the last plate
                            //if (BasePlate)
                            //{
                            //    tcClient.WriteAny(Path, (short)88);
                            //    tcClient.WriteAny(PartDataReq, false);
                            //}
                            //else
                            #endregion
                            //}
                            #endregion

                            SendData(About, currentBatch, currentStackId);
                        }
                        else
                        {
                            //there is dont have currentBatch , Close the request 
                            //tcClient.WriteAny(PartDataReq, false);
                            Console.WriteLine("There is no board to pick！");
                            LogHandle.WriteLog_Info((int)LogMark.抓板, "There is no board to pick！");
                        }
                    }

                    LogHandle.WriteLog_Info((int)LogMark.抓板, "Send Data");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    LogHandle.WriteLog_Info((int)LogMark.抓板, ex.Message);
                }
                finally
                {
                    Interlocked.Exchange(ref inTimer_PickBoard, 0);
                }
            }
        }
        #endregion

        #region 反馈抓板请求数据
        /// <summary>
        /// 反馈抓板请求数据
        /// </summary>
        /// <param name="About"></param>
        /// <param name="channel1"></param>
        /// <param name="channel2"></param>
        /// <param name="channel3"></param>
        /// <param name="channel4"></param>
        /// <param name="channel5"></param>
        private void SendData(int About, string Batch, string StackId)
        {
            ConCh1Status = (bool)tcClient.ReadAny(ConCh1FB, typeof(bool));
            ConCh2Status = (bool)tcClient.ReadAny(ConCh2FB, typeof(bool));
            ConCh3Status = (bool)tcClient.ReadAny(ConCh3FB, typeof(bool));
            ConCh4Status = (bool)tcClient.ReadAny(ConCh4FB, typeof(bool));
            ConCh5Status = (bool)tcClient.ReadAny(ConCh5FB, typeof(bool));

            var channel = -1;
            //有当前批次,给plc通道号
            if (ConCh1Status)
                channel = 1;
            else if (ConCh2Status)
                channel = 2;
            else if (ConCh3Status)
                channel = 3;
            else if (ConCh4Status)
                channel = 4;
            else if (ConCh5Status)
                channel = 5;
            else
                channel = -1;//there is no channel need The Board .

            if (channel != -1)
                SendDataToPlc(channel, About, Batch, StackId);
            else
            {
                Console.WriteLine(DateTime.Now + "SendData ：There is no channel to send board");
                MessageBox.Show("There is no channel to send board");
                App.Current.Dispatcher.Invoke(() =>
                {
                    Alarmlist.Add(new Alarm() { Message = "SendData ：There is no channel to send board", Timestamp = DateTime.Now });

                });
            }
        }
        #endregion

        #region 发送板料信息到plc
        /// <summary>
        /// 发送板料信息到plc
        /// </summary>
        /// <param name="hps">输送线通道号1,2,3,4,5</param>
        /// <param name="about">plates stack channel</param>
        /// <param name="Batch">Batch</param>
        /// <param name="StackId">StackId</param>
        private void SendDataToPlc(int hps, int about, string Batch, string StackId)
        {
            try
            {
                using (EDM Db = new EDM())
                {
                    var BoardInfo = Db.Stack_table.Where(s => s.Batch == Batch && s.StackId == StackId && s.Status == (int)StackStatus.抓板区).OrderByDescending(s => s.Pos).FirstOrDefault();

                    if (BoardInfo == null)
                    {
                        //无数据
                        LogHandle.WriteLog_Info((int)LogMark.抓板, "SendDataToPlc : There is no Board to pick ");
                        return;
                    }

                    var PartId = BoardInfo.PartID;

                    var map = BoardInfo.Map;

                    string splitBatchIDtring = map.ToString().Split('.')[0].Substring(0, map.ToString().Split('.')[0].LastIndexOf('-'));

                    string splitPattern = map.ToString().Split('.')[0].Split('-')[map.ToString().Split('.')[0].Split('-').Length - 1];
                    //批次编号
                    tcClient.WriteAny(BatchID, splitBatchIDtring, new int[] { 30 });
                    //板料长度
                    tcClient.WriteAny(Length, (short)Convert.ToInt16(BoardInfo.Len));
                    //板料宽度
                    tcClient.WriteAny(tWidth, (short)Convert.ToInt16(BoardInfo.Width));
                    //板料厚度
                    tcClient.WriteAny(Thinkness, (short)Convert.ToInt16(BoardInfo.Thin));
                    //路径分配编号                                                        
                    tcClient.WriteAny(Path, (short)Convert.ToInt16(hps));
                    _PartID = Convert.ToInt16(BoardInfo.PartID);
                    //板料ID编号
                    tcClient.WriteAny(PathID, (short)_PartID);
                    //抓板通道号
                    tcClient.WriteAny(PickPartChannel, (short)about);
                    //堆垛编号
                    tcClient.WriteAny(StackID, StackId, new int[] { 30 });
                    //锯切图号
                    tcClient.WriteAny(Pattern, splitPattern, new int[] { 30 });

                    BoardInfo.Status = (int)StackStatus.单板开始抓板;
                    BoardInfo.UpdateTime = DateTime.Now;

                    Db.SaveChanges();
                    //清空请求
                    tcClient.WriteAny(PartDataReq, false);

                    upe_Background(BoardInfo.Pos, (int)BoardInfo.Status, (int)BoardInfo.About);

                    LogHandle.WriteLog_Info((int)LogMark.抓板, "Batch : " + Batch + ", StackId :" + StackId + ", PartId :" + PartId + ", HPS :" + hps);
                    #region StackIndo's Status update by trigger when the board picked finish 
                    //StackIndo's Status update by trigger when the board picked finish 
                    //var StackInfo_table = Db.StackInfo_table.Where(s => s.Batch == Batch && s.StackId == StackId && s.Status == (int)StackStatus.抓板区).FirstOrDefault();

                    //if (StackInfo_table != null)
                    //{
                    //    StackInfo_table.Status = (int)StackStatus.整垛开始抓板;
                    //}
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogHandle.WriteLog_Error((int)LogMark.抓板, "SendDataToPlc :" + ex.Message);
                App.Current.Dispatcher.Invoke(() =>
                {
                    Alarmlist.Add(new Alarm() { Message = ex.Message, Timestamp = DateTime.Now });
                });
            }
        }
        #endregion

        #endregion

        #region Pick Board Finish Timer
        /// <summary>
        /// Pick Board Finish Timer
        /// </summary>
        private void PickBoardFinishTimer()
        {
            // Timers 非UI线程 
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;//设置是否执行Elapsed事件
            timer.Elapsed += new ElapsedEventHandler(PickBoardFinishEvent);//绑定Elapsed事件
            timer.Interval = 1000;//设置时间间隔
        }

        /// <summary>
        /// Pick Board Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PickBoardFinishEvent(object sender, EventArgs e)
        {
            //Timer lock control the Repeat
            if (Interlocked.Exchange(ref inTimer_PickBoardFinish, 1) == 0)
            {
                int about = -1;
                //int BatchSurplusCount = -1;
                int StackSurplusCount = -1;
                try
                {
                    var Fin = (bool)tcClient.ReadAny(PickPartFinishReqFB, typeof(bool));
                    if (!Fin)
                    {
                        return;
                    }
                    LogHandle.WriteLog_Info((int)LogMark.抓板, "reveice Finish");

                    using (EDM Db = new EDM())
                    {
                        //找到 stack_table 中 status = 2(开始抓板) Board 
                        var BoardInfo = Db.Stack_table.Where(s => s.Status == (int)StackStatus.单板开始抓板).OrderByDescending(s => s.Pos).FirstOrDefault();
                        if (BoardInfo == null)
                        {
                            LogHandle.WriteLog_Info((int)LogMark.抓板, "There is no Board Status is 2");
                            return;
                        }
                        about = (int)BoardInfo.About;
                        int _PartID = Convert.ToInt16(BoardInfo.PartID);
                        string _StackID = BoardInfo.StackId;
                        string _BatchId = BoardInfo.Batch;

                        BoardInfo.Status = (int)StackStatus.单板抓板完成;
                        BoardInfo.UpdateTime = DateTime.Now;

                        //后续修改为存储过程
                        var stackInfo_table = Db.StackInfo_table.FirstOrDefault(s => s.StackId == _StackID);

                        stackInfo_table.CurrentCount += 1;

                        var batchinfo_table = Db.Batch_table.FirstOrDefault(s => s.BatchId == _BatchId);

                        batchinfo_table.ComNum += 1;

                        Db.SaveChanges();

                        tcClient.WriteAny(PickPartChannel, (short)0);
                        tcClient.WriteAny(PickPartFinishReqFB, false);

                        LogHandle.WriteLog_Info((int)LogMark.抓板, "RE Finish");

                        StackSurplusCount = stackInfo_table.PlateAmount - stackInfo_table.CurrentCount;

                        switch (about)
                        {
                            case 1:
                                this.st1Count.Dispatcher.Invoke(new Action(() => { this.st1Count.Text = StackSurplusCount.ToString(); }));
                                break;
                            case 2:
                                this.st2Count.Dispatcher.Invoke(new Action(() => { this.st2Count.Text = StackSurplusCount.ToString(); }));
                                break;
                        }
                        upe_Background(BoardInfo.Pos, 3, (int)BoardInfo.About);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    LogHandle.WriteLog_Error((int)LogMark.抓板, "PickBoardFinishEvent :" + ex.Message); ;
                }
                finally
                {
                    Interlocked.Exchange(ref inTimer_PickBoardFinish, 0);
                }
            }
        }
        #endregion

        #region ScanBoards

        #region Left Scanner
        /// <summary>
        /// Left ScanBoards Timer
        /// </summary>
        private void LeftScanBoardsTimer()
        {
            // Timers 非UI线程 
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;//设置是否执行Elapsed事件
            timer.Elapsed += new ElapsedEventHandler(LeftScanBoardsEvent);//绑定Elapsed事件
            timer.Interval = 1000;//设置时间间隔
        }

        /// <summary>
        /// Left ScanBoards Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LeftScanBoardsEvent(object sender, EventArgs e)
        {
            try
            {
                if (Interlocked.Exchange(ref inTimer_LeftScanBoards, 1) == 0)
                {
                    ScanBoardsHandle((int)ScanStation.左侧下层出料口);
                    Interlocked.Exchange(ref inTimer_LeftScanBoards, 0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("LeftScanBoardsEvent " + ex.Message);
                LogHandle.WriteLog_Error((int)LogMark.左侧下层扫码, ex.Message);
                Interlocked.Exchange(ref inTimer_LeftScanBoards, 0);
            }
        }

        #endregion

        #region Right Scanner
        /// <summary>
        /// Right ScanBoards Timer
        /// </summary>
        private void RightScanBoardsTimer()
        {
            // Timers 非UI线程 
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;//设置是否执行Elapsed事件
            timer.Elapsed += new ElapsedEventHandler(RightScanBoardsEvent);//绑定Elapsed事件
            timer.Interval = 1000;//设置时间间隔
        }

        /// <summary>
        /// Right ScanBoards Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RightScanBoardsEvent(object sender, EventArgs e)
        {
            try
            {
                if (Interlocked.Exchange(ref inTimer_RightScanBoards, 1) == 0)
                {
                    ScanBoardsHandle((int)ScanStation.右侧上层出料口);
                    Interlocked.Exchange(ref inTimer_RightScanBoards, 0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RightScanBoardsEvent " + ex.Message);
                LogHandle.WriteLog_Error((int)LogMark.右侧上层扫码, ex.Message);
                Interlocked.Exchange(ref inTimer_RightScanBoards, 0);
            }
        }

        #endregion

        #endregion

        #region HPS
        /// <summary>
        /// SendCutPic to HPS
        /// </summary>
        public void SendCutPicTimer()
        {
            // Timers 非UI线程 
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;//设置是否执行Elapsed事件
            timer.Elapsed += new ElapsedEventHandler(SendCutPicEvent);//绑定Elapsed事件
            timer.Interval = 3000;//设置时间间隔
        }

        private void SendCutPicEvent(object sender, EventArgs e)
        {
            //Timer lock control the Repeat
            if (Interlocked.Exchange(ref inTimer_SendCutPic, 1) == 0)
            {
                try
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        SendCutPic(i);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    LogHandle.WriteLog_Info((int)LogMark.锯切图, ex.Message);
                }
                finally
                {
                    Interlocked.Exchange(ref inTimer_SendCutPic, 0);
                }
            }
        }
        #endregion

        #endregion

        #region Fun

        #region Req_PutOnThePlates 上料请求
        /// <summary>
        /// Req_PutOnThePlates 上料请求
        /// 
        /// 2021-05-13 22:36:36 Wade Wei
        /// </summary>
        /// <param name="About"></param>
        /// <returns></returns>
        public bool Req_PutOnThePlates(int About)
        {
            bool Result = false;
            int plcPara_LoadRel = -1;
            int plcPara_LoadReq = -1;
            switch (About)
            {
                case 1:
                    plcPara_LoadRel = LeftLoadRel;
                    plcPara_LoadReq = LeftLoadReq;
                    break;
                case 2:
                    plcPara_LoadRel = RightLoadRel;
                    plcPara_LoadReq = RightLoadReq;
                    break;
                default:
                    break;
            }
            try
            {
                using (EDM Db = new EDM())
                {
                    var StackInfo = Db.StackInfo_table.FirstOrDefault(s => s.Status == (int)StackStatus.数据解析并存储完成);
                    if (StackInfo != null)
                    {
                        tcClient.WriteAny(plcPara_LoadRel, true);
                        //Update StackInfo ：About 
                        StackInfo.About = About;
                        //更新Stack_table
                        var stack_Table = Db.Stack_table.Where(s => s.StackId == StackInfo.StackId).ToList();

                        if (stack_Table.Count <= 0)
                        {
                            return false;
                        }
                        stack_Table.ForEach(s =>
                        {
                            s.About = About;
                        });

                        tcClient.WriteAny(plcPara_LoadReq, false);

                        /* Update ScanQrCode Status = 3 means OnLine */
                        var ScanQRCodeInfo = Db.ScanQrCodeRecord.OrderByDescending(s => s.ScanTime).FirstOrDefault(s => s.StackId == StackInfo.StackId);
                        ScanQRCodeInfo.Status = (int)ScanQRCodeStatus.上线完成;

                        Db.SaveChanges();
                        Result = true;
                    }
                    else
                    {
                        Console.WriteLine("--Req_PutOnThePlate err! About : " + About);
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            Alarmlist.Add(new Alarm() { Message = "--Req_PutOnThePlate err! About : " + About, Timestamp = DateTime.Now });
                        });
                        Result = false;
                    }
                }
                return Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region EnterThePickPlateArea 板垛进入抓板区
        /// <summary>
        /// EnterThePickPlateArea
        /// 
        /// 2021-05-13 22:47:52 Wade Wei
        /// </summary>
        /// <param name="About"></param>
        /// <returns></returns>
        public bool EnterThePickPlateArea(int About)
        {
            bool Result = false;
            int PartInWork = -1;
            switch (About)
            {
                case 1:
                    PartInWork = LeftPartInWork;
                    break;
                case 2:
                    PartInWork = RightPartInWork;
                    break;
                default:
                    break;
            }
            try
            {
                //2021年5月6日13:37:40 
                using (EDM Db = new EDM())
                {
                    //Get the Stack Infomation Which Status equals 1
                    var stack_Table = Db.Stack_table.Where(s => s.Status == (int)StackStatus.数据解析并存储完成 && s.About == About).ToList();

                    if (stack_Table.Count <= 0)
                    {
                        MessageBox.Show("About : " + About + " There is no Stack Infomation , Please check the system!");
                        return false;
                    }

                    //置位PLC value
                    tcClient.WriteAny(PartInWork, false);

                    var Up_Time = DateTime.Now;

                    stack_Table.ForEach(s =>
                    {
                        s.Status = (int)StackStatus.抓板区;
                        s.UpdateTime = Up_Time;
                    });

                    var StackId = stack_Table[0].StackId;

                    var BatchId = stack_Table[0].Batch;

                    var stackInfo = Db.StackInfo_table.FirstOrDefault(s => s.StackId == StackId);

                    stackInfo.Status = (int)StackStatus.抓板区;

                    Db.SaveChanges();
                    //更新堆垛使用情况
                    view(StackId);

                    switch (About)
                    {
                        case (int)PlatesChannel.左侧:
                            //更新堆垛id
                            st1.Text = StackId;
                            //更新批次id
                            st3.Text = BatchId;
                            //更新进料状态,等待抓取
                            st5.Fill = new SolidColorBrush(Colors.Yellow);
                            break;
                        case (int)PlatesChannel.右侧:
                            //更新堆垛id
                            st2.Text = StackId;
                            //更新批次id
                            st4.Text = BatchId;
                            //更新进料状态,等待抓取
                            st6.Fill = new SolidColorBrush(Colors.Yellow);
                            break;
                        default:
                            break;
                    }
                }

                return Result = true;
            }
            catch (Exception ex)
            {
                return Result;
            }
        }
        #endregion

        #region Send cutting pic 
        /// <summary>
        /// Send cutting pic 
        /// </summary>
        /// <param name="MachineNo"></param>
        public void SendCutPic(int MachineNo)
        {
            var PartDataReq = false;
            int PartData = -1;
            var HPSPlateId = "";
            int BatchIDFB = -1;
            var _PatternFB = "";
            int PatternFB = -1;
            int PatternOKFB = -1;
            int PartDataFBReset = -1;
            int BoardPartId = -1;
            int _BoardPartId = -1;
            //TextBlock textBlock = new TextBlock();
            try
            {
                switch (MachineNo)
                {
                    case 1:
                        PartData = CH1PartDataFB;
                        BatchIDFB = CH1BatchIDFB;
                        PatternFB = CH1PatternFB;
                        PatternOKFB = CH1PatternOKFB;
                        PartDataFBReset = CH1PartDataFBReset;
                        //textBlock = hps1;
                        BoardPartId = CH1PartIDFB;
                        break;
                    case 2:
                        PartData = CH2PartDataFB;
                        BatchIDFB = CH2BatchIDFB;
                        PatternFB = CH2PatternFB;
                        PatternOKFB = CH2PatternOKFB;
                        PartDataFBReset = CH2PartDataFBReset;
                        //textBlock = hps2;
                        BoardPartId = CH2PartIDFB;
                        break;
                    case 3:
                        PartData = CH3PartDataFB;
                        BatchIDFB = CH3BatchIDFB;
                        PatternFB = CH3PatternFB;
                        PatternOKFB = CH3PatternOKFB;
                        PartDataFBReset = CH3PartDataFBReset;
                        //textBlock = hps3;
                        BoardPartId = CH3PartIDFB;
                        break;
                    case 4:
                        PartData = CH4PartDataFB;
                        BatchIDFB = CH4BatchIDFB;
                        PatternFB = CH4PatternFB;
                        PatternOKFB = CH4PatternOKFB;
                        PartDataFBReset = CH4PartDataFBReset;
                        //textBlock = hps4;
                        BoardPartId = CH4PartIDFB;
                        break;
                    case 5:
                        PartData = CH5PartDataFB;
                        BatchIDFB = CH5BatchIDFB;
                        PatternFB = CH5PatternFB;
                        PatternOKFB = CH5PatternOKFB;
                        PartDataFBReset = CH5PartDataFBReset;
                        //textBlock = hps5;
                        BoardPartId = CH5PartIDFB;
                        break;
                    default:
                        break;
                }

                PartDataReq = (bool)tcClient.ReadAny(PartData, typeof(bool));

                if (!PartDataReq)
                {
                    //no Requset
                    return;
                }

                HPSPlateId = tcClient.ReadAny(BatchIDFB, typeof(string), new int[] { 30 }).ToString();//批次
                _PatternFB = tcClient.ReadAny(PatternFB, typeof(string), new int[] { 30 }).ToString();//cut pic bmp
                _BoardPartId = Convert.ToInt32(tcClient.ReadAny(BoardPartId, typeof(int)));//PartId

                if (string.IsNullOrWhiteSpace(HPSPlateId) || string.IsNullOrWhiteSpace(_PatternFB) || _BoardPartId == 0)// 
                {
                    LogHandle.WriteLog_Info((int)LogMark.锯切图, "MachineNo : " + MachineNo + "HPSPlateId : " + HPSPlateId
                                                            + "_PatternFB : " + _PatternFB + "_BoardPartId" + _BoardPartId);
                    return;
                }

                if (SendActivateFileToHps(MachineNo, HPSPlateId, _PatternFB))
                {
                    switch (MachineNo)
                    {
                        case 1:
                            this.hps1.Dispatcher.Invoke(new Action(() => { this.hps1.Text = HPSPlateId + "-" + _PatternFB; }));
                            break;
                        case 2:
                            this.hps2.Dispatcher.Invoke(new Action(() => { this.hps2.Text = HPSPlateId + "-" + _PatternFB; }));
                            break;
                        case 3:
                            this.hps3.Dispatcher.Invoke(new Action(() => { this.hps3.Text = HPSPlateId + "-" + _PatternFB; }));
                            break;
                        case 4:
                            this.hps4.Dispatcher.Invoke(new Action(() => { this.hps4.Text = HPSPlateId + "-" + _PatternFB; }));
                            break;
                        case 5:
                            this.hps5.Dispatcher.Invoke(new Action(() => { this.hps5.Text = HPSPlateId + "-" + _PatternFB; }));
                            break;
                        default:
                            break;
                    }
                    tcClient.WriteAny(PatternOKFB, true);
                    //锯切图发送记录
                    using (EDM Db = new EDM())
                    {
                        SendCutPicRecord sendCutPicRecord = new SendCutPicRecord()
                        {
                            MachineNo = MachineNo,
                            BoardBatch = HPSPlateId,
                            BoardPartId = _BoardPartId,
                            Pic = _PatternFB,
                            SendTime = DateTime.Now
                        };

                        Db.SendCutPicRecord.Add(sendCutPicRecord);
                        Db.SaveChanges();
                    }
                }

                tcClient.WriteAny(PartDataFBReset, false);
                LogHandle.WriteLog_Info((int)LogMark.锯切图, "HPSPlateId : " + HPSPlateId + " / _PatternFB : " + _PatternFB + " / _BoardPartId" + _BoardPartId + " : Send success");
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendCutPic MachineNo : " + MachineNo + ex.ToString());
                LogHandle.WriteLog_Error((int)LogMark.锯切图, "SendCutPic MachineNo : " + ex.Message);
                App.Current.Dispatcher.Invoke(() =>
                {
                    Alarmlist.Add(new Alarm() { Message = ex.Message + "SendCutPic MachineNo : " + MachineNo, Timestamp = DateTime.Now });
                });
            }
        }
        #endregion

        #region 生成锯切图neu文件到指定位置
        /// <summary>
        /// 生成锯切图neu文件到指定位置
        /// </summary>
        /// <param name="HPS">锯</param>
        /// <param name="PartID">板件编号</param>
        /// <param name="StackID">堆垛编号</param>
        /// <param name="Pattern">锯切图编号</param>
        /// <returns>成功返回true，否则false</returns>
        private bool SendActivateFileToHps(int HPS, string SawID, string Pattern)
        {
            bool complete = false;
            try
            {
                string PatternIP = config.AppSettings.Settings["PatternIP_" + HPS].Value;
                string Use = config.AppSettings.Settings["PatternUsePwd_" + HPS].Value.Split(',')[0];
                string Pwd = config.AppSettings.Settings["PatternUsePwd_" + HPS].Value.Split(',')[1];
                string PatternGoal = config.AppSettings.Settings["PatternGoal_" + HPS].Value;
                string FileNme = "aktplan.neu";

                if (Ping(PatternIP))
                {
                    if (Connect(PatternIP, Use, Pwd))
                    {
                        if (!string.IsNullOrEmpty(Pattern) && !string.IsNullOrEmpty(SawID))
                        {
                            string str1 = "LAUF=\"" + SawID + "\"";
                            string str2 = "PLAN=\"" + Pattern + "\"";
                            string str3 = "ANZAHL=\"1\"";
                            string str4 = "DREHENINFO=\"0\"";
                            string str5 = "QUELLPLATZ=\"0\"";
                            string str6 = string.Empty;
                            string str7 = "DREHEN = 0";

                            if (File.Exists(@"\\" + PatternIP + @"\" + PatternGoal + @"\aktplan.err"))
                            {
                                //上一块板料锯切出错
                                File.Delete(@"\\" + PatternIP + @"\" + PatternGoal + @"\aktplan.err");
                            }
                            if (File.Exists(@"\\" + PatternIP + @"\" + PatternGoal + @"\aktplan.erl"))
                            {
                                File.Delete(@"\\" + PatternIP + @"\" + PatternGoal + @"\aktplan.erl");
                            }

                            if (File.Exists(@"\\" + PatternIP + @"\" + PatternGoal + @"\" + FileNme))
                            {
                                //上一块板料锯切未执行
                                return false;
                            }

                            using (StreamWriter sw = new StreamWriter(@"\\" + PatternIP + @"\" + PatternGoal + @"\" + FileNme, false, Encoding.GetEncoding("gb2312")))
                            {
                                sw.WriteLine(str1);
                                sw.WriteLine(str2);
                                sw.WriteLine(str3);
                                sw.WriteLine(str4);
                                sw.WriteLine(str5);
                                sw.WriteLine(str6);
                                sw.WriteLine(str7);
                            }
                            if (File.Exists(@"\\" + PatternIP + @"\" + PatternGoal + @"\aktplan.err"))
                            {
                                //上一块板料锯切出错
                                return false;
                            }

                            complete = true;
                        }
                        else
                        {
                            //
                            LogHandle.WriteLog_Info((int)LogMark.锯切图, "SendActivateFileToHps succeed! MachineNo :" + HPS + " bmp :" + Pattern);
                            return false;
                        }
                        LogHandle.WriteLog_Info((int)LogMark.锯切图, "SendActivateFileToHps succeed! MachineNo :" + HPS + " bmp :" + Pattern);
                    }
                    else
                    {
                        Console.WriteLine("Share File Open Failed: 10.16.247.30 ");
                    }
                }
                else
                {
                    Console.WriteLine("can not Connected with 10.16.247.30 ");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "SendActivateFileToHps failed! MachineNo :" + HPS);
                LogHandle.WriteLog_Error((int)LogMark.锯切图, "SendActivateFileToHps failed! MachineNo :" + HPS + ex.Message);
                App.Current.Dispatcher.Invoke(() =>
                {
                    Alarmlist.Add(new Alarm() { Message = ex.Message + "SendActivateFileToHps failed!", Timestamp = DateTime.Now });

                });
            }
            return complete;
        }
        #endregion

        #region ScanBoardsHandle
        /// <summary>
        /// ScanBoardsHandle
        /// 
        /// handle scanner 
        /// </summary>
        /// <param name="Station"></param>
        public void ScanBoardsHandle(int Station)
        {
            string ScannerUpi = "";
            int ScannerLorR = -1;
            int RedLightFB = -1;
            int GreenLightFB = -1;
            int YellowLightFB = -1;
            int ReqUpi = -1;
            int logMark = -1;
            int Data_Req = -1;
            try
            {
                switch (Station)
                {
                    case 1:
                        ScannerLorR = leftUPI;
                        RedLightFB = LeftRedLightFB;
                        GreenLightFB = LeftGreenLightFB;
                        YellowLightFB = LeftYellowLightFB;
                        logMark = (int)LogMark.左侧下层扫码;
                        Data_Req = ReqLeftUPI;
                        break;
                    case 2:
                        ScannerLorR = RightUPI;
                        RedLightFB = RightRedLightFB;
                        GreenLightFB = RightGreenLightFB;
                        YellowLightFB = RightYellowLightFB;
                        logMark = (int)LogMark.右侧上层扫码;
                        Data_Req = ReqRightUPI;
                        break;
                }

                var Req = (bool)tcClient.ReadAny(Data_Req, typeof(bool));

                if (!Req)
                {
                    return;
                }

                ScannerUpi = tcClient.ReadAny(ScannerLorR, typeof(String), new int[] { 50 }).ToString();

                LogHandle.WriteLog_Info(logMark, "ScanBoardsHandle : " + ScannerUpi.ToString());

                using (EDM Db = new EDM())
                {
                    ScanBoardsRecord scanBoardsRecord = new ScanBoardsRecord();

                    if (ScannerUpi.ToUpper().Contains("ERROR"))
                    {
                        scanBoardsRecord.Upi = ScannerUpi;
                        scanBoardsRecord.Batch = null;
                        scanBoardsRecord.Station = Station;
                        scanBoardsRecord.ScanTime = DateTime.Now;

                        Db.ScanBoardsRecord.Add(scanBoardsRecord);

                        Db.SaveChanges();

                        return;
                    }

                    var BatchBoardsInfo = Db.Board_table.FirstOrDefault(s => s.Upi == ScannerUpi);

                    if (BatchBoardsInfo == null)
                    {
                        LogHandle.WriteLog_Info(logMark, "ScanBoardsHandle ScannerUpi :" + ScannerUpi + "There is no info about that");
                        return;
                    }
                    var BoardBatchId = BatchBoardsInfo.BatchId;

                    BatchBoardsInfo.Status = 1;

                    scanBoardsRecord.Upi = ScannerUpi;
                    scanBoardsRecord.Batch = BoardBatchId;
                    scanBoardsRecord.Station = Station;
                    scanBoardsRecord.ScanTime = DateTime.Now;

                    Db.ScanBoardsRecord.Add(scanBoardsRecord);

                    Db.SaveChanges();

                    if (!string.IsNullOrWhiteSpace(BoardBatchId))
                    {
                        if (!Equals(BoardBatchId, lprebatchid))
                        {
                            tcClient.WriteAny(RedLightFB, true);
                            tcClient.WriteAny(GreenLightFB, false);
                            tcClient.WriteAny(YellowLightFB, false);
                        }
                        else
                        {
                            tcClient.WriteAny(RedLightFB, false);
                            tcClient.WriteAny(GreenLightFB, true);
                            tcClient.WriteAny(YellowLightFB, false);
                        }
                    }
                    else
                    {
                        tcClient.WriteAny(RedLightFB, true);
                        tcClient.WriteAny(GreenLightFB, true);
                        tcClient.WriteAny(YellowLightFB, true);
                    }

                    var NotScanCount = Db.Board_table.Where(s => s.Status == 0 && s.BatchId == BoardBatchId).Count();

                    if (NotScanCount == 0)
                    {
                        tcClient.WriteAny(RedLightFB, false);
                        tcClient.WriteAny(GreenLightFB, true);
                        tcClient.WriteAny(YellowLightFB, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ScanBoardsHandle Station :" + Station + ex.ToString());
                LogHandle.WriteLog_Error(logMark, "ScanBoardsHandle Station :" + Station + ex.ToString());
                App.Current.Dispatcher.Invoke(() =>
                {
                    Alarmlist.Add(new Alarm() { Message = ex.Message + "ScanBoardsHandle Station : " + Station, Timestamp = DateTime.Now });
                });
            }
            finally
            {
                tcClient.WriteAny(Data_Req, false);
            }
        }
        #endregion

        #endregion

        #region Run 
        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Run", "Cancel", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    StartSystem();
                    this.SystemRun.Text = "RUNING !";
                    this.SystemRun.IsEnabled = false;
                    LogHandle.WriteLog_Info((int)LogMark.系统, "System start Runing");
                }
                else
                {
                    MessageBox.Show("The system can not Runing , please check the system log");
                    LogHandle.WriteLog_Info((int)LogMark.系统, "The system can not Runing , please check the system log");
                }
            }
            catch (Exception ex)
            {
                LogHandle.WriteLog_Error((int)LogMark.系统, ex.Message);
            }
        }
        #endregion
    }
}
