using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ThongKe.Data.Models;
using ThongKe.Data.Repository;
using ThongKe.Helps;
using ThongKe.Models;

namespace ThongKe.Controllers
{
    public class UsersController : BaseController
    {

        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public UserViewModel UserVM { get; set; }
        MaHoaSHA1 mh = new MaHoaSHA1();
        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            UserVM = new UserViewModel()
            {
                Users = _unitOfWork.userRepository.GetAll(),
                Dmchinhanhs = _unitOfWork.dmChiNhanhRepository.GetAll(),// _unitOfWork.chiNhanhRepository.GetAll(),
                Dmdailies = _unitOfWork.dMDaiLyRepository.GetAll(),
                KhoiViewModels = khoiViewModels(),
                RoleViewModels = RoleViewModels(),
                User = new Users(),
                OldPass = ""
            };
        }

        public IActionResult Index()
        {
            var user = HttpContext.Session.Get<Users>("loginUser");
            if (user.Nhom != "Admins" && user.Nhom != "KDO")
            {
                return View("AccessDenied");
            }
            var a = UserVM.Users.Count();
            return View(UserVM);
        }

        // Get Create method
        public async Task<IActionResult> Create()
        {
            var user = HttpContext.Session.Get<Users>("loginUser");
            if (user.Nhom != "Admins" && user.Nhom != "KDO")
            {
                return View("AccessDenied");
            }
            UserVM.PhongBans = PhongBans();
            UserVM.Dmdailies = _unitOfWork.dMDaiLyRepository.GetAll();
            UserVM.Roles = await _unitOfWork.roleRepository.GetRoles();

            return View(UserVM);
        }

        // Post: Create Method
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            var user = HttpContext.Session.Get<Users>("loginUser");
            if (!ModelState.IsValid)
            {
                return View(UserVM);
                
            }

            UserVM.User.Ngaytao = DateTime.Now;
            UserVM.User.Nguoitao = user.Nguoitao;
            UserVM.User.Password = mh.EncodeSHA1(UserVM.User.Password);

            _unitOfWork.userRepository.Create(UserVM.User);
            await _unitOfWork.Complete();
            SetAlert("Thêm User thành cong.", "success");
            return RedirectToAction(nameof(Index));
        }

        // Get Edit method
        public async Task<IActionResult> Edit(string id)
        {
            var user = HttpContext.Session.Get<Users>("loginUser");
            if (user.Nhom != "Admins" && user.Nhom != "KDO")
            {
                return View("AccessDenied");
            }

            UserVM.User = _unitOfWork.userRepository.Find(x => x.Username.Equals(id)).FirstOrDefault();
            if (UserVM.User != null)
            {
                UserVM.PhongBans = PhongBans();
                UserVM.Dmdailies = _unitOfWork.dMDaiLyRepository.GetAll();
                UserVM.Roles = await _unitOfWork.roleRepository.GetRoles();
                return View(UserVM);
            }
            else
            {
                ViewBag.ErrorMessage = "User is not found.";
                return View(nameof(NotFound));
            }
        }

        // Post: Create Method
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(string id)
        {
            var user = HttpContext.Session.Get<Users>("loginUser");

            if (!UserVM.User.Username.Equals(id))
            {
                ViewBag.ErrorMessage = "User is not found.";
                return View(nameof(NotFound));
            }
            if (!ModelState.IsValid)
            {
                UserVM = new UserViewModel()
                {
                    Users = _unitOfWork.userRepository.GetAll(),
                    Dmchinhanhs = _unitOfWork.dmChiNhanhRepository.GetAll(),
                    Dmdailies = _unitOfWork.dMDaiLyRepository.GetAll(),
                    KhoiViewModels = khoiViewModels(),
                    RoleViewModels = RoleViewModels(),
                    User = _unitOfWork.userRepository.Find(x => x.Username.Equals(id)).FirstOrDefault()
                };
                return View(UserVM);
            }

            if (UserVM.PassToEdit != null) //password field is required
            {
                UserVM.User.Password = mh.EncodeSHA1(UserVM.PassToEdit);
                UserVM.User.Ngaydoimk = DateTime.Now;
            }
            else
            {
                UserVM.User.Password = UserVM.OldPass;
            }
            UserVM.User.Ngaycapnhat = DateTime.Now;
            UserVM.User.Nguoicapnhat = user.Nguoicapnhat;
            _unitOfWork.userRepository.Update(UserVM.User);
            await _unitOfWork.Complete();
            SetAlert("Cập nhật User thành cong.", "success");
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(string id)
        {
            var user = HttpContext.Session.Get<Users>("loginUser");
            if (user.Nhom != "Admins" && user.Nhom != "KDO")
            {
                return View("AccessDenied");
            }

            UserVM.User = _unitOfWork.userRepository.Find(x => x.Username.Equals(id)).FirstOrDefault();
            if (UserVM.User != null)
            {
                return View(UserVM);
            }
            else
            {
                ViewBag.ErrorMessage = "User is not found.";
                return View(nameof(NotFound));
            }
        }

        // Get Delete method
        public IActionResult Delete(string id)
        {
            var user = HttpContext.Session.Get<Users>("loginUser");
            if (user.Nhom != "Admins" && user.Nhom != "KDO")
            {
                return View("AccessDenied");
            }

            UserVM.User = _unitOfWork.userRepository.Find(x => x.Username.Equals(id)).FirstOrDefault();
            if (UserVM.User != null)
            {
                return View(UserVM);
            }
            else
            {
                ViewBag.ErrorMessage = "User is not found.";
                return View(nameof(NotFound));
            }
        }

        // Post: Delete Method
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            var user = _unitOfWork.userRepository.Find(x => x.Username.Equals(id)).FirstOrDefault();
            if (user == null)
            {
                return View(nameof(NotFound));
            }
            _unitOfWork.userRepository.Delete(user);
            await _unitOfWork.Complete();
            SetAlert("Xóa thành cong.", "success");
            return RedirectToAction(nameof(Index));
        }

        //////////////////////////////////////////////////////////////////////////////////

        public List<KhoiViewModel> khoiViewModels()
        {
            return new List<KhoiViewModel>()
            {
                new KhoiViewModel() { Id = 1, Name = "OB" },
                new KhoiViewModel() { Id = 2, Name = "ND" },
                new KhoiViewModel() { Id = 3, Name = "IB" }
            };
        }

        public List<RoleViewModel> RoleViewModels()
        {
            return new List<RoleViewModel>()
            {
                new RoleViewModel() { Id = "Users", Name = "Users" },
                new RoleViewModel() { Id = "Admins", Name = "Admins" },
                new RoleViewModel() { Id = "TNB", Name = "Tây nam bộ" },
                new RoleViewModel() { Id = "DNB", Name = "Dông nam bộ" },
                new RoleViewModel() { Id = "MT", Name = "Miền trung" },
                new RoleViewModel() { Id = "MB", Name = "Miền bắ" },
                new RoleViewModel() { Id = "KDO", Name = "KDOnline" },
            };
        }

        private List<Data.Models_QLTour.Phongban> PhongBans()
        {
            //return _unitOfWork.phongBanRepository.GetAll()
            //                                     .Where(x => !string.IsNullOrEmpty(x.Macode))
            //                                     .ToList();
            var phongbans = _unitOfWork.phongBanRepository.GetAll().Where(x => !string.IsNullOrEmpty(x.Macode)).ToList();
            var phongban = _unitOfWork.phongBanRepository.Find(x => x.Maphong == "KT").FirstOrDefault();
            phongbans.Add(phongban);
            return phongbans;
        }

    }
}