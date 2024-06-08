using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanTraiCay05.Models;
using WebsiteBanTraiCay05.Models.EF;
using WebsiteBanTraiCay05.Models.Payment;

namespace WebsiteBanTraiCay05.Controllers
{
    public class CartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ShoppingCart
        public ActionResult Index()
        {
            Cart cart = (Cart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return View(cart.Items);
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };
            var checkProduct = db.Products.FirstOrDefault(x => x.Id == id);
            if (checkProduct != null)
            {
                Cart cart = (Cart)Session["Cart"];
                if (cart == null)
                {
                    cart = new Cart();
                }
                CartItem item = new CartItem
                {
                    ProductId = checkProduct.Id,
                    ProductName = checkProduct.Title,
                    CategoryName = checkProduct.ProductCategory.Title,
                    Alias = checkProduct.Alias,
                    Quantity = quantity
                };
                if (checkProduct.ProductImage.FirstOrDefault(x => x.IsDefault) != null)
                {
                    item.ProductImg = checkProduct.ProductImage.FirstOrDefault(x => x.IsDefault).Image;
                }
                item.Price = checkProduct.Price;
                if (checkProduct.PriceSale > 0)
                {
                    item.Price = (decimal)checkProduct.PriceSale;
                }
                item.TotalPrice = item.Price * item.Quantity;
                cart.AddToCart(item, quantity);
                Session["Cart"] = cart;
                code = new { Success = true, msg = "Thêm sản phẩm vào giỏ thành công!", code = 1, Count = cart.Items.Count };
            }
            return Json(code);
        }

        [HttpPost]
        public ActionResult Update(int id, int quantity)
        {
            Cart cart = (Cart)Session["Cart"];
            if (cart != null)
            {
                cart.UpdateQuantity(id, quantity);
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };
            Cart cart = (Cart)Session["Cart"];
            if (cart != null)
            {
                var checkProduct = cart.Items.FirstOrDefault(x => x.ProductId == id);
                if (checkProduct != null)
                {
                    cart.Remove(id);
                    code = new { Success = true, msg = "", code = 1, Count = cart.Items.Count };
                }
            }
            return Json(code);
        }

        public ActionResult VnpayReturn()
        {
            var a = UrlPayment(0, "DH8405");
            return View();
        }

        public ActionResult CheckOut()
        {
            Cart cart = (Cart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(OrderViewModel req)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            if (ModelState.IsValid)
            {
                Cart cart = (Cart)Session["Cart"];
                if (cart != null)
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            Order order = new Order();
                            order.CustomerName = req.CustomerName;
                            order.Phone = req.Phone;
                            order.Address = req.Address;
                            order.Email = req.Email;
                            cart.Items.ForEach(x => order.OrderDetails.Add(new OrderDetails
                            {
                                ProductId = x.ProductId,
                                Quantity = x.Quantity,
                                Price = x.Price
                            }));
                            order.TotalAmount = cart.Items.Sum(x => (x.Price * x.Quantity));
                            order.TypePayment = req.TypePayment;
                            order.CreatedDate = DateTime.Now;
                            order.ModifiedDate = DateTime.Now;
                            order.CreatedBy = req.Phone;
                            Random random = new Random();
                            order.Code = "DH" + random.Next(0, 9) + random.Next(0, 9) + random.Next(0, 9) + random.Next(0, 9);
                            // Thêm đơn hàng vào cơ sở dữ liệu
                            db.Orders.Add(order);
                            db.SaveChanges();
                            ////// Gửi mail cho khách hàng
                            //var strProduct = "";
                            //var intoMoney = decimal.Zero;
                            //var totalAmount = decimal.Zero;
                            //foreach (var product in cart.Items)
                            //{
                            //    strProduct += "<tr>";
                            //    strProduct += "<td>" + product.ProductName + "</td>";
                            //    strProduct += "<td>" + product.Quantity + "</td>";
                            //    strProduct += "<td>" + WebsiteBanTraiCay05.Common.Common.FormatNumber(product.TotalPrice, 0) + "</td>";
                            //    strProduct += "</tr>";
                            //    intoMoney += product.Price * product.Quantity;
                            //}
                            //totalAmount = intoMoney;
                            //string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/sendEmailClient.html"));
                            //contentCustomer = contentCustomer.Replace("{{MaDon}}", order.Code);
                            //contentCustomer = contentCustomer.Replace("{{SanPham}}", strProduct);
                            //contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                            //contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", order.CustomerName);
                            //contentCustomer = contentCustomer.Replace("{{Phone}}", order.Phone);
                            //contentCustomer = contentCustomer.Replace("{{Email}}", req.Email);
                            //contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", order.Address);
                            //contentCustomer = contentCustomer.Replace("{{ThanhTien}}", WebsiteBanTraiCay05.Common.Common.FormatNumber(intoMoney, 0));
                            //contentCustomer = contentCustomer.Replace("{{TongTien}}", WebsiteBanTraiCay05.Common.Common.FormatNumber(totalAmount + 30000, 0));
                            //WebsiteBanTraiCay05.Common.Common.SendMail("Cửa hàng trái cây CNS", "Đơn hàng #" + order.Code, contentCustomer.ToString(), req.Email);
                            //string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/sendEmailAdmin.html"));
                            //contentAdmin = contentAdmin.Replace("{{MaDon}}", order.Code);
                            //contentAdmin = contentAdmin.Replace("{{SanPham}}", strProduct);
                            //contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                            //contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", order.CustomerName);
                            //contentAdmin = contentAdmin.Replace("{{Phone}}", order.Phone);
                            //contentAdmin = contentAdmin.Replace("{{Email}}", req.Email);
                            //contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", order.Address);
                            //contentAdmin = contentAdmin.Replace("{{ThanhTien}}", WebsiteBanTraiCay05.Common.Common.FormatNumber(intoMoney, 0));
                            //contentAdmin = contentAdmin.Replace("{{TongTien}}", WebsiteBanTraiCay05.Common.Common.FormatNumber(totalAmount + 30000, 0));
                            //WebsiteBanTraiCay05.Common.Common.SendMail("Cửa hàng trái cây CNS", "Đơn hàng mới #" + order.Code, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);
                            // Clear cart sau khi đã đặt hàng thành công
                            cart.ClearCart();
                            code = new { Success = true, Code = req.TypePayment, Url = "" };
                            if (req.TypePayment == 2)
                            {
                                var url = UrlPayment(req.TypePaymentVN, order.Code);
                                code = new { Success = true, Code = req.TypePayment, Url = url };
                            }
                            // Commit giao dịch nếu mọi thứ thành công
                            transaction.Commit();
                            return RedirectToAction("CheckOutSuccess");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return View("Error");
                        }
                    }
                }
            }
            return Json(code);
        }

        public ActionResult CheckOutSuccess()
        {
            return View();
        }

        public ActionResult ShowCount()
        {
            Cart cart = (Cart)Session["Cart"];
            if (cart != null)
            {
                return Json(new { Count = cart.Items.Count }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Partial_CartItem()
        {
            Cart cart = (Cart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }

        public ActionResult Partial_InformationProduct()
        {
           Cart cart = (Cart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }

        public ActionResult Partial_InformationCustomer()
        {
            return PartialView();
        }

        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            var order = db.Orders.FirstOrDefault(x => x.Code == orderCode);
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var price = (long)order.TotalAmount * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng:" + order.Code);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Code); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing
            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return urlPayment;
        }
    }
}