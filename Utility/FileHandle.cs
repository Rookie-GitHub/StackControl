using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Xml;

namespace BasicDriver
{
    class FileHandle
    {
        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="xContent">内容</param>
        /// <param name="xFilePath">路径</param>
        public static void WriteFile(string xContent, string xFilePath)
        {
            try
            {
                FileStream fs;
                if (!IsFileExists(xFilePath))
                {
                    fs = new FileStream(xFilePath, FileMode.Create);
                }
                else
                {
                    fs = new FileStream(xFilePath, FileMode.Append);
                }

                StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                sw.WriteLine(xContent);
                sw.Close();
            }
            catch (Exception ex)
            {
               // LogHandle.WriteLog(3, Globle.LOG_TYPE_ERROR, "WriteFile:" + ex.Message);
            }
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="xPath">文件路径</param>
        /// <returns></returns>
        public static bool IsFileExists(string xPath)
        {
            try
            {
                if (File.Exists(xPath))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                LogHandle.WriteLog(3, Globle.LOG_TYPE_ERROR, "IsFileExists" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 保存xml文件
        /// </summary>
        /// <param name="ConnenctionString">连接字符串</param>
        /// <param name="strKey">目标元素</param>
        /// <returns></returns>
        public bool SaveXmlFile(string ConnenctionString, string strKey)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                //获得配置文件的全路径
                string strFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                doc.Load(strFileName);
                //找出名称为“add”的所有元素
                XmlNodeList nodes = doc.GetElementsByTagName("add");
                for (int i = 0; i < nodes.Count; i++)
                {
                    //获得将当前元素的key属性
                    XmlAttribute att = nodes[i].Attributes["key"];
                    //根据元素的第一个属性来判断当前的元素是不是目标元素
                    if (att.Value == strKey)
                    {
                        //对目标元素中的第二个属性赋值
                        att = nodes[i].Attributes["value"];
                        att.Value = ConnenctionString;
                        break;
                    }
                }
                //保存上面的修改
                doc.Save(strFileName);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
