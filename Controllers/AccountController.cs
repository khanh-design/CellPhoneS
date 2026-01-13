using Microsoft.AspNetCore.Mvc;
using CPhoneS.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;
using System;

namespace CPhoneS.Controllers
{
    public class AccountController : Controller
    {
        private readonly CPhoneSContext _context;

        // Constructor để inject DbContext
        public AccountController(CPhoneSContext context)
        {
            _context = context;
        }

        // Action GET hiển thị form đăng ký
        public IActionResult Register()
        {
            return View();
        }

        // Action POST xử lý đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra nếu đã tồn tại user với Email hoặc UserName
                if (_context.ApplicationUsers.Any(u => u.UserName == model.UserName))
                {
                    ModelState.AddModelError("UserName", "Tên người dùng đã tồn tại.");
                    return View(model);
                }

                if (_context.ApplicationUsers.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại.");
                    return View(model);
                }

                // Mã hóa mật khẩu trước khi lưu vào cơ sở dữ liệu
                model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);

                // Lưu người dùng mới vào database
                _context.ApplicationUsers.Add(model);
                _context.SaveChanges();

                TempData["Message"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

        // Action GET hiển thị form đăng nhập
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl; // Lưu returnUrl để trả về trang trước đó sau khi đăng nhập
            return View();
        }

        // Action POST xử lý đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string userName, string password, string returnUrl = null)
        {
            // Kiểm tra tên người dùng và mật khẩu không để trống
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Tên người dùng hoặc mật khẩu không được để trống.");
                return View();  // Trả về view nếu thông tin không hợp lệ
            }

            // Lấy người dùng từ cơ sở dữ liệu dựa trên tên người dùng
            var user = _context.ApplicationUsers.FirstOrDefault(u => u.UserName == userName);

            // Kiểm tra nếu người dùng không tồn tại hoặc mật khẩu sai
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Tên người dùng hoặc mật khẩu không đúng.");
                return View();  // Trả về view nếu đăng nhập thất bại
            }

            // Lưu thông tin người dùng vào session
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("FullName", user.FullName ?? "");  // Lưu tên đầy đủ vào session

            // Tạo thông báo thành công khi đăng nhập
            TempData["Message"] = "Đăng nhập thành công!";

            // Kiểm tra nếu có returnUrl, chuyển hướng về đó
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Nếu không có returnUrl, chuyển hướng về trang chủ hoặc một trang khác
            return RedirectToAction("Index", "Home");
        }

        // Action đăng xuất
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Đăng xuất người dùng
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("FullName"); // Xóa tên đầy đủ khỏi session
            TempData["Message"] = "Đăng xuất thành công!";
            return RedirectToAction("Index", "Home");
        }

        // Kiểm tra xem người dùng đã đăng nhập chưa (có thông tin trong session)
        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserName"));
        }

        // Action để đảm bảo người dùng đã đăng nhập
        private IActionResult EnsureUserIsLoggedIn(string returnUrl = null)
        {
            if (!IsUserLoggedIn())
            {
                TempData["Message"] = "Vui lòng đăng nhập trước khi tiếp tục.";
                return RedirectToAction("Login", "Account", new { returnUrl });
            }
            return null; // Nếu đã đăng nhập, không làm gì thêm
        }

        // Hỗ trợ việc quay lại trang trước khi đăng nhập (nếu có)
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);  // Chuyển hướng đến URL local
            }
            else
            {
                return RedirectToAction("Index", "Home");  // Nếu không, chuyển hướng về trang chủ
            }
        }

        // Kiểm tra nếu người dùng đã đăng nhập trước khi truy cập vào các trang yêu cầu đăng nhập
        public IActionResult ProtectedPage()
        {
            var result = EnsureUserIsLoggedIn();
            if (result != null)
            {
                return result; // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            }

            // Nếu đã đăng nhập, hiển thị trang yêu cầu đăng nhập
            return View();
        }
    }
}
