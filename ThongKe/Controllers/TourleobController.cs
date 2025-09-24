using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Models;
using ThongKe.Data.Repository;
using ThongKe.Helps;
using ThongKe.Models;
using ThongKe.Services;

namespace ThongKe.Controllers
{
    public class TourleobController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaoCaoService _baoCaoService;

        public TourleobController(IUnitOfWork unitOfWork, IBaoCaoService baoCaoService)
        {
            _unitOfWork = unitOfWork;
            _baoCaoService = baoCaoService;
        }
        public async Task<IActionResult> GetKhachvetours(string tungay = null, string denngay = null, string chinhanh = null, string quay = null)
        {
            Data.DTOs.Tourleob.KhachVaVetourDTO list = await ListKhachVetours(tungay, denngay, chinhanh, quay);
            return View(list);
        }

        private async Task<Data.DTOs.Tourleob.KhachVaVetourDTO> ListKhachVetours(string tungay, string denngay, string chinhanh, string quay)
        {
            var user = HttpContext.Session.Get<Users>("loginUser");

            ViewBag.TuNgay = tungay;
            ViewBag.DenNgay = denngay;
            ViewBag.chiNhanh = chinhanh;
            List<string> chinhanhs = new List<string>();

            if (user.RoleId != 8) // 8: Admins
            {
                if (user.RoleId == 9) // 9: Users
                {
                    chinhanhs.Add(user.Chinhanh);
                }
                else // admin khuvuc
                {
                    var role1 = await _baoCaoService.GetRoleById(user.RoleId);
                    var listMaCN = role1.ChiNhanhQL.Split(',').ToList();

                    chinhanhs.AddRange(listMaCN);
                }
            }
            else // admin tong
            {
                chinhanhs.AddRange(_unitOfWork.dmChiNhanhRepository.GetAll().Select(x => x.Macn).Distinct().ToList());
            }
            ViewBag.ChiNhanhs = chinhanhs.OrderBy(x => x);
            ViewBag.dmdailys = _unitOfWork.tourleobRepository.GetDmdailys(new List<string>());
            ViewBag.daily = quay;
            DateTime searchFromDate, searchToDate;
            if (tungay == null || denngay == null)
            {
                searchFromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                searchToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                ViewBag.TuNgay = searchFromDate.ToString("dd/MM/yyyy");
                ViewBag.DenNgay = searchToDate.ToString("dd/MM/yyyy");
            }
            else
            {
                searchFromDate = DateTime.ParseExact(tungay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                searchToDate = DateTime.ParseExact(denngay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            ViewBag.chiNhanh = chinhanh ?? user.Chinhanh;
            var list = _unitOfWork.tourleobRepository.GetKhachvetours(searchFromDate, searchToDate, chinhanh ?? user.Chinhanh, quay);
            return list;
        }
        [HttpPost]
        public async Task<IActionResult> GetKhachvetoursExcel(string tungay = null, string denngay = null, string chinhanh = null, string quay = null)
        {

            Data.DTOs.Tourleob.KhachVaVetourDTO list = await ListKhachVetours(tungay, denngay, chinhanh, quay);
            string fromTo = "";
            ExcelPackage ExcelApp = new ExcelPackage();
            ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
            // Định dạng chiều dài cho cột
            xlSheet.Column(1).Width = 10;//STT
            xlSheet.Column(2).Width = 20;//Sgtcode
            xlSheet.Column(3).Width = 30;//Tuyến tq
            xlSheet.Column(4).Width = 15;//Ngày xuất vé
            xlSheet.Column(5).Width = 25;//Người xuất vé
            xlSheet.Column(6).Width = 20;//Đại lý xuất vé
            xlSheet.Column(7).Width = 10;//Số khách
            xlSheet.Column(8).Width = 20;//Doanh số

            xlSheet.Cells[2, 1].Value = $"BÁO CÁO DOANH THU THEO KHÁCH VÉ TOUR quầy: {quay}";
            xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
            xlSheet.Cells[2, 1, 2, 8].Merge = true;
            Tool.setCenterAligment(2, 1, 2, 8, xlSheet);
            // dinh dang tu ngay den ngay
            if (tungay == denngay)
            {
                fromTo = "Ngày: " + tungay;
            }
            else
            {
                fromTo = "Từ ngày: " + tungay + " đến ngày: " + denngay;
            }
            xlSheet.Cells[3, 1].Value = fromTo;
            xlSheet.Cells[3, 1, 3, 8].Merge = true;
            xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
            Tool.setCenterAligment(3, 1, 3, 8, xlSheet);

            // Tạo header
            xlSheet.Cells[5, 1].Value = "Stt";
            xlSheet.Cells[5, 2].Value = "Sgtcode";
            xlSheet.Cells[5, 3].Value = "Tuyến tq";
            xlSheet.Cells[5, 4].Value = "Ngày xuất vé";
            xlSheet.Cells[5, 5].Value = "Người xuất vé";
            xlSheet.Cells[5, 6].Value = "Đại lý xuất vé";
            xlSheet.Cells[5, 7].Value = "Số khách";
            xlSheet.Cells[5, 8].Value = "Doanh số";

            xlSheet.Cells[5, 1, 5, 8].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));

            // do du lieu tu table
            int dong = 5;

            var d = list.VetourDTOs;

            //du lieu
            int iRowIndex = 6;
            int idem = 1;

            if (d != null && d.Count > 0)
            {
                foreach (var vm in d)
                {
                    xlSheet.Cells[iRowIndex, 1].Value = idem;
                    xlSheet.Cells[iRowIndex, 2].Value = vm.Sgtcode;
                    xlSheet.Cells[iRowIndex, 3].Value = vm.TuyenTq;
                    xlSheet.Cells[iRowIndex, 4].Value = vm.Ngayxuatve.Value.ToString("dd/MM/yyyy");
                    xlSheet.Cells[iRowIndex, 5].Value = vm.Nguoixuatve;
                    xlSheet.Cells[iRowIndex, 6].Value = vm.Dailyxuatve;
                    xlSheet.Cells[iRowIndex, 7].Value = list.KhachvetourDTOs.Where(x => x.Idvetour == vm.Id).Count();
                    xlSheet.Cells[iRowIndex, 8].Value = vm.Giatour + vm.Dichvukhac - vm.Giamgia;
                    iRowIndex += 1;
                    idem += 1;
                    dong++;
                }
            }
            else
            {
                SetAlert("No sale.", "warning");
                return RedirectToAction(nameof(GetKhachvetours));
            }
            xlSheet.Cells[6, 1, dong, 8].Style.Font.SetFromFont(new Font("Times New Roman", 11, FontStyle.Regular));
            dong++;
            //// Merger cot 4,5 ghi tổng tiền
            //setRightAligment(dong, 3, dong, 3, xlSheet);
            //xlSheet.Cells[dong, 1, dong, 2].Merge = true;
            //xlSheet.Cells[dong, 1].Value = "Tổng tiền: ";
            // Sum tổng tiền
            xlSheet.Cells[dong, 7].Formula = "SUM(G6:G" + (6 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 8].Formula = "SUM(H6:H" + (6 + d.Count() - 1) + ")";

            Tool.setBorder(5, 1, dong, 8, xlSheet);

            xlSheet.Cells[dong, 7, dong, 8].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));
            Tool.setCenterAligment(6, 1, 6 + d.Count(), 1, xlSheet);
            Tool.NumberFormat(6, 7, dong, 8, xlSheet);

            //xlSheet.View.FreezePanes(6, 20);

            //end du lieu

            byte[] fileContents;
            fileContents = ExcelApp.GetAsByteArray();

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }
            string sFilename = "DoanhThuKhachvetour_" + quay + "_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: sFilename
            );
        }

    }
}
