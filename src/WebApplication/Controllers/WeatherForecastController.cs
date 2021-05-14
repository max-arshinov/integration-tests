using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Authorize(AuthenticationSchemes = IdentityServerJwtConstants.IdentityServerJwtBearerScheme)]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        

        [HttpGet(nameof(Api3))]
        [Authorize(Policy = "Api3")]
        public IActionResult Api3()
        {
            return Ok();
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var u = User;
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }
    }
}