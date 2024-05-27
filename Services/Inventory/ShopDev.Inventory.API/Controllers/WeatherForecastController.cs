using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopDev.Inventory.ApplicationServices.ProductModule.Abstract;
using ShopDev.Inventory.ApplicationServices.ProductModule.Dtos;

namespace ShopDev.Logistic.API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private readonly IProductService _productService;
		private static readonly string[] Summaries = new[]
		{
			"Freezing",
			"Bracing",
			"Chilly",
			"Cool",
			"Mild",
			"Warm",
			"Balmy",
			"Hot",
			"Sweltering",
			"Scorching"
		};

		private readonly ILogger<WeatherForecastController> _logger;

		public WeatherForecastController(
			ILogger<WeatherForecastController> logger,
			IProductService productService
		)
		{
			_logger = logger;
			_productService = productService;
		}

		[Authorize]
		[HttpGet("GetWeatherForecast")]
		public IEnumerable<object> Get()
		{
			return Enumerable
				.Range(1, 5)
				.Select(index => new
				{
					Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
					TemperatureC = Random.Shared.Next(-20, 55),
					Summary = Summaries[Random.Shared.Next(Summaries.Length)]
				})
				.ToArray();
		}

		[HttpPost("create")]
		public void Create(ProductCreateDto input)
		{
			_productService.Create(input);
		}

		[HttpGet("find-by-id/{id}")]
		public ProductDetailDto Create(string id)
		{
			return _productService.FindById(id);
		}

		[HttpPut("update")]
		public void Update(ProductUpdateDto input)
		{
			 _productService.Update(input);
			return;
		}


	}
}
