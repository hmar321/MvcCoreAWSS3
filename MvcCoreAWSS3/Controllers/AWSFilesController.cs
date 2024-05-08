using Microsoft.AspNetCore.Mvc;
using MvcCoreAWSS3.Services;

namespace MvcCoreAWSS3.Controllers
{
    public class AWSFilesController : Controller
    {
        private ServiceStorageS3 service;

        public AWSFilesController(ServiceStorageS3 service)
        {
            this.service = service;
        }

        public async Task<IActionResult> Index()
        {
            List<string> keyFiles = await this.service.GetVersionsFileAsync();
            return View(keyFiles);
        }

        public IActionResult UploadFile()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            using (Stream stream = file.OpenReadStream())
            {
                await this.service.UploadFileAsync(file.FileName, stream);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteFile(string fileName)
        {
            await this.service.DeleteFileAsync(fileName);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> GetFile(string fileName)
        {
            Stream stream =await this.service.GetPrivateFileAsync(fileName);
            return File(stream, "image/png");
        }
    }
}
