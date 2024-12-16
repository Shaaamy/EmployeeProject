using Microsoft.CodeAnalysis;
using static NuGet.Packaging.PackagingConstants;

namespace Demo.PL.Helpers
{
    public static class DocumentSettings
    {
        //Upload
        public static string UploadFile(IFormFile file , string FolderName) 
        {
        //1. Get Located Folder Path 
        var FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", FolderName);
        //2. Get File Name and Make it Unique 
        var FileName = $"{Guid.NewGuid}{file.FileName}";
        //3. Get File Path[Folder Path + FileName]
        var FilePath= Path.Combine(FolderPath, FileName);
        //4. Save File As Streams
        using var Fs = new FileStream(FilePath ,FileMode.Create);
        file.CopyTo(Fs);
        //5. Return File Name
        return FileName;
        }

        //Delete
        public static void DeleteFile(string FileName, string FolderName)
        {
            //1. Get File Path 
            string FilePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\Files" ,FolderName,FileName);
            //2. Check if File Exists Or No
            if(File.Exists(FilePath))
            {
                File.Delete(FilePath);  
            }
        }
    }
}
