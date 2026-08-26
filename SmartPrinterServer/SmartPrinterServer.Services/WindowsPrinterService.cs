using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PDFtoImage;
using SkiaSharp;
using SmartPrinterServer.Models;

namespace SmartPrinterServer.Services;

public class WindowsPrinterService : IPrinterService
{
	private readonly PrinterOptions _options;

	private readonly ILogger<WindowsPrinterService> _logger;

	public WindowsPrinterService(ILogger<WindowsPrinterService> logger, IOptions<PrinterOptions> options)
	{
		_logger = logger;
		_options = options.Value;
	}

	public List<string> GetPrinters()
	{
		ProcessStartInfo startInfo = new ProcessStartInfo
		{
			FileName = "powershell.exe",
			Arguments = "-NoProfile -Command \"Get-Printer | Select-Object -ExpandProperty Name\"",
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true
		};
		using Process process = Process.Start(startInfo);
		if (process == null)
		{
			throw new PrinterException("Could not start PowerShell.");
		}
		string text = process.StandardOutput.ReadToEnd();
		string text2 = process.StandardError.ReadToEnd();
		process.WaitForExit();
		if (process.ExitCode != 0)
		{
			throw new PrinterException("Failed to get Windows printers: " + text2);
		}
		return (from name in text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
			select name.Trim() into name
			where !string.IsNullOrWhiteSpace(name)
			select name).ToList();
	}

	public string GetConfiguredPrinter()
	{
		return _options.Name;
	}

	public bool IsPrinterAvailable()
	{
		string configuredPrinter = GetConfiguredPrinter();
		List<string> printers = GetPrinters();
		return printers.Contains<string>(configuredPrinter, StringComparer.OrdinalIgnoreCase);
	}

	public async Task PrintAsync(string filePath, CancellationToken cancellationToken)
	{
		if (!File.Exists(filePath))
		{
			throw new PrinterException("Print file does not exist: " + filePath);
		}
		string printerName = GetConfiguredPrinter();
		ValidatePrinter(printerName);
		_logger.LogInformation("Selected printer: {PrinterName}", printerName);
		_logger.LogInformation("Printing PDF: {FilePath}", filePath);
		await PrintPdfAsync(filePath, printerName, cancellationToken);
	}

	private async Task PrintPdfAsync(string filePath, string printerName, CancellationToken cancellationToken)
	{
		await Task.Run(delegate
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			cancellationToken.ThrowIfCancellationRequested();
			using FileStream fileStream = File.OpenRead(filePath);
			IEnumerable<SKBitmap> source = Conversion.ToImages((Stream)fileStream, false, (string)null, new RenderOptions(300, (int?)null, (int?)null, false, false, false, (PdfRotation)0, (PdfAntiAliasing)7, (SKColor?)null, (RectangleF?)null, false, false, false));
			List<SKBitmap> pageImages = source.ToList();
			try
			{
				if (pageImages.Count == 0)
				{
					throw new PrinterException("PDF contains no pages.");
				}
				_logger.LogInformation("PDF contains {PageCount} pages.", pageImages.Count);
                PrintDocument printDocument = new PrintDocument();
				try
				{
					printDocument.PrinterSettings.PrinterName = printerName;
					printDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
					printDocument.OriginAtMargins = false;
					if (!printDocument.PrinterSettings.IsValid)
					{
						throw new PrinterException("Printer '" + printerName + "' is not valid.");
					}
					int currentPage = 0;
					printDocument.PrintPage += delegate(object sender, PrintPageEventArgs e)
					{
						cancellationToken.ThrowIfCancellationRequested();
						using Bitmap bitmap = ConvertToDrawingBitmap(pageImages[currentPage]);
						float num = (float)bitmap.Width / 300f;
						float num2 = (float)bitmap.Height / 300f;
						int num3 = (int)(num * e.Graphics.DpiX);
						int num4 = (int)(num2 * e.Graphics.DpiY);
						Rectangle marginBounds = e.MarginBounds;
						float val = (float)marginBounds.Width / (float)num3;
						float val2 = (float)marginBounds.Height / (float)num4;
						float num5 = Math.Min(1f, Math.Min(val, val2));
						num3 = (int)((float)num3 * num5);
						num4 = (int)((float)num4 * num5);
						int x = marginBounds.Left + (marginBounds.Width - num3) / 2;
						int y = marginBounds.Top + (marginBounds.Height - num4) / 2;
						e.Graphics.DrawImage(bitmap, new Rectangle(x, y, num3, num4));
						currentPage++;
						e.HasMorePages = currentPage < pageImages.Count;
					};
					_logger.LogInformation("Sending PDF to Windows print spooler.");
					printDocument.Print();
					_logger.LogInformation("PDF sent to printer {PrinterName}.", printerName);
				}
				finally
				{
					((IDisposable)printDocument)?.Dispose();
				}
			}
			finally
			{
				foreach (SKBitmap item in pageImages)
				{
					((SKNativeObject)item).Dispose();
				}
			}
		}, cancellationToken);
	}

	private static Bitmap ConvertToDrawingBitmap(SKBitmap skBitmap)
	{
		SKImage val = SKImage.FromBitmap(skBitmap);
		try
		{
			SKData val2 = val.Encode((SKEncodedImageFormat)4, 100);
			try
			{
				using MemoryStream stream = new MemoryStream(val2.ToArray());
				return new Bitmap(stream);
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void ValidatePrinter(string printerName)
	{
		List<string> printers = GetPrinters();
		if (!printers.Contains<string>(printerName, StringComparer.OrdinalIgnoreCase))
		{
			throw new PrinterException("Printer '" + printerName + "' was not found.");
		}
	}
}
