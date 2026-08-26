using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SmartPrinterServer.Services;

namespace SmartPrinterServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrintersController : ControllerBase
{
	private readonly IPrinterService _printerService;

	private readonly WindowsPrintTestService _testPrintService;

	public PrintersController(IPrinterService printerService, WindowsPrintTestService testPrintService)
	{
		_printerService = printerService;
		_testPrintService = testPrintService;
	}

	[HttpGet]
	public IActionResult GetPrinters()
	{
		List<string> printers = _printerService.GetPrinters();
		return Ok(printers);
	}

	[HttpGet("configured")]
	public IActionResult GetConfiguredPrinter()
	{
		return Ok(_printerService.GetConfiguredPrinter());
	}

	[HttpGet("status")]
	public IActionResult GetStatus()
	{
		string configuredPrinter = _printerService.GetConfiguredPrinter();
		bool available = _printerService.IsPrinterAvailable();
		return Ok(new
		{
			printer = configuredPrinter,
			available = available
		});
	}

	[HttpPost("test-print")]
	public IActionResult TestPrint()
	{
		string configuredPrinter = _printerService.GetConfiguredPrinter();
		_testPrintService.PrintTestPage(configuredPrinter);
		return Ok(new
		{
			message = "Printer test endpoint created.",
			printer = configuredPrinter
		});
	}
}
