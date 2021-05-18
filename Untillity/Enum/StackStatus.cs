using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// The Status of the PlateStack
    /// </summary>
    public enum StackStatus
    {
        数据解析并存储完成 = 0,
        抓板区 = 1,
        单板开始抓板 = 2,
        整垛开始抓板 = 2,
        单板抓板完成 = 3,
        整垛抓板完成 = 3
    }
}
