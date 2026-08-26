using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartPrinterServer.Services;

public interface IPrinterService
{
	List<string> GetPrinters();

	string GetConfiguredPrinter();

	Task PrintAsync(string filePath, CancellationToken cancellationToken);

	bool IsPrinterAvailable();
}
