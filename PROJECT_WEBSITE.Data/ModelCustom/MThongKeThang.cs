using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MThongKeThang
    {
        public int STT { get; set; }

        public DateTime? Ngay { get; set; }

        public int SoDonHang { get; set; }

        public int SoLuong { get; set; }

        public decimal? TienHang { get; set; }

        public decimal? GiamGia { get; set; }

        public decimal? ShipPrice { get; set; }

    }
    public class MThongKeNhanVien
    {
        public int STT { get; set; }

        public string UserName { get; set; }

        public int UserID { get; set; }

        public int SoDonHang { get; set; }

        public int SoLuong { get; set; }

        public decimal? TienHang { get; set; }
    }
    public class MThongKeKhachHang
    {
        public int STT { get; set; }

        public string CustomerName { get; set; }

        public int CustomerID { get; set; }

        public int SoDonHang { get; set; }

        public int SoLuong { get; set; }

        public decimal? TienHang { get; set; }
    }
    public class MThongKeHangHoa
    {
        public int STT { get; set; }

        public int? UserID { get; set; }

        public string KyTuCaterory { get; set; }

        public string ProductName { get; set; }

        public int ProductID { get; set; }

        public string ProductIDDisplay { get; set; }

        public string DVT { get; set; }

        public int SoDonHang { get; set; }

        public int SoLuong { get; set; }

        public DateTime CreateDate { get; set; }

        public decimal? TienHang { get; set; }
    }
    public class MThongKeHangHoaChitiet
    {
        public int STT { get; set; }

        public int OrderID { get; set; }

        public string KyTuCaterory { get; set; }

        public int? UserID { get; set; }

        public DateTime CreateDate { get; set; }

        public string ProductIDDisplay { get; set; }

        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public string DVT { get; set; }

        public int SoLuong { get; set; }

        public decimal? DonGia { get; set; }

        public decimal? ThanhTien { get; set; }

        public string NguoiBan { get; set; }
    }
    public class SearchThongKe
    {
         public DateTime? TuNgay { get; set; }

         public DateTime? DenNgay { get; set; }
    }


    public class MThongKeLoiNhuanThang
    {
        public int STT { get; set; }

        public int? UserID { get; set; }

        public DateTime? CreateDate { get; set; }

        public decimal? TienHang { get; set; }

        public decimal? DoanhThu { get; set; }

        public decimal? Von { get; set; }

        public int Lai { get; set; }

        public List<int> OrderList { get; set; }


        public string ShipAddress { get; set; }

        public string ShipPhone { get; set; }

        public string ShipEmail { get; set; }

        public string Discription { get; set; }

        public string UserName { get; set; }

        //Chi tiết
        public int OrderID { get; set; }

        public string CustomerName { get; set; }
    }

    public class MThongKeLoiNhuanKhachHang
    {
        public int STT { get; set; }

        public int? CustomerID { get; set; }

        public int? UserID { get; set; }

        public DateTime? CreateDate { get; set; }

        public decimal? TienHang { get; set; }

        public decimal? DoanhThu { get; set; }

        public decimal? Von { get; set; }

        public int Lai { get; set; }

        public List<int> OrderList { get; set; }


        public string ShipAddress { get; set; }

        public string ShipPhone { get; set; }

        public string ShipEmail { get; set; }

        public string Discription { get; set; }

        public string UserName { get; set; }

        //Chi tiết
        public int OrderID { get; set; }

        public string CustomerName { get; set; }
    }

    public class MBaoCaoLaiLaiLo
    {
        public DateTime KyTruocTu { get; set; }
        public DateTime KyTruocDen { get; set; }

        public DateTime KyBaoCaoTu { get; set; }
        public DateTime KyBaoCaoDen { get; set; }


        public string DoanhSoBanHang { get; set; }

        public decimal? KyTruocDoanhSoBanHang { get; set; }

        public decimal? KyBaoCaoDoanhSoBanHang { get; set; }

        public decimal? ThaydoiDoanhSoBanHang { get; set; }


        public string GiamGia { get; set; }

        public decimal? KyTruocGiamGia { get; set; }

        public decimal? KyBaoCaoGiamGia { get; set; }

        public decimal? ThaydoiGiamGia { get; set; }


        public string PhiVanChuyen { get; set; }

        public decimal? KyTruocPhiVanChuyen { get; set; }

        public decimal? KyBaoCaoPhiVanChuyen { get; set; }

        public decimal? ThaydoiPhiVanChuyen { get; set; }


        public string DoanhThu { get; set; }

        public decimal? KyTruocDoanhThu { get; set; }

        public decimal? KyBaoCaoDoanhThu { get; set; }

        public decimal? ThaydoiDoanhThu { get; set; }


        public string VonHangHoa { get; set; }

        public decimal? KyTruocVonHangHoa { get; set; }

        public decimal? KyBaoCaoVonHangHoa { get; set; }

        public decimal? ThaydoiVonHangHoa { get; set; }


        public string LaiGop { get; set; }

        public decimal? KyTruocLaiGop { get; set; }

        public decimal? KyBaoCaoLaiGop { get; set; }

        public decimal? ThaydoiLaigop { get; set; }


        public string TienGiamChiaDoanhThu { get; set; }

        public double? KyTruocTienGiamChiaDoanhThu { get; set; }

        public double? KyBaoCaoTienGiamChiaDoanhThu { get; set; }

        public double? ThaydoiTienGiamChiaDoanhThu { get; set; }


        public string LaiGopChiaDoanhThu { get; set; }

        public double? KyTruocTienGiamChiaLaiGop { get; set; }

        public double? KyBaoCaoTienGiamChiaLaiGop { get; set; }

        public double? ThaydoiLaiGopChiaDoanhThu { get; set; }
    }


    public class SelectThang
    {
        public string TenThang { get; set; }

        public int ValueThang { get; set; }
    }
    public class SelectDasboard
    {
        public string TenThang { get; set; }

        public int ValueThang { get; set; }

        public string TenNamNay { get; set; }

        public int ValueNamNay { get; set; }

        public string TenNamTruoc { get; set; }

        public int ValueNamTruoc { get; set; }
        
        public List<SelectThang> ListThang { get; set; }
    }

    public class selectToltal
    {
        public string Name { get; set; }

        public int Value { get; set; }
    }


}
