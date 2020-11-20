using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ThongKe.Data.Repository;
using ThongKe.Models;

namespace ThongKe.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult LoadDataThongKeSoKhachOB()
        {
            string khoi = "OB";

            var listOB = _unitOfWork.thongKeRepository.ThongKeSoKhachOB(khoi);//doanhthuKhachLeHeThongEntities
            foreach(var item in listOB)
            {
                item.SoKhachHT = item.SoKhachHT == null ? 0 : item.SoKhachHT;
                item.SoKhachTT = item.SoKhachTT == null ? 0 : item.SoKhachTT;
            }
            return Json(new
            {
                data = listOB,
                status = true
            });
        }

        public JsonResult LoadDataThongKeSoKhachND()
        {
            string khoi = "ND";

            var listOB = _unitOfWork.thongKeRepository.ThongKeSoKhachOB(khoi);//doanhthuKhachLeHeThongEntities
            foreach (var item in listOB)
            {
                item.SoKhachHT = item.SoKhachHT == null ? 0 : item.SoKhachHT;
                item.SoKhachTT = item.SoKhachTT == null ? 0 : item.SoKhachTT;
            }
            return Json(new
            {
                data = listOB,
                status = true
            });
        }
        
        public JsonResult LoadDataThongKeDoanhThuOB()
        {
            string khoi = "OB";

            var listOB = _unitOfWork.thongKeRepository.ThongKeDoanhThuOB(khoi);//doanhthuKhachLeHeThongEntities
            foreach (var item in listOB)
            {
                item.DoanhThuHT = item.DoanhThuHT == null ? 0 : item.DoanhThuHT;
                item.DoanhThuTT = item.DoanhThuTT == null ? 0 : item.DoanhThuTT;
            }
            return Json(new
            {
                data = listOB,
                status = true
            });
        }

        public JsonResult LoadDataThongKeDoanhThuND()
        {
            string khoi = "ND";

            var listOB = _unitOfWork.thongKeRepository.ThongKeDoanhThuOB(khoi);//doanhthuKhachLeHeThongEntities
            foreach (var item in listOB)
            {
                item.DoanhThuHT = item.DoanhThuHT == null ? 0 : item.DoanhThuHT;
                item.DoanhThuTT = item.DoanhThuTT == null ? 0 : item.DoanhThuTT;
            }
            return Json(new
            {
                data = listOB,
                status = true
            });
        }
    }
}
