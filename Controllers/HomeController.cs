// HomeController.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
//using System.Web.Mvc;
using ExcelToDB.Models;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;


namespace ExcelToDB.Controllers
{
    public class HomeController : Controller
    {
        private readonly SchoolContext _context;
        private readonly ILogger<HomeController> _logger;


        public HomeController(SchoolContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger= logger;;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogError("No file uploaded.");
                ViewBag.Error = "No file uploaded.";
                return View("Index");
            }

            List<Student> students = new List<Student>();

            try
            {
                // Set the EPPlus license context
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++) // Start at 2 to skip the header row
                        {
                            var firstName = worksheet.Cells[row, 1]?.Text?.Trim();
                            var lastName = worksheet.Cells[row, 2]?.Text?.Trim();
                            var ageCell = worksheet.Cells[row, 3]?.Text?.Trim();

                            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName) && string.IsNullOrEmpty(ageCell))
                            {
                                // Skip empty rows
                                continue;
                            }

                            if (!int.TryParse(ageCell, out int age))
                            {
                                age = 0; // Default value or handle it accordingly
                            }

                            students.Add(new Student
                            {
                                FirstName = firstName,
                                LastName = lastName,
                                Age = age
                            });
                        }
                    }
                }

                _context.Students.AddRange(students);
                await _context.SaveChangesAsync();

                ViewBag.Message = "Excel data imported successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the file.");
                ViewBag.Error = "An error occurred while uploading the file.";
            }

            return View("Index");
        }

    }

}





//using System.Diagnostics;
//using ExcelToDB.Models;
//using Microsoft.AspNetCore.Mvc;

//{
//    public class HomeController : Controller
//    {
//        private readonly ILogger<HomeController> _logger;

//        public HomeController(ILogger<HomeController> logger)
//        {
//            _logger = logger;
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }

//        public IActionResult Privacy()
//        {
//            return View();
//        }

//        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//        public IActionResult Error()
//        {
//            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//        }
//    }
//}
