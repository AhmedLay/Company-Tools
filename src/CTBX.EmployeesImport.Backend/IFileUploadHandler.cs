using CTBX.EmployeesImport.Shared;

namespace CTBX.EmployeesImport.Backend;

public interface IFileUploadHandler
{
    Task PersistToDb(FileRecord fileRecord);
    Task<string> SaveFileToFolder(string folderPath, FileData file);
}

