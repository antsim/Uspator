using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Uspator.Model;

namespace Uspator.Controllers
{
    [Route("/")]
    public class BotController : Controller
    {
        [HttpPost]
        public JsonResult Post([FromForm] ServerRequest value)
        {
            Console.WriteLine(JsonConvert.SerializeObject(value));

            var tasks = new List<TaskBase> {new NoopTask()};
            
            return new JsonResult(tasks);
        }
    }
}