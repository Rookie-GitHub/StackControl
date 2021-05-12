using StackControl.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace StackControl.basic
{
    public class SqlHelper
    {
        private static string ConnectionStr = StackControl.Properties.Settings.Default.DBConnect;

        public static void InsertRemainBatchId(string stackid, string pos, int feed, int remainNums)
        {

            try
            {
                string sql = $"insert into [StacksInfo].[dbo].[StorageBatch] values(N'{stackid}','{remainNums}','{pos}', {feed})";
                using (SqlConnection con = new SqlConnection(ConnectionStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "-InsertRemainBatchId err!");
            }
        }

        /// <summary>
        /// 已扫描未标记
        /// </summary>
        /// <returns>检查到已扫描未标记则返回ture,否则false</returns>
        public static bool CheckUnmark()
        {
            bool Ren = false;
            string sql = $"SELECT [StackId] FROM [StacksInfo].[dbo].[Stack_table] WHERE [About]=0 AND [Status] = 1 Group by [StackId] ";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            Ren = reader.HasRows;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "--CheckUnmark err!");
            }
            return Ren;
        }

        /// <summary>
        /// 检查扫描的堆垛 是否属于生产完成的批次
        /// </summary>
        /// <param name="sa"></param>
        /// <returns></returns>
        public static bool CheckCom(string sa)
        {
            bool Ren = false;
            string sql = $"select [BatchId] from [dbo].[Batch_table] where [BatchId] = (select [Batch] from [dbo].[Stack_table] where [StackId] = N'{sa}') and [Status] = 2 ";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            Ren = reader.HasRows;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "--CheckCom err!");
            }
            return Ren;
        }

        /// <summary>
        /// 堆垛信息写入Stack_table表
        /// </summary>
        /// <param name="stackModel"></param>
        public static void Insert_Stack_table(StackModel stackModel)
        {
            try
            {
                string sql = $@"INSERT INTO [StacksInfo].[dbo].[Stack_table] 
VALUES(N'{stackModel.StackId}',N'{stackModel.Batch}','{stackModel.PartID}','{stackModel.Len}','{stackModel.Width}','{stackModel.Thin}','{stackModel.Material}','{stackModel.Pos}','{stackModel.Pattern}',N'{stackModel.Map}',{stackModel.Status},{stackModel.About},GETDATE())";
                using (SqlConnection con = new SqlConnection(ConnectionStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "-Insert_Stack_table err!");
            }
        }

        /// <summary>
        /// 板料信息写入Board_table表
        /// </summary>
        /// <param name="BatchId">批次编号</param>
        /// <param name="Number">序号</param>
        /// <param name="Upi">板料编号</param>
        public static void Insert_Board_table(string BatchId, int Number, string Upi)
        {
            try
            {
                string sql = $@"INSERT INTO [StacksInfo].[dbo].[Board_table] 
    VALUES(N'{BatchId}',{Number},'{Upi}',NULL)";
                using (SqlConnection con = new SqlConnection(ConnectionStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "-Insert_Board_table err!");
            }
        }

        /// <summary>
        /// 批次数量信息写入Batch_table表
        /// </summary>
        /// <param name="BatchId">批次编号</param>
        /// <param name="BatchCount">序号</param>
        public static void Insert_Batch_table(string BatchId, int Number)
        {
            try
            {
                string sql = $@"INSERT INTO [StacksInfo].[dbo].[Batch_table]
    VALUES(N'{BatchId}','{Number}',0,0,GetDate())";
                using (SqlConnection con = new SqlConnection(ConnectionStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "-Insert_Batch_table err!");
            }
        }

        /// <summary>
        /// 检查堆垛是否存在
        /// </summary>
        /// <param name="StackId">堆垛编号</param>
        /// <returns>存在则返回true，否则false</returns>
        public static bool CheckStackInfo(string StackId)
        {
            bool Ren = false;
            string sql = $@"SELECT count(0) FROM[Stack_table]  WHERE[StackId] = '{StackId}'";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            Ren = reader.HasRows;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "--CheckStackInfo err!");
            }
            return Ren;
        }

        /// <summary>
        /// 标记堆垛进料口
        /// </summary>
        /// <param name="about">左/右</param>
        /// <returns></returns>
        public static bool UpdateAbout(int about)
        {
            bool Ren = false;
            string sql = $"UPDATE [StacksInfo].[dbo].[Stack_table] SET [About] = {about} WHERE [About] = 0 AND [Status] = 1";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        Ren = cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "--UpdateAbout err!");
            }
            return Ren;
        }

        /// <summary>
        /// 堆垛进入抓板区，修改状态
        /// </summary>
        /// <param name="about">左右</param>
        /// <param name="orig">原来的状态1</param>
        /// <param name="now">现在的状态2</param>
        /// <returns></returns>
        //public static bool UpdateStatus(int about, int orig = 1, int now = 2)
        //{
        //    bool Ren = false;
        //    string sql = $"UPDATE [StacksInfo].[dbo].[Stack_table] SET [Status] = {now} WHERE [About] = {about} AND [Status] = {orig}";
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(ConnectionStr))
        //        {
        //            using (SqlCommand cmd = new SqlCommand(sql, con))
        //            {
        //                con.Open();
        //                Ren = cmd.ExecuteNonQuery() > 0;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message + "--UpdateStatus err!");
        //    }
        //    return Ren;
        //}

        /// <summary>
        /// 堆垛进入抓板区，修改状态 和 返回堆垛id、批次id
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public static (string StackId, string BatchId) update_StackStatus(int about)
        {
            string StackId = string.Empty;
            string BatchId = string.Empty;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionStr))
                {
                    using (SqlCommand cmd = new SqlCommand("Proc_upe_StackStatus", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();

                        cmd.Parameters.Add("@about", SqlDbType.Int);
                        cmd.Parameters["@about"].Value = about;
                        cmd.Parameters.Add("@StackId", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@BatchId", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();

                        var stackId = cmd.Parameters["@StackId"].Value;
                        StackId = (stackId == DBNull.Value) ? string.Empty : stackId.ToString();
                        var batchId = cmd.Parameters["@BatchID"].Value;
                        BatchId = (batchId == DBNull.Value) ? string.Empty : batchId.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "-- upe_StackStatus err!");
            }
            return (StackId, BatchId);
        }

        public static (int, int, int,int) UpdateStatus2(string BatchID, string StackID, string PartID)
        {
            int Pos = -1;
            int About = -1;
            int BatchSurplusCount = -1;
            int StackSurplusCount = -1;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionStr))
                {
                    using (SqlCommand cmd = new SqlCommand("UpDateStatus2", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        cmd.Parameters.Add("@BatchID", SqlDbType.NVarChar);
                        cmd.Parameters["@BatchID"].Value = BatchID;
                        cmd.Parameters.Add("@StackID", SqlDbType.NVarChar);
                        cmd.Parameters["@StackID"].Value = StackID;
                        cmd.Parameters.Add("@PartID", SqlDbType.NVarChar);
                        cmd.Parameters["@PartID"].Value = PartID;

                        cmd.Parameters.Add("@Pos", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@About", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@BatchSurplusCount", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@StackSurplusCount", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();

                        var pos = cmd.Parameters["@Pos"].Value;
                        Pos = (pos == DBNull.Value) ? -1 : Convert.ToInt32(pos);
                        var about = cmd.Parameters["@About"].Value;
                        About = (about == DBNull.Value) ? -1 : Convert.ToInt32(about);
                        var Surplus = cmd.Parameters["@BatchSurplusCount"].Value;
                        BatchSurplusCount = (Surplus == DBNull.Value) ? -1 : Convert.ToInt32(Surplus);
                        var StackSurplus = cmd.Parameters["@StackSurplusCount"].Value;
                        StackSurplusCount = (StackSurplus == DBNull.Value) ? -1 : Convert.ToInt32(StackSurplus);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "--UpdateStatus2 err!");
            }
            return (Pos, About,BatchSurplusCount, StackSurplusCount);
        }

        /// <summary>
        /// 获取将要抓取的板件信息
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="stackid"></param>
        /// <returns></returns>
        public static DataTable GetBoardDeatail(int About)
        {
            DataTable dataTable = new DataTable();
            string sql = $@"SELECT TOP 1 Batch,[Len],Width,Thin,PartID,(SELECT COUNT(0) FROM [Stack_table] WHERE Batch = T1.Batch AND [Status]=3) AS Change,[StackId],[Map]
FROM[Stack_table] T1 WHERE About ={About} and  [Status] = 2 ORDER BY [Pos] DESC";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        using (SqlDataAdapter DA = new SqlDataAdapter())
                        {
                            DA.SelectCommand = cmd;
                            DA.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "--GetPartDeatail err!");
            }
            return dataTable;
        }

        //public static void UpdateStatus2(string BatchID, string StackID,string PartID)
        //{

        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(ConnectionStr))
        //        {
        //            using (SqlCommand cmd = new SqlCommand("UpDateStatus2", con))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                con.Open();
        //                cmd.Parameters.Add("@BatchID", SqlDbType.NVarChar);
        //                cmd.Parameters["@BatchID"].Value = BatchID;
        //                cmd.Parameters.Add("@StackID", SqlDbType.NVarChar);
        //                cmd.Parameters["@StackID"].Value = StackID;
        //                cmd.Parameters.Add("@PartID", SqlDbType.NVarChar);
        //                cmd.Parameters["@PartID"].Value = PartID;
        //                cmd.ExecuteNonQuery();    
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message + "--UpdateStatus2 err!");
        //    }

        //}

        /// <summary>
        /// 检查是否为新批次
        /// </summary>
        /// <param name="upi"></param>
        /// <param name="batchid"></param>
        public static (string CurBatchID, int ret) CheckNewBatchBoard(string upi)
        {
            string CurBatchID = string.Empty;
            int Ret = -1;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionStr))
                {
                    using (SqlCommand cmd = new SqlCommand("CheckNewBatchBoard", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        cmd.Parameters.Add("@upi", SqlDbType.VarChar);
                        cmd.Parameters["@upi"].Value = upi;
                        cmd.Parameters.Add("@CurBatchID", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@Ret", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();

                        var batchid = cmd.Parameters["@CurBatchID"].Value;
                        CurBatchID = (batchid == DBNull.Value) ? string.Empty : batchid.ToString();
                        Ret = (int)cmd.Parameters["@Ret"].Value;

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "--CheckNewBatchBoard err!");
            }
            return (CurBatchID, Ret);

        }

        /// <summary>
        /// 确定当前批次号
        /// </summary>
        /// <returns></returns>
        public static string CheckCurrentBatch()
        {
            string CurrentBatch = string.Empty;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionStr))
                {
                    using (SqlCommand cmd = new SqlCommand("CheckCurrentBatchID", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        cmd.Parameters.Add("@BatchID", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();

                        var currentbatch = cmd.Parameters["@BatchID"].Value;

                        CurrentBatch = (currentbatch == DBNull.Value) ? string.Empty : currentbatch.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "--CheckCurrentBatch err!");

            }
            return CurrentBatch;
        }

        /// <summary>
        /// 判断当前工作区的堆垛是否属于当前生产批次
        /// </summary>
        /// <param name="BatchID">当前生产批次号</param>
        /// <returns>属于则返回1或2，否则返回0</returns>
        public static int ExistCurrentBatch(string BatchID)
        {

            string sql = $" select top 1 [About] from [dbo].[Stack_table] where [Status] = 2 and [Batch] = '{BatchID}' order by [DateTime] asc";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    return Convert.ToInt32(reader["About"]);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "--ExistCurrentBatch err!");
            }
            return 0;
        }

        /// <summary>
        /// 修改批次状态
        /// </summary>
        /// <param name="BatchId"></param>
        /// <returns></returns>
        /// 
        public static bool Update_BatchStatus(string BatchId, int status)
        {
            bool Ren = false;
            string sql = $"update Batch_table set [Status] = {status} where [BatchId] = '{BatchId}'";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        Ren = cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "--Update_BatchStatus err!");
            }
            return Ren;
        }
        /// <summary>
        /// 查找当前批次下的板料数量
        /// </summary>
        /// <param name="BatchId"></param>
        /// <returns></returns>
        public static (int, int) QueryBatchQuantity(string BatchId)
        {
            int curBatch = -1;
            int tolBatch = -1;
            string sql = $" select top 1 [ComNum],[AllNum] from [dbo].[Batch_table] where [BatchId]='{BatchId}'";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    curBatch = Convert.ToInt32(reader["ComNum"]);
                                    tolBatch = Convert.ToInt32(reader["AllNum"]);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "--QueryBatchQuantity err!");
            }
            return (curBatch, tolBatch);

        }

        /// <summary>
        /// 获取当前在工作区的堆垛
        /// </summary>
        /// <param name="StackId"></param>
        /// <returns></returns>
        public static DataTable Get_WorkStack(string StackId = "")
        {
            DataTable dataTable = new DataTable();
            string sql = "";
            if (!string.IsNullOrWhiteSpace(StackId))
            {
                sql = $@"SELECT [Pos],[Status],[About] FROM [dbo].[Stack_table] WHERE [StackId] ='{StackId}'";
            }
            else
            {
                sql = $@"SELECT [Pos],[Status],[About] FROM [dbo].[Stack_table] WHERE [StackId] IN (SELECT [StackId] FROM [dbo].[Stack_table] WHERE [Status] = 2 GROUP BY [StackId])";
            }


            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        using (SqlDataAdapter DA = new SqlDataAdapter())
                        {
                            DA.SelectCommand = cmd;
                            DA.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "--Get_WorkStack err!");
            }
            return dataTable;
        }

        public static DataTable Get_WorkId()
        {
            DataTable dataTable = new DataTable();
            string sql = @"SELECT [StackId],[Batch],[About] FROM [dbo].[Stack_table] 
  WHERE[StackId] IN(SELECT[StackId] FROM[dbo].[Stack_table] WHERE[Status] = 2 GROUP BY[StackId])
  GROUP BY[StackId],[Batch],[About]";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        using (SqlDataAdapter DA = new SqlDataAdapter())
                        {
                            DA.SelectCommand = cmd;
                            DA.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "--Get_WorkId err!");
            }
            return dataTable;
        }

    }

}
