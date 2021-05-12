namespace StackControl.Models
{
    public class StackModel
    {
        /// <summary>
        /// 堆垛编号
        /// </summary>
        public string StackId { get; set; }
        /// <summary>
        /// 批次编号
        /// </summary>
        public string Batch { get; set; }
        /// <summary>
        /// 板件编号
        /// </summary>
        public int PartID { get; set; }
        /// <summary>
        /// 长
        /// </summary>
        public int Len { get; set; }
        /// <summary>
        /// 宽
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 厚
        /// </summary>
        public int Thin { get; set; }
        /// <summary>
        /// 花色
        /// </summary>
        public string Material { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int Pos { get; set; }
        /// <summary>
        /// 切割图号
        /// </summary>
        public int Pattern { get; set; }
        /// <summary>
        /// Map
        /// </summary>
        public string Map { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 上料位置 左/右
        /// </summary>
        public int? About { get; set; }

    }
}
