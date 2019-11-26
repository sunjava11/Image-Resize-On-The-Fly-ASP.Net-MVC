namespace sunjava1.ImageResize.Web.HttpHandlerCustoms
{
    public class ImageResizeCustomHttpHandler : IHttpHandler
    {
        public bool IsReusable {
        get {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {

            string fileName = System.IO.Path.GetFileName(context.Request.Url.ToString());
            string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(context.Request.Url.ToString());
            string fileExtention = System.IO.Path.GetExtension(context.Request.Url.ToString());

            string uploadFolder = HostingEnvironment.MapPath("~/uploads/");

            if (System.IO.File.Exists(uploadFolder + fileName))
            {
                StreamReader sr = new StreamReader(uploadFolder + fileName);
                
                Image originalImage = System.Drawing.Image.FromStream(sr.BaseStream, true, true);

                Bitmap resized = ResizeImage(originalImage,200,200);


                if(Directory.Exists(uploadFolder+"resized/")==false)
                {
                    Directory.CreateDirectory(uploadFolder + "resized/");
                }

                string resizedFileAbsolutePath = uploadFolder + "/resized/" + fileNameWithoutExt + fileExtention;

                resized.Save(resizedFileAbsolutePath);

                context.Response.ContentType = "image/jpg";
                context.Response.WriteFile(resizedFileAbsolutePath);
            }
        }



        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
