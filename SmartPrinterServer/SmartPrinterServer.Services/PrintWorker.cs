using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartPrinterServer.Models;

namespace SmartPrinterServer.Services;

public class PrintWorker : BackgroundService
{
	private readonly IPrintQueue _printQueue;

	private readonly ILogger<PrintWorker> _logger;

	private readonly IServiceScopeFactory _scopeFactory;

	private readonly IPrinterService _servicePrinter;

	public PrintWorker(IPrintQueue printQueue, ILogger<PrintWorker> logger, IServiceScopeFactory scopeFactory, IPrinterService servicePrinter)
	{
		_printQueue = printQueue;
		_logger = logger;
		_scopeFactory = scopeFactory;
		_servicePrinter = servicePrinter;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Print worker started.");
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await ProcessJobAsync(await _printQueue.DequeueAsync(stoppingToken), stoppingToken);
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, "Error processing print job.");
			}
		}
		_logger.LogInformation("Print worker stopped.");
	}

	private async Task ProcessJobAsync(PrintJob job, CancellationToken cancellationToken)
	{
		using IServiceScope scope = _scopeFactory.CreateScope();
		IPrintJobService printJobService = scope.ServiceProvider.GetRequiredService<IPrintJobService>();
		try
		{
			await printJobService.UpdateStatusAsync(job.ID, PrintJobStatus.Printing);
			_logger.LogInformation("Printing {FileName}...", job.FileName);
			await _servicePrinter.PrintAsync(job.FilePath, cancellationToken);
			await printJobService.UpdateStatusAsync(job.ID, PrintJobStatus.Completed);
			_logger.LogInformation("PrintJob {JobId} completed.", job.ID);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "PrintJob {JobId} failed.", job.ID);
			await printJobService.UpdateStatusAsync(job.ID, PrintJobStatus.Failed);
		}
	}
}
