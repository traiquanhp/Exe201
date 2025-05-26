using System.ComponentModel.DataAnnotations;

namespace SmartCookFinal.ModelDTO
{
	public class ThucDonRequestViewModel
	{
		[Required]
		public string TenNguoiDung { get; set; }

		[Required]
		public string GioiTinh { get; set; } // Nam / Nữ

		[Required]
		public int Tuoi { get; set; }

		[Required]
		public float ChieuCao { get; set; } // cm

		[Required]
		public float CanNang { get; set; } // kg

		[Required]
		public string MucDoHoatDong { get; set; } // Ít, Vừa, Nhiều

		[Required]
		public string MucTieu { get; set; } // Giảm cân / Giữ cân / Tăng cân

		public string CheDoAn { get; set; } // Chay / Keto / Không Gluten...

		public string DiUng { get; set; } // Dị ứng gì không

		public string KhongThich { get; set; } // Món không thích

		[Required]
		public int SoBuaMotNgay { get; set; } = 3;

		public decimal? NganSachToiDa { get; set; }
	}
}
