using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SmartPrinterServer.Services;

public interface IFileStorageService
{
	Task<string> SaveFileAsync(IFormFile file);

	bool DeleteFile(string filePath);
}
