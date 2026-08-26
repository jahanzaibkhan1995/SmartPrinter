using System;

namespace SmartPrinterServer.Services;

public class PrinterException : Exception
{
	public PrinterException(string message)
		: base(message)
	{
	}

	public PrinterException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
