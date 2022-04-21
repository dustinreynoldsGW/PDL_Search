using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PDL_Search.Models;

namespace PDL_Search.Controllers
{
    public class SearchController : Controller
    {
        private readonly ILogger<SearchController> _logger;

        public object Session { get; private set; }

        public SearchController(ILogger<SearchController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(DrugModel modelDrug)
        {
            //DrugModel modelDrug = new DrugModel();
            return View(modelDrug);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult ExecuteSearch(DrugModel modelDrug)
        //{
        //    // Retrieve Sample Data in a JSON String format
        //    String myData = PDL_Search.Classes.DBFunctions.GetJSON(modelDrug);

        //    // Convert JSON string to JSON.  Ie. Parse
        //    JsonResult myJson = Json(myData);            
            
        //    // Assign JSON String to Model so that it can be returned
        //    modelDrug.DrugList = myData;
        //    modelDrug.NDC = "test";

        //    // Return the data
        //    return View("Index",modelDrug);
        //}

        [HttpPost]
        public IActionResult GetDrugList(DrugModel modelDrug)
        {
            // Retrieve Sample Data in a JSON String format
            String myData = PDL_Search.Classes.DBFunctions.GetJSON(modelDrug);

            // Convert JSON string to JSON.  Ie. Parse
            JsonResult myJson = Json(myData);

            // Return the data
            return myJson;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
