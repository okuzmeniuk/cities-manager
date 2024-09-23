using CitiesManager.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.WebAPI.Controllers.v2
{
	[ApiVersion("2.0")]
	public class CitiesController : CustomControllerBase
	{
		private readonly ApplicationDbContext _context;

		public CitiesController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: api/Cities
		/// <summary>
		/// Gets the list of city names from 'Cities' table
		/// </summary>
		/// <returns>List of city names</returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<string?>>> GetCities()
		{
			return await _context.Cities.Select(c => c.CityName).ToListAsync();
		}
	}
}
