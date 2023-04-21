namespace EntityFramework_Slider.Helpers
{
    public static class FileHelper
    {
        // CheckCrateUploadMethod
        public static bool CheckFileType(this IFormFile file, string pattern) // IFormFile file tipine bidene extation yaziram
        {
            return file.ContentType.Contains(pattern);     // !slider.Photo.ContentType.Contains("image/") icine ne ideyirem gonderiem  ("image/") ola biler ve yaxuda pdf ve s 
        }


        // CheckCrateUploadMethod
        public static bool CheckFileSize(this IFormFile file, long size)
        {
            return file.Length / 1024 < size;
        }



        //DeleteUploadMethod
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        //FileSerachMethod
        public static string GetFilePath(string root, string folder, string file)
        {
            return Path.Combine(root, folder, file);
        }

    }
}
