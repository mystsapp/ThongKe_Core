using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.DTOs.Tourleob;
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
            //xlSheet.Column(4).Width = 15;//Chi nhánh - xuất vé
            xlSheet.Column(4).Width = 25;//Bắt đầu
            xlSheet.Column(5).Width = 20;//Kết thúc
            xlSheet.Column(6).Width = 10;//Số khách
            xlSheet.Column(7).Width = 20;//Doanh số

            xlSheet.Cells[2, 1].Value = $"BÁO CÁO DOANH THU THEO KHÁCH VÉ TOUR quầy: {quay}";
            xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
            xlSheet.Cells[2, 1, 2, 7].Merge = true;
            Tool.setCenterAligment(2, 1, 2, 7, xlSheet);
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
            xlSheet.Cells[3, 1, 3, 7].Merge = true;
            xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
            Tool.setCenterAligment(3, 1, 3, 7, xlSheet);

            // Tạo header
            xlSheet.Cells[5, 1].Value = "Stt";
            xlSheet.Cells[5, 2].Value = "Sgtcode";
            xlSheet.Cells[5, 3].Value = "Tuyến tq";
            //xlSheet.Cells[5, 4].Value = "Chi nhánh"; // xuất vé
            xlSheet.Cells[5, 4].Value = "Bắt đầu";
            xlSheet.Cells[5, 5].Value = "Kết thúc";
            xlSheet.Cells[5, 6].Value = "Số khách";
            xlSheet.Cells[5, 7].Value = "Doanh số";

            xlSheet.Cells[5, 1, 5, 7].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));

            // do du lieu tu table
            int dong = 6;
            var d = (from p in list.VetourDTOs
                     group p by new { p.Sgtcode } into g
                     select new VetourDTOgroupSgtcode
                     {
                         Sgtcode = g.Key.Sgtcode,
                         TuyenTq = g.First().TuyenTq,
                         Batdau = g.First().Batdau,
                         Ketthuc = g.First().Ketthuc,
                         VetourDTOs = g.ToList()
                     }).ToList();

            //du lieu
            int iRowIndex = 6;
            int idem = 1;

            if (d != null && d.Count > 0)
            {
                foreach (var vm in d)
                {

                    xlSheet.Cells[iRowIndex, 1].Value = idem;
                    xlSheet.Cells[iRowIndex, 3].Value = vm.TuyenTq;
                    xlSheet.Cells[iRowIndex, 4].Value = vm.Batdau;
                    xlSheet.Cells[iRowIndex, 5].Value = vm.Ketthuc;
                    xlSheet.Cells[iRowIndex + 1, 2].Value = vm.Sgtcode;
                    xlSheet.Cells[iRowIndex + 1, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlSheet.Cells[iRowIndex, 6].Value = vm.VetourDTOs.Sum(x => x.Chiemcho);
                    xlSheet.Cells[iRowIndex, 7].Value = vm.VetourDTOs.Sum(x => x.DoanhThu); // item.VetourDTOs.Sum(x => x.DoanhThu);
                    xlSheet.Cells[iRowIndex, 1, iRowIndex, 8].Style.Font.SetFromFont(new Font("Times New Roman", 11, FontStyle.Bold));
                    iRowIndex++;
                    dong++;
                    var d1 = (from p in vm.VetourDTOs
                              group p by new { p.TuyenTq, p.Chinhanh } into g
                              select new VetourDTOgroupTuyentqChinhanh
                              {
                                  TuyenTq = g.Key.TuyenTq,
                                  Chinhanh = g.Key.Chinhanh,
                                  VetourDTOs = g.ToList()
                              }).ToList();
                    int i = iRowIndex;
                    
                    foreach (var item in d1)
                    {
                        //xlSheet.Cells[iRowIndex, 2].Value = item.Sgtcode;
                        
                        //xlSheet.Cells[iRowIndex, 6].Value = item.VetourDTOs.Sum(x => x.Chiemcho);
                        //xlSheet.Cells[iRowIndex, 7].Value = item.VetourDTOs.Sum(x => x.DoanhThu); // item.VetourDTOs.Sum(x => x.DoanhThu);
                        //xlSheet.Cells[iRowIndex, 1, iRowIndex, 8].Style.Font.SetFromFont(new Font("Times New Roman", 11, FontStyle.Bold));
                        //iRowIndex++;
                        //dong++;

                        xlSheet.Cells[iRowIndex, 3].Value = item.Chinhanh; // chinhanh xuat ve
                        xlSheet.Cells[iRowIndex, 6].Value = item.VetourDTOs.Sum(x => x.Chiemcho); // theo tuyentq, chinhanh xuat ve
                        xlSheet.Cells[iRowIndex, 7].Value = item.VetourDTOs.Sum(x => x.DoanhThu); // theo tuyentq, chinhanh xuat ve
                        //foreach (var item1 in item.VetourDTOs)
                        //{
                        //    xlSheet.Cells[iRowIndex, 2].Value = item.Sgtcode;
                        //    xlSheet.Cells[iRowIndex, 3].Value = item1.Chinhanh; // chinhanh xuat ve
                        //    xlSheet.Cells[iRowIndex, 6].Value = item1.Chiemcho; // theo tuyentq, chinhanh xuat ve
                        //    xlSheet.Cells[iRowIndex, 7].Value = item1.DoanhThu; // theo tuyentq, chinhanh xuat ve
                        //    iRowIndex += 1;
                        //    dong++;
                        //}
                        iRowIndex += 1;
                        dong++;
                    }
                    var count = (d1.Count == 1 ? i : (d1.Count - 1 + i));
                    //try
                    //{
                        xlSheet.Cells[$"B{i}:B{count}"].Merge = true;
                    //}
                    //catch (Exception ex)
                    //{

                    //    throw;
                    //}
                    
                    Tool.DateFormat(6, 4, dong, 5, xlSheet);
                    xlSheet.Cells[iRowIndex, 6].Value = list.VetourDTOs.Sum(x => x.Chiemcho); // theo tuyentq, chinhanh xuat ve
                    xlSheet.Cells[iRowIndex, 7].Value = list.VetourDTOs.Sum(x => x.DoanhThu); // theo tuyentq, chinhanh xuat ve
                    xlSheet.Cells[iRowIndex, 6, iRowIndex, 7].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));
                    idem += 1;
                }
            }
            else
            {
                SetAlert("No sale.", "warning");
                return RedirectToAction(nameof(GetKhachvetours));
            }

            //dong++;
            //// Merger cot 4,5 ghi tổng tiền
            //setRightAligment(dong, 3, dong, 3, xlSheet);
            //xlSheet.Cells[dong, 1, dong, 2].Merge = true;
            //xlSheet.Cells[dong, 1].Value = "Tổng tiền: ";
            //// Sum tổng tiền
            //xlSheet.Cells[dong, 6].Formula = "SUM(G6:G" + (6 + d.Count() - 1) + ")";
            //xlSheet.Cells[dong, 7].Formula = "SUM(H6:H" + (6 + d.Count() - 1) + ")";

            Tool.setBorder(5, 1, dong, 7, xlSheet);

            //xlSheet.Cells[dong, 7, dong, 7].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));
            Tool.setCenterAligment(6, 1, iRowIndex - 1, 1, xlSheet);
            Tool.NumberFormat(6, 6, dong, 7, xlSheet);

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
