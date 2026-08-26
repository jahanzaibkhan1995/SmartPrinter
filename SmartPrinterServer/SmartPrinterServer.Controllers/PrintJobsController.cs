using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartPrinterServer.Models;
using SmartPrinterServer.Services;

namespace SmartPrinterServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrintJobsController : ControllerBase
{
	private readonly IPrintJobService _printJobService;

	private readonly IFileStorageService _fileStorageService;

	public PrintJobsController(IPrintJobService printJobService, IFileStorageService fileStorageService)
	{
		_printJobService = printJobService;
		_fileStorageService = fileStorageService;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll()
	{
		return Ok(await _printJobService.GetAllAsync());
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetById(int id)
	{
		PrintJob job = await _printJobService.GetByIdAsync(id);
		if (job == null)
		{
			return NotFound();
		}
		return Ok(job);
	}

	[HttpPost]
	public async Task<IActionResult> Create(IFormFile file)
	{
		if (file == null || file.Length == 0)
		{
			return BadRequest("A PDF file is required.");
		}
		string extension = Path.GetExtension(file.FileName);
		if (!extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
		{
			return BadRequest("Only PDF files are allowed.");
		}
		PrintJob job = await _printJobService.CreateAsync(file);
		return CreatedAtAction("GetById", new
		{
			id = job.ID
		}, job);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Cancel(int id)
	{
		if (!(await _printJobService.CancelAsync(id)))
		{
			return NotFound();
		}
		return NoContent();
	}
}
