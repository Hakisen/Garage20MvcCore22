    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage20MvcCore22.Models;
using Garage20MvcCore22.ViewModels;

namespace Garage20MvcCore22.Controllers
{
    public class ParkedVehiclesController : Controller
    {
        private readonly Garage20MvcCore22Context _context;

        public ParkedVehiclesController(Garage20MvcCore22Context context)
        {
            _context = context;
        }

        // GET: ParkedVehicles
        public async Task<IActionResult> Index()
        {
            //ParkedVehicle parkedVehicle = new ParkedVehicle();
            return View(await _context.ParkedVehicle.ToListAsync());
            //return View();
        }

        public async Task<IActionResult> AllVehicles()
        {
            return View(await _context.ParkedVehicle.ToListAsync());
        }

        public async Task<IActionResult> ParkedVehicles()
        {
            return View(await _context.ParkedVehicle.Where(p => p.Parked == true).ToListAsync());
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
                return View(nameof(ParkedVehicle));
            }
            var model = await _context.ParkedVehicle
            .FirstOrDefaultAsync(m => m.Id == id);


            if (model.Parked == false)
            {
                RedirectToAction(nameof(ParkedVehicle));
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
            exitvehicle.Parked = false;
          
            var kvitto = new Kvitto();
            kvitto.StartTime = exitvehicle.StartTime;
            kvitto.EndTime = DateTime.Now;
            var between = kvitto.EndTime.Subtract(kvitto.StartTime);
            kvitto.Duration=string.Format("{0} dagar,{1} timmar,{2} minuter", between.Days, between.Hours, between.Minutes);
            kvitto.TotalPrice = Math.Floor(between.TotalMinutes * 0.7);

            _context.SaveChangesAsync();

            return View(kvitto);
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
