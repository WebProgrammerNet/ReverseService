using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog.Web;

namespace ReverseService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReverseController : ControllerBase
    {
        NLog.Logger _logger = NLogBuilder.ConfigureNLog("Nlog.config").GetCurrentClassLogger();

        [HttpGet]
        public string Get()
        {
            return "I am Reverse Controller!";
        }

        [HttpGet]
        [Route("reversed")]
        public async Task<string> Reversed(string str)
        {
            try
            {
                if (String.IsNullOrEmpty(str) || String.IsNullOrWhiteSpace(str))
                {
                    return null;
                }

                string[] arry;
                if (str.Contains(' '))
                {
                    arry = str.Split(' ');
                }
                else
                {
                    arry = new string[1];
                    arry[0] = str;
                }
                StringBuilder builder = new StringBuilder();

                foreach (var word in arry)
                {
                    if (arry[0] != word)
                    {
                        builder.Append(" ");
                    }
                    for (int i = word.Length - 1; i >= 0; i--)
                    {
                        builder.Append(word[i]);
                    }
                }
                string _result = builder.ToString();
                return await Task.FromResult(_result);
            }
            catch (Exception ex)
            {
                _logger.Error($"Warning for Reverese Seervice - ReverseService.Controllers.ReverseController Time: { DateTimeOffset.UtcNow.ToString()}");
                throw new ArgumentException("Something is wrong");
            }
        }
    }
}