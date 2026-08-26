using System.Threading;
using System.Threading.Tasks;

namespace SmartPrinterServer.Services;

public interface IPrintService
{
	Task PrintAsync(string filePath, string printerName, CancellationToken cancellationToken);
}
