using Microsoft.EntityFrameworkCore;
using SmartPrinterServer.Models;

namespace SmartPrinterServer.Data;

public class PrintDbContext : DbContext
{
	public DbSet<PrintJob> PrintJobs => ((DbContext)this).Set<PrintJob>();

	public PrintDbContext(DbContextOptions<PrintDbContext> options)
		: base((DbContextOptions)(object)options)
	{
	}
}
