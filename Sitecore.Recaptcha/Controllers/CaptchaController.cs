using Sitecore.Recaptcha.ExecuteResult;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sitecore.Recaptcha.Controllers
{
    public class CaptchaController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReCaptcha()
        {
            var captchaSettings = Sitecore.Context.Database.GetItem("/sitecore/content/Home/CaptchaSettings");
            return new ImageResult(GenerateRandomNumber(captchaSettings.Fields["GenerateAlphaNumeric"].Value == "1", System.Convert.ToInt32(captchaSettings.Fields["TotalDigit"].Value)));
        }

        public ContentResult RegenerateCaptcha()
        {
            var captchaSettings = Sitecore.Context.Database.GetItem("/sitecore/content/Home/CaptchaSettings");
            var captcha = GenerateRandomNumber(captchaSettings.Fields["GenerateAlphaNumeric"].Value == "1", System.Convert.ToInt32(captchaSettings.Fields["TotalDigit"].Value));
            var byteArray = RegenerateCaptcha(captcha);
            return Content(System.Convert.ToBase64String(byteArray));
        }

        #region GenerateRandomNumber
        /// <summary>
        /// Generate Random Alpha Numeric String
        /// </summary>
        /// <param name="generateAlphaNumeric">True / False to generate Alpha Numeric Captcha Code</param>
        /// <param name="totalDigit">Total Number of digits of Captcha Code</param>
        /// <returns>Random Captcha Code</returns>
        public static string GenerateRandomNumber(bool generateAlphaNumeric, int totalDigit)
        {
            string alphabets = "ABCDEFGHIJKLMNPQRSTUVWXYZ";
            string small_alphabets = "abcdefghijklmnpqrstuvwxyz";
            string numbers = "123456789";
            string characters = numbers;

            if (generateAlphaNumeric)
            {
                characters += alphabets + small_alphabets + numbers;
            }

            int length = totalDigit;
            string otp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character) != -1);
                otp += character;
            }

            return otp;
        }
        #endregion

        #region RegenerateCaptcha
        /// <summary>
        /// Regenerate Captcha Code
        /// </summary>
        /// <param name="_captcha">Alpha Numeric Captcha Code</param>
        /// <returns>Bytes of Alhpa-Numeric Captcha Code</returns>
        public static byte[] RegenerateCaptcha(string _captcha)
        {
            byte[] bytearray = null;
            int Height = 38;
            int Width = 80;

            using (Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                int FontSize = 11;
                Color color = Color.Black;
                Font font = new Font(FontFamily.GenericSansSerif, FontSize, FontStyle.Regular);
                SolidBrush BrushBackColor = new SolidBrush(Color.DarkGray);
                Pen BorderPen = new Pen(color);

                Rectangle displayRectangle = new Rectangle(new Point(0, 0), new Size(Width - 1, Height - 1));

                graphics.FillRectangle(BrushBackColor, displayRectangle);
                graphics.DrawRectangle(BorderPen, displayRectangle);

                graphics.DrawString(_captcha, font, Brushes.Black, 14, 9);

                var Fs = new FileStream(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\Captcha" + _captcha + ".jpg", FileMode.Create);
                bitmap.Save(Fs, ImageFormat.Jpeg);
                bitmap.Dispose();

                Image img = Image.FromStream(Fs);
                Fs.Close();
                Fs.Dispose();

                bytearray = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\Captcha" + _captcha + ".jpg");
                System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\Captcha" + _captcha + ".jpg");
            }

            return bytearray;
        }
        #endregion
    }
}