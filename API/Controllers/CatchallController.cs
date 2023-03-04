using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using API.Extensions;
using Microsoft.AspNetCore.Hosting;

using static System.IO.File;
using API.Helpers;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    public class CatchallController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IOptions<ApiSettings> _apiSettings;

        public CatchallController(IWebHostEnvironment environment, IOptions<ApiSettings> apiSettings)
        {
            _environment = environment;
            _apiSettings = apiSettings;
        }

        public class FileType
        {
            public static readonly (string, string) JavaScript = ("Scripts", "js");
            public static readonly (string, string) Stylesheet = ("Styles", "css");

            private readonly (string, string) _fileType;

            public string Folder => _fileType.Item1;
            public string FileExtension => _fileType.Item2;

            public FileType((string, string) fileType)
            {
                _fileType = fileType;
            }
        }

        private IList<string> GetFiles(string viewName, FileType fileType)
        {
            viewName = viewName.ToLower();

            var reg1 = new Regex(@"^_");
            var reg2 = new Regex($"{viewName}");

            var file = Path.Combine(_environment.WebRootPath, fileType.Folder, $"{viewName}.{fileType.FileExtension}");
            var fileExists = Exists(file);

            var fileList = Directory.GetFiles(Path.Combine(_environment.WebRootPath, fileType.Folder), $"*.{fileType.FileExtension}")
                .Select(Path.GetFileName)
                .Where(n => n.Contains('_'))
                .Where(n => reg1.IsMatch(n) || reg2.IsMatch(n))
                .ToList();

            if (fileExists)
                fileList.Add($"{viewName}.{fileType.FileExtension}");

            return fileList;
        }

        // GET: /Catchall/
        [AllowAnonymous]
        public ActionResult GetView(string view = "Index")
        {
            var viewExists = _apiSettings.Value.Views.Contains(view);

            // check if view name requested is not found
            if (!viewExists)
                return NotFound();

            // otherwise just return the view
            var viewModel = new
            {
                Title = view.FirstCharToUpper(),
                Scripts = GetFiles(view, new FileType(FileType.JavaScript)),
                Styles = GetFiles(view, new FileType(FileType.Stylesheet)),
            };
            return View(Path.Combine("Views", $"{view}.cshtml"), viewModel);
        }
    }
}
