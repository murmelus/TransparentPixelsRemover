using System.Windows;
using System.Windows.Controls;

namespace TransparentPixelsRemover.uielements
{
    public partial class scrollControl : UserControl
    {
        public static readonly DependencyProperty ScrollerTitleResourceProperty =
      DependencyProperty.Register("ScrollerTitleResource", typeof(string), typeof(scrollControl), new PropertyMetadata("", OnContainerTitleResourceChanged));

        public string ScrollerTitleResource
        {
            get { return (string)GetValue(ScrollerTitleResourceProperty); }
            set { SetValue(ScrollerTitleResourceProperty, value); }
        }

        private static void OnContainerTitleResourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is scrollControl userControl && e.NewValue is string resourceName)
            {
                if (Application.Current.Resources.Contains(resourceName))
                {
                    userControl.mainTextLabel.SetResourceReference(ContentProperty, resourceName);
                }
            }
        }

        public scrollControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double sliderValue = (sender as Slider).Value;

            int valueWithoutDecimal = (int)sliderValue;

            percentageLabel.Content = valueWithoutDecimal.ToString() + "%"; 
        }

        public double getThreshold()
        {
            if (mainSlider.Value == 0)
            {
                return 0.0;
            }

            return mainSlider.Value / mainSlider.Maximum;
        }
    }
}
