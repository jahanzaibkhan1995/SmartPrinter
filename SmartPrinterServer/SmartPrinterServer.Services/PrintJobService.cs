using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartPrinterServer.Data;
using SmartPrinterServer.Models;

namespace SmartPrinterServer.Services;

public class PrintJobService : IPrintJobService
{
	private readonly IPrintQueue _printQueue;

	private readonly PrintDbContext _context;

	private readonly IFileStorageService _fileStorageService;

	public PrintJobService(PrintDbContext context, IFileStorageService fileStorageService, IPrintQueue printQueue)
	{
		_context = context;
		_fileStorageService = fileStorageService;
		_printQueue = printQueue;
	}

	public async Task<List<PrintJob>> GetAllAsync()
	{
		return await EntityFrameworkQueryableExtensions.ToListAsync<PrintJob>((IQueryable<PrintJob>)_context.PrintJobs, default(CancellationToken));
	}

	public async Task<PrintJob?> GetByIdAsync(int id)
	{
		return await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync<PrintJob>((IQueryable<PrintJob>)_context.PrintJobs, (Expression<Func<PrintJob, bool>>)((PrintJob job) => job.ID == id), default(CancellationToken));
	}

	public async Task<PrintJob> CreateAsync(IFormFile file)
	{
		string filePath = await _fileStorageService.SaveFileAsync(file);
		PrintJob job = new PrintJob
		{
			FileName = file.FileName,
			FilePath = filePath,
			Status = PrintJobStatus.Pending,
			CreateAt = DateTime.UtcNow
		};
		_context.PrintJobs.Add(job);
		await ((DbContext)_context).SaveChangesAsync(default(CancellationToken));
		_printQueue.Enqueue(job);
		return job;
	}

	public async Task<bool> CancelAsync(int id)
	{
		PrintJob job = await GetByIdAsync(id);
		if (job == null)
		{
			return false;
		}
		if (job.Status != PrintJobStatus.Pending)
		{
			return false;
		}
		job.Status = PrintJobStatus.Cancelled;
		await ((DbContext)_context).SaveChangesAsync(default(CancellationToken));
		return true;
	}

	public async Task UpdateStatusAsync(int jobId, PrintJobStatus status)
	{
		PrintJob job = await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync<PrintJob>((IQueryable<PrintJob>)_context.PrintJobs, (Expression<Func<PrintJob, bool>>)((PrintJob j) => j.ID == jobId), default(CancellationToken));
		if (job != null)
		{
			job.Status = status;
			if (status == PrintJobStatus.Printing)
			{
				job.StartedAt = DateTime.UtcNow;
			}
			if (status == PrintJobStatus.Completed)
			{
				job.CompletedAt = DateTime.UtcNow;
			}
			await ((DbContext)_context).SaveChangesAsync(default(CancellationToken));
		}
	}
}
