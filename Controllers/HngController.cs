using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/hello")]
public class HngController : ControllerBase
{


    public HngController()
    {
    }


    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Hng(string name)
    {

       var Ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var client = new HttpClient
        {
            BaseAddress = new Uri("https://api.weatherapi.com/")
        };

        var responseTask = await client.GetAsync($"v1/current.json?q={Ip}&key='apikey-here");
        var myresult = await responseTask.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<ResultObject>(myresult);

        if (data == null)
        {
            return BadRequest(new
            {
                Message = "We could not read your IP",
                IsSuccessful = false
            });
        }

        return Ok(new
        {
            client_ip = Ip,
            location =  data.location.region,
            greeting = $"Hello, {name}!, the temperature is {data.current.temp_c} degrees Celcius in {data.location.region}"
        });
    }

}



public class ResultObject
{
    public Location location { get; set; }
    public Current current { get; set; }
    public City city { get; set; }
    public string ip { get; set; }
}

public class City
{
    public string name { get; set; }
}

public class Location
{
    public string region { get; set; }
}

public class Current
{
    public double temp_c { get; set; }
}


