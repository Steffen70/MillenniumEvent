using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;

using static System.IO.File;

namespace API.Controllers
{
    public class CatchallController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        public CatchallController(IWebHostEnvironment environment)
        {
            _environment = environment;
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

            var file = $@"{_environment.WebRootPath}\{fileType.Folder}\{viewName}.{fileType.FileExtension}";
            var fileExists = Exists(file);

            var fileList = Directory.GetFiles($@"{_environment.WebRootPath}\{fileType.Folder}\", $"*.{fileType.FileExtension}")
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
        public ActionResult Index(string name = "Index")
        {
            var file = $@"Views\{name}.cshtml";
            var viewExists = Exists(file);

            // check if view name requested is not found
            if (!viewExists)
                return NotFound();

            // otherwise just return the view
            var viewModel = new
            {
                Title = name,
                Scripts = GetFiles(name, new FileType(FileType.JavaScript)),
                Styles = GetFiles(name, new FileType(FileType.Stylesheet)),
            };
            return View(file, viewModel);
        }
    }
}
