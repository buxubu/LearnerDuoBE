using Microsoft.AspNetCore.Http;

namespace BasicDelivery.Helper
{
    public static class Unilities
    {
        public static async Task<string>/*<List<String>>*/ UploadMutipleImages(List<IFormFile> mutiFile)
        {
            //List<string> lstThumbName = new List<string>();
            string folderIamges = Path.Combine(Directory.GetCurrentDirectory(), "UploadFile");
            foreach (var item in mutiFile)
            {
                string path = Path.Combine(folderIamges, item.FileName);

                var supportedFileTypes = new[] { "jpg", "jpeg", "png", "gif" };
                var fileExt = System.IO.Path.GetExtension(item.FileName).Substring(1);
                if (supportedFileTypes.Contains(fileExt.ToLower()))
                {
                    await item.CopyToAsync(new FileStream(path, FileMode.Create));
                }
                else
                {
                    return "file not correct type images";
                }
                //lstThumbName.Add(item.FileName);
            }
            return "Upload success";
            //return lstThumbName;
        }

        public static async Task<string> UploadImages(IFormFile file)
        {
            string folderIamges = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ThumpCates");
            string path = Path.Combine(folderIamges, file.FileName);

            var supportedFileTypes = new[] { "jpg", "jpeg", "png", "gif" };
            var fileExt = Path.GetExtension(file.FileName).Substring(1);
            if (supportedFileTypes.Contains(fileExt.ToLower()))
            {
                await file.CopyToAsync(new FileStream(path, FileMode.Create));
            }
            else
            {
                return "file not correct type images";
            }
            return file.FileName;

        }


        public static async Task<string> UploadFileExcel(IFormFile file)
        {
            string folderExcel = Path.Combine(Directory.GetCurrentDirectory(), "UploadFileExcel");
            string path = Path.Combine(folderExcel, file.FileName);

            var supportedFileTypes = new[] { "xlsx", "xlsm", "xlsb", "xltx" };
            var fileExt = Path.GetExtension(file.FileName).Substring(1);
            if (supportedFileTypes.Contains(fileExt.ToLower()))
            {
                await file.CopyToAsync(new FileStream(path, FileMode.Create));
            }
            else
            {
                return "file not correct type excel";
            }
            return path;

        }

        public static string Tovnd(int money)
        {
            return money.ToString("#,##0") + " d";
        }

        public static string RemoveUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                                            "đ",
                                            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
                                            "í","ì","ỉ","ĩ","ị",
                                            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
                                            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
                                            "ý","ỳ","ỷ","ỹ","ỵ"};

            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                                            "d",
                                            "e","e","e","e","e","e","e","e","e","e","e",
                                            "i","i","i","i","i",
                                            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
                                            "u","u","u","u","u","u","u","u","u","u","u",
                                            "y","y","y","y","y"};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i]);
            }
            return text;
        }
    }


}


