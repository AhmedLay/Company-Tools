using CTBX.EmployeesImport.Shared;
using CTBX.ImportHolidays.Shared;

namespace CTBX.ImportHolidays.Backend
{
    internal interface IFileImportHandler
    {
        Task UpdateFileStatus(int id, string status);
        Task<List<Holiday>> ConvertFileToHolidays(string filepath);
        Task DeleteFileFromFolder(string filepath);
        Task ImportHolidayFromFile(string filepath);
        Task<IEnumerable<FileRecord>> GetPendingFiles();
    }
}
