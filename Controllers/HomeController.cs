using CPhoneS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CPhoneS.Controllers
{
    public class HomeController : Controller
    {
        private readonly CPhoneSContext _context;

        public HomeController(CPhoneSContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var CPhoneSContext = _context.SanPhams.Include(s => s.MaDmNavigation);
            return View(await CPhoneSContext.ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
