using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TableBooking.Data;
using TableBooking.Models;

namespace TableBooking.Controllers
{
    public class BookingController : Controller
    {

        private readonly AppDbContext _context;
        public BookingController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(DateTime? date, TimeSpan? startTime, TimeSpan? endTime)
        {
            

            ViewBag.Date = date?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.Time = startTime?.ToString(@"hh\:mm") ?? DateTime.Now.ToString("HH:mm");

            ViewBag.EndTime = endTime?.ToString(@"hh\:mm") ?? DateTime.Now.AddHours(1).ToString("HH:mm");

            if (date.HasValue && startTime.HasValue && endTime.HasValue)
            {
                var start = date.Value.Date + startTime.Value;
                var end = date.Value.Date + endTime.Value;

                var reservedTableIds = _context.Reservations
        .Where(r => r.Date == date)
        .AsEnumerable() // Переходить на LINQ to Objects
        .Where(r =>
            start < r.Date + r.EndTime &&
            end > r.Date + r.StartTime)
        .Select(r => r.TableId)
        .ToList();


                var availableTables = _context.Tables
                    .Where(t => !reservedTableIds.Contains(t.Id))
                    .ToList();

                return View(availableTables);
            }

            var allTables = _context.Tables.ToList();
            return View(allTables);
        }


        [HttpGet]
        public IActionResult TableById(int tableId)
        {
            var selectedTable = _context.Tables.FirstOrDefault(t => t.Id == tableId);
            return View(selectedTable);
        }

        [HttpPost]
        public IActionResult MakeReservation(int tableId,   DateTime? date, TimeSpan? startTime, TimeSpan? endTime)
        {
            if(!date.HasValue || !startTime.HasValue || !endTime.HasValue)
            {
                return BadRequest("Pleasee choose data and time");
            }
            DateTime start = date.Value.Date + startTime.Value;
            DateTime end = date.Value.Date + endTime.Value;


            var reservations = _context.Reservations
        .Where(r => r.TableId == tableId && r.Date == date.Value.Date)
        .ToList();

            bool isBusy = reservations.Any(r =>
       r.TableId == tableId &&
       date.Value.Date == r.Date &&
       (
           (start < r.Date.Add(r.EndTime)) &&
           (end > r.Date.Add(r.StartTime))
       )
   );

            if (isBusy)
            {
                TempData["Error"] = "This table is busy at the selected time!";
                return RedirectToAction("TableById", new { tableId });
            }
            var reservation = new Reservation
            {
                TableId = tableId,
                Date = date.Value.Date,
                StartTime = startTime.Value,
                EndTime = endTime.Value
            };

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            TempData["Success"] = "Table successfully reserved";
            return RedirectToAction("ReservationById", new { reservationId = reservation.Id });
        }

        public IActionResult ReservationById(int reservationId)
        {
            var reservation = _context.Reservations
        .Include(r => r.Table) 
        .FirstOrDefault(r => r.Id == reservationId);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        [HttpPost]
        public IActionResult DeleteReservationById(int reservationId)
        {
            
            var reservation = _context.Reservations.Find(reservationId);
            if (reservation == null)
            {
                return NotFound();
            }
            _context.Reservations.Remove(reservation);
            _context.SaveChanges();
            TempData["Success"] = "Reservation deleted successfully!";
            return RedirectToAction("Index","Booking");

        }
    }
}
