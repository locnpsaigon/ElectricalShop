using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Web;

namespace ElectricalShop.Common
{
    public class ImageExtension
    {
        public static bool IsImageUploaded(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" }; // add more if u like...

            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        public static Image ResizeImage(Image sourceImage, Size size)
        {
            return (Image)(new Bitmap(sourceImage, size));
        }

        public static Image ScaleImage(Image sourceImage, int width)
        {
            float aspectRatio = (float)width / (float)sourceImage.Size.Width;
            Size newSize = new Size(width, (int)(sourceImage.Size.Height * aspectRatio));
            return ResizeImage(sourceImage, newSize);
        }

        public static string SaveImageToDisk(Image image, string uploadPath, String filename)
        {
            var filePath = Path.Combine(HttpContext.Current.Server.MapPath(uploadPath) + filename);

            var duplicateCount = 0;
            while (System.IO.File.Exists(filePath))
            {
                duplicateCount++;

                var fileName = Path.GetFileNameWithoutExtension(filename) + duplicateCount.ToString();
                var fileExt = Path.GetExtension(filename);
                filePath = Path.Combine(HttpContext.Current.Server.MapPath(uploadPath) + fileName + fileExt);
            }

            // save file 
            image.Save(filePath);

            return Path.GetFileName(filePath);
        }

    }
}