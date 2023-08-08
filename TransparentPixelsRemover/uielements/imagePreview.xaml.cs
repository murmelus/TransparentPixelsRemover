using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TransparentPixelsRemover.uielements
{
    public partial class imagePreview : UserControl
    {
        private imagesPreviewContainer _imagePreviewContainer;

        public imagesPreviewContainer ImagePreviewContainer
        {
            set
            {
                _imagePreviewContainer = value;
            }
        }

        imageDetails imagePreviewInfo;
        public imagePreview()
        {
            InitializeComponent();
        }

        public void setImagePreviewInfo(imageDetails imagePreviewInfo)
        {
            this.imagePreviewInfo = imagePreviewInfo;

            reloadPreviewInfo();
        }

        public imageDetails getImagePreviewInfo()
        {
            return imagePreviewInfo;
        }

        private void reloadPreviewInfo()
        {
            imageBox.Background = imagePreviewInfo.previewImage;
            imageName.Content = imagePreviewInfo.imageName;
            imagePath.Content = imagePreviewInfo.imageFolder;
        }

        public void setSelectedMode(object sender, MouseButtonEventArgs e)
        {
            if (selectionGrid.Visibility == Visibility.Hidden)
            {
                _imagePreviewContainer.addSelectedPreview(this);
                setSelectedMode();
            }
            else
            {
                _imagePreviewContainer.removeSelectedPreview(this);
                setUnselectedMode();
            }
        }

        public void setSelectedMode()
        {
            selectionGrid.Visibility = Visibility.Visible;
            
        }

        public void setUnselectedMode()
        {
            selectionGrid.Visibility = Visibility.Hidden;
        }

        public void setUnactiveAnalyseStyle()
        {
            currentAnalyseImageGrid.Visibility = Visibility.Hidden;
        }

        private void ImagePreview_LeftClick(object sender, MouseButtonEventArgs e)
        {
            if (currentAnalyseImageGrid.Visibility == Visibility.Hidden)
            {
                currentAnalyseImageGrid.Visibility = Visibility.Visible;
                _imagePreviewContainer.setCurrentAnalyseImage(this);
            }
            else
            {
                currentAnalyseImageGrid.Visibility = Visibility.Hidden;
            }
            
        }
    }
}
