using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult ResultOk()
        {
            return Ok();
        }
        
        [HttpGet]
        public IActionResult ResultOkNotEmpty()
        {
            return Ok(new {A = "A", B = "B"});
        }

        [HttpPost]
        public IActionResult ResultCreated([FromBody]Models.Data data)
        {
            return Created("/home/1", data);
        }
        
        [HttpGet]
        public IActionResult ResultCreatedViaStatusCode()
        {
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpGet]
        public IActionResult MovedFrom()
        {
            // 301
            return RedirectToActionPermanent(nameof(RedirectTo1));
        }
        
        [HttpPost]
        public IActionResult PreserveMethodFrom([FromBody]Models.Data data)
        {
            // 308
            return RedirectToActionPermanentPreserveMethod(nameof(RedirectToWithBody));
        }

        [HttpPost]
        public IActionResult RedirectToWithBody([FromBody] Models.Data data)
        {
            return Ok(data);
        }
        
        [HttpGet]
        public IActionResult RedirectFrom()
        {
            // HttpStatusCode.Found 302
            
            // Moved is a synonym for MovedPermanently.
            // HttpStatusCode.Moved 301
            // HttpStatusCode.MovedPermanently 301
                
            return RedirectToAction(nameof(RedirectTo1));
        }
        
        [HttpGet]
        public IActionResult RedirectTo1()
        {
            return RedirectToAction(nameof(RedirectTo2));
        }
        
        [HttpGet]
        public IActionResult RedirectTo2()
        {
            return Ok();
        }

        [HttpGet]
        public IActionResult Exception()
        {
            throw new InvalidOperationException("Invalid");
        }

        [HttpGet]
        public async Task<IActionResult> BadGateway([FromServices] HttpClient httpClient)
        {
            try
            {
                await httpClient.GetAsync("http://несуществующийдомен.com");  
            }
            catch (HttpRequestException)
            {
                return StatusCode((int) HttpStatusCode.BadGateway);
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GatewayTimeout([FromServices] HttpClient httpClient)
        {
            try
            {
                httpClient.Timeout = TimeSpan.FromMilliseconds(20);
                await httpClient.GetAsync("http://yandex.ru");
            }
            catch (TaskCanceledException)
            {
                return StatusCode((int) HttpStatusCode.GatewayTimeout);
            }
            catch (TimeoutException)
            {
                return StatusCode((int) HttpStatusCode.GatewayTimeout);
            }

            return Ok();
        }
        
        private static Dictionary<int, object> Values = new()
        {
            {1, new {A = "A", B = "B"}},
            {2, new {C = "C", D = "D"}},
            {3, new {E = "E", F = "F"}},
        };

        [HttpGet]
        [Authorize]
        public IActionResult ReturnForbiddenWithoutAuthorizationHeader()
        {
            return Ok();
        }
        
        [HttpGet("/[controller]/{id}")]
        public IActionResult Get(int id)
        {
            var val = Values.ContainsKey(id)
                ? Values[id]
                : null;

            return Ok(val);
        }

        [HttpGet]
        public IActionResult ResultNoContent()
        {
            return NoContent();
        }

        [HttpGet]
        public IActionResult ResultOkNullNoContent()
        {
            return Ok(null); // 204
        }

        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        // }
    }
}