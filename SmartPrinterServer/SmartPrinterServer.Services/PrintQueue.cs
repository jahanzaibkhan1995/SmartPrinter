using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using SmartPrinterServer.Models;

namespace SmartPrinterServer.Services;

public class PrintQueue : IPrintQueue
{
	private readonly ConcurrentQueue<PrintJob> _queue = new ConcurrentQueue<PrintJob>();

	private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

	public void Enqueue(PrintJob job)
	{
		_queue.Enqueue(job);
		_signal.Release();
	}

	public async Task<PrintJob> DequeueAsync(CancellationToken cancellationToken)
	{
		await _signal.WaitAsync(cancellationToken);
		_queue.TryDequeue(out PrintJob job);
		return job;
	}
}
