using System.Windows.Media.Imaging;

namespace TransparentPixelsRemover
{
    public interface IHalfTransparentAnalyser
    {
        public void setBitmap(BitmapImage bitmap);

        public void setTransparentThreshold(double transparentThreshold);

        public BitmapImage CreateTransparencyMap();

        public BitmapImage RemoveTransparentThreshold();

        public BitmapImage Create_GM_Preview();

        public double countPercentOfTranslucent();
    }
}
