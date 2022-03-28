using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PROJECT_WEBSITE.WebAPP.Common
{
    [Serializable]
    public class UserLogin
    {
        public int UserID { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string RoleString { get; set; }

        public int Role { get; set; }

        public List<int> Roles { get; set; }

    }
    public enum Role
    {
        QuanLyDonhang = 2,
        QuanLyKhoHang = 3,
        QuanLyKhachHang = 4,
        QuanLyTinTuc = 5,
        QuanLySlide= 6,
        QuanLyChanTrang = 7,
        QuanLyGioiThieu = 8,
        BaoCaoDoanhSo = 9,
        BaoCaoLoiNhuan = 10,
        TimTOPK = 11,
        Dashboard = 12,
        QuanLyNhapHang =13,
        QuanLyCombo = 14,
        QuanLyMaKhuyenMai = 15,
        QuanLyNhanVien = 16
    }
}