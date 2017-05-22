#region NameSpace
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Mvc;
#endregion

#region MainCode
namespace Sitecore.Recaptcha.ExecuteResult
{
    public class ImageResult : ActionResult
    {
        private readonly string _captcha;

        #region Constructor
        /// <summary>
        /// Image Result Constructor
        /// </summary>
        /// <param name="CaptchaCode">Alpha-Numeric Captcha Code</param>
        /// <param name="SessionName">Session Name</param>
        public ImageResult(string CaptchaCode)
        {
            _captcha = CaptchaCode;
        }
        #endregion

        #region ExecuteResult
        /// <summary>
        /// Execute Result Method
        /// </summary>
        /// <param name="context">Controller Context</param>
        [ValidateAntiForgeryToken]
        public override void ExecuteResult(ControllerContext context)
        {
            Color BackColor = Color.DarkGray;
            //string FontName = "Times New Roman";
            int FontSize = 11;
            int Height = 38;
            int Width = 80;

            using (Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                Color color = Color.Black;
                Font font = new Font(FontFamily.GenericSansSerif, FontSize, FontStyle.Regular);

                SolidBrush BrushBackColor = new SolidBrush(BackColor);
                Pen BorderPen = new Pen(color);

                Rectangle displayRectangle = new Rectangle(new Point(0, 0), new Size(Width - 1, Height - 1));

                graphics.FillRectangle(BrushBackColor, displayRectangle);
                graphics.DrawRectangle(BorderPen, displayRectangle);

                graphics.DrawString(_captcha, font, Brushes.Black, 14, 9);
                context.HttpContext.Response.ContentType = "image/jpg";

                bitmap.Save(context.HttpContext.Response.OutputStream, ImageFormat.Jpeg);
                bitmap.Dispose();
            }
        }
        #endregion
    }
}
#endregion