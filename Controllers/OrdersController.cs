using Microsoft.AspNetCore.Mvc;
using CPhoneS.Models; // Đảm bảo dùng đúng namespace chứa DbContext và Models
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CPhoneS.Controllers
{
    public class OrdersController : Controller
    {
        private readonly CPhoneSContext _context;

        // Constructor để inject DbContext
        public OrdersController(CPhoneSContext context)
        {
            _context = context;
        }

        // Action hiển thị danh sách đơn hàng
        public IActionResult Index()
        {
            var orders = _context.Orders.ToList(); // Lấy danh sách đơn hàng
            return View(orders);
        }

        // Action hiển thị chi tiết đơn hàng
        public IActionResult Details(int id)
        {
            // Tìm đơn hàng với các chi tiết liên quan
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            // Gửi chi tiết đơn hàng vào ViewBag (nếu cần)
            ViewBag.OrderDetails = order.OrderDetails;

            // Truyền Order vào view
            return View(order);
        }
        public IActionResult MyOrders()
        {
            // Lấy danh sách đơn hàng từ cơ sở dữ liệu
            var orders = _context.Orders.ToList(); // Lọc theo khách hàng nếu có thông tin đăng nhập

            return View(orders);
        }

        public IActionResult OrderDetails(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            // Lấy chi tiết đơn hàng và thông tin sản phẩm
            var orderDetails = _context.OrderDetails
                .Where(od => od.OrderId == id)
                .Include(od => od.Product)  // Nạp thông tin sản phẩm liên quan
                .ToList();

            // Truyền dữ liệu vào ViewBag
            ViewBag.OrderDetails = orderDetails;

            return View(order);
        }

        // Action cập nhật trạng thái đơn hàng
        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            _context.SaveChanges();

            TempData["Message"] = "Trạng thái đơn hàng đã được cập nhật!";
            return RedirectToAction("Index");
        }

        public IActionResult PlaceOrder(Order order)
        {
            if (order.OrderDetails.Any(od => od.Quantity > 20))
            {
                TempData["Message"] = "Vui lòng liên hệ Zalo để đặt số lượng lớn.";
                return RedirectToAction("Index", "Home"); // Trang phù hợp
            }

            // Thực hiện logic lưu đơn hàng
            _context.Orders.Add(order);
            _context.SaveChanges();

            TempData["Message"] = "Đơn hàng của bạn đã được đặt thành công!";
            return RedirectToAction("OrderSuccess", "Order");
        }

        public IActionResult OrderSuccess()
        {
            return View(); // Tạo view để thông báo thành công
        }
    }
}
