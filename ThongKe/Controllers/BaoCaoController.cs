using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ThongKe.Data.DTOs;
using ThongKe.Data.Models;
using ThongKe.Data.Models_QLTour;
using ThongKe.Data.Repository;
using ThongKe.Helps;
using ThongKe.Models;
using ThongKe.Models.TourTheoNgay;
using ThongKe.Services;

namespace ThongKe.Controllers
{
    public class BaoCaoController : BaseController
    {
        //Users user;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaoCaoService _baoCaoService;

        [BindProperty]
        public BaoCaoViewModel BaoCaoVM { get; set; }

        public BaoCaoController(IUnitOfWork unitOfWork, IBaoCaoService baoCaoService)
        {
            _unitOfWork = unitOfWork;
            _baoCaoService = baoCaoService;
            BaoCaoVM = new BaoCaoViewModel()
            {
                TourTheoNgay_IB = new Models.TourTheoNgay.TourTheoNgay_IB(),
                TourTheoNgay_ND = new Models.TourTheoNgay.TourTheoNgay_ND(),
                TourTheoNgay_OB = new Models.TourTheoNgay.TourTheoNgay_OB()
            };

        }

        public IActionResult Index()
        {
            return View();
        }

        /////////////////////////////////////// Sale theo quay ///////////////////////////////////////////////////////////////////
        public IActionResult SaleTheoQuay(string tungay = null, string denngay = null, string chiNhanh = null, string khoi = null)
        {
            var user = HttpContext.Session.Get<Users>("loginUser");
            var dtSaleQuayVM = new DoanhthuSaleQuayViewModel();

            dtSaleQuayVM.TuNgay = tungay;
            dtSaleQuayVM.DenNgay = denngay;
            dtSaleQuayVM.Khoi = khoi;

            string[] chiNhanhs = null;
            if (user.Nhom != "Users")
            {
                if (user.Nhom != "Admins")
                {
                    chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Where(x => x.Nhom == user.Nhom).Select(x => x.Chinhanh1).Distinct().ToArray();

                }
                else
                {
                    chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

                }
                for (int i = 0; i < chiNhanhs.Count(); i++)
                {
                    var cnToreturn = new ChiNhanhToReturnViewModel()
                    {
                        Stt = i,
                        Name = chiNhanhs[i]
                    };

                    dtSaleQuayVM.chiNhanhToReturnViewModels.Add(cnToreturn);
                }
                dtSaleQuayVM.KhoiViewModels_KL = KhoiViewModels_KL();
            }
            else
            {
                dtSaleQuayVM.chiNhanhToReturnViewModels.Add(new ChiNhanhToReturnViewModel() { Stt = 1, Name = user.Chinhanh });
                dtSaleQuayVM.KhoiViewModels_KL = KhoiViewModels_KL().Where(x => x.Name.Equals(user.Khoi)).ToList();

            }


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
            var user = HttpContext.Session.Get<Users>("loginUser");
            var dtSaleQuayVM = new DoanhthuSaleQuayViewModel();
            dtSaleQuayVM.TuNgay = tungay;
            dtSaleQuayVM.DenNgay = denngay;
            dtSaleQuayVM.Khoi = khoi;
            string[] chiNhanhs = null;
            if (user.Nhom != "Users")
            {
                if (user.Nhom != "Admins")
                {
                    chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Where(x => x.Nhom == user.Nhom).Select(x => x.Chinhanh1).Distinct().ToArray();

                }
                else
                {
                    chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

                }
                for (int i = 0; i < chiNhanhs.Count(); i++)
                {
                    var cnToreturn = new ChiNhanhToReturnViewModel()
                    {
                        Stt = i,
                        Name = chiNhanhs[i]
                    };

                    dtSaleQuayVM.chiNhanhToReturnViewModels.Add(cnToreturn);
                }
                dtSaleQuayVM.KhoiViewModels_KL = KhoiViewModels_KL();
            }
            else
            {
                dtSaleQuayVM.chiNhanhToReturnViewModels.Add(new ChiNhanhToReturnViewModel() { Stt = 1, Name = user.Chinhanh });
                dtSaleQuayVM.KhoiViewModels_KL = KhoiViewModels_KL().Where(x => x.Name.Equals(user.Khoi)).ToList();

            }

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
                    return RedirectToAction(nameof(SaleTheoNgayDi));
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
                return RedirectToAction("SaleTheoNgayDi");
            }
        }

        /////////////////////////////////////// Sale Theo Tuyen Tham Quan ///////////////////////////////////////////////////////////////
        public IActionResult SaleTheoTuyenThamQuan(string tungay = null, string denngay = null, string tuyentq = null, string khoi = null)
        {
            ViewBag.searchFromDate = tungay;
            ViewBag.searchToDate = denngay;
            ViewBag.ttq = tuyentq;
            ViewBag.khoi = khoi;

            var user = HttpContext.Session.Get<Users>("loginUser");
            var dtSaleTuyenVM = new DoanhThuSaleTuyenViewModel();
            dtSaleTuyenVM.TuNgay = tungay;
            dtSaleTuyenVM.DenNgay = denngay;
            dtSaleTuyenVM.Khoi = khoi;
            tuyentq = string.IsNullOrEmpty(tuyentq) ? "" : tuyentq.Trim();

            if (user.Nhom != "Users")
            {
                dtSaleTuyenVM.KhoiViewModels_KL = KhoiViewModels_KL();
                dtSaleTuyenVM.tuyenThamQuanViewModels = _unitOfWork.userRepository.GetAllTuyentqByKhoi("OB");
            }

            else
            {
                dtSaleTuyenVM.KhoiViewModels_KL = KhoiViewModels_KL().Where(x => x.Name.Equals(user.Khoi)).ToList();
                dtSaleTuyenVM.tuyenThamQuanViewModels = _unitOfWork.userRepository.GetAllTuyentqByKhoi(user.Khoi);
            }
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
            //var dtQuayTheoNgayBanVM = new DoanthuQuayNgayBanViewModel();

            //var chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

            //for (int i = 0; i < chiNhanhs.Count(); i++)
            //{
            //    var cnToreturn = new ChiNhanhToReturnViewModel()
            //    {
            //        Stt = i,
            //        Name = chiNhanhs[i]
            //    };

            //    dtQuayTheoNgayBanVM.chiNhanhToReturnViewModels.Add(cnToreturn);
            //}
            //dtQuayTheoNgayBanVM.KhoiViewModels_KL = KhoiViewModels_KL();

            var user = HttpContext.Session.Get<Users>("loginUser");
            var dtQuayTheoNgayBanVM = new DoanthuQuayNgayBanViewModel();
            dtQuayTheoNgayBanVM.TuNgay = tungay;
            dtQuayTheoNgayBanVM.DenNgay = denngay;
            dtQuayTheoNgayBanVM.Khoi = khoi;
            string[] chiNhanhs = null;
            if (user.Nhom != "Users")
            {
                if (user.Nhom != "Admins")
                {
                    chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Where(x => x.Nhom == user.Nhom).Select(x => x.Chinhanh1).Distinct().ToArray();

                }
                else
                {
                    chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

                }
                for (int i = 0; i < chiNhanhs.Count(); i++)
                {
                    var cnToreturn = new ChiNhanhToReturnViewModel()
                    {
                        Stt = i,
                        Name = chiNhanhs[i]
                    };

                    dtQuayTheoNgayBanVM.chiNhanhToReturnViewModels.Add(cnToreturn);
                }
                dtQuayTheoNgayBanVM.KhoiViewModels_KL = KhoiViewModels_KL();
            }
            else
            {
                dtQuayTheoNgayBanVM.chiNhanhToReturnViewModels.Add(new ChiNhanhToReturnViewModel() { Stt = 1, Name = user.Chinhanh });
                dtQuayTheoNgayBanVM.KhoiViewModels_KL = KhoiViewModels_KL().Where(x => x.Name.Equals(user.Khoi)).ToList();

            }

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
            //var dtQuayTheoNgayDiVM = new DoanthuQuayNgayBanViewModel();

            //var chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

            //for (int i = 0; i < chiNhanhs.Count(); i++)
            //{
            //    var cnToreturn = new ChiNhanhToReturnViewModel()
            //    {
            //        Stt = i,
            //        Name = chiNhanhs[i]
            //    };

            //    dtQuayTheoNgayDiVM.chiNhanhToReturnViewModels.Add(cnToreturn);
            //}

            //dtQuayTheoNgayDiVM.KhoiViewModels_KL = KhoiViewModels_KL();

            var user = HttpContext.Session.Get<Users>("loginUser");
            var dtQuayTheoNgayDiVM = new DoanthuQuayNgayBanViewModel();
            dtQuayTheoNgayDiVM.TuNgay = tungay;
            dtQuayTheoNgayDiVM.DenNgay = denngay;
            dtQuayTheoNgayDiVM.Khoi = khoi;
            string[] chiNhanhs = null;
            if (user.Nhom != "Users")
            {
                if (user.Nhom != "Admins")
                {
                    chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Where(x => x.Nhom == user.Nhom).Select(x => x.Chinhanh1).Distinct().ToArray();

                }
                else
                {
                    chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

                }
                for (int i = 0; i < chiNhanhs.Count(); i++)
                {
                    var cnToreturn = new ChiNhanhToReturnViewModel()
                    {
                        Stt = i,
                        Name = chiNhanhs[i]
                    };

                    dtQuayTheoNgayDiVM.chiNhanhToReturnViewModels.Add(cnToreturn);
                }
                dtQuayTheoNgayDiVM.KhoiViewModels_KL = KhoiViewModels_KL();
            }
            else
            {
                dtQuayTheoNgayDiVM.chiNhanhToReturnViewModels.Add(new ChiNhanhToReturnViewModel() { Stt = 1, Name = user.Chinhanh });
                dtQuayTheoNgayDiVM.KhoiViewModels_KL = KhoiViewModels_KL().Where(x => x.Name.Equals(user.Khoi)).ToList();

            }

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
            //var doanTheoNgayDiVM = new DoanTheoNgayDiViewModel();

            //var chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

            //for (int i = 0; i < chiNhanhs.Count(); i++)
            //{
            //    var cnToreturn = new ChiNhanhToReturnViewModel()
            //    {
            //        Stt = i,
            //        Name = chiNhanhs[i]
            //    };

            //    doanTheoNgayDiVM.chiNhanhToReturnViewModels.Add(cnToreturn);
            //}

            //doanTheoNgayDiVM.KhoiViewModels_KL = KhoiViewModels_KL();

            var user = HttpContext.Session.Get<Users>("loginUser");
            var doanTheoNgayDiVM = new DoanTheoNgayDiViewModel();
            doanTheoNgayDiVM.TuNgay = tungay;
            doanTheoNgayDiVM.DenNgay = denngay;
            doanTheoNgayDiVM.Khoi = khoi;
            string[] chiNhanhs = null;
            if (user.Nhom != "Users")
            {
                if (user.Nhom != "Admins")
                {
                    chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Where(x => x.Nhom == user.Nhom).Select(x => x.Chinhanh1).Distinct().ToArray();

                }
                else
                {
                    chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

                }
                for (int i = 0; i < chiNhanhs.Count(); i++)
                {
                    var cnToreturn = new ChiNhanhToReturnViewModel()
                    {
                        Stt = i,
                        Name = chiNhanhs[i]
                    };

                    doanTheoNgayDiVM.chiNhanhToReturnViewModels.Add(cnToreturn);
                }
                doanTheoNgayDiVM.KhoiViewModels_KL = KhoiViewModels_KL();
            }
            else
            {
                doanTheoNgayDiVM.chiNhanhToReturnViewModels.Add(new ChiNhanhToReturnViewModel() { Stt = 1, Name = user.Chinhanh });
                doanTheoNgayDiVM.KhoiViewModels_KL = KhoiViewModels_KL().Where(x => x.Name.Equals(user.Khoi)).ToList();

            }

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
            //var tuyentqTheoNgayDiVM = new TuyentqTheoNgayDiViewModel();

            //var chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

            //for (int i = 0; i < chiNhanhs.Count(); i++)
            //{
            //    var cnToreturn = new ChiNhanhToReturnViewModel()
            //    {
            //        Stt = i,
            //        Name = chiNhanhs[i]
            //    };

            //    tuyentqTheoNgayDiVM.ChiNhanhToReturnViewModels.Add(cnToreturn);
            //}

            //tuyentqTheoNgayDiVM.KhoiViewModels_KL = KhoiViewModels_KL();

            var user = HttpContext.Session.Get<Users>("loginUser");
            var tuyentqTheoNgayDiVM = new TuyentqTheoNgayDiViewModel();
            tuyentqTheoNgayDiVM.TuNgay = tungay;
            tuyentqTheoNgayDiVM.DenNgay = denngay;
            tuyentqTheoNgayDiVM.Khoi = khoi;
            string[] chiNhanhs = null;
            if (user.Nhom != "Users")
            {
                if (user.Nhom != "Admins")
                {
                    chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Where(x => x.Nhom == user.Nhom).Select(x => x.Chinhanh1).Distinct().ToArray();

                }
                else
                {
                    chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

                }
                for (int i = 0; i < chiNhanhs.Count(); i++)
                {
                    var cnToreturn = new ChiNhanhToReturnViewModel()
                    {
                        Stt = i,
                        Name = chiNhanhs[i]
                    };

                    tuyentqTheoNgayDiVM.ChiNhanhToReturnViewModels.Add(cnToreturn);
                }
                tuyentqTheoNgayDiVM.KhoiViewModels_KL = KhoiViewModels_KL();
            }
            else
            {
                tuyentqTheoNgayDiVM.ChiNhanhToReturnViewModels.Add(new ChiNhanhToReturnViewModel() { Stt = 1, Name = user.Chinhanh });
                tuyentqTheoNgayDiVM.KhoiViewModels_KL = KhoiViewModels_KL().Where(x => x.Name.Equals(user.Khoi)).ToList();

            }


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
            //var tuyentqTheoQuy = new TuyentqTheoQuyViewModel();

            //var chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

            //for (int i = 0; i < chiNhanhs.Count(); i++)
            //{
            //    var cnToreturn = new ChiNhanhToReturnViewModel()
            //    {
            //        Stt = i,
            //        Name = chiNhanhs[i]
            //    };

            //    tuyentqTheoQuy.ChiNhanhToReturnViewModels.Add(cnToreturn);
            //}

            //tuyentqTheoQuy.KhoiViewModels_KL = KhoiViewModels_KL();
            //tuyentqTheoQuy.QuyViewModels = QuyViewModels();

            var user = HttpContext.Session.Get<Users>("loginUser");
            var tuyentqTheoQuy = new TuyentqTheoQuyViewModel();
            string[] chiNhanhs = null;
            if (user.Nhom != "Users")
            {
                if (user.Nhom != "Admins")
                {
                    chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Where(x => x.Nhom == user.Nhom).Select(x => x.Chinhanh1).Distinct().ToArray();

                }
                else
                {
                    chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

                }
                for (int i = 0; i < chiNhanhs.Count(); i++)
                {
                    var cnToreturn = new ChiNhanhToReturnViewModel()
                    {
                        Stt = i,
                        Name = chiNhanhs[i]
                    };

                    tuyentqTheoQuy.ChiNhanhToReturnViewModels.Add(cnToreturn);
                }
                tuyentqTheoQuy.KhoiViewModels_KL = KhoiViewModels_KL();
            }
            else
            {
                tuyentqTheoQuy.ChiNhanhToReturnViewModels.Add(new ChiNhanhToReturnViewModel() { Stt = 1, Name = user.Chinhanh });
                tuyentqTheoQuy.KhoiViewModels_KL = KhoiViewModels_KL().Where(x => x.Name.Equals(user.Khoi)).ToList();

            }

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

        /////////////////////////////////////// Khach le he thong ///////////////////////////////////////////////////////////////////////////
        public IActionResult KhachLeHeThong(string tungay = null, string denngay = null, string chiNhanh = null, string khoi = null)
        {
            //var khachLeHeThongVM = new KhachLeHeThongViewModel();

            //var chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

            //for (int i = 0; i < chiNhanhs.Count(); i++)
            //{
            //    var cnToreturn = new ChiNhanhToReturnViewModel()
            //    {
            //        Stt = i,
            //        Name = chiNhanhs[i]
            //    };

            //    khachLeHeThongVM.ChiNhanhToReturnViewModels.Add(cnToreturn);
            //}

            //khachLeHeThongVM.KhoiViewModels_KL = KhoiViewModels_KL();

            var user = HttpContext.Session.Get<Users>("loginUser");
            var khachLeHeThongVM = new KhachLeHeThongViewModel();
            string[] chiNhanhs = null;
            if (user.Nhom != "Users")
            {
                if (user.Nhom != "Admins")
                {
                    chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Where(x => x.Nhom == user.Nhom).Select(x => x.Chinhanh1).Distinct().ToArray();

                }
                else
                {
                    chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

                }
                for (int i = 0; i < chiNhanhs.Count(); i++)
                {
                    var cnToreturn = new ChiNhanhToReturnViewModel()
                    {
                        Stt = i,
                        Name = chiNhanhs[i]
                    };

                    khachLeHeThongVM.ChiNhanhToReturnViewModels.Add(cnToreturn);
                }
                khachLeHeThongVM.KhoiViewModels_KL = KhoiViewModels_KL();
            }
            else
            {
                khachLeHeThongVM.ChiNhanhToReturnViewModels.Add(new ChiNhanhToReturnViewModel() { Stt = 1, Name = user.Chinhanh });
                khachLeHeThongVM.KhoiViewModels_KL = KhoiViewModels_KL().Where(x => x.Name.Equals(user.Khoi)).ToList();

            }

            try
            {
                ViewBag.searchFromDate = tungay;
                ViewBag.searchToDate = denngay;
                chiNhanh = chiNhanh ?? "";
                ViewBag.chiNhanh = chiNhanh;
                ViewBag.khoi = khoi;

                if (tungay == null || denngay == null)
                {
                    return View("KhachLeHeThong", khachLeHeThongVM);
                }

                var list = _unitOfWork.thongKeRepository.listKhachLeHeThong(tungay, denngay, chiNhanh, khoi);
                khachLeHeThongVM.DoanhthuToanhethongs = list;
                return View(khachLeHeThongVM);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return View("KhachLeHeThong", khachLeHeThongVM);
            }
        }

        public IActionResult KhachLeHeThongPost(string tungay, string denngay, string chinhanh, string khoi)//(string tungay,string denngay, string daily)
        {
            if (tungay == null || denngay == null)
            {
                return RedirectToAction("KhachLeHeThong");
            }
            try
            {
                DateTime.Parse(tungay);
                DateTime.Parse(denngay);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return RedirectToAction("KhachLeHeThong");
            }
            //chinhanh = String.IsNullOrEmpty(chinhanh) ? Session["chinhanh"].ToString() : chinhanh;
            //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;

            chinhanh = chinhanh ?? "";
            string fromTo = "";
            ExcelPackage ExcelApp = new ExcelPackage();
            ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("lienketkhachle");
            // Định dạng chiều dài cho cột
            xlSheet.Column(1).Width = 10;//STT
            xlSheet.Column(2).Width = 10;// cn
            xlSheet.Column(3).Width = 40;// quay
            xlSheet.Column(4).Width = 10;// so khach hien tai
            xlSheet.Column(5).Width = 20;// doanh số hien tai
            xlSheet.Column(6).Width = 10;// so khach nam truoc
            xlSheet.Column(7).Width = 20; // doanh thu nam truoc
            xlSheet.Column(8).Width = 15; // ti le so khach
            xlSheet.Column(9).Width = 15;// doanh thu so sanh

            xlSheet.Cells[2, 1].Value = "LIÊN KẾT QUẦY KHÁCH LẼ HỆ THỐNG " + khoi + "  " + chinhanh;
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

            xlSheet.Cells[5, 1].Value = "STT ";
            xlSheet.Cells[5, 1, 6, 1].Merge = true;
            xlSheet.Cells[5, 2].Value = "CN";
            xlSheet.Cells[5, 2, 6, 2].Merge = true;
            xlSheet.Cells[5, 3].Value = "Văn phòng xuất vé ";
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
            xlSheet.Cells[5, 1, 6, 9].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));

            // do du lieu tu table
            int dong = 6;

            var d = _unitOfWork.thongKeRepository.listKhachLeHeThong(tungay, denngay, chinhanh, khoi).ToList();// Session["daily"].ToString(), Session["khoi"].ToString());

            var ranges = d.To2DArray(x => x.Stt, x => x.Chinhanh,
                                    x => x.Dailyxuatve, x => x.Khachht,
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
                return RedirectToAction(nameof(KhachLeHeThong));
            }

            dong++;
            // phan tram tong
            xlSheet.Cells[dong, 8].Formula = "=(" + (xlSheet.Cells[dong, 4]).Address + "-" + (xlSheet.Cells[dong, 6]).Address + ")/" + (xlSheet.Cells[dong, 6]).Address;
            xlSheet.Cells[dong, 9].Formula = "=(" + (xlSheet.Cells[dong, 5]).Address + "-" + (xlSheet.Cells[dong, 7]).Address + ")/" + (xlSheet.Cells[dong, 7]).Address;
            //// Merger cot 4,5 ghi tổng tiền
            //setRightAligment(dong, 3, dong, 3, xlSheet);
            //xlSheet.Cells[dong, 1, dong, 2].Merge = true;
            //xlSheet.Cells[dong, 1].Value = "Tổng tiền: ";
            // Sum tổng tiền
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
            string sFilename = "LienketKhachle_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: sFilename
            );
        }

        /////////////////////////////////////// Khach Huy ///////////////////////////////////////////////////////////////////
        public IActionResult KhachHuy(string tungay = null, string denngay = null, string chiNhanh = null, string khoi = null)
        {
            //var khachHuyVM = new KhachHuyViewModel();

            //var chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

            //for (int i = 0; i < chiNhanhs.Count(); i++)
            //{
            //    var cnToreturn = new ChiNhanhToReturnViewModel()
            //    {
            //        Stt = i,
            //        Name = chiNhanhs[i]
            //    };

            //    khachHuyVM.ChiNhanhToReturnViewModels.Add(cnToreturn);
            //}

            //khachHuyVM.KhoiViewModels_KL = KhoiViewModels_KL();

            var user = HttpContext.Session.Get<Users>("loginUser");
            var khachHuyVM = new KhachHuyViewModel();
            string[] chiNhanhs = null;
            if (user.Nhom != "Users")
            {
                if (user.Nhom != "Admins")
                {
                    chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Where(x => x.Nhom == user.Nhom).Select(x => x.Chinhanh1).Distinct().ToArray();

                }
                else
                {
                    chiNhanhs = _unitOfWork.chiNhanhRepository.GetAll().Select(x => x.Chinhanh1).Distinct().ToArray();

                }
                for (int i = 0; i < chiNhanhs.Count(); i++)
                {
                    var cnToreturn = new ChiNhanhToReturnViewModel()
                    {
                        Stt = i,
                        Name = chiNhanhs[i]
                    };

                    khachHuyVM.ChiNhanhToReturnViewModels.Add(cnToreturn);
                }
                khachHuyVM.KhoiViewModels_KL = KhoiViewModels_KL();
            }
            else
            {
                khachHuyVM.ChiNhanhToReturnViewModels.Add(new ChiNhanhToReturnViewModel() { Stt = 1, Name = user.Chinhanh });
                khachHuyVM.KhoiViewModels_KL = KhoiViewModels_KL().Where(x => x.Name.Equals(user.Khoi)).ToList();

            }

            try
            {
                ViewBag.searchFromDate = tungay;
                ViewBag.searchToDate = denngay;
                chiNhanh = chiNhanh ?? "";
                ViewBag.chiNhanh = chiNhanh;
                ViewBag.khoi = khoi;

                if (tungay == null || denngay == null)
                {
                    return View("KhachHuy", khachHuyVM);
                }

                var list = _unitOfWork.thongKeRepository.listKhachHuy(tungay, denngay, chiNhanh, khoi);
                khachHuyVM.KhachHuys = list;
                return View(khachHuyVM);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return View("KhachHuy", khachHuyVM);
            }
        }

        [HttpPost]
        public IActionResult KhachHuyPost(string tungay, string denngay, string chinhanh, string khoi)//(string tungay,string denngay, string daily)
        {
            if (tungay == null || denngay == null)
            {
                return RedirectToAction("KhachHuy");
            }
            try
            {
                DateTime.Parse(tungay);
                DateTime.Parse(denngay);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return RedirectToAction("KhachHuy");
            }
            //chinhanh = String.IsNullOrEmpty(chinhanh) ? Session["chinhanh"].ToString() : chinhanh;
            //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;

            chinhanh = chinhanh ?? "";
            string fromTo = "";
            ExcelPackage ExcelApp = new ExcelPackage();
            ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
            // Định dạng chiều dài cho cột
            xlSheet.Column(1).Width = 10;//stt
            xlSheet.Column(2).Width = 30;// ten khach
            xlSheet.Column(3).Width = 30;// sgtcode
            xlSheet.Column(4).Width = 10;// Vetourid
            xlSheet.Column(5).Width = 40;// Tuyến tq

            xlSheet.Column(6).Width = 30;// Bắt đầu
            xlSheet.Column(7).Width = 30;// Kết thúc
            xlSheet.Column(8).Width = 20;// Giá tour
            xlSheet.Column(9).Width = 40;// Người hủy vé
            xlSheet.Column(10).Width = 30;// Đại lý hủy vé
            xlSheet.Column(11).Width = 10;// Chi nhánh
            xlSheet.Column(12).Width = 30;// Ngày hủy vé

            xlSheet.Cells[2, 1].Value = "THỐNG KÊ HỦY ĐOÀN " + khoi + " " + chinhanh;
            xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
            xlSheet.Cells[2, 1, 2, 12].Merge = true;
            setCenterAligment(2, 1, 2, 12, xlSheet);
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
            xlSheet.Cells[3, 1, 3, 12].Merge = true;
            xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
            setCenterAligment(3, 1, 3, 12, xlSheet);

            // Tạo header
            xlSheet.Cells[5, 1].Value = "STT";
            xlSheet.Cells[5, 2].Value = "Tên khách";
            xlSheet.Cells[5, 3].Value = "Sgt code";
            xlSheet.Cells[5, 4].Value = "Vetourid";
            xlSheet.Cells[5, 5].Value = "Tuyến tq";

            xlSheet.Cells[5, 6].Value = "Bắt đầu";
            xlSheet.Cells[5, 7].Value = "Kết thúc";
            xlSheet.Cells[5, 8].Value = "Giá tour";
            xlSheet.Cells[5, 9].Value = "Người hủy vé";
            xlSheet.Cells[5, 10].Value = "Đại lý hủy vé";
            xlSheet.Cells[5, 11].Value = "Chi nhánh";
            xlSheet.Cells[5, 12].Value = "Ngày hủy vé";

            xlSheet.Cells[5, 1, 5, 12].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));

            // do du lieu tu table  
            int dong = 5;

            var d = _unitOfWork.thongKeRepository.listKhachHuy(tungay, denngay, chinhanh, khoi);// Session["daily"].ToString(), Session["khoi"].ToString());

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

                    xlSheet.Cells[iRowIndex, 2].Value = vm.tenkhach;
                    TrSetCellBorder(xlSheet, iRowIndex, 2, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 3].Value = vm.sgtcode;
                    TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 4].Value = vm.vetourid;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 5].Value = vm.tuyentq;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 6].Value = vm.batdau;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 7].Value = vm.ketthuc;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 8].Value = vm.giatour;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 9].Value = vm.nguoihuyve;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 10].Value = vm.dailyhuyve;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 11].Value = vm.chinhanh;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 11].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 12].Value = vm.ngayhuyve;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 12].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    iRowIndex += 1;
                    idem += 1;
                    dong++;
                }
            }
            else
            {
                SetAlert("No sale.", "warning");
                return RedirectToAction(nameof(KhachHuy));
            }

            dong++;
            //// Merger cot 4,5 ghi tổng tiền
            //setRightAligment(dong, 3, dong, 3, xlSheet);
            //xlSheet.Cells[dong, 1, dong, 2].Merge = true;
            //xlSheet.Cells[dong, 1].Value = "Tổng tiền: ";
            // Sum tổng tiền
            xlSheet.Cells[dong, 7].Value = "TC";
            xlSheet.Cells[dong, 8].Formula = "SUM(H6:H" + (6 + d.Count() - 1) + ")";

            // Sum so khach
            xlSheet.Cells[dong, 1].Value = "Số khách";
            xlSheet.Cells[dong, 2].Value = d.Count();
            setBorder(dong, 7, dong, 7, xlSheet);
            setFontBold(dong, 1, dong, 1, 12, xlSheet);

            setBorder(dong, 2, dong, 2, xlSheet);
            setBorder(dong, 1, dong, 1, xlSheet);
            setFontBold(dong, 1, dong, 1, 12, xlSheet);

            setBorder(5, 1, 5 + d.Count(), 12, xlSheet);
            setFontBold(5, 1, 5, 12, 11, xlSheet);
            setFontSize(6, 1, 6 + d.Count(), 12, 11, xlSheet);
            // canh giua cot stt
            setCenterAligment(6, 1, 6 + d.Count(), 1, xlSheet);
            // canh giua code chinhanh
            setCenterAligment(6, 11, 6 + d.Count(), 11, xlSheet);
            NumberFormat(6, 8, 6 + d.Count(), 8, xlSheet);
            // định dạng số cot tong cong
            NumberFormat(dong, 8, dong, 8, xlSheet);
            setBorder(dong, 8, dong, 8, xlSheet);
            setFontBold(dong, 8, dong, 8, 12, xlSheet);
            // DateFormat
            DateFormat(6, 6, 6 + d.Count(), 6, xlSheet);
            DateFormat(6, 7, 6 + d.Count(), 7, xlSheet);
            DateFormat(6, 12, 6 + d.Count(), 12, xlSheet);

            //xlSheet.View.FreezePanes(6, 20);

            //end du lieu

            byte[] fileContents;
            fileContents = ExcelApp.GetAsByteArray();

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }
            string sFilename = "DSKhachHuy_" + khoi + " " + chinhanh + "_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: sFilename
            );
        }

        /////////////////////////////////////// Kinh doanh online ///////////////////////////////////////////////////////////////////
        public IActionResult KinhDoanhOnline(string tungay = null, string denngay = null, string khoi = null)
        {
            var user = HttpContext.Session.Get<Users>("loginUser");
            if (user.Nhom != "Admins" && user.Nhom != "KDO")
            {
                return View("AccessDenied");
            }
            var thongKeWebVM = new ThongKeWebViewModel();

            thongKeWebVM.KhoiViewModels_KL = KhoiViewModels_KL();

            try
            {
                ViewBag.searchFromDate = tungay;
                ViewBag.searchToDate = denngay;
                ViewBag.khoi = khoi;

                if (tungay == null || denngay == null)
                {
                    return View("KinhDoanhOnline", thongKeWebVM);
                }

                var list = _unitOfWork.thongKeRepository.listThongKeWeb(tungay, denngay, khoi);
                thongKeWebVM.Thongkewebs = list;
                return View(thongKeWebVM);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return View("KinhDoanhOnline", thongKeWebVM);
            }
        }

        public IActionResult KinhDoanhOnlineChiTietToExcel(string tungay, string denngay, string chinhanh, string khoi)
        {
            try
            {
                //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;
                string fromTo = "";
                ExcelPackage ExcelApp = new ExcelPackage();
                ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
                // Định dạng chiều dài cho cột
                xlSheet.Column(1).Width = 10;//STT
                xlSheet.Column(2).Width = 25;//SGTCODE
                xlSheet.Column(3).Width = 35;// TUYEN TQ
                xlSheet.Column(4).Width = 15;// NGAY DI
                xlSheet.Column(5).Width = 15;// NGAY VE
                xlSheet.Column(6).Width = 30;// TEN KHACH
                xlSheet.Column(7).Width = 15;//  SERIAL
                xlSheet.Column(8).Width = 15;//  HUY VE
                xlSheet.Column(9).Width = 10;//  SÔ KHÁCH
                xlSheet.Column(10).Width = 15;//  DOANH SO
                xlSheet.Column(11).Width = 30;//  sale
                xlSheet.Column(12).Width = 30;//  DAI LY 
                xlSheet.Column(13).Width = 20;//  KENH GD


                xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH THU ONLINE THEO NGÀY BÁN " + chinhanh;
                xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
                xlSheet.Cells[2, 1, 2, 12].Merge = true;
                setCenterAligment(2, 1, 2, 12, xlSheet);
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
                xlSheet.Cells[3, 1, 3, 13].Merge = true;
                xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
                setCenterAligment(3, 1, 3, 13, xlSheet);

                // Tạo header
                xlSheet.Cells[5, 1].Value = "STT";
                xlSheet.Cells[5, 2].Value = "Sgt Code";
                xlSheet.Cells[5, 3].Value = "Hành trình ";
                xlSheet.Cells[5, 4].Value = "Ngày đi";
                xlSheet.Cells[5, 5].Value = "Ngày về";
                xlSheet.Cells[5, 6].Value = "Tên khách";
                xlSheet.Cells[5, 7].Value = "Serial";
                xlSheet.Cells[5, 8].Value = "Huỷ vé";
                xlSheet.Cells[5, 9].Value = "Số khách";
                xlSheet.Cells[5, 10].Value = "Doanh số";
                xlSheet.Cells[5, 11].Value = "Nhân viên";
                xlSheet.Cells[5, 12].Value = "Đại lý";
                xlSheet.Cells[5, 13].Value = "Kênh GD";
                xlSheet.Cells[5, 1, 5, 13].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));

                // do du lieu tu table
                int dong = 5;

                var d = _unitOfWork.thongKeRepository.ThongKeWebChiTietToExcel(tungay, denngay, chinhanh, khoi);// Session["fullName"].ToString());

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

                        xlSheet.Cells[iRowIndex, 3].Value = vm.Hanhtrinh;
                        TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 4].Value = vm.Ngaydi;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 5].Value = vm.Ngayve;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 6].Value = vm.Tenkhach;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 7].Value = vm.Serial;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 8].Value = vm.Huyve;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 9].Value = vm.Sokhach;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 10].Value = vm.Doanhso;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 11].Value = vm.Nguoixuatve;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 11].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 12].Value = vm.Dailyxuatve;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 12].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 13].Value = vm.Kenhgd;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 13].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        iRowIndex += 1;
                        idem += 1;
                        dong++;
                    }
                }
                else
                {
                    SetAlert("No sale.", "warning");
                    return RedirectToAction(nameof(KinhDoanhOnline));
                }

                dong++;

                xlSheet.Cells[dong, 8].Value = "TC";
                xlSheet.Cells[dong, 9].Formula = "SUM(I6:I" + (6 + d.Count() - 1) + ")";
                xlSheet.Cells[dong, 10].Formula = "SUM(J6:J" + (6 + d.Count() - 1) + ")";
                // định dạng số
                NumberFormat(6, 10, 6 + d.Count(), 10, xlSheet);
                setFontSize(6, 1, 6 + d.Count(), 13, 12, xlSheet);
                setBorder(5, 1, 5 + d.Count(), 13, xlSheet);
                setFontBold(5, 1, 5, 10, 13, xlSheet);

                // canh giưa cot stt
                setCenterAligment(6, 1, 6 + d.Count(), 2, xlSheet);
                // canh giưa cot so khach
                setCenterAligment(6, 9, 6 + d.Count(), 9, xlSheet);

                setBorder(dong, 8, dong, 10, xlSheet);
                setFontBold(dong, 8, dong, 10, 12, xlSheet);
                // canh giưa cot ngay di va ngày ve
                setCenterAligment(6, 4, 6 + d.Count(), 5, xlSheet);
                DateFormat(6, 4, 6 + d.Count(), 5, xlSheet);

                //end du lieu

                byte[] fileContents;
                fileContents = ExcelApp.GetAsByteArray();

                if (fileContents == null || fileContents.Length == 0)
                {
                    return NotFound();
                }
                string sFilename = "DoanhThuKinhDoanhOnline" + "_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

                return File(
                    fileContents: fileContents,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: sFilename
                );
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return RedirectToAction("KinhDoanhOnline");
            }
        }

        /////////////////////////////////////// Kinh doanh online Ngay Di ///////////////////////////////////////////////////////////////////
        public IActionResult KinhDoanhOnlineNgayDi(string tungay = null, string denngay = null, string khoi = null)
        {
            var user = HttpContext.Session.Get<Users>("loginUser");
            if (user.Nhom != "Admins" && user.Nhom != "KDO")
            {
                return View("AccessDenied");
            }

            var thongKeWebVM = new ThongKeWebViewModel();

            thongKeWebVM.KhoiViewModels_KL = KhoiViewModels_KL();

            try
            {
                ViewBag.searchFromDate = tungay;
                ViewBag.searchToDate = denngay;
                ViewBag.khoi = khoi;

                if (tungay == null || denngay == null)
                {
                    return View("KinhDoanhOnlineNgayDi", thongKeWebVM);
                }

                var list = _unitOfWork.thongKeRepository.listThongKeWebNgayDi(tungay, denngay, khoi);
                thongKeWebVM.Thongkewebs = list;
                return View(thongKeWebVM);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return View("KinhDoanhOnlineNgayDi", thongKeWebVM);
            }
        }

        [HttpPost]
        public IActionResult KinhDoanhOnlineNgayDiPost(string tungay, string denngay, string khoi)
        {
            if (tungay == null || denngay == null)
            {
                return RedirectToAction("KinhDoanhOnlineNgayDi");
            }
            try
            {
                DateTime.Parse(tungay);
                DateTime.Parse(denngay);
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return RedirectToAction("KinhDoanhOnlineNgayDi");
            }
            //chinhanh = String.IsNullOrEmpty(chinhanh) ? Session["chinhanh"].ToString() : chinhanh;
            //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;
            string fromTo = "";
            ExcelPackage ExcelApp = new ExcelPackage();
            ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
            // Định dạng chiều dài cho cột
            xlSheet.Column(1).Width = 10;//STT
            xlSheet.Column(2).Width = 25;//SGTCODE
            xlSheet.Column(3).Width = 35;// TUYEN TQ
            xlSheet.Column(4).Width = 15;// NGAY DI
            xlSheet.Column(5).Width = 15;// NGAY VE
            xlSheet.Column(6).Width = 30;// TEN KHACH
            xlSheet.Column(7).Width = 15;//  SERIAL
            xlSheet.Column(8).Width = 15;//  HUY VE
            xlSheet.Column(9).Width = 10;//  SÔ KHÁCH
            xlSheet.Column(10).Width = 15;//  DOANH SO
            xlSheet.Column(11).Width = 30;//  sale
            xlSheet.Column(12).Width = 30;//  DAI LY 
            xlSheet.Column(13).Width = 20;//  KENH GD


            xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH THU ONLINE THEO NGÀY DI ";
            xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
            xlSheet.Cells[2, 1, 2, 12].Merge = true;
            setCenterAligment(2, 1, 2, 12, xlSheet);
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
            xlSheet.Cells[3, 1, 3, 13].Merge = true;
            xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
            setCenterAligment(3, 1, 3, 13, xlSheet);

            // Tạo header
            xlSheet.Cells[5, 1].Value = "STT";
            xlSheet.Cells[5, 2].Value = "Sgt Code";
            xlSheet.Cells[5, 3].Value = "Hành trình ";
            xlSheet.Cells[5, 4].Value = "Ngày đi";
            xlSheet.Cells[5, 5].Value = "Ngày về";
            xlSheet.Cells[5, 6].Value = "Tên khách";
            xlSheet.Cells[5, 7].Value = "Serial";
            xlSheet.Cells[5, 8].Value = "Huỷ vé";
            xlSheet.Cells[5, 9].Value = "Số khách";
            xlSheet.Cells[5, 10].Value = "Doanh số";
            xlSheet.Cells[5, 11].Value = "Nhân viên";
            xlSheet.Cells[5, 12].Value = "Đại lý";
            xlSheet.Cells[5, 13].Value = "Kênh GD";
            xlSheet.Cells[5, 1, 5, 13].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));

            // do du lieu tu table
            int dong = 5;

            var d = _unitOfWork.thongKeRepository.ThongKeWebNgayDiToExcel(tungay, denngay, "", khoi);// Session["daily"].ToString(), Session["khoi"].ToString());

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

                    xlSheet.Cells[iRowIndex, 3].Value = vm.Hanhtrinh;
                    TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 4].Value = vm.Ngaydi;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 5].Value = vm.Ngayve;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 6].Value = vm.Tenkhach;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 7].Value = vm.Serial;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 8].Value = vm.Huyve;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 9].Value = vm.Sokhach;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 10].Value = vm.Doanhso;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 11].Value = vm.Nguoixuatve;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 11].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 12].Value = vm.Dailyxuatve;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 12].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    xlSheet.Cells[iRowIndex, 13].Value = vm.Kenhgd;
                    TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                    xlSheet.Cells[iRowIndex, 13].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    iRowIndex += 1;
                    idem += 1;
                    dong++;
                }
            }
            else
            {
                SetAlert("No sale.", "warning");
                return RedirectToAction(nameof(KhachHuy));
            }

            dong++;

            xlSheet.Cells[dong, 8].Value = "TC";
            xlSheet.Cells[dong, 9].Formula = "SUM(I6:I" + (6 + d.Count() - 1) + ")";
            xlSheet.Cells[dong, 10].Formula = "SUM(J6:J" + (6 + d.Count() - 1) + ")";
            // định dạng số
            NumberFormat(6, 10, 6 + d.Count(), 10, xlSheet);
            setFontSize(6, 1, 6 + d.Count(), 13, 12, xlSheet);
            setBorder(5, 1, 5 + d.Count(), 13, xlSheet);
            setFontBold(5, 1, 5, 10, 13, xlSheet);

            // canh giưa cot stt
            setCenterAligment(6, 1, 6 + d.Count(), 2, xlSheet);
            // canh giưa cot so khach
            setCenterAligment(6, 9, 6 + d.Count(), 9, xlSheet);

            setBorder(dong, 8, dong, 10, xlSheet);
            setFontBold(dong, 8, dong, 10, 12, xlSheet);
            // canh giưa cot ngay di va ngày ve
            setCenterAligment(6, 4, 6 + d.Count(), 5, xlSheet);
            DateFormat(6, 4, 6 + d.Count(), 5, xlSheet);

            //end du lieu

            byte[] fileContents;
            fileContents = ExcelApp.GetAsByteArray();

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }
            string sFilename = "DoanhThuKinhDoanhOnline_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: sFilename
            );
        }

        public IActionResult KinhDoanhOnlineNgayDiChiTietToExcel(string tungay, string denngay, string chinhanh, string khoi)
        {
            try
            {
                //khoi = String.IsNullOrEmpty(khoi) ? Session["khoi"].ToString() : khoi;
                string fromTo = "";
                ExcelPackage ExcelApp = new ExcelPackage();
                ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
                // Định dạng chiều dài cho cột
                xlSheet.Column(1).Width = 10;//STT
                xlSheet.Column(2).Width = 25;//SGTCODE
                xlSheet.Column(3).Width = 35;// TUYEN TQ
                xlSheet.Column(4).Width = 15;// NGAY DI
                xlSheet.Column(5).Width = 15;// NGAY VE
                xlSheet.Column(6).Width = 30;// TEN KHACH
                xlSheet.Column(7).Width = 15;//  SERIAL
                xlSheet.Column(8).Width = 15;//  HUY VE
                xlSheet.Column(9).Width = 10;//  SO KHACH
                xlSheet.Column(10).Width = 15;//  DOANH SO
                xlSheet.Column(11).Width = 30;//  sale
                xlSheet.Column(12).Width = 30;//  DAI LY 
                xlSheet.Column(13).Width = 20;//  KENH GD


                xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH THU ONLINE THEO NGÀY ĐI TOUR " + chinhanh;
                xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
                xlSheet.Cells[2, 1, 2, 13].Merge = true;
                setCenterAligment(2, 1, 2, 13, xlSheet);
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
                xlSheet.Cells[3, 1, 3, 13].Merge = true;
                xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
                setCenterAligment(3, 1, 3, 13, xlSheet);

                // Tạo header
                xlSheet.Cells[5, 1].Value = "STT";
                xlSheet.Cells[5, 2].Value = "Sgt Code";
                xlSheet.Cells[5, 3].Value = "Hành trình ";
                xlSheet.Cells[5, 4].Value = "Ngày đi";
                xlSheet.Cells[5, 5].Value = "Ngày về";
                xlSheet.Cells[5, 6].Value = "Tên khách";
                xlSheet.Cells[5, 7].Value = "Serial";
                xlSheet.Cells[5, 8].Value = "Huỷ vé";
                xlSheet.Cells[5, 9].Value = "Số khách";
                xlSheet.Cells[5, 10].Value = "Doanh số";
                xlSheet.Cells[5, 11].Value = "Nhân viên";
                xlSheet.Cells[5, 12].Value = "Đại lý";
                xlSheet.Cells[5, 13].Value = "Kênh GD";
                xlSheet.Cells[5, 1, 5, 13].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));

                // do du lieu tu table
                int dong = 5;

                var d = _unitOfWork.thongKeRepository.ThongKeWebNgayDiToExcel(tungay, denngay, chinhanh, khoi);// Session["fullName"].ToString());

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

                        xlSheet.Cells[iRowIndex, 3].Value = vm.Hanhtrinh;
                        TrSetCellBorder(xlSheet, iRowIndex, 3, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 4].Value = vm.Ngaydi;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 5].Value = vm.Ngayve;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 6].Value = vm.Tenkhach;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 7].Value = vm.Serial;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 8].Value = vm.Huyve;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 9].Value = vm.Sokhach;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 10].Value = vm.Doanhso;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 11].Value = vm.Nguoixuatve;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 11].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 12].Value = vm.Dailyxuatve;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 12].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        xlSheet.Cells[iRowIndex, 13].Value = vm.Kenhgd;
                        TrSetCellBorder(xlSheet, iRowIndex, 4, ExcelBorderStyle.Dotted, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 10, FontStyle.Regular);
                        xlSheet.Cells[iRowIndex, 13].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        iRowIndex += 1;
                        idem += 1;
                        dong++;
                    }
                }
                else
                {
                    SetAlert("No sale.", "warning");
                    return RedirectToAction(nameof(KinhDoanhOnline));
                }

                dong++;

                xlSheet.Cells[dong, 8].Value = "TC";
                xlSheet.Cells[dong, 9].Formula = "SUM(I6:I" + (6 + d.Count() - 1) + ")";
                xlSheet.Cells[dong, 10].Formula = "SUM(J6:J" + (6 + d.Count() - 1) + ")";
                // định dạng số
                NumberFormat(6, 10, 6 + d.Count(), 10, xlSheet);
                setFontSize(6, 1, 6 + d.Count(), 13, 12, xlSheet);
                setBorder(5, 1, 5 + d.Count(), 13, xlSheet);
                setFontBold(5, 1, 5, 10, 13, xlSheet);

                // canh giưa cot stt
                setCenterAligment(6, 1, 6 + d.Count(), 2, xlSheet);
                // canh giưa cot so khach
                setCenterAligment(6, 9, 6 + d.Count(), 9, xlSheet);

                setBorder(dong, 8, dong, 10, xlSheet);
                setFontBold(dong, 8, dong, 10, 12, xlSheet);
                // canh giưa cot ngay di va ngày ve
                setCenterAligment(6, 4, 6 + d.Count(), 5, xlSheet);
                DateFormat(6, 4, 6 + d.Count(), 5, xlSheet);

                //end du lieu

                byte[] fileContents;
                fileContents = ExcelApp.GetAsByteArray();

                if (fileContents == null || fileContents.Length == 0)
                {
                    return NotFound();
                }
                string sFilename = "DoanhThuKinhDoanhOnline" + "_" + System.DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".xlsx";

                return File(
                    fileContents: fileContents,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: sFilename
                );
            }
            catch
            {
                SetAlert("Lỗi định dạng ngày tháng", "error");
                return RedirectToAction("KinhDoanhOnline");
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpGet]
        public JsonResult GetAllTuyentqByKhoi(string khoi)
        {
            var model = _unitOfWork.userRepository.GetAllTuyentqByKhoi(khoi);
            //var viewModel = Mapper.Map<IEnumerable<chinhanh>, IEnumerable<chinhanhViewModel>>(model);
            return Json(new
            {
                data = JsonConvert.SerializeObject(model)
            });
        }

        public List<KhoiViewModel> KhoiViewModels_KL()
        {
            return new List<KhoiViewModel>()
            {
                new KhoiViewModel() { Id = 1, Name = "OB" },
                new KhoiViewModel() { Id = 2, Name = "ND" }
            };
        }

        public List<KhoiViewModel> KhoiViewModels_KD()
        {
            return new List<KhoiViewModel>()
            {
                new KhoiViewModel() { Id = 1, Name = "OB" },
                new KhoiViewModel() { Id = 2, Name = "ND" },
                new KhoiViewModel() { Id = 3, Name = "IB" }
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


        #region Khach doan
        #region Doanh so theo sale
        public async Task<IActionResult> DoanhSoTheoSale(string searchFromDate = null, string searchToDate = null,
            string Macn = null, string khoi = null)
        {

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // from session
            var user = HttpContext.Session.Get<Users>("loginUser");

            //// moi load vao
            if (string.IsNullOrEmpty(searchFromDate) && string.IsNullOrEmpty(searchToDate))
            {
                //var currentTime = DateTime.Now;
                //string TuNgayDenNgayString = LoadTuNgayDenNgay(currentTime.Month.ToString(), currentTime.Month.ToString(), currentTime.Year.ToString());
                //searchFromDate = TuNgayDenNgayString.Split('-')[0];
                //searchToDate = TuNgayDenNgayString.Split('-')[1];

                searchFromDate = DateTime.Now.ToShortDateString();
                searchToDate = (new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).AddDays(-1)).ToString();
            }
            else // da chon ngay thang - // check date correct
            {
                try
                {
                    Convert.ToDateTime(searchFromDate);
                    Convert.ToDateTime(searchToDate);
                }
                catch (Exception)
                {
                    //BaoCaoVM = new BaoCaoViewModel()
                    //{
                    //    Dmchinhanhs = _unitOfWork.dmChiNhanhRepository.GetAll(),
                    //    Tourkinds = _unitOfWork.tourKindRepository.GetAll(),
                    //    TourBaoCaoDtosTheoNgay = new TourBaoCaoDtosTheoNgay()
                    //};

                    ViewBag.Macn = Macn;
                    ViewBag.searchFromDate = searchFromDate;
                    ViewBag.searchToDate = searchToDate;

                    ModelState.AddModelError("", "Lỗi định dạng ngày tháng.");
                    return View(BaoCaoVM);
                }
            }

            ViewBag.Macn = Macn;
            ViewBag.khoi = khoi;
            ViewBag.searchFromDate = searchFromDate;
            ViewBag.searchToDate = searchToDate;

            BaoCaoVM.Companies = _baoCaoService.GetCompanies();
            BaoCaoVM.Dmchinhanhs = _baoCaoService.GetAllChiNhanh().Where(x => !string.IsNullOrEmpty(x.Macn));
            BaoCaoVM.Khois_KD = KhoiViewModels_KD();
            List<string> maCns = new List<string>();
            if (user.RoleId != 8) // 8: Admins
            {
                if (user.RoleId == 9) // 9: Users
                {
                    //BaoCaoVM.Dmchinhanhs = new List<Dmchinhanh>() { new Dmchinhanh() { Macn = user.MaCN } };
                    //BaoCaoVM.TourBaoCaoDtos = _baoCaoService.DoanhSoTheoSale(searchFromDate, searchToDate, BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList());
                    //BaoCaoVM.TourBaoCaoDtos = BaoCaoVM.TourBaoCaoDtos.Where(x => x.NguoiTao == user.Username);

                    // hien thi tren view
                    BaoCaoVM.Dmchinhanhs = new List<Dmchinhanh>() { new Dmchinhanh() { Macn = user.Chinhanh } };
                    BaoCaoVM.Khois_KD = KhoiViewModels_KD().Where(x => x.Name == user.Khoi);

                    switch (khoi)
                    {
                        case "IB":
                            if (!string.IsNullOrEmpty(user.PhongBanQL)) // co ql phongban khac' --> IB
                            {

                                var phongBanQL = user.PhongBanQL.Split(',').ToList();
                                BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoSale_IB(searchFromDate, searchToDate,
                                    BaoCaoVM.Dmchinhanhs.ToList(), phongBanQL, "");


                                //var phanKhuCNs = await _unitOfWork.phanKhuCNRepository.FindIncludeOneAsync(x => x.Role, y => y.RoleId == user.RoleId);
                                //foreach (var item in phanKhuCNs)
                                //{
                                //    maCns.AddRange(item.ChiNhanhs.Split(',').ToList());
                                //}
                                //BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Where(item1 => maCns.Any(item2 => item1.Macn == item2));

                            }
                            else
                            {
                                BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoSale_IB(searchFromDate, searchToDate,
                                    BaoCaoVM.Dmchinhanhs.ToList(), new List<string>(), user.Username);
                                //BaoCaoVM.TourIBDTOs = BaoCaoVM.TourIBDTOs.Where(x => x.NguoiTao == user.Username);
                            }
                            DoanhSoTheoSaleGroupbyNguoiTao_IB();
                            break;

                        case "ND":
                            // do tournd ko co daily -> lay theo chinhanh
                            BaoCaoVM.TourNDDTOs = _baoCaoService.DoanhSoTheoSale_ND(searchFromDate, searchToDate,
                                BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList(), user.Username);
                            //BaoCaoVM.TourNDDTOs = BaoCaoVM.TourNDDTOs.Where(x => x.Nguoitao == user.Username);
                            DoanhSoTheoSaleGroupbyNguoiTao_ND();
                            break;

                        case "OB":
                            // do tourob ko co daily -> lay theo chinhanh
                            BaoCaoVM.TourOBDTOs = _baoCaoService.DoanhSoTheoSale_OB(searchFromDate, searchToDate,
                                BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList(), user.Username);
                            //BaoCaoVM.TourOBDTOs = BaoCaoVM.TourOBDTOs.Where(x => x.Nguoitao == user.Username);
                            DoanhSoTheoSaleGroupbyNguoiTao_OB();
                            break;

                    }

                }
                else // admin khuvuc
                {
                    // phanKhuCNs = co cn QL
                    var role1 = await _baoCaoService.GetRoleById(user.RoleId);
                    var listMaCN = role1.ChiNhanhQL.Split(',').ToList();
                    maCns.AddRange(listMaCN);

                    // hien thi tren view
                    BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Where(x => maCns.Any(y => x.Macn == y));
                    // lay het 3 khoi

                    switch (khoi)
                    {
                        case "IB":

                            var dmChiNhanhs = string.IsNullOrEmpty(Macn) ? BaoCaoVM.Dmchinhanhs.ToList() :
                                BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == Macn).ToList();

                            BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoSale_IB(searchFromDate, searchToDate,
                                    dmChiNhanhs, new List<string>(), "");

                            DoanhSoTheoSaleGroupbyNguoiTao_IB();
                            break;

                        case "ND":
                            maCns = string.IsNullOrEmpty(Macn) ? BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList() :
                                new List<string> { Macn };
                            // do tournd ko co daily -> lay theo chinhanh
                            BaoCaoVM.TourNDDTOs = _baoCaoService.DoanhSoTheoSale_ND(searchFromDate, searchToDate,
                                    BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList(), "");

                            DoanhSoTheoSaleGroupbyNguoiTao_ND();
                            break;

                        case "OB":
                            maCns = string.IsNullOrEmpty(Macn) ? BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList() :
                                new List<string> { Macn };

                            // do tourOB ko co daily -> lay theo chinhanh
                            BaoCaoVM.TourOBDTOs = _baoCaoService.DoanhSoTheoSale_OB(searchFromDate, searchToDate,
                                    BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList(), "");

                            DoanhSoTheoSaleGroupbyNguoiTao_OB();
                            break;

                    }


                }
            }
            else // admin tong
            {

                switch (khoi)
                {
                    case "IB":

                        var dmChiNhanhs = string.IsNullOrEmpty(Macn) ? BaoCaoVM.Dmchinhanhs.ToList() :
                            BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == Macn).ToList();

                        BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoSale_IB(searchFromDate, searchToDate,
                            dmChiNhanhs, new List<string>(), "");

                        DoanhSoTheoSaleGroupbyNguoiTao_IB();

                        break;

                    case "ND":
                        maCns = string.IsNullOrEmpty(Macn) ? BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList() :
                            BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == Macn).Select(x => x.Macn).ToList();

                        BaoCaoVM.TourNDDTOs = _baoCaoService.DoanhSoTheoSale_ND(searchFromDate, searchToDate, maCns, "");

                        DoanhSoTheoSaleGroupbyNguoiTao_ND();
                        break;

                    case "OB":

                        maCns = string.IsNullOrEmpty(Macn) ? BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList() :
                            BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == Macn).Select(x => x.Macn).ToList();

                        BaoCaoVM.TourOBDTOs = _baoCaoService.DoanhSoTheoSale_OB(searchFromDate, searchToDate, maCns, "");

                        DoanhSoTheoSaleGroupbyNguoiTao_OB();
                        break;

                }

            }
            BaoCaoVM.Khoi = khoi;
            return View(BaoCaoVM);
        }

        [HttpPost]
        public async Task<IActionResult> DoanhSoTheoSaleExcel(string searchFromDate = null, string searchToDate = null,
            string Macn = null, string khoi = null)
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // from session
            var user = HttpContext.Session.Get<Users>("loginUser");

            //// moi load vao
            if (string.IsNullOrEmpty(searchFromDate) && string.IsNullOrEmpty(searchToDate))
            {
                //var currentTime = DateTime.Now;
                //string TuNgayDenNgayString = LoadTuNgayDenNgay(currentTime.Month.ToString(), currentTime.Month.ToString(), currentTime.Year.ToString());
                //searchFromDate = TuNgayDenNgayString.Split('-')[0];
                //searchToDate = TuNgayDenNgayString.Split('-')[1];

                searchFromDate = DateTime.Now.ToShortDateString();
                searchToDate = (new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).AddDays(-1)).ToString();
            }
            else // da chon ngay thang - // check date correct
            {
                try
                {
                    Convert.ToDateTime(searchFromDate);
                    Convert.ToDateTime(searchToDate);
                }
                catch (Exception)
                {
                    //BaoCaoVM = new BaoCaoViewModel()
                    //{
                    //    Dmchinhanhs = _unitOfWork.dmChiNhanhRepository.GetAll(),
                    //    Tourkinds = _unitOfWork.tourKindRepository.GetAll(),
                    //    TourBaoCaoDtosTheoNgay = new TourBaoCaoDtosTheoNgay()
                    //};

                    ViewBag.Macn = Macn;
                    ViewBag.searchFromDate = searchFromDate;
                    ViewBag.searchToDate = searchToDate;

                    ModelState.AddModelError("", "Lỗi định dạng ngày tháng.");
                    return View(BaoCaoVM);
                }
            }

            ViewBag.Macn = Macn;
            ViewBag.khoi = khoi;
            ViewBag.searchFromDate = searchFromDate;
            ViewBag.searchToDate = searchToDate;

            string fromTo = "";
            ExcelPackage ExcelApp = new ExcelPackage();
            ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
            // Định dạng chiều dài cho cột
            xlSheet.Column(1).Width = 10;// STT
            xlSheet.Column(2).Width = 20;// Code đoàn
            xlSheet.Column(3).Width = 35;// Tên công ty/Khách hàng
            xlSheet.Column(4).Width = 15;// bat dau
            xlSheet.Column(5).Width = 15;// ket thuc
            xlSheet.Column(6).Width = 40;// Chủ đề tour
            xlSheet.Column(7).Width = 40;// Tuyến tham quan
            xlSheet.Column(8).Width = 10;// SK dự kiến
            xlSheet.Column(9).Width = 20;// Doanh số dự kiến
            xlSheet.Column(10).Width = 10;// SK thực tế
            xlSheet.Column(11).Width = 20;// Doanh số thực tế
            xlSheet.Column(12).Width = 10;// Sales

            xlSheet.Cells[1, 1].Value = "CÔNG TY DVLH SAIGONTOURIST";
            xlSheet.Cells[1, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
            xlSheet.Cells[1, 1, 1, 12].Merge = true;

            xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH SỐ THEO SALES";
            xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
            xlSheet.Cells[2, 1, 2, 12].Merge = true;
            setCenterAligment(2, 1, 2, 12, xlSheet);
            //// dinh dang tu ngay den ngay
            //if (string.IsNullOrEmpty(searchFromDate) && string.IsNullOrEmpty(searchToDate))
            //{
            //    ViewBag.searchFromDate = searchFromDate;
            //    ViewBag.searchToDate = searchToDate;
            //    SetAlert("Từ ngày đến ngày không được để trống.", "warning");
            //    return RedirectToAction(nameof(DoanhSoTheoSale));
            //}
            if (searchFromDate == searchToDate)
            {
                fromTo = "Ngày: " + searchFromDate;
            }
            else
            {
                fromTo = "Từ ngày: " + searchFromDate + " đến ngày: " + searchToDate;
            }
            xlSheet.Cells[3, 1].Value = fromTo;
            xlSheet.Cells[3, 1, 3, 12].Merge = true;
            xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
            setCenterAligment(3, 1, 3, 10, xlSheet);

            // Tạo header
            xlSheet.Cells[5, 1].Value = "STT";
            xlSheet.Cells[5, 2].Value = "Code đoàn ";
            xlSheet.Cells[5, 3].Value = "Tên công ty/Khách hàng ";
            xlSheet.Cells[5, 4].Value = "Bắt đầu ";
            xlSheet.Cells[5, 5].Value = "Kết thúc";
            xlSheet.Cells[5, 6].Value = "Chủ đề tour";
            xlSheet.Cells[5, 7].Value = "Tuyến tham quan";
            xlSheet.Cells[5, 8].Value = "SK dự kiến";
            xlSheet.Cells[5, 9].Value = "Doanh số dự kiến";
            xlSheet.Cells[5, 10].Value = "SK thực tế";
            xlSheet.Cells[5, 11].Value = "Doanh số thực tế";
            xlSheet.Cells[5, 12].Value = "Sales";

            xlSheet.Cells[5, 1, 5, 12].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));
            setBorder(5, 1, 5, 12, xlSheet);
            setCenterAligment(5, 1, 5, 12, xlSheet);

            // do du lieu tu table
            int dong = 6;

            BaoCaoVM.Companies = _baoCaoService.GetCompanies();
            BaoCaoVM.Dmchinhanhs = _baoCaoService.GetAllChiNhanh();//.Where(x => !string.IsNullOrEmpty(x.Macn));            
            BaoCaoVM.Khois_KD = KhoiViewModels_KD();
            List<string> maCns = new List<string>();

            if (user.RoleId != 8) // 8: Admins
            {
                if (user.RoleId == 9) // 9: Users
                {

                    // hien thi tren view
                    BaoCaoVM.Dmchinhanhs = new List<Dmchinhanh>() { new Dmchinhanh() { Macn = user.Chinhanh } };
                    BaoCaoVM.Khois_KD = KhoiViewModels_KD().Where(x => x.Name == user.Khoi);

                    switch (khoi)
                    {
                        case "IB":
                            if (!string.IsNullOrEmpty(user.PhongBanQL)) // co ql phongban khac' --> IB
                            {

                                var phongBanQL = user.PhongBanQL.Split(',').ToList();
                                BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoSale_IB(searchFromDate, searchToDate,
                                    BaoCaoVM.Dmchinhanhs.ToList(), phongBanQL, "");
                            }
                            else
                            {
                                BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoSale_IB(searchFromDate, searchToDate,
                                    BaoCaoVM.Dmchinhanhs.ToList(), new List<string>(), user.Username);
                                //BaoCaoVM.TourIBDTOs = BaoCaoVM.TourIBDTOs.Where(x => x.NguoiTao == user.Username);
                            }
                            DoanhSoTheoSaleGroupbyNguoiTao_IB();
                            break;

                        case "ND":
                            // do tournd ko co daily -> lay theo chinhanh
                            BaoCaoVM.TourNDDTOs = _baoCaoService.DoanhSoTheoSale_ND(searchFromDate, searchToDate,
                                BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList(), user.Username);
                            //BaoCaoVM.TourNDDTOs = BaoCaoVM.TourNDDTOs.Where(x => x.Nguoitao == user.Username);
                            DoanhSoTheoSaleGroupbyNguoiTao_ND();
                            break;

                        case "OB":
                            // do tourob ko co daily -> lay theo chinhanh
                            BaoCaoVM.TourOBDTOs = _baoCaoService.DoanhSoTheoSale_OB(searchFromDate, searchToDate,
                                BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList(), user.Username);
                            //BaoCaoVM.TourOBDTOs = BaoCaoVM.TourOBDTOs.Where(x => x.Nguoitao == user.Username);
                            DoanhSoTheoSaleGroupbyNguoiTao_OB();
                            break;

                    }

                }
                else // admin khuvuc
                {
                    // phanKhuCNs = co cn QL
                    var role1 = await _baoCaoService.GetRoleById(user.RoleId);
                    var listMaCN = role1.ChiNhanhQL.Split(',').ToList();
                    maCns.AddRange(listMaCN);

                    // hien thi tren view
                    BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Where(x => maCns.Any(y => x.Macn == y));
                    // lay het 3 khoi

                    switch (khoi)
                    {
                        case "IB":

                            var dmChiNhanhs = string.IsNullOrEmpty(Macn) ? BaoCaoVM.Dmchinhanhs.ToList() :
                                BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == Macn).ToList();

                            BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoSale_IB(searchFromDate, searchToDate,
                                    dmChiNhanhs, new List<string>(), "");

                            DoanhSoTheoSaleGroupbyNguoiTao_IB();
                            break;

                        case "ND":
                            maCns = string.IsNullOrEmpty(Macn) ? BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList() :
                                new List<string> { Macn };
                            // do tournd ko co daily -> lay theo chinhanh
                            BaoCaoVM.TourNDDTOs = _baoCaoService.DoanhSoTheoSale_ND(searchFromDate, searchToDate,
                                    BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList(), "");

                            DoanhSoTheoSaleGroupbyNguoiTao_ND();
                            break;

                        case "OB":
                            maCns = string.IsNullOrEmpty(Macn) ? BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList() :
                                new List<string> { Macn };

                            // do tourOB ko co daily -> lay theo chinhanh
                            BaoCaoVM.TourOBDTOs = _baoCaoService.DoanhSoTheoSale_OB(searchFromDate, searchToDate,
                                    BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList(), "");

                            DoanhSoTheoSaleGroupbyNguoiTao_OB();
                            break;

                    }

                }
            }
            else // admin tong
            {
                switch (khoi)
                {
                    case "IB":

                        var dmChiNhanhs = string.IsNullOrEmpty(Macn) ? BaoCaoVM.Dmchinhanhs.ToList() :
                            BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == Macn).ToList();

                        BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoSale_IB(searchFromDate, searchToDate,
                            dmChiNhanhs, new List<string>(), "");

                        DoanhSoTheoSaleGroupbyNguoiTao_IB();

                        break;

                    case "ND":

                        maCns = string.IsNullOrEmpty(Macn) ? BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList() :
                            BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == Macn).Select(x => x.Macn).ToList();

                        BaoCaoVM.TourNDDTOs = _baoCaoService.DoanhSoTheoSale_ND(searchFromDate, searchToDate, maCns, "");

                        DoanhSoTheoSaleGroupbyNguoiTao_ND();
                        break;

                    case "OB":

                        maCns = string.IsNullOrEmpty(Macn) ? BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList() :
                            BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == Macn).Select(x => x.Macn).ToList();

                        BaoCaoVM.TourOBDTOs = _baoCaoService.DoanhSoTheoSale_OB(searchFromDate, searchToDate, maCns, "");

                        DoanhSoTheoSaleGroupbyNguoiTao_OB();
                        break;

                }

            }

            BaoCaoVM.Khoi = khoi;

            //du lieu
            //int iRowIndex = 6;

            Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#D3D3D3");// ColorTranslator.FromHtml("#D3D3D3");
            Color colorTotalRow = ColorTranslator.FromHtml("#66ccff");
            Color colorThanhLy = ColorTranslator.FromHtml("#7FFF00");
            Color colorChuaThanhLy = ColorTranslator.FromHtml("#FFDEAD");

            int idem = 1;

            switch (khoi)
            {
                case "IB":
                    if (BaoCaoVM.TourIBDTOs != null)
                    {
                        foreach (var vm in BaoCaoVM.TourIBDtosGroupByNguoiTaos)
                        {
                            foreach (var item in vm.TourIBDTOs)
                            {
                                xlSheet.Cells[dong, 1].Value = idem;
                                TrSetCellBorder(xlSheet, dong, 1, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Justify, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                //xlSheet.Cells[dong, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 2].Value = item.Sgtcode;

                                xlSheet.Cells[dong, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;

                                if (item.TrangThai == "3")
                                {
                                    xlSheet.Cells[dong, 2].Style.Fill.BackgroundColor.SetColor(colorThanhLy);
                                }
                                else if (item.TrangThai == "2")
                                {
                                    xlSheet.Cells[dong, 2].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                                }
                                else if (item.TrangThai == "4")
                                {
                                    xlSheet.Cells[dong, 2].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                }
                                else
                                {
                                    xlSheet.Cells[dong, 2].Style.Fill.BackgroundColor.SetColor(Color.White);
                                }

                                TrSetCellBorder(xlSheet, dong, 2, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 3].Value = BaoCaoVM.Companies.FirstOrDefault(x => x.CompanyId == item.CompanyId).Name;
                                TrSetCellBorder(xlSheet, dong, 3, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 4].Value = item.NgayDen.ToShortDateString();
                                TrSetCellBorder(xlSheet, dong, 4, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                //xlSheet.Cells[dong, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 5].Value = item.NgayDi.ToShortDateString();
                                TrSetCellBorder(xlSheet, dong, 5, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                //xlSheet.Cells[dong, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 6].Value = item.ChuDeTour;
                                TrSetCellBorder(xlSheet, dong, 6, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 7].Value = item.TuyenTQ;
                                TrSetCellBorder(xlSheet, dong, 7, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                //xlSheet.Cells[dong, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 8].Value = item.SoKhachDK;
                                TrSetCellBorder(xlSheet, dong, 8, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 9].Value = item.DoanhThuDK;
                                xlSheet.Cells[dong, 9].Style.Numberformat.Format = "#,##0";
                                TrSetCellBorder(xlSheet, dong, 9, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                //xlSheet.Cells[dong, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 10].Value = item.SoKhachTT;
                                TrSetCellBorder(xlSheet, dong, 10, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 11].Value = item.DoanhThuTT;
                                xlSheet.Cells[dong, 11].Style.Numberformat.Format = "#,##0";
                                TrSetCellBorder(xlSheet, dong, 11, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 12].Value = item.NguoiTao;
                                TrSetCellBorder(xlSheet, dong, 12, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                //setBorder(5, 1, dong, 10, xlSheet);

                                dong++;
                                idem++;
                            }

                            xlSheet.Cells[dong, 2].Value = "TỔNG CỘNG:";
                            TrSetCellBorder(xlSheet, dong, 2, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            xlSheet.Cells[dong, 3].Value = "CHƯA THANH LÝ HỢP ĐỒNG:";
                            TrSetCellBorder(xlSheet, dong, 3, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            xlSheet.Cells[dong, 4].Value = vm.TourIBDTOs.FirstOrDefault().ChuaThanhLyHopDong;
                            TrSetCellBorder(xlSheet, dong, 4, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

                            xlSheet.Cells[dong + 1, 3].Value = "ĐÃ THANH LÝ HỢP ĐỒNG:";
                            TrSetCellBorder(xlSheet, dong + 1, 3, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            xlSheet.Cells[dong + 1, 4].Value = vm.TourIBDTOs.FirstOrDefault().DaThanhLyHopDong;
                            TrSetCellBorder(xlSheet, dong + 1, 4, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

                            xlSheet.Cells[dong + 2, 3].Value = "TỔNG CỘNG:";
                            TrSetCellBorder(xlSheet, dong + 2, 3, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            xlSheet.Cells[dong + 2, 4].Value = vm.TourIBDTOs.FirstOrDefault().TongSoKhachTheoSale;
                            TrSetCellBorder(xlSheet, dong + 2, 4, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

                            xlSheet.Cells[dong + 2, 5].Value = vm.TourIBDTOs.FirstOrDefault().TongCongTheoTungSale;
                            TrSetCellBorder(xlSheet, dong + 2, 5, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

                            setBorder(dong, 1, dong + 2, 12, xlSheet);
                            setFontBold(dong, 1, dong + 2, 12, 12, xlSheet);
                            NumberFormat(dong, 1, dong + 2, 5, xlSheet);

                            //xlSheet.Cells[dong, 1, dong, 12].Merge = true;
                            //xlSheet.Cells[dong, 1].Value = vm.NguoiTao;
                            //xlSheet.Cells[dong, 1].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));
                            ////TrSetCellBorder(xlSheet, dong, 1, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            //xlSheet.Cells[dong, 1].Style.Font.Bold = true;

                            //NumberFormat(6, 8, dong + 1, 9, xlSheet);
                            dong = dong + 3;
                            //idem = 1;
                        }

                        xlSheet.Cells[dong, 2].Value = "TỔNG CỘNG:";
                        TrSetCellBorder(xlSheet, dong, 2, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                        xlSheet.Cells[dong, 4].Value = BaoCaoVM.TongSK;
                        TrSetCellBorder(xlSheet, dong, 4, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                        xlSheet.Cells[dong, 5].Value = BaoCaoVM.TongCong.Value;
                        TrSetCellBorder(xlSheet, dong, 5, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

                        NumberFormat(dong, 2, dong, 5, xlSheet);
                        setFontBold(dong, 2, dong, 5, 12, xlSheet);
                        setBorder(dong, 2, dong, 5, xlSheet);
                    }
                    else
                    {
                        SetAlert("No sale.", "warning");
                        return RedirectToAction(nameof(DoanhSoTheoSale));
                    }
                    setCenterAligment(6, 1, 6 + dong + 2, 1, xlSheet);
                    // canh giua code chinhanh
                    setCenterAligment(6, 3, 6 + dong + 2, 3, xlSheet);
                    break;

                case "ND":
                    if (BaoCaoVM.TourNDDTOs != null)
                    {
                        foreach (var vm in BaoCaoVM.TourNDDtosGroupByNguoiTaos)
                        {
                            foreach (var item in vm.TourNDDTOs)
                            {
                                xlSheet.Cells[dong, 1].Value = idem;
                                TrSetCellBorder(xlSheet, dong, 1, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Justify, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                //xlSheet.Cells[dong, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 2].Value = item.Sgtcode;

                                xlSheet.Cells[dong, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;

                                if (item.Trangthai == "3")
                                {
                                    xlSheet.Cells[dong, 2].Style.Fill.BackgroundColor.SetColor(colorThanhLy);
                                }
                                else if (item.Trangthai == "2")
                                {
                                    xlSheet.Cells[dong, 2].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                                }
                                else if (item.Trangthai == "4")
                                {
                                    xlSheet.Cells[dong, 2].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                }
                                else
                                {
                                    xlSheet.Cells[dong, 2].Style.Fill.BackgroundColor.SetColor(Color.White);
                                }

                                TrSetCellBorder(xlSheet, dong, 2, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 3].Value = item.Tenkh;// BaoCaoVM.Companies.FirstOrDefault(x => x.CompanyId == item.CompanyId).Name;
                                TrSetCellBorder(xlSheet, dong, 3, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 4].Value = item.Batdau.Value.ToShortDateString();
                                TrSetCellBorder(xlSheet, dong, 4, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                //xlSheet.Cells[dong, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 5].Value = item.Ketthuc.Value.ToShortDateString();
                                TrSetCellBorder(xlSheet, dong, 5, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                //xlSheet.Cells[dong, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 6].Value = item.Chudetour;
                                TrSetCellBorder(xlSheet, dong, 6, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 7].Value = item.Tuyentq;
                                TrSetCellBorder(xlSheet, dong, 7, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                //xlSheet.Cells[dong, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 8].Value = item.Sokhachdk;
                                TrSetCellBorder(xlSheet, dong, 8, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 9].Value = item.Doanhthudk;
                                xlSheet.Cells[dong, 9].Style.Numberformat.Format = "#,##0";
                                TrSetCellBorder(xlSheet, dong, 9, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                //xlSheet.Cells[dong, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 10].Value = item.Sokhachtt;
                                TrSetCellBorder(xlSheet, dong, 10, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 11].Value = item.Doanhthutt;
                                xlSheet.Cells[dong, 11].Style.Numberformat.Format = "#,##0";
                                TrSetCellBorder(xlSheet, dong, 11, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 12].Value = item.Nguoitao;
                                TrSetCellBorder(xlSheet, dong, 12, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                //setBorder(5, 1, dong, 10, xlSheet);

                                dong++;
                                idem++;
                            }

                            xlSheet.Cells[dong, 2].Value = "TỔNG CỘNG:";
                            TrSetCellBorder(xlSheet, dong, 2, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            xlSheet.Cells[dong, 3].Value = "CHƯA THANH LÝ HỢP ĐỒNG:";
                            TrSetCellBorder(xlSheet, dong, 3, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            xlSheet.Cells[dong, 4].Value = vm.TourNDDTOs.FirstOrDefault().ChuaThanhLyHopDong;
                            TrSetCellBorder(xlSheet, dong, 4, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

                            xlSheet.Cells[dong + 1, 3].Value = "ĐÃ THANH LÝ HỢP ĐỒNG:";
                            TrSetCellBorder(xlSheet, dong + 1, 3, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            xlSheet.Cells[dong + 1, 4].Value = vm.TourNDDTOs.FirstOrDefault().DaThanhLyHopDong;
                            TrSetCellBorder(xlSheet, dong + 1, 4, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

                            xlSheet.Cells[dong + 2, 3].Value = "TỔNG CỘNG:";
                            TrSetCellBorder(xlSheet, dong + 2, 3, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            xlSheet.Cells[dong + 2, 4].Value = vm.TourNDDTOs.FirstOrDefault().TongSoKhachTheoSale;
                            TrSetCellBorder(xlSheet, dong + 2, 4, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

                            xlSheet.Cells[dong + 2, 5].Value = vm.TourNDDTOs.FirstOrDefault().TongCongTheoTungSale;
                            TrSetCellBorder(xlSheet, dong + 2, 5, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

                            setBorder(dong, 1, dong + 2, 12, xlSheet);
                            setFontBold(dong, 1, dong + 2, 12, 12, xlSheet);
                            NumberFormat(dong, 1, dong + 2, 5, xlSheet);

                            //xlSheet.Cells[dong, 1, dong, 12].Merge = true;
                            //xlSheet.Cells[dong, 1].Value = vm.NguoiTao;
                            //xlSheet.Cells[dong, 1].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));
                            ////TrSetCellBorder(xlSheet, dong, 1, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            //xlSheet.Cells[dong, 1].Style.Font.Bold = true;

                            //NumberFormat(6, 8, dong + 1, 9, xlSheet);
                            dong = dong + 3;
                            //idem = 1;
                        }

                        xlSheet.Cells[dong, 2].Value = "TỔNG CỘNG:";
                        TrSetCellBorder(xlSheet, dong, 2, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                        xlSheet.Cells[dong, 4].Value = BaoCaoVM.TongSK;
                        TrSetCellBorder(xlSheet, dong, 4, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                        xlSheet.Cells[dong, 5].Value = BaoCaoVM.TongCong.Value;
                        TrSetCellBorder(xlSheet, dong, 5, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

                        NumberFormat(dong, 2, dong, 5, xlSheet);
                        setFontBold(dong, 2, dong, 5, 12, xlSheet);
                        setBorder(dong, 2, dong, 5, xlSheet);
                    }
                    else
                    {
                        SetAlert("No sale.", "warning");
                        return RedirectToAction(nameof(DoanhSoTheoSale));
                    }
                    setCenterAligment(6, 1, 6 + dong + 2, 1, xlSheet);
                    // canh giua code chinhanh
                    setCenterAligment(6, 3, 6 + dong + 2, 3, xlSheet);
                    break;

                case "OB":
                    if (BaoCaoVM.TourOBDTOs != null)
                    {
                        foreach (var vm in BaoCaoVM.TourOBDtosGroupByNguoiTaos)
                        {
                            foreach (var item in vm.TourOBDTOs)
                            {
                                xlSheet.Cells[dong, 1].Value = idem;
                                TrSetCellBorder(xlSheet, dong, 1, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Justify, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                //xlSheet.Cells[dong, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 2].Value = item.Sgtcode;

                                xlSheet.Cells[dong, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;

                                if (item.Trangthai == "3")
                                {
                                    xlSheet.Cells[dong, 2].Style.Fill.BackgroundColor.SetColor(colorThanhLy);
                                }
                                else if (item.Trangthai == "2")
                                {
                                    xlSheet.Cells[dong, 2].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                                }
                                else if (item.Trangthai == "4")
                                {
                                    xlSheet.Cells[dong, 2].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                }
                                else
                                {
                                    xlSheet.Cells[dong, 2].Style.Fill.BackgroundColor.SetColor(Color.White);
                                }

                                TrSetCellBorder(xlSheet, dong, 2, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 3].Value = item.Tenkh;// BaoCaoVM.Companies.FirstOrDefault(x => x.CompanyId == item.CompanyId).Name;
                                TrSetCellBorder(xlSheet, dong, 3, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 4].Value = item.Batdau.Value.ToShortDateString();
                                TrSetCellBorder(xlSheet, dong, 4, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                //xlSheet.Cells[dong, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 5].Value = item.Ketthuc.Value.ToShortDateString();
                                TrSetCellBorder(xlSheet, dong, 5, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                //xlSheet.Cells[dong, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 6].Value = item.Chudetour;
                                TrSetCellBorder(xlSheet, dong, 6, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 7].Value = item.Tuyentq;
                                TrSetCellBorder(xlSheet, dong, 7, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                //xlSheet.Cells[dong, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 8].Value = item.Sokhachdk;
                                TrSetCellBorder(xlSheet, dong, 8, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 9].Value = item.Doanhthudk;
                                xlSheet.Cells[dong, 9].Style.Numberformat.Format = "#,##0";
                                TrSetCellBorder(xlSheet, dong, 9, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                //xlSheet.Cells[dong, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 10].Value = item.Sokhachtt;
                                TrSetCellBorder(xlSheet, dong, 10, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 11].Value = item.Doanhthutt;
                                xlSheet.Cells[dong, 11].Style.Numberformat.Format = "#,##0";
                                TrSetCellBorder(xlSheet, dong, 11, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                xlSheet.Cells[dong, 12].Value = item.Nguoitao;
                                TrSetCellBorder(xlSheet, dong, 12, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                                // xlSheet.Cells[dong, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                //setBorder(5, 1, dong, 10, xlSheet);

                                dong++;
                                idem++;
                            }

                            xlSheet.Cells[dong, 2].Value = "TỔNG CỘNG:";
                            TrSetCellBorder(xlSheet, dong, 2, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            xlSheet.Cells[dong, 3].Value = "CHƯA THANH LÝ HỢP ĐỒNG:";
                            TrSetCellBorder(xlSheet, dong, 3, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            xlSheet.Cells[dong, 4].Value = vm.TourOBDTOs.FirstOrDefault().ChuaThanhLyHopDong;
                            TrSetCellBorder(xlSheet, dong, 4, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

                            xlSheet.Cells[dong + 1, 3].Value = "ĐÃ THANH LÝ HỢP ĐỒNG:";
                            TrSetCellBorder(xlSheet, dong + 1, 3, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            xlSheet.Cells[dong + 1, 4].Value = vm.TourOBDTOs.FirstOrDefault().DaThanhLyHopDong;
                            TrSetCellBorder(xlSheet, dong + 1, 4, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

                            xlSheet.Cells[dong + 2, 3].Value = "TỔNG CỘNG:";
                            TrSetCellBorder(xlSheet, dong + 2, 3, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            xlSheet.Cells[dong + 2, 4].Value = vm.TourOBDTOs.FirstOrDefault().TongSoKhachTheoSale;
                            TrSetCellBorder(xlSheet, dong + 2, 4, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

                            xlSheet.Cells[dong + 2, 5].Value = vm.TourOBDTOs.FirstOrDefault().TongCongTheoTungSale;
                            TrSetCellBorder(xlSheet, dong + 2, 5, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

                            setBorder(dong, 1, dong + 2, 12, xlSheet);
                            setFontBold(dong, 1, dong + 2, 12, 12, xlSheet);
                            NumberFormat(dong, 1, dong + 2, 5, xlSheet);

                            //xlSheet.Cells[dong, 1, dong, 12].Merge = true;
                            //xlSheet.Cells[dong, 1].Value = vm.NguoiTao;
                            //xlSheet.Cells[dong, 1].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));
                            ////TrSetCellBorder(xlSheet, dong, 1, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            //xlSheet.Cells[dong, 1].Style.Font.Bold = true;

                            //NumberFormat(6, 8, dong + 1, 9, xlSheet);
                            dong = dong + 3;
                            //idem = 1;
                        }

                        xlSheet.Cells[dong, 2].Value = "TỔNG CỘNG:";
                        TrSetCellBorder(xlSheet, dong, 2, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                        xlSheet.Cells[dong, 4].Value = BaoCaoVM.TongSK;
                        TrSetCellBorder(xlSheet, dong, 4, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                        xlSheet.Cells[dong, 5].Value = BaoCaoVM.TongCong.Value;
                        TrSetCellBorder(xlSheet, dong, 5, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

                        NumberFormat(dong, 2, dong, 5, xlSheet);
                        setFontBold(dong, 2, dong, 5, 12, xlSheet);
                        setBorder(dong, 2, dong, 5, xlSheet);
                    }
                    else
                    {
                        SetAlert("No sale.", "warning");
                        return RedirectToAction(nameof(DoanhSoTheoSale));
                    }
                    setCenterAligment(6, 1, 6 + dong + 2, 1, xlSheet);
                    // canh giua code chinhanh
                    setCenterAligment(6, 3, 6 + dong + 2, 3, xlSheet);
                    break;
            }

            //dong++;
            //// Merger cot 4,5 ghi tổng tiền
            //setRightAligment(dong, 3, dong, 3, xlSheet);
            //xlSheet.Cells[dong, 1, dong, 2].Merge = true;
            //xlSheet.Cells[dong, 1].Value = "Tổng tiền: ";

            // Sum tổng tiền
            // xlSheet.Cells[dong, 5].Value = "TC:";
            //DateTimeFormat(6, 4, 6 + d.Count(), 4, xlSheet);
            // DateTimeFormat(6, 4, 9, 4, xlSheet);
            // setCenterAligment(6, 4, 9, 4, xlSheet);
            // xlSheet.Cells[dong, 6].Formula = "SUM(F6:F" + (6 + d.Count() - 1) + ")";

            //setBorder(5, 1, 5 + d.Count() + 2, 10, xlSheet);

            //setFontBold(5, 1, 5, 8, 11, xlSheet);
            //setFontSize(6, 1, 6 + d.Count() + 2, 8, 11, xlSheet);
            // canh giua cot stt
            //setCenterAligment(6, 1, 6 + dong + 2, 1, xlSheet);
            // canh giua code chinhanh
            //setCenterAligment(6, 3, 6 + dong + 2, 3, xlSheet);
            // NumberFormat(6, 6, 6 + d.Count(), 6, xlSheet);
            // định dạng số cot, đơn giá, thành tiền tong cong
            // NumberFormat(6, 8, dong, 9, xlSheet);

            // setBorder(dong, 5, dong, 6, xlSheet);
            // setFontBold(dong, 5, dong, 6, 12, xlSheet);

            //xlSheet.View.FreezePanes(6, 20);

            //end du lieu

            byte[] fileContents;
            try
            {
                fileContents = ExcelApp.GetAsByteArray();
                switch (khoi)
                {
                    case "IB":
                        return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "DoanhSoTheoSale_IB_" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ".xlsx");

                    case "ND":
                        return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "DoanhSoTheoSale_ND_" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ".xlsx");

                    case "OB":
                        return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "DoanhSoTheoSale_OB_" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ".xlsx");

                }

            }
            catch (Exception)
            {
                throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> DoanhSoTheoSaleExcelChart(string searchFromDate = null, string searchToDate = null,
            string Macn = null, string khoi = null)
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // from session
            var user = HttpContext.Session.Get<Users>("loginUser");

            //// moi load vao
            if (string.IsNullOrEmpty(searchFromDate) && string.IsNullOrEmpty(searchToDate))
            {
                //var currentTime = DateTime.Now;
                //string TuNgayDenNgayString = LoadTuNgayDenNgay(currentTime.Month.ToString(), currentTime.Month.ToString(), currentTime.Year.ToString());
                //searchFromDate = TuNgayDenNgayString.Split('-')[0];
                //searchToDate = TuNgayDenNgayString.Split('-')[1];

                searchFromDate = DateTime.Now.ToShortDateString();
                searchToDate = (new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).AddDays(-1)).ToString();
            }
            else // da chon ngay thang - // check date correct
            {
                try
                {
                    Convert.ToDateTime(searchFromDate);
                    Convert.ToDateTime(searchToDate);
                }
                catch (Exception)
                {
                    //BaoCaoVM = new BaoCaoViewModel()
                    //{
                    //    Dmchinhanhs = _unitOfWork.dmChiNhanhRepository.GetAll(),
                    //    Tourkinds = _unitOfWork.tourKindRepository.GetAll(),
                    //    TourBaoCaoDtosTheoNgay = new TourBaoCaoDtosTheoNgay()
                    //};

                    ViewBag.Macn = Macn;
                    ViewBag.searchFromDate = searchFromDate;
                    ViewBag.searchToDate = searchToDate;

                    ModelState.AddModelError("", "Lỗi định dạng ngày tháng.");
                    return View(BaoCaoVM);
                }
            }

            ViewBag.Macn = Macn;
            ViewBag.khoi = khoi;
            ViewBag.searchFromDate = searchFromDate;
            ViewBag.searchToDate = searchToDate;

            string fromTo = "";
            ExcelPackage ExcelApp = new ExcelPackage();
            ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
            // Định dạng chiều dài cho cột
            xlSheet.Column(1).Width = 20;// STT
            xlSheet.Column(2).Width = 20;// Code đoàn

            xlSheet.Cells[1, 1].Value = "CÔNG TY DVLH SAIGONTOURIST";
            xlSheet.Cells[1, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
            xlSheet.Cells[1, 1, 1, 17].Merge = true;

            xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH SỐ THEO SALES";
            xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
            xlSheet.Cells[2, 1, 2, 17].Merge = true;
            setCenterAligment(2, 1, 2, 17, xlSheet);
            // dinh dang tu ngay den ngay
            if (string.IsNullOrEmpty(searchFromDate) && string.IsNullOrEmpty(searchToDate))
            {
                ViewBag.searchFromDate = searchFromDate;
                ViewBag.searchToDate = searchToDate;
                SetAlert("Từ ngày đến ngày không được để trống.", "warning");
                return RedirectToAction(nameof(DoanhSoTheoSale));
            }
            if (searchFromDate == searchToDate)
            {
                fromTo = "Ngày: " + searchFromDate;
            }
            else
            {
                fromTo = "Từ ngày: " + searchFromDate + " đến ngày: " + searchToDate;
            }
            xlSheet.Cells[3, 1].Value = fromTo;
            xlSheet.Cells[3, 1, 3, 17].Merge = true;
            xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
            setCenterAligment(3, 1, 3, 17, xlSheet);

            // Tạo header
            xlSheet.Cells[5, 1].Value = "Sale";
            xlSheet.Cells[5, 2].Value = "Doanh số ";

            xlSheet.Cells[5, 1, 5, 2].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));
            setBorder(5, 1, 5, 2, xlSheet);
            setCenterAligment(5, 1, 5, 2, xlSheet);

            // do du lieu tu table
            int dong = 6;

            BaoCaoVM.Companies = _baoCaoService.GetCompanies();
            BaoCaoVM.Dmchinhanhs = _baoCaoService.GetAllChiNhanh();//.Where(x => !string.IsNullOrEmpty(x.Macn));            
            BaoCaoVM.Khois_KD = KhoiViewModels_KD();
            List<string> maCns = new List<string>();

            if (user.RoleId != 8) // 8: Admins
            {
                if (user.RoleId == 9) // 9: Users
                {

                    // hien thi tren view
                    BaoCaoVM.Dmchinhanhs = new List<Dmchinhanh>() { new Dmchinhanh() { Macn = user.Chinhanh } };
                    BaoCaoVM.Khois_KD = KhoiViewModels_KD().Where(x => x.Name == user.Khoi);

                    switch (khoi)
                    {
                        case "IB":
                            if (!string.IsNullOrEmpty(user.PhongBanQL)) // co ql phongban khac' --> IB
                            {

                                var phongBanQL = user.PhongBanQL.Split(',').ToList();
                                BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoSale_IB(searchFromDate, searchToDate,
                                    BaoCaoVM.Dmchinhanhs.ToList(), phongBanQL, "");
                            }
                            else
                            {
                                BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoSale_IB(searchFromDate, searchToDate,
                                    BaoCaoVM.Dmchinhanhs.ToList(), new List<string>(), user.Username);
                                //BaoCaoVM.TourIBDTOs = BaoCaoVM.TourIBDTOs.Where(x => x.NguoiTao == user.Username);
                            }
                            DoanhSoTheoSaleGroupbyNguoiTao_IB();
                            break;

                        case "ND":
                            // do tournd ko co daily -> lay theo chinhanh
                            BaoCaoVM.TourNDDTOs = _baoCaoService.DoanhSoTheoSale_ND(searchFromDate, searchToDate,
                                BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList(), user.Username);
                            //BaoCaoVM.TourNDDTOs = BaoCaoVM.TourNDDTOs.Where(x => x.Nguoitao == user.Username);
                            DoanhSoTheoSaleGroupbyNguoiTao_ND();
                            break;

                        case "OB":
                            // do tourob ko co daily -> lay theo chinhanh
                            BaoCaoVM.TourOBDTOs = _baoCaoService.DoanhSoTheoSale_OB(searchFromDate, searchToDate,
                                BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList(), user.Username);
                            //BaoCaoVM.TourOBDTOs = BaoCaoVM.TourOBDTOs.Where(x => x.Nguoitao == user.Username);
                            DoanhSoTheoSaleGroupbyNguoiTao_OB();
                            break;

                    }

                }
                else // admin khuvuc
                {
                    // phanKhuCNs = co cn QL
                    var role1 = await _baoCaoService.GetRoleById(user.RoleId);
                    var listMaCN = role1.ChiNhanhQL.Split(',').ToList();
                    maCns.AddRange(listMaCN);

                    // hien thi tren view
                    BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Where(x => maCns.Any(y => x.Macn == y));
                    // lay het 3 khoi

                    switch (khoi)
                    {
                        case "IB":

                            var dmChiNhanhs = string.IsNullOrEmpty(Macn) ? BaoCaoVM.Dmchinhanhs.ToList() :
                                BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == Macn).ToList();

                            BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoSale_IB(searchFromDate, searchToDate,
                                    dmChiNhanhs, new List<string>(), "");

                            DoanhSoTheoSaleGroupbyNguoiTao_IB();
                            break;

                        case "ND":
                            maCns = string.IsNullOrEmpty(Macn) ? BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList() :
                                new List<string> { Macn };
                            // do tournd ko co daily -> lay theo chinhanh
                            BaoCaoVM.TourNDDTOs = _baoCaoService.DoanhSoTheoSale_ND(searchFromDate, searchToDate,
                                    BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList(), "");

                            DoanhSoTheoSaleGroupbyNguoiTao_ND();
                            break;

                        case "OB":
                            maCns = string.IsNullOrEmpty(Macn) ? BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList() :
                                new List<string> { Macn };

                            // do tourOB ko co daily -> lay theo chinhanh
                            BaoCaoVM.TourOBDTOs = _baoCaoService.DoanhSoTheoSale_OB(searchFromDate, searchToDate,
                                    BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList(), "");

                            DoanhSoTheoSaleGroupbyNguoiTao_OB();
                            break;

                    }

                }
            }
            else // admin tong
            {
                switch (khoi)
                {
                    case "IB":

                        var dmChiNhanhs = string.IsNullOrEmpty(Macn) ? BaoCaoVM.Dmchinhanhs.ToList() :
                            BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == Macn).ToList();

                        BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoSale_IB(searchFromDate, searchToDate,
                            dmChiNhanhs, new List<string>(), "");

                        DoanhSoTheoSaleGroupbyNguoiTao_IB();

                        break;

                    case "ND":

                        maCns = string.IsNullOrEmpty(Macn) ? BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList() :
                            BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == Macn).Select(x => x.Macn).ToList();

                        BaoCaoVM.TourNDDTOs = _baoCaoService.DoanhSoTheoSale_ND(searchFromDate, searchToDate, maCns, "");

                        DoanhSoTheoSaleGroupbyNguoiTao_ND();
                        break;

                    case "OB":

                        maCns = string.IsNullOrEmpty(Macn) ? BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList() :
                            BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == Macn).Select(x => x.Macn).ToList();

                        BaoCaoVM.TourOBDTOs = _baoCaoService.DoanhSoTheoSale_OB(searchFromDate, searchToDate, maCns, "");

                        DoanhSoTheoSaleGroupbyNguoiTao_OB();
                        break;

                }

            }

            BaoCaoVM.Khoi = khoi;

            //du lieu
            //int iRowIndex = 6;

            Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#D3D3D3");// ColorTranslator.FromHtml("#D3D3D3");
            Color colorTotalRow = ColorTranslator.FromHtml("#66ccff");
            Color colorThanhLy = ColorTranslator.FromHtml("#7FFF00");
            Color colorChuaThanhLy = ColorTranslator.FromHtml("#FFDEAD");

            switch (khoi)
            {

                case "IB":

                    IEnumerable<TourDTOChart> tourDTOCharts = BaoCaoVM.TourIBDTOs.GroupBy(x => x.NguoiTao).Select(x => new TourDTOChart
                    {
                        TenTheoCN = x.First().MaCNTao + " - " + x.First().NguoiTao,
                        MaCN = x.First().MaCNTao,
                        NguoiTao = x.First().NguoiTao,
                        DoanhThuTT = x.Sum(x => x.DoanhThuTT)
                    });

                    var iTotalRow1 = tourDTOCharts.Count();

                    if (tourDTOCharts != null)
                    {
                        foreach (var item in tourDTOCharts)
                        {
                            xlSheet.Cells[dong, 1].Value = item.TenTheoCN;
                            TrSetCellBorder(xlSheet, dong, 1, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            // xlSheet.Cells[dong, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            xlSheet.Cells[dong, 2].Value = item.DoanhThuTT;
                            TrSetCellBorder(xlSheet, dong, 2, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            //xlSheet.Cells[dong, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            NumberFormat(dong, 2, dong, 2, xlSheet);
                            dong++;
                        }
                    }
                    else
                    {
                        SetAlert("No sale.", "warning");
                        return RedirectToAction(nameof(DoanhSoTheoSale));
                    }

                    #region "Chart"

                    // chart
                    var lineChart = xlSheet.Drawings.AddChart("lineChart", eChartType.ColumnClustered);
                    //var lineChart = ExcelApp.Workbook.Worksheets.AddChart("lineChart", eChartType.ColumnClustered);
                    //xlSheet.Cells["A1"].LoadFromDataTable(dt1, false);
                    //set the title
                    lineChart.Title.Font.LatinFont = "Times New Roman";
                    lineChart.Title.Font.Size = 16;
                    lineChart.Title.Font.Bold = true;
                    lineChart.Title.Text = "Đoàn đi tour từ ngày " + fromTo;
                    //create the ranges for the chart
                    iTotalRow1 = iTotalRow1 + 6;//+1 do bat dau tu row a2,b2
                    var rangeLabel = xlSheet.Cells["A6:A" + iTotalRow1];
                    var range1 = xlSheet.Cells["B6:B" + iTotalRow1];
                    //var range2 = xlSheet.Cells["B3:K3"];
                    //add the ranges to the chart
                    var lineSerires = (ExcelBarChartSerie)lineChart.Series.Add(range1, rangeLabel);
                    //lineChart.Series.Add(range2, rangeLabel);

                    lineSerires.DataLabel.Font.LatinFont = "Times New Roman";
                    lineSerires.DataLabel.Font.Size = 13;
                    //set the names of the legend
                    lineChart.Series[0].Header = "Doanh số";
                    //lineChart.Series[1].Header = xlSheet.Cells["A3"].Value.ToString();
                    //position of the legend
                    lineChart.Legend.Position = eLegendPosition.Right;

                    //size of the chart
                    if (iTotalRow1 < 10)
                    {
                        lineChart.SetSize(800, 600);
                    }
                    else if (iTotalRow1 >= 10 && iTotalRow1 < 20)
                    {
                        lineChart.SetSize(1024, 786);
                    }
                    else
                    {
                        lineChart.SetSize(1920, 1080);
                    }

                    //add the chart at cell B6
                    lineChart.SetPosition(4, 0, 4, 0);

                    xlSheet.Cells.AutoFitColumns();
                    #endregion "Chart"

                    // chart
                    break;

                case "ND":

                    IEnumerable<TourDTOChart> tourDTOCharts_ND = BaoCaoVM.TourNDDTOs.GroupBy(x => x.Nguoitao).Select(x => new TourDTOChart
                    {
                        TenTheoCN = x.First().Chinhanh + " - " + x.First().Nguoitao,
                        MaCN = x.First().Chinhanh,
                        NguoiTao = x.First().Nguoitao,
                        DoanhThuTT = x.Sum(x => x.Doanhthutt == null ? 0 : x.Doanhthutt.Value)
                    });

                    var iTotalRow1_ND = tourDTOCharts_ND.Count();

                    if (tourDTOCharts_ND != null)
                    {
                        foreach (var item in tourDTOCharts_ND)
                        {
                            xlSheet.Cells[dong, 1].Value = item.TenTheoCN;
                            TrSetCellBorder(xlSheet, dong, 1, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            // xlSheet.Cells[dong, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            xlSheet.Cells[dong, 2].Value = item.DoanhThuTT;
                            TrSetCellBorder(xlSheet, dong, 2, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            //xlSheet.Cells[dong, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            NumberFormat(dong, 2, dong, 2, xlSheet);
                            dong++;
                        }
                    }
                    else
                    {
                        SetAlert("No sale.", "warning");
                        return RedirectToAction(nameof(DoanhSoTheoSale));
                    }

                    #region "Chart"

                    // chart
                    var lineChart_ND = xlSheet.Drawings.AddChart("lineChart", eChartType.ColumnClustered);
                    //var lineChart = ExcelApp.Workbook.Worksheets.AddChart("lineChart", eChartType.ColumnClustered);
                    //xlSheet.Cells["A1"].LoadFromDataTable(dt1, false);
                    //set the title
                    lineChart_ND.Title.Font.LatinFont = "Times New Roman";
                    lineChart_ND.Title.Font.Size = 16;
                    lineChart_ND.Title.Font.Bold = true;
                    lineChart_ND.Title.Text = "Đoàn đi tour từ ngày " + fromTo;
                    //create the ranges for the chart
                    iTotalRow1_ND = iTotalRow1_ND + 6;//+1 do bat dau tu row a2,b2
                    var rangeLabel_ND = xlSheet.Cells["A6:A" + iTotalRow1_ND];
                    var range1_ND = xlSheet.Cells["B6:B" + iTotalRow1_ND];
                    //var range2 = xlSheet.Cells["B3:K3"];
                    //add the ranges to the chart
                    var lineSerires_ND = (ExcelBarChartSerie)lineChart_ND.Series.Add(range1_ND, rangeLabel_ND);
                    //lineChart.Series.Add(range2, rangeLabel);

                    lineSerires_ND.DataLabel.Font.LatinFont = "Times New Roman";
                    lineSerires_ND.DataLabel.Font.Size = 13;
                    //set the names of the legend
                    lineChart_ND.Series[0].Header = "Doanh số";
                    //lineChart.Series[1].Header = xlSheet.Cells["A3"].Value.ToString();
                    //position of the legend
                    lineChart_ND.Legend.Position = eLegendPosition.Right;

                    //size of the chart
                    if (iTotalRow1_ND < 10)
                    {
                        lineChart_ND.SetSize(800, 600);
                    }
                    else if (iTotalRow1_ND >= 10 && iTotalRow1_ND < 20)
                    {
                        lineChart_ND.SetSize(1024, 786);
                    }
                    else
                    {
                        lineChart_ND.SetSize(1920, 1080);
                    }

                    //add the chart at cell B6
                    lineChart_ND.SetPosition(4, 0, 4, 0);

                    xlSheet.Cells.AutoFitColumns();
                    #endregion "Chart"

                    // chart
                    break;
                case "OB":

                    IEnumerable<TourDTOChart> tourDTOCharts_OB = BaoCaoVM.TourOBDTOs.GroupBy(x => x.Nguoitao).Select(x => new TourDTOChart
                    {
                        TenTheoCN = x.First().Chinhanh + " - " + x.First().Nguoitao,
                        MaCN = x.First().Chinhanh,
                        NguoiTao = x.First().Nguoitao,
                        DoanhThuTT = x.Sum(x => x.Doanhthutt == null ? 0 : x.Doanhthutt.Value)
                    });

                    var iTotalRow1_OB = tourDTOCharts_OB.Count();

                    if (tourDTOCharts_OB != null)
                    {
                        foreach (var item in tourDTOCharts_OB)
                        {
                            xlSheet.Cells[dong, 1].Value = item.TenTheoCN;
                            TrSetCellBorder(xlSheet, dong, 1, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            // xlSheet.Cells[dong, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            xlSheet.Cells[dong, 2].Value = item.DoanhThuTT;
                            TrSetCellBorder(xlSheet, dong, 2, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
                            //xlSheet.Cells[dong, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            NumberFormat(dong, 2, dong, 2, xlSheet);
                            dong++;
                        }
                    }
                    else
                    {
                        SetAlert("No sale.", "warning");
                        return RedirectToAction(nameof(DoanhSoTheoSale));
                    }

                    #region "Chart"

                    // chart
                    var lineChart_OB = xlSheet.Drawings.AddChart("lineChart", eChartType.ColumnClustered);
                    //var lineChart = ExcelApp.Workbook.Worksheets.AddChart("lineChart", eChartType.ColumnClustered);
                    //xlSheet.Cells["A1"].LoadFromDataTable(dt1, false);
                    //set the title
                    lineChart_OB.Title.Font.LatinFont = "Times New Roman";
                    lineChart_OB.Title.Font.Size = 16;
                    lineChart_OB.Title.Font.Bold = true;
                    lineChart_OB.Title.Text = "Đoàn đi tour từ ngày " + fromTo;
                    //create the ranges for the chart
                    iTotalRow1_OB = iTotalRow1_OB + 6;//+1 do bat dau tu row a2,b2
                    var rangeLabel_OB = xlSheet.Cells["A6:A" + iTotalRow1_OB];
                    var range1_OB = xlSheet.Cells["B6:B" + iTotalRow1_OB];
                    //var range2 = xlSheet.Cells["B3:K3"];
                    //add the ranges to the chart
                    var lineSerires_OB = (ExcelBarChartSerie)lineChart_OB.Series.Add(range1_OB, rangeLabel_OB);
                    //lineChart.Series.Add(range2, rangeLabel);

                    lineSerires_OB.DataLabel.Font.LatinFont = "Times New Roman";
                    lineSerires_OB.DataLabel.Font.Size = 13;
                    //set the names of the legend
                    lineChart_OB.Series[0].Header = "Doanh số";
                    //lineChart.Series[1].Header = xlSheet.Cells["A3"].Value.ToString();
                    //position of the legend
                    lineChart_OB.Legend.Position = eLegendPosition.Right;

                    //size of the chart
                    if (iTotalRow1_OB < 10)
                    {
                        lineChart_OB.SetSize(800, 600);
                    }
                    else if (iTotalRow1_OB >= 10 && iTotalRow1_OB < 20)
                    {
                        lineChart_OB.SetSize(1024, 786);
                    }
                    else
                    {
                        lineChart_OB.SetSize(1920, 1080);
                    }

                    //add the chart at cell B6
                    lineChart_OB.SetPosition(4, 0, 4, 0);

                    xlSheet.Cells.AutoFitColumns();
                    #endregion "Chart"

                    // chart
                    break;
            }


            //dong++;
            //// Merger cot 4,5 ghi tổng tiền
            //setRightAligment(dong, 3, dong, 3, xlSheet);
            //xlSheet.Cells[dong, 1, dong, 2].Merge = true;
            //xlSheet.Cells[dong, 1].Value = "Tổng tiền: ";

            // Sum tổng tiền
            // xlSheet.Cells[dong, 5].Value = "TC:";
            //DateTimeFormat(6, 4, 6 + d.Count(), 4, xlSheet);
            // DateTimeFormat(6, 4, 9, 4, xlSheet);
            // setCenterAligment(6, 4, 9, 4, xlSheet);
            // xlSheet.Cells[dong, 6].Formula = "SUM(F6:F" + (6 + d.Count() - 1) + ")";

            //setBorder(5, 1, 5 + d.Count() + 2, 10, xlSheet);

            //setFontBold(5, 1, 5, 8, 11, xlSheet);
            //setFontSize(6, 1, 6 + d.Count() + 2, 8, 11, xlSheet);
            // canh giua cot stt
            //setCenterAligment(6, 1, 6 + dong + 2, 1, xlSheet);
            // canh giua code chinhanh
            //setCenterAligment(6, 3, 6 + dong + 2, 3, xlSheet);
            // NumberFormat(6, 6, 6 + d.Count(), 6, xlSheet);
            // định dạng số cot, đơn giá, thành tiền tong cong
            // NumberFormat(6, 8, dong, 9, xlSheet);

            // setBorder(dong, 5, dong, 6, xlSheet);
            // setFontBold(dong, 5, dong, 6, 12, xlSheet);

            //xlSheet.View.FreezePanes(6, 20);

            //end du lieu

            byte[] fileContents;
            try
            {
                fileContents = ExcelApp.GetAsByteArray();
                switch (khoi)
                {
                    case "IB":
                        return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "DoanhSoTheoSaleExcelChart_IB_" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ".xlsx");

                    case "ND":
                        return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "DoanhSoTheoSaleExcelChart_ND_" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ".xlsx");

                    case "OB":
                        return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "DoanhSoTheoSaleExcelChart_OB_" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ".xlsx");

                    default:
                        return NoContent();

                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion


        #region Doanh so theo thang

        public async Task<IActionResult> DoanhSoTheoThang(string tuThang1, string denThang1, string nam1,
                                              string tuThang2, string denThang2, string nam2,
                                              string maCn = null, string khoi = null)
        {

            // from session
            var user = HttpContext.Session.Get<Users>("loginUser");

            // moi load vao
            var currentYear = DateTime.Now.Year;
            var previousYear = currentYear - 1;

            BaoCaoVM = new BaoCaoViewModel()
            {
                Dmchinhanhs = _unitOfWork.dmChiNhanhRepository.GetAll(),
                Thangs = Thangs()
            };

            if (string.IsNullOrEmpty(nam1))
            {
                nam1 = previousYear.ToString();
            }

            if (string.IsNullOrEmpty(nam2))
            {
                nam2 = currentYear.ToString();
            }

            tuThang1 ??= "1";
            denThang1 ??= "12";
            tuThang2 ??= "1";
            denThang2 ??= "12";

            ViewBag.tuThang1 = tuThang1;
            ViewBag.denThang1 = denThang1;
            ViewBag.nam1 = nam1;

            ViewBag.tuThang2 = tuThang2;
            ViewBag.denThang2 = denThang2;
            ViewBag.nam2 = nam2;

            ViewBag.chiNhanh = maCn ?? user.Chinhanh;
            ViewBag.khoi = khoi ?? user.Khoi;

            // Error: bat dau phai nho hon ket thuc
            if ((int.Parse(tuThang1) > int.Parse(denThang1)) || (int.Parse(tuThang2) > int.Parse(denThang2)))
            {
                ModelState.AddModelError("", "Ngày bắt đầu phải nhỏ hơn ngày kết thúc!");
                //1
                //.TourBaoCaoTheoThangs1 = TourBaoCaoTheoThangViewModels("1", "12", nam1, BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList());
                //1

                //2
                //BaoCaoVM.TourBaoCaoTheoThangs2 = TourBaoCaoTheoThangViewModels("1", "12", nam2, BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList());
                //2
                return View(BaoCaoVM);
            }
            // Error: bat dau phai nho hon ket thuc

            BaoCaoVM.Dmchinhanhs = _baoCaoService.GetAllChiNhanh().Where(x => !string.IsNullOrEmpty(x.Macn));
            BaoCaoVM.Khois_KD = KhoiViewModels_KD();
            List<string> maCns = new List<string>();
            if (user.RoleId != 8) // 8: Admins
            {
                if (user.RoleId == 9) // 9: Users
                {

                    List<Dmchinhanh> listCN = new List<Dmchinhanh>();
                    listCN.Add(BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == user.Chinhanh).FirstOrDefault());
                    BaoCaoVM.Dmchinhanhs = new List<Dmchinhanh>() { new Dmchinhanh() { Macn = user.Chinhanh } };
                    BaoCaoVM.Khois_KD = KhoiViewModels_KD().Where(x => x.Name == user.Khoi);

                    switch (khoi)
                    {
                        case "IB":
                            if (!string.IsNullOrEmpty(user.PhongBanQL)) // co ql phongban khac' --> IB
                            {

                                var phongBanQL = user.PhongBanQL.Split(',').ToList();
                                // 1
                                BaoCaoVM.TourBaoCaoTheoThangs1_IB = TourBaoCaoTheoThangViewModels_IB(tuThang1, denThang1, nam1, listCN, phongBanQL, "");
                                // 1

                                // 2
                                BaoCaoVM.TourBaoCaoTheoThangs2_IB = TourBaoCaoTheoThangViewModels_IB(tuThang2, denThang2, nam2, listCN, phongBanQL, "");
                                // 2


                            }
                            else
                            {

                                //1
                                BaoCaoVM.TourBaoCaoTheoThangs1_IB = TourBaoCaoTheoThangViewModels_IB(tuThang1, denThang1, nam1, listCN, new List<string>(), user.Username);
                                //1

                                //2
                                BaoCaoVM.TourBaoCaoTheoThangs2_IB = TourBaoCaoTheoThangViewModels_IB(tuThang2, denThang2, nam2, listCN, new List<string>(), user.Username);
                                //2

                            }
                            break;

                        case "ND":
                            // do tournd ko co daily -> lay theo chinhanh
                            // do saledoan ND ko co STN => STN = STS
                            var maCn1 = user.Chinhanh == "STN" ? "STS" : user.Chinhanh;
                            maCns = new List<string>();
                            maCns.Add(maCn1);
                            //1
                            BaoCaoVM.TourBaoCaoTheoThangs1_ND = TourBaoCaoTheoThangViewModels_ND(tuThang1, denThang1, nam1, maCns, user.Username);
                            //1

                            //2
                            BaoCaoVM.TourBaoCaoTheoThangs2_ND = TourBaoCaoTheoThangViewModels_ND(tuThang2, denThang2, nam2, maCns, user.Username);
                            //2

                            break;

                            //case "OB":
                            //    // do tourob ko co daily -> lay theo chinhanh
                            //    BaoCaoVM.TourOBDTOs = _baoCaoService.DoanhSoTheoChiNhanh_OB(searchFromDate, searchToDate, BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList());
                            //    //BaoCaoVM.TourOBDTOs = BaoCaoVM.TourOBDTOs.Where(x => x.Nguoitao == user.Username);
                            //    DoanhSoTheoSaleGroupbyNguoiTao_OB();
                            //    break;

                    }


                }
                else // admin khu vuc
                {
                    // phanKhuCNs = co cn QL
                    var role1 = await _baoCaoService.GetRoleById(user.RoleId);
                    var listMaCN = role1.ChiNhanhQL.Split(',').ToList();
                    maCns.AddRange(listMaCN);

                    // hien thi tren view
                    BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Where(x => maCns.Any(y => x.Macn == y));
                    if (!string.IsNullOrEmpty(khoi)) // luc chon khoi
                    {
                        switch (khoi)
                        {
                            case "IB":

                                if (!string.IsNullOrEmpty(maCn)) // co' chon chinhanh | else lấy hết
                                {
                                    BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == maCn);
                                }
                                //1
                                BaoCaoVM.TourBaoCaoTheoThangs1_IB = TourBaoCaoTheoThangViewModels_IB(tuThang1, denThang1, nam1, BaoCaoVM.Dmchinhanhs.ToList(),
                                    new List<string>(), "");
                                //1

                                //2
                                BaoCaoVM.TourBaoCaoTheoThangs2_IB = TourBaoCaoTheoThangViewModels_IB(tuThang2, denThang2, nam2, BaoCaoVM.Dmchinhanhs.ToList(),
                                    new List<string>(), "");
                                //2

                                //if (string.IsNullOrEmpty(maCn)) // ko chon cn => lay het cn dang co'
                                //{
                                //    //1
                                //    BaoCaoVM.TourBaoCaoTheoThangs1_IB = TourBaoCaoTheoThangViewModels_IB(tuThang1, denThang1, nam1, BaoCaoVM.Dmchinhanhs.ToList(),
                                //        new List<string>(), "");
                                //    //1

                                //    //2
                                //    BaoCaoVM.TourBaoCaoTheoThangs2_IB = TourBaoCaoTheoThangViewModels_IB(tuThang2, denThang2, nam2, BaoCaoVM.Dmchinhanhs.ToList(),
                                //        new List<string>(), "");
                                //    //2

                                //}
                                //else // co' chon chinhanh
                                //{
                                //    BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == maCn);
                                //    //1
                                //    BaoCaoVM.TourBaoCaoTheoThangs1_IB = TourBaoCaoTheoThangViewModels_IB(tuThang1, denThang1, nam1, BaoCaoVM.Dmchinhanhs.ToList(),
                                //        new List<string>(), "");
                                //    //1

                                //    //2
                                //    BaoCaoVM.TourBaoCaoTheoThangs2_IB = TourBaoCaoTheoThangViewModels_IB(tuThang2, denThang2, nam2, BaoCaoVM.Dmchinhanhs.ToList(),
                                //        new List<string>(), "");
                                //    //2

                                //}

                                break;

                            case "ND":
                                // do tournd ko co daily -> lay theo chinhanh
                                if (!string.IsNullOrEmpty(maCn)) // co' chon chinhanh
                                {
                                    // do saledoan ND ko co STN => STN = STS
                                    maCn = maCn == "STN" ? "STS" : maCn;
                                    maCns = new List<string>();
                                    maCns.Add(maCn);
                                }
                                //1
                                BaoCaoVM.TourBaoCaoTheoThangs1_ND = TourBaoCaoTheoThangViewModels_ND(tuThang1, denThang1, nam1, maCns, "");
                                //1

                                //2
                                BaoCaoVM.TourBaoCaoTheoThangs2_ND = TourBaoCaoTheoThangViewModels_ND(tuThang2, denThang2, nam2, maCns, "");
                                //2

                                //if (string.IsNullOrEmpty(maCn)) // ko chon cn => lay het cn dang co'
                                //{
                                //    //1
                                //    BaoCaoVM.TourBaoCaoTheoThangs1_ND = TourBaoCaoTheoThangViewModels_ND(tuThang1, denThang1, nam1, maCns, "");
                                //    //1

                                //    //2
                                //    BaoCaoVM.TourBaoCaoTheoThangs2_ND = TourBaoCaoTheoThangViewModels_ND(tuThang2, denThang2, nam2, maCns, "");
                                //    //2

                                //}
                                //else // co' chon chinhanh
                                //{
                                //    // do saledoan ND ko co STN => STN = STS
                                //    maCn = maCn == "STN" ? "STS" : maCn;
                                //    maCns = new List<string>();
                                //    maCns.Add(maCn);
                                //    //1
                                //    BaoCaoVM.TourBaoCaoTheoThangs1_ND = TourBaoCaoTheoThangViewModels_ND(tuThang1, denThang1, nam1, maCns, "");
                                //    //1

                                //    //2
                                //    BaoCaoVM.TourBaoCaoTheoThangs2_ND = TourBaoCaoTheoThangViewModels_ND(tuThang2, denThang2, nam2, maCns, "");
                                //    //2

                                //}

                                break;

                            case "OB":
                                // do tourOB ko co daily -> lay theo chinhanh

                                if (!string.IsNullOrEmpty(maCn)) // co' chon chinhanh
                                {
                                    //// do saledoan ND ko co STN => STN = STS
                                    //maCn = maCn == "STN" ? "STS" : maCn;
                                    maCns = new List<string>();
                                    maCns.Add(maCn);
                                }

                                //1
                                BaoCaoVM.TourBaoCaoTheoThangs1_OB = TourBaoCaoTheoThangViewModels_OB(tuThang1, denThang1, nam1, maCns, "");
                                //1

                                //2
                                BaoCaoVM.TourBaoCaoTheoThangs2_OB = TourBaoCaoTheoThangViewModels_OB(tuThang2, denThang2, nam2, maCns, "");
                                //2
                                break;
                        }

                    }
                    else // moi load vao
                    {
                        //var role = await _baoCaoService.GetRoleById(user.RoleId);
                        //var chiNhanhQL = role.ChiNhanhQL.Split(',').ToList(); // chiNhanhQL
                        //maCns.AddRange(chiNhanhQL);

                        //BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Where(item1 => maCns.Any(item2 => item1.Macn == item2));
                    }

                }
            }
            else // admin tong
            {
                // lay het cn va khoi
                if (!string.IsNullOrEmpty(khoi)) // luc chon khoi --> co lun chinhanh
                {
                    switch (khoi)
                    {
                        case "IB":
                            if (string.IsNullOrEmpty(maCn)) // ko chon cn => lay het cn dang co'
                            {

                                //1
                                BaoCaoVM.TourBaoCaoTheoThangs1_IB = TourBaoCaoTheoThangViewModels_IB(tuThang1, denThang1, nam1, BaoCaoVM.Dmchinhanhs.ToList(), new List<string>(), "");
                                //1

                                //2
                                BaoCaoVM.TourBaoCaoTheoThangs2_IB = TourBaoCaoTheoThangViewModels_IB(tuThang2, denThang2, nam2, BaoCaoVM.Dmchinhanhs.ToList(), new List<string>(), "");
                                //2

                            }
                            else // co' chon chinhanh
                            {
                                BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == maCn);
                                //1
                                BaoCaoVM.TourBaoCaoTheoThangs1_IB = TourBaoCaoTheoThangViewModels_IB(tuThang1, denThang1, nam1,
                                    BaoCaoVM.Dmchinhanhs.ToList(), new List<string>(), "");
                                //1

                                //2
                                BaoCaoVM.TourBaoCaoTheoThangs2_IB = TourBaoCaoTheoThangViewModels_IB(tuThang2, denThang2, nam2,
                                    BaoCaoVM.Dmchinhanhs.ToList(), new List<string>(), "");
                                //2

                            }

                            break;

                        case "ND":

                            // do tournd ko co daily -> lay theo chinhanh
                            maCns = BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList();
                            if (!string.IsNullOrEmpty(maCn)) // co' chon chinhanh
                            {
                                // do saledoan ND ko co STN => STN = STS
                                maCn = maCn == "STN" ? "STS" : maCn;
                                maCns = new List<string>();
                                maCns.Add(maCn);
                            }

                            //1
                            BaoCaoVM.TourBaoCaoTheoThangs1_ND = TourBaoCaoTheoThangViewModels_ND(tuThang1, denThang1, nam1, maCns, "");
                            //1

                            //2
                            BaoCaoVM.TourBaoCaoTheoThangs2_ND = TourBaoCaoTheoThangViewModels_ND(tuThang2, denThang2, nam2, maCns, "");
                            //2

                            //// do tournd ko co daily -> lay theo chinhanh
                            //maCns = new List<string>();
                            //maCns.AddRange(BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList());
                            //if (string.IsNullOrEmpty(maCn)) // ko chon cn => lay het cn dang co'
                            //{
                            //    //1
                            //    BaoCaoVM.TourBaoCaoTheoThangs1_ND = TourBaoCaoTheoThangViewModels_ND(tuThang1, denThang1, nam1, maCns, "");
                            //    //1

                            //    //2
                            //    BaoCaoVM.TourBaoCaoTheoThangs2_ND = TourBaoCaoTheoThangViewModels_ND(tuThang2, denThang2, nam2, maCns, "");
                            //    //2
                            //}
                            //else // co' chon chinhanh
                            //{
                            //    // do saledoan ND ko co STN => STN = STS
                            //    maCn = maCn == "STN" ? "STS" : maCn;
                            //    maCns = new List<string>();
                            //    maCns.Add(maCn);

                            //    //1
                            //    BaoCaoVM.TourBaoCaoTheoThangs1_ND = TourBaoCaoTheoThangViewModels_ND(tuThang1, denThang1, nam1, maCns, "");
                            //    //1

                            //    //2
                            //    BaoCaoVM.TourBaoCaoTheoThangs2_ND = TourBaoCaoTheoThangViewModels_ND(tuThang2, denThang2, nam2, maCns, "");
                            //    //2
                            //}

                            break;

                        case "OB":

                            // do tourOB ko co daily -> lay theo chinhanh
                            maCns = BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList();
                            if (!string.IsNullOrEmpty(maCn)) // co' chon chinhanh
                            {
                                // do saledoan ND ko co STN => STN = STS
                                //maCn = maCn == "STN" ? "STS" : maCn;
                                maCns = new List<string>();
                                maCns.Add(maCn);
                            }

                            //1
                            BaoCaoVM.TourBaoCaoTheoThangs1_OB = TourBaoCaoTheoThangViewModels_OB(tuThang1, denThang1, nam1, maCns, "");
                            //1

                            //2
                            BaoCaoVM.TourBaoCaoTheoThangs2_OB = TourBaoCaoTheoThangViewModels_OB(tuThang2, denThang2, nam2, maCns, "");
                            //2
                            break;

                    }

                }

            }

            BaoCaoVM.Khoi = khoi;
            return View(BaoCaoVM);
        }

        //public IActionResult DoanhSoTheoThangExcel(string tuThang1, string denThang1, string nam1,
        //                                      string tuThang2, string denThang2, string nam2, string chiNhanh)
        //{
        //    // from session
        //    var user = HttpContext.Session.Gets<User>("loginUser").SingleOrDefault();

        //    // moi load vao
        //    var currentYear = DateTime.Now.Year;
        //    var previousYear = currentYear - 1;

        //    BaoCaoVM = new BaoCaoViewModel()
        //    {
        //        Dmchinhanhs = _unitOfWork.dmChiNhanhRepository.GetAll(),
        //        Thangs = Thangs()
        //    };

        //    if (string.IsNullOrEmpty(nam1))
        //    {
        //        nam1 = previousYear.ToString();
        //    }

        //    if (string.IsNullOrEmpty(nam2))
        //    {
        //        nam2 = currentYear.ToString();
        //    }

        //    tuThang1 ??= "1";
        //    denThang1 ??= "12";
        //    tuThang2 ??= "1";
        //    denThang2 ??= "12";

        //    ViewBag.tuThang1 = tuThang1;
        //    ViewBag.denThang1 = denThang1;
        //    ViewBag.nam1 = nam1;

        //    ViewBag.tuThang2 = tuThang2;
        //    ViewBag.denThang2 = denThang2;
        //    ViewBag.nam2 = nam2;

        //    ViewBag.chiNhanh = chiNhanh;

        //    // Error: bat dau phai nho hon ket thuc
        //    if ((int.Parse(tuThang1) > int.Parse(denThang1)) || (int.Parse(tuThang2) > int.Parse(denThang2)))
        //    {
        //        ModelState.AddModelError("", "Ngày bắt đầu phải nhỏ hơn ngày kết thúc!");
        //        ////1
        //        //BaoCaoVM.TourBaoCaoTheoThangs1 = TourBaoCaoTheoThangViewModels("1", "12", nam1, BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList());
        //        ////1

        //        ////2
        //        //BaoCaoVM.TourBaoCaoTheoThangs2 = TourBaoCaoTheoThangViewModels("1", "12", nam2, BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList());
        //        ////2
        //        return View(BaoCaoVM);
        //    }
        //    // Error: bat dau phai nho hon ket thuc

        //    if (user.Role.RoleName != "Admins")
        //    {
        //        if (user.Role.RoleName == "Users")
        //        {
        //            List<Dmchinhanh> listCN = new List<Dmchinhanh>();
        //            listCN.Add(BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == user.MaCN).FirstOrDefault());
        //            BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == user.MaCN);

        //            if (!string.IsNullOrEmpty(user.PhongBans)) // co ql phongban khac'
        //            {
        //                var phongBans = user.PhongBans.Split(',').ToList();
        //                //1
        //                BaoCaoVM.TourBaoCaoTheoThangs1 = TourBaoCaoTheoThangViewModels(tuThang1, denThang1, nam1, listCN, phongBans, "");
        //                //1

        //                //2
        //                BaoCaoVM.TourBaoCaoTheoThangs2 = TourBaoCaoTheoThangViewModels(tuThang2, denThang2, nam2, listCN, phongBans, "");
        //                //2
        //            }
        //            else
        //            {
        //                //1
        //                BaoCaoVM.TourBaoCaoTheoThangs1 = TourBaoCaoTheoThangViewModels(tuThang1, denThang1, nam1, listCN, new List<string>(), user.Username);
        //                //1

        //                //2
        //                BaoCaoVM.TourBaoCaoTheoThangs2 = TourBaoCaoTheoThangViewModels(tuThang2, denThang2, nam2, listCN, new List<string>(), user.Username);
        //                //2
        //            }


        //            //List<string> MaCNs = new List<string>();
        //            //MaCNs.Add(user.MaCN);
        //            ////1
        //            //BaoCaoVM.TourBaoCaoTheoThangs1 = TourBaoCaoTheoThangViewModels(tuThang1, denThang1, nam1, MaCNs);
        //            ////1

        //            ////2
        //            //BaoCaoVM.TourBaoCaoTheoThangs2 = TourBaoCaoTheoThangViewModels(tuThang2, denThang2, nam2, MaCNs);
        //            ////2
        //        }
        //        else // admin khu vuc
        //        {
        //            // chon chi nhanh
        //            if (!string.IsNullOrEmpty(chiNhanh))  // da chon chinhanh //if (chiNhanh != "-- Select --")// da chon chinhanh
        //            {
        //                ////List<string> MaCNs = _unitOfWork.dmChiNhanhRepository.Find(x => x.Macn == chiNhanh).Select(x => x.Macn).ToList();
        //                //List<string> MaCNs = new List<string>();
        //                //MaCNs.Add(chiNhanh);
        //                ////1
        //                //BaoCaoVM.TourBaoCaoTheoThangs1 = TourBaoCaoTheoThangViewModels(tuThang1, denThang1, nam1, MaCNs);
        //                ////1

        //                ////2
        //                //BaoCaoVM.TourBaoCaoTheoThangs2 = TourBaoCaoTheoThangViewModels(tuThang2, denThang2, nam2, MaCNs);
        //                ////2

        //                List<Dmchinhanh> listMaCN = new List<Dmchinhanh>();
        //                listMaCN.Add(BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == chiNhanh).FirstOrDefault());

        //                //dmchinhanh theo role
        //                var listMaCNTheoRole = _unitOfWork.phanKhuCNRepository.GetById(user.RoleId).ChiNhanhs.Split(',').ToList();
        //                BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Where(item1 => listMaCNTheoRole.Any(item2 => item1.Macn == item2));
        //                BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Append(new Dmchinhanh() { Macn = "-- Select --" }).OrderBy(x => x.Macn);
        //                //dmchinhanh theo role

        //                if (!string.IsNullOrEmpty(user.PhongBans)) // co ql phongban khac'
        //                {
        //                    var phongBans = user.PhongBans.Split(',').ToList();
        //                    //1
        //                    BaoCaoVM.TourBaoCaoTheoThangs1 = TourBaoCaoTheoThangViewModels(tuThang1, denThang1, nam1, listMaCN, phongBans, "");
        //                    //1

        //                    //2
        //                    BaoCaoVM.TourBaoCaoTheoThangs2 = TourBaoCaoTheoThangViewModels(tuThang2, denThang2, nam2, listMaCN, phongBans, "");
        //                    //2
        //                }


        //            }
        //            else  // admin khu vuc: moi load vao
        //            {
        //                ////1
        //                //var listMaCN = _unitOfWork.phanKhuCNRepository.GetById(user.RoleId).ChiNhanhs.Split(',').ToList();

        //                //BaoCaoVM.TourBaoCaoTheoThangs1 = TourBaoCaoTheoThangViewModels(tuThang1, denThang1, nam1, listMaCN);
        //                ////1

        //                ////2
        //                //BaoCaoVM.TourBaoCaoTheoThangs2 = TourBaoCaoTheoThangViewModels(tuThang2, denThang2, nam2, listMaCN);
        //                ////2

        //                var chiNhanhQLs = _unitOfWork.phanKhuCNRepository.GetById(user.RoleId).ChiNhanhs.Split(',').ToList();
        //                var listMaCN = new List<Dmchinhanh>();
        //                listMaCN.AddRange(BaoCaoVM.Dmchinhanhs.Where(x => chiNhanhQLs.Any(y => x.Macn == y)));
        //                // chi lay nhung cn minh QL thoi
        //                BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Where(item1 => listMaCN.Any(item2 => item1.Macn == item2.Macn));
        //                BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Append(new Dmchinhanh() { Macn = "-- Select --" }).OrderBy(x => x.Macn);

        //                if (!string.IsNullOrEmpty(user.PhongBans)) // co ql phongban khac'
        //                {
        //                    var phongBans = user.PhongBans.Split(',').ToList();
        //                    //1
        //                    BaoCaoVM.TourBaoCaoTheoThangs1 = TourBaoCaoTheoThangViewModels(tuThang1, denThang1, nam1, listMaCN, phongBans, "");
        //                    //1

        //                    //2
        //                    BaoCaoVM.TourBaoCaoTheoThangs2 = TourBaoCaoTheoThangViewModels(tuThang2, denThang2, nam2, listMaCN, phongBans, "");
        //                    //2
        //                }

        //            }
        //            // chon chi nhanh
        //        }
        //    }
        //    else // admin tong
        //    {
        //        if (!string.IsNullOrEmpty(chiNhanh)) // da chon chinhanh
        //        {
        //            ////List<string> MaCNs = _unitOfWork.dmChiNhanhRepository.Find(x => x.Macn == chiNhanh).Select(x => x.Macn).ToList();
        //            //List<string> MaCNs = new List<string>();
        //            //MaCNs.Add(chiNhanh);
        //            ////1
        //            //BaoCaoVM.TourBaoCaoTheoThangs1 = TourBaoCaoTheoThangViewModels(tuThang1, denThang1, nam1, MaCNs);
        //            ////1

        //            ////2
        //            //BaoCaoVM.TourBaoCaoTheoThangs2 = TourBaoCaoTheoThangViewModels(tuThang2, denThang2, nam2, MaCNs);
        //            ////2

        //            List<Dmchinhanh> listMaCN = _unitOfWork.dmChiNhanhRepository.Find(x => x.Macn == chiNhanh).ToList();
        //            if (!string.IsNullOrEmpty(user.PhongBans)) // co ql phongban khac'
        //            {
        //                var phongBans = user.PhongBans.Split(',').ToList();
        //                //1
        //                BaoCaoVM.TourBaoCaoTheoThangs1 = TourBaoCaoTheoThangViewModels(tuThang1, denThang1, nam1, listMaCN, phongBans, "");
        //                //1

        //                //2
        //                BaoCaoVM.TourBaoCaoTheoThangs2 = TourBaoCaoTheoThangViewModels(tuThang2, denThang2, nam2, listMaCN, phongBans, "");
        //                //2
        //            }
        //        }
        //        else  // admin tong: moi load vao
        //        {
        //            ////1
        //            //BaoCaoVM.TourBaoCaoTheoThangs1 = TourBaoCaoTheoThangViewModels(tuThang1, denThang1, nam1, BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList());
        //            ////1

        //            ////2
        //            //BaoCaoVM.TourBaoCaoTheoThangs2 = TourBaoCaoTheoThangViewModels(tuThang2, denThang2, nam2, BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList());
        //            ////2

        //            //1
        //            BaoCaoVM.TourBaoCaoTheoThangs1 = TourBaoCaoTheoThangViewModels(tuThang1, denThang1, nam1, new List<Dmchinhanh>(), new List<string>(), "");
        //            //1

        //            //2
        //            BaoCaoVM.TourBaoCaoTheoThangs2 = TourBaoCaoTheoThangViewModels(tuThang2, denThang2, nam2, new List<Dmchinhanh>(), new List<string>(), "");
        //            //2
        //        }
        //    }
        //    // moi load vao

        //    byte[] fileContents;
        //    try
        //    {
        //        var thangNam1 = tuThang1 + "/" + denThang1 + "/" + nam1;
        //        var thangNam2 = tuThang2 + "/" + denThang2 + "/" + nam2;
        //        var excelPackage = DoanhThuTheoThangExcelResult(thangNam1, thangNam2, BaoCaoVM.TourBaoCaoTheoThangs1, BaoCaoVM.TourBaoCaoTheoThangs2);
        //        fileContents = excelPackage.GetAsByteArray();
        //        return File(
        //        fileContents: fileContents,
        //        contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //        fileDownloadName: "DoanhSoTheoThang_" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ".xlsx");
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //private ExcelPackage DoanhThuTheoThangExcelResult(string thangNam1, string thangNam2,
        //                                                  IEnumerable<TourBaoCaoTheoThangViewModel> tourBaoCaoTheoThangViewModels1,
        //                                                  IEnumerable<TourBaoCaoTheoThangViewModel> tourBaoCaoTheoThangViewModels2)
        //{
        //    var string1 = thangNam1.Split('/');
        //    var string2 = thangNam2.Split('/');
        //    string fromTo = "Từ tháng " + string1[0] + " đến tháng " + string1[1] + " năm " + string1[2] + " - " + "từ tháng " + string2[0] + " đến tháng " + string2[1] + " năm " + string2[2];
        //    ExcelPackage ExcelApp = new ExcelPackage();
        //    ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
        //    // Định dạng chiều dài cho cột
        //    xlSheet.Column(1).Width = 10;// STT
        //    xlSheet.Column(2).Width = 12;// Tháng
        //    xlSheet.Column(3).Width = 25;// Số khách năm 1
        //    xlSheet.Column(4).Width = 20;// Doanh số năm 1
        //    xlSheet.Column(5).Width = 20;// Doanh thu năm 1
        //    xlSheet.Column(6).Width = 25;// Số khách năm 2
        //    xlSheet.Column(7).Width = 20;// Doanh số năm 2
        //    xlSheet.Column(8).Width = 20;// Doanh thu năm 2
        //    xlSheet.Column(9).Width = 10;// Tỉ lệ SK
        //    xlSheet.Column(10).Width = 10;// Tỉ lệ DT

        //    xlSheet.Cells[1, 1].Value = "CÔNG TY DVLH SAIGONTOURIST";
        //    xlSheet.Cells[1, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
        //    xlSheet.Cells[1, 1, 1, 10].Merge = true;

        //    xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH SỐ THEO THÁNG";
        //    xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
        //    xlSheet.Cells[2, 1, 2, 10].Merge = true;
        //    setCenterAligment(2, 1, 2, 10, xlSheet);

        //    xlSheet.Cells[3, 1].Value = fromTo;
        //    xlSheet.Cells[3, 1, 3, 10].Merge = true;
        //    xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
        //    setCenterAligment(3, 1, 3, 10, xlSheet);

        //    // Tạo header
        //    xlSheet.Cells[5, 1].Value = "STT";
        //    xlSheet.Cells[5, 2].Value = "Tháng ";
        //    xlSheet.Cells[5, 3].Value = "Số khách năm " + string1[2];
        //    xlSheet.Cells[5, 4].Value = "Doanh số năm " + string1[2];
        //    xlSheet.Cells[5, 5].Value = "Doanh thu năm " + string1[2];
        //    xlSheet.Cells[5, 6].Value = "Số khách năm " + string2[2];
        //    xlSheet.Cells[5, 7].Value = "Doanh số năm " + string2[2];
        //    xlSheet.Cells[5, 8].Value = "Doanh thu năm " + string2[2];
        //    xlSheet.Cells[5, 9].Value = "Tỷ lệ SK";
        //    xlSheet.Cells[5, 10].Value = "Tỷ lệ DT";

        //    xlSheet.Cells[5, 1, 5, 10].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));
        //    setBorder(5, 1, 5, 10, xlSheet);
        //    setCenterAligment(5, 1, 5, 10, xlSheet);
        //    // do du lieu tu table
        //    int dong = 6;

        //    //du lieu
        //    //int iRowIndex = 6;

        //    Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#D3D3D3");// ColorTranslator.FromHtml("#D3D3D3");
        //    Color colorTotalRow = ColorTranslator.FromHtml("#66ccff");
        //    Color colorThanhLy = ColorTranslator.FromHtml("#7FFF00");
        //    Color colorChuaThanhLy = ColorTranslator.FromHtml("#FFDEAD");

        //    //int idem = 1;

        //    var tour1Array = tourBaoCaoTheoThangViewModels1.OrderBy(x => int.Parse(x.Thang)).ToArray();
        //    var tour2Array = tourBaoCaoTheoThangViewModels2.OrderBy(x => int.Parse(x.Thang)).ToArray();

        //    decimal doanhThu1tong = 0, doanhThu2tong = 0;

        //    for (int i = 0; i <= 11; i++)
        //    {
        //        xlSheet.Cells[dong, 1].Value = (i + 1);
        //        TrSetCellBorder(xlSheet, dong, 1, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //        //xlSheet.Cells[dong, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //        xlSheet.Cells[dong, 2].Value = "Tháng " + (i + 1);
        //        TrSetCellBorder(xlSheet, dong, 2, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //        // xlSheet.Cells[dong, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //        xlSheet.Cells[dong, 3].Value = tour1Array[i].SoKhach;
        //        TrSetCellBorder(xlSheet, dong, 3, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //        // xlSheet.Cells[dong, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //        xlSheet.Cells[dong, 4].Value = tour1Array[i].DoanhSo;
        //        TrSetCellBorder(xlSheet, dong, 4, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //        //xlSheet.Cells[dong, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //        var doanhThu1 = tour1Array[i].DoanhSo * 10 / 11;
        //        doanhThu1tong += doanhThu1;

        //        xlSheet.Cells[dong, 5].Value = doanhThu1;
        //        TrSetCellBorder(xlSheet, dong, 5, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //        //xlSheet.Cells[dong, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //        xlSheet.Cells[dong, 6].Value = tour2Array[i].SoKhach;
        //        TrSetCellBorder(xlSheet, dong, 6, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //        // xlSheet.Cells[dong, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //        xlSheet.Cells[dong, 7].Value = tour2Array[i].DoanhSo;
        //        TrSetCellBorder(xlSheet, dong, 7, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //        //xlSheet.Cells[dong, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //        var doanhThu2 = tour2Array[i].DoanhSo * 10 / 11;
        //        doanhThu2tong += doanhThu2;

        //        xlSheet.Cells[dong, 8].Value = doanhThu2;
        //        TrSetCellBorder(xlSheet, dong, 8, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //        // xlSheet.Cells[dong, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //        int tyLeSK;
        //        if (tour1Array[i].SoKhach > 0)
        //        {
        //            tyLeSK = tour2Array[i].SoKhach / tour1Array[i].SoKhach * 100;
        //        }
        //        else
        //        {
        //            tyLeSK = 0;
        //        }
        //        xlSheet.Cells[dong, 9].Value = tyLeSK.ToString("N0") + "%";
        //        TrSetCellBorder(xlSheet, dong, 9, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //        //xlSheet.Cells[dong, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //        decimal tyLeDoanhThu = 0;
        //        if (doanhThu1 > 0)
        //        {
        //            tyLeDoanhThu = doanhThu2 / doanhThu1 * 100;
        //        }

        //        xlSheet.Cells[dong, 10].Value = tyLeDoanhThu.ToString("N0") + "%";
        //        TrSetCellBorder(xlSheet, dong, 10, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //        // xlSheet.Cells[dong, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //        dong++;
        //        //idem++;
        //    }
        //    // 1
        //    xlSheet.Cells[dong, 2].Value = "Tổng Cộng: ";
        //    TrSetCellBorder(xlSheet, dong, 2, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //    var tongSK1 = tourBaoCaoTheoThangViewModels1.Sum(x => x.SoKhach);
        //    xlSheet.Cells[dong, 3].Value = tongSK1;
        //    TrSetCellBorder(xlSheet, dong, 3, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //    xlSheet.Cells[dong, 4].Value = tourBaoCaoTheoThangViewModels1.Sum(x => x.DoanhSo);
        //    TrSetCellBorder(xlSheet, dong, 4, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //    xlSheet.Cells[dong, 5].Value = doanhThu1tong;
        //    TrSetCellBorder(xlSheet, dong, 5, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

        //    // 2
        //    var tongSK2 = tourBaoCaoTheoThangViewModels2.Sum(x => x.SoKhach);
        //    xlSheet.Cells[dong, 6].Value = tourBaoCaoTheoThangViewModels2.Sum(x => x.SoKhach);
        //    TrSetCellBorder(xlSheet, dong, 6, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //    xlSheet.Cells[dong, 7].Value = tourBaoCaoTheoThangViewModels2.Sum(x => x.DoanhSo);
        //    TrSetCellBorder(xlSheet, dong, 7, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //    xlSheet.Cells[dong, 8].Value = doanhThu2tong;
        //    TrSetCellBorder(xlSheet, dong, 8, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

        //    // tyle tyLeSKCuaTong = tongSK2 / tongSK1 * 100; tyLeDTCuaTong = doanhThu2Tong / doanhThu1Tong * 100
        //    decimal tongSK2ChiaTongSK1 = 0;
        //    if (tongSK1 > 0)
        //    {
        //        tongSK2ChiaTongSK1 = Convert.ToDecimal(tongSK2) / Convert.ToDecimal(tongSK1);
        //    }

        //    xlSheet.Cells[dong, 9].Value = (tongSK2ChiaTongSK1 * 100).ToString("N0") + "%";
        //    TrSetCellBorder(xlSheet, dong, 9, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //    decimal doanhThu2tongChiadoanhThu1tong = 0;
        //    if (doanhThu1tong > 0)
        //    {
        //        doanhThu2tongChiadoanhThu1tong = doanhThu2tong / doanhThu1tong;
        //    }
        //    xlSheet.Cells[dong, 10].Value = (doanhThu2tongChiadoanhThu1tong * 100).ToString("N0") + "%";
        //    TrSetCellBorder(xlSheet, dong, 10, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

        //    setFontBold(dong, 2, dong, 10, 12, xlSheet);
        //    NumberFormat(6, 3, 6 + dong, 8, xlSheet);

        //    //dong++;
        //    //// Merger cot 4,5 ghi tổng tiền
        //    //setRightAligment(dong, 3, dong, 3, xlSheet);
        //    //xlSheet.Cells[dong, 1, dong, 2].Merge = true;
        //    //xlSheet.Cells[dong, 1].Value = "Tổng tiền: ";

        //    // Sum tổng tiền
        //    // xlSheet.Cells[dong, 5].Value = "TC:";
        //    //DateTimeFormat(6, 4, 6 + d.Count(), 4, xlSheet);
        //    // DateTimeFormat(6, 4, 9, 4, xlSheet);
        //    // setCenterAligment(6, 4, 9, 4, xlSheet);
        //    // xlSheet.Cells[dong, 6].Formula = "SUM(F6:F" + (6 + d.Count() - 1) + ")";

        //    //setBorder(5, 1, 5 + d.Count() + 2, 10, xlSheet);

        //    //setFontBold(5, 1, 5, 8, 11, xlSheet);
        //    //setFontSize(6, 1, 6 + d.Count() + 2, 8, 11, xlSheet);
        //    // canh giua cot stt
        //    //setCenterAligment(6, 1, 6 + dong + 2, 1, xlSheet);
        //    // canh giua code chinhanh
        //    //setCenterAligment(6, 3, 6 + dong + 2, 3, xlSheet);
        //    // NumberFormat(6, 6, 6 + d.Count(), 6, xlSheet);
        //    // định dạng số cot, đơn giá, thành tiền tong cong
        //    // NumberFormat(6, 8, dong, 9, xlSheet);

        //    // setBorder(dong, 5, dong, 6, xlSheet);
        //    // setFontBold(dong, 5, dong, 6, 12, xlSheet);

        //    //xlSheet.View.FreezePanes(6, 20);

        //    //end du lieu

        //    return ExcelApp;

        //    //byte[] fileContents;
        //    //try
        //    //{
        //    //    fileContents = ExcelApp.GetAsByteArray();
        //    //    return File(
        //    //    fileContents: fileContents,
        //    //    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //    //    fileDownloadName: "DoanhSoTheoSale_" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ".xlsx");
        //    //}
        //    //catch (Exception)
        //    //{
        //    //    throw;
        //    //}
        //}

        private IEnumerable<TourBaoCaoTheoThangViewModel> TourBaoCaoTheoThangViewModels_IB(string tuThang1, string denThang1, string nam1,
            List<Dmchinhanh> chiNhanhs, List<string> phongBans, string username)
        {
            var searchFromDate = "01/" + tuThang1 + "/" + nam1;
            string searchToDate = "01/" + denThang1 + "/" + nam1;

            // thang co 31 ngay
            if (denThang1 == "1" || denThang1 == "3" || denThang1 == "5" || denThang1 == "7" || denThang1 == "8" || denThang1 == "10" || denThang1 == "12")
            {
                searchToDate = "31/" + denThang1 + "/" + nam1;
            }
            // thang co 30 ngay
            if (denThang1 == "4" || denThang1 == "6" || denThang1 == "9" || denThang1 == "11")
            {
                searchToDate = "30/" + denThang1 + "/" + nam1;
            }
            // kiem tra nam nhuan
            if ((denThang1 == "2") && (int.Parse(nam1) % 400 == 0)) // chia het 400 => nam nhuan
            {
                searchToDate = "29/" + denThang1 + "/" + nam1;
            }
            if ((denThang1 == "2") && (int.Parse(nam1) % 400 != 0)) // ko phai nam nhuan
            {
                searchToDate = "28/" + denThang1 + "/" + nam1;
            }
            //BaoCaoVM.TourBaoCaoDtos
            IEnumerable<TourIBDTO> tourIBDTOs = _baoCaoService.DoanhSoTheoThang_IB(searchFromDate, searchToDate, chiNhanhs, phongBans, username);

            var tourBaoCaoDtos = tourIBDTOs.GroupBy(x => x.NgayTao.Month);
            IEnumerable<TourBaoCaoTheoThangViewModel> tourBaoCaoTheoThangViewModels = tourBaoCaoDtos.Select(x => new TourBaoCaoTheoThangViewModel()
            {
                Thang = x.First().NgayTao.Month.ToString(),
                SoKhach = x.Sum(x => x.SoKhachTT == 0 ? x.SoKhachDK : x.SoKhachTT),
                DoanhSo = x.Sum(x => x.DoanhThuTT == 0 ? x.DoanhThuDK : x.DoanhThuTT)
            });

            var TourBaoCaoTheoThangs1Array = tourBaoCaoTheoThangViewModels.ToArray();
            var count = 12 - TourBaoCaoTheoThangs1Array.Length;

            if (count != 0) // chua du 12 thang
            {
                // add list du 12 thang
                List<TourBaoCaoTheoThangViewModel> list = new List<TourBaoCaoTheoThangViewModel>();
                for (int i = 1; i <= 12; i++)
                {
                    list.Add(new TourBaoCaoTheoThangViewModel() { Thang = i.ToString(), SoKhach = 0, DoanhSo = 0 });
                }

                if (tourBaoCaoTheoThangViewModels.Count() != 0)
                {
                    // chi lay nhung item ma thang khong co' trong BaoCaoVM.TourBaoCaoTheoThangs1
                    foreach (var item in tourBaoCaoTheoThangViewModels)
                    {
                        var itemInList = list.Where(x => int.Parse(x.Thang) == int.Parse(item.Thang));
                        list.Remove(itemInList.FirstOrDefault());
                    }

                    tourBaoCaoTheoThangViewModels = tourBaoCaoTheoThangViewModels.Concat(list);
                }
                else
                {
                    tourBaoCaoTheoThangViewModels = list;
                }

                // add cho du 12 con vao BaoCaoVM.TourBaoCaoTheoThangs1
            }
            return tourBaoCaoTheoThangViewModels;
        }

        private IEnumerable<TourBaoCaoTheoThangViewModel> TourBaoCaoTheoThangViewModels_ND(string tuThang1, string denThang1, string nam1,
            List<string> chiNhanhs, string username)
        {
            var searchFromDate = "01/" + tuThang1 + "/" + nam1;
            string searchToDate = "01/" + denThang1 + "/" + nam1;

            // thang co 31 ngay
            if (denThang1 == "1" || denThang1 == "3" || denThang1 == "5" || denThang1 == "7" || denThang1 == "8" || denThang1 == "10" || denThang1 == "12")
            {
                searchToDate = "31/" + denThang1 + "/" + nam1;
            }
            // thang co 30 ngay
            if (denThang1 == "4" || denThang1 == "6" || denThang1 == "9" || denThang1 == "11")
            {
                searchToDate = "30/" + denThang1 + "/" + nam1;
            }
            // kiem tra nam nhuan
            if ((denThang1 == "2") && (int.Parse(nam1) % 400 == 0)) // chia het 400 => nam nhuan
            {
                searchToDate = "29/" + denThang1 + "/" + nam1;
            }
            if ((denThang1 == "2") && (int.Parse(nam1) % 400 != 0)) // ko phai nam nhuan
            {
                searchToDate = "28/" + denThang1 + "/" + nam1;
            }
            //BaoCaoVM.TourBaoCaoDtos
            IEnumerable<TourNDDTO> TourNDDTOs = _baoCaoService.DoanhSoTheoThang_ND(searchFromDate, searchToDate, chiNhanhs, username);

            var tourBaoCaoDtos = TourNDDTOs.GroupBy(x => x.Ngaytao.Value.Month);

            IEnumerable<TourBaoCaoTheoThangViewModel> tourBaoCaoTheoThangViewModels = tourBaoCaoDtos.Select(x => new TourBaoCaoTheoThangViewModel()
            {
                Thang = x.First().Ngaytao.Value.Month.ToString(),
                SoKhach = x.Sum(x => x.Sokhachtt == 0 ? x.Sokhachdk : x.Sokhachtt),
                DoanhSo = x.Sum(x => x.Doanhthutt == 0 ? x.Doanhthudk : x.Doanhthutt)
            });

            var TourBaoCaoTheoThangs1Array = tourBaoCaoTheoThangViewModels.ToArray();
            var count = 12 - TourBaoCaoTheoThangs1Array.Length;

            if (count != 0) // chua du 12 thang
            {
                // add list du 12 thang
                List<TourBaoCaoTheoThangViewModel> list = new List<TourBaoCaoTheoThangViewModel>();
                for (int i = 1; i <= 12; i++)
                {
                    list.Add(new TourBaoCaoTheoThangViewModel() { Thang = i.ToString(), SoKhach = 0, DoanhSo = 0 });
                }

                if (tourBaoCaoTheoThangViewModels.Count() != 0)
                {
                    // chi lay nhung item ma thang khong co' trong BaoCaoVM.TourBaoCaoTheoThangs1
                    foreach (var item in tourBaoCaoTheoThangViewModels)
                    {
                        var itemInList = list.Where(x => int.Parse(x.Thang) == int.Parse(item.Thang));
                        list.Remove(itemInList.FirstOrDefault());
                    }

                    tourBaoCaoTheoThangViewModels = tourBaoCaoTheoThangViewModels.Concat(list);
                }
                else
                {
                    tourBaoCaoTheoThangViewModels = list;
                }

                // add cho du 12 con vao BaoCaoVM.TourBaoCaoTheoThangs1
            }
            return tourBaoCaoTheoThangViewModels;
        }


        private IEnumerable<TourBaoCaoTheoThangViewModel> TourBaoCaoTheoThangViewModels_OB(string tuThang1, string denThang1, string nam1,
            List<string> chiNhanhs, string username)
        {
            var searchFromDate = "01/" + tuThang1 + "/" + nam1;
            string searchToDate = "01/" + denThang1 + "/" + nam1;

            // thang co 31 ngay
            if (denThang1 == "1" || denThang1 == "3" || denThang1 == "5" || denThang1 == "7" || denThang1 == "8" || denThang1 == "10" || denThang1 == "12")
            {
                searchToDate = "31/" + denThang1 + "/" + nam1;
            }
            // thang co 30 ngay
            if (denThang1 == "4" || denThang1 == "6" || denThang1 == "9" || denThang1 == "11")
            {
                searchToDate = "30/" + denThang1 + "/" + nam1;
            }
            // kiem tra nam nhuan
            if ((denThang1 == "2") && (int.Parse(nam1) % 400 == 0)) // chia het 400 => nam nhuan
            {
                searchToDate = "29/" + denThang1 + "/" + nam1;
            }
            if ((denThang1 == "2") && (int.Parse(nam1) % 400 != 0)) // ko phai nam nhuan
            {
                searchToDate = "28/" + denThang1 + "/" + nam1;
            }
            //BaoCaoVM.TourBaoCaoDtos
            IEnumerable<TourOBDTO> TourOBDTOs = _baoCaoService.DoanhSoTheoThang_OB(searchFromDate, searchToDate, chiNhanhs, username);

            var tourBaoCaoDtos = TourOBDTOs.GroupBy(x => x.Ngaytao.Value.Month);

            IEnumerable<TourBaoCaoTheoThangViewModel> tourBaoCaoTheoThangViewModels = tourBaoCaoDtos.Select(x => new TourBaoCaoTheoThangViewModel()
            {
                Thang = x.First().Ngaytao.Value.Month.ToString(),
                SoKhach = x.Sum(x => x.Sokhachtt == 0 ? x.Sokhachdk : x.Sokhachtt),
                DoanhSo = x.Sum(x => x.Doanhthutt == 0 ? x.Doanhthudk : x.Doanhthutt)
            });

            var TourBaoCaoTheoThangs1Array = tourBaoCaoTheoThangViewModels.ToArray();
            var count = 12 - TourBaoCaoTheoThangs1Array.Length;

            if (count != 0) // chua du 12 thang
            {
                // add list du 12 thang
                List<TourBaoCaoTheoThangViewModel> list = new List<TourBaoCaoTheoThangViewModel>();
                for (int i = 1; i <= 12; i++)
                {
                    list.Add(new TourBaoCaoTheoThangViewModel() { Thang = i.ToString(), SoKhach = 0, DoanhSo = 0 });
                }

                if (tourBaoCaoTheoThangViewModels.Count() != 0)
                {
                    // chi lay nhung item ma thang khong co' trong BaoCaoVM.TourBaoCaoTheoThangs1
                    foreach (var item in tourBaoCaoTheoThangViewModels)
                    {
                        var itemInList = list.Where(x => int.Parse(x.Thang) == int.Parse(item.Thang));
                        list.Remove(itemInList.FirstOrDefault());
                    }

                    tourBaoCaoTheoThangViewModels = tourBaoCaoTheoThangViewModels.Concat(list);
                }
                else
                {
                    tourBaoCaoTheoThangViewModels = list;
                }

                // add cho du 12 con vao BaoCaoVM.TourBaoCaoTheoThangs1
            }
            return tourBaoCaoTheoThangViewModels;
        }


        private IEnumerable<ListViewModel> Thangs()
        {
            return new List<ListViewModel>
            {
                new ListViewModel(){Name = "1" },
                new ListViewModel(){Name = "2" },
                new ListViewModel(){Name = "3" },
                new ListViewModel(){Name = "4" },
                new ListViewModel(){Name = "5" },
                new ListViewModel(){Name = "6" },
                new ListViewModel(){Name = "7" },
                new ListViewModel(){Name = "8" },
                new ListViewModel(){Name = "9" },
                new ListViewModel(){Name = "10" },
                new ListViewModel(){Name = "11" },
                new ListViewModel(){Name = "12" },
            };
        }

        #endregion Doanh so theo thang

        #region Doanh so theo ngay di

        public async Task<IActionResult> DoanhSoTheoNgayDi(string searchFromDate = null, string searchToDate = null,
            string loaiTour = null, string maCn = null, string khoi = null)
        {

            // from session
            var user = HttpContext.Session.Get<Users>("loginUser");

            //// moi load vao

            //BaoCaoVM.Tourkinds = _baoCaoService.GetTourinds().Append(new Tourkind() { TourkindInf = "" }).OrderBy(x => x.TourkindInf);// BaoCaoVM.Tourkinds.Append(new Tourkind() { TourkindInf = "" }).OrderBy(x => x.TourkindInf);
            BaoCaoVM.Loaitours = _baoCaoService.GetLoaiTours().Append(new Data.Models_KDND.Loaitour { Tenloaitour = "" }).OrderBy(x => x.Tenloaitour);

            if (string.IsNullOrEmpty(searchFromDate) && string.IsNullOrEmpty(searchToDate))
            {
                var currentTime = DateTime.Now;
                string TuNgayDenNgayString = LoadTuNgayDenNgay(currentTime.Month.ToString(), currentTime.Month.ToString(), currentTime.Year.ToString());
                searchFromDate = DateTime.Now.ToShortDateString();// "01/" + currentTime.Month + "/" + currentTime.Year;// TuNgayDenNgayString.Split('-')[0];
                searchToDate = "31/12/" + DateTime.Now.ToString("yyyy");// TuNgayDenNgayString.Split('-')[1];
            }
            else // da chon ngay thang - // check date correct
            {
                try
                {
                    var fromDate = Convert.ToDateTime(searchFromDate);
                    var toDate = Convert.ToDateTime(searchToDate);
                    if (fromDate > toDate)
                    {
                        ModelState.AddModelError("", "Lỗi ngày tháng.");
                        ViewBag.macn = maCn;
                        ViewBag.loaiTour = loaiTour;
                        ViewBag.searchFromDate = searchFromDate;
                        ViewBag.searchToDate = searchToDate;
                        return View(BaoCaoVM);
                    }
                }
                catch (Exception)
                {
                    BaoCaoVM = new BaoCaoViewModel()
                    {
                        Dmchinhanhs = _unitOfWork.dmChiNhanhRepository.GetAll(),
                        Tourkinds = _unitOfWork.tourKindRepository.GetAll(),
                        TourTheoNgay_IB = new Models.TourTheoNgay.TourTheoNgay_IB(),
                        TourTheoNgay_ND = new Models.TourTheoNgay.TourTheoNgay_ND(),
                        TourTheoNgay_OB = new Models.TourTheoNgay.TourTheoNgay_OB()
                    };

                    ViewBag.macn = maCn;
                    ViewBag.loaiTour = loaiTour;
                    ViewBag.searchFromDate = searchFromDate;
                    ViewBag.searchToDate = searchToDate;
                    ViewBag.khoi = khoi ?? user.Khoi;

                    ModelState.AddModelError("", "Lỗi định dạng ngày tháng.");
                    return View(BaoCaoVM);
                }
            }

            ViewBag.macn = maCn;
            ViewBag.loaiTour = loaiTour;
            ViewBag.searchFromDate = searchFromDate;// DateTime.Parse(searchFromDate).ToShortDateString();
            ViewBag.searchToDate = searchToDate;// DateTime.Parse(searchToDate).ToShortDateString();
            ViewBag.khoi = khoi ?? user.Khoi;

            BaoCaoVM.Companies = _baoCaoService.GetCompanies(); // IB
            BaoCaoVM.Dmchinhanhs = _baoCaoService.GetAllChiNhanh().Where(x => !string.IsNullOrEmpty(x.Macn));
            BaoCaoVM.Khois_KD = KhoiViewModels_KD();
            List<string> maCns = new List<string>();

            if (user.RoleId != 8) // 8: Admins
            {
                if (user.RoleId == 9) // 9: Users
                {
                    switch (khoi)
                    {
                        case "IB":
                            List<Dmchinhanh> dmchinhanhs1 = BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == user.Chinhanh).ToList();
                            if (!string.IsNullOrEmpty(user.PhongBanQL)) // co ql phongban khac' --> IB
                            {
                                var phongBanQL = user.PhongBanQL.Split(',').ToList();
                                BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoNgay_IB(searchFromDate, searchToDate, loaiTour, dmchinhanhs1, phongBanQL, "").ToList();
                            }
                            else // ko ql phongban khac' --> IB
                            {
                                BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoNgay_IB(searchFromDate, searchToDate, loaiTour, dmchinhanhs1, new List<string>(), user.Username).ToList();
                            }
                            DoanhSoTheoNgay_IB();
                            break;
                        case "ND":
                            maCn = user.Chinhanh == "STN" ? "STS" : user.Chinhanh; // do sale doan nd ko co STN
                            maCns = BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == maCn).Select(x => x.Macn).ToList();
                            BaoCaoVM.TourNDDTOs = _baoCaoService.DoanhSoTheoNgay_ND(searchFromDate, searchToDate, loaiTour, maCns, user.Username).ToList();

                            DoanhSoTheoNgay_ND();
                            break;
                        case "OB":
                            maCns = BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == user.Chinhanh).Select(x => x.Macn).ToList();
                            BaoCaoVM.TourOBDTOs = _baoCaoService.DoanhSoTheoNgay_OB(searchFromDate, searchToDate, loaiTour, maCns, user.Username).ToList();

                            DoanhSoTheoNgay_OB();
                            break;
                    }

                }
                else // admin khuvuc
                {
                    // phanKhuCNs = co cn QL
                    var role1 = await _baoCaoService.GetRoleById(user.RoleId);
                    var listMaCN = role1.ChiNhanhQL.Split(',').ToList();
                    maCns.AddRange(listMaCN);

                    // hien thi tren view
                    BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Where(x => maCns.Any(y => x.Macn == y));

                    if (!string.IsNullOrEmpty(khoi)) // luc chon khoi
                    {
                        switch (khoi)
                        {
                            case "IB":
                                if (!string.IsNullOrEmpty(maCn)) // co' chon chinhanh | else lấy hết
                                {
                                    BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == maCn);
                                }
                                if (!string.IsNullOrEmpty(user.PhongBanQL)) // co ql phongban khac'
                                {
                                    var phongBanQL = user.PhongBanQL.Split(',').ToList();
                                    BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoNgay_IB(searchFromDate, searchToDate, loaiTour, BaoCaoVM.Dmchinhanhs.ToList(), phongBanQL, "");

                                }
                                else
                                {
                                    BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoNgay_IB(searchFromDate, searchToDate, loaiTour, BaoCaoVM.Dmchinhanhs.ToList(), new List<string>(), "");
                                }
                                DoanhSoTheoNgay_IB();
                                break;
                            case "ND":
                                // do tournd ko co daily -> lay theo chinhanh
                                if (!string.IsNullOrEmpty(maCn)) // ko chon cn => lay het cn dang co'
                                {
                                    maCn = maCn == "STN" ? "STS" : maCn; // do sale doan nd ko co STN
                                    maCns = new List<string>() { maCn };
                                }

                                BaoCaoVM.TourNDDTOs = _baoCaoService.DoanhSoTheoNgay_ND(searchFromDate, searchToDate, loaiTour, maCns, "").ToList();
                                DoanhSoTheoNgay_ND();
                                break;
                            case "OB":
                                // do tourob ko co daily -> lay theo chinhanh
                                if (!string.IsNullOrEmpty(maCn)) // ko chon cn => lay het cn dang co'
                                {
                                    maCns = new List<string>() { maCn };
                                }

                                BaoCaoVM.TourOBDTOs = _baoCaoService.DoanhSoTheoNgay_OB(searchFromDate, searchToDate, loaiTour, maCns, "").ToList();
                                DoanhSoTheoNgay_OB();
                                break;
                        }
                    }








                }
            }
            else // admin tong
            {
                maCns = BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList();
                switch (khoi)
                {

                    case "IB":
                        BaoCaoVM.Dmchinhanhs = string.IsNullOrEmpty(maCn) ? BaoCaoVM.Dmchinhanhs.ToList() :
                            BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == maCn);

                        BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoNgay_IB(searchFromDate, searchToDate, loaiTour, BaoCaoVM.Dmchinhanhs.ToList(), new List<string>(), "");
                        DoanhSoTheoNgay_IB();
                        break;
                    case "ND":
                        // do tournd ko co daily -> lay theo chinhanh
                        if (!string.IsNullOrEmpty(maCn)) // ko chon cn => lay het cn dang co'
                        {
                            maCn = maCn == "STN" ? "STS" : maCn;
                            maCns = new List<string>() { maCn };
                        }

                        BaoCaoVM.TourNDDTOs = _baoCaoService.DoanhSoTheoNgay_ND(searchFromDate, searchToDate, loaiTour, maCns, "").ToList();
                        DoanhSoTheoNgay_ND();
                        break;
                    case "OB":
                        // do tourob ko co daily -> lay theo chinhanh
                        if (!string.IsNullOrEmpty(maCn)) // ko chon cn => lay het cn dang co'
                        {
                            maCns = new List<string>() { maCn };
                        }

                        BaoCaoVM.TourOBDTOs = _baoCaoService.DoanhSoTheoNgay_OB(searchFromDate, searchToDate, loaiTour, maCns, "").ToList();
                        DoanhSoTheoNgay_OB();
                        break;
                }

            }
            BaoCaoVM.Khoi = khoi;
            return View(BaoCaoVM);
        }
        #endregion
        #endregion

        #region Doanh so theo thi truong

        public async Task<IActionResult> DoanhSoTheoThiTruong(string searchFromDate = null, string searchToDate = null,
            string thiTruong = null, string maCn = null, string khoi = null)
        {

            // from session
            var user = HttpContext.Session.Get<Users>("loginUser");

            string fromDate, toDate;
            if (string.IsNullOrEmpty(searchFromDate) && string.IsNullOrEmpty(searchToDate))
            {
                //var currentTime = DateTime.Now;
                //string TuNgayDenNgayString = LoadTuNgayDenNgay(currentTime.Month.ToString(), currentTime.Month.ToString(), currentTime.Year.ToString());
                //searchFromDate = TuNgayDenNgayString.Split('-')[0];
                //searchToDate = TuNgayDenNgayString.Split('-')[1];

                fromDate = "01/01/" + DateTime.Now.ToString("yyyy");
                toDate = "31/12/" + DateTime.Now.ToString("yyyy");

                searchFromDate = fromDate;
                searchToDate = toDate;
            }
            else // da chon ngay thang - // check date correct
            {
                try
                {
                    var dateTime = Convert.ToDateTime(searchFromDate);
                    var dateTime1 = Convert.ToDateTime(searchToDate);
                    if (dateTime > dateTime1)
                    {
                        ModelState.AddModelError("", "Lỗi ngày tháng.");
                        ViewBag.macn = maCn;
                        ViewBag.searchFromDate = searchFromDate;
                        ViewBag.searchToDate = searchToDate;
                        return View(BaoCaoVM);
                    }
                }
                catch (Exception)
                {
                    BaoCaoVM = new BaoCaoViewModel()
                    {
                        Dmchinhanhs = _unitOfWork.dmChiNhanhRepository.GetAll(),
                        Tourkinds = _unitOfWork.tourKindRepository.GetAll(),
                        TourTheoNgay_IB = new TourTheoNgay_IB(),
                        TourTheoNgay_ND = new TourTheoNgay_ND(),
                        TourTheoNgay_OB = new TourTheoNgay_OB()
                    };

                    ViewBag.macn = maCn;
                    ViewBag.thiTruong = thiTruong;
                    ViewBag.searchFromDate = searchFromDate;
                    ViewBag.searchToDate = searchToDate;
                    ViewBag.khoi = khoi ?? user.Khoi;

                    ModelState.AddModelError("", "Lỗi định dạng ngày tháng.");
                    return View(BaoCaoVM);
                }
            }

            ViewBag.macn = maCn;
            ViewBag.thiTruong = thiTruong;
            ViewBag.searchFromDate = searchFromDate;
            ViewBag.searchToDate = searchToDate;
            ViewBag.khoi = khoi ?? user.Khoi;

            BaoCaoVM.Phongbans = _unitOfWork.phongBanRepository.GetAll().Where(x => !string.IsNullOrEmpty(x.Macode)); // IB
            BaoCaoVM.Companies = _baoCaoService.GetCompanies(); // IB
            BaoCaoVM.Dmchinhanhs = _baoCaoService.GetAllChiNhanh();//.Where(x => !string.IsNullOrEmpty(x.Macn));
            BaoCaoVM.Khois_KD = KhoiViewModels_KD().Where(x => x.Name == "IB");
            List<string> thiTruongs = new List<string>();
            List<string> maCns = new List<string>();

            if (user.RoleId != 8) // 8: Admins
            {
                if (user.RoleId == 9) // 9: Users
                {
                    switch (khoi)
                    {
                        case "IB":
                            List<Dmchinhanh> dmchinhanhs1 = BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == user.Chinhanh).ToList();
                            if (!string.IsNullOrEmpty(user.PhongBanQL)) // co ql phongban khac' --> IB
                            {
                                var phongBanQL = user.PhongBanQL.Split(',').ToList();
                                BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoThiTruong_IB(searchFromDate, searchToDate, dmchinhanhs1, phongBanQL, "").ToList();
                            }
                            else // ko ql phongban khac' --> IB
                            {
                                BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoThiTruong_IB(searchFromDate, searchToDate, dmchinhanhs1, new List<string>(), user.Username).ToList();
                            }
                            DoanhSoTheoNgay_IB();
                            break;
                        default:
                            List<Dmchinhanh> dmchinhanhs = BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == user.Chinhanh).ToList();
                            if (!string.IsNullOrEmpty(user.PhongBanQL)) // co ql phongban khac' --> IB
                            {
                                var phongBanQL = user.PhongBanQL.Split(',').ToList();
                                BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoThiTruong_IB(searchFromDate, searchToDate, dmchinhanhs, phongBanQL, "").ToList();
                            }
                            else // ko ql phongban khac' --> IB
                            {
                                BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoThiTruong_IB(searchFromDate, searchToDate, dmchinhanhs, new List<string>(), user.Username).ToList();
                            }
                            DoanhSoTheoNgay_IB();
                            break;
                    }

                }
                else // admin khuvuc
                {
                    // phanKhuCNs = co cn QL
                    var role1 = await _baoCaoService.GetRoleById(user.RoleId);
                    var listMaCN = role1.ChiNhanhQL.Split(',').ToList();
                    maCns.AddRange(listMaCN);

                    // hien thi tren view
                    BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Where(x => maCns.Any(y => x.Macn == y));
                    if (!string.IsNullOrEmpty(khoi)) // luc chon khoi
                    {
                        switch (khoi)
                        {
                            case "IB":
                                if (!string.IsNullOrEmpty(maCn)) // co' chon chinhanh | else lấy hết
                                {
                                    BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == maCn);
                                }
                                if (!string.IsNullOrEmpty(user.PhongBanQL)) // co ql phongban khac'
                                {
                                    var phongBanQL = user.PhongBanQL.Split(',').ToList();
                                    BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoThiTruong_IB(searchFromDate, searchToDate, BaoCaoVM.Dmchinhanhs.ToList(), phongBanQL, "");

                                }
                                else
                                {
                                    BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoThiTruong_IB(searchFromDate, searchToDate, BaoCaoVM.Dmchinhanhs.ToList(), new List<string>(), "");
                                }
                                DoanhSoTheoNgay_IB();
                                break;
                            default:
                                if (!string.IsNullOrEmpty(maCn)) // co' chon chinhanh | else lấy hết
                                {
                                    BaoCaoVM.Dmchinhanhs = BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == maCn);
                                }
                                if (!string.IsNullOrEmpty(user.PhongBanQL)) // co ql phongban khac'
                                {
                                    var phongBanQL = user.PhongBanQL.Split(',').ToList();
                                    BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoThiTruong_IB(searchFromDate, searchToDate, BaoCaoVM.Dmchinhanhs.ToList(), phongBanQL, "");

                                }
                                else
                                {
                                    BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoThiTruong_IB(searchFromDate, searchToDate, BaoCaoVM.Dmchinhanhs.ToList(), new List<string>(), "");
                                }
                                DoanhSoTheoNgay_IB();
                                break;
                        }
                    }

                }
            }
            else // admin tong
            {
                //maCns = BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList();
                thiTruongs = string.IsNullOrEmpty(thiTruong) ? BaoCaoVM.Phongbans.Select(x => x.Maphong).ToList() :
                    new List<string>() { thiTruong };

                switch (khoi)
                {
                    case "IB":
                        var dmchinhanhs = string.IsNullOrEmpty(maCn) ? BaoCaoVM.Dmchinhanhs.ToList() :
                            BaoCaoVM.Dmchinhanhs.Where(x => x.Macn == maCn);

                        BaoCaoVM.TourIBDTOs = _baoCaoService.DoanhSoTheoThiTruong_IB(searchFromDate, searchToDate, dmchinhanhs.ToList(), thiTruongs, "");
                        DoanhSoTheoNgay_IB();
                        break;

                }

            }

            return View(BaoCaoVM);
        }

        //[HttpPost]
        //public async Task<IActionResult> DoanhSoTheoThiTruongExcel(string searchFromDate = null, string searchToDate = null, string thiTruong = null)
        //{
        //    BaoCaoVM.TourBaoCaoDtosTheoNgay = new TourBaoCaoDtosTheoNgay();
        //    // from session
        //    var user = HttpContext.Session.Gets<User>("loginUser").SingleOrDefault();

        //    //BaoCaoVM.Dmchinhanhs = _unitOfWork.dmChiNhanhRepository.GetAll();

        //    //ViewBag.macn = macn;
        //    ViewBag.thiTruong = thiTruong;
        //    ViewBag.searchFromDate = searchFromDate;
        //    ViewBag.searchToDate = searchToDate;

        //    string fromTo = "";
        //    ExcelPackage ExcelApp = new ExcelPackage();
        //    ExcelWorksheet xlSheet = ExcelApp.Workbook.Worksheets.Add("Report");
        //    // Định dạng chiều dài cho cột
        //    xlSheet.Column(1).Width = 10;// STT
        //    xlSheet.Column(2).Width = 20;// Code đoàn
        //    xlSheet.Column(3).Width = 15;// Ngày đi
        //    xlSheet.Column(4).Width = 15;// Ngày về
        //    xlSheet.Column(5).Width = 40;// Tuyến tham quan
        //    xlSheet.Column(6).Width = 10;// Số khách
        //    xlSheet.Column(7).Width = 15;// Doanh số
        //    xlSheet.Column(8).Width = 15;// Sales
        //    xlSheet.Column(9).Width = 35;// Tên công ty/Khách hàng
        //    xlSheet.Column(10).Width = 25;// Loại tour
        //    xlSheet.Column(11).Width = 15;// Nguồn tour
        //    xlSheet.Column(12).Width = 15;// Ngày tạo
        //    xlSheet.Column(13).Width = 15;// Thị trường

        //    xlSheet.Cells[1, 1].Value = "CÔNG TY DVLH SAIGONTOURIST";
        //    xlSheet.Cells[1, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
        //    xlSheet.Cells[1, 1, 1, 13].Merge = true;

        //    xlSheet.Cells[2, 1].Value = "BÁO CÁO DOANH SỐ THEO THỊ TRƯỜNG";
        //    xlSheet.Cells[2, 1].Style.Font.SetFromFont(new Font("Times New Roman", 16, FontStyle.Bold));
        //    xlSheet.Cells[2, 1, 2, 13].Merge = true;
        //    setCenterAligment(2, 1, 2, 13, xlSheet);

        //    // dinh dang tu ngay den ngay
        //    if (string.IsNullOrEmpty(searchFromDate) && string.IsNullOrEmpty(searchToDate))
        //    {
        //        ViewBag.searchFromDate = searchFromDate;
        //        ViewBag.searchToDate = searchToDate;
        //        SetAlert("Từ ngày đến ngày không được để trống.", "warning");
        //        return RedirectToAction(nameof(DoanhSoTheoThiTruong));
        //    }
        //    if (searchFromDate == searchToDate)
        //    {
        //        fromTo = "Ngày: " + searchFromDate;
        //    }
        //    else
        //    {
        //        fromTo = "Từ ngày: " + searchFromDate + " đến ngày: " + searchToDate;
        //    }
        //    xlSheet.Cells[3, 1].Value = fromTo;
        //    xlSheet.Cells[3, 1, 3, 12].Merge = true;
        //    xlSheet.Cells[3, 1].Style.Font.SetFromFont(new Font("Times New Roman", 14, FontStyle.Bold));
        //    setCenterAligment(3, 1, 3, 12, xlSheet);

        //    // Tạo header
        //    xlSheet.Cells[5, 1].Value = "STT";
        //    xlSheet.Cells[5, 2].Value = "Code đoàn ";
        //    xlSheet.Cells[5, 3].Value = "Ngày đi";
        //    xlSheet.Cells[5, 4].Value = "Ngày về";
        //    xlSheet.Cells[5, 5].Value = "Tuyến tham quan";
        //    xlSheet.Cells[5, 6].Value = "Số khách";
        //    xlSheet.Cells[5, 7].Value = "Doanh số";
        //    xlSheet.Cells[5, 8].Value = "Sales";
        //    xlSheet.Cells[5, 9].Value = "Tên công ty/Khách hàng ";
        //    xlSheet.Cells[5, 10].Value = "Loại tour";
        //    xlSheet.Cells[5, 11].Value = "Nguồn tour";
        //    xlSheet.Cells[5, 12].Value = "Ngày tạo";
        //    xlSheet.Cells[5, 13].Value = "Thị trường";

        //    xlSheet.Cells[5, 1, 5, 13].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));
        //    setBorder(5, 1, 5, 13, xlSheet);
        //    setCenterAligment(5, 1, 5, 13, xlSheet);

        //    // do du lieu tu table
        //    int dong = 6;

        //    //// moi load vao
        //    List<string> thiTruongs = new List<string>();
        //    List<string> maCns = new List<string>();

        //    BaoCaoVM.Phongbans = _unitOfWork.phongBanRepository.GetAll().Where(x => !string.IsNullOrEmpty(x.Macode));

        //    if (user.Role.RoleName != "Admins")
        //    {
        //        if (user.Role.RoleName == "Users")
        //        {
        //            ////BaoCaoVM.Dmchinhanhs = new List<Dmchinhanh>() { new Dmchinhanh() { Macn = user.MaCN } };
        //            //BaoCaoVM.Phongbans = _unitOfWork.phongBanRepository.Find(x => x.Maphong == user.PhongBanId);
        //            //BaoCaoVM.TourBaoCaoDtos = _baoCaoService.DoanhSoTheoThiTruong(searchFromDate, searchToDate, BaoCaoVM.Phongbans.Select(x => x.Maphong).ToList()); //, BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList());
        //            //BaoCaoVM.TourBaoCaoDtos = BaoCaoVM.TourBaoCaoDtos.Where(x => x.NguoiTao == user.Username);

        //            if (!string.IsNullOrEmpty(user.PhongBans)) // co ql phongban khac'
        //            {
        //                var phongBans = user.PhongBans.Split(',').ToList();
        //                BaoCaoVM.TourBaoCaoDtos = _baoCaoService.DoanhSoTheoThiTruong(searchFromDate, searchToDate, phongBans);
        //                BaoCaoVM.Phongbans = BaoCaoVM.Phongbans.Where(x => phongBans.Any(y => y == x.Maphong));
        //            }
        //            else
        //            {
        //                BaoCaoVM.Phongbans = BaoCaoVM.Phongbans.Where(x => x.Maphong == user.PhongBanId);
        //                BaoCaoVM.TourBaoCaoDtos = _baoCaoService.DoanhSoTheoThiTruong(searchFromDate, searchToDate, BaoCaoVM.Phongbans.Select(x => x.Maphong).ToList()); //, BaoCaoVM.Dmchinhanhs.Select(x => x.Macn).ToList());

        //                // loc theo thi truong cua minh
        //                //BaoCaoVM.TourBaoCaoDtos = BaoCaoVM.TourBaoCaoDtos.Where(x => x.ThiTruongByNguoiTao == user.PhongBanId);

        //                // loc chi nhanh
        //                BaoCaoVM.TourBaoCaoDtos = BaoCaoVM.TourBaoCaoDtos.Where(x => x.NguoiTao == user.Username);

        //            }
        //            DoanhSoTheoNgay();
        //        }
        //        else
        //        {
        //            var phanKhuCNs = await _unitOfWork.phanKhuCNRepository.FindIncludeOneAsync(x => x.Role, y => y.RoleId == user.RoleId);

        //            foreach (var item in phanKhuCNs)
        //            {
        //                maCns.AddRange(item.ChiNhanhs.Split(',').ToList());
        //            }
        //            thiTruongs = new List<string>();
        //            foreach (var item in maCns)
        //            {
        //                var users = await _unitOfWork.userRepository.FindIncludeOneAsync(x => x.Role, y => y.MaCN == item);
        //                var phongBanIds = users.Select(x => x.PhongBanId).Distinct();
        //                thiTruongs.AddRange(phongBanIds);
        //            }

        //            BaoCaoVM.TourBaoCaoDtos = _baoCaoService.DoanhSoTheoThiTruong(searchFromDate, searchToDate, thiTruongs);
        //            if (!string.IsNullOrEmpty(thiTruong))
        //            {
        //                BaoCaoVM.TourBaoCaoDtos = BaoCaoVM.TourBaoCaoDtos.Where(x => x.ThiTruongByNguoiTao == thiTruong);
        //            }

        //            // loc chi nhanh
        //            BaoCaoVM.TourBaoCaoDtos = BaoCaoVM.TourBaoCaoDtos.Where(item1 => maCns.Any(item2 => item1.MaCNTao == item2));

        //            DoanhSoTheoNgay();
        //        }
        //    }
        //    else
        //    {
        //        thiTruongs = new List<string>();
        //        thiTruongs.AddRange(_unitOfWork.phongBanRepository.GetAll().Where(x => !string.IsNullOrEmpty(x.Macode)).Select(x => x.Maphong).Distinct());
        //        BaoCaoVM.TourBaoCaoDtos = _baoCaoService.DoanhSoTheoThiTruong(searchFromDate, searchToDate, thiTruongs);
        //        if (!string.IsNullOrEmpty(thiTruong))
        //        {
        //            BaoCaoVM.TourBaoCaoDtos = BaoCaoVM.TourBaoCaoDtos.Where(x => x.ThiTruongByNguoiTao == thiTruong);
        //        }
        //        DoanhSoTheoNgay();
        //    }

        //    //return View(BaoCaoVM);

        //    //du lieu
        //    //int iRowIndex = 6;

        //    Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#D3D3D3");// ColorTranslator.FromHtml("#D3D3D3");
        //    Color colorTotalRow = ColorTranslator.FromHtml("#66ccff");
        //    Color colorThanhLy = ColorTranslator.FromHtml("#7FFF00");
        //    Color colorChuaThanhLy = ColorTranslator.FromHtml("#FFDEAD");

        //    int idem = 1;

        //    if (BaoCaoVM.TourBaoCaoDtosTheoNgay.TourBaoCaoDtos != null)
        //    {
        //        foreach (var item in BaoCaoVM.TourBaoCaoDtosTheoNgay.TourBaoCaoDtos)
        //        {
        //            xlSheet.Cells[dong, 1].Value = idem;
        //            TrSetCellBorder(xlSheet, dong, 1, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Justify, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //            //xlSheet.Cells[dong, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //            xlSheet.Cells[dong, 2].Value = item.Sgtcode;
        //            xlSheet.Cells[dong, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //            if (item.TrangThai == "3")
        //            {
        //                xlSheet.Cells[dong, 2].Style.Fill.BackgroundColor.SetColor(colorThanhLy);
        //            }
        //            else if (item.TrangThai == "2")
        //            {
        //                xlSheet.Cells[dong, 2].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
        //            }
        //            else if (item.TrangThai == "4")
        //            {
        //                xlSheet.Cells[dong, 2].Style.Fill.BackgroundColor.SetColor(Color.Red);
        //            }
        //            else
        //            {
        //                xlSheet.Cells[dong, 2].Style.Fill.BackgroundColor.SetColor(Color.White);
        //            }

        //            TrSetCellBorder(xlSheet, dong, 2, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //            // xlSheet.Cells[dong, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //            xlSheet.Cells[dong, 3].Value = item.NgayDen.ToShortDateString();
        //            TrSetCellBorder(xlSheet, dong, 3, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //            // xlSheet.Cells[dong, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //            xlSheet.Cells[dong, 4].Value = item.NgayDi.ToShortDateString();
        //            TrSetCellBorder(xlSheet, dong, 4, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //            //xlSheet.Cells[dong, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //            xlSheet.Cells[dong, 5].Value = item.TuyenTQ;
        //            TrSetCellBorder(xlSheet, dong, 5, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //            //xlSheet.Cells[dong, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //            xlSheet.Cells[dong, 6].Value = (item.SoKhachTT == 0) ? 0 : item.SoKhachTT;
        //            xlSheet.Cells[dong, 6].Style.Numberformat.Format = "#,##0";
        //            TrSetCellBorder(xlSheet, dong, 6, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //            // xlSheet.Cells[dong, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //            xlSheet.Cells[dong, 7].Value = (item.DoanhThuTT == 0) ? 0 : item.DoanhThuTT;
        //            xlSheet.Cells[dong, 7].Style.Numberformat.Format = "#,##0";
        //            TrSetCellBorder(xlSheet, dong, 7, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //            //xlSheet.Cells[dong, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //            xlSheet.Cells[dong, 8].Value = item.NguoiTao;
        //            TrSetCellBorder(xlSheet, dong, 8, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //            // xlSheet.Cells[dong, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //            xlSheet.Cells[dong, 9].Value = item.CompanyName;
        //            TrSetCellBorder(xlSheet, dong, 9, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //            // xlSheet.Cells[dong, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //            xlSheet.Cells[dong, 10].Value = item.TenLoaiTour;
        //            TrSetCellBorder(xlSheet, dong, 10, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //            // xlSheet.Cells[dong, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //            xlSheet.Cells[dong, 11].Value = item.NguonTour;
        //            TrSetCellBorder(xlSheet, dong, 11, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //            // xlSheet.Cells[dong, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //            xlSheet.Cells[dong, 12].Value = item.NgayTao;
        //            TrSetCellBorder(xlSheet, dong, 12, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //            // xlSheet.Cells[dong, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //            xlSheet.Cells[dong, 13].Value = item.ThiTruongByNguoiTao;
        //            TrSetCellBorder(xlSheet, dong, 13, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //            // xlSheet.Cells[dong, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //            //setBorder(5, 1, dong, 10, xlSheet);

        //            dong++;
        //            idem++;
        //        }

        //        xlSheet.Cells[dong, 5].Value = "TỔNG CỘNG:";
        //        TrSetCellBorder(xlSheet, dong, 5, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //        xlSheet.Cells[dong, 6].Value = BaoCaoVM.TourBaoCaoDtosTheoNgay.TongSK;
        //        TrSetCellBorder(xlSheet, dong, 6, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //        xlSheet.Cells[dong, 7].Value = BaoCaoVM.TourBaoCaoDtosTheoNgay.TongDS;
        //        TrSetCellBorder(xlSheet, dong, 7, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

        //        xlSheet.Cells[dong + 1, 5].Value = "Các đoàn đã thanh lý:";
        //        TrSetCellBorder(xlSheet, dong + 1, 5, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //        xlSheet.Cells[dong + 1, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //        xlSheet.Cells[dong + 1, 5].Style.Fill.BackgroundColor.SetColor(colorThanhLy);
        //        xlSheet.Cells[dong + 1, 6].Value = BaoCaoVM.TourBaoCaoDtosTheoNgay.TongSKCacDoanDaThanhLy;
        //        TrSetCellBorder(xlSheet, dong + 1, 6, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //        xlSheet.Cells[dong + 1, 7].Value = BaoCaoVM.TourBaoCaoDtosTheoNgay.TongDSCacDoanDaThanhLy;
        //        TrSetCellBorder(xlSheet, dong + 1, 7, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

        //        xlSheet.Cells[dong + 2, 5].Value = "Các đoàn chưa thanh lý:";
        //        TrSetCellBorder(xlSheet, dong + 2, 5, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //        xlSheet.Cells[dong + 2, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //        xlSheet.Cells[dong + 2, 5].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
        //        xlSheet.Cells[dong + 2, 6].Value = BaoCaoVM.TourBaoCaoDtosTheoNgay.TongSKCacDoanChuaThanhLy;
        //        TrSetCellBorder(xlSheet, dong + 2, 6, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //        xlSheet.Cells[dong + 2, 7].Value = BaoCaoVM.TourBaoCaoDtosTheoNgay.TongDSCacDoanChuaThanhLy;
        //        TrSetCellBorder(xlSheet, dong + 2, 7, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

        //        xlSheet.Cells[dong + 3, 5].Value = "Các đoàn chưa ký hợp đồng:";
        //        TrSetCellBorder(xlSheet, dong + 3, 5, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

        //        xlSheet.Cells[dong + 3, 6].Value = BaoCaoVM.TourBaoCaoDtosTheoNgay.TongSKCacDoanChuaKyHD;
        //        TrSetCellBorder(xlSheet, dong + 3, 6, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //        xlSheet.Cells[dong + 3, 7].Value = BaoCaoVM.TourBaoCaoDtosTheoNgay.TongDSCacDoanChuaKyHD;
        //        TrSetCellBorder(xlSheet, dong + 3, 7, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Right, Color.Silver, "Times New Roman", 12, FontStyle.Regular);

        //        setBorder(dong, 1, dong + 3, 13, xlSheet);
        //        setFontBold(dong, 1, dong + 3, 13, 12, xlSheet);
        //        NumberFormat(6, 6, dong + 3, 7, xlSheet);

        //        DateFormat(6, 12, dong, 12, xlSheet);
        //        //xlSheet.Cells[dong, 1, dong, 12].Merge = true;
        //        //xlSheet.Cells[dong, 1].Value = vm.NguoiTao;
        //        //xlSheet.Cells[dong, 1].Style.Font.SetFromFont(new Font("Times New Roman", 12, FontStyle.Bold));
        //        ////TrSetCellBorder(xlSheet, dong, 1, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, Color.Silver, "Times New Roman", 12, FontStyle.Regular);
        //        //xlSheet.Cells[dong, 1].Style.Font.Bold = true;

        //        //NumberFormat(6, 8, dong + 1, 9, xlSheet);
        //        //dong = dong + 3;
        //        //idem = 1;

        //        //NumberFormat(dong, 2, dong, 5, xlSheet);
        //        //setFontBold(dong, 2, dong, 5, 12, xlSheet);
        //        //setBorder(dong, 2, dong, 5, xlSheet);
        //    }
        //    else
        //    {
        //        SetAlert("No sale.", "warning");
        //        return RedirectToAction(nameof(DoanhSoTheoThiTruong));
        //    }

        //    //dong++;
        //    //// Merger cot 4,5 ghi tổng tiền
        //    //setRightAligment(dong, 3, dong, 3, xlSheet);
        //    //xlSheet.Cells[dong, 1, dong, 2].Merge = true;
        //    //xlSheet.Cells[dong, 1].Value = "Tổng tiền: ";

        //    // Sum tổng tiền
        //    // xlSheet.Cells[dong, 5].Value = "TC:";
        //    //DateTimeFormat(6, 4, 6 + d.Count(), 4, xlSheet);
        //    // DateTimeFormat(6, 4, 9, 4, xlSheet);
        //    // setCenterAligment(6, 4, 9, 4, xlSheet);
        //    // xlSheet.Cells[dong, 6].Formula = "SUM(F6:F" + (6 + d.Count() - 1) + ")";

        //    //setBorder(5, 1, 5 + d.Count() + 2, 10, xlSheet);

        //    //setFontBold(5, 1, 5, 8, 11, xlSheet);
        //    //setFontSize(6, 1, 6 + d.Count() + 2, 8, 11, xlSheet);
        //    // canh giua cot stt
        //    setCenterAligment(6, 1, 6 + dong + 2, 1, xlSheet);
        //    // canh giua code chinhanh
        //    setCenterAligment(6, 3, 6 + dong + 2, 3, xlSheet);
        //    // NumberFormat(6, 6, 6 + d.Count(), 6, xlSheet);
        //    // định dạng số cot, đơn giá, thành tiền tong cong
        //    // NumberFormat(6, 8, dong, 9, xlSheet);

        //    // setBorder(dong, 5, dong, 6, xlSheet);
        //    // setFontBold(dong, 5, dong, 6, 12, xlSheet);

        //    //xlSheet.View.FreezePanes(6, 20);

        //    //end du lieu

        //    byte[] fileContents;
        //    try
        //    {
        //        fileContents = ExcelApp.GetAsByteArray();
        //        return File(
        //        fileContents: fileContents,
        //        contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //        fileDownloadName: "DoanhSoTheoThiTruong_" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ".xlsx");
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        #endregion Doanh so theo thi truong

        private string LoadTuNgayDenNgay(string tuThang1, string denThang1, string nam1)
        {
            string searchFromDate = "01/" + tuThang1 + "/" + nam1;
            string searchToDate = "01/" + denThang1 + "/" + nam1;

            // thang co 31 ngay
            if (denThang1 == "1" || denThang1 == "3" || denThang1 == "5" || denThang1 == "7" || denThang1 == "8" || denThang1 == "10" || denThang1 == "12")
            {
                searchToDate = "31/" + denThang1 + "/" + nam1;
            }
            // thang co 30 ngay
            if (denThang1 == "4" || denThang1 == "6" || denThang1 == "9" || denThang1 == "11")
            {
                searchToDate = "30/" + denThang1 + "/" + nam1;
            }
            // kiem tra nam nhuan
            if ((denThang1 == "2") && (int.Parse(nam1) % 400 == 0)) // chia het 400 => nam nhuan
            {
                searchToDate = "29/" + denThang1 + "/" + nam1;
            }
            if ((denThang1 == "2") && (int.Parse(nam1) % 400 != 0)) // ko phai nam nhuan
            {
                searchToDate = "28/" + denThang1 + "/" + nam1;
            }

            return searchFromDate + "-" + searchToDate;
        }


        private void DoanhSoTheoSaleGroupbyNguoiTao_IB()
        {
            ///////////////////////////////// group by ////////////////////////////////////////////

            //With Query Syntax
            var results1 = (
                from p in BaoCaoVM.TourIBDTOs
                group p by p.NguoiTao into g
                select new TourIBDtosGroupByNguoiTaoViewModel()
                {
                    NguoiTao = g.Key,
                    TourIBDTOs = g.ToList()
                }
                ).ToList();
            BaoCaoVM.TourIBDtosGroupByNguoiTaos = results1;
            ////////////// tinh TC /////////////////////

            foreach (var item in results1)
            {
                ////decimal? tongCong = 0;
                //// chua thanh ly hop dong
                //var chuaThanhLyHopDong = item.TourBaoCaoDtos.Where(x => string.IsNullOrEmpty(x.NgayThanhLyHD.ToString())).Sum(x => (x.DoanhThuTT == 0) ? x.DoanhThuDK : x.DoanhThuTT);
                //// da thanh ly hop dong
                //var daThanhLyHopDong = item.TourBaoCaoDtos.Where(x => !string.IsNullOrEmpty(x.NgayThanhLyHD.ToString())).Sum(x => (x.DoanhThuTT == 0) ? x.DoanhThuDK : x.DoanhThuTT);
                //// tong cong theo tung sale
                //var tongCongTheoTungSale = chuaThanhLyHopDong + daThanhLyHopDong;
                // sokhach
                var soKhach = item.TourIBDTOs.Sum(x => (x.SoKhachTT == 0) ? x.SoKhachDK : x.SoKhachTT);

                decimal chuaThanhLyHopDong = 0, daThanhLyHopDong = 0;
                foreach (var itemDto in item.TourIBDTOs)
                {
                    var ngayThanhLyHD = itemDto.NgayThanhLyHD.ToString("dd/MM/yyyy");
                    if (ngayThanhLyHD == "01/01/0001" || string.IsNullOrEmpty(ngayThanhLyHD))
                    {
                        chuaThanhLyHopDong += (itemDto.DoanhThuTT == 0) ? itemDto.DoanhThuDK : itemDto.DoanhThuTT;
                    }
                    else
                    {
                        daThanhLyHopDong += (itemDto.DoanhThuTT == 0) ? itemDto.DoanhThuDK : itemDto.DoanhThuTT;
                    }
                }

                foreach (var item1 in item.TourIBDTOs)
                {
                    item1.ChuaThanhLyHopDong = chuaThanhLyHopDong;
                    item1.DaThanhLyHopDong = daThanhLyHopDong;
                    item1.TongCongTheoTungSale = chuaThanhLyHopDong + daThanhLyHopDong;
                    item1.TongSoKhachTheoSale = soKhach;
                }

                //foreach (var item1 in item.ChiTietHdViewModels)
                //{
                //    item1.TC = tongCong;
                //}
            }

            decimal? tongCong = 0;
            int tongCongSK = 0;
            foreach (var item in results1)
            {
                tongCong += item.TourIBDTOs.FirstOrDefault().ChuaThanhLyHopDong + item.TourIBDTOs.FirstOrDefault().DaThanhLyHopDong;
                tongCongSK += item.TourIBDTOs.FirstOrDefault().TongSoKhachTheoSale;
            }
            BaoCaoVM.TongCong = tongCong;
            BaoCaoVM.TongSK = tongCongSK;
            ////////////// tinh TC /////////////////////

            //foreach (var item in results1)
            //{
            //    System.Diagnostics.Debug.WriteLine(item.NoiLamViec);
            //    foreach (var car in item.ChiTietHdViewModels)
            //    {
            //        System.Diagnostics.Debug.WriteLine(car.TenMon);
            //    }
            //}

            //System.Diagnostics.Debug.WriteLine("-----------");

            //////////////////////////// group by/////////////////////////////////////////////////
        }
        private void DoanhSoTheoSaleGroupbyNguoiTao_ND()
        {
            ///////////////////////////////// group by ////////////////////////////////////////////

            //With Query Syntax
            var results1 = (
                from p in BaoCaoVM.TourNDDTOs
                group p by p.Nguoitao into g
                select new TourNDDtosGroupByNguoiTaoViewModel()
                {
                    NguoiTao = g.Key,
                    TourNDDTOs = g.ToList()
                }
                ).ToList();
            BaoCaoVM.TourNDDtosGroupByNguoiTaos = results1;
            ////////////// tinh TC /////////////////////

            foreach (var item in results1)
            {
                ////decimal? tongCong = 0;
                //// chua thanh ly hop dong
                //var chuaThanhLyHopDong = item.TourBaoCaoDtos.Where(x => string.IsNullOrEmpty(x.NgayThanhLyHD.ToString())).Sum(x => (x.DoanhThuTT == 0) ? x.DoanhThuDK : x.DoanhThuTT);
                //// da thanh ly hop dong
                //var daThanhLyHopDong = item.TourBaoCaoDtos.Where(x => !string.IsNullOrEmpty(x.NgayThanhLyHD.ToString())).Sum(x => (x.DoanhThuTT == 0) ? x.DoanhThuDK : x.DoanhThuTT);
                //// tong cong theo tung sale
                //var tongCongTheoTungSale = chuaThanhLyHopDong + daThanhLyHopDong;
                // sokhach
                var soKhach = item.TourNDDTOs.Sum(x => (x.Sokhachtt == 0) ? x.Sokhachdk : x.Sokhachtt);

                decimal? chuaThanhLyHopDong = 0, daThanhLyHopDong = 0;
                foreach (var itemDto in item.TourNDDTOs)
                {
                    var ngayThanhLyHD = itemDto.Ngaythanhlyhd == null ? "" : itemDto.Ngaythanhlyhd.Value.ToString("dd/MM/yyyy");
                    if (ngayThanhLyHD == "01/01/0001" || string.IsNullOrEmpty(ngayThanhLyHD))
                    {
                        chuaThanhLyHopDong += (itemDto.Doanhthutt == 0 || itemDto.Doanhthutt == null) ? itemDto.Doanhthudk : itemDto.Doanhthutt;
                    }
                    else
                    {
                        daThanhLyHopDong += (itemDto.Doanhthutt == 0) ? itemDto.Doanhthudk : itemDto.Doanhthutt;
                    }
                }

                foreach (var item1 in item.TourNDDTOs)
                {
                    item1.ChuaThanhLyHopDong = chuaThanhLyHopDong;
                    item1.DaThanhLyHopDong = daThanhLyHopDong;
                    item1.TongCongTheoTungSale = chuaThanhLyHopDong + daThanhLyHopDong;
                    item1.TongSoKhachTheoSale = soKhach;
                }

                //foreach (var item1 in item.ChiTietHdViewModels)
                //{
                //    item1.TC = tongCong;
                //}
            }

            decimal? tongCong = 0;
            int? tongCongSK = 0;
            foreach (var item in results1)
            {
                tongCong += item.TourNDDTOs.FirstOrDefault().ChuaThanhLyHopDong + item.TourNDDTOs.FirstOrDefault().DaThanhLyHopDong;
                tongCongSK += item.TourNDDTOs.FirstOrDefault().TongSoKhachTheoSale;
            }
            BaoCaoVM.TongCong = tongCong;
            BaoCaoVM.TongSK = tongCongSK;
            ////////////// tinh TC /////////////////////

            //foreach (var item in results1)
            //{
            //    System.Diagnostics.Debug.WriteLine(item.NoiLamViec);
            //    foreach (var car in item.ChiTietHdViewModels)
            //    {
            //        System.Diagnostics.Debug.WriteLine(car.TenMon);
            //    }
            //}

            //System.Diagnostics.Debug.WriteLine("-----------");

            //////////////////////////// group by/////////////////////////////////////////////////
        }

        private void DoanhSoTheoSaleGroupbyNguoiTao_OB()
        {
            ///////////////////////////////// group by ////////////////////////////////////////////

            //With Query Syntax
            var results1 = (
                from p in BaoCaoVM.TourOBDTOs
                group p by p.Nguoitao into g
                select new TourOBDtosGroupByNguoiTaoViewModel()
                {
                    NguoiTao = g.Key,
                    TourOBDTOs = g.ToList()
                }
                ).ToList();
            BaoCaoVM.TourOBDtosGroupByNguoiTaos = results1;
            ////////////// tinh TC /////////////////////

            foreach (var item in results1)
            {
                ////decimal? tongCong = 0;
                //// chua thanh ly hop dong
                //var chuaThanhLyHopDong = item.TourBaoCaoDtos.Where(x => string.IsNullOrEmpty(x.NgayThanhLyHD.ToString())).Sum(x => (x.DoanhThuTT == 0) ? x.DoanhThuDK : x.DoanhThuTT);
                //// da thanh ly hop dong
                //var daThanhLyHopDong = item.TourBaoCaoDtos.Where(x => !string.IsNullOrEmpty(x.NgayThanhLyHD.ToString())).Sum(x => (x.DoanhThuTT == 0) ? x.DoanhThuDK : x.DoanhThuTT);
                //// tong cong theo tung sale
                //var tongCongTheoTungSale = chuaThanhLyHopDong + daThanhLyHopDong;
                // sokhach
                var soKhach = item.TourOBDTOs.Sum(x => (x.Sokhachtt == 0) ? x.Sokhachdk : x.Sokhachtt);

                decimal? chuaThanhLyHopDong = 0, daThanhLyHopDong = 0;
                foreach (var itemDto in item.TourOBDTOs)
                {
                    var ngayThanhLyHD = itemDto.Ngaythanhlyhd == null ? "" : itemDto.Ngaythanhlyhd.Value.ToString("dd/MM/yyyy");
                    if (ngayThanhLyHD == "01/01/0001" || string.IsNullOrEmpty(ngayThanhLyHD))
                    {
                        chuaThanhLyHopDong += (itemDto.Doanhthutt == 0 || itemDto.Doanhthutt == null) ?
                            itemDto.Doanhthudk : itemDto.Doanhthutt;
                    }
                    else
                    {
                        daThanhLyHopDong += (itemDto.Doanhthutt == 0 || itemDto.Doanhthutt == null) ?
                            itemDto.Doanhthudk : itemDto.Doanhthutt;
                    }
                }

                foreach (var item1 in item.TourOBDTOs)
                {
                    item1.ChuaThanhLyHopDong = chuaThanhLyHopDong;
                    item1.DaThanhLyHopDong = daThanhLyHopDong;
                    item1.TongCongTheoTungSale = chuaThanhLyHopDong + daThanhLyHopDong;
                    item1.TongSoKhachTheoSale = soKhach;
                }

                //foreach (var item1 in item.ChiTietHdViewModels)
                //{
                //    item1.TC = tongCong;
                //}
            }

            decimal? tongCong = 0;
            int? tongCongSK = 0;
            foreach (var item in results1)
            {
                tongCong += item.TourOBDTOs.FirstOrDefault().ChuaThanhLyHopDong + item.TourOBDTOs.FirstOrDefault().DaThanhLyHopDong;
                tongCongSK += item.TourOBDTOs.FirstOrDefault().TongSoKhachTheoSale;
            }
            BaoCaoVM.TongCong = tongCong;
            BaoCaoVM.TongSK = tongCongSK;
            ////////////// tinh TC /////////////////////

            //foreach (var item in results1)
            //{
            //    System.Diagnostics.Debug.WriteLine(item.NoiLamViec);
            //    foreach (var car in item.ChiTietHdViewModels)
            //    {
            //        System.Diagnostics.Debug.WriteLine(car.TenMon);
            //    }
            //}

            //System.Diagnostics.Debug.WriteLine("-----------");

            //////////////////////////// group by/////////////////////////////////////////////////
        }

        // DoanhSoTheoNgay_IB
        private void DoanhSoTheoNgay_IB()
        {
            BaoCaoVM.TourTheoNgay_IB.TourIBDTOs = BaoCaoVM.TourIBDTOs;
            //foreach (var item in BaoCaoVM.TourBaoCaoDtos)
            //{
            //var sk = (item.SoKhachTT == 0) ? item.SoKhachDK : item.SoKhachTT;
            var sk = BaoCaoVM.TourIBDTOs.Sum(x => (x.SoKhachTT == 0) ? x.SoKhachDK : x.SoKhachTT);
            //var ds = BaoCaoVM.TourBaoCaoDtos.Sum(x => (x.DoanhThuTT == 0) ? x.DoanhThuDK : x.DoanhThuTT);
            var ds = BaoCaoVM.TourIBDTOs.Sum(x => x.DoanhThuTT);

            //var skCacDoanDaThanhLy = BaoCaoVM.TourBaoCaoDtos.Where(x => x.TrangThai == "3") // (3)
            //                                                .Sum(x => (x.SoKhachTT == 0) ? x.SoKhachDK : x.SoKhachTT);
            var skCacDoanDaThanhLy = BaoCaoVM.TourIBDTOs.Where(x => x.TrangThai == "3") // (3)
                                                            .Sum(x => x.SoKhachTT);

            //var dsCacDoanDaThanhLy = BaoCaoVM.TourBaoCaoDtos.Where(x => x.TrangThai == "3")
            //                                                .Sum(x => (x.DoanhThuTT == 0) ? x.DoanhThuDK : x.DoanhThuTT);

            var dsCacDoanDaThanhLy = BaoCaoVM.TourIBDTOs.Where(x => x.TrangThai == "3")
                                                            .Sum(x => x.DoanhThuTT);

            //var skCacDoanChuaThanhLy = BaoCaoVM.TourBaoCaoDtos.Where(x => x.TrangThai != "3") // gom moitao (0), da damphan (1), da ky Hd (2)
            //                                                  .Sum(x => (x.SoKhachTT == 0) ? x.SoKhachDK : x.SoKhachTT);

            var skCacDoanChuaThanhLy = BaoCaoVM.TourIBDTOs.Where(x => x.TrangThai != "3") // gom moitao (0), da damphan (1), da ky Hd (2)
                                                  .Sum(x => x.SoKhachTT);

            //var dsCacDoanChuaThanhLy = BaoCaoVM.TourBaoCaoDtos.Where(x => x.TrangThai != "3")
            //                                                  .Sum(x => (x.DoanhThuTT == 0) ? x.DoanhThuDK : x.DoanhThuTT);

            var dsCacDoanChuaThanhLy = BaoCaoVM.TourIBDTOs.Where(x => x.TrangThai != "3")
                                                              .Sum(x => x.DoanhThuTT);

            //var skCacDoanChuaKyHD = BaoCaoVM.TourBaoCaoDtos.Where(x => x.TrangThai == "0" || x.TrangThai == "1") // gom moitao (0), da damphan (1)
            //                                               .Sum(x => (x.SoKhachTT == 0) ? x.SoKhachDK : x.SoKhachTT);

            var skCacDoanChuaKyHD = BaoCaoVM.TourIBDTOs.Where(x => x.TrangThai == "0" || x.TrangThai == "1") // gom moitao (0), da damphan (1)
                                                           .Sum(x => x.SoKhachTT);

            //var dsCacDoanChuaKyHD = BaoCaoVM.TourBaoCaoDtos.Where(x => x.TrangThai == "0" || x.TrangThai == "1")
            //                                               .Sum(x => (x.DoanhThuTT == 0) ? x.DoanhThuDK : x.DoanhThuTT);

            var dsCacDoanChuaKyHD = BaoCaoVM.TourIBDTOs.Where(x => x.TrangThai == "0" || x.TrangThai == "1")
                                                           .Sum(x => x.DoanhThuTT);

            BaoCaoVM.TourTheoNgay_IB.TongSK += sk;
            BaoCaoVM.TourTheoNgay_IB.TongDS += ds;

            BaoCaoVM.TourTheoNgay_IB.TongSKCacDoanDaThanhLy += skCacDoanDaThanhLy;
            BaoCaoVM.TourTheoNgay_IB.TongDSCacDoanDaThanhLy += dsCacDoanDaThanhLy;

            BaoCaoVM.TourTheoNgay_IB.TongSKCacDoanChuaThanhLy += skCacDoanChuaThanhLy;
            BaoCaoVM.TourTheoNgay_IB.TongDSCacDoanChuaThanhLy += dsCacDoanChuaThanhLy;

            BaoCaoVM.TourTheoNgay_IB.TongSKCacDoanChuaKyHD += skCacDoanChuaKyHD;
            BaoCaoVM.TourTheoNgay_IB.TongDSCacDoanChuaKyHD += dsCacDoanChuaKyHD;

            //}

            //foreach (var item1 in item.ChiTietHdViewModels)
            //{
            //    item1.TC = tongCong;
            //}
        }

        // DoanhSoTheoNgay_ND
        private void DoanhSoTheoNgay_ND()
        {
            BaoCaoVM.TourTheoNgay_ND.TourNDDTOs = BaoCaoVM.TourNDDTOs;
            //foreach (var item in BaoCaoVM.TourBaoCaoDtos)
            //{
            //var sk = (item.SoKhachTT == 0) ? item.SoKhachDK : item.SoKhachTT;
            var sk = BaoCaoVM.TourNDDTOs.Sum(x => (x.Sokhachtt == 0) ? x.Sokhachdk : x.Sokhachtt);
            //var ds = BaoCaoVM.TourBaoCaoDtos.Sum(x => (x.DoanhThuTT == 0) ? x.DoanhThuDK : x.DoanhThuTT);
            var ds = BaoCaoVM.TourNDDTOs.Sum(x => x.Doanhthutt);

            //var skCacDoanDaThanhLy = BaoCaoVM.TourBaoCaoDtos.Where(x => x.TrangThai == "3") // (3)
            //                                                .Sum(x => (x.SoKhachTT == 0) ? x.SoKhachDK : x.SoKhachTT);
            var skCacDoanDaThanhLy = BaoCaoVM.TourNDDTOs.Where(x => x.Trangthai == "3") // (3)
                                                            .Sum(x => x.Sokhachtt);

            //var dsCacDoanDaThanhLy = BaoCaoVM.TourBaoCaoDtos.Where(x => x.TrangThai == "3")
            //                                                .Sum(x => (x.DoanhThuTT == 0) ? x.DoanhThuDK : x.DoanhThuTT);

            var dsCacDoanDaThanhLy = BaoCaoVM.TourNDDTOs.Where(x => x.Trangthai == "3")
                                                            .Sum(x => x.Doanhthutt);

            //var skCacDoanChuaThanhLy = BaoCaoVM.TourBaoCaoDtos.Where(x => x.TrangThai != "3") // gom moitao (0), da damphan (1), da ky Hd (2)
            //                                                  .Sum(x => (x.SoKhachTT == 0) ? x.SoKhachDK : x.SoKhachTT);

            var skCacDoanChuaThanhLy = BaoCaoVM.TourNDDTOs.Where(x => x.Trangthai != "3") // gom moitao (0), da damphan (1), da ky Hd (2)
                                                  .Sum(x => x.Sokhachtt);

            //var dsCacDoanChuaThanhLy = BaoCaoVM.TourBaoCaoDtos.Where(x => x.TrangThai != "3")
            //                                                  .Sum(x => (x.DoanhThuTT == 0) ? x.DoanhThuDK : x.DoanhThuTT);

            var dsCacDoanChuaThanhLy = BaoCaoVM.TourNDDTOs.Where(x => x.Trangthai != "3")
                                                              .Sum(x => x.Doanhthutt);

            //var skCacDoanChuaKyHD = BaoCaoVM.TourBaoCaoDtos.Where(x => x.TrangThai == "0" || x.TrangThai == "1") // gom moitao (0), da damphan (1)
            //                                               .Sum(x => (x.SoKhachTT == 0) ? x.SoKhachDK : x.SoKhachTT);

            var skCacDoanChuaKyHD = BaoCaoVM.TourNDDTOs.Where(x => x.Trangthai == "0" || x.Trangthai == "1") // gom moitao (0), da damphan (1)
                                                           .Sum(x => x.Sokhachtt);

            //var dsCacDoanChuaKyHD = BaoCaoVM.TourBaoCaoDtos.Where(x => x.TrangThai == "0" || x.TrangThai == "1")
            //                                               .Sum(x => (x.DoanhThuTT == 0) ? x.DoanhThuDK : x.DoanhThuTT);

            var dsCacDoanChuaKyHD = BaoCaoVM.TourNDDTOs.Where(x => x.Trangthai == "0" || x.Trangthai == "1")
                                                           .Sum(x => x.Doanhthutt);

            BaoCaoVM.TourTheoNgay_ND.TongSK += sk ?? 0;
            BaoCaoVM.TourTheoNgay_ND.TongDS += ds ?? 0;

            BaoCaoVM.TourTheoNgay_ND.TongSKCacDoanDaThanhLy += skCacDoanDaThanhLy ?? 0;
            BaoCaoVM.TourTheoNgay_ND.TongDSCacDoanDaThanhLy += dsCacDoanDaThanhLy ?? 0;

            BaoCaoVM.TourTheoNgay_ND.TongSKCacDoanChuaThanhLy += skCacDoanChuaThanhLy ?? 0;
            BaoCaoVM.TourTheoNgay_ND.TongDSCacDoanChuaThanhLy += dsCacDoanChuaThanhLy ?? 0;

            BaoCaoVM.TourTheoNgay_ND.TongSKCacDoanChuaKyHD += skCacDoanChuaKyHD ?? 0;
            BaoCaoVM.TourTheoNgay_ND.TongDSCacDoanChuaKyHD += dsCacDoanChuaKyHD ?? 0;

            //}

            //foreach (var item1 in item.ChiTietHdViewModels)
            //{
            //    item1.TC = tongCong;
            //}
        }

        // DoanhSoTheoNgay_OB
        private void DoanhSoTheoNgay_OB()
        {
            BaoCaoVM.TourTheoNgay_OB.TourOBDTOs = BaoCaoVM.TourOBDTOs;
            //foreach (var item in BaoCaoVM.TourBaoCaoDtos)
            //{
            //var sk = (item.SoKhachTT == 0) ? item.SoKhachDK : item.SoKhachTT;
            var sk = BaoCaoVM.TourOBDTOs.Sum(x => (x.Sokhachtt == 0) ? x.Sokhachdk : x.Sokhachtt);
            //var ds = BaoCaoVM.TourBaoCaoDtos.Sum(x => (x.DoanhThuTT == 0) ? x.DoanhThuDK : x.DoanhThuTT);
            var ds = BaoCaoVM.TourOBDTOs.Sum(x => x.Doanhthutt);

            //var skCacDoanDaThanhLy = BaoCaoVM.TourBaoCaoDtos.Where(x => x.TrangThai == "3") // (3)
            //                                                .Sum(x => (x.SoKhachTT == 0) ? x.SoKhachDK : x.SoKhachTT);
            var skCacDoanDaThanhLy = BaoCaoVM.TourOBDTOs.Where(x => x.Trangthai == "3") // (3)
                                                            .Sum(x => x.Sokhachtt);

            //var dsCacDoanDaThanhLy = BaoCaoVM.TourBaoCaoDtos.Where(x => x.TrangThai == "3")
            //                                                .Sum(x => (x.DoanhThuTT == 0) ? x.DoanhThuDK : x.DoanhThuTT);

            var dsCacDoanDaThanhLy = BaoCaoVM.TourOBDTOs.Where(x => x.Trangthai == "3")
                                                            .Sum(x => x.Doanhthutt);

            //var skCacDoanChuaThanhLy = BaoCaoVM.TourBaoCaoDtos.Where(x => x.TrangThai != "3") // gom moitao (0), da damphan (1), da ky Hd (2)
            //                                                  .Sum(x => (x.SoKhachTT == 0) ? x.SoKhachDK : x.SoKhachTT);

            var skCacDoanChuaThanhLy = BaoCaoVM.TourOBDTOs.Where(x => x.Trangthai != "3") // gom moitao (0), da damphan (1), da ky Hd (2)
                                                  .Sum(x => x.Sokhachtt);

            //var dsCacDoanChuaThanhLy = BaoCaoVM.TourBaoCaoDtos.Where(x => x.TrangThai != "3")
            //                                                  .Sum(x => (x.DoanhThuTT == 0) ? x.DoanhThuDK : x.DoanhThuTT);

            var dsCacDoanChuaThanhLy = BaoCaoVM.TourOBDTOs.Where(x => x.Trangthai != "3")
                                                              .Sum(x => x.Doanhthutt);

            //var skCacDoanChuaKyHD = BaoCaoVM.TourBaoCaoDtos.Where(x => x.TrangThai == "0" || x.TrangThai == "1") // gom moitao (0), da damphan (1)
            //                                               .Sum(x => (x.SoKhachTT == 0) ? x.SoKhachDK : x.SoKhachTT);

            var skCacDoanChuaKyHD = BaoCaoVM.TourOBDTOs.Where(x => x.Trangthai == "0" || x.Trangthai == "1") // gom moitao (0), da damphan (1)
                                                           .Sum(x => x.Sokhachtt);

            //var dsCacDoanChuaKyHD = BaoCaoVM.TourBaoCaoDtos.Where(x => x.TrangThai == "0" || x.TrangThai == "1")
            //                                               .Sum(x => (x.DoanhThuTT == 0) ? x.DoanhThuDK : x.DoanhThuTT);

            var dsCacDoanChuaKyHD = BaoCaoVM.TourOBDTOs.Where(x => x.Trangthai == "0" || x.Trangthai == "1")
                                                           .Sum(x => x.Doanhthutt);

            BaoCaoVM.TourTheoNgay_OB.TongSK += sk ?? 0;
            BaoCaoVM.TourTheoNgay_OB.TongDS += ds ?? 0;

            BaoCaoVM.TourTheoNgay_OB.TongSKCacDoanDaThanhLy += skCacDoanDaThanhLy ?? 0;
            BaoCaoVM.TourTheoNgay_OB.TongDSCacDoanDaThanhLy += dsCacDoanDaThanhLy ?? 0;

            BaoCaoVM.TourTheoNgay_OB.TongSKCacDoanChuaThanhLy += skCacDoanChuaThanhLy ?? 0;
            BaoCaoVM.TourTheoNgay_OB.TongDSCacDoanChuaThanhLy += dsCacDoanChuaThanhLy ?? 0;

            BaoCaoVM.TourTheoNgay_OB.TongSKCacDoanChuaKyHD += skCacDoanChuaKyHD ?? 0;
            BaoCaoVM.TourTheoNgay_OB.TongDSCacDoanChuaKyHD += dsCacDoanChuaKyHD ?? 0;

            //}

            //foreach (var item1 in item.ChiTietHdViewModels)
            //{
            //    item1.TC = tongCong;
            //}
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