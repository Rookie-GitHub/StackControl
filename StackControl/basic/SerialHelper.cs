using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SerialCom.SerialLogic.SerialPortHelper;

namespace SerialCom.SerialLogic
{
    public class SerialHelper:IDisposable
    {
        private SerialPortHelper serialPortHelper;
       
        public SerialHelper()
        {

        }
        public SerialHelper(string portname, int baudRate, int parity, int databits, int stopbits ,EventHandler<SerialPortRecvEventArgs> RecEvent )
        {
            serialPortHelper = new SerialPortHelper();
            //serialPortHelper.OpenSerialPort(/*串口号*/, 115200/*波特率*/, 0/*校验位*/, 8/*数据位*/, 1/*停止位*/);
            serialPortHelper.OpenSerialPort(portname/*串口号*/, baudRate/*波特率*/, parity/*校验位*/, databits/*数据位*/, stopbits/*停止位*/);
            // 订阅事件 可以放在 Form_Load 中 或者其他函数中，但必须执行
            serialPortHelper.ReceivedDataEvent += RecEvent;
        }

        public void Dispose()
        {
            serialPortHelper.CloseSerialPort();
        } 
    }
}
