using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Untillity
{
    /// <summary>
    /// The status of the ScanQRCode
    /// </summary>
    public enum ScanQRCodeStatus
    {
        扫描完成 = 1,
        数据解析并存储完成 = 2,
        上线完成 = 3
    }
}
