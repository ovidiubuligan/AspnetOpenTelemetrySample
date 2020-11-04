using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ServiceA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceA : ControllerBase
    {
        // GET: api/<ResourceA>
        [HttpGet]
        public string Get()
        {
            var client = new HttpClient();
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            //client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            System.Threading.Thread.Sleep(100);
            var stringTask = client.GetStringAsync("https://localhost:44314/api/ResourceB");

            var msg =  stringTask.Result;
            //Console.Write(msg);
            return msg;
        }

        //// GET api/<ResourceA>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<ResourceA>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<ResourceA>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<ResourceA>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
