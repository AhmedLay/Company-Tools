using CTBX.EmployeesImport.Shared;

namespace CTBX.EmployeesImport.Backend;

public interface IFileUploadHandler
{
    Task PersistToDb(FileRecord fileRecord);
    Task<string> SaveFileToFolder(string folderPath, EmployeesFileData file);
    Task<List<FileRecord>> GetAllFileRecordsAsync();
    Task<List<Employee>> GetEmployeesDataAsync();
}

