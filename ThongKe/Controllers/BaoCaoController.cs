using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ThongKe.Data.Models;
using ThongKe.Data.Repository;
using ThongKe.Helps;
using ThongKe.Models;

namespace ThongKe.Controllers
{
    public class BaoCaoController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BaoCaoController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        /////////////////////////////////////// Sale theo quay ///////////////////////////////////////////////////////////////////
        public IActionResult SaleTheoQuay(string tungay = null, string denngay = null, string chiNhanh = null, string khoi = null)
        {
            var dtSaleQuayVM = new DoanhthuSaleQuayViewModel();

            var chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

            for (int i = 0; i < chiNhanhs.Count(); i++)
            {
                var cnToreturn = new ChiNhanhToReturnViewModel()
                {
                    Stt = i,
                    Name = chiNhanhs[i]
                };

                dtSaleQuayVM.chiNhanhToReturnViewModels.Add(cnToreturn);
            }

            dtSaleQuayVM.khoiViewModels = khoiViewModels();

            try
            {
                ViewBag.searchFromDate = tungay;
                ViewBag.searchToDate = denngay;
                chiNhanh = chiNhanh ?? "";
                ViewBag.chiNhanh = chiNhanh;
                ViewBag.khoi = khoi;

                if (tungay == null || denngay == null)
                {
                    return View("SaleTheoQuay", dtSaleQuayVM);
                }

                var list = _unitOfWork.thongKeRepository.listSaleTheoQuay(tungay, denngay, chiNhanh, khoi);
                dtSaleQuayVM.DoanhthuSaleQuays = list;
                return View(dtSaleQuayVM);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return View("SaleTheoQuay", dtSaleQuayVM);
            }
        }

        [HttpPost]
        public IActionResult SaleTheoQuayPost(string tungay, string denngay, string chinhanh, string khoi)//(string tungay,string denngay, string daily)
        {
            if (tungay == null || denngay == null)
            {
                return RedirectToAction("SaleTheoQuay");
            }
            try
            {
                DateTime.Parse(tungay);
                DateTime.Parse(denngay);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return RedirectToAction("SaleTheoQuay");
            }
            //chinhanh = String.IsNullOrEmpty(chinhanh) ? Session["chinhanh"].ToString() : chinhanh;
            //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;

            chinhanh = chinhanh ?? "";
            string fromTo = "";
            ExcelPackage ExcelApp = new ExcelPackage();
            ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
            // Định dạng chiều dài cho cột
            xlSheet.Column(1).Width = 10;//stt
            xlSheet.Column(2).Width = 50;// sales
            xlSheet.Column(3).Width = 10;//stt
            xlSheet.Column(4).Width = 30;// doanh so
            xlSheet.Column(5).Width = 30;// doanh thu sale

            xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH THU THEO NGÀY BÁN QUẦY " + khoi + " " + chinhanh;
            xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
            xlSheet.Cells[2, 1, 2, 5].Merge = true;
            setCenterAligment(2, 1, 2, 5, xlSheet);
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
            xlSheet.Cells[3, 1, 3, 5].Merge = true;
            xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
            setCenterAligment(3, 1, 3, 5, xlSheet);

            // Tạo header
            xlSheet.Cells[5, 1].Value = "STT";
            xlSheet.Cells[5, 2].Value = "Nhân viên ";
            xlSheet.Cells[5, 3].Value = "Code chinhanh ";

            xlSheet.Cells[5, 4].Value = "Tổng tiền";
            xlSheet.Cells[5, 5].Value = "Doanh số";

            xlSheet.Cells[5, 1, 5, 5].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));

            // do du lieu tu table
            int dong = 5;

            var d = _unitOfWork.thongKeRepository.listSaleTheoQuay(tungay, denngay, chinhanh, khoi);// Session["daily"].ToString(), Session["khoi"].ToString());

            //du lieu
            int iRowIndex = 6;
            int idem = 1;

            if (d != null)
            {
                foreach (var vm in d)
                {
                    xlSheet.Cells[iRowIndex, 1].Value = idem;
                    TrSetCellBorder(xlSheet, iRowIndex, 1, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 2].Value = vm.Nguoixuatve;
                    TrSetCellBorder(xlSheet, iRowIndex, 2, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 3].Value = vm.Chinhanh;
                    TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 4].Value = vm.Doanhso;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 5].Value = vm.Thucthu;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    iRowIndex += 1;
                    idem += 1;
                    dong++;
                }
            }
            else
            {
                SetAlert("No sale.", "warning");
                return RedirectToAction(nameof(SaleTheoQuay));
            }

            dong++;
            //// Merger cot 4,5 ghi tổng tiền
            //setRightAligment(dong, 3, dong, 3, xlSheet);
            //xlSheet.Cells[dong, 1, dong, 2].Merge = true;
            //xlSheet.Cells[dong, 1].Value = "Tổng tiền: ";
            // Sum tổng tiền
            xlSheet.Cells[dong, 4].Formula = "SUM(D6:D" + (6 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 5].Formula = "SUM(E6:E" + (6 + d.Count() - 1) + ")";

            setBorder(5, 1, 5 + d.Count(), 5, xlSheet);
            setFontBold(5, 1, 5, 5, 11, xlSheet);
            setFontSize(6, 1, 6 + d.Count(), 5, 11, xlSheet);
            // canh giua cot stt
            setCenterAligment(6, 1, 6 + d.Count(), 1, xlSheet);
            // canh giua code chinhanh
            setCenterAligment(6, 3, 6 + d.Count(), 3, xlSheet);
            NumberFormat(6, 4, 6 + d.Count(), 5, xlSheet);
            // định dạng số cot tong cong
            NumberFormat(dong, 4, dong, 5, xlSheet);
            setBorder(dong, 4, dong, 5, xlSheet);
            setFontBold(dong, 4, dong, 5, 12, xlSheet);

            //xlSheet.View.FreezePanes(6, 20);

            //end du lieu

            byte[] fileContents;
            fileContents = ExcelApp.GetAsByteArray();

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }
            string sFilename = "DoanhThuSale_" + khoi + " " + chinhanh + "_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: sFilename
            );
        }

        public IActionResult SaleTheoQuayChiTietToExcel(string tungay, string denngay, string nhanvien, string chinhanh, string khoi)
        {
            try
            {
                nhanvien = convertToUnSign3(nhanvien);
                //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;
                string fromTo = "";
                ExcelPackage ExcelApp = new ExcelPackage();
                ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("DoanhthuSale");
                // Định dạng chiều dài cho cột
                xlSheet.Column(1).Width = 10;//stt
                xlSheet.Column(2).Width = 10;// chi nhanh
                xlSheet.Column(3).Width = 25;// code
                xlSheet.Column(4).Width = 25;// tuyen tham quan
                xlSheet.Column(5).Width = 40;// ten khach
                xlSheet.Column(6).Width = 10;// so khach
                xlSheet.Column(7).Width = 20;//doanhthu
                xlSheet.Column(8).Width = 20;//thuc thu
                xlSheet.Column(9).Width = 35;//sales

                xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH THU THEO NGÀY BÁN SALE " + nhanvien;
                xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
                xlSheet.Cells[2, 1, 2, 9].Merge = true;
                if (tungay == denngay)
                {
                    fromTo = "Ngày: " + tungay;
                }
                else
                {
                    fromTo = "Từ ngày: " + tungay + " đến ngày: " + denngay;
                }
                xlSheet.Cells[3, 1].Value = fromTo;
                xlSheet.Cells[3, 1, 3, 9].Merge = true;
                xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
                setCenterAligment(2, 1, 3, 9, xlSheet);

                // Tạo header
                xlSheet.Cells[5, 1].Value = "STT";
                xlSheet.Cells[5, 2].Value = "Code CN";
                xlSheet.Cells[5, 3].Value = "Code Đoàn";
                xlSheet.Cells[5, 4].Value = "Tuyến tham quan";
                xlSheet.Cells[5, 5].Value = "Tên khách";
                xlSheet.Cells[5, 6].Value = "Số khách";
                xlSheet.Cells[5, 7].Value = "Tổng tiền";
                xlSheet.Cells[5, 8].Value = "Doanh số";
                xlSheet.Cells[5, 9].Value = "Sales";

                xlSheet.Cells[5, 1, 5, 9].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));

                int dong = 5;
                var d = _unitOfWork.thongKeRepository.SaleTheoQuayChiTietToExcel(tungay, denngay, nhanvien, chinhanh, khoi);// Session["fullName"].ToString());

                //du lieu
                int iRowIndex = 6;
                int idem = 1;

                if (d != null)
                {
                    foreach (var vm in d)
                    {
                        xlSheet.Cells[iRowIndex, 1].Value = idem;
                        TrSetCellBorder(xlSheet, iRowIndex, 1, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 2].Value = vm.Chinhanh;
                        TrSetCellBorder(xlSheet, iRowIndex, 2, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 3].Value = vm.Sgtcode;
                        TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 4].Value = vm.Tuyentq;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 5].Value = vm.Tenkhach;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 6].Value = vm.Chiemcho;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 7].Value = vm.Doanhthu;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 8].Value = vm.Thucthu;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 9].Value = vm.Nguoixuatve;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        iRowIndex += 1;
                        idem += 1;
                        dong++;
                    }
                }
                else
                {
                    SetAlert("No sale.", "warning");
                    return RedirectToAction(nameof(SaleTheoQuay));
                }

                dong++;
                // Merger cot 4,5 ghi tổng tiền
                //setRightAligment(dong, 4, dong, 5, xlSheet);
                //xlSheet.Cells[dong, 4, dong, 5].Merge = true;
                //xlSheet.Cells[dong, 4].Value = "Tổng tiền: ";

                //// Sum tổng tiền
                xlSheet.Cells[dong, 8].Formula = "SUM(H6:H" + (6 + d.Count() - 1) + ")";
                //xlSheet.Cells[dong, 7].Formula = "SUM(G6:G" + (6 + d.Count() - 1) + ")";
                //// định dạng số
                NumberFormat(dong, 6, dong, 6, xlSheet);
                setBorder(5, 1, 5 + d.Count(), 9, xlSheet);
                setFontBold(5, 1, 5, 9, 12, xlSheet);
                setFontSize(6, 1, 6 + d.Count(), 9, 12, xlSheet);
                NumberFormat(6, 7, 6 + d.Count(), 8, xlSheet);
                setCenterAligment(6, 1, 6 + d.Count(), 3, xlSheet);
                setCenterAligment(6, 6, 6 + d.Count(), 6, xlSheet);
                xlSheet.View.FreezePanes(6, 20);

                //end du lieu

                byte[] fileContents;
                fileContents = ExcelApp.GetAsByteArray();

                if (fileContents == null || fileContents.Length == 0)
                {
                    return NotFound();
                }
                string sFilename = "DoanhThuSale_" + khoi + "_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

                return File(
                    fileContents: fileContents,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: sFilename
                );
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return RedirectToAction("SaleTheoQuay");
            }
        }

        /////////////////////////////////////// Sale Theo Ngay Di //////////////////////////////////////////////////////////////////
        public IActionResult SaleTheoNgayDi(string tungay = null, string denngay = null, string chiNhanh = null, string khoi = null)
        {
            var dtSaleQuayVM = new DoanhthuSaleQuayViewModel();

            var chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

            for (int i = 0; i < chiNhanhs.Count(); i++)
            {
                var cnToreturn = new ChiNhanhToReturnViewModel()
                {
                    Stt = i,
                    Name = chiNhanhs[i]
                };

                dtSaleQuayVM.chiNhanhToReturnViewModels.Add(cnToreturn);
            }

            dtSaleQuayVM.khoiViewModels = khoiViewModels();

            try
            {
                ViewBag.searchFromDate = tungay;
                ViewBag.searchToDate = denngay;
                chiNhanh = chiNhanh ?? "";
                ViewBag.chiNhanh = chiNhanh;
                ViewBag.khoi = khoi;

                if (tungay == null || denngay == null)
                {
                    return View("SaleTheoNgayDi", dtSaleQuayVM);
                }

                var list = _unitOfWork.thongKeRepository.ListSaleTheoNgayDi(tungay, denngay, chiNhanh, khoi);
                dtSaleQuayVM.DoanhthuSaleQuays = list;
                return View(dtSaleQuayVM);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return View("SaleTheoNgayDi", dtSaleQuayVM);
            }
        }

        [HttpPost]
        public IActionResult SaleTheoNgayDiPost(string tungay, string denngay, string chinhanh, string khoi)
        {
            if (tungay == null || denngay == null)
            {
                return RedirectToAction("SaleTheoNgayDi");
            }
            try
            {
                DateTime.Parse(tungay);
                DateTime.Parse(denngay);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return RedirectToAction("SaleTheoNgayDi");
            }
            chinhanh = chinhanh ?? "";
            // cn = Session["chinhanh"].ToString();
            //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;
            string fromTo = "";
            ExcelPackage ExcelApp = new ExcelPackage();
            ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
            // Định dạng chiều dài cho cột
            xlSheet.Column(1).Width = 10;//stt
            xlSheet.Column(2).Width = 50;// sales
            xlSheet.Column(3).Width = 10;// code cn
            xlSheet.Column(4).Width = 30;// doanh so
            xlSheet.Column(5).Width = 30;// doanh thu sale

            xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH THU THEO NGÀY ĐI SALE " + khoi + " " + chinhanh;
            xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
            xlSheet.Cells[2, 1, 2, 5].Merge = true;
            setCenterAligment(2, 1, 2, 5, xlSheet);
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
            xlSheet.Cells[3, 1, 3, 5].Merge = true;
            xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
            setCenterAligment(3, 1, 3, 5, xlSheet);

            // Tạo header
            xlSheet.Cells[5, 1].Value = "STT";
            xlSheet.Cells[5, 2].Value = "Nhân viên ";
            xlSheet.Cells[5, 3].Value = "Code CN ";

            xlSheet.Cells[5, 4].Value = "Tổng tiền";
            xlSheet.Cells[5, 5].Value = "Doanh số";

            xlSheet.Cells[5, 1, 5, 5].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));

            int dong = 5;
            var d = _unitOfWork.thongKeRepository.SaleTheoNgayDiPost(tungay, denngay, chinhanh, khoi);// Session["fullName"].ToString());

            //du lieu
            int iRowIndex = 6;
            int idem = 1;

            if (d != null)
            {
                foreach (var vm in d)
                {
                    xlSheet.Cells[iRowIndex, 1].Value = idem;
                    TrSetCellBorder(xlSheet, iRowIndex, 1, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 2].Value = vm.Nguoixuatve;
                    TrSetCellBorder(xlSheet, iRowIndex, 2, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 3].Value = vm.Chinhanh;
                    TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 4].Value = vm.Doanhso;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 5].Value = vm.Thucthu;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    iRowIndex += 1;
                    idem += 1;
                    dong++;
                }
            }
            else
            {
                SetAlert("No sale.", "warning");
                return RedirectToAction(nameof(SaleTheoNgayDi));
            }

            dong++;
            //// Merger cot 4,5 ghi tổng tiền
            //setRightAligment(dong, 3, dong, 3, xlSheet);
            //xlSheet.Cells[dong, 1, dong, 2].Merge = true;
            //xlSheet.Cells[dong, 1].Value = "Tổng tiền: ";

            // Sum tổng tiền
            xlSheet.Cells[dong, 4].Formula = "SUM(D6:D" + (6 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 5].Formula = "SUM(E6:E" + (6 + d.Count() - 1) + ")";

            // định dạng số
            NumberFormat(dong, 4, dong, 5, xlSheet);

            setBorder(5, 1, 5 + d.Count(), 5, xlSheet);
            setFontBold(5, 1, 5, 5, 11, xlSheet);
            setFontSize(6, 1, 6 + d.Count(), 5, 11, xlSheet);
            // canh giua cot stt
            setCenterAligment(6, 1, 6 + d.Count(), 1, xlSheet);
            // canh giua code cn
            setCenterAligment(6, 3, 6 + d.Count(), 3, xlSheet);
            NumberFormat(6, 4, 6 + d.Count(), 5, xlSheet);
            // định dạng số cot tong cong
            NumberFormat(dong, 4, dong, 5, xlSheet);
            setBorder(dong, 4, dong, 5, xlSheet);
            setFontBold(dong, 4, dong, 5, 12, xlSheet);

            //end du lieu

            byte[] fileContents;
            fileContents = ExcelApp.GetAsByteArray();

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }
            string sFilename = "DoanhThuSale_" + khoi + "_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: sFilename
            );
        }

        public IActionResult SaleTheoNgayDiChiTietToExcel(string tungay, string denngay, string nhanvien, string chinhanh, string khoi)
        {
            try
            {
                nhanvien = convertToUnSign3(nhanvien);
                //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;
                string fromTo = "";
                ExcelPackage ExcelApp = new ExcelPackage();
                ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
                // Định dạng chiều dài cho cột
                xlSheet.Column(1).Width = 10;//stt
                xlSheet.Column(2).Width = 10;// chi nhanh
                xlSheet.Column(3).Width = 25;// sgtcode
                xlSheet.Column(4).Width = 25;// tuyen tham quan
                xlSheet.Column(5).Width = 40;// ten khach
                xlSheet.Column(6).Width = 10;// so khach
                xlSheet.Column(7).Width = 20;//doanhthu
                xlSheet.Column(8).Width = 20;//thuc thu
                xlSheet.Column(9).Width = 35;//sales

                xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH THU THEO NGÀY ĐI SALE " + nhanvien;
                xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
                xlSheet.Cells[2, 1, 2, 8].Merge = true;
                if (tungay == denngay)
                {
                    fromTo = "Ngày: " + tungay;
                }
                else
                {
                    fromTo = "Từ ngày: " + tungay + " đến ngày: " + denngay;
                }
                xlSheet.Cells[3, 1].Value = fromTo;
                xlSheet.Cells[3, 1, 3, 9].Merge = true;
                xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
                setCenterAligment(2, 1, 3, 9, xlSheet);

                // Tạo header
                xlSheet.Cells[5, 1].Value = "STT";
                xlSheet.Cells[5, 2].Value = "Code CN";
                xlSheet.Cells[5, 3].Value = "Code Đoàn";
                xlSheet.Cells[5, 4].Value = "Tuyến tham quan";
                xlSheet.Cells[5, 5].Value = "Tên khách";
                xlSheet.Cells[5, 6].Value = "Số khách";
                xlSheet.Cells[5, 7].Value = "Tổng tiền";
                xlSheet.Cells[5, 8].Value = "Doanh số";
                xlSheet.Cells[5, 9].Value = "Sales";

                xlSheet.Cells[5, 1, 5, 9].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));

                int dong = 5;
                var d = _unitOfWork.thongKeRepository.SaleTheoNgayDiChiTietToExcel(tungay, denngay, nhanvien, chinhanh, khoi);// Session["fullName"].ToString());

                //du lieu
                int iRowIndex = 6;
                int idem = 1;

                if (d != null)
                {
                    foreach (var vm in d)
                    {
                        xlSheet.Cells[iRowIndex, 1].Value = idem;
                        TrSetCellBorder(xlSheet, iRowIndex, 1, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 2].Value = vm.Chinhanh;
                        TrSetCellBorder(xlSheet, iRowIndex, 2, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 3].Value = vm.Sgtcode;
                        TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 4].Value = vm.Tuyentq;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 5].Value = vm.Tenkhach;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 6].Value = vm.Chiemcho;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 7].Value = vm.Doanhthu;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 8].Value = vm.Thucthu;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 9].Value = vm.Nguoixuatve;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        iRowIndex += 1;
                        idem += 1;
                        dong++;
                    }
                }
                else
                {
                    SetAlert("No sale.", "warning");
                    return RedirectToAction(nameof(SaleTheoQuay));
                }

                dong++;
                // Merger cot 4,5 ghi tổng tiền
                //setRightAligment(dong, 4, dong, 5, xlSheet);
                //xlSheet.Cells[dong, 4, dong, 5].Merge = true;
                //xlSheet.Cells[dong, 4].Value = "Tổng tiền: ";

                //// Sum tổng tiền
                xlSheet.Cells[dong, 8].Formula = "SUM(H6:H" + (6 + d.Count() - 1) + ")";
                //xlSheet.Cells[dong, 7].Formula = "SUM(G6:G" + (6 + d.Count() - 1) + ")";
                //// định dạng số
                NumberFormat(dong, 6, dong, 6, xlSheet);
                setBorder(5, 1, 5 + d.Count(), 9, xlSheet);
                setFontBold(5, 1, 5, 9, 12, xlSheet);
                setFontSize(6, 1, 6 + d.Count(), 9, 12, xlSheet);
                NumberFormat(6, 7, 6 + d.Count(), 8, xlSheet);
                setCenterAligment(6, 1, 6 + d.Count(), 3, xlSheet);
                setCenterAligment(6, 6, 6 + d.Count(), 6, xlSheet);
                xlSheet.View.FreezePanes(6, 20);

                //end du lieu

                byte[] fileContents;
                fileContents = ExcelApp.GetAsByteArray();

                if (fileContents == null || fileContents.Length == 0)
                {
                    return NotFound();
                }
                string sFilename = "DoanhThuSale_" + nhanvien + "_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

                return File(
                    fileContents: fileContents,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: sFilename
                );
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return RedirectToAction("SaleTheoQuay");
            }
        }

        /////////////////////////////////////// Sale Theo Tuyen Tham Quan ///////////////////////////////////////////////////////////////
        public IActionResult SaleTheoTuyenThamQuan(string tungay = null, string denngay = null, string tuyentq = null, string khoi = null)
        {
            ViewBag.searchFromDate = tungay;
            ViewBag.searchToDate = denngay;
            ViewBag.ttq = tuyentq;
            ViewBag.khoi = khoi;

            var dtSaleTuyenVM = new DoanhThuSaleTuyenViewModel();
            khoi = khoi ?? "OB";
            tuyentq = string.IsNullOrEmpty(tuyentq) ? "" : tuyentq.Trim();

            dtSaleTuyenVM.khoiViewModels = khoiViewModels();

            var tuyentqByKhois = _unitOfWork.accountRepository.GetAllTuyentqByKhoi(khoi);

            dtSaleTuyenVM.tuyenThamQuanViewModels = tuyentqByKhois;
            try
            {
                if (tungay == null || denngay == null)
                {
                    return View("SaleTheoTuyenThamQuan", dtSaleTuyenVM);
                }

                var list = _unitOfWork.thongKeRepository.ListSaleTheoTuyenThamQuan(tungay, denngay, tuyentq, khoi);
                dtSaleTuyenVM.DoanhthuSaleTuyens = list;
                return View(dtSaleTuyenVM);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return View("SaleTheoTuyenThamQuan", dtSaleTuyenVM);
            }
        }

        [HttpPost]
        public IActionResult SaleTheoTuyenThamQuanPost(string tungay, string denngay, string tuyentq, string khoi)
        {
            ViewBag.searchFromDate = tungay;
            ViewBag.searchToDate = denngay;
            ViewBag.ttq = tuyentq;

            tuyentq = string.IsNullOrEmpty(tuyentq) ? "" : tuyentq.Trim();

            if (tungay == null || denngay == null)
            {
                return RedirectToAction("SaleTheoTuyenThamQuan");
            }
            try
            {
                DateTime.Parse(tungay);
                DateTime.Parse(denngay);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return RedirectToAction("SaleTheoTuyenThamQuan");
            }

            // cn = Session["chinhanh"].ToString();
            //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;
            string fromTo = "";
            ExcelPackage ExcelApp = new ExcelPackage();
            ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
            // Định dạng chiều dài cho cột
            xlSheet.Column(1).Width = 10;//stt
            xlSheet.Column(2).Width = 50;// sales
            xlSheet.Column(3).Width = 10;// code cn
            xlSheet.Column(4).Width = 50;// tuyentq
            xlSheet.Column(5).Width = 20;// doanh so
            xlSheet.Column(6).Width = 20;// doanh thu sale

            xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH THU SALE THEO TUYẾN " + tuyentq;
            xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
            xlSheet.Cells[2, 1, 2, 6].Merge = true;
            setCenterAligment(2, 1, 2, 6, xlSheet);
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
            xlSheet.Cells[3, 1, 3, 6].Merge = true;
            xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
            setCenterAligment(3, 1, 3, 6, xlSheet);

            // Tạo header
            xlSheet.Cells[5, 1].Value = "STT";
            xlSheet.Cells[5, 2].Value = "Nhân viên ";
            xlSheet.Cells[5, 3].Value = "Code CN ";
            xlSheet.Cells[5, 4].Value = "Tuyến tham quan";
            //xlSheet.Cells[5, 5].Value = "Tổng tiền";
            //xlSheet.Cells[5, 6].Value = "Doanh số";

            xlSheet.Cells[5, 1, 5, 5].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));
            int dong = 5;
            var d = _unitOfWork.thongKeRepository.ListSaleTheoTuyenThamQuan(tungay, denngay, tuyentq, khoi);// Session["fullName"].ToString());

            //du lieu
            int iRowIndex = 6;
            int idem = 1;

            if (d != null)
            {
                foreach (var vm in d)
                {
                    xlSheet.Cells[iRowIndex, 1].Value = idem;
                    TrSetCellBorder(xlSheet, iRowIndex, 1, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 2].Value = vm.Nguoixuatve;
                    TrSetCellBorder(xlSheet, iRowIndex, 2, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 3].Value = vm.Chinhanh;
                    TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 4].Value = vm.Tuyentq;
                    TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 5].Value = vm.Doanhso;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 6].Value = vm.Thucthu;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    iRowIndex += 1;
                    idem += 1;
                    dong++;
                }
            }
            else
            {
                SetAlert("No sale.", "warning");
                return RedirectToAction(nameof(SaleTheoNgayDi));
            }

            dong++;
            //// Merger cot 4,5 ghi tổng tiền
            //setRightAligment(dong, 3, dong, 3, xlSheet);
            //xlSheet.Cells[dong, 1, dong, 2].Merge = true;
            //xlSheet.Cells[dong, 1].Value = "Tổng tiền: ";

            // Sum tổng tiền
            xlSheet.Cells[dong, 5].Formula = "SUM(E6:E" + (6 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 6].Formula = "SUM(F6:F" + (6 + d.Count() - 1) + ")";

            // định dạng số
            NumberFormat(dong, 5, dong, 6, xlSheet);

            setBorder(5, 1, 5 + d.Count(), 6, xlSheet);
            setFontBold(5, 1, 5, 6, 11, xlSheet);
            setFontSize(6, 1, 6 + d.Count(), 5, 11, xlSheet);
            // canh giua cot stt
            setCenterAligment(6, 1, 6 + d.Count(), 1, xlSheet);
            // canh giua code cn
            setCenterAligment(6, 3, 6 + d.Count(), 3, xlSheet);
            NumberFormat(6, 5, 6 + d.Count(), 6, xlSheet);
            // định dạng số cot tong cong
            //NumberFormat(dong, 4, dong, 5, xlSheet);
            setBorder(dong, 5, dong, 6, xlSheet);
            setFontBold(dong, 5, dong, 6, 12, xlSheet);

            //xlSheet.View.FreezePanes(6, 20);

            //end du lieu

            byte[] fileContents;
            fileContents = ExcelApp.GetAsByteArray();

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }
            string sFilename = "DoanhThuSaleTheoTuyen_" + khoi + "_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: sFilename
            );
        }

        public IActionResult SaleTheoTuyenThamQuanChiTietToExcel(string tungay, string denngay, string nhanvien, string tuyentq, string khoi)
        {
            try
            {
                nhanvien = convertToUnSign3(nhanvien);
                //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;
                string fromTo = "";
                ExcelPackage ExcelApp = new ExcelPackage();
                ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
                // Định dạng chiều dài cho cột
                xlSheet.Column(1).Width = 10;//stt
                xlSheet.Column(2).Width = 10;// chi nhanh
                xlSheet.Column(3).Width = 25;// sgtcode
                xlSheet.Column(4).Width = 25;// tuyen tham quan
                xlSheet.Column(5).Width = 40;// ten khach
                xlSheet.Column(6).Width = 10;// so khach
                xlSheet.Column(7).Width = 20;//doanhthu
                xlSheet.Column(8).Width = 20;//thuc thu
                xlSheet.Column(9).Width = 35;//sales

                xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH THU SALE THEO TUYEN " + tuyentq.ToUpper();
                xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
                xlSheet.Cells[2, 1, 2, 8].Merge = true;
                if (tungay == denngay)
                {
                    fromTo = "Ngày: " + tungay;
                }
                else
                {
                    fromTo = "Từ ngày: " + tungay + " đến ngày: " + denngay;
                }
                xlSheet.Cells[3, 1].Value = fromTo;
                xlSheet.Cells[3, 1, 3, 9].Merge = true;
                xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
                setCenterAligment(2, 1, 3, 9, xlSheet);

                // Tạo header
                xlSheet.Cells[5, 1].Value = "STT";
                xlSheet.Cells[5, 2].Value = "Code CN";
                xlSheet.Cells[5, 3].Value = "Code Đoàn";
                xlSheet.Cells[5, 4].Value = "Tuyến tham quan";
                xlSheet.Cells[5, 5].Value = "Tên khách";
                xlSheet.Cells[5, 6].Value = "Số khách";
                xlSheet.Cells[5, 7].Value = "Tổng tiền";
                xlSheet.Cells[5, 8].Value = "Doanh số";
                xlSheet.Cells[5, 9].Value = "Sales";

                xlSheet.Cells[5, 1, 5, 9].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));

                int dong = 5;
                var d = _unitOfWork.thongKeRepository.SaleTheoTuyenThamQuanChiTietToExcel(tungay, denngay, nhanvien, tuyentq, khoi);// Session["fullName"].ToString());

                //du lieu
                int iRowIndex = 6;
                int idem = 1;

                if (d != null)
                {
                    foreach (var vm in d)
                    {
                        xlSheet.Cells[iRowIndex, 1].Value = idem;
                        TrSetCellBorder(xlSheet, iRowIndex, 1, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 2].Value = vm.Chinhanh;
                        TrSetCellBorder(xlSheet, iRowIndex, 2, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 3].Value = vm.Sgtcode;
                        TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 4].Value = vm.Tuyentq;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 5].Value = vm.Tenkhach;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 6].Value = vm.Chiemcho;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 7].Value = vm.Doanhthu;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 8].Value = vm.Thucthu;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 9].Value = vm.Nguoixuatve;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        iRowIndex += 1;
                        idem += 1;
                        dong++;
                    }
                }
                else
                {
                    SetAlert("No sale.", "warning");
                    return RedirectToAction(nameof(SaleTheoQuay));
                }

                dong++;
                // Merger cot 4,5 ghi tổng tiền
                //setRightAligment(dong, 4, dong, 5, xlSheet);
                //xlSheet.Cells[dong, 4, dong, 5].Merge = true;
                //xlSheet.Cells[dong, 4].Value = "Tổng tiền: ";

                //// Sum tổng tiền
                xlSheet.Cells[dong, 8].Formula = "SUM(H6:H" + (6 + d.Count() - 1) + ")";
                //xlSheet.Cells[dong, 7].Formula = "SUM(G6:G" + (6 + d.Count() - 1) + ")";
                //// định dạng số
                NumberFormat(dong, 7, dong, 8, xlSheet);
                setBorder(5, 1, 5 + d.Count(), 9, xlSheet);
                setFontBold(5, 1, 5, 9, 12, xlSheet);
                setFontSize(6, 1, 6 + d.Count(), 9, 12, xlSheet);
                NumberFormat(6, 7, 6 + d.Count(), 8, xlSheet);
                setCenterAligment(6, 1, 6 + d.Count(), 3, xlSheet);
                setCenterAligment(6, 6, 6 + d.Count(), 6, xlSheet);
                xlSheet.View.FreezePanes(6, 20);

                //end du lieu

                byte[] fileContents;
                fileContents = ExcelApp.GetAsByteArray();

                if (fileContents == null || fileContents.Length == 0)
                {
                    return NotFound();
                }
                string sFilename = "DoanhThuTheoTuyentqSale_" + nhanvien + "_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

                return File(
                    fileContents: fileContents,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: sFilename
                );
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return RedirectToAction("SaleTheoTuyenThamQuan");
            }
        }

        ////////////////////////////////////// Quay Theo Ngay Ban ////////////////////////////////////////////////////////////////////////////
        public IActionResult QuayTheoNgayBan(string tungay = null, string denngay = null, string chiNhanh = null, string khoi = null)
        {
            var dtQuayTheoNgayBanVM = new DoanthuQuayNgayBanViewModel();

            var chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

            for (int i = 0; i < chiNhanhs.Count(); i++)
            {
                var cnToreturn = new ChiNhanhToReturnViewModel()
                {
                    Stt = i,
                    Name = chiNhanhs[i]
                };

                dtQuayTheoNgayBanVM.chiNhanhToReturnViewModels.Add(cnToreturn);
            }
            dtQuayTheoNgayBanVM.khoiViewModels = khoiViewModels();

            try
            {
                ViewBag.searchFromDate = tungay;
                ViewBag.searchToDate = denngay;
                chiNhanh = chiNhanh ?? "";
                ViewBag.chiNhanh = chiNhanh;
                ViewBag.khoi = khoi;

                if (tungay == null || denngay == null)
                {
                    return View("QuayTheoNgayBan", dtQuayTheoNgayBanVM);
                }

                var list = _unitOfWork.thongKeRepository.listQuayTheoNgayBan(tungay, denngay, chiNhanh, khoi);
                dtQuayTheoNgayBanVM.DoanthuQuayNgayBans = list;
                return View(dtQuayTheoNgayBanVM);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return View("QuayTheoNgayBan", dtQuayTheoNgayBanVM);
            }
        }

        public IActionResult QuayTheoNgayBanPost(string tungay, string denngay, string chinhanh, string khoi)//(string tungay,string denngay, string daily)
        {
            if (tungay == null || denngay == null)
            {
                return RedirectToAction("QuayTheoNgayBan");
            }
            try
            {
                DateTime.Parse(tungay);
                DateTime.Parse(denngay);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return RedirectToAction("QuayTheoNgayBan");
            }
            //chinhanh = String.IsNullOrEmpty(chinhanh) ? Session["chinhanh"].ToString() : chinhanh;
            //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;

            chinhanh = chinhanh ?? "";
            string fromTo = "";
            ExcelPackage ExcelApp = new ExcelPackage();
            ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
            // Định dạng chiều dài cho cột
            xlSheet.Column(1).Width = 10;//stt
            xlSheet.Column(2).Width = 40;// quay
            xlSheet.Column(3).Width = 10;// cn
            xlSheet.Column(4).Width = 10;// so khach
            xlSheet.Column(5).Width = 20;// doanh số
            xlSheet.Column(6).Width = 20;// doanh thu

            xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH THU THEO NGÀY BÁN QUẦY " + khoi + "  " + chinhanh;
            xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
            xlSheet.Cells[2, 1, 2, 6].Merge = true;
            setCenterAligment(2, 1, 2, 6, xlSheet);
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
            xlSheet.Cells[3, 1, 3, 6].Merge = true;
            xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
            setCenterAligment(3, 1, 3, 6, xlSheet);

            // Tạo header
            xlSheet.Cells[5, 1].Value = "STT";
            xlSheet.Cells[5, 2].Value = "Văn phòng xuất vé ";
            xlSheet.Cells[5, 3].Value = "Code CN ";
            xlSheet.Cells[5, 4].Value = "Số khách";
            xlSheet.Cells[5, 5].Value = "Tổng tiền";
            xlSheet.Cells[5, 6].Value = "Doanh số";
            xlSheet.Cells[5, 1, 5, 6].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));

            // do du lieu tu table
            int dong = 5;

            var d = _unitOfWork.thongKeRepository.listQuayTheoNgayBan(tungay, denngay, chinhanh, khoi);// Session["daily"].ToString(), Session["khoi"].ToString());

            //du lieu
            int iRowIndex = 6;
            int idem = 1;

            if (d != null)
            {
                foreach (var vm in d)
                {
                    xlSheet.Cells[iRowIndex, 1].Value = idem;
                    TrSetCellBorder(xlSheet, iRowIndex, 1, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 2].Value = vm.Dailyxuatve;
                    TrSetCellBorder(xlSheet, iRowIndex, 2, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 3].Value = vm.Chinhanh;
                    TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 4].Value = vm.Sokhach;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 5].Value = vm.Doanhso;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 6].Value = vm.Doanhthu;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    iRowIndex += 1;
                    idem += 1;
                    dong++;
                }
            }
            else
            {
                SetAlert("No sale.", "warning");
                return RedirectToAction(nameof(QuayTheoNgayBan));
            }

            dong++;

            // Sum tổng tiền
            xlSheet.Cells[dong, 3].Value = "TC";
            xlSheet.Cells[dong, 4].Formula = "SUM(D6:D" + (6 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 5].Formula = "SUM(E6:E" + (6 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 6].Formula = "SUM(F6:F" + (6 + d.Count() - 1) + ")";
            // định dạng số
            NumberFormat(dong, 5, dong, 6, xlSheet);
            setFontSize(6, 1, 6 + d.Count(), 6, 12, xlSheet);
            setBorder(5, 1, 5 + d.Count(), 6, xlSheet);
            // font bold tieu de bang
            setFontBold(5, 1, 5, 6, 12, xlSheet);
            // font bold dong cuoi cùng
            setFontBold(dong, 1, dong, 6, 12, xlSheet);
            setBorder(dong, 3, dong, 6, xlSheet);
            // canh giưa cot stt
            setCenterAligment(6, 1, 6 + d.Count(), 1, xlSheet);

            // canh giưa cot chinhanh va so khach
            setCenterAligment(6, 3, 6 + d.Count(), 4, xlSheet);
            // dinh dạng number cot sokhach, doanh so, thuc thu
            NumberFormat(6, 5, 6 + d.Count(), 6, xlSheet);

            //xlSheet.View.FreezePanes(6, 20);

            //end du lieu

            byte[] fileContents;
            fileContents = ExcelApp.GetAsByteArray();

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }
            string sFilename = "DoanhThuQuay" + khoi + " " + chinhanh + "_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: sFilename
            );
        }

        public IActionResult QuayTheoNgayBanChiTietToExcel(string tungay, string denngay, string quay, string chinhanh, string khoi)
        {
            try
            {
                //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;
                string fromTo = "";
                ExcelPackage ExcelApp = new ExcelPackage();
                ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
                // Định dạng chiều dài cho cột
                xlSheet.Column(1).Width = 10;//STT
                xlSheet.Column(2).Width = 10;//Code CN
                xlSheet.Column(3).Width = 25;// SGTCODE
                xlSheet.Column(4).Width = 15;// serial
                xlSheet.Column(5).Width = 30;// ten khach
                xlSheet.Column(6).Width = 40;// tuyen tq
                xlSheet.Column(7).Width = 15;//  ngay di
                xlSheet.Column(8).Width = 15;//  ngay ve
                xlSheet.Column(9).Width = 15;//  gia tour
                xlSheet.Column(10).Width = 30;//  sale

                xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH THU THEO NGÀY BÁN QUẦY " + quay;
                xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
                xlSheet.Cells[2, 1, 2, 10].Merge = true;
                setCenterAligment(2, 1, 2, 10, xlSheet);
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
                xlSheet.Cells[3, 1, 3, 10].Merge = true;
                xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
                setCenterAligment(3, 1, 3, 10, xlSheet);

                // Tạo header
                xlSheet.Cells[5, 1].Value = "STT";
                xlSheet.Cells[5, 2].Value = "Code CN";
                xlSheet.Cells[5, 3].Value = "Sgt Code ";
                xlSheet.Cells[5, 4].Value = "Serial";
                xlSheet.Cells[5, 5].Value = "Tên khách";
                xlSheet.Cells[5, 6].Value = "Hành trình";
                xlSheet.Cells[5, 7].Value = "Ngày đi";
                xlSheet.Cells[5, 8].Value = "Ngày về";
                xlSheet.Cells[5, 9].Value = "Doanh số";
                xlSheet.Cells[5, 10].Value = "Nhân viên";
                xlSheet.Cells[5, 1, 5, 10].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));

                int dong = 5;
                var d = _unitOfWork.thongKeRepository.QuayTheoNgayBanChiTietToExcel(tungay, denngay, quay, chinhanh, khoi);// Session["fullName"].ToString());

                //du lieu
                int iRowIndex = 6;
                int idem = 1;

                if (d != null)
                {
                    foreach (var vm in d)
                    {
                        xlSheet.Cells[iRowIndex, 1].Value = idem;
                        TrSetCellBorder(xlSheet, iRowIndex, 1, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 2].Value = vm.Chinhanh;
                        TrSetCellBorder(xlSheet, iRowIndex, 2, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 3].Value = vm.Sgtcode;
                        TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 4].Value = vm.Serial;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 5].Value = vm.Tenkhach;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 6].Value = vm.Hanhtrinh;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 7].Value = vm.Ngaydi;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 8].Value = vm.Ngayve;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 9].Value = vm.Giave;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 10].Value = vm.Nguoiban;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        iRowIndex += 1;
                        idem += 1;
                        dong++;
                    }
                }
                else
                {
                    SetAlert("No sale.", "warning");
                    return RedirectToAction(nameof(QuayTheoNgayBan));
                }

                dong++;
                //// Merger cot 4,5 ghi tổng tiền
                //setRightAligment(dong, 3, dong, 3, xlSheet);
                //xlSheet.Cells[dong, 1, dong, 2].Merge = true;
                //xlSheet.Cells[dong, 1].Value = "Tổng tiền: ";
                // Sum tổng tiền
                xlSheet.Cells[dong, 8].Value = "TC";
                xlSheet.Cells[dong, 9].Formula = "SUM(I6:I" + (6 + d.Count() - 1) + ")";

                // định dạng số
                NumberFormat(dong, 8, dong, 8, xlSheet);
                setFontSize(6, 1, 6 + d.Count(), 11, 12, xlSheet);
                setBorder(5, 1, 5 + d.Count(), 10, xlSheet);
                setFontBold(5, 1, 5, 10, 12, xlSheet);

                // canh giưa cot stt
                setCenterAligment(6, 1, 6 + d.Count(), 2, xlSheet);

                setBorder(dong, 8, dong, 9, xlSheet);
                setFontBold(dong, 8, dong, 9, 12, xlSheet);
                // canh giưa cot ngay di va ngày ve
                setCenterAligment(6, 7, 6 + d.Count(), 8, xlSheet);
                // dinh dạng number cot gia ve
                NumberFormat(6, 9, 6 + d.Count(), 9, xlSheet);

                //xlSheet.View.FreezePanes(6, 20);

                //end du lieu

                byte[] fileContents;
                fileContents = ExcelApp.GetAsByteArray();

                if (fileContents == null || fileContents.Length == 0)
                {
                    return NotFound();
                }
                string sFilename = "DoanhThuQuayChitiet" + "_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

                return File(
                    fileContents: fileContents,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: sFilename
                );
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return RedirectToAction("QuayTheoNgayBan");
            }
        }

        /////////////////////////////////////// Quay Theo ngay di ///////////////////////////////////////////////////////////////////////////
        public IActionResult QuayTheoNgayDi(string tungay = null, string denngay = null, string chiNhanh = null, string khoi = null)
        {
            var dtQuayTheoNgayDiVM = new DoanthuQuayNgayBanViewModel();

            var chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

            for (int i = 0; i < chiNhanhs.Count(); i++)
            {
                var cnToreturn = new ChiNhanhToReturnViewModel()
                {
                    Stt = i,
                    Name = chiNhanhs[i]
                };

                dtQuayTheoNgayDiVM.chiNhanhToReturnViewModels.Add(cnToreturn);
            }

            dtQuayTheoNgayDiVM.khoiViewModels = khoiViewModels();

            try
            {
                ViewBag.searchFromDate = tungay;
                ViewBag.searchToDate = denngay;
                chiNhanh = chiNhanh ?? "";
                ViewBag.chiNhanh = chiNhanh;
                ViewBag.khoi = khoi;

                if (tungay == null || denngay == null)
                {
                    return View("QuayTheoNgayDi", dtQuayTheoNgayDiVM);
                }

                var list = _unitOfWork.thongKeRepository.listQuayTheoNgayDi(tungay, denngay, chiNhanh, khoi);
                dtQuayTheoNgayDiVM.DoanthuQuayNgayBans = list;
                return View(dtQuayTheoNgayDiVM);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return View("QuayTheoNgayDi", dtQuayTheoNgayDiVM);
            }
        }

        public IActionResult QuayTheoNgayDiPost(string tungay, string denngay, string chinhanh, string khoi)//(string tungay,string denngay, string daily)
        {
            if (tungay == null || denngay == null)
            {
                return RedirectToAction("QuayTheoNgayDi");
            }
            try
            {
                DateTime.Parse(tungay);
                DateTime.Parse(denngay);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return RedirectToAction("QuayTheoNgayDi");
            }
            //chinhanh = String.IsNullOrEmpty(chinhanh) ? Session["chinhanh"].ToString() : chinhanh;
            //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;

            chinhanh = chinhanh ?? "";
            string fromTo = "";
            ExcelPackage ExcelApp = new ExcelPackage();
            ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
            // Định dạng chiều dài cho cột
            xlSheet.Column(1).Width = 10;//STT
            xlSheet.Column(2).Width = 40;// quay
            xlSheet.Column(3).Width = 10;// cn
            xlSheet.Column(4).Width = 10;// so khach
            xlSheet.Column(5).Width = 20;// doanh số
            xlSheet.Column(6).Width = 20;// doanh thu

            xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH THU THEO NGÀY ĐI QUẦY " + khoi + " " + chinhanh;
            xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
            xlSheet.Cells[2, 1, 2, 6].Merge = true;
            setCenterAligment(2, 1, 2, 6, xlSheet);
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
            xlSheet.Cells[3, 1, 3, 6].Merge = true;
            xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
            setCenterAligment(3, 1, 3, 6, xlSheet);

            // Tạo header
            xlSheet.Cells[5, 1].Value = "STT";
            xlSheet.Cells[5, 2].Value = "Văn phòng xuất vé ";
            xlSheet.Cells[5, 3].Value = "Code CN ";
            xlSheet.Cells[5, 4].Value = "Số khách";
            xlSheet.Cells[5, 5].Value = "Tổng tiền";
            xlSheet.Cells[5, 6].Value = "Doanh số";
            xlSheet.Cells[5, 1, 5, 6].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));

            // do du lieu tu table
            int dong = 5;

            var d = _unitOfWork.thongKeRepository.listQuayTheoNgayDi(tungay, denngay, chinhanh, khoi);// Session["daily"].ToString(), Session["khoi"].ToString());

            //du lieu
            int iRowIndex = 6;
            int idem = 1;

            if (d != null)
            {
                foreach (var vm in d)
                {
                    xlSheet.Cells[iRowIndex, 1].Value = idem;
                    TrSetCellBorder(xlSheet, iRowIndex, 1, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 2].Value = vm.Dailyxuatve;
                    TrSetCellBorder(xlSheet, iRowIndex, 2, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 3].Value = vm.Chinhanh;
                    TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 4].Value = vm.Sokhach;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 5].Value = vm.Doanhso;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 6].Value = vm.Doanhthu;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    iRowIndex += 1;
                    idem += 1;
                    dong++;
                }
            }
            else
            {
                SetAlert("No sale.", "warning");
                return RedirectToAction(nameof(QuayTheoNgayDi));
            }

            dong++;
            //// Merger cot 4,5 ghi tổng tiền
            //setRightAligment(dong, 3, dong, 3, xlSheet);
            //xlSheet.Cells[dong, 1, dong, 2].Merge = true;
            //xlSheet.Cells[dong, 1].Value = "Tổng tiền: ";
            // Sum tổng tiền
            xlSheet.Cells[dong, 3].Value = "TC";
            xlSheet.Cells[dong, 4].Formula = "SUM(D6:D" + (6 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 5].Formula = "SUM(E6:E" + (6 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 6].Formula = "SUM(F6:F" + (6 + d.Count() - 1) + ")";
            // định dạng số
            NumberFormat(dong, 5, dong, 6, xlSheet);
            setFontSize(6, 1, 6 + d.Count(), 6, 12, xlSheet);
            setBorder(5, 1, 5 + d.Count(), 6, xlSheet);
            setFontBold(5, 1, 5, 6, 12, xlSheet);
            // canh giưa cot stt
            setCenterAligment(6, 1, 6 + d.Count(), 1, xlSheet);

            setBorder(dong, 3, dong, 6, xlSheet);
            setFontBold(dong, 1, dong, 6, 12, xlSheet);
            // canh giưa cot chinhanh va so khach
            setCenterAligment(6, 3, 6 + d.Count(), 4, xlSheet);
            // dinh dạng number cot sokhach, doanh so, thuc thu
            NumberFormat(6, 5, 6 + d.Count(), 6, xlSheet);

            //xlSheet.View.FreezePanes(6, 20);

            //end du lieu

            byte[] fileContents;
            fileContents = ExcelApp.GetAsByteArray();

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }
            string sFilename = "DoanhThuQuay" + "_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: sFilename
            );
        }

        public IActionResult QuayTheoNgayDiChiTietToExcel(string tungay, string denngay, string quay, string chinhanh, string khoi)
        {
            try
            {
                //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;
                string fromTo = "";
                ExcelPackage ExcelApp = new ExcelPackage();
                ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
                // Định dạng chiều dài cho cột
                xlSheet.Column(1).Width = 10;//STT
                xlSheet.Column(2).Width = 10;//STT
                xlSheet.Column(3).Width = 25;// SGTCODE
                xlSheet.Column(4).Width = 15;// serial
                xlSheet.Column(5).Width = 30;// ten khach
                xlSheet.Column(6).Width = 40;// tuyen tq
                xlSheet.Column(7).Width = 15;//  ngay di
                xlSheet.Column(8).Width = 15;//  ngay ve
                xlSheet.Column(9).Width = 15;//  gia tour
                xlSheet.Column(10).Width = 30;//  sale

                xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH THU THEO NGÀY ĐI QUẦY " + quay;
                xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
                xlSheet.Cells[2, 1, 2, 10].Merge = true;
                setCenterAligment(2, 1, 2, 10, xlSheet);
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
                xlSheet.Cells[3, 1, 3, 10].Merge = true;
                xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
                setCenterAligment(3, 1, 3, 10, xlSheet);

                // Tạo header
                xlSheet.Cells[5, 1].Value = "STT";
                xlSheet.Cells[5, 2].Value = "Code CN";
                xlSheet.Cells[5, 3].Value = "Sgt Code ";
                xlSheet.Cells[5, 4].Value = "Serial";
                xlSheet.Cells[5, 5].Value = "Tên khách";
                xlSheet.Cells[5, 6].Value = "Hành trình";
                xlSheet.Cells[5, 7].Value = "Ngày đi";
                xlSheet.Cells[5, 8].Value = "Ngày về";
                xlSheet.Cells[5, 9].Value = "Doanh số";
                xlSheet.Cells[5, 10].Value = "Nhân viên";
                xlSheet.Cells[5, 1, 5, 10].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));

                int dong = 5;
                var d = _unitOfWork.thongKeRepository.QuayTheoNgayDiChiTietToExcel(tungay, denngay, quay, chinhanh, khoi);// Session["fullName"].ToString());

                //du lieu
                int iRowIndex = 6;
                int idem = 1;

                if (d != null)
                {
                    foreach (var vm in d)
                    {
                        xlSheet.Cells[iRowIndex, 1].Value = idem;
                        TrSetCellBorder(xlSheet, iRowIndex, 1, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 2].Value = vm.Chinhanh;
                        TrSetCellBorder(xlSheet, iRowIndex, 2, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 3].Value = vm.Sgtcode;
                        TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 4].Value = vm.Serial;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 5].Value = vm.Tenkhach;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 6].Value = vm.Hanhtrinh;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 7].Value = vm.Ngaydi;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 8].Value = vm.Ngayve;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 9].Value = vm.Giave;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 10].Value = vm.Nguoiban;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        iRowIndex += 1;
                        idem += 1;
                        dong++;
                    }
                }
                else
                {
                    SetAlert("No sale.", "warning");
                    return RedirectToAction(nameof(QuayTheoNgayDi));
                }

                dong++;
                //// Merger cot 4,5 ghi tổng tiền
                //setRightAligment(dong, 3, dong, 3, xlSheet);
                //xlSheet.Cells[dong, 1, dong, 2].Merge = true;
                //xlSheet.Cells[dong, 1].Value = "Tổng tiền: ";
                // Sum tổng tiền
                xlSheet.Cells[dong, 8].Value = "TC";
                xlSheet.Cells[dong, 9].Formula = "SUM(I6:I" + (6 + d.Count() - 1) + ")";

                // định dạng số
                NumberFormat(dong, 8, dong, 8, xlSheet);
                setFontSize(6, 1, 6 + d.Count(), 11, 12, xlSheet);
                setBorder(5, 1, 5 + d.Count(), 10, xlSheet);
                setFontBold(5, 1, 5, 10, 12, xlSheet);

                // canh giưa cot stt
                setCenterAligment(6, 1, 6 + d.Count(), 2, xlSheet);

                setBorder(dong, 8, dong, 9, xlSheet);
                setFontBold(dong, 8, dong, 9, 12, xlSheet);
                // canh giưa cot ngay di va ngày ve
                setCenterAligment(6, 7, 6 + d.Count(), 8, xlSheet);
                // dinh dạng number cot gia ve
                NumberFormat(6, 9, 6 + d.Count(), 9, xlSheet);

                // xlSheet.View.FreezePanes(6, 20);

                //end du lieu

                byte[] fileContents;
                fileContents = ExcelApp.GetAsByteArray();

                if (fileContents == null || fileContents.Length == 0)
                {
                    return NotFound();
                }
                string sFilename = "DoanhThuQuayChitiet" + "_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

                return File(
                    fileContents: fileContents,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: sFilename
                );
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return RedirectToAction("QuayTheoNgayDi");
            }
        }

        /////////////////////////////////////// Doan Theo ngay di ///////////////////////////////////////////////////////////////////////////
        public IActionResult DoanTheoNgayDi(string tungay = null, string denngay = null, string chiNhanh = null, string khoi = null)
        {
            var doanTheoNgayDiVM = new DoanTheoNgayDiViewModel();

            var chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

            for (int i = 0; i < chiNhanhs.Count(); i++)
            {
                var cnToreturn = new ChiNhanhToReturnViewModel()
                {
                    Stt = i,
                    Name = chiNhanhs[i]
                };

                doanTheoNgayDiVM.chiNhanhToReturnViewModels.Add(cnToreturn);
            }

            doanTheoNgayDiVM.khoiViewModels = khoiViewModels();

            try
            {
                ViewBag.searchFromDate = tungay;
                ViewBag.searchToDate = denngay;
                chiNhanh = chiNhanh ?? "";
                ViewBag.chiNhanh = chiNhanh;
                ViewBag.khoi = khoi;

                if (tungay == null || denngay == null)
                {
                    return View("DoanTheoNgayDi", doanTheoNgayDiVM);
                }

                var list = _unitOfWork.thongKeRepository.listDoanTheoNgayDi(tungay, denngay, chiNhanh, khoi);
                doanTheoNgayDiVM.DoanhthuDoanNgayDis = list;
                return View(doanTheoNgayDiVM);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return View("DoanTheoNgayDi", doanTheoNgayDiVM);
            }
        }

        public IActionResult DoanTheoNgayDiPost(string tungay, string denngay, string chinhanh, string khoi)//(string tungay,string denngay, string daily)
        {
            if (tungay == null || denngay == null)
            {
                return RedirectToAction("DoanTheoNgayDi");
            }
            try
            {
                DateTime.Parse(tungay);
                DateTime.Parse(denngay);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return RedirectToAction("DoanTheoNgayDi");
            }
            //chinhanh = String.IsNullOrEmpty(chinhanh) ? Session["chinhanh"].ToString() : chinhanh;
            //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;

            chinhanh = chinhanh ?? "";
            string fromTo = "";
            ExcelPackage ExcelApp = new ExcelPackage();
            ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
            // Định dạng chiều dài cho cột
            xlSheet.Column(1).Width = 10;//STT
            xlSheet.Column(2).Width = 25;// sgtcode
            xlSheet.Column(3).Width = 40;// tuyen tq
            xlSheet.Column(4).Width = 20;// bat dau 
            xlSheet.Column(5).Width = 20;// ket thu
            xlSheet.Column(6).Width = 10;// so khach
            xlSheet.Column(7).Width = 25;//doanh thu

            xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH THU THEO ĐOÀN  " + khoi + "  " + chinhanh;
            xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
            xlSheet.Cells[2, 1, 2, 7].Merge = true;
            setCenterAligment(2, 1, 2, 7, xlSheet);
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
            setCenterAligment(3, 1, 3, 7, xlSheet);

            // Tạo header
            xlSheet.Cells[5, 1].Value = "STT";
            xlSheet.Cells[5, 2].Value = "Sgt Code ";
            xlSheet.Cells[5, 3].Value = "Tuyến tham quan ";
            xlSheet.Cells[5, 4].Value = "Ngày đi";
            xlSheet.Cells[5, 5].Value = "Ngày về";
            xlSheet.Cells[5, 6].Value = "Số khách";
            xlSheet.Cells[5, 7].Value = "Doanh số bán";
            xlSheet.Cells[5, 1, 5, 7].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));
            // do du lieu tu table
            int dong = 5;

            var d = _unitOfWork.thongKeRepository.listDoanTheoNgayDi(tungay, denngay, chinhanh, khoi);// Session["daily"].ToString(), Session["khoi"].ToString());

            //du lieu
            int iRowIndex = 6;
            int idem = 1;

            if (d != null)
            {
                foreach (var vm in d)
                {
                    xlSheet.Cells[iRowIndex, 1].Value = idem;
                    TrSetCellBorder(xlSheet, iRowIndex, 1, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 2].Value = vm.Sgtcode;
                    TrSetCellBorder(xlSheet, iRowIndex, 2, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 3].Value = vm.Tuyentq;
                    TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 4].Value = vm.Batdau;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 5].Value = vm.Ketthuc;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 6].Value = vm.Sokhach;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 7].Value = vm.Doanhthu;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    iRowIndex += 1;
                    idem += 1;
                    dong++;
                }
            }
            else
            {
                SetAlert("No sale.", "warning");
                return RedirectToAction(nameof(DoanTheoNgayDi));
            }

            dong++;
            //// Merger cot 4,5 ghi tổng tiền
            //setRightAligment(dong, 3, dong, 3, xlSheet);
            //xlSheet.Cells[dong, 1, dong, 2].Merge = true;
            //xlSheet.Cells[dong, 1].Value = "Tổng tiền: ";
            // Sum tổng tiền
            xlSheet.Cells[dong, 5].Value = "TC";
            xlSheet.Cells[dong, 6].Formula = "SUM(F6:F" + (6 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 7].Formula = "SUM(G6:G" + (6 + d.Count() - 1) + ")";

            // định dạng số
            NumberFormat(dong, 6, dong, 7, xlSheet);

            setBorder(5, 1, 5 + d.Count(), 7, xlSheet);
            setFontBold(5, 1, 5, 6, 12, xlSheet);
            setFontSize(6, 1, 6 + d.Count(), 7, 12, xlSheet);
            // dinh dang giua cho cot stt
            setCenterAligment(6, 1, 6 + d.Count(), 1, xlSheet);

            setBorder(dong, 5, dong, 7, xlSheet);
            setFontBold(dong, 5, dong, 7, 12, xlSheet);

            // dinh dạng ngay thang cho cot ngay di , ngay ve
            DateTimeFormat(6, 4, 6 + d.Count(), 5, xlSheet);
            // canh giưa cot  ngay di, ngay ve, so khach 
            setCenterAligment(6, 4, 6 + d.Count(), 6, xlSheet);
            // dinh dạng number cot doanh so
            NumberFormat(6, 7, 6 + d.Count(), 7, xlSheet);

            //xlSheet.View.FreezePanes(6, 20);

            //end du lieu

            byte[] fileContents;
            fileContents = ExcelApp.GetAsByteArray();

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }
            string sFilename = "DoanhThuDoan_" + khoi + "_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: sFilename
            );
        }

        public IActionResult DoanTheoNgayDiChiTietToExcel(string sgtcode, string khoi)
        {
            //try
            //{
            //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;
            string fromTo = "";

            var dtuyen = _unitOfWork.thongKeRepository.getTourbySgtcode(sgtcode, khoi);
            string tuyentq = dtuyen.FirstOrDefault().tuyentq;
            string ngay = "ĐOÀN: " + sgtcode + " bắt đầu: " + dtuyen.FirstOrDefault().batdau.ToString("dd/MM/yyyy HH:mm") + " kết thúc: " + dtuyen.FirstOrDefault().ketthuc.ToString("dd/MM/yyyy HH:mm");

            ExcelPackage ExcelApp = new ExcelPackage();
            ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("DoanhthuDoan");
            // Định dạng chiều dài cho cột
            xlSheet.Column(1).Width = 7;//stt
            xlSheet.Column(2).Width = 20;// Serial
            xlSheet.Column(3).Width = 40;// Ten khach
            xlSheet.Column(4).Width = 45;// Dia chi
            xlSheet.Column(5).Width = 30;// Diem don
            xlSheet.Column(6).Width = 10;// Gia ve
            xlSheet.Column(7).Width = 10;//Thuc thu
            xlSheet.Column(8).Width = 10;//Cong no
            xlSheet.Column(9).Width = 45;//Ghi chu

            xlSheet.Cells[2, 1].Value = tuyentq;// "BÁO CÁO DOANH THU THEO NGÀY ĐI " + sgtcode;
            xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
            xlSheet.Cells[2, 1, 2, 8].Merge = true;

            xlSheet.Cells[3, 1].Value = ngay;
            xlSheet.Cells[3, 1, 3, 8].Merge = true;
            xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Regular));
            setCenterAligment(2, 1, 3, 8, xlSheet);

            // Tạo header
            xlSheet.Cells[5, 1].Value = "STT";
            xlSheet.Cells[5, 2].Value = "Serial";
            xlSheet.Cells[5, 3].Value = "Tên khách";
            xlSheet.Cells[5, 4].Value = "Địa chỉ - Tel";
            xlSheet.Cells[5, 5].Value = "Điểm đón";
            xlSheet.Cells[5, 6].Value = "Giá vé";
            xlSheet.Cells[5, 7].Value = "Thực thu";
            xlSheet.Cells[5, 8].Value = "Công nợ";
            xlSheet.Cells[5, 9].Value = "Ghi chú";

            xlSheet.Cells[5, 1, 5, 9].Style.Font.SetFromFont(new Font("Times New Roman", 10, FontStyle.Bold));
            setBorder(5, 1, 5, 9, xlSheet);

            int dong = 5;
            int giongnhau = 0;
            var d = _unitOfWork.thongKeRepository.DoanTheoNgayDiChiTietToExcel(sgtcode, khoi).ToList();// Session["fullName"].ToString());

            var ranges = d.To2DArray(x => x.Id, x => x.Vetourid,
                                    x => x.Stt, x => x.Serial,
                                    x => x.Tenkhach, x => x.Diachi,
                                    x => x.Diemdon, x => x.Giave,
                                    x => x.Thucthu, x => x.Congno,
                                    x => x.Ghichu);
            //du lieu
            int iRowIndex = 6;
            int idem = 1;

            if (d != null)
            {
                for (int i = 0; i < d.Count(); i++)
                {
                    dong++;
                    for (int j = 2; j < 11; j++)
                    {
                        if (ranges[i, j] == null)
                        {
                            xlSheet.Cells[dong, j - 1].Value = " ";
                        }
                        else
                        {
                            xlSheet.Cells[dong, j - 1].Value = ranges[i, j];
                        }
                    }

                    if (i > 0 && ranges[i, 1].ToString() == ranges[i - 1, 1].ToString())
                    {
                        giongnhau++;

                    }
                    else
                    {
                        giongnhau = 0;
                    }
                    if (giongnhau > 0)
                    {
                        mergercell(dong - giongnhau, 2, dong, 2, xlSheet);
                        mergercell(dong - giongnhau, 5, dong, 5, xlSheet);
                        numberMergercell(dong - giongnhau, 6, dong, 6, xlSheet);
                        numberMergercell(dong - giongnhau, 7, dong, 7, xlSheet);
                        numberMergercell(dong - giongnhau, 8, dong, 8, xlSheet);
                        mergercell(dong - giongnhau, 9, dong, 9, xlSheet);
                        setBorderAround(dong - giongnhau, 1, dong, 1, xlSheet);
                        setBorderAround(dong - giongnhau, 2, dong, 2, xlSheet);
                        setBorderAround(dong - giongnhau, 3, dong, 3, xlSheet);
                        setBorderAround(dong - giongnhau, 4, dong, 4, xlSheet);
                        setBorderAround(dong - giongnhau, 5, dong, 5, xlSheet);
                        setBorderAround(dong - giongnhau, 6, dong, 6, xlSheet);
                        setBorderAround(dong - giongnhau, 7, dong, 7, xlSheet);
                        setBorderAround(dong - giongnhau, 8, dong, 8, xlSheet);
                        setBorderAround(dong - giongnhau, 9, dong, 9, xlSheet);
                    }
                    else
                    {
                        wrapText(dong, 2, dong, 2, xlSheet);
                        wrapText(dong, 5, dong, 9, xlSheet);
                        wrapText(dong, 9, dong, 9, xlSheet);
                        setBorderAround(dong, 1, dong, 1, xlSheet);
                        setBorderAround(dong, 2, dong, 2, xlSheet);
                        setBorderAround(dong, 3, dong, 3, xlSheet);
                        setBorderAround(dong, 4, dong, 4, xlSheet);
                        setBorderAround(dong, 5, dong, 5, xlSheet);
                        setBorderAround(dong, 6, dong, 6, xlSheet);
                        setBorderAround(dong, 7, dong, 7, xlSheet);
                        setBorderAround(dong, 8, dong, 8, xlSheet);
                        setBorderAround(dong, 9, dong, 9, xlSheet);
                    }
                }
                //foreach (var vm in d)
                //{
                //    xlSheet.Cells[iRowIndex, 1].Value = idem;
                //    TrSetCellBorder(xlSheet, iRowIndex, 1, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                //    xlSheet.Cells[iRowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //    xlSheet.Cells[iRowIndex, 2].Value = vm.Serial;
                //    TrSetCellBorder(xlSheet, iRowIndex, 2, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                //    xlSheet.Cells[iRowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //    xlSheet.Cells[iRowIndex, 3].Value = vm.Tenkhach;
                //    TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                //    xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //    xlSheet.Cells[iRowIndex, 4].Value = vm.Diachi;
                //    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                //    xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //    xlSheet.Cells[iRowIndex, 5].Value = vm.Diemdon;
                //    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                //    xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //    xlSheet.Cells[iRowIndex, 6].Value = vm.Giave;
                //    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                //    xlSheet.Cells[iRowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //    xlSheet.Cells[iRowIndex, 7].Value = vm.Thucthu;
                //    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                //    xlSheet.Cells[iRowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //    xlSheet.Cells[iRowIndex, 8].Value = vm.Congno;
                //    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                //    xlSheet.Cells[iRowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //    xlSheet.Cells[iRowIndex, 9].Value = vm.Ghichu;
                //    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                //    xlSheet.Cells[iRowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //    iRowIndex += 1;
                //    idem += 1;
                //    dong++;

                //}


            }
            else
            {
                SetAlert("No sale.", "warning");
                return RedirectToAction(nameof(DoanTheoNgayDi));
            }

            dong++;
            // set border
            //  setBorder(5, 1, 5 + d.Count(), 9, xlSheet);
            setFontSize(6, 1, 6 + d.Count() + 1, 9, 9, xlSheet);
            setCenterAligment(6, 1, 6 + d.Count(), 1, xlSheet);
            wrapText(6, 6, 6 + d.Count() + 1, 8, xlSheet);

            //// Sum tổng tiền
            xlSheet.Cells[dong, 6].Formula = "SUM(F6:F" + (6 + d.Count() - 1) + ")";
            NumberFormat(6, 6, 6 + d.Count() + 1, 8, xlSheet);

            //end du lieu

            byte[] fileContents;
            fileContents = ExcelApp.GetAsByteArray();

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }
            string sFilename = "DoanhThuDoan_" + sgtcode + "_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: sFilename
            );
            //}
            //catch
            //{
            //    SetAlert("Lỗi định dạng ngày tháng", "error");
            //    return RedirectToAction("DoanTheoNgayDi");
            //}
        }

        /////////////////////////////////////// Tuyentq Theo ngay di ///////////////////////////////////////////////////////////////////////////
        public IActionResult TuyentqTheoNgayDi(string tungay = null, string denngay = null, string chiNhanh = null, string khoi = null)
        {
            var tuyentqTheoNgayDiVM = new TuyentqTheoNgayDiViewModel();

            var chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

            for (int i = 0; i < chiNhanhs.Count(); i++)
            {
                var cnToreturn = new ChiNhanhToReturnViewModel()
                {
                    Stt = i,
                    Name = chiNhanhs[i]
                };

                tuyentqTheoNgayDiVM.ChiNhanhToReturnViewModels.Add(cnToreturn);
            }

            tuyentqTheoNgayDiVM.KhoiViewModels = khoiViewModels();

            try
            {
                ViewBag.searchFromDate = tungay;
                ViewBag.searchToDate = denngay;
                chiNhanh = chiNhanh ?? "";
                ViewBag.chiNhanh = chiNhanh;
                ViewBag.khoi = khoi;

                if (tungay == null || denngay == null)
                {
                    return View("TuyentqTheoNgayDi", tuyentqTheoNgayDiVM);
                }

                var list = _unitOfWork.thongKeRepository.listTuyentqTheoNgayDi(tungay, denngay, chiNhanh, khoi);
                tuyentqTheoNgayDiVM.TuyentqNgaydis = list;
                return View(tuyentqTheoNgayDiVM);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return View("TuyentqTheoNgayDi", tuyentqTheoNgayDiVM);
            }
        }

        public IActionResult TuyentqTheoNgayDiPost(string tungay, string denngay, string chinhanh, string khoi)//(string tungay,string denngay, string daily)
        {
            if (tungay == null || denngay == null)
            {
                return RedirectToAction("TuyentqTheoNgayDi");
            }
            try
            {
                DateTime.Parse(tungay);
                DateTime.Parse(denngay);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return RedirectToAction("TuyentqTheoNgayDi");
            }
            //chinhanh = String.IsNullOrEmpty(chinhanh) ? Session["chinhanh"].ToString() : chinhanh;
            //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;

            chinhanh = chinhanh ?? "";
            string fromTo = "";
            ExcelPackage ExcelApp = new ExcelPackage();
            ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
            // Định dạng chiều dài cho cột
            xlSheet.Column(1).Width = 10;//STT
            xlSheet.Column(2).Width = 10;// chi nhanh
            xlSheet.Column(3).Width = 40;// tuyen tq
            xlSheet.Column(4).Width = 10;// sk ht
            xlSheet.Column(5).Width = 20;// doanh thu ht
            xlSheet.Column(6).Width = 10;// sk nam truoc
            xlSheet.Column(7).Width = 20;//doanh thu nam truoc
            xlSheet.Column(8).Width = 15;// ti le khach
            xlSheet.Column(9).Width = 15;// so sanh doanh thu

            xlSheet.Cells[2, 1].Value = "TUYẾN THAM QUAN THEO NGÀY ĐI TOUR " + chinhanh;
            xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
            xlSheet.Cells[2, 1, 2, 9].Merge = true;
            setCenterAligment(2, 1, 2, 9, xlSheet);
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
            xlSheet.Cells[3, 1, 3, 9].Merge = true;
            xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
            setCenterAligment(3, 1, 3, 9, xlSheet);

            // Tạo header
            // Tạo header

            xlSheet.Cells[5, 1].Value = "STT ";
            xlSheet.Cells[5, 1, 6, 1].Merge = true;
            xlSheet.Cells[5, 2].Value = "CN";
            xlSheet.Cells[5, 2, 6, 2].Merge = true;
            xlSheet.Cells[5, 3].Value = "Tuyến tham quan ";
            xlSheet.Cells[5, 3, 6, 3].Merge = true;

            xlSheet.Cells[5, 4].Value = "Thời điểm thống kê";
            xlSheet.Cells[5, 4, 5, 5].Merge = true;


            xlSheet.Cells[5, 6].Value = "So sánh cùng kỳ";
            xlSheet.Cells[5, 6, 5, 7].Merge = true;

            xlSheet.Cells[5, 8].Value = "Tỉ lệ % tăng giảm ";
            xlSheet.Cells[5, 8, 5, 9].Merge = true;
            // dong thu 2
            xlSheet.Cells[6, 4].Value = "Số khách";
            xlSheet.Cells[6, 5].Value = "Doanh số";
            xlSheet.Cells[6, 6].Value = "Số khách";
            xlSheet.Cells[6, 7].Value = "Doanh số";
            xlSheet.Cells[6, 8].Value = "Số khách";
            xlSheet.Cells[6, 9].Value = "Doanh số";
            setCenterAligment(5, 1, 6, 9, xlSheet);
            xlSheet.Cells[5, 1, 6, 9].Style.Font.SetFromFont(new Font("Times New Roman", 11, FontStyle.Bold));


            xlSheet.Cells[5, 1, 5, 9].Style.Font.SetFromFont(new Font("Times New Roman", 11, FontStyle.Bold));
            // do du lieu tu table
            int dong = 6;

            var d = _unitOfWork.thongKeRepository.listTuyentqTheoNgayDi(tungay, denngay, chinhanh, khoi).ToList();// Session["daily"].ToString(), Session["khoi"].ToString());

            var ranges = d.To2DArray(x => x.Stt, x => x.Chinhanh,
                                    x => x.Tuyentq, x => x.Khachht,
                                    x => x.Thucthuht, x => x.Khachcu,
                                    x => x.Thucthucu);

            //du lieu
            int iRowIndex = 6;
            int idem = 1;

            if (d != null)
            {
                //foreach (var vm in d)
                //{
                //    xlSheet.Cells[iRowIndex, 1].Value = idem;
                //    TrSetCellBorder(xlSheet, iRowIndex, 1, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                //    xlSheet.Cells[iRowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //    xlSheet.Cells[iRowIndex, 2].Value = vm.Chinhanh;
                //    TrSetCellBorder(xlSheet, iRowIndex, 2, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                //    xlSheet.Cells[iRowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //    xlSheet.Cells[iRowIndex, 3].Value = vm.Tuyentq;
                //    TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                //    xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //    xlSheet.Cells[iRowIndex, 4].Value = vm.Khachht;
                //    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                //    xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //    xlSheet.Cells[iRowIndex, 5].Value = vm.Thucthuht;
                //    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                //    xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //    xlSheet.Cells[iRowIndex, 6].Value = vm.Khachcu;
                //    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                //    xlSheet.Cells[iRowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //    xlSheet.Cells[iRowIndex, 7].Value = vm.Thucthucu;
                //    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                //    xlSheet.Cells[iRowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;



                //    iRowIndex += 1;
                //    idem += 1;
                //    dong++;
                //}

                for (int i = 0; i < d.Count(); i++)
                {
                    dong++;
                    for (int j = 0; j < 7; j++)
                    {
                        if (ranges[i, j] == null)
                        {
                            xlSheet.Cells[dong, j + 1].Value = 0;
                        }
                        else
                        {
                            xlSheet.Cells[dong, j + 1].Value = ranges[i, j];
                        }
                        var dong4 = (xlSheet.Cells[dong, 4].Value != null) ? xlSheet.Cells[dong, 4].Value.ToString() : "0";
                        var dong6 = (xlSheet.Cells[dong, 6].Value != null) ? xlSheet.Cells[dong, 6].Value.ToString() : "0";
                        if (dong4 == "0" || dong6 == "0")
                        {
                            xlSheet.Cells[dong, 8].Value = 0;
                        }
                        else
                        {
                            xlSheet.Cells[dong, 8].Formula = "=(" + (xlSheet.Cells[dong, 4]).Address + "-" + (xlSheet.Cells[dong, 6]).Address + ")/" + (xlSheet.Cells[dong, 6]).Address;
                        }
                        var dong5 = (xlSheet.Cells[dong, 5].Value != null) ? xlSheet.Cells[dong, 5].Value.ToString() : "0";
                        var dong7 = (xlSheet.Cells[dong, 7].Value != null) ? xlSheet.Cells[dong, 7].Value.ToString() : "0";
                        if (dong5 == "0" || dong7 == "0")
                        {
                            xlSheet.Cells[dong, 9].Value = 0;
                        }
                        else
                        {
                            xlSheet.Cells[dong, 9].Formula = "=(" + (xlSheet.Cells[dong, 5]).Address + "-" + (xlSheet.Cells[dong, 7]).Address + ")/" + (xlSheet.Cells[dong, 7]).Address;
                        }
                    }
                }
            }
            else
            {
                SetAlert("No sale.", "warning");
                return RedirectToAction(nameof(TuyentqTheoNgayDi));
            }

            dong++;
            // phan tram tong
            xlSheet.Cells[dong, 8].Formula = "=(" + (xlSheet.Cells[dong, 4]).Address + "-" + (xlSheet.Cells[dong, 6]).Address + ")/" + (xlSheet.Cells[dong, 6]).Address;
            xlSheet.Cells[dong, 9].Formula = "=(" + (xlSheet.Cells[dong, 5]).Address + "-" + (xlSheet.Cells[dong, 7]).Address + ")/" + (xlSheet.Cells[dong, 7]).Address;
            xlSheet.Cells[dong, 4].Formula = "SUM(D6:D" + (7 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 5].Formula = "SUM(E6:E" + (7 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 6].Formula = "SUM(F6:F" + (7 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 7].Formula = "SUM(G6:G" + (7 + d.Count() - 1) + ")";
            //xlSheet.Cells[dong, 8].Formula = "SUM(H6:H" + (7 + d.Count() - 1) + ")";
            //xlSheet.Cells[dong, 9].Formula = "SUM(I6:I" + (7 + d.Count() - 1) + ")";
            PhantramFormat(6, 8, 6 + d.Count() + 1, 9, xlSheet);
            // định dạng số
            NumberFormat(dong, 4, dong, 7, xlSheet);

            setBorder(5, 1, 5 + d.Count() + 2, 9, xlSheet);
            setFontBold(5, 1, 5, 5, 12, xlSheet);
            setFontSize(7, 1, 6 + d.Count() + 2, 9, 12, xlSheet);
            // dinh dang giu cho so khach
            setCenterAligment(7, 1, 7 + d.Count(), 2, xlSheet);
            setCenterAligment(7, 4, 7 + d.Count(), 4, xlSheet);
            setCenterAligment(7, 6, 7 + d.Count(), 6, xlSheet);
            setCenterAligment(7, 8, 7 + d.Count(), 9, xlSheet);
            // dinh dạng number cot sokhach, doanh so, thuc thu
            NumberFormat(7, 5, 7 + d.Count() + 1, 5, xlSheet);
            NumberFormat(7, 7, 6 + d.Count() + 1, 7, xlSheet);


            setBorder(dong, 4, dong, 9, xlSheet);
            setFontBold(dong, 4, dong, 9, 12, xlSheet);

            //xlSheet.View.FreezePanes(7, 20);

            //end du lieu

            byte[] fileContents;
            fileContents = ExcelApp.GetAsByteArray();

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }
            string sFilename = "DoanhThuTuyentq_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: sFilename
            );
        }

        public IActionResult TuyentqTheoNgayDiChiTietToExcel(string tungay, string denngay, string tuyentq, string chinhanh, string khoi)
        {
            try
            {
                //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;
                string fromTo = "";
                ExcelPackage ExcelApp = new ExcelPackage();
                ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
                // Định dạng chiều dài cho cột
                xlSheet.Column(1).Width = 10;//STT
                xlSheet.Column(2).Width = 10;// Chi nhánh
                xlSheet.Column(3).Width = 25;// Tuyến tq
                xlSheet.Column(4).Width = 25;// SGT Code
                xlSheet.Column(5).Width = 10;// Vetour Id
                xlSheet.Column(6).Width = 40;// Bắt đầu 
                xlSheet.Column(7).Width = 40;// Kết thúc
                xlSheet.Column(8).Width = 40;// Đại lý xuất vé
                xlSheet.Column(9).Width = 10;// Số khách
                xlSheet.Column(10).Width = 20;// Doanh thu


                xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH THU THEO NGÀY ĐI TUYẾN " + tuyentq;
                xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
                xlSheet.Cells[2, 1, 2, 10].Merge = true;
                setCenterAligment(2, 1, 2, 10, xlSheet);
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
                xlSheet.Cells[3, 1, 3, 10].Merge = true;
                xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
                setCenterAligment(3, 1, 3, 10, xlSheet);

                // Tạo header
                xlSheet.Cells[5, 1].Value = "STT";
                xlSheet.Cells[5, 2].Value = "Code CN";
                xlSheet.Cells[5, 3].Value = "Tuyến tq ";
                xlSheet.Cells[5, 4].Value = "SGT Code";
                xlSheet.Cells[5, 5].Value = "Vetour Id";
                xlSheet.Cells[5, 6].Value = "Bắt đầu";
                xlSheet.Cells[5, 7].Value = "Kết thúc";
                xlSheet.Cells[5, 8].Value = "Đại lý xuất vé";
                xlSheet.Cells[5, 9].Value = "Số khách";
                xlSheet.Cells[5, 10].Value = "Doanh thu";
                xlSheet.Cells[5, 1, 5, 10].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));

                int dong = 5;
                var d = _unitOfWork.thongKeRepository.TuyentqTheoNgayDiChiTietToExcel(tungay, denngay, chinhanh, tuyentq, khoi);// Session["fullName"].ToString());

                //du lieu
                int iRowIndex = 6;
                int idem = 1;

                if (d != null)
                {
                    foreach (var vm in d)
                    {
                        xlSheet.Cells[iRowIndex, 1].Value = idem;
                        TrSetCellBorder(xlSheet, iRowIndex, 1, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 2].Value = vm.chinhanh;
                        TrSetCellBorder(xlSheet, iRowIndex, 2, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 3].Value = vm.tuyentq;
                        TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 4].Value = vm.sgtcode;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 5].Value = vm.vetourid;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 6].Value = vm.batdau;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 7].Value = vm.ketthuc;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 8].Value = vm.dailyxuatve;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 9].Value = vm.sk;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 10].Value = vm.doanhthu;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        iRowIndex += 1;
                        idem += 1;
                        dong++;
                    }
                }
                else
                {
                    SetAlert("No sale.", "warning");
                    return RedirectToAction(nameof(TuyentqTheoNgayDi));
                }

                dong++;
                //// Merger cot 4,5 ghi tổng tiền
                //setRightAligment(dong, 3, dong, 3, xlSheet);
                //xlSheet.Cells[dong, 1, dong, 2].Merge = true;
                //xlSheet.Cells[dong, 1].Value = "Tổng tiền: ";
                // Sum tổng tiền
                xlSheet.Cells[dong, 9].Value = "TC";
                xlSheet.Cells[dong, 10].Formula = "SUM(J6:J" + (6 + d.Count() - 1) + ")";

                // định dạng số
                NumberFormat(dong, 10, dong, 10, xlSheet);
                setFontSize(6, 1, 6 + d.Count(), 10, 12, xlSheet);
                setBorder(5, 1, 5 + d.Count(), 10, xlSheet);
                setFontBold(5, 1, 5, 10, 12, xlSheet);

                // canh giưa cot stt
                setCenterAligment(6, 1, 6 + d.Count(), 2, xlSheet);

                setBorder(dong, 9, dong, 10, xlSheet);
                setFontBold(dong, 9, dong, 10, 12, xlSheet);
                // canh giưa cot ngay di va ngày ve
                setCenterAligment(6, 6, 6 + d.Count(), 7, xlSheet);
                // dinh dạng number cot gia ve
                NumberFormat(6, 10, 6 + d.Count(), 10, xlSheet);

                // dinh dang DateTime batdau ketthuc
                DateFormat(6, 6, 6 + d.Count(), 6, xlSheet);
                DateFormat(6, 7, 6 + d.Count(), 7, xlSheet);

                // xlSheet.View.FreezePanes(6, 20);

                //end du lieu

                byte[] fileContents;
                fileContents = ExcelApp.GetAsByteArray();

                if (fileContents == null || fileContents.Length == 0)
                {
                    return NotFound();
                }
                string sFilename = "DoanhThuQuayChitiet" + "_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

                return File(
                    fileContents: fileContents,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: sFilename
                );
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return RedirectToAction("QuayTheoNgayDi");
            }
        }

        /////////////////////////////////////// Tuyentq theo ngay quy///////////////////////////////////////////////////////////////////
        public IActionResult TuyentqTheoQuy()
        {
            var tuyentqTheoQuy = new TuyentqTheoQuyViewModel();

            var chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

            for (int i = 0; i < chiNhanhs.Count(); i++)
            {
                var cnToreturn = new ChiNhanhToReturnViewModel()
                {
                    Stt = i,
                    Name = chiNhanhs[i]
                };

                tuyentqTheoQuy.ChiNhanhToReturnViewModels.Add(cnToreturn);
            }

            tuyentqTheoQuy.KhoiViewModels = khoiViewModels();
            tuyentqTheoQuy.QuyViewModels = QuyViewModels();


            //ViewBag.searchFromDate = tungay;
            //ViewBag.searchToDate = denngay;
            //chiNhanh = chiNhanh ?? "";
            //ViewBag.chiNhanh = chiNhanh;
            //ViewBag.khoi = khoi;


            return View(tuyentqTheoQuy);

        }

        public IActionResult TuyentqTheoQuyPost(int quy, int nam, string chinhanh, string khoi)
        {
            //cn = String.IsNullOrEmpty(cn) ? Session["chinhanh"].ToString() : cn;
            //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;
            chinhanh = chinhanh ?? "";
            int thang = 1;
            switch (quy)
            {
                case 1:
                    thang = 1;
                    break;
                case 2:
                    thang = 4;
                    break;
                case 3:
                    thang = 7;
                    break;
                case 4:
                    thang = 10;
                    break;
                default:
                    thang = 1;
                    break;

            }

            ExcelPackage ExcelApp = new ExcelPackage();
            ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
            // Định dạng chiều dài cho cột
            xlSheet.Column(1).Width = 10;//stt
            xlSheet.Column(2).Width = 50;// tuyen tq
            xlSheet.Column(3).Width = 10;// sk thang 1 nam hien tai
            xlSheet.Column(4).Width = 15;// doanh so thang 1 nam hien tai
            xlSheet.Column(5).Width = 10;// sk thang 1 nam trươc
            xlSheet.Column(6).Width = 15;// doanh so thang 1 nam truoc

            xlSheet.Column(7).Width = 10;// sk thang 2 nam hien tai
            xlSheet.Column(8).Width = 15;// doanh so thang 2 nam hien tai
            xlSheet.Column(9).Width = 10;// sk thang 2 nam trươc
            xlSheet.Column(10).Width = 15;// doanh so thang 2 nam truoc

            xlSheet.Column(11).Width = 10;// sk thang 3 nam hien tai
            xlSheet.Column(12).Width = 15;// doanh so thang 3 nam hien tai
            xlSheet.Column(13).Width = 10;// sk thang 3 nam trươc
            xlSheet.Column(14).Width = 15;// doanh so thang 3 nam truoc

            xlSheet.Cells[2, 1].Value = "THỐNG KÊ TUYẾN TQ THEO QUÝ " + quy + " NĂM " + nam + " " + chinhanh;
            xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
            xlSheet.Cells[2, 1, 2, 14].Merge = true;
            setCenterAligment(2, 1, 2, 14, xlSheet);
            // dinh dang tu ngay den ngay

            //xlSheet.Cells[4, 1].Value = "";
            //xlSheet.Cells[3, 1, 3, 6].Merge = true;
            //xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
            //setCenterAligment(3, 1, 3, 6, xlSheet);

            // Tạo header
            xlSheet.Cells[4, 1, 6, 14].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));

            xlSheet.Cells[4, 1].Value = "STT";
            xlSheet.Cells[4, 1, 6, 1].Merge = true;
            xlSheet.Cells[4, 2].Value = "Tuyến tham quan ";
            xlSheet.Cells[4, 2, 6, 2].Merge = true;
            // thang thứ nhất
            xlSheet.Cells[4, 3].Value = "Tháng " + thang;
            xlSheet.Cells[4, 3, 4, 6].Merge = true;
            // nam hiên tại của tháng thứ nhất
            xlSheet.Cells[5, 3].Value = nam;
            xlSheet.Cells[5, 3, 5, 4].Merge = true;
            // năm trước đó của tháng thứ nhất
            xlSheet.Cells[5, 5].Value = (nam - 1).ToString();
            xlSheet.Cells[5, 5, 5, 6].Merge = true;
            xlSheet.Cells[6, 3].Value = "SK";
            xlSheet.Cells[6, 4].Value = "Doanh số";
            // so khach va doanh so năm trước tháng 1
            xlSheet.Cells[6, 5].Value = "SK";
            xlSheet.Cells[6, 6].Value = "Doanh số";

            // thang thứ hai
            xlSheet.Cells[4, 7].Value = "Tháng " + (thang + 1).ToString();
            xlSheet.Cells[4, 7, 4, 10].Merge = true;
            // nam hiên tại của tháng thứ hai
            xlSheet.Cells[5, 7].Value = nam;
            xlSheet.Cells[5, 7, 5, 8].Merge = true;
            // năm trước đó của tháng thứ hai
            xlSheet.Cells[5, 9].Value = (nam - 1).ToString();
            xlSheet.Cells[5, 9, 5, 10].Merge = true;
            xlSheet.Cells[6, 7].Value = "SK";
            xlSheet.Cells[6, 8].Value = "Doanh số";
            // so khach va doanh so năm trước tháng 1
            xlSheet.Cells[6, 9].Value = "SK";
            xlSheet.Cells[6, 10].Value = "Doanh số";


            // thang thứ ba
            xlSheet.Cells[4, 11].Value = "Tháng " + (thang + 2).ToString();
            xlSheet.Cells[4, 11, 4, 14].Merge = true;
            // nam hiên tại của tháng thứ ba
            xlSheet.Cells[5, 11].Value = nam;
            xlSheet.Cells[5, 11, 5, 12].Merge = true;
            // năm trước đó của tháng thứ nhất
            xlSheet.Cells[5, 13].Value = (nam - 1).ToString();
            xlSheet.Cells[5, 13, 5, 14].Merge = true;
            xlSheet.Cells[6, 11].Value = "SK";
            xlSheet.Cells[6, 12].Value = "Doanh số";
            // so khach va doanh so năm trước tháng 1
            xlSheet.Cells[6, 13].Value = "SK";
            xlSheet.Cells[6, 14].Value = "Doanh số";
            // canh giữa cho tiêu đề bảng
            setCenterAligment(4, 1, 6, 14, xlSheet);

            // do du lieu tu table
            int dong = 6;

            var d = _unitOfWork.thongKeRepository.TuyenTqTheoQuyToExcel(quy, nam, chinhanh, khoi).ToList();// Session["daily"].ToString(), Session["khoi"].ToString());

            
            //du lieu
            int iRowIndex = 7;
            int idem = 1;

            if (d != null)
            {
                foreach (var vm in d)
                {
                    xlSheet.Cells[iRowIndex, 1].Value = idem;
                    TrSetCellBorder(xlSheet, iRowIndex, 1, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 2].Value = vm.Tuyentq;
                    TrSetCellBorder(xlSheet, iRowIndex, 2, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 3].Value = vm.Sk1;
                    TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 4].Value = vm.Doanhso1;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 5].Value = vm.Sk11;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 6].Value = vm.Doanhso11;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 7].Value = vm.Sk2;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    
                    xlSheet.Cells[iRowIndex, 8].Value = vm.Doanhso2;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    
                    xlSheet.Cells[iRowIndex, 9].Value = vm.Sk21;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    
                    xlSheet.Cells[iRowIndex, 10].Value = vm.Doanhso21;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    
                    xlSheet.Cells[iRowIndex, 11].Value = vm.Sk3;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 11].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    
                    xlSheet.Cells[iRowIndex, 12].Value = vm.Doanhso3;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 12].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    
                    xlSheet.Cells[iRowIndex, 13].Value = vm.Sk31;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 13].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    
                    xlSheet.Cells[iRowIndex, 14].Value = vm.Doanhso31;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 14].Style.Border.Right.Style = ExcelBorderStyle.Thin;



                    iRowIndex += 1;
                    idem += 1;
                    dong++;
                }


            }
            else
            {
                SetAlert("No sale.", "warning");
                return RedirectToAction(nameof(TuyentqTheoQuy));
            }

            dong++;


            setBorder(4, 1, 4 + d.Count() + 2, 14, xlSheet);
            setFontSize(7, 1, 7 + d.Count(), 14, 11, xlSheet);

            // định dạng number cho cột doanh số
            NumberFormat(7, 3, 7 + d.Count() + 1, 14, xlSheet);

            // canh giua cot stt
            setCenterAligment(7, 1, 7 + d.Count(), 1, xlSheet);
            // canh giua so khach thang 1
            setCenterAligment(7, 3, 7 + d.Count(), 3, xlSheet);
            setCenterAligment(7, 5, 7 + d.Count(), 5, xlSheet);
            setCenterAligment(7, 7, 7 + d.Count(), 7, xlSheet);
            setCenterAligment(7, 9, 7 + d.Count(), 9, xlSheet);
            setCenterAligment(7, 11, 7 + d.Count(), 11, xlSheet);
            setCenterAligment(7, 13, 7 + d.Count(), 13, xlSheet);
            //
            xlSheet.Cells[dong, 3].Formula = "SUM(C7:C" + (7 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 4].Formula = "SUM(D7:D" + (7 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 5].Formula = "SUM(E7:E" + (7 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 6].Formula = "SUM(F7:F" + (7 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 7].Formula = "SUM(G7:G" + (7 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 8].Formula = "SUM(H7:H" + (7 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 9].Formula = "SUM(I7:I" + (7 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 10].Formula = "SUM(J7:J" + (7 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 11].Formula = "SUM(K7:K" + (7 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 12].Formula = "SUM(L7:L" + (7 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 13].Formula = "SUM(M7:M" + (7 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 14].Formula = "SUM(N7:N" + (7 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 3, dong, 14].Style.Font.SetFromFont(new Font("Times New Roman", 11, FontStyle.Bold));
            setBorder(dong, 3, dong, 14, xlSheet);
            //xlSheet.View.FreezePanes(7, 20);

            //end du lieu

            byte[] fileContents;
            fileContents = ExcelApp.GetAsByteArray();

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }
            string sFilename = "DoanhThuTuyentq_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: sFilename
            );
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpGet]
        public JsonResult GetAllTuyentqByKhoi(string khoi)
        {
            var model = _unitOfWork.accountRepository.GetAllTuyentqByKhoi(khoi);
            //var viewModel = Mapper.Map<IEnumerable<chinhanh>, IEnumerable<chinhanhViewModel>>(model);
            return Json(new
            {
                data = JsonConvert.SerializeObject(model)
            });
        }

        public List<KhoiViewModel> khoiViewModels()
        {
            return new List<KhoiViewModel>()
            {
                new KhoiViewModel() { Id = 1, Name = "OB" },
                new KhoiViewModel() { Id = 2, Name = "ND" }
            };
        }

        public List<QuyViewModel> QuyViewModels()
        {
            return new List<QuyViewModel>()
            {
                new QuyViewModel() { Id = 1, Name = "Qúy 1" },
                new QuyViewModel() { Id = 2, Name = "Qúy 2" },
                new QuyViewModel() { Id = 3, Name = "Qúy 3" },
                new QuyViewModel() { Id = 4, Name = "Qúy 4" }
            };
        }

        protected void SetAlert(string message, string type)
        {
            TempData["AlertMessage"] = message;
            if (type == "success")
            {
                TempData["AlertType"] = "alert-success";
            }
            else if (type == "warning")
            {
                TempData["AlertType"] = "alert-warning";
            }
            else if (type == "error")
            {
                TempData["AlertType"] = "alert-error";
            }
        }




        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void NumberFormat(int fromRow, int fromColumn, int toRow, int toColumn, ExcelWorksheet sheet)
        {
            using (var range = sheet.Cells[fromRow, fromColumn, toRow, toColumn])
            {
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                range.Style.Numberformat.Format = "#,#0";
            }
        }

        private static void DateFormat(int fromRow, int fromColumn, int toRow, int toColumn, ExcelWorksheet sheet)
        {
            using (var range = sheet.Cells[fromRow, fromColumn, toRow, toColumn])
            {
                range.Style.Numberformat.Format = "dd/MM/yyyy";
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
        }

        private static void DateTimeFormat(int fromRow, int fromColumn, int toRow, int toColumn, ExcelWorksheet sheet)
        {
            using (var range = sheet.Cells[fromRow, fromColumn, toRow, toColumn])
            {
                range.Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
        }

        private static void setRightAligment(int fromRow, int fromColumn, int toRow, int toColumn, ExcelWorksheet sheet)
        {
            using (var range = sheet.Cells[fromRow, fromColumn, toRow, toColumn])
            {
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            }
        }

        private static void setCenterAligment(int fromRow, int fromColumn, int toRow, int toColumn, ExcelWorksheet sheet)
        {
            using (var range = sheet.Cells[fromRow, fromColumn, toRow, toColumn])
            {
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
        }

        private static void setFontSize(int fromRow, int fromColumn, int toRow, int toColumn, int fontSize, ExcelWorksheet sheet)
        {
            using (var range = sheet.Cells[fromRow, fromColumn, toRow, toColumn])
            {
                range.Style.Font.SetFromFont(new Font("Times New Roman", fontSize, FontStyle.Regular));
            }
        }

        private static void setFontBold(int fromRow, int fromColumn, int toRow, int toColumn, int fontSize, ExcelWorksheet sheet)
        {
            using (var range = sheet.Cells[fromRow, fromColumn, toRow, toColumn])
            {
                range.Style.Font.SetFromFont(new Font("Times New Roman", fontSize, FontStyle.Bold));
            }
        }

        private static void setBorder(int fromRow, int fromColumn, int toRow, int toColumn, ExcelWorksheet sheet)
        {
            using (var range = sheet.Cells[fromRow, fromColumn, toRow, toColumn])
            {
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }
        }

        private static void setBorderAround(int fromRow, int fromColumn, int toRow, int toColumn, ExcelWorksheet sheet)
        {
            using (var range = sheet.Cells[fromRow, fromColumn, toRow, toColumn])
            {
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }
        }

        private static void PhantramFormat(int fromRow, int fromColumn, int toRow, int toColumn, ExcelWorksheet sheet)
        {
            using (var range = sheet.Cells[fromRow, fromColumn, toRow, toColumn])
            {
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.Numberformat.Format = "0 %";
            }
        }

        private static void mergercell(int fromRow, int fromColumn, int toRow, int toColumn, ExcelWorksheet sheet)
        {
            using (var range = sheet.Cells[fromRow, fromColumn, toRow, toColumn])
            {
                range.Merge = true;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                range.Style.WrapText = true;
            }
        }

        private static void numberMergercell(int fromRow, int fromColumn, int toRow, int toColumn, ExcelWorksheet sheet)
        {
            using (var range = sheet.Cells[fromRow, fromColumn, toRow, toColumn])
            {
                var a = sheet.Cells[fromRow, fromColumn].Value;
                range.Merge = true;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                range.Clear();
                sheet.Cells[fromRow, fromColumn].Value = a;
            }
        }

        private static void wrapText(int fromRow, int fromColumn, int toRow, int toColumn, ExcelWorksheet sheet)
        {
            using (var range = sheet.Cells[fromRow, fromColumn, toRow, toColumn])
            {
                range.Style.WrapText = true;
            }
        }

        public static string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        ///////////////// new ///////////////////
        ///
        public void TrSetCellBorder(ExcelWorksheet xlSheet, int iRowIndex, int colIndex, ExcelBorderStyle excelBorderStyle, ExcelHorizontalAlignment excelHorizontalAlignment, Color borderColor, string fontName, int fontSize, FontStyle fontStyle)
        {
            xlSheet.Cells[iRowIndex, colIndex].Style.HorizontalAlignment = excelHorizontalAlignment;
            // Set Border
            xlSheet.Cells[iRowIndex, colIndex].Style.Border.Left.Style = excelBorderStyle;
            xlSheet.Cells[iRowIndex, colIndex].Style.Border.Top.Style = excelBorderStyle;
            xlSheet.Cells[iRowIndex, colIndex].Style.Border.Right.Style = excelBorderStyle;
            xlSheet.Cells[iRowIndex, colIndex].Style.Border.Bottom.Style = excelBorderStyle;
            // Set màu ch Border
            //xlSheet.Cells[iRowIndex, colIndex].Style.Border.Left.Color.SetColor(borderColor);
            //xlSheet.Cells[iRowIndex, colIndex].Style.Border.Top.Color.SetColor(borderColor);
            //xlSheet.Cells[iRowIndex, colIndex].Style.Border.Right.Color.SetColor(borderColor);
            //xlSheet.Cells[iRowIndex, colIndex].Style.Border.Bottom.Color.SetColor(borderColor);

            // Set Font cho text  trong Range hiện tại
            xlSheet.Cells[iRowIndex, colIndex].Style.Font.SetFromFont(new Font(fontName, fontSize, fontStyle));
        }
    }
}