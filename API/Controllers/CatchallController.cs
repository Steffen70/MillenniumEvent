using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Hosting;

using static System.IO.File;
using API.Helpers;
using Microsoft.Extensions.Options;
using API.Services;

namespace API.Controllers
{
    public class CatchallController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ViewListService _viewListService;

        public CatchallController(IWebHostEnvironment environment, ViewListService viewListService)
        {
            _environment = environment;
            _viewListService = viewListService;
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
        public async Task<ActionResult> GetView(string view = "Index")
        {
            var viewExists = (await _viewListService.GetViewNames()).Contains(view);

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

            var razorFileType = new FileType(FileType.RazorPage);
            return View(Path.Combine(razorFileType.Folder, $"{view}.{razorFileType.FileExtension}"), viewModel);
        }
    }
}
