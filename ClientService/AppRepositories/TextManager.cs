using ClientService.AppInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static ClientService.Helpers.MethodTypes;

namespace ClientService.AppRepositories
{
    public class TextManager : ITextManager
    {
        NLog.Logger _logger = NLogBuilder.ConfigureNLog("Nlog.config").GetCurrentClassLogger();

        public async Task<bool> CreateFile(IFormFile testFile, string resultpath)
        {
            try
            {
                using (FileStream fileStream = new FileStream(resultpath, FileMode.Create))
                {
                    await testFile.CopyToAsync(fileStream);
                }
                _logger.Info($"Succes for TextManager Time: " + DateTimeOffset.UtcNow.ToString() + "TestLogging");//"yyyyMMddHHmmssFFF"

                return true;
            }
            catch (Exception ex)
            {
                _logger.Fatal($"Warning for TextManager Time: " + DateTimeOffset.UtcNow.ToString() + "Error" + ex.ToString());//"yyyyMMddHHmmssFFF"
                throw new ArgumentException("Something is wrong");
            }

        }

        public string HandlerPath(string folder, string fileName)
        {
            try
            {
                string result = "";
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                    result = Path.Combine(Directory.GetCurrentDirectory(), folder, fileName);
                }
                else
                {
                    result = Path.Combine(Directory.GetCurrentDirectory(), folder, fileName);
                };

                Directory.CreateDirectory(folder);
                return result;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Something is wrong");
            }
        }

        public string PathIdentify(string env, string folder, string manager)
        {
            string location = Path.Combine(env, manager);
            string path;

            if (folder.Contains("-"))
            {
                var temp = folder.Split('-');
                string city = temp[0];
                string district = temp[1];
                string longpath = Path.Combine(city, district);

                path = Path.Combine(location, longpath);
                return path;
            }
            else
            {
                path = Path.Combine(location, folder);
                return path;
            }
        }

        public string HandlerFolder(string subname)
        {

            if (subname.Contains("-"))
            {
                var temp = subname.Split('-');
                string city = temp[0];
                string district = temp[1];
                string longpath = Path.Combine(city, district);
                return longpath;
            }
            else
            {
                return subname;
            }
        }

        public async Task<string[]> ReadAndWriteFile(string path, string outpath)
        {
            try
            {
                List<string> lines = File.ReadAllLines(path).ToList();
                if (lines.Count < 10)
                {
                    _logger.Error($"Warning for TextManager.ReadAndWriteFile don't enught CountLine (10) Time: : {DateTimeOffset.UtcNow.ToString()}  Method Type  {MethodType.Read_and_Write.ToString()}");

                    return null;
                }

                foreach (var item in lines)
                {
                    using HttpClient _client = new HttpClient();

                    HttpResponseMessage response = await _client.GetAsync($"https://localhost:44383/api/reverse/reversed?str={item}");
                    using (HttpContent content = response.Content)
                    {
                        _logger.Info($"Succes for TextManager.ReadAndWriteFile Time: " + DateTimeOffset.UtcNow.ToString() + "TestLogging");//"yyyyMMddHHmmssFFF"

                        var mycontent = await content.ReadAsStringAsync();
                        if (mycontent != null)
                        {
                            using (StreamWriter writer = new StreamWriter(outpath, true))
                            {
                                writer.WriteLine(mycontent);
                            }
                        }
                    }
                }
                List<string> reversedlines = File.ReadAllLines(outpath).ToList();
                return await Task.FromResult(lines.ToArray());
            }
            catch (Exception ex)
            {
                _logger.Fatal($"Warning for TextManager.ReadAndWriteFile Time: : {DateTimeOffset.UtcNow.ToString()}  Method Type  {MethodType.Read_and_Write.ToString()} Error  {ex.ToString()}");   
                throw new ArgumentException("Something is wrong");
            }
        }

        #region
        public async Task<string[]> ReadFile1(string path)
        {
            string result;
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(file, Encoding.UTF8))
                {
                    result = reader.ReadToEnd().Replace(Environment.NewLine, "");
                    reader.Close();
                }
                file.Close();
            }

            string[] massiv = result.Split(';');
            return await Task.FromResult(massiv);
        }

        public static string test0(string path)
        {
            StringBuilder sb = new StringBuilder();

            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    sb.Append(reader.ReadLine());
                }
            }

            File.WriteAllText(path, sb.ToString());
            return sb.ToString();
        }

        public List<string> ReadFile3(string path)
        {
            List<string> result = new List<string>();
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {

                using (StreamReader reader = new StreamReader(file, Encoding.UTF8))
                {
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        result.Add(line);
                        line = reader.ReadLine();

                    }
                    reader.Close();
                }
                file.Close();
            }

            List<string> lines = File.ReadAllLines(path).ToList();
            return result;
        }
        #endregion

        //Id = 37, Status = WaitingForActivation, Method = "{null}", Result = "{Not yet computed}"

    }
}
