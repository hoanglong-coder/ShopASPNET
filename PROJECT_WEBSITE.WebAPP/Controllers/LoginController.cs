using PROJECT_WEBSITE.Data.DAO;
using PROJECT_WEBSITE.Data.EF;
using PROJECT_WEBSITE.WebAPP.Common;
using PROJECT_WEBSITE.WebAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace PROJECT_WEBSITE.WebAPP.Controllers
{
    public class LoginController : Controller
    {
        // GET: Admin/Login
        public ActionResult Index()
        {
            DbWebsite db = new DbWebsite();

            var lst = db.FooterCategories.ToList();
            ViewBag.ListCategoryFooter = lst;
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var dao = new CustomerDAO();
                var result = dao.Login(model.Phone, model.Password);
                if (result == 1)
                {
                    var customer = dao.GetById(model.Phone);
                    var customerSession = new CustomerLogin();
                    customerSession.Phone = customer.Phone;
                    customerSession.CustomerID = customer.CustomerID;
                    Session.Add(Common.CommonConstants.CUSTOMER_SESSION, customerSession);

                    //Thêm vào Cart
                    var cart = Session[Common.CommonConstants.CART_SESSION];
                    var list = (List<CartItem>)cart;
                    if (list.Count != 0)
                    {

                        string cartCustomer = new CustomerDAO().LoadCart(model.Phone);
                        List<CartItem> cartItems = new List<CartItem>();
                        if (cartCustomer != null)
                        {
                            XDocument xDocument = XDocument.Parse(cartCustomer);
                            var data = xDocument.Descendants("CartItem").Select(o => new
                            {
                                id = o.Element("ID").Value,
                                quan = o.Element("Quantity").Value,
                            });

                            foreach (var item in data)
                            {
                                int id = int.Parse(item.id);
                                string quan = item.quan;
                                CartItem cartItem = new CartItem();
                                cartItem.Product = new ProductDAO().DetailProduct(id);
                                cartItem.Quantity = int.Parse(quan);
                                cartItems.Add(cartItem);
                            }
                        }

                    }
                    //Lấy sản phẩm ra
                    string cartcustomer = new CustomerDAO().LoadCart(model.Phone);
                    if (cartcustomer != null)
                    {
                        XDocument xDocument = XDocument.Parse(cartcustomer);
                        var data = xDocument.Descendants("CartItem").Select(o => new
                        {
                            id = o.Element("ID").Value,
                            quan = o.Element("Quantity").Value,
                        });
                        List<CartItem> cartItems = new List<CartItem>();
                        foreach (var item in data)
                        {
                            int id = int.Parse(item.id);
                            string quan = item.quan;
                            CartItem cartItem = new CartItem();
                            cartItem.Product = new ProductDAO().DetailProduct(id);
                            cartItem.Quantity = int.Parse(quan);
                            cartItems.Add(cartItem);
                        }
                        Session[Common.CommonConstants.CART_SESSION] = cartItems;
                    }


                    return RedirectToAction("Index", "Home");
                }
                else if (result == 0)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại.");
                }
                else if (result == -1)
                {
                    ModelState.AddModelError("", "Tài khoản bị khóa.");
                }
                else if (result == -2)
                {
                    ModelState.AddModelError("", "Mật khẩu không đúng.");
                }
                else
                {
                    ModelState.AddModelError("", "Đăng nhập không thành công.");
                }
            }
            return View("Index");

        }

        [HttpPost]
        public ActionResult Resgister(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                bool kt = new CustomerDAO().Register(model.Name, model.Phone, model.Password);
                if (kt)
                {
                    var user = new LoginModel();
                    user.Phone = model.Phone;
                    user.Password = model.Password;
                    ViewBag.Status = "1";
                    return View("Index");
                }
                else
                {
                    ViewBag.Status = "0";
                    ModelState.AddModelError("", "Số điện thoại này đã được dăng ký.");
                }
            }
            return View("Index");
        }
        public ActionResult ClearSession()
        {
            Session.Remove(Common.CommonConstants.CUSTOMER_SESSION);
            Session.Remove(Common.CommonConstants.CART_SESSION);
            return RedirectToAction("Index", "Home");
        }
    }
}