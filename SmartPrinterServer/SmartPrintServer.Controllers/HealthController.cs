using Microsoft.AspNetCore.Mvc;

namespace SmartPrintServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
	[HttpGet]
	public IActionResult Get()
	{
		return Ok(new
		{
			status = "Running"
		});
	}
}
