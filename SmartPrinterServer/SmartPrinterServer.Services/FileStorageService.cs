using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace SmartPrinterServer.Services;

public class FileStorageService : IFileStorageService
{
	private readonly string _uploadDirectory;

	public FileStorageService(IWebHostEnvironment environment)
	{
		_uploadDirectory = Path.Combine(environment.ContentRootPath, "Uploads");
		Directory.CreateDirectory(_uploadDirectory);
	}

	public async Task<string> SaveFileAsync(IFormFile file)
	{
		string extension = Path.GetExtension(file.FileName);
		string filePath = Path.Combine(path2: $"{Guid.NewGuid()}{extension}", path1: _uploadDirectory);
		string result;
		await using (FileStream stream = new FileStream(filePath, FileMode.Create))
		{
			await file.CopyToAsync(stream);
			result = filePath;
		}
		return result;
	}

	public bool DeleteFile(string filePath)
	{
		if (!File.Exists(filePath))
		{
			return false;
		}
		File.Delete(filePath);
		return true;
	}
}
