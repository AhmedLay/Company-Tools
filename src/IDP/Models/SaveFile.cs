namespace CTBX.EmployeesImport.API.Model
{
    public class SaveFile
    {
        public SaveFile()
        {
            Files = new List<FileData>();
        }
        public List<FileData> Files { get; set; }
    }


    public class FileData
    {
        public byte[] ImagesBytes { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }

    }
}
