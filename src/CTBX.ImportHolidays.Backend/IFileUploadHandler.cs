using CTBX.ImportHolidays.Shared;

namespace CTBX.ImportHolidays.Backend
{
    internal interface IFileUploadHandler
    {
        Task PersistToDb(FileRecord fileRecord);
        Task<string> SaveFileToFolder(string folderPath, FileData file);
    }
}
