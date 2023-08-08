using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TransparentPixelsRemover.uielements
{
    public struct imageDetails
    {
        public ImageBrush previewImage;
        public string imageName;
        public string imageFolder;
    }

    public partial class imagesPreviewContainer : UserControl
    {
        Dictionary<int, imagePreview> imagePreviews;

        string selectedPath;

        public event Action changeCurrentFolder;
        public event Action clearBox;
        public event Action itemSelected;

        public string SelectedPath { get { return selectedPath; } }

        HashSet<int> selectedPreviewsIndicies;
        int lastSelectedIndex;

        public imagesPreviewContainer()
        {
            InitializeComponent();

            imagePreviews = new Dictionary<int, imagePreview>();
            selectedPreviewsIndicies = new HashSet<int>();
            lastSelectedIndex = -1;
        }

        private void changeFolder(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedPath = folderBrowserDialog.SelectedPath;
                refillPreviewImages();
                selectedPreviewsIndicies.Clear();
                lastSelectedIndex = -1;
                changeCurrentFolder?.Invoke();
            }

        }

        private bool refillPreviewImages()
        {
            previewImagesPanel.Children.Clear();
            imagePreviews.Clear();

            bool folderPathNonValidity = String.IsNullOrEmpty(selectedPath) || !Directory.Exists(selectedPath);

            if (folderPathNonValidity)
            {
                return false;
            }

            List<imageDetails> imageDetails = new List<imageDetails>();

            fillPreviewImagesList(imageDetails, selectedPath);

            foreach (imageDetails imageDetailsElement in imageDetails)
            {
                imagePreview newImagePreview = new imagePreview();

                newImagePreview.setImagePreviewInfo(imageDetailsElement);

                newImagePreview.ImagePreviewContainer = this;

                imagePreviews.Add(newImagePreview.GetHashCode(), newImagePreview);

                previewImagesPanel.Children.Add(newImagePreview);
            }

            return true;
        }

        public void addSelectedPreview(imagePreview imagePreview)
        {
            selectedPreviewsIndicies.Add(imagePreview.GetHashCode());
        }

        public void removeSelectedPreview(imagePreview imagePreview)
        {
            selectedPreviewsIndicies.Remove(imagePreview.GetHashCode());
        }

        public void setCurrentAnalyseImage(imagePreview imagePreview)
        {
            if (lastSelectedIndex != -1 && imagePreviews[lastSelectedIndex]!= imagePreview)
            {
                imagePreviews[lastSelectedIndex].setUnactiveAnalyseStyle();
            }
            
            lastSelectedIndex = imagePreview.GetHashCode();
            itemSelected?.Invoke();
        }

        private void fillPreviewImagesList(List<imageDetails> imageDetailsList, string folderPath)
        {

            DirectoryInfo directory = new DirectoryInfo(folderPath);

            foreach (FileInfo file in directory.GetFiles("*.png"))
            {
                imageDetails imageDetails = new imageDetails();
                imageDetails.previewImage = GetPreviewImageBrush(file.FullName);
                imageDetails.imageName = file.Name;
                imageDetails.imageFolder = file.FullName;
                imageDetailsList.Add(imageDetails);
            }

            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                fillPreviewImagesList(imageDetailsList, subDirectory.FullName);
            }
        }

        public ImageBrush GetPreviewImageBrush(string imagePath)
        {
            BitmapImage originalImage = ImagePreparer.GetBitmapImageFromFile(imagePath);

            return ImagePreparer.ImageBrushFromBitmapImage(originalImage, 78);
        }

        private void clearContainer(object sender, RoutedEventArgs e)
        {

            selectedPath = "";
            selectedPreviewsIndicies.Clear();
            refillPreviewImages();
            lastSelectedIndex = -1;
            clearBox?.Invoke();
        }

        public BitmapImage? getLastSelectedImageOrFirst()
        {
            string selectedElementPath = "";

            if (lastSelectedIndex == -1)
            {
                if (imagePreviews.Count == 0)
                    return null;

                selectedElementPath = imagePreviews.Values.First().getImagePreviewInfo().imageFolder;
            }
            else
            {
                selectedElementPath = imagePreviews[lastSelectedIndex].getImagePreviewInfo().imageFolder;
            }

            return ImagePreparer.GetBitmapImageFromFile(selectedElementPath);
        }

        public List<string> getSelectedImagesPaths()
        {
            List<string> selectedImagesPaths = new List<string>();

            foreach(int selectedIndex in  selectedPreviewsIndicies)
            {
                string imagePath = imagePreviews[selectedIndex].getImagePreviewInfo().imageFolder;
                selectedImagesPaths.Add(imagePath);
            }

            return selectedImagesPaths;
        }

        public List<string> getAllImagesPaths()
        {
            List<string> allImagesPaths = new List<string>();

            foreach (imagePreview imagePreviewItem in imagePreviews.Values)
            {
                string imagePath = imagePreviewItem.getImagePreviewInfo().imageFolder;
                allImagesPaths.Add(imagePath);
            }

            return allImagesPaths;
        }
    }
}