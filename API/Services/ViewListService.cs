using System;
using API.Data;
using API.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace API.Services
{
    public class ViewListService
    {
        private static List<string> _viewNames;

        private readonly IWebHostEnvironment _env;

        public ViewListService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task CacheViewNames()
        {
            if (!_env.IsDevelopment())
                return;

            var fileType = new FileType(FileType.RazorPage);
            _viewNames = Directory
                .GetFiles(Path.Combine(".", fileType.Folder), $"*.{fileType.FileExtension}")
                .Select(Path.GetFileNameWithoutExtension)
                .Where(n => !n.StartsWith('_')).ToList();

            var viewNamesJson = JsonSerializer.Serialize(_viewNames);
            await File.WriteAllTextAsync(Path.Combine(".", "Data", "ViewNames.json"), viewNamesJson);
        }

        public async Task<List<string>> GetViewNames()
        {
            if (_viewNames != null)
                return _viewNames;

            var jsonPath = Path.Combine(".", "Data", "ViewNames.json");
            if (!File.Exists(jsonPath))
                throw new Exception("Start the application first in your development environment to cache the view names!");

            var viewNamesJson = await File.ReadAllTextAsync(jsonPath);
            _viewNames = JsonSerializer.Deserialize<List<string>>(viewNamesJson);
            return _viewNames;
        }
    }
}
