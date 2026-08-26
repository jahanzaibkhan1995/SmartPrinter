using System.Threading;
using System.Threading.Tasks;
using SmartPrinterServer.Models;

namespace SmartPrinterServer.Services;

public interface IPrintQueue
{
	void Enqueue(PrintJob job);

	Task<PrintJob> DequeueAsync(CancellationToken cancellationToken);
}
