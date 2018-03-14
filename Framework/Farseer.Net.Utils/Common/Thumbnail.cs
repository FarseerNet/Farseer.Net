using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace FS.Utils.Common
{
    /// <summary>
    ///     Thumbnail 的摘要说明。
    /// </summary>
    public abstract class Thumbnail
    {
        /// <summary>
        ///     水印位置
        /// </summary>
        public enum MarkPlace
        {
            /// <summary>
            ///     左上角
            /// </summary>
            [Display(Name = "左上角")]
            LeftTop,

            /// <summary>
            ///     右上角
            /// </summary>
            [Display(Name = "右上角")]
            RightTop,

            /// <summary>
            ///     左下角
            /// </summary>
            [Display(Name = "左下角")]
            LeftBottom,

            /// <summary>
            ///     右下角
            /// </summary>
            [Display(Name = "右下角")]
            RightBottom,

            /// <summary>
            ///     图片中央
            /// </summary>
            [Display(Name = "图片中央")]
            Center
        }

        /// <summary>
        ///     图片尺寸类型
        /// </summary>
        public enum ThumbnailType
        {
            /// <summary>
            ///     指定宽,高按比例
            /// </summary>
            W,

            /// <summary>
            ///     指定高，宽按比例
            /// </summary>
            H,

            /// <summary>
            ///     取最小值
            /// </summary>
            Min,

            /// <summary>
            ///     取最大值
            /// </summary>
            Max,

            /// <summary>
            ///     按最大值缩放再裁剪，不变形
            /// </summary>
            Cut
        }

        /// <summary>
        ///     创建缩略图（缩略图存在时、原图片不存在时，不生成）
        /// </summary>
        /// <param name="oldImagePath">原始图片</param>
        /// <param name="newImagePath">新图片</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="level">图片质量: 1 - 100</param>
        /// <param name="mode">图片尺寸类型 </param>
        public static byte[] MakeThumbnail(string oldImagePath, string newImagePath, int width, int height, ThumbnailType mode, int level)
        {
            newImagePath = newImagePath.Replace("\\", "/");

            // 判断原图片是否存在
            if (!File.Exists(oldImagePath)) { return null; }
            // 判断缩略图存在时，则直接返回
            if (File.Exists(newImagePath)) { return File.ReadAllBytes(newImagePath); }
            // 创建文件夹 
            Directory.CreateDirectory(newImagePath.Substring(0, newImagePath.LastIndexOf("/")));

            using (var oldImage = Image.FromFile(oldImagePath))
            {
                var toWidth = width = oldImage.Width >= width ? width : oldImage.Width;
                var toHeight = height = oldImage.Height >= height ? height : oldImage.Height;

                #region 计算宽高

                switch (mode)
                {
                    case ThumbnailType.W:
                        toHeight = oldImage.Height * width / oldImage.Width;
                        break;
                    case ThumbnailType.H:
                        toWidth = oldImage.Width * height / oldImage.Height;
                        break;
                    case ThumbnailType.Min:
                        toHeight = oldImage.Height * width / oldImage.Width;
                        if (toHeight > height)
                        {
                            toWidth = oldImage.Width * height / oldImage.Height;
                            toHeight = height;
                        }
                        break;
                    case ThumbnailType.Max: //宽度高度取最大值
                        toHeight = oldImage.Height * width / oldImage.Width;
                        if (toHeight < height)
                        {
                            toWidth = oldImage.Width * height / oldImage.Height;
                            toHeight = height;
                        }
                        break;
                }
                // 如果尺寸没变，则做复制操作
                if (toWidth == oldImage.Width && toHeight == oldImage.Height)
                {
                    File.Copy(oldImagePath, newImagePath, true);
                    return File.ReadAllBytes(newImagePath);
                }

                #endregion

                using (var bm = new Bitmap(toWidth, toHeight))
                {
                    using (var g = Graphics.FromImage(bm))
                    {
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.High;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.Clear(Color.White);
                        g.DrawImage(oldImage, new Rectangle(0, 0, toWidth, toHeight), new Rectangle(0, 0, oldImage.Width, oldImage.Height), GraphicsUnit.Pixel);
                        g.Dispose();
                    }
                    SetQuality(level);
                    bm.Save(newImagePath, Ici, Ep);
                    bm.Dispose();
                }
                oldImage.Dispose();
            }
            if (mode == ThumbnailType.Cut) { Cut(newImagePath, 0, 0, width, height); }

            return File.ReadAllBytes(newImagePath);
        }

        /// <summary>
        ///     裁剪图片
        /// </summary>
        /// <param name="pic">图片路径</param>
        /// <param name="left">左边距</param>
        /// <param name="top">上边距</param>
        /// <param name="width">裁剪后宽度</param>
        /// <param name="height">裁剪后高度</param>
        public static void Cut(string pic, int left, int top, int width, int height)
        {
            using (Image bm = new Bitmap(width, height))
            {
                string tmpName;
                using (var image = Image.FromFile(pic))
                {
                    using (var g = Graphics.FromImage(bm))
                    {
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.High;
                        g.SmoothingMode = SmoothingMode.HighQuality;

                        g.Clear(Color.White);
                        //在指定位置并且按指定大小绘制原图片的指定部分 
                        g.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(left, top, width, height), GraphicsUnit.Pixel);
                        tmpName = pic + ".jpg";
                        SetQuality(100);
                        bm.Save(tmpName, Ici, Ep);
                    }
                }
                File.Delete(pic);
                File.Move(tmpName, pic);
            }
        }

        /// <summary>
        ///     图片水印
        /// </summary>
        /// <param name="pic">原始图片</param>
        /// <param name="markImage">水印</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        public static void Mark(string pic, Image markImage, int x, int y)
        {
            string tmpName;
            using (var image = Image.FromFile(pic))
            {
                if (markImage.Width > image.Width || markImage.Height > image.Height)
                {
                    markImage.Dispose();
                    return;
                }
                using (var bitmap = new Bitmap(image.Width, image.Height))
                {
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        g.DrawImage(image, 0, 0, image.Width, image.Height);
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawImage(markImage, new Rectangle(x, y, markImage.Width, markImage.Height), 0, 0, markImage.Width, markImage.Height, GraphicsUnit.Pixel);
                        markImage.Dispose();
                    }
                    tmpName = pic + ".tmp";
                    SetQuality(100);
                    bitmap.Save(tmpName, Ici, Ep);
                }
            }
            File.Delete(pic);
            File.Move(tmpName, pic);
        }

        /// <summary>
        ///     水印
        /// </summary>
        /// <param name="pic">原始图片</param>
        /// <param name="markImage">水印</param>
        /// <param name="place">水印位置</param>
        public static void Mark(string pic, Image markImage, MarkPlace place)
        {
            int y;
            var x = y = 0;
            using (var image = Image.FromFile(pic))
            {
                switch (place)
                {
                    case MarkPlace.RightBottom:
                        x = image.Width - markImage.Width - 2;
                        y = image.Height - markImage.Height - 2;
                        break;
                    case MarkPlace.LeftBottom:
                        x = 12;
                        y = image.Height - markImage.Height - 12;
                        break;
                    case MarkPlace.LeftTop:
                        x = y = 12;
                        break;
                    case MarkPlace.RightTop:
                        x = image.Width - markImage.Width - 12;
                        y = 12;
                        break;
                    case MarkPlace.Center:
                        x = (image.Width - markImage.Width) / 2;
                        y = (image.Height - markImage.Height) / 2;
                        break;
                }
            }
            Mark(pic, markImage, x, y);
            markImage.Dispose();
        }

        /// <summary>
        ///     图片水印
        /// </summary>
        /// <param name="pic">原始图片</param>
        /// <param name="markImagePath">水印路径</param>
        /// <param name="place">水印位置</param>
        public static void Mark(string pic, string markImagePath, MarkPlace place)
        {
            using (var mark = Image.FromFile(markImagePath)) { Mark(pic, mark, place); }
        }

        /// <summary>
        ///     文字水印
        /// </summary>
        /// <param name="pic">原始图片</param>
        /// <param name="text">水印文字</param>
        /// <param name="place">水印位置</param>
        /// <param name="fontfamily">文字字体</param>
        /// <param name="fontsize">文字大小</param>
        /// <param name="fontcolor">文字颜色</param>
        public static void Mark(string pic, string text, MarkPlace place, string fontfamily, int? fontsize, Brush fontcolor)
        {
            using (Image txt = ToPicture(text, fontfamily, fontsize, fontcolor)) { Mark(pic, txt, place); }
        }

        /// <summary>
        ///     转化文本为Bitmap
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="fontfamily">字体</param>
        /// <param name="fontsize">字号</param>
        /// <param name="fontcolor">文字颜色</param>
        /// <returns></returns>
        public static Bitmap ToPicture(string text, string fontfamily, int? fontsize, Brush fontcolor)
        {
            if (fontcolor == null)
                fontcolor = Brushes.White;
            if (fontfamily == null)
                fontfamily = "宋体";
            if (fontsize == null)
                fontsize = 20;
            var font = new Font(fontfamily, (float)fontsize);
            var bmp = new Bitmap(10, 10);
            var ImageSize = Size.Empty;
            // 计算图片大小
            using (var g = Graphics.FromImage(bmp))
            {
                var size = g.MeasureString(text, font, 10000);
                ImageSize.Width = (int)size.Width + 5;
                ImageSize.Height = (int)size.Height + 5;
            }
            // 创建图片
            bmp = new Bitmap(ImageSize.Width, ImageSize.Height);
            // 绘制文本
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;
                using (var f = new StringFormat())
                {
                    f.Alignment = StringAlignment.Center;
                    f.LineAlignment = StringAlignment.Center;
                    //f.FormatFlags = StringFormatFlags.NoWrap;
                    g.DrawString(text, font, fontcolor, new RectangleF(2, 2, ImageSize.Width, ImageSize.Height), f);
                }
            }
            return bmp;
        }

        /// <summary>
        ///     图片叠加
        /// </summary>
        /// <param name="width">画布宽度</param>
        /// <param name="height">画布高度</param>
        /// <param name="piclist">图片集合</param>
        /// <param name="textlist">文本集合</param>
        /// <param name="path">图片保存路径</param>
        public static void AppendPicture(int width, int height, List<Picture> piclist, List<Text> textlist, string path)
        {
            using (Image frm = new Bitmap(width, height))
            {
                using (var g = Graphics.FromImage(frm))
                {
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.High;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.Clear(Color.White);
                    Image image;
                    foreach (var p in piclist) { using (image = Image.FromFile(p.Path)) { g.DrawImage(image, new Rectangle(p.Left, p.Top, width, height), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel); } }
                    if (textlist.Count > 0) { foreach (var p in textlist) { using (image = ToPicture(p.Txt, p.FontFamily, p.FontSize, p.FontColor)) { g.DrawImage(image, new Rectangle(p.Left, p.Top, width, height), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel); } } }
                    SetQuality(100);
                    frm.Save(path, Ici, Ep);
                }
            }
        }

        #region 控制图片质量

        /// <summary>
        ///     未解释
        /// </summary>
        public static ImageCodecInfo Ici;

        /// <summary>
        ///     未解释
        /// </summary>
        public static EncoderParameters Ep;

        /// <summary>
        ///     设置质量等级
        /// </summary>
        /// <param name="level">质量等级</param>
        public static void SetQuality(int level)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            Ici = null;
            foreach (var codec in codecs)
            {
                if (codec.MimeType == "image/jpeg")
                    Ici = codec;
            }
            Ep = new EncoderParameters { Param = { [0] = new EncoderParameter(Encoder.Quality, level) } };
        }

        #endregion
    }

    /// <summary>
    ///     图片实体
    /// </summary>
    public class Picture
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="left">左距离</param>
        /// <param name="top">顶部距离</param>
        /// <param name="path">路径</param>
        public Picture(int left, int top, string path)
        {
            Left = left;
            Top = top;
            Path = path;
        }

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="place"></param>
        /// <param name="path">路径</param>
        public Picture(Thumbnail.MarkPlace place, string path)
        {
            switch (place)
            {
                case Thumbnail.MarkPlace.LeftTop:
                    Left = 12;
                    Top = 60;
                    break;
                default:
                    Left = 300;
                    Top = 12;
                    break;
            }
            Path = path;
        }

        /// <summary>
        ///     左距离
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        ///     顶部距离
        /// </summary>
        public int Top { get; set; }

        /// <summary>
        ///     路径
        /// </summary>
        public string Path { get; set; }
    }

    /// <summary>
    ///     文本实体
    /// </summary>
    public class Text
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="left">左距离</param>
        /// <param name="top">顶部距离</param>
        /// <param name="txt">文字</param>
        /// <param name="fontfamily">文字类型</param>
        /// <param name="fontsize">文字大小</param>
        /// <param name="fontcolor">文字颜色</param>
        public Text(int left, int top, string txt, string fontfamily, int? fontsize, Brush fontcolor)
        {
            Left = left;
            Top = top;
            Txt = txt;
            FontFamily = fontfamily ?? " 宋体";
            FontSize = fontsize ?? 12;
            FontColor = fontcolor ?? Brushes.White;
        }

        /// <summary>
        ///     左距离
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        ///     顶部距离
        /// </summary>
        public int Top { get; set; }

        /// <summary>
        ///     文件
        /// </summary>
        public string Txt { get; set; }

        /// <summary>
        ///     字体类型
        /// </summary>
        public string FontFamily { get; set; }

        /// <summary>
        ///     字体大小
        /// </summary>
        public int? FontSize { get; set; }

        /// <summary>
        ///     字体颜色
        /// </summary>
        public Brush FontColor { get; set; }
    }
}