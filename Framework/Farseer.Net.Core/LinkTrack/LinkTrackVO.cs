namespace FS.Core.LinkTrack
{
    public class LinkTrackVO
    {
        /// <summary>
        /// 提前定义好10个背景色
        /// </summary>
        public static string[] RgbaList = {"95,184,120,0.4", "65,105,225,0.4", "219,112,147,0.4", "128,0,128,0.4", "153,50,204,0.4", "123,104,238,0.4", "119,136,153,0.4", "70,130,180,0.4", "0,139,139,0.4", "34,139,34,0.4", "128,128,0,0.4", "238,232,170,0.4", "218,165,32,0.4", "255,165,0,0.4", "255,140,0,0.4", "210,105,30,0.4"};

        /// <summary>
        /// 时间轴的背景色
        /// </summary>
        public string Rgba { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 时间轴位置
        /// </summary>
        public long   StartTs { get; set; }
        public long   UseTs   { get; set; }
        public string Caption { get; set; }
        public string AppId   { get; set; }
    }
}