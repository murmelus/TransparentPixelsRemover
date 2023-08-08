using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TransparentPixelsRemover.uielements
{
    public partial class labeledImageBox : UserControl
    {
        public static readonly DependencyProperty ContainerTitleResourceProperty =
        DependencyProperty.Register("ContainerTitleResource", typeof(string), typeof(labeledImageBox), new PropertyMetadata("", OnContainerTitleResourceChanged));
        public string ContainerTitleResource
        {
            get { return (string)GetValue(ContainerTitleResourceProperty); }
            set { SetValue(ContainerTitleResourceProperty, value); }
        }

        private static void OnContainerTitleResourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is labeledImageBox userControl && e.NewValue is string resourceName)
            {
                if (Application.Current.Resources.Contains(resourceName))
                {
                    userControl.mainLabel.SetResourceReference(ContentProperty, resourceName);
                }
            }
        }

        public void setLabelTextByResource(string resourceName)
        {
            if (Application.Current.Resources.Contains(resourceName))
            {
                mainLabel.SetResourceReference(ContentProperty, resourceName);
            }
            
        } 
        public void setImage(ImageBrush imageBrush)
        {
            imageGrid.Background = imageBrush;
        }

        public void setDefaultBackground()
        {
            SolidColorBrush beigeBrush = App.Current.Resources["ColorFillImageBoxGrid"] as SolidColorBrush;

            imageGrid.Background = beigeBrush;
        }

        public int getRequiredSize()
        {
            return Convert.ToInt32(imageGrid.Width);
        }

        public labeledImageBox()
        {
            InitializeComponent();
            DataContext = this;
        }

    }
}
