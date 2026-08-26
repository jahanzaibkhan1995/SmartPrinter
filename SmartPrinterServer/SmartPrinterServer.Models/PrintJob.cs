using System;
using System.ComponentModel.DataAnnotations;

namespace SmartPrinterServer.Models;

public class PrintJob
{
	[Key]
	public int ID { get; set; }

	public string FileName { get; set; } = string.Empty;

	public string FilePath { get; set; } = string.Empty;

	public PrintJobStatus Status { get; set; }

	public DateTime CreateAt { get; set; } = DateTime.UtcNow;

	public DateTime? StartedAt { get; set; }

	public DateTime? CompletedAt { get; set; }
}
