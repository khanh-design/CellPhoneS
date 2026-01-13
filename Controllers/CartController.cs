using Microsoft.AspNetCore.Mvc;
using CPhoneS.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace CPhoneS.Controllers
{
    public class CartController : Controller
    {
        private readonly CPhoneSContext _context;

        public CartController(CPhoneSContext context)
        {
            _context = context;
        }

        // Lấy giỏ hàng từ Session
        private Dictionary<int, int> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("Cart");

            if (string.IsNullOrEmpty(cartJson))
            {
                return new Dictionary<int, int>(); // Trả về giỏ hàng trống nếu không có gì trong session
            }

            try
            {
                return JsonSerializer.Deserialize<Dictionary<int, int>>(cartJson) ?? new Dictionary<int, int>();
            }
            catch (JsonException)
            {
                TempData["Error"] = "Lỗi khi xử lý giỏ hàng trong Session.";
                return new Dictionary<int, int>(); // Nếu lỗi, trả về giỏ hàng trống
            }
        }

        // Lưu giỏ hàng vào Session
        private void SaveCartToSession(Dictionary<int, int> cart)
        {
            try
            {
                var cartJson = JsonSerializer.Serialize(cart);
                HttpContext.Session.SetString("Cart", cartJson);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi khi lưu giỏ hàng vào Session!";
                Console.WriteLine(ex.Message); // Log lỗi nếu cần thiết
            }
        }

        // Hiển thị giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCartFromSession();

            // Lấy thông tin sản phẩm từ cơ sở dữ liệu
            var cartItems = cart.Select(group =>
            {
                var product = _context.SanPhams.FirstOrDefault(p => p.MaSp == group.Key);
                if (product == null)
                {
                    return null; // Xử lý nếu sản phẩm không tồn tại
                }

                return new CartItem
                {
                    ProductId = group.Key,
                    ProductName = product.TenSp,
                    Quantity = group.Value,
                    Price = product.GiaSp,
                    ImageUrl = product.AnhSp
                };
            }).Where(item => item != null).ToList();

            // Tính tổng giá trị giỏ hàng
            decimal totalAmount = cartItems.Sum(item => item.Price * item.Quantity);
            ViewBag.TotalAmount = totalAmount;

            return View(cartItems);
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public IActionResult AddToCart(int id, int quantity)
        {
            if (quantity <= 0)
            {
                TempData["Error"] = "Số lượng sản phẩm phải lớn hơn 0!";
                return RedirectToAction("Details", "SanPham", new { id });
            }

            var product = _context.SanPhams.FirstOrDefault(p => p.MaSp == id);
            if (product == null)
            {
                TempData["Error"] = "Sản phẩm không tồn tại!";
                return RedirectToAction("Index");
            }

            var cart = GetCartFromSession();

            // Cập nhật giỏ hàng nếu sản phẩm đã tồn tại
            if (cart.ContainsKey(id))
            {
                cart[id] += quantity;
            }
            else
            {
                cart[id] = quantity;
            }

            // Lưu giỏ hàng vào session
            SaveCartToSession(cart);

            TempData["Message"] = "Sản phẩm đã được thêm vào giỏ hàng!";

            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ hàng
        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCartFromSession();

            if (cart.ContainsKey(id))
            {
                cart.Remove(id);
                SaveCartToSession(cart);
                TempData["Message"] = "Sản phẩm đã được xóa khỏi giỏ hàng!";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy sản phẩm trong giỏ hàng!";
            }

            return RedirectToAction("Index");
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            if (quantity <= 0)
            {
                // Nếu số lượng <= 0, xóa sản phẩm khỏi giỏ hàng
                return RedirectToAction("RemoveFromCart", new { id });
            }

            var cart = GetCartFromSession();

            if (cart.ContainsKey(id))
            {
                cart[id] = quantity;
                SaveCartToSession(cart);
                TempData["Message"] = "Số lượng sản phẩm đã được cập nhật!";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy sản phẩm trong giỏ hàng!";
            }

            return RedirectToAction("Index");
        }

        // Xóa toàn bộ giỏ hàng
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            TempData["Message"] = "Giỏ hàng đã được xóa!";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult ProcessPayment(string Name, string Address, string PaymentMethod)
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            var cart = string.IsNullOrEmpty(cartJson) ? new Dictionary<int, int>() : JsonSerializer.Deserialize<Dictionary<int, int>>(cartJson);

            if (cart.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn trống. Vui lòng thêm sản phẩm vào giỏ hàng.";
                return RedirectToAction("Index");
            }

            var cartItems = cart.Select(group =>
            {
                var product = _context.SanPhams.FirstOrDefault(p => p.MaSp == group.Key);
                return new CartItem
                {
                    ProductId = group.Key,
                    ProductName = product?.TenSp,
                    Quantity = group.Value,
                    Price = product?.GiaSp ?? 0,
                    ImageUrl = product?.AnhSp
                };
            }).ToList();

            decimal totalAmount = cartItems.Sum(item => item.Price * item.Quantity);

            var order = new Order
            {
                CustomerName = Name,
                CustomerAddress = Address,
                PaymentMethod = PaymentMethod,
                TotalAmount = totalAmount,
                OrderDate = DateTime.Now,
                Status = "Pending" // Trạng thái mặc định
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                _context.OrderDetails.Add(orderDetail);
            }
            _context.SaveChanges();

            HttpContext.Session.Remove("Cart");

            TempData["SuccessMessage"] = "Thanh toán thành công! Bạn có thể theo dõi đơn hàng trong mục 'Đơn hàng của tôi'.";

            return RedirectToAction("MyOrders", "Orders");
        }


        // Thanh toán giỏ hàng
        public IActionResult Checkout()
        {
            var cart = GetCartFromSession();

            if (cart.Count == 0)
            {
                TempData["Error"] = "Giỏ hàng của bạn trống. Vui lòng thêm sản phẩm trước khi thanh toán!";
                return RedirectToAction("Index");
            }

            var cartItems = cart.Select(group =>
            {
                var product = _context.SanPhams.FirstOrDefault(p => p.MaSp == group.Key);
                if (product == null)
                {
                    return null; // Nếu không tìm thấy sản phẩm
                }

                return new CartItem
                {
                    ProductId = group.Key,
                    ProductName = product.TenSp,
                    Quantity = group.Value,
                    Price = product.GiaSp,
                    ImageUrl = product.AnhSp
                };
            }).Where(item => item != null).ToList();

            // Tính tổng số tiền
            decimal totalAmount = cartItems.Sum(item => item.Price * item.Quantity);
            ViewBag.TotalAmount = totalAmount;

            return View(cartItems);
        }

        // Xác nhận thanh toán và lưu thông tin đơn hàng
        [HttpPost]
        public IActionResult ConfirmCheckout(string name, string address, string paymentMethod)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(paymentMethod))
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin thanh toán!";
                return RedirectToAction("Checkout");
            }

            var cart = GetCartFromSession();

            if (cart.Count == 0)
            {
                TempData["Error"] = "Giỏ hàng của bạn trống. Vui lòng thêm sản phẩm trước khi thanh toán!";
                return RedirectToAction("Index");
            }

            var cartItems = cart.Select(group =>
            {
                var product = _context.SanPhams.FirstOrDefault(p => p.MaSp == group.Key);
                if (product == null)
                {
                    return null;
                }

                return new CartItem
                {
                    ProductId = group.Key,
                    ProductName = product.TenSp,
                    Quantity = group.Value,
                    Price = product.GiaSp,
                    ImageUrl = product.AnhSp
                };
            }).Where(item => item != null).ToList();

            decimal totalAmount = cartItems.Sum(item => item.Price * item.Quantity);

            var order = new Order
            {
                CustomerName = name,
                CustomerAddress = address,
                PaymentMethod = paymentMethod,
                TotalAmount = totalAmount,
                OrderDate = DateTime.Now
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };

                _context.OrderDetails.Add(orderDetail);
            }

            _context.SaveChanges();

            HttpContext.Session.Remove("Cart");

            TempData["Message"] = "Thanh toán thành công!";
            return RedirectToAction("Index");
        }
    }
}
