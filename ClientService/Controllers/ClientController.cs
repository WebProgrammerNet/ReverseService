using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClientService.AppInterfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog.Web;
using static ClientService.Helpers.MethodTypes;

namespace ClientService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ITextManager _txtconfig;
        NLog.Logger _logger = NLogBuilder.ConfigureNLog("Nlog.config").GetCurrentClassLogger();

        public ClientController(IWebHostEnvironment webHostEnvironment, ITextManager textManager)
        {
            _env = webHostEnvironment;
            _txtconfig = textManager;
        }

        [HttpGet]
        public string Get()
        {
            return "I am Client Controller!";
        }

        [HttpPost]
        [Route("uploadData")]
        public async Task<ActionResult> UploadData (IFormFile textfile, string folder)
        {
            try
            {
                // var filename = textfile.FileName.Split('.')[textfile.FileName.Split('.').Length - 1]; // => format file
                var fileName = textfile.FileName;

                string pathidentify = _txtconfig.PathIdentify(_env.WebRootPath, folder, "FileManager");
                string handlerpath = _txtconfig.HandlerPath(pathidentify, fileName);
                string temp = handlerpath;
                bool flag = await _txtconfig.CreateFile(textfile, handlerpath);

                _logger.Info($"Succes Time: {DateTimeOffset.UtcNow.ToString()}  Method Type {MethodType.Uploading.ToString()}");
                return (flag) ? StatusCode(200, "Ok") : StatusCode(400, "Bad request");
            }
            catch (Exception ex)
            {
                _logger.Error($"Warning Time: {DateTimeOffset.UtcNow.ToString()}  Method Type  {MethodType.Uploading.ToString()} Error  {ex.ToString()}");  
                    //"yyyyMMddHHmmssFFF"
                throw new ArgumentException("Something is wrong");
            }
        }

        [HttpPost]
        [Route("readData")]

        public async Task<ActionResult> ReadData(string folder, string filename)
        {
            try
            {
                string partpath = _txtconfig.PathIdentify(_env.WebRootPath, folder, "FileManager");
                string path = _txtconfig.HandlerPath(partpath, filename + ".txt");

                string outpartpath = _txtconfig.PathIdentify(_env.WebRootPath, folder, "OutFileManager");
                string outpath = _txtconfig.HandlerPath(outpartpath, "reversed_" + filename + ".txt");

                string[] res = await _txtconfig.ReadAndWriteFile(path, outpath);
                if (res == null || res.Length < 1 )
                {
                    return StatusCode(400,"Bad Request");
                }
                _logger.Info($"Succes Time: " + DateTimeOffset.UtcNow.ToString() + "Method Type" + MethodType.Read_and_Write.ToString());//"yyyyMMddHHmmssFFF"

                return StatusCode(200, res);
            }
            catch (Exception ex)
            {
                _logger.Error($"Warning Time: " + DateTimeOffset.UtcNow.ToString() + "Method Type" + MethodType.Read_and_Write.ToString() +
                   "Error" + ex.ToString());//"yyyyMMddHHmmssFFF"
                throw new ArgumentException("Something is wrong");
            }
        }

        #region For Test
        [HttpPost]
        [Route("readData1")]

        public async Task<ActionResult> ReadData1(string folder, string filename)
        {
            string partpath = _txtconfig.PathIdentify(_env.WebRootPath, folder, "FileManager");
            string path = _txtconfig.HandlerPath(partpath, filename + ".txt");
            string[] res = await _txtconfig.ReadFile1(path);

            return StatusCode(200, res);
        }

        [HttpPost]
        [Route("readData3")]

        public ActionResult ReadData3(string folder, string filename)
        {
            string partpath = _txtconfig.PathIdentify(_env.WebRootPath, folder, "FileManager");
            string path = _txtconfig.HandlerPath(partpath, filename + ".txt");
            var res = _txtconfig.ReadFile3(path);

            return StatusCode(200, res);
        }
        #endregion
    }
}