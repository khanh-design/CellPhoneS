using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CPhoneS.Models;

namespace CPhoneS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminKhachHangController : Controller
    {
        private readonly CPhoneSContext _context;

        public AdminKhachHangController(CPhoneSContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminKhachHang
        public async Task<IActionResult> Index()
        {
            var customersWithOrders = await _context.KhachHangs
                                                    .Include(kh => kh.DonHangs) // Bao gồm đơn hàng của mỗi khách hàng
                                                    .ThenInclude(dh => dh.ChiTietDonHangs) // Bao gồm chi tiết đơn hàng của mỗi đơn hàng
                                                    .ToListAsync();
            return View(customersWithOrders);
        }

        // GET: Admin/AdminKhachHang/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy thông tin khách hàng và các đơn hàng của họ
            var khachHang = await _context.KhachHangs
                .Include(kh => kh.DonHangs) // Bao gồm đơn hàng của khách hàng
                .ThenInclude(dh => dh.ChiTietDonHangs) // Bao gồm chi tiết đơn hàng
                .FirstOrDefaultAsync(m => m.MaKh == id);

            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        // GET: Admin/AdminKhachHang/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaKh,TenKh,Diachi,Ngaysinh,Phone,Email,CreateDate")] KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(khachHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(khachHang);
        }

        // GET: Admin/AdminKhachHang/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }
            return View(khachHang);
        }

        // POST: Admin/AdminKhachHang/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaKh,TenKh,Diachi,Ngaysinh,Phone,Email,CreateDate")] KhachHang khachHang)
        {
            if (id != khachHang.MaKh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khachHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachHangExists(khachHang.MaKh))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(khachHang);
        }

        // GET: Admin/AdminKhachHang/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.MaKh == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        // POST: Admin/AdminKhachHang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khachHang = await _context.KhachHangs.FindAsync(id);
            _context.KhachHangs.Remove(khachHang);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KhachHangExists(int id)
        {
            return _context.KhachHangs.Any(e => e.MaKh == id);
        }
    }
}
