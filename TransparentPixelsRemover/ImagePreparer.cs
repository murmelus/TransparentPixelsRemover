using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace TransparentPixelsRemover
{
    public static class ImagePreparer
    {
        public static ImageBrush ImageBrushFromBitmapImage(BitmapImage sourceImage, int size)
        {
            double scaleFactor = (double)size / Math.Min(sourceImage.PixelWidth, sourceImage.PixelHeight);

            int width = (int)(sourceImage.PixelWidth * scaleFactor);
            int height = (int)(sourceImage.PixelHeight * scaleFactor);

            TransformedBitmap transformedBitmap = new TransformedBitmap(sourceImage, new ScaleTransform(scaleFactor, scaleFactor));

            CroppedBitmap croppedBitmap = new CroppedBitmap(transformedBitmap, new Int32Rect(0, 0, width, height));

            BitmapFrame thumbnailFrame = BitmapFrame.Create(croppedBitmap);

            return new ImageBrush(thumbnailFrame)
            {
                AlignmentX = AlignmentX.Center,
                AlignmentY = AlignmentY.Center,

                Stretch = Stretch.Uniform
            };
        }

        public static BitmapImage GetBitmapImageFromFile(string imagePath)
        {
            BitmapImage originalImage = new BitmapImage();
            originalImage.BeginInit();
            originalImage.UriSource = new Uri(imagePath);
            originalImage.CacheOption = BitmapCacheOption.None;
            originalImage.EndInit();

            return originalImage;

        }

    }
}
