using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage20MvcCore22.Models;
using Garage20MvcCore22.ViewModels;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
//using System.Web;
using Microsoft.AspNetCore.Hosting;

namespace Garage20MvcCore22.Controllers
{
    public class ParkedVehiclesController : Controller
    {
        private readonly Garage20MvcCore22Context _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ParkedVehiclesController(Garage20MvcCore22Context context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: ParkedVehicles
        public async Task<IActionResult> Index()
        {
            //ParkedVehicle parkedVehicle = new ParkedVehicle();
            return View(await _context.ParkedVehicle.ToListAsync());
            //return View();
        }

       public async Task<IActionResult> ParkedVehicles(string sortOrder, string SearchString)
        {
            var parkedVehicles = await _context.ParkedVehicle.Where(p => p.Parked == true).ToListAsync();

            ViewBag.RegSortParm = String.IsNullOrEmpty(sortOrder) ? "RegNr_desc" : "";
            ViewBag.VTypeSortParm = sortOrder == "VehicleType" ? "VehicleType_desc" : "VehicleType";
            ViewBag.ModelSortParm = sortOrder == "Model" ? "Model_desc" : "Model";
            ViewBag.NRWheelSortParm = sortOrder == "NrOfWheels" ? "NrOfWheels_desc" : "NrOfWheels";
            ViewBag.ColorSortParm = sortOrder == "Color" ? "Color_desc" : "Color";
            ViewBag.BrandSortParm = sortOrder == "Brand" ? "Brand_desc" : "Brand";
            ViewBag.StartTimeSortParm = sortOrder == "StartTime" ? "StartTime_desc" : "StartTime";

            var vehicles = from v in parkedVehicles select v;

            if (!String.IsNullOrEmpty(SearchString))
            {
                SearchString = SearchString.ToUpper();

                vehicles = vehicles.Where(s => s.RegNr.Contains(SearchString) || s.VehicleType.ToString().ToUpper().Contains(SearchString) || s.NrOfWheels.ToString().ToUpper().Contains(SearchString) || s.Color.ToUpper().Contains(SearchString) || s.Model.ToUpper().Contains(SearchString) || s.Brand.ToUpper().Contains(SearchString) || s.StartTime.ToString().Contains(SearchString));
                //return RedirectToAction("Details",vehicles);
            }

            switch (sortOrder)
            {
                case "RegNr_desc":
                    vehicles = vehicles.OrderByDescending(s => s.RegNr);
                    break;
                case "VehicleType":
                    vehicles = vehicles.OrderBy(s => s.VehicleType);
                    break;
                case "VehicleType_desc":
                    vehicles = vehicles.OrderByDescending(s => s.VehicleType);
                    break;
                case "Model":
                    vehicles = vehicles.OrderBy(s => s.Model);
                    break;
                case "Model_desc":
                    vehicles = vehicles.OrderByDescending(s => s.NrOfWheels);
                    break;
                case "NrOfWheels":
                    vehicles = vehicles.OrderBy(s => s.NrOfWheels);
                    break;
                case "NrOfWheels_desc":
                    vehicles = vehicles.OrderByDescending(s => s.Model);
                    break;
                case "Color":
                    vehicles = vehicles.OrderBy(s => s.Color);
                    break;
                case "Color_desc":
                    vehicles = vehicles.OrderByDescending(s => s.Color);
                    break;
                case "Brand":
                    vehicles = vehicles.OrderBy(s => s.Brand);
                    break;
                case "Brand_desc":
                    vehicles = vehicles.OrderByDescending(s => s.Brand);
                    break;
                case "StartTime":
                    vehicles = vehicles.OrderBy(s => s.StartTime);
                    break;
                case "StartTime_desc":
                    vehicles = vehicles.OrderByDescending(s => s.StartTime);
                    break;

                default:
                    vehicles = vehicles.OrderBy(s => s.RegNr);
                    break;
            }
            return View(vehicles.ToList());

            }


            // GET: ParkedVehicles/Details/5
            public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkedVehicle = await _context.ParkedVehicle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parkedVehicle == null)
            {
                return NotFound();
            }

            return View(parkedVehicle);
        }

        // GET: ParkedVehicles/Create
        public IActionResult CheckIn()
        {
            return View();
        }

        // POST: ParkedVehicles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn([Bind("Id,VehicleType,RegNr,NrOfWheels,Color,Model,Brand,StartTime,Parked")] ParkedVehicle parkedVehicle)
        {
            if (ModelState.IsValid)
            {
                parkedVehicle.RegNr.ToUpper();
                parkedVehicle.StartTime = DateTime.Now;
                parkedVehicle.Parked = true;
                _context.Add(parkedVehicle);
                await _context.SaveChangesAsync();
                TempData["Success"] = "CheckIn Is Done Successfully!";
               return RedirectToAction(nameof(Index));
            }
            else{
                TempData["Failure"] = "CheckIn Can not be Done!";

            }
           
            return View(parkedVehicle);
        }

        [HttpGet]
        public async Task<IActionResult> CheckOut(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(ParkedVehicles));
            }
            var model = await _context.ParkedVehicle
            .FirstOrDefaultAsync(m => m.Id == id);


            if (model.Parked == false)
            {
                RedirectToAction(nameof(ParkedVehicles));
            }
            return View(model);
        }

        public ActionResult Receipt(int? id)
        {
            if(id==null){
                return NotFound();
            }
        
            var exitvehicle = _context.ParkedVehicle.FirstOrDefault(v => v.Id == id);
            if(exitvehicle==null){
                return NotFound();
            }
            //exitvehicle.Parked = false;  //temporary, uncomment later
          
            var kvitto = new Kvitto();
            kvitto.RegNr = exitvehicle.RegNr;
            kvitto.StartTime = exitvehicle.StartTime;
            kvitto.EndTime = DateTime.Now;
            var between = kvitto.EndTime.Subtract(kvitto.StartTime);
            kvitto.Duration=string.Format("{0} dagar,{1} timmar,{2} minuter", between.Days, between.Hours, between.Minutes);
            kvitto.TotalPrice = Math.Floor(between.TotalMinutes * 0.7 );

            _context.SaveChangesAsync();

            string ReceiptsData = kvitto.EndTime.Year.ToString() + "_" 
                + (kvitto.EndTime.Month < 10 ? "0" : "") + kvitto.EndTime.Month.ToString() + "_" 
                + kvitto.EndTime.Day.ToString() + "_" 
                + kvitto.EndTime.Hour.ToString() + "_" 
                + kvitto.EndTime.Minute.ToString() + "_"+ 
                kvitto.RegNr.ToString();
       
            var webRootPath = _hostingEnvironment.WebRootPath;
            var contentRootPath = _hostingEnvironment.ContentRootPath; // Not used (HK/MK)
            string receiptsfolder = "\\Receipts";
            if(!Directory.Exists(webRootPath + receiptsfolder))
            {
                Directory.CreateDirectory(webRootPath + receiptsfolder);
            }

            var path = webRootPath + $"/Receipts/" + $"/{ReceiptsData}.dat";

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, kvitto);
            stream.Close();

            //Not working, why?
            //JsonSerializer serializer = new JsonSerializer();
            //serializer.Converters.Add(new JavaScriptDateTimeConverter());
            //serializer.NullValueHandling = NullValueHandling.Ignore;
            //Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            //using (StreamWriter sw = new StreamWriter(path))
            //using (JsonWriter writer = new JsonTextWriter(sw))
            //{
            //    serializer.Serialize(writer, kvitto);
            //}

            return View(kvitto);
        }

        public ActionResult ShowReceipts()
        {
            //show all files with receipts
            var webRootPath = _hostingEnvironment.WebRootPath;
            var contentRootPath = _hostingEnvironment.ContentRootPath;
            var receiptsPath = webRootPath + $"/Receipts/";
            int lngth = receiptsPath.Length;

            List<string> filesList = Directory.GetFiles(receiptsPath).Select(p => p.Remove(0, lngth)).ToList();

            //List<string> list = Directory.GetFiles(path).Select(p => p.Remove(2, 2)).ToList(); //OK!
            //IEnumerable<string> fileList = array.ToList().Select(p => p.Remove(1, 2));
            //IEnumerable<string> fileList2 = fileList.Select(p => p.Remove(1, 2));
            //fileList = fileList.ToList();
            //Here we read and display all files. Use Async!
            ViewBag.Message = "Receipts";

            return View(filesList);
        }

        public ActionResult ReceiptDetails(string fileName)
        {
            if (fileName == null)
            {
                return NotFound();
            }

            var webRootPath = _hostingEnvironment.WebRootPath;
            var path = webRootPath + $"/Receipts/";

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path + $"{fileName}", FileMode.Open, FileAccess.Read);
            Kvitto kvitto = (Kvitto)formatter.Deserialize(stream);

            if (kvitto == null)
            {
                return NotFound();
            }

            //return NotFound();
            return View(kvitto);
        }

        public ActionResult AllVehicles(string sortOrder, string SearchString)
        {
            ViewBag.RegSortParm = String.IsNullOrEmpty(sortOrder) ? "RegNr_desc" : "";
            ViewBag.VTypeSortParm = sortOrder == "VehicleType" ? "VehicleType_desc" : "VehicleType";
            ViewBag.ModelSortParm = sortOrder == "Model" ? "Model_desc" : "Model";
            ViewBag.NRWheelSortParm = sortOrder == "NrOfWheels" ? "NrOfWheels_desc" : "NrOfWheels";
            ViewBag.ColorSortParm = sortOrder == "Color" ? "Color_desc" : "Color";
            ViewBag.BrandSortParm = sortOrder == "Brand" ? "Brand_desc" : "Brand";
            ViewBag.StartTimeSortParm = sortOrder == "StartTime" ? "StartTime_desc" : "StartTime";

            var vehicles = from v in _context.ParkedVehicle select v;

            if (!String.IsNullOrEmpty(SearchString))
            {
                SearchString = SearchString.ToUpper();

                vehicles = vehicles.Where(s => s.RegNr.Contains(SearchString) || s.VehicleType.ToString().ToUpper().Contains(SearchString) || s.NrOfWheels.ToString().ToUpper().Contains(SearchString) || s.Color.ToUpper().Contains(SearchString) || s.Model.ToUpper().Contains(SearchString) || s.Brand.ToUpper().Contains(SearchString) || s.StartTime.ToString().Contains(SearchString));
                //return RedirectToAction("Details",vehicles);
            }


            switch (sortOrder)
            {
                case "RegNr_desc":
                    vehicles = vehicles.OrderByDescending(s => s.RegNr);
                    break;
                case "VehicleType":
                    vehicles = vehicles.OrderBy(s => s.VehicleType);
                    break;
                case "VehicleType_desc":
                    vehicles = vehicles.OrderByDescending(s => s.VehicleType);
                    break;
                case "Model":
                    vehicles = vehicles.OrderBy(s => s.Model);
                    break;
                case "Model_desc":
                    vehicles = vehicles.OrderByDescending(s => s.NrOfWheels);
                    break;
                case "NrOfWheels":
                    vehicles = vehicles.OrderBy(s => s.NrOfWheels);
                    break;
                case "NrOfWheels_desc":
                    vehicles = vehicles.OrderByDescending(s => s.Model);
                    break;
                case "Color":
                    vehicles = vehicles.OrderBy(s => s.Color);
                    break;
                case "Color_desc":
                    vehicles = vehicles.OrderByDescending(s => s.Color);
                    break;
                case "Brand":
                    vehicles = vehicles.OrderBy(s => s.Brand);
                    break;
                case "Brand_desc":
                    vehicles = vehicles.OrderByDescending(s => s.Brand);
                    break;
                case "StartTime":
                    vehicles = vehicles.OrderBy(s => s.StartTime);
                    break;
                case "StartTime_desc":
                    vehicles = vehicles.OrderByDescending(s => s.StartTime);
                    break;

                default:
                    vehicles = vehicles.OrderBy(s => s.RegNr);
                    break;
            }
            return View(vehicles.ToList());
        }

        // GET: ParkedVehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkedVehicle = await _context.ParkedVehicle.FindAsync(id);
            if (parkedVehicle == null)
            {
                return NotFound();
            }
            return View(parkedVehicle);
        }

        // POST: ParkedVehicles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VehicleType,RegNr,NrOfWheels,Color,Model,Brand")] ParkedVehicle parkedVehicle)
        {
            if (id != parkedVehicle.Id)
            {
                TempData["Failure"]="Vehicle With This Id Does Not Exist!";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var editVehicle = _context.ParkedVehicle.AsNoTracking().FirstOrDefault(v => v.Id == id);
                parkedVehicle.Parked = editVehicle.Parked;
                parkedVehicle.StartTime = editVehicle.StartTime;
                try
                {
                  
                    _context.Update(parkedVehicle);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Edit IS Done Successfully!";

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParkedVehicleExists(parkedVehicle.Id))
                    {
                        TempData["Failure"] = "Update Could Not Be Done!";
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

              
                return RedirectToAction(nameof(Index));
            }else{
                TempData["Failure"] = "Edit Could not be Done!";
            }
            return View(parkedVehicle);
        }

        // GET: ParkedVehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkedVehicle = await _context.ParkedVehicle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parkedVehicle == null)
            {
               
                return NotFound();
            }

    
            return View(parkedVehicle);
        }

        // POST: ParkedVehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var parkedVehicle = await _context.ParkedVehicle.FindAsync(id);
            _context.ParkedVehicle.Remove(parkedVehicle);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Vehicle Removed From DataBase Successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool ParkedVehicleExists(int id)
        {
            return _context.ParkedVehicle.Any(e => e.Id == id);
        }
    }
}
