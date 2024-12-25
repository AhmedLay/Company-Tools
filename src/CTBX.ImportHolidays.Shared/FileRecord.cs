namespace CTBX.ImportHolidays.Shared;

public class FileRecord
{
    public int Id { get; set; }

    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileStatus { get; set; } = string.Empty;
    public DateTimeOffset UploadDate { get; set; }
}
