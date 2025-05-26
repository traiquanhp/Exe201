using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCookFinal.ModelDTO;
using SmartCookFinal.Models;

namespace SmartCookFinal.Controllers
{
	public class ThucDonController : Controller
	{

        private readonly SmartCookContext _context;
        public ThucDonController(SmartCookContext context)
        {
            _context = context;
        }

        [HttpGet]
		public IActionResult NhapChiSo()
		{
			return View();
		}

        [HttpPost]
        public IActionResult NhapChiSo(ThucDonRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                return TaoThucDon(model); // Gọi luôn hàm xử lý tạo thực đơn
            }

            return View(model);
        }


        private double TinhBMR(string gioiTinh, int tuoi, double canNang, double chieuCao)
        {
            if (gioiTinh.ToLower() == "nam")
            {
                return 66.47 + (13.75 * canNang) + (5.003 * chieuCao) - (6.755 * tuoi);
            }
            else
            {
                return 655.1 + (9.563 * canNang) + (1.850 * chieuCao) - (4.676 * tuoi);
            }
        }

        private double TinhTDEE(double bmr, string mucDoHoatDong)
        {
            switch (mucDoHoatDong.ToLower())
            {
                case "it":
                    return bmr * 1.2;
                case "vua":
                    return bmr * 1.55;
                case "nhieu":
                    return bmr * 1.9;
                default:
                    return bmr * 1.2;
            }
        }

        private double DieuChinhTheoMucTieu(double tdee, string mucTieu)
        {
            switch (mucTieu.ToLower())
            {
                case "giam can":
                    return tdee - 300;
                case "tang can":
                    return tdee + 300;
                default:
                    return tdee;
            }
        }

        private double TinhCaloMoiBua(double tongCalo, int soBua)
        {
            return tongCalo / soBua;
        }

        public List<MonAn> TimMonAnPhuHop(
            double caloMoiBua,
            string cheDoAn,
            string loaiBua,
            List<string> diUng,
            List<string> khongThich)
        {
            var query = _context.MonAns
    .Where(m =>
        m.LuongCalo != null &&
        m.LoaiBuaAn != null &&
        m.TrangThai == true &&
        m.LuongCalo <= caloMoiBua + 1000 &&
        m.LuongCalo >= caloMoiBua - 1000 &&
        m.LoaiBuaAn == loaiBua);


            // Lọc theo chế độ ăn
            if (!string.IsNullOrEmpty(cheDoAn))
            {
                switch (cheDoAn.ToLower())
                {
                    case "chay":
                        query = query.Where(m => m.Chay != null && m.Chay == true);
                        break;
                    case "keto":
                        query = query.Where(m => m.AnKeto != null && m.AnKeto == true);
                        break;
                    case "không gluten":
                        query = query.Where(m => m.AnKhongGluten != null && m.AnKhongGluten == true);
                        break;
                }
            }


            // Dị ứng hoặc không thích
            if (diUng != null && diUng.Any())
            {
                foreach (var item in diUng)
                {
                    query = query.Where(m => m.NguyenLieuChinh != null && !m.NguyenLieuChinh.Contains(item));
                }
            }

            if (khongThich != null && khongThich.Any())
            {
                foreach (var item in khongThich)
                {
                    query = query.Where(m => m.TenMon != null && !m.TenMon.Contains(item));
                }
            }



            try
            {
                var monAn = query.Take(2).ToList();
                return monAn;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi đọc món ăn: " + ex.Message);
                throw;
            }

        }


        public IActionResult TaoThucDon(ThucDonRequestViewModel model)
        {
            double bmr = TinhBMR(model.GioiTinh, model.Tuoi, model.CanNang, model.ChieuCao);
            double tdee = TinhTDEE(bmr, model.MucDoHoatDong);
            double tongCalo = DieuChinhTheoMucTieu(tdee, model.MucTieu);
            double caloMoiBua = TinhCaloMoiBua(tongCalo, model.SoBuaMotNgay);

            var danhSachThucDon = new List<MonAn>();

            string[] buas = { "Sáng", "Trưa", "Tối" };

            foreach (var bua in buas.Take(model.SoBuaMotNgay))
            {
                var monAn = TimMonAnPhuHop(
                    caloMoiBua,
                    model.CheDoAn,
                    bua,
                    model.DiUng?.Split(',').ToList() ?? new List<string>(),
                    model.KhongThich?.Split(',').ToList() ?? new List<string>()
                );

                danhSachThucDon.AddRange(monAn);
            }

            var ketQua = new ThucDonKetQuaViewModel
            {
                BMR = Math.Round(bmr, 2),
                TDEE = Math.Round(tdee, 2),
                TongCaloDieuChinh = Math.Round(tongCalo, 2),
                CaloMoiBua = Math.Round(caloMoiBua, 2),
                SoBuaMotNgay = model.SoBuaMotNgay
            };

            var viewTongHop = new ThucDonViewTongHop
            {
                DanhSachMonAn = danhSachThucDon,
                KetQua = ketQua
            };

            return View("TaoThucDon", viewTongHop);
        }


        
    }

}
