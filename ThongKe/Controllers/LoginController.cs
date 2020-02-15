using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThongKe.Data.Models;
using ThongKe.Data.Repository;
using ThongKe.Helps;
using ThongKe.Models;

namespace ThongKe.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public LoginViewModel LoginVM { get; set; }
        MaHoaSHA1 sha1 = new MaHoaSHA1();
        public LoginController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, ActionName("Index")]
        public IActionResult IndexPost()
        {
            if (ModelState.IsValid)
            {
                var result = _unitOfWork.userRepository.Login(LoginVM.Username, "010");
                if (result == null)
                {
                    ModelState.AddModelError("", "Tài khoản này không tồn tại");
                }
                else
                {
                    if (!result.Trangthai)
                    {
                        ModelState.AddModelError("", "Tài khoản này đã bị khóa");
                    }
                    string modelPass = sha1.EncodeSHA1(LoginVM.Password);
                    if (result.Password != modelPass)
                    {
                        ModelState.AddModelError("", "Mật khẩu không đúng");
                    }
                    if (result.Password == modelPass)
                    {
                        var user = _unitOfWork.userRepository.GetById(LoginVM.Username);
                        HttpContext.Session.Set("loginUser", user);
                        
                        HttpContext.Session.SetString("username", user.Username);
                        //HttpContext.Session.SetString("password", model.Password);
                        //HttpContext.Session.SetString("hoten", result.Hoten);
                        //HttpContext.Session.SetString("phong", result.Maphong);
                        //HttpContext.Session.SetString("chinhanh", result.Macn);
                        //HttpContext.Session.SetString("dienthoai", String.IsNullOrEmpty(result.Dienthoai) ? "" : result.Dienthoai);
                        //HttpContext.Session.SetString("macode", result.Macode);
                        //HttpContext.Session.SetString("roleId", result.Macode);

                        //DateTime ngaydoimk = Convert.ToDateTime(result.Ngaydoimk);
                        //int kq = (DateTime.Now.Month - ngaydoimk.Month) + 12 * (DateTime.Now.Year - ngaydoimk.Year);
                        //if (kq >= 3)
                        //{
                        //    return View("changepass");
                        //}
                        //else if (result.Doimk)
                        //{
                        //    return View("changepass");
                        //}

                        if (result.Doimk)
                        {
                            return View("changepass");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }

                    }
                }
            }
            return View();
        }

        public IActionResult changepass(string strUrl)
        {
            var user = HttpContext.Session.Get<Users>("loginUser");
            ChangePassViewModel changPassVM = new ChangePassViewModel()
            {
                Username = user.Username,
                Password = user.Password,
                strUrl = strUrl
            };
            return View(changPassVM);
        }
        [HttpPost]
        public IActionResult changepass(ChangePassViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = HttpContext.Session.Get<Users>("loginUser");
                string oldPass = user.Password;
                string modelPass = sha1.EncodeSHA1(model.Password);
                if (oldPass != modelPass)
                {
                    ModelState.AddModelError("", "Mật khẩu cũ không đúng");
                }
                //else if (model.Newpassword != model.Confirmpassword)
                //{
                //    ModelState.AddModelError("", "Mật khẩu nhập lại không đúng.");
                //}
                else
                {
                    int result = _unitOfWork.userRepository.Changepass(model.Username, sha1.EncodeSHA1(model.NewPassword));
                    if (result > 0)
                    {
                        return LocalRedirect(model.strUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không thể đổi mật khẩu.");
                    }
                }

            }
            return View();
        }

        public IActionResult logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}