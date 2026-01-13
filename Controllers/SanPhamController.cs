using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using CPhoneS.Models;
using System.Text.Json;

namespace CPhoneS.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly CPhoneSContext _context;

        // Constructor: Inject DbContext để làm việc với cơ sở dữ liệu
        public SanPhamController(CPhoneSContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var CPhoneSContext = _context.SanPhams.Include(s => s.MaDmNavigation);
            return View(await CPhoneSContext.ToListAsync());
        }

        // Hiển thị chi tiết của một sản phẩm
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaDmNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // Thêm sản phẩm vào giỏ hàng
        public IActionResult AddToCart(int id)
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            if (!IsUserLoggedIn())
            {
                TempData["Message"] = "Vui lòng đăng nhập trước khi mua hàng.";
                return RedirectToAction("Login", "Account"); // Chuyển hướng tới trang đăng nhập
            }

            var sanPham = _context.SanPhams.FirstOrDefault(s => s.MaSp == id);
            if (sanPham == null)
            {
                TempData["Message"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index");
            }

            // Lấy giỏ hàng từ Session (nếu có)
            List<CartItem> cart = GetCartFromSession();

            // Kiểm tra nếu sản phẩm đã có trong giỏ hàng
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);
            if (cartItem != null)
            {
                // Nếu sản phẩm đã có trong giỏ hàng, tăng số lượng
                cartItem.Quantity += 1;
            }
            else
            {
                // Nếu sản phẩm chưa có, thêm sản phẩm mới vào giỏ hàng
                cart.Add(new CartItem { ProductId = id, Quantity = 1 });
            }

            // Lưu giỏ hàng vào Session
            SaveCartToSession(cart);

            TempData["Message"] = $"Đã thêm sản phẩm {sanPham.TenSp} vào giỏ hàng!";
            return RedirectToAction("Index", "Cart"); // Chuyển hướng đến giỏ hàng
        }

        // Kiểm tra người dùng đã đăng nhập chưa (có thông tin trong session)
        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserName"));
        }

        // Lấy giỏ hàng từ Session
        private List<CartItem> GetCartFromSession()
        {
            var cartData = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartData))
            {
                return new List<CartItem>();  // Trả về giỏ hàng rỗng nếu không có gì trong Session
            }

            try
            {
                // Nếu có dữ liệu, chuyển chuỗi JSON thành danh sách CartItem
                return JsonSerializer.Deserialize<List<CartItem>>(cartData) ?? new List<CartItem>();
            }
            catch (JsonException ex)
            {
                // Log lỗi và trả về giỏ hàng rỗng nếu có lỗi khi giải mã
                Console.WriteLine($"Error deserializing cart data: {ex.Message}");
                return new List<CartItem>();  // Trả về giỏ hàng rỗng
            }
        }

        // Lưu giỏ hàng vào Session
        private void SaveCartToSession(List<CartItem> cart)
        {
            try
            {
                var cartData = JsonSerializer.Serialize(cart);  // Chuyển danh sách CartItem thành chuỗi JSON
                HttpContext.Session.SetString("Cart", cartData); // Lưu vào Session
            }
            catch (Exception ex)
            {
                // Log lỗi nếu có vấn đề khi lưu giỏ hàng vào Session
                Console.WriteLine($"Error saving cart data: {ex.Message}");
            }
        }
    }
}