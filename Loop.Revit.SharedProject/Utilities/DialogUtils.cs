using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;



namespace Loop.Revit.Utilities
{
    public static class DialogUtils
    {
        public static string SelectSingleFile(string filterText, string fileExtension)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = filterText, // Example: "Image files (*.jpg;*.png)|*.jpg;*.png|All files (*.*)|*.*"
                DefaultExt = fileExtension, // Example: "txt"
                Multiselect = false
            };

            var result = dialog.ShowDialog();
            var filePath = dialog.FileName;

            return result != DialogResult.OK ? string.Empty : filePath;

        }




        /*
        public static string SelectMulitipleFiles(string defaultFileExtension)
        {
            var dialog = new OpenFileDialog()
            {
                DefaultExt = defaultFileExtension,
            };

            var result = dialog.ShowDialog();
            var filePath = dialog.FileNames;

            return result != DialogResult.OK ? string.Empty : filePath;

        }
        */
    }
}
