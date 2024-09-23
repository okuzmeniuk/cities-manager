﻿using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.Models
{
	public class City
	{
		[Key]
		public Guid CityID { get; set; }

		[Required(ErrorMessage = "City Name cannot be blank")]
		public string? CityName { get; set; }

		public City() { }

		public City(Guid cityID, string? cityName)
		{
			CityID = cityID;
			CityName = cityName;
		}
	}
}
