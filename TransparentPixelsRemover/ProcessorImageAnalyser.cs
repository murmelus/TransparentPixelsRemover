using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TransparentPixelsRemover
{
    class ProcessorImageAnalyser : IHalfTransparentAnalyser
    {
        BitmapImage bitmap;
        double transparentThreshold=70;

        public void setBitmap(BitmapImage bitmap) 
        {
            this.bitmap = bitmap;
        }

        public void setTransparentThreshold(double transparentThreshold)
        {
            if (transparentThreshold ==-10)
            {
                this.transparentThreshold = 0.02;
            }
            else
            {
                this.transparentThreshold = transparentThreshold;
            }
            
        }

        public BitmapImage CreateTransparencyMap()
        {
            FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);
            int bytesPerPixel = (convertedBitmap.Format.BitsPerPixel + 7) / 8;
            
            byte[] pixels = new byte[convertedBitmap.PixelWidth * convertedBitmap.PixelHeight * bytesPerPixel];
            convertedBitmap.CopyPixels(pixels, convertedBitmap.PixelWidth * bytesPerPixel, 0);

            Color colorForHalfTransparent = (Color)App.Current.Resources["OpacityMaskForHalfTransparent"];
            Color colorForNonTransparent = (Color)App.Current.Resources["OpacityMaskForNonTransparent"];
            Color colorForTransparent = (Color)App.Current.Resources["OpacityMaskForTransparent"];

            for (int y = 0; y < convertedBitmap.PixelHeight; y++)
            {
                for (int x = 0; x < convertedBitmap.PixelWidth; x++)
                {
                    int index = (y * convertedBitmap.PixelWidth + x) * bytesPerPixel;
                    byte alpha = pixels[index + 3];

                    if (alpha == 255)
                    {
                        pixels[index] = colorForNonTransparent.B; 
                        pixels[index + 1] = colorForNonTransparent.G;
                        pixels[index + 2] = colorForNonTransparent.R;
                    }
                    else if(alpha > 0)
                    {

                        pixels[index] = colorForHalfTransparent.B;
                        pixels[index + 1] = colorForHalfTransparent.G;
                        pixels[index + 2] = colorForHalfTransparent.R;
                        pixels[index + 3] = 255;
                    }
                    else
                    {
                        pixels[index] = colorForTransparent.B;
                        pixels[index + 1] = colorForTransparent.G;
                        pixels[index + 2] = colorForTransparent.R;
                        pixels[index + 3] = 255;
                    }
                }
            }

            BitmapImage resultBitmapImage = translateToBitmapImage(convertedBitmap,bytesPerPixel,pixels);

            return resultBitmapImage;
        }

        public BitmapImage RemoveTransparentThreshold()
        {
            FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);
            int bytesPerPixel = (convertedBitmap.Format.BitsPerPixel + 7) / 8;

            byte[] pixels = new byte[convertedBitmap.PixelWidth * convertedBitmap.PixelHeight * bytesPerPixel];
            convertedBitmap.CopyPixels(pixels, convertedBitmap.PixelWidth * bytesPerPixel, 0);

            for (int y = 0; y < convertedBitmap.PixelHeight; y++)
            {
                for (int x = 0; x < convertedBitmap.PixelWidth; x++)
                {
                    int index = (y * convertedBitmap.PixelWidth + x) * bytesPerPixel;
                    byte alpha = pixels[index + 3];

                    if (alpha <= (byte)((1-transparentThreshold) * 255))
                    {
                        pixels[index + 3] = 0; 
                    }
                    else
                    {
                        pixels[index + 3] = 255; 
                    }
                    
                }
            }

            BitmapImage resultBitmapImage = translateToBitmapImage(convertedBitmap, bytesPerPixel, pixels);

            pixels = null;

            return resultBitmapImage;
        }

        public BitmapImage Create_GM_Preview()
        {
            FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);
            int bytesPerPixel = (convertedBitmap.Format.BitsPerPixel + 7) / 8;

            byte[] pixels = new byte[convertedBitmap.PixelWidth * convertedBitmap.PixelHeight * bytesPerPixel];
            convertedBitmap.CopyPixels(pixels, convertedBitmap.PixelWidth * bytesPerPixel, 0);

            for (int y = 0; y < convertedBitmap.PixelHeight; y++)
            {
                for (int x = 0; x < convertedBitmap.PixelWidth; x++)
                {
                    int index = (y * convertedBitmap.PixelWidth + x) * bytesPerPixel;
                    byte alpha = pixels[index + 3];

                    if (alpha < 255 && alpha > 0)
                    {
                        pixels[index + 3] = 255;
                    }
                    
                    if(alpha == 0)
                    {
                        pixels[index] = 255; 
                        pixels[index + 1] = 255; 
                        pixels[index + 2] = 255;
                        pixels[index + 3] = 255;
                    }
                }
            }

            BitmapImage resultBitmapImage = translateToBitmapImage(convertedBitmap, bytesPerPixel, pixels);

            return resultBitmapImage;
        }

        public double countPercentOfTranslucent()
        {

            FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);
            int bytesPerPixel = (convertedBitmap.Format.BitsPerPixel + 7) / 8;

            int totalPixels = convertedBitmap.PixelWidth * convertedBitmap.PixelHeight;

            byte[] pixels = new byte[totalPixels * bytesPerPixel];
            convertedBitmap.CopyPixels(pixels, convertedBitmap.PixelWidth * bytesPerPixel, 0);

            int countOfHalfTransparent = 0;

            for (int y = 0; y < convertedBitmap.PixelHeight; y++)
            {
                for (int x = 0; x < convertedBitmap.PixelWidth; x++)
                {
                    int index = (y * convertedBitmap.PixelWidth + x) * bytesPerPixel;
                    byte alpha = pixels[index + 3];

                    if (alpha < 255 && alpha > 0)
                    {
                        countOfHalfTransparent++;
                    }

                }
            }

            return Convert.ToDouble(countOfHalfTransparent)/ Convert.ToDouble(totalPixels);
          
        }

        private BitmapImage translateToBitmapImage(FormatConvertedBitmap bitmap, int bytesPerPixel, byte[] pixels)
        {
            BitmapSource resultBitmapSource = BitmapSource.Create(
                bitmap.PixelWidth,
                bitmap.PixelHeight,
                bitmap.DpiX, bitmap.DpiY,
                PixelFormats.Bgra32, null,
                pixels, bitmap.PixelWidth * bytesPerPixel);

            BitmapImage resultBitmapImage = new BitmapImage();
            var memoryStream = new System.IO.MemoryStream();
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(resultBitmapSource));
            encoder.Save(memoryStream);
            memoryStream.Position = 0;
            resultBitmapImage.BeginInit();
            resultBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            resultBitmapImage.StreamSource = memoryStream;
            resultBitmapImage.EndInit();
            resultBitmapImage.Freeze();

            return resultBitmapImage;
        }

       
    }
}
