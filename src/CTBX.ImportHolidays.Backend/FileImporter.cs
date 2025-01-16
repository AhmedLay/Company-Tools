using Microsoft.Extensions.Logging;


namespace CTBX.ImportHolidays.Backend;


//public class FileImporter
//{
//    private readonly FileImportService _fileImportService;
//    private readonly ILogger<FileImporter> _logger;

//    public FileImporter(FileImportService fileImportService, ILogger<FileImporter> logger)
//    {
//        _fileImportService = fileImportService;
//        _logger = logger;
//    }

//    public async Task ImportHolidayFromFiles(CancellationToken cancellationToken)
//    {
//        _logger.LogInformation("Start processing files.");

//        var pendingFiles = await _fileImportService.GetPendingFiles();

//        foreach (var file in pendingFiles)
//        {
//            _logger.LogDebug("Processing file {FileName}.", file.FileName);

//            await _fileImportService.Handle(new UpdateFileStatus(file.Id, "In Progress"), cancellationToken);

//            try
//            {
//                await _fileImportService.Handle(new ImportHolidayFromFileCommand(file.FilePath), cancellationToken);
//                await _fileImportService.Handle(new DeleteFile(file.FilePath), cancellationToken);
//                await _fileImportService.Handle(new UpdateFileStatus(file.Id, "Completed"), cancellationToken);
//                _logger.LogInformation("File {FileName} successfully processed.", file.FileName);
//            }

//            catch (Exception ex)
//            {
//                await _fileImportService.Handle(new UpdateFileStatus(file.Id, "Failed"), cancellationToken);
//                await _fileImportService.Handle(new DeleteFile(file.FilePath), cancellationToken);
//                _logger.LogError(ex, "Failed to process file {FileName}.", file.FileName);
//            }
//        }

//        _logger.LogInformation("Finished processing files.");
//    }
//}




//public class FileImporter
//{
//    private readonly FileImportService _fileImportService;
//    private readonly ILogger<FileImportService> _logger;


//    public FileImporter(FileImportService fileImportService, ILogger<FileImportService> logger)
//    {
//        _fileImportService = fileImportService;
//        _logger = logger;
//    }
//    public async Task ImportHolidayFromFile()
//    {
//        _logger.LogInformation("Start processing files");
//        var pendingFiles = await _fileImportService.GetPendingFiles();
//        foreach (var file in pendingFiles)
//        {
//            //await _fileImportService.UpdateFileStatus(file.Id, "In Progress");
//            _logger.LogDebug("Processing file {fileName}.", file.FileName);

//            try
//            {
//                //await _fileImportService.ImportHolidayFromFile(file.FilePath);
//                //await _fileImportService.Handle<OperationResult>(new PersistHolidaysFromFile(file.FileName, file.FileContent), cancellationToken);
//                //await _fileImportService.DeleteFileFromFolder(file.FilePath);
//                //await _fileImportService.UpdateFileStatus(file.Id, "In Progress");
//                _logger.LogInformation("{fileName} successfully processed.", file.FileName);
//            }
//            catch (Exception ex)
//            {
//                //await _fileImportService.UpdateFileStatus(file.Id, "Failed");
//                //await _fileImportService.DeleteFileFromFolder(file.FilePath);
//                _logger.LogError(ex, "{fileName} failed.", file.FileName);
//            }

//        }

//    }

//}
