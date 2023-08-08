using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using TransparentPixelsRemover.uielements;

namespace TransparentPixelsRemover
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IHalfTransparentAnalyser analyser;
        ResourceSettingsManager langManager;
        ResourceSettingsManager themeManager;
        UserConfigManager userConfigManager;
        public MainWindow()
        {
            InitializeComponent();

            analyser = new ProcessorImageAnalyser();

            imagesPreviewBox.changeCurrentFolder += previewAnalyseImage;
            imagesPreviewBox.itemSelected += previewAnalyseImage;
            imagesPreviewBox.clearBox += clearAnalyserImageBackground;

            langManager = new ResourceSettingsManager("languages\\langInit.json", "__LanguageSettings__");
            themeManager = new ResourceSettingsManager("themes\\themeInit.json", "__ThemeSettings__");
            userConfigManager = new UserConfigManager("userSettings.json");
        }

        private void clearAnalyserImageBackground()
        {
            
            selectedImageBefore.setDefaultBackground();
            selectedImagetOpacityMap.setDefaultBackground();
            selectedImageAfter.setDefaultBackground();
            selectedImageGMView.setDefaultBackground();
            selectedImagetOpacityMapAfter.setDefaultBackground();
            selectedImageGMViewAfter.setDefaultBackground();
            translucentPixelsPercentage.Content = "0%";
        }

        private void previewAnalyseImage()
        {
            BitmapImage? selectedImage = imagesPreviewBox.getLastSelectedImageOrFirst();

            if (selectedImage is null)
            {
                return;
            }

            analyser.setBitmap(selectedImage);

            double threshold = theScrollControl.getThreshold();

            analyser.setTransparentThreshold(threshold);

            int size = selectedImageBefore.getRequiredSize();

            selectedImageBefore.setImage(ImagePreparer.ImageBrushFromBitmapImage(selectedImage, size));

            BitmapImage bitmapOpacityMap = analyser.CreateTransparencyMap();
            selectedImagetOpacityMap.setImage(ImagePreparer.ImageBrushFromBitmapImage(bitmapOpacityMap, size));

            BitmapImage bitmapGMView = analyser.Create_GM_Preview();
            selectedImageGMView.setImage(ImagePreparer.ImageBrushFromBitmapImage(bitmapGMView, size));

            BitmapImage bitmapAfter = analyser.RemoveTransparentThreshold();
            selectedImageAfter.setImage(ImagePreparer.ImageBrushFromBitmapImage(bitmapAfter, size));

            string percentOfTranslucent = String.Format("{0:0.000%}", analyser.countPercentOfTranslucent());
            translucentPixelsPercentage.Content = percentOfTranslucent;

            analyser.setBitmap(bitmapAfter);

            BitmapImage bitmapOpacityMapAfter = analyser.CreateTransparencyMap();
            selectedImagetOpacityMapAfter.setImage(ImagePreparer.ImageBrushFromBitmapImage(bitmapOpacityMapAfter, size));

            BitmapImage bitmapGMViewAfter = analyser.Create_GM_Preview();
            selectedImageGMViewAfter.setImage(ImagePreparer.ImageBrushFromBitmapImage(bitmapGMViewAfter, size));

            
        }

        private void tryAnalyseSelectedImage(object sender, RoutedEventArgs e)
        {
            previewAnalyseImage();
        }

        private void languagesComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            languagesComboBox.ItemsSource = langManager.getSettingsNames();

            string languageKey = "";

            if (!userConfigManager.TryGetValue("__LanguageSettings__", out languageKey))
            {
                languageKey = "English";
            }

            languagesComboBox.SelectedValue = languageKey;
        }

        private void languagesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (languagesComboBox.SelectedItem is string selectedLang)
            {
                langManager.setResource(selectedLang);
                userConfigManager.TrySetValue("__LanguageSettings__", selectedLang);
            }
        }

        private void themesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (themesComboBox.SelectedItem is string selectedTheme)
            {
                themeManager.setResource(selectedTheme);
                userConfigManager.TrySetValue("__ThemeSettings__", selectedTheme);
            }
        }

        private void themesComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            themesComboBox.ItemsSource = themeManager.getSettingsNames();

            string themeKey = "";

            if (!userConfigManager.TryGetValue("__ThemeSettings__", out themeKey))
            {
                themeKey = "Dark";
            }

            themesComboBox.SelectedValue = themeKey;
        }

        private void SaveSelectedImages(object sender, RoutedEventArgs e)
        {
            List<string> imagesPath = imagesPreviewBox.getSelectedImagesPaths();

            RenderImages(imagesPath, imagesPreviewBox.SelectedPath);
        }

        private void SaveAllImages(object sender, RoutedEventArgs e)
        {
            List<string> imagesPath = imagesPreviewBox.getAllImagesPaths();

            RenderImages(imagesPath,imagesPreviewBox.SelectedPath);
        }

        private void RenderImages(List<string> imagesPath, string inputFolder)
        {
            string outputFolder = ChooseOutputFolder();

            if (String.IsNullOrEmpty(outputFolder))
            {
                return;
            }

            foreach (string imagePath in imagesPath)
            {
                // Удаляем общую часть пути (inputFolder) и заменяем ее на outputFolder
                
                string relativePath = imagePath.Replace(inputFolder, "");
                string outputFilePath = outputFolder +  relativePath;

                // Создаем папку для изображения, если она не существует
                string outputDirectory = outputFolder + Path.GetDirectoryName(relativePath);
                Directory.CreateDirectory(outputDirectory);

                // Получаем BitmapImage из пути файла
                BitmapImage bitmapImage = ImagePreparer.GetBitmapImageFromFile(imagePath);

                // Применяем анализ к BitmapImage
                analyser.setBitmap(bitmapImage);
                double threshold = theScrollControl.getThreshold();
                analyser.setTransparentThreshold(threshold);
                BitmapImage bitmapAfter = analyser.RemoveTransparentThreshold();

                // Сохраняем полученное изображение в формате PNG
                using (FileStream stream = new FileStream(outputFilePath, FileMode.Create))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapAfter));
                    encoder.Save(stream);
                }
            }

        }

        private string ChooseOutputFolder()
        {
            string selectedPath;
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedPath = folderBrowserDialog.SelectedPath;
            }
            else
            {
                selectedPath = "";
            }
            return selectedPath;
        }

        
    }
}
