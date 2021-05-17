using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Untillity
{
    /// <summary>
    /// The Status of the PlateStack
    /// </summary>
    public enum StackStatus
    {
        数据解析并存储完成 = 0,
        抓板区 = 1,
        开始抓板 = 2,
        抓板完成 = 3
    }
}
