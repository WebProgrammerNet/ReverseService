using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientService.AppInterfaces
{
    public interface ITextManager
    {
        public string HandlerFolder(string subname);
        public string PathIdentify(string env, string folder, string manager);
        public string HandlerPath(string folder, string fileName);
        public Task<bool> CreateFile(IFormFile testFile, string resultpat);
        public Task<string[]> ReadAndWriteFile(string path, string outpath);
        public Task<string[]> ReadFile1(string path);
        public List<string> ReadFile3(string path);
    }
}
