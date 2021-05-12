﻿using SerialCom.SerialLogic;
using StackControl.basic;
using StackControl.Models;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using TwinCAT.Ads;

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
        private int CH1PartDataFBInt;
        private int CH2PartDataFBInt;
        private int CH3PartDataFBInt;
        private int CH4PartDataFBInt;
        private int CH5PartDataFBInt;
        private int LeftScanDataReqInt;
        private int RightScanDataReqInt;
        private int PickPartFinishReqInt;
        //private int PickPart_Channel;
        private bool Heart = false;
        private bool HeartBroken = false;
        // plc handler
        private int PickPartFinishReqFB;
        private int CH1PathFB;
        private int CH1BatchIDFB;
        private int CH1StackIDFB;
        private int CH1PatternFB;
        private int CH2PathFB;
        private int CH2BatchIDFB;
        private int CH2StackIDFB;
        private int CH2PatternFB;
        private int CH3PathFB;
        private int CH3BatchIDFB;
        private int CH3StackIDFB;
        private int CH3PatternFB;
        private int CH4PathFB;
        private int CH4BatchIDFB;
        private int CH4StackIDFB;
        private int CH4PatternFB;
        private int CH5PathFB;
        private int CH5BatchIDFB;
        private int CH5StackIDFB;
        private int CH5PatternFB;
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
        //public static string StorageBatchId;
        public string CurrentBatchId;
        public string NextBatchId;
        public string CurrentScannerBatchId;
        public bool ChangeBatchAlarm;
        public bool LeftChangeBatchAlarm;
        public bool RightChangeBatchAlarm;
        //public static string CurrentStackId;
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
        string _StackID = "";
        int _PartID = -1;
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
            FirstConfigPLC();
            PickBoardTimer();
            PickBoardFinishTimer();
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
                MessageBox.Show(ex.Message + "Cannot connect with PLC(ConnPLC)");
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
        #endregion


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

        public void ConfigPLCTags()
        {
            try
            {
                #region  notification              
                LeftLoadInt = tcClient.AddDeviceNotification(".LeftLoad_Req", dataStream, 0, 1, AdsTransMode.OnChange, 100, 0, null);
                RightLoadInt = tcClient.AddDeviceNotification(".RightLoad_Req", dataStream, 1, 1, AdsTransMode.OnChange, 100, 0, null);

                LeftPart_InWorkInt = tcClient.AddDeviceNotification(".LeftPart_InWork", dataStream, 2, 1, AdsTransMode.OnChange, 100, 0, null);
                RightPart_InWorkInt = tcClient.AddDeviceNotification(".RightPart_InWork", dataStream, 3, 1, AdsTransMode.OnChange, 100, 0, null);

                ConCh1Int = tcClient.AddDeviceNotification(".ConCh1_Status", dataStream, 4, 1, AdsTransMode.OnChange, 100, 0, null);
                ConCh2Int = tcClient.AddDeviceNotification(".ConCh2_Status", dataStream, 5, 1, AdsTransMode.OnChange, 100, 0, null);
                ConCh3Int = tcClient.AddDeviceNotification(".ConCh3_Status", dataStream, 6, 1, AdsTransMode.OnChange, 100, 0, null);
                ConCh4Int = tcClient.AddDeviceNotification(".ConCh4_Status", dataStream, 7, 1, AdsTransMode.OnChange, 100, 0, null);
                ConCh5Int = tcClient.AddDeviceNotification(".ConCh5_Status", dataStream, 8, 1, AdsTransMode.OnChange, 100, 0, null);

                PartDataReqInt = tcClient.AddDeviceNotification(".PartData_Req", dataStream, 9, 1, AdsTransMode.OnChange, 100, 0, null);

                CH1PartDataFBInt = tcClient.AddDeviceNotification(".CH1_PartData_FB", dataStream, 10, 1, AdsTransMode.OnChange, 100, 0, null);
                CH2PartDataFBInt = tcClient.AddDeviceNotification(".CH2_PartData_FB", dataStream, 11, 1, AdsTransMode.OnChange, 100, 0, null);
                CH3PartDataFBInt = tcClient.AddDeviceNotification(".CH3_PartData_FB", dataStream, 12, 1, AdsTransMode.OnChange, 100, 0, null);
                CH4PartDataFBInt = tcClient.AddDeviceNotification(".CH4_PartData_FB", dataStream, 13, 1, AdsTransMode.OnChange, 100, 0, null);
                CH5PartDataFBInt = tcClient.AddDeviceNotification(".CH5_PartData_FB", dataStream, 14, 1, AdsTransMode.OnChange, 100, 0, null);

                LeftScanDataReqInt = tcClient.AddDeviceNotification(".LeftScanData_Req", dataStream, 15, 1, AdsTransMode.OnChange, 100, 0, null);
                RightScanDataReqInt = tcClient.AddDeviceNotification(".RightScanData_Req", dataStream, 16, 1, AdsTransMode.OnChange, 100, 0, null);

                PickPartFinishReqInt = tcClient.AddDeviceNotification(".PickPartFinish", dataStream, 17, 1, AdsTransMode.OnChange, 100, 0, null);



                tcClient.AdsNotification += new AdsNotificationEventHandler(tcClient_OnNotification);
                #endregion 

                #region handler
                HeatBeatInt = tcClient.CreateVariableHandle(".HeartBeat");
                //plc -----> stack control
                CH1PathFB = tcClient.CreateVariableHandle(".CH1_Path_FB");
                CH1BatchIDFB = tcClient.CreateVariableHandle(".CH1_BatchID_FB");
                CH1StackIDFB = tcClient.CreateVariableHandle(".CH1_StackID_FB");
                CH1PatternFB = tcClient.CreateVariableHandle(".CH1_Pattern_FB");
                CH2PathFB = tcClient.CreateVariableHandle(".CH2_Path_FB");
                CH2BatchIDFB = tcClient.CreateVariableHandle(".CH2_BatchID_FB");
                CH2StackIDFB = tcClient.CreateVariableHandle(".CH2_StackID_FB");
                CH2PatternFB = tcClient.CreateVariableHandle(".CH2_Pattern_FB");
                CH3PathFB = tcClient.CreateVariableHandle(".CH3_Path_FB");
                CH3BatchIDFB = tcClient.CreateVariableHandle(".CH3_BatchID_FB");
                CH3StackIDFB = tcClient.CreateVariableHandle(".CH3_StackID_FB");
                CH3PatternFB = tcClient.CreateVariableHandle(".CH3_Pattern_FB");
                CH4PathFB = tcClient.CreateVariableHandle(".CH4_Path_FB");
                CH4BatchIDFB = tcClient.CreateVariableHandle(".CH4_BatchID_FB");
                CH4StackIDFB = tcClient.CreateVariableHandle(".CH4_StackID_FB");
                CH4PatternFB = tcClient.CreateVariableHandle(".CH4_Pattern_FB");
                CH5PathFB = tcClient.CreateVariableHandle(".CH5_Path_FB");
                CH5BatchIDFB = tcClient.CreateVariableHandle(".CH5_BatchID_FB");
                CH5StackIDFB = tcClient.CreateVariableHandle(".CH5_StackID_FB");
                CH5PatternFB = tcClient.CreateVariableHandle(".CH5_Pattern_FB");
                ReqLeftUPI = tcClient.CreateVariableHandle(".LeftScanData_Req");
                leftUPI = tcClient.CreateVariableHandle(".LeftUPI");
                ReqRightUPI = tcClient.CreateVariableHandle(".RightScanData_Req");
                RightUPI = tcClient.CreateVariableHandle(".RightUPI");
                PickPartFinishReqFB = tcClient.CreateVariableHandle(".PickPartFinish");

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "-notification err!");
                App.Current.Dispatcher.Invoke(() =>
                {
                    Alarmlist.Add(new Alarm() { Message = ex.Message, Timestamp = DateTime.Now });

                });
            }
        }

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
                        if (basic.SqlHelper.CheckStackInfo(StackId))
                        {
                            tcClient.WriteAny(LeftLoadRel, true);

                            //发送完成修改状态
                            basic.SqlHelper.UpdateAbout(1);
                            Thread.Sleep(50);
                            tcClient.WriteAny(LeftLoadReq, false);
                        }
                        else
                            Console.WriteLine("--CheckStackInfo err!");
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            Alarmlist.Add(new Alarm() { Message = "--CheckStackInfo err!", Timestamp = DateTime.Now });

                        });

                    }

                }
                else if (e.NotificationHandle == RightLoadInt)
                {
                    if (binRead.ReadBoolean())
                    {
                        if (basic.SqlHelper.CheckStackInfo(StackId))
                        {
                            tcClient.WriteAny(RightLoadRel, true);

                            //发送完成修改状态
                            basic.SqlHelper.UpdateAbout(2);
                            Thread.Sleep(50);
                            tcClient.WriteAny(RightLoadReq, false);
                        }
                        else
                            Console.WriteLine("--CheckStackInfo err!");
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            Alarmlist.Add(new Alarm() { Message = "--CheckStackInfo err!", Timestamp = DateTime.Now });

                        });

                    }
                }
                #endregion

                #region 进入抓板区
                else if (e.NotificationHandle == LeftPart_InWorkInt)
                {
                    if (binRead.ReadBoolean())
                    {
                        //2021年5月6日13:37:40
                        (string StackId, string BatchId) = SqlHelper.update_StackStatus(1);
                        if (!string.IsNullOrWhiteSpace(StackId) && !string.IsNullOrWhiteSpace(BatchId))
                        {
                            tcClient.WriteAny(LeftPartInWork, false);
                            //更新堆垛使用情况
                            view(StackId);
                            //更新堆垛id
                            st1.Text = StackId;
                            //更新批次id
                            st3.Text = BatchId;
                            //更新进料状态
                            st5.Fill = new SolidColorBrush(Colors.Red);
                        }
                    }
                }
                else if (e.NotificationHandle == RightPart_InWorkInt)
                {
                    if (binRead.ReadBoolean())
                    {
                        (string StackId, string BatchId) = SqlHelper.update_StackStatus(2);
                        if (!string.IsNullOrWhiteSpace(StackId) && !string.IsNullOrWhiteSpace(BatchId))
                        {
                            tcClient.WriteAny(LeftPartInWork, false);
                            //更新堆垛使用情况
                            view(StackId);
                            //更新堆垛id
                            st2.Text = StackId;
                            //更新批次id
                            st4.Text = BatchId;
                            //更新进料状态
                            st6.Fill = new SolidColorBrush(Colors.Red);
                        }
                    }
                }
                #endregion

                #region convery status          
                else if (e.NotificationHandle == ConCh1Int)
                {
                    ConCh1Status = binRead.ReadBoolean();
                }
                else if (e.NotificationHandle == ConCh2Int)
                {
                    ConCh2Status = binRead.ReadBoolean();
                }
                else if (e.NotificationHandle == ConCh3Int)
                {
                    ConCh3Status = binRead.ReadBoolean();
                }
                else if (e.NotificationHandle == ConCh4Int)
                {
                    ConCh4Status = binRead.ReadBoolean();
                }
                else if (e.NotificationHandle == ConCh5Int)
                {
                    ConCh5Status = binRead.ReadBoolean();
                }
                #endregion

                #region partdata request 板料请求信号
                //else if (e.NotificationHandle == PartDataReqInt)
                //{
                //    if (binRead.ReadBoolean())
                //    {
                //        int About = -1;
                //        int PickPart_Channel = Convert.ToInt32(tcClient.ReadAny(PickPartChannel, typeof(int)));
                //        if (PickPart_Channel == 99)
                //        {
                //            //确定当前批次
                //            currentBatch = SqlHelper.CheckCurrentBatch();
                //            if (!string.IsNullOrEmpty(currentBatch))
                //            {

                //                About = SqlHelper.ExistCurrentBatch(currentBatch);
                //                //工作区是否有当前批次判断
                //                if (About > 0)
                //                {
                //                    //有当前批次,给plc通道号
                //                    (int curBatchNum, int tolBatchNum) = SqlHelper.QueryBatchQuantity(currentBatch);
                //                    if (curBatchNum <= tolBatchNum - 3)
                //                    {
                //                        SendData(About, 1, 2, 3, 4, 5);
                //                    }
                //                    else
                //                    {
                //                        SendData(About, 1, 2, 3, 0, 0);
                //                    }
                //                }
                //                else
                //                {
                //                    //没有当前批次
                //                    //等待当前批次，或者强制更新为其他批次
                //                    while (!ForceChangeBatch)
                //                    {
                //                        Thread.Sleep(200);
                //                        About = SqlHelper.ExistCurrentBatch(currentBatch);
                //                        if (About > 0)
                //                        {
                //                            //有当前批次,给plc通道号
                //                            (int curBatchNum, int tolBatchNum) = SqlHelper.QueryBatchQuantity(currentBatch);
                //                            if (curBatchNum <= tolBatchNum - 3)
                //                            {
                //                                SendData(About, 1, 2, 3, 4, 5);
                //                            }
                //                            else
                //                            {
                //                                SendData(About, 1, 2, 3, 0, 0);
                //                            }
                //                            break;
                //                        }
                //                    }
                //                    if (ForceChangeBatch)
                //                    {
                //                        SqlHelper.Update_BatchStatus(currentBatch, 2);
                //                        currentBatch = SqlHelper.CheckCurrentBatch();
                //                        About = SqlHelper.ExistCurrentBatch(currentBatch);
                //                        if (About > 0)
                //                        {
                //                            (int curBatchNum, int tolBatchNum) = SqlHelper.QueryBatchQuantity(currentBatch);
                //                            if (curBatchNum <= tolBatchNum - 3)
                //                            {
                //                                SendData(About, 1, 2, 3, 4, 5);
                //                            }
                //                            else
                //                            {
                //                                SendData(About, 1, 2, 3, 0, 0);
                //                            }
                //                        }
                //                    }
                //                }
                //            }

                //        }
                //        else if (PickPart_Channel == 1 || PickPart_Channel == 2)
                //        {
                //            currentBatch = SqlHelper.CheckCurrentBatch();
                //            (int curBatchNum, int tolBatchNum) = SqlHelper.QueryBatchQuantity(currentBatch);
                //            if (curBatchNum == tolBatchNum)
                //            {
                //                tcClient.WriteAny(Path, (short)88);
                //                tcClient.WriteAny(PartDataReq, false);
                //            }
                //            else if (curBatchNum <= tolBatchNum - 3)
                //            {
                //                SendData(PickPart_Channel, 1, 2, 3, 4, 5);
                //            }
                //            else
                //            {
                //                SendData(PickPart_Channel, 1, 2, 3, 0, 0);
                //            }
                //        }

                //        //给plc发送新批次信息
                //        if (!Equals(CurrentBatch, currentBatch) && !string.IsNullOrWhiteSpace(currentBatch))
                //        {
                //            //给plc发送新批次信息
                //            tcClient.WriteAny(NewBatchID, currentBatch, new int[] { 30 });
                //            tcClient.WriteAny(NewBatchIDReady, true);
                //        }

                //        CurrentBatch = currentBatch;
                //    }

                //}

                //else if (e.NotificationHandle == PickPartFinishReqInt)
                //{
                //    if (binRead.ReadBoolean())
                //    {
                //        currentBatch = SqlHelper.CheckCurrentBatch();
                //        int about = SqlHelper.ExistCurrentBatch(currentBatch);
                //        DataTable DT = basic.SqlHelper.GetBoardDeatail(about);
                //        int _PartID = Convert.ToInt16(DT.Rows[0][4]);
                //        string _StackID = DT.Rows[0][6].ToString();
                //        (int Pos, int About) = SqlHelper.UpdateStatus2(currentBatch, _StackID, _PartID.ToString());
                //        Thread.Sleep(100);
                //        upe_Background(Pos, 3, About);

                //        tcClient.WriteAny(PickPartChannel, (short)0);
                //        tcClient.WriteAny(PickPartFinishReqFB, false);
                //    }
                //}

                #endregion

                #region Get plate info before to HPS
                else if (e.NotificationHandle == CH1PartDataFBInt)
                {
                    if (binRead.ReadBoolean())
                    {
                        NO1HPSPlateID = tcClient.ReadAny(CH1BatchIDFB, typeof(string), new int[] { 30 }).ToString();      //板料编号
                        string _CH1PatternFB = tcClient.ReadAny(CH1PatternFB, typeof(string), new int[] { 30 }).ToString();       //锯切图编号
                        if (SendActivateFileToHps(1, NO1HPSPlateID, _CH1PatternFB))
                        {
                            hps1.Text = NO1HPSPlateID + "-" + _CH1PatternFB;
                            tcClient.WriteAny(CH1PatternOKFB, true);
                        }
                        tcClient.WriteAny(CH1PartDataFBReset, false);
                    }
                }
                else if (e.NotificationHandle == CH2PartDataFBInt)
                {
                    if (binRead.ReadBoolean())
                    {
                        NO2HPSPlateID = tcClient.ReadAny(CH2BatchIDFB, typeof(string), new int[] { 30 }).ToString();
                        string _CH2PatternFB = tcClient.ReadAny(CH2PatternFB, typeof(string), new int[] { 30 }).ToString();
                        if (SendActivateFileToHps(2, NO2HPSPlateID, _CH2PatternFB))
                        {
                            hps2.Text = NO2HPSPlateID + "-" + _CH2PatternFB;
                            tcClient.WriteAny(CH2PatternOKFB, true);
                        }

                        tcClient.WriteAny(CH2PartDataFBReset, false);
                    }
                }
                else if (e.NotificationHandle == CH3PartDataFBInt)
                {
                    if (binRead.ReadBoolean())
                    {
                        NO3HPSPlateID = tcClient.ReadAny(CH3BatchIDFB, typeof(string), new int[] { 30 }).ToString();
                        string _CH3PatternFB = tcClient.ReadAny(CH3PatternFB, typeof(string), new int[] { 30 }).ToString();
                        if (SendActivateFileToHps(3, NO3HPSPlateID, _CH3PatternFB))
                        {
                            hps3.Text = NO3HPSPlateID + "-" + _CH3PatternFB;
                            tcClient.WriteAny(CH3PatternOKFB, true);
                        }
                        tcClient.WriteAny(CH3PartDataFBReset, false);
                    }
                }
                else if (e.NotificationHandle == CH4PartDataFBInt)
                {
                    if (binRead.ReadBoolean())
                    {
                        NO4HPSPlateID = tcClient.ReadAny(CH4BatchIDFB, typeof(string), new int[] { 30 }).ToString();
                        string _CH4PatternFB = tcClient.ReadAny(CH4PatternFB, typeof(string), new int[] { 30 }).ToString();
                        if (SendActivateFileToHps(4, NO4HPSPlateID, _CH4PatternFB))
                        {
                            hps4.Text = NO4HPSPlateID + "-" + _CH4PatternFB;
                            tcClient.WriteAny(CH4PatternOKFB, true);
                        }
                        tcClient.WriteAny(CH4PartDataFBReset, false);
                    }
                }
                else if (e.NotificationHandle == CH5PartDataFBInt)
                {
                    if (binRead.ReadBoolean())
                    {
                        NO5HPSPlateID = tcClient.ReadAny(CH5BatchIDFB, typeof(string), new int[] { 30 }).ToString();
                        string _CH5StackIDFB = tcClient.ReadAny(CH5StackIDFB, typeof(string), new int[] { 30 }).ToString();
                        string _CH5PatternFB = tcClient.ReadAny(CH5PatternFB, typeof(string), new int[] { 30 }).ToString();
                        if (SendActivateFileToHps(5, NO5HPSPlateID, _CH5PatternFB))
                        {
                            hps5.Text = NO5HPSPlateID + "-" + _CH5PatternFB;
                            tcClient.WriteAny(CH5PatternOKFB, true);
                        }
                        tcClient.WriteAny(CH5PartDataFBReset, false);
                    }
                }

                #endregion

                #region scanner
                else if (e.NotificationHandle == LeftScanDataReqInt)
                {
                    if (binRead.ReadBoolean())
                    {
                        string leftScanner = tcClient.ReadAny(leftUPI, typeof(String), new int[] { 50 }).ToString();
                        (string CurBatchID, int ret) = basic.SqlHelper.CheckNewBatchBoard(leftScanner);
                        if (!(Equals(CurBatchID, string.Empty)))
                        {
                            if (!Equals(CurBatchID, lprebatchid))
                            {
                                tcClient.WriteAny(LeftRedLightFB, true);
                                tcClient.WriteAny(LeftGreenLightFB, false);
                                tcClient.WriteAny(LeftYellowLightFB, false);

                            }
                            else if (Equals(CurBatchID, lprebatchid) && !Equals(CurBatchID, string.Empty))
                            {
                                tcClient.WriteAny(LeftGreenLightFB, true);
                                tcClient.WriteAny(LeftRedLightFB, false);
                                tcClient.WriteAny(LeftYellowLightFB, false);
                            }
                            else if (ret == 1)
                            {
                                tcClient.WriteAny(LeftRedLightFB, false);
                                tcClient.WriteAny(LeftYellowLightFB, true);
                                tcClient.WriteAny(LeftGreenLightFB, true);
                            }
                            lprebatchid = CurBatchID;

                        }
                        else
                        {
                            tcClient.WriteAny(LeftRedLightFB, true);
                            tcClient.WriteAny(LeftYellowLightFB, true);
                            tcClient.WriteAny(LeftGreenLightFB, true);

                        }
                        Thread.Sleep(100);
                        tcClient.WriteAny(ReqLeftUPI, false);
                    }
                }
                else if (e.NotificationHandle == RightScanDataReqInt)
                {
                    if (binRead.ReadBoolean())
                    {
                        string rightScanner = tcClient.ReadAny(RightUPI, typeof(String), new int[] { 50 }).ToString();
                        (string CurBatchID, int ret) = basic.SqlHelper.CheckNewBatchBoard(rightScanner);

                        if (!(Equals(CurBatchID, string.Empty)))
                        {
                            if (!Equals(CurBatchID, rprebatchid))
                            {
                                tcClient.WriteAny(RightRedLightFB, true);
                                tcClient.WriteAny(RightGreenLightFB, false);
                                tcClient.WriteAny(RightYellowLightFB, false);

                            }
                            else if (Equals(CurBatchID, rprebatchid) && !Equals(CurBatchID, string.Empty))
                            {
                                tcClient.WriteAny(RightGreenLightFB, true);
                                tcClient.WriteAny(RightRedLightFB, false);
                                tcClient.WriteAny(RightYellowLightFB, false);
                            }
                            else if (ret == 1)
                            {
                                tcClient.WriteAny(RightRedLightFB, false);
                                tcClient.WriteAny(RightYellowLightFB, true);
                                tcClient.WriteAny(RightGreenLightFB, true);
                            }
                            rprebatchid = CurBatchID;

                        }
                        else
                        {
                            tcClient.WriteAny(RightRedLightFB, true);
                            tcClient.WriteAny(RightYellowLightFB, true);
                            tcClient.WriteAny(RightGreenLightFB, true);
                        }
                        Thread.Sleep(100);
                        tcClient.WriteAny(ReqRightUPI, false);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ": tcClient_OnNotification Error!");
                App.Current.Dispatcher.Invoke(() =>
                {
                    Alarmlist.Add(new Alarm() { Message = ex.Message + ": tcClient_OnNotification Error!", Timestamp = DateTime.Now });

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
                //int waittime = Convert.ToInt32(config.AppSettings.Settings["WaitTime"].Value);
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
                            {//上一块板料锯切出错
                                File.Delete(@"\\" + PatternIP + @"\" + PatternGoal + @"\aktplan.err");
                            }
                            if (File.Exists(@"\\" + PatternIP + @"\" + PatternGoal + @"\aktplan.erl"))
                            {
                                File.Delete(@"\\" + PatternIP + @"\" + PatternGoal + @"\aktplan.erl");
                            }

                            if (File.Exists(@"\\" + PatternIP + @"\" + PatternGoal + @"\" + FileNme))
                            {//上一块板料锯切未执行
                                return false;
                            }

                            using (StreamWriter sw = new StreamWriter(@"\\" + PatternIP + @"\" + PatternGoal + @"\" + FileNme, false, Encoding.GetEncoding("gb2312")))
                            {
                                for (int i = 8; i > 0; i--)
                                {
                                    switch (i)
                                    {
                                        case 7:
                                            sw.WriteLine(str1);
                                            break;
                                        case 6:
                                            sw.WriteLine(str2);
                                            break;
                                        case 5:
                                            sw.WriteLine(str3);
                                            break;
                                        case 4:
                                            sw.WriteLine(str4);
                                            break;
                                        case 3:
                                            sw.WriteLine(str5);
                                            break;
                                        case 2:
                                            sw.WriteLine(str6);
                                            break;
                                        case 1:
                                            sw.WriteLine(str7);
                                            break;
                                    }
                                }
                            }
                            //2021 05 10
                            //Thread.Sleep(waittime);
                            if (File.Exists(@"\\" + PatternIP + @"\" + PatternGoal + @"\aktplan.err"))
                            {//上一块板料锯切出错
                                return false;
                            }

                            complete = true;
                        }
                        Disconnect(PatternIP, Use, Pwd);
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
                Console.WriteLine(ex.Message + "SendActivateFileToHps failed!");
                App.Current.Dispatcher.Invoke(() =>
                {
                    Alarmlist.Add(new Alarm() { Message = ex.Message + "SendActivateFileToHps failed!", Timestamp = DateTime.Now });

                });

            }
            return complete;
        }
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
                string dosLine = @"net use \\" + remoteHost + " " + passWord + " " + " /user:" + userName + ">NUL";
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

        #region ScanQRCode
        
        #region ScanQRCode Connect
        /// <summary>
        /// ScanQRCode
        /// </summary>
        public void ScanQRCode()
        {
            SerialCom = new SerialHelper("COM2"/*串口号*/, 115200/*波特率*/, 0/*校验位*/, 8/*数据位*/, 1/*停止位*/, serialPortHelper_ReceivedDataEvent);
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
            // DataInfo
            var Recvmessage = (Encoding.Default.GetString(args.RecvData, 0, args.RecvDataLength)).Replace('\r', ' ').Trim();
            StackId = Recvmessage;
            Console.WriteLine(Recvmessage);
            Analysis(Recvmessage);
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

            if (Ping(Getcsv_IP))
            {

                if (Connect(Getcsv_IP, Use, Pwd))
                {
                    try
                    {
                        if (basic.SqlHelper.CheckUnmark())
                        {
                            Console.WriteLine(DateTime.Now + " 扫描失败！上料口存在未标记左右的堆垛，请完成操作后重新扫描。");
                            return;
                        }
                        if (basic.SqlHelper.CheckCom(StackId))
                        {
                            Console.WriteLine(DateTime.Now + "已经完成的批次再次上线，请重新确认批次号");
                            return;
                        }
                        //存储堆垛信息
                        string[] Stack = File.ReadAllLines(@"\\" + Getcsv_IP + @"\" + Path + @"\" + FileNme, Encoding.Default);
                        //BUG 
                        foreach (var dataRow in Stack)
                        {
                            StackModel stackModel = new StackModel
                            {
                                StackId = scanRes,
                                Batch = dataRow.Split(',')[0].Split('_')[0],
                                PartID = Convert.ToInt32(dataRow.Split(',')[1]),
                                Len = Convert.ToInt32(dataRow.Split(',')[2]),
                                Width = Convert.ToInt32(dataRow.Split(',')[3]),
                                Thin = Convert.ToInt32(dataRow.Split(',')[4]),
                                Material = dataRow.Split(',')[5],
                                Pos = Convert.ToInt32(dataRow.Split(',')[6]),
                                Pattern = Convert.ToInt32(dataRow.Split(',')[7]),
                                Map = dataRow.Split(',')[8],
                                Status = 1,
                                About = 0
                            };
                            basic.SqlHelper.Insert_Stack_table(stackModel);
                        }
                        File.Move(@"\\" + Getcsv_IP + @"\" + Path + @"\" + FileNme, @"\\" + Getcsv_IP + @"\" + Goal + @"\" + FileNme);

                        //存储板料信息
                        if (File.Exists(@"\\" + Getcsv_IP + @"\" + Path + @"\" + FileNme_Parts))
                        {
                            string[] Parts = File.ReadAllLines(@"\\" + Getcsv_IP + @"\" + Path + @"\" + FileNme_Parts, Encoding.Default);
                            foreach (var dataRow in Parts)
                            {
                                string BatchId = dataRow.Split(',')[0];
                                int Number = Convert.ToInt32(dataRow.Split(',')[1]);
                                string Upi = dataRow.Split(',')[2];
                                basic.SqlHelper.Insert_Board_table(BatchId, Number, Upi);
                            }
                            File.Move(@"\\" + Getcsv_IP + @"\" + Path + @"\" + FileNme_Parts, @"\\" + Getcsv_IP + @"\" + Goal + @"\" + FileNme_Parts);
                        }

                        //存储批次数量信息
                        if (File.Exists(@"\\" + Getcsv_IP + @"\" + Path + @"\" + FileNme_All))
                        {
                            string[] BatchInfo = File.ReadAllLines(@"\\" + Getcsv_IP + @"\" + Path + @"\" + FileNme_All, Encoding.Default);
                            string BatchId = BatchInfo[0].Substring(0, BatchInfo[0].Split('.')[0].LastIndexOf('_'));
                            int BatchNum = Convert.ToInt32(BatchInfo[0].Split(',')[1]);
                            basic.SqlHelper.Insert_Batch_table(BatchId, BatchNum);

                            File.Move(@"\\" + Getcsv_IP + @"\" + Path + @"\" + FileNme_All, @"\\" + Getcsv_IP + @"\" + Goal + @"\" + FileNme_All);
                        }
                        Disconnect(Getcsv_IP, Use, Pwd);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
        #endregion
        #endregion

        #region Pick Board Timer

        #region PickBoardTimer
        /// <summary>
        /// Pick Board Timer
        /// </summary>
        private void PickBoardTimer()
        {
            DispatcherTimer showTimer = new DispatcherTimer();
            showTimer.Tick += new EventHandler(PickBoardEvent);
            showTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            showTimer.Start();
        }
        /// <summary>
        /// Pick Board Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PickBoardEvent(object sender, EventArgs e)
        {
            try
            {
                var Req = (bool)tcClient.ReadAny(PartDataReq, typeof(bool));
                if (!Req)
                {
                    return;
                }

                int About = -1;
                int PickPart_Channel = Convert.ToInt32(tcClient.ReadAny(PickPartChannel, typeof(int)));
                if (PickPart_Channel == 99)
                {
                    //确定当前批次
                    currentBatch = SqlHelper.CheckCurrentBatch();
                    if (!string.IsNullOrEmpty(currentBatch))
                    {

                        About = SqlHelper.ExistCurrentBatch(currentBatch);
                        //工作区是否有当前批次判断
                        if (About > 0)
                        {
                            SendData(About, 1, 2, 3, 4, 5);
                        }
                        else
                        {
                            //没有当前批次
                            //等待当前批次，或者强制更新为其他批次
                            while (!ForceChangeBatch)
                            {
                                Thread.Sleep(200);
                                About = SqlHelper.ExistCurrentBatch(currentBatch);
                                if (About > 0)
                                {
                                    SendData(About, 1, 2, 3, 4, 5);
                                    break;
                                }
                            }
                            if (ForceChangeBatch)
                            {
                                SqlHelper.Update_BatchStatus(currentBatch, 2);
                                currentBatch = SqlHelper.CheckCurrentBatch();
                                About = SqlHelper.ExistCurrentBatch(currentBatch);
                                if (About > 0)
                                {
                                    SendData(About, 1, 2, 3, 4, 5);
                                }
                            }
                        }
                    }

                }
                else if (PickPart_Channel == 1 || PickPart_Channel == 2)
                {
                    currentBatch = SqlHelper.CheckCurrentBatch();
                    if (string.IsNullOrWhiteSpace(currentBatch))
                    {
                        //there is dont have currentBatch , Close the request 
                        tcClient.WriteAny(PartDataReq, false);
                        return;
                    }
                    (int curBatchNum, int tolBatchNum) = SqlHelper.QueryBatchQuantity(currentBatch);
                    //小于40张
                    if (tolBatchNum < 40 && curBatchNum == tolBatchNum)
                    {
                        //The backing plate
                        tcClient.WriteAny(Path, (short)88);
                        tcClient.WriteAny(PartDataReq, false);
                    }
                    else if (tolBatchNum > 40 && curBatchNum % 40 == 0)
                    {
                        //The backing plate
                        tcClient.WriteAny(Path, (short)88);
                        tcClient.WriteAny(PartDataReq, false);
                    }
                    else if (curBatchNum == tolBatchNum)
                    {
                        //The backing plate
                        tcClient.WriteAny(Path, (short)88);
                        tcClient.WriteAny(PartDataReq, false);
                    }
                    else
                    {
                        SendData(PickPart_Channel, 1, 2, 3, 4, 5);
                    }
                }

                //给plc发送新批次信息
                if (!Equals(CurrentBatch, currentBatch) && !string.IsNullOrWhiteSpace(currentBatch))
                {
                    //给plc发送新批次信息
                    tcClient.WriteAny(NewBatchID, currentBatch, new int[] { 30 });
                    tcClient.WriteAny(NewBatchIDReady, true);
                }

                CurrentBatch = currentBatch;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
        private void SendData(int About, int channel1, int channel2, int channel3, int channel4, int channel5)
        {
            //有当前批次,给plc通道号
            if (ConCh1Status)
                SendDataToPlc(channel1, About);
            else if (ConCh2Status)
                SendDataToPlc(channel2, About);
            else if (ConCh3Status)
                SendDataToPlc(channel3, About);
            else if (ConCh4Status)
                SendDataToPlc(channel4, About);
            else if (ConCh5Status)
                SendDataToPlc(channel5, About);
            else
                return;//there is no channel need The Board .
        }
        #endregion

        #region 发送板料信息到plc
        /// <summary>
        /// 发送板料信息到plc
        /// </summary>
        /// <param name="hps">输送线通道号1,2,3,4,5</param>
        private void SendDataToPlc(int hps, int about)
        {
            try
            {
                DataTable DT = basic.SqlHelper.GetBoardDeatail(about);
                string splitPattern = DT.Rows[0][7].ToString().Split('.')[0].Split('-')[DT.Rows[0][7].ToString().Split('.')[0].Split('-').Length - 1];
                string splitBatchIDtring = DT.Rows[0][7].ToString().Split('.')[0].Substring(0, DT.Rows[0][7].ToString().Split('.')[0].LastIndexOf('-'));
                tcClient.WriteAny(BatchID, splitBatchIDtring, new int[] { 30 });                         //批次编号
                tcClient.WriteAny(Length, (short)Convert.ToInt16(DT.Rows[0][1]));                        //板料长度
                tcClient.WriteAny(tWidth, (short)Convert.ToInt16(DT.Rows[0][2]));                        //板料宽度
                tcClient.WriteAny(Thinkness, (short)Convert.ToInt16(DT.Rows[0][3]));                     //板料厚度
                tcClient.WriteAny(Path, (short)Convert.ToInt16(hps));
                //路径分配编号                                                        
                _PartID = Convert.ToInt16(DT.Rows[0][4]);
                tcClient.WriteAny(PathID, (short)_PartID);                                              //板料ID编号
                tcClient.WriteAny(PickPartChannel, (short)about);
                //抓板通道号
                _StackID = DT.Rows[0][6].ToString();
                tcClient.WriteAny(StackID, _StackID, new int[] { 30 });                                    //堆垛编号
                tcClient.WriteAny(Pattern, splitPattern, new int[] { 30 });                              //锯切图号
                Thread.Sleep(100);
                tcClient.WriteAny(PartDataReq, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
            DispatcherTimer showTimer = new DispatcherTimer();
            showTimer.Tick += new EventHandler(PickBoardFinishEvent);
            showTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            showTimer.Start();
        }
        /// <summary>
        /// Pick Board Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PickBoardFinishEvent(object sender, EventArgs e)
        {
            try
            {
                var Fin = (bool)tcClient.ReadAny(PickPartFinishReqFB, typeof(bool));
                if (!Fin)
                {
                    return;
                }

                currentBatch = SqlHelper.CheckCurrentBatch();
                int about = SqlHelper.ExistCurrentBatch(currentBatch);
                DataTable DT = basic.SqlHelper.GetBoardDeatail(about);
                int _PartID = Convert.ToInt16(DT.Rows[0][4]);
                string _StackID = DT.Rows[0][6].ToString();
                (int Pos, int About, int BatchSurplusCount, int StackSurplusCount) = SqlHelper.UpdateStatus2(currentBatch, _StackID, _PartID.ToString());

                tcClient.WriteAny(PickPartChannel, (short)0);
                tcClient.WriteAny(PickPartFinishReqFB, false);

                //chuli view 
                switch (about)
                {
                    case 1:
                        this.st1Count.Text = StackSurplusCount.ToString();
                        break;
                    case 2:
                        this.st2Count.Text = StackSurplusCount.ToString();
                        break;
                }
                upe_Background(Pos, 3, About);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region About View

        #region View Refresh
        /// <summary>
        /// View Refresh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            view();
        }
        #endregion

        #region Load View Data
        /// <summary>
        /// Load View Data
        /// </summary>
        /// <param name="StackId"></param>
        public void view(string StackId = "")
        {
            DataTable dataTable = basic.SqlHelper.Get_WorkStack(StackId);
            foreach (DataRow dr in dataTable.Rows)
            {
                int Pos = Convert.ToInt32(dr[0]);       //序号
                int Status = Convert.ToInt32(dr[1]);    //状态
                int About = Convert.ToInt32(dr[2]);     //左右
                upe_Background(Pos, Status, About);
            }

            DataTable dataTable1 = basic.SqlHelper.Get_WorkId();
            foreach (DataRow dr in dataTable1.Rows)
            {
                if (Convert.ToInt32(dr[2]) == 1)
                {
                    st1.Text = dr[0].ToString();                    //更新堆垛id
                    st3.Text = dr[1].ToString();                    //更新批次id
                    st5.Fill = new SolidColorBrush(Colors.Green);     //更新进料状态
                }
                if (Convert.ToInt32(dr[2]) == 2)
                {
                    st2.Text = dr[0].ToString();
                    st4.Text = dr[1].ToString();
                    st6.Fill = new SolidColorBrush(Colors.Green);
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
            if (status == 2)
                border.Background = new SolidColorBrush(Colors.Green);
            else if (status == 3)
                border.Background = new SolidColorBrush(Colors.White);

            if (border == A40 && status == 3)
            {
                //更新堆垛id
                st1.Text = "";
                //更新批次id
                st3.Text = "";
                //更新进料状态
                st5.Fill = new SolidColorBrush(Colors.Yellow);
            }
            if (border == B40 && status == 3)
            {
                //更新堆垛id
                st2.Text = "";
                //更新批次id
                st4.Text = "";
                //更新进料状态
                st6.Fill = new SolidColorBrush(Colors.Yellow);
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
                tcClient.DeleteDeviceNotification(LeftLoadInt);
                tcClient.DeleteDeviceNotification(ReqLeftUPI);
                tcClient.DeleteDeviceNotification(RightLoadInt);
                tcClient.DeleteDeviceNotification(RightUPI);

                tcClient.DeleteDeviceNotification(LeftPartInWork);
                tcClient.DeleteDeviceNotification(RightPartInWork);
                tcClient.DeleteDeviceNotification(LeftLoadReq);
                tcClient.DeleteDeviceNotification(RightLoadReq);
                tcClient.DeleteDeviceNotification(ConCh1Int);
                tcClient.DeleteDeviceNotification(ConCh2Int);
                tcClient.DeleteDeviceNotification(ConCh3Int);
                tcClient.DeleteDeviceNotification(ConCh4Int);
                tcClient.DeleteDeviceNotification(ConCh5Int);
                tcClient.DeleteDeviceNotification(PartDataReqInt);
                tcClient.DeleteDeviceNotification(CH1PartDataFBInt);
                tcClient.DeleteDeviceNotification(CH2PartDataFBInt);
                tcClient.DeleteDeviceNotification(CH3PartDataFBInt);
                tcClient.DeleteDeviceNotification(CH4PartDataFBInt);
                tcClient.DeleteDeviceNotification(CH5PartDataFBInt);
                tcClient.DeleteDeviceNotification(LeftScanDataReqInt);
                tcClient.DeleteDeviceNotification(RightScanDataReqInt);

                tcClient.DeleteVariableHandle(CH1PathFB);
                tcClient.DeleteVariableHandle(CH1BatchIDFB);
                tcClient.DeleteVariableHandle(CH2PathFB);
                tcClient.DeleteVariableHandle(CH2BatchIDFB);
                tcClient.DeleteVariableHandle(CH3PathFB);
                tcClient.DeleteVariableHandle(CH3BatchIDFB);
                tcClient.DeleteVariableHandle(CH4PathFB);
                tcClient.DeleteVariableHandle(CH4BatchIDFB);
                tcClient.DeleteVariableHandle(CH5PathFB);
                tcClient.DeleteVariableHandle(CH5BatchIDFB);
                tcClient.DeleteVariableHandle(leftUPI);
                tcClient.DeleteVariableHandle(RightUPI);
                tcClient.DeleteVariableHandle(LeftLoadRel);
                tcClient.DeleteVariableHandle(RightLoadRel);
                tcClient.DeleteDeviceNotification(PickPartFinishReqFB);
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
                //  tcClient.DeleteVariableHandle(PickPart_Channel);

                tcClient.DeleteVariableHandle(LeftRedLightFB);
                tcClient.DeleteVariableHandle(LeftGreenLightFB);
                tcClient.DeleteVariableHandle(LeftYellowLightFB);

                tcClient.DeleteVariableHandle(RightRedLightFB);
                tcClient.DeleteVariableHandle(RightGreenLightFB);
                tcClient.DeleteVariableHandle(RightYellowLightFB);

                tcClient.DeleteDeviceNotification(CH1PatternOKFB);
                tcClient.DeleteDeviceNotification(CH2PatternOKFB);
                tcClient.DeleteDeviceNotification(CH3PatternOKFB);
                tcClient.DeleteDeviceNotification(CH4PatternOKFB);
                tcClient.DeleteDeviceNotification(CH5PatternOKFB);


                tcClient.Dispose();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ":closing error");
            }

        }
        #endregion

        #region show current time
        private void showTime()
        {
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
    }
}
