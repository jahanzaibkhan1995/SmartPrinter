using System;
using System.Drawing;
using System.Drawing.Printing;
using Microsoft.Extensions.Logging;

namespace SmartPrinterServer.Services;

public class WindowsPrintTestService
{
	private readonly ILogger<WindowsPrintTestService> _logger;

	public WindowsPrintTestService(ILogger<WindowsPrintTestService> logger)
	{
		_logger = logger;
	}

	public void PrintTestPage(string printerName)
	{
		PrintDocument printDocument = new PrintDocument();
		try
		{
			printDocument.PrinterSettings.PrinterName = printerName;
			if (!printDocument.PrinterSettings.IsValid)
			{
				throw new PrinterException("Printer '" + printerName + "' is not valid.");
			}
			printDocument.PrintPage += delegate(object sender, PrintPageEventArgs e)
			{
				using Font font = new Font("Arial", 20f);
				e.Graphics.DrawString("Smart Print Server Test Page", font, Brushes.Black, 100f, 100f);
				e.Graphics.DrawString("Printer: " + printerName, font, Brushes.Black, 100f, 150f);
				e.Graphics.DrawString($"Time: {DateTime.Now}", font, Brushes.Black, 100f, 200f);
			};
			_logger.LogInformation("Sending test page to {Printer}", printerName);
			printDocument.Print();
			_logger.LogInformation("Test page sent to Windows print spooler.");
		}
		finally
		{
			((IDisposable)printDocument)?.Dispose();
		}
	}
}
