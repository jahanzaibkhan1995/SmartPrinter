using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SmartPrinterServer.Models;

namespace SmartPrinterServer.Services;

public interface IPrintJobService
{
	Task<List<PrintJob>> GetAllAsync();

	Task<PrintJob?> GetByIdAsync(int id);

	Task<PrintJob> CreateAsync(IFormFile file);

	Task<bool> CancelAsync(int id);

	Task UpdateStatusAsync(int jobId, PrintJobStatus status);
}
