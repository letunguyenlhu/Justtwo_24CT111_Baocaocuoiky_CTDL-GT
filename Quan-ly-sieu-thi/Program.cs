// Bước 1: Khai báo các thư viện chuẩn
using System;
using System.Collections.Generic; // Cho Dictionary, List
using System.Text;            // Cho Console.OutputEncoding

// ===== Bắt đầu Định nghĩa các Kiểu Dữ liệu và Class =====

#region 1. Kiểu Dữ liệu (Model)

/// <summary>
/// Định nghĩa kiểu Mặt Hàng bằng 'struct'.
/// </summary>
public struct MatHang
{
    // Biến công khai (public fields)
    public string MaMatHang;
    public string Ten;
    public string DonViTinh;
    public decimal DonGia;
    public int SoLuongTon;

    /// <summary>
    /// Constructor (hàm khởi tạo) để gán giá trị và kiểm tra (validate)
    /// </summary>
    public MatHang(string ma, string ten, string dvt, decimal gia, int sl)
    {
        if (string.IsNullOrWhiteSpace(ma))
            throw new Exception("Lỗi: Mã mặt hàng không được để trống.");
        if (string.IsNullOrWhiteSpace(ten))
            throw new Exception("Lỗi: Tên mặt hàng không được để trống.");
        if (gia < 0)
            throw new Exception("Lỗi: Đơn giá không được âm.");
        if (sl < 0)
            throw new Exception("Lỗi: Số lượng tồn không được âm.");

        this.MaMatHang = ma;
        this.Ten = ten;
        this.DonViTinh = dvt;
        this.DonGia = gia;
        this.SoLuongTon = sl;
    }

    /// <summary>
    /// Ghi đè (override) hàm ToString() để in thông tin ra cho đẹp.
    /// </summary>
    public override string ToString()
    {
        return "[Mã: " + MaMatHang + "] - Tên: " + Ten +
               " (ĐVT: " + DonViTinh + ") - Giá: " + DonGia +
               " VNĐ - Tồn: " + SoLuongTon;
    }
}

#endregion

#region 2. Lớp Nghiệp vụ (Service/Logic)

/// <summary>
/// Lớp 'QuanLySieuThi' (dùng class) để quản lý kho hàng.
/// </summary>
public class QuanLySieuThi
{
    private Dictionary<string, MatHang> _khoHang;

    /// <summary>
    /// Hàm khởi tạo của lớp QuanLySieuThi.
    /// </summary>
    public QuanLySieuThi()
    {
        _khoHang = new Dictionary<string, MatHang>();
    }

    /// <summary>
    /// (Public) Thêm một mặt hàng mới.
    /// </summary>
    public bool ThemMatHang(MatHang mh)
    {
        if (_khoHang.ContainsKey(mh.MaMatHang))
        {
            return false;
        }
        _khoHang.Add(mh.MaMatHang, mh);
        return true;
    }

    /// <summary>
    /// (Public) Thêm nhiều mặt hàng từ một danh sách.
    /// </summary>
    public int ThemNhieuMatHang(List<MatHang> danhSach)
    {
        int count = 0;
        foreach (MatHang mh in danhSach)
        {
            if (ThemMatHang(mh))
            {
                count = count + 1;
            }
        }
        return count;
    }


    /// <summary>
    /// (Public) Tìm mặt hàng theo mã.
    /// </summary>
    public MatHang TimTheoMa(string ma)
    {
        if (_khoHang.ContainsKey(ma))
        {
            return _khoHang[ma];
        }
        return new MatHang(); // Trả về struct rỗng
    }

    /// <summary>
    /// (Public) Tìm mặt hàng theo tên (chứa).
    /// </summary>
    public List<MatHang> TimTheoTen(string ten)
    {
        List<MatHang> ketQua = new List<MatHang>();
        string tenTim = ten.ToLower();

        foreach (MatHang mh in _khoHang.Values)
        {
            if (mh.Ten.ToLower().Contains(tenTim))
            {
                ketQua.Add(mh);
            }
        }
        return ketQua;
    }

    /// <summary>
    /// (Public) Cập nhật mặt hàng.
    /// </summary>
    public bool CapNhatMatHang(string ma, MatHang mhMoi)
    {
        if (_khoHang.ContainsKey(ma))
        {
            _khoHang[ma] = mhMoi;
            return true;
        }
        return false;
    }

    /// <summary>
    /// (Public) Xóa mặt hàng khỏi kho.
    /// </summary>
    public bool XoaMatHang(string ma)
    {
        return _khoHang.Remove(ma);
    }

    /// <summary>
    /// (Public) Lấy toàn bộ danh sách mặt hàng.
    /// </summary>
    public List<MatHang> LayToanBoMatHang()
    {
        List<MatHang> danhSach = new List<MatHang>();
        foreach (MatHang mh in _khoHang.Values)
        {
            danhSach.Add(mh);
        }
        return danhSach;
    }

    /// <summary>
    /// (Public) Lấy tổng số lượng mặt hàng đang quản lý.
    /// </summary>
    public int DemSoLuong()
    {
        return _khoHang.Count;
    }
}

#endregion

#region 3. Lớp Thuật toán Tổ hợp (Algorithm)

/// <summary>
/// Lớp 'static' (tĩnh) chứa thuật toán liệt kê tổ hợp.
/// </summary>
public static class ThuVienToHop
{
    private static long _demToHop;

    /// <summary>
    /// (Public) Hàm chính, sẽ gọi hàm đệ quy và IN TRỰC TIẾP ra Console.
    /// </summary>
    public static void LietKeVaInToHop(List<MatHang> danhSachNguon, int m)
    {
        // <<< THAY ĐỔI >>> N trong hàm này giờ là n (tập con) mà người dùng chọn
        int n_tapCon = danhSachNguon.Count;

        if (danhSachNguon == null || m <= 0 || m > n_tapCon)
        {
            Console.WriteLine("Dữ liệu không hợp lệ để tính tổ hợp.");
            return;
        }

        List<MatHang> comboHienTai = new List<MatHang>();
        _demToHop = 0;

        Console.WriteLine("--- Bắt đầu liệt kê C(" + n_tapCon + ", " + m + ") ---");

        LietKeRecursive(danhSachNguon, m, 0, comboHienTai);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("--- Hoàn thành. Tìm thấy " + _demToHop + " tổ hợp. ---");
        Console.ResetColor();
    }

    /// <summary>
    /// Hàm đệ quy (helper), 'private' (nội bộ).
    /// </summary>
    private static void LietKeRecursive(
        List<MatHang> danhSachNguon,
        int m,
        int indexBatDau,
        List<MatHang> comboHienTai)
    {
        if (comboHienTai.Count == m)
        {
            _demToHop++;
            Console.Write("Combo #" + _demToHop + ": ");
            decimal tongGia = 0;

            for (int i = 0; i < comboHienTai.Count; i++)
            {
                MatHang mh = comboHienTai[i];
                Console.Write("[" + mh.MaMatHang + " - " + mh.Ten + "]");
                tongGia = tongGia + mh.DonGia;
                if (i < comboHienTai.Count - 1)
                {
                    Console.Write(" + ");
                }
            }
            Console.WriteLine(" | Tổng giá: " + tongGia);

            return;
        }

        for (int i = indexBatDau; i < danhSachNguon.Count; i++)
        {
            comboHienTai.Add(danhSachNguon[i]);
            LietKeRecursive(danhSachNguon, m, i + 1, comboHienTai);
            comboHienTai.RemoveAt(comboHienTai.Count - 1);
        }
    }
}

#endregion

#region 4. Lớp Tạo Dữ liệu Mẫu (Utility)

/// <summary>
/// Lớp 'static' (tĩnh) để tạo dữ liệu mẫu ngẫu nhiên.
/// </summary>
public static class TestDataGenerator
{
    private static Random _random = new Random();
    private static string[] TenTienTo = { "Bánh", "Sữa", "Kẹo", "Nước ngọt", "Rau", "Thịt", "Cá", "Gạo", "Mì" };
    private static string[] TenHauTo = { "Oreo", "TH True Milk", "Chupa Chups", "Coca Cola", "Cải xanh", "Bò Kobe", "Hồi", "ST25", "Hảo Hảo" };
    private static string[] DonVi = { "Hộp", "Thùng", "Kg", "Gói", "Chai" };

    /// <summary>
    /// (Public) Tạo ra một danh sách các mặt hàng ngẫu nhiên.
    /// </summary>
    public static List<MatHang> Generate(int soLuong)
    {
        // <<< THAY ĐỔI >>> Giới hạn n < 10000 theo yêu cầu mới
        if (soLuong >= 10000)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Cảnh báo: Số lượng N (" + soLuong + ") quá lớn, giảm xuống còn 9999.");
            soLuong = 9999;
            Console.ResetColor();
        }

        List<MatHang> danhSach = new List<MatHang>(soLuong);
        for (int i = 0; i < soLuong; i++)
        {
            try
            {
                // <<< THAY ĐỔI >>> Format mã 4 số cho N < 10000
                string ma = "MH" + i.ToString("D4");
                string ten = TenTienTo[_random.Next(TenTienTo.Length)] + " " + TenHauTo[_random.Next(TenHauTo.Length)] + " #" + i;
                string dvt = DonVi[_random.Next(DonVi.Length)];
                decimal gia = _random.Next(5000, 500000);
                int tonKho = _random.Next(10, 1000);
                danhSach.Add(new MatHang(ma, ten, dvt, gia, tonKho));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Loi tao data: " + ex.Message);
            }
        }
        return danhSach;
    }
}

#endregion

#region 5. Lớp Chính (Entry Point)

/// <summary>
/// Lớp 'Program' chứa hàm 'Main' - điểm khởi đầu của ứng dụng Console.
/// </summary>
public class Program
{
    private static QuanLySieuThi _quanLySieuThi = new QuanLySieuThi();

    /// <summary>
    /// (Public) Hàm Main, điểm bắt đầu của chương trình.
    /// </summary>
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        Console.WriteLine("Chào mừng bạn đến với chương trình Quản lý Siêu thị!");
        Console.WriteLine("Kho hàng hiện đang rỗng. Hãy dùng chức năng '8' để tạo dữ liệu.");

        bool dangChay = true;
        while (dangChay)
        {
            Console.WriteLine("");
            Console.WriteLine("===== MENU QUẢN LÝ SIÊU THỊ ABC (Đơn giản) =====");
            int soLuongTrongKho = _quanLySieuThi.DemSoLuong(); // <<< THAY ĐỔI >>>
            Console.WriteLine("Hiện có " + soLuongTrongKho + " mặt hàng (N_kho = " + soLuongTrongKho + ")."); // <<< THAY ĐỔI >>>
            Console.WriteLine("1. Thêm mặt hàng mới ");
            Console.WriteLine("2. Tìm mặt hàng theo Mã");
            Console.WriteLine("3. Tìm mặt hàng theo Tên");
            Console.WriteLine("4. Cập nhật mặt hàng (theo Mã)");
            Console.WriteLine("5. Xóa mặt hàng (theo Mã)");
            Console.WriteLine("6. Tạo combo"); // <<< THAY ĐỔI >>>
            Console.WriteLine("7. In toàn bộ danh sách");
            Console.WriteLine("8. Tạo dữ liệu lưu kho (N < 10000)"); // <<< THAY ĐỔI >>>
            Console.WriteLine("0. Thoát");
            Console.Write("Vui lòng chọn chức năng: ");

            string luaChon = Console.ReadLine();
            Console.Clear();

            switch (luaChon)
            {
                case "1":
                    ThemMatHang();
                    break;
                case "2":
                    TimTheoMa();
                    break;
                case "3":
                    TimTheoTen();
                    break;
                case "4":
                    CapNhatMatHang();
                    break;
                case "5":
                    XoaMatHang();
                    break;
                case "6":
                    LietKeToHop(); // <<< THAY ĐỔI >>> Logic hàm này đã được cập nhật
                    break;
                case "7":
                    InToanBo();
                    break;
                case "8":
                    TaoNgauNhienDuLieu();
                    break;
                case "0":
                    dangChay = false;
                    Console.WriteLine("Đã thoát chương trình.");
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng chọn lại.");
                    Console.ResetColor();
                    break;
            }
        }
    }

    // ----- Các hàm xử lý Menu (private static) -----

    /// <summary>
    /// Chức năng 8: Tạo N dữ liệu ngẫu nhiên
    /// </summary>
    private static void TaoNgauNhienDuLieu()
    {
        // <<< THAY ĐỔI >>> Cập nhật câu hỏi theo N < 10000
        Console.Write("Bạn muốn tạo mới bao nhiêu (N < 10000) mặt hàng? (Sẽ xóa kho cũ!): ");
        int n;

        if (int.TryParse(Console.ReadLine(), out n) == false || n <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Số lượng N không hợp lệ.");
            Console.ResetColor();
            return;
        }

        // <<< THAY ĐỔI >>> Kiểm tra n < 10000
        if (n >= 10000)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Lỗi: N phải nhỏ hơn 10000.");
            Console.ResetColor();
            return;
        }

        try
        {
            Console.WriteLine("Đang xóa kho cũ...");
            _quanLySieuThi = new QuanLySieuThi();

            Console.WriteLine("Đang tạo " + n + " dữ liệu mới... Vui lòng đợi.");
            List<MatHang> dataMau = TestDataGenerator.Generate(n);

            int soLuongThem = _quanLySieuThi.ThemNhieuMatHang(dataMau);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Đã tạo thành công " + soLuongThem + " mặt hàng.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Đã xảy ra lỗi khi tạo dữ liệu: " + ex.Message);
            Console.ResetColor();
        }
    }


    private static void ThemMatHang()
    {
        try
        {
            Console.Write("Nhập mã mặt hàng (duy nhất): ");
            string ma = Console.ReadLine();
            Console.Write("Nhập tên: ");
            string ten = Console.ReadLine();
            Console.Write("Nhập đơn vị tính: ");
            string dvt = Console.ReadLine();
            Console.Write("Nhập đơn giá: ");
            decimal gia = decimal.Parse(Console.ReadLine());
            Console.Write("Nhập số lượng tồn: ");
            int sl = int.Parse(Console.ReadLine());

            MatHang mh = new MatHang(ma, ten, dvt, gia, sl);

            if (_quanLySieuThi.ThemMatHang(mh))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Thêm thành công!");
                Console.ResetColor();
                // THAY BẰNG DÒNG NÀY:
                InToanBo(); // Gọi hàm in toàn bộ danh sách
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Lỗi: Mã mặt hàng '" + ma + "' đã tồn tại.");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Lỗi: " + ex.Message);
            Console.ResetColor();
        }
    }

    private static void TimTheoMa()
    {
        Console.Write("Nhập mã mặt hàng cần tìm: ");
        string ma = Console.ReadLine();

        MatHang mhTimDuoc = _quanLySieuThi.TimTheoMa(ma);

        if (mhTimDuoc.MaMatHang != null)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Đã tìm thấy:");
            Console.WriteLine(mhTimDuoc.ToString());
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Không tìm thấy mặt hàng có mã '" + ma + "'.");
            Console.ResetColor();
        }
    }

    private static void TimTheoTen()
    {
        Console.Write("Nhập tên mặt hàng cần tìm: ");
        string ten = Console.ReadLine();

        List<MatHang> danhSach = _quanLySieuThi.TimTheoTen(ten);

        if (danhSach.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Tìm thấy " + danhSach.Count + " kết quả:");
            foreach (MatHang mh in danhSach)
            {
                Console.WriteLine(mh.ToString());
            }
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Không tìm thấy mặt hàng nào có tên chứa '" + ten + "'.");
            Console.ResetColor();
        }
    }

    private static void CapNhatMatHang()
    {
        Console.Write("Nhập mã mặt hàng cần cập nhật: ");
        string ma = Console.ReadLine();

        MatHang mhCanSua = _quanLySieuThi.TimTheoMa(ma);

        if (mhCanSua.MaMatHang == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Không tìm thấy mặt hàng có mã '" + ma + "'.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine("Đã tìm thấy: " + mhCanSua.ToString());
        Console.WriteLine("Nhập thông tin mới (để trống nếu muốn giữ cũ):");

        try
        {
            Console.Write("Tên mới (cũ: " + mhCanSua.Ten + "): ");
            string tenMoi = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(tenMoi) == false)
            {
                mhCanSua.Ten = tenMoi;
            }

            Console.Write("ĐVT mới (cũ: " + mhCanSua.DonViTinh + "): ");
            string dvtMoi = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(dvtMoi) == false)
            {
                mhCanSua.DonViTinh = dvtMoi;
            }

            Console.Write("Giá mới (cũ: " + mhCanSua.DonGia + "): ");
            string giaMoiStr = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(giaMoiStr) == false)
            {
                mhCanSua.DonGia = decimal.Parse(giaMoiStr);
            }

            Console.Write("SL tồn mới (cũ: " + mhCanSua.SoLuongTon + "): ");
            string slMoiStr = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(slMoiStr) == false)
            {
                mhCanSua.SoLuongTon = int.Parse(slMoiStr);
            }

            if (_quanLySieuThi.CapNhatMatHang(ma, mhCanSua))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Cập nhật thành công!");
                Console.WriteLine(mhCanSua.ToString());
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Lỗi: Cập nhật thất bại.");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Lỗi nhập liệu: " + ex.Message);
            Console.ResetColor();
        }
    }

    private static void XoaMatHang()
    {
        Console.Write("Nhập mã mặt hàng cần XÓA: ");
        string ma = Console.ReadLine();

        MatHang mh = _quanLySieuThi.TimTheoMa(ma);
        if (mh.MaMatHang == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Không tìm thấy mặt hàng có mã '" + ma + "'.");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Bạn có chắc muốn xóa mặt hàng sau?");
        Console.WriteLine(mh.ToString());
        Console.Write("Nhập 'YES' để xác nhận: ");
        string xacNhan = Console.ReadLine();
        Console.ResetColor();

        if (xacNhan.ToUpper() == "YES")
        {
            if (_quanLySieuThi.XoaMatHang(ma))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Xóa thành công!");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Lỗi: Xóa thất bại.");
                Console.ResetColor();
            }
        }
        else
        {
            Console.WriteLine("Đã hủy thao tác xóa.");
        }
    }

    // <<< THAY ĐỔI >>> Toàn bộ hàm này đã được viết lại
    private static void LietKeToHop()
    {
        // Lấy N (kho)
        List<MatHang> danhSachDayDu = _quanLySieuThi.LayToanBoMatHang();
        int n_kho = danhSachDayDu.Count;

        Console.WriteLine("Hiện có " + n_kho + " mặt hàng (N_kho = " + n_kho + ").");
        if (n_kho == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Kho rỗng. Vui lòng dùng chức năng '8' để tạo dữ liệu trước.");
            Console.ResetColor();
            return;
        }

        // --- Bước 1: Hỏi n (tập con) ---
        Console.Write("Bạn muốn lấy bao nhiêu sản phẩm (n) từ kho để tạo combo? (n <= " + n_kho + "): ");
        int n_tapCon; // Đây là 'n' mới mà bạn muốn
        if (int.TryParse(Console.ReadLine(), out n_tapCon) == false || n_tapCon <= 0 || n_tapCon > n_kho)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Giá trị 'n' không hợp lệ (Vui lòng nhập n nhỏ hơn số sản phẩm trong kho đang chứa.");
            Console.ResetColor();
            return;
        }

        // --- Bước 2: Hỏi m (combo) ---
        Console.Write("Từ " + n_tapCon + " sản phẩm đó, bạn muốn một combo chứa bao nhiêu sản phẩm (m)? (m <= " + n_tapCon + "): ");
        int m_combo; // Đây là 'm'
        if (int.TryParse(Console.ReadLine(), out m_combo) == false || m_combo <= 0 || m_combo > n_tapCon)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Giá trị 'm' (combo) không hợp lệ.");
            Console.ResetColor();
            return;
        }

        // --- Bước 3: Cảnh báo ---
        if (n_tapCon > 20) // Luôn cảnh báo nếu n (tập con) > 20
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("CẢNH BÁO: Số lượng n (tập con = " + n_tapCon + ") là lớn.");
            Console.WriteLine("Việc liệt kê tổ hợp C(" + n_tapCon + ", " + m_combo + ") có thể mất nhiều thời gian.");
            Console.Write("Bạn có chắc muốn tiếp tục? (YES/NO): ");

            if (Console.ReadLine().ToUpper() != "YES")
            {
                Console.WriteLine("Đã hủy.");
                return;
            }
            Console.ResetColor();
        }

        // --- Bước 4: Tạo danh sách con ---
        // Chúng ta sẽ lấy n_tapCon sản phẩm đầu tiên từ kho
        List<MatHang> danhSachCon = new List<MatHang>();
        for (int i = 0; i < n_tapCon; i++)
        {
            danhSachCon.Add(danhSachDayDu[i]);
        }

        // --- Bước 5: Chạy thuật toán ---
        // Gọi hàm static, truyền vào DANH SÁCH CON và M
        ThuVienToHop.LietKeVaInToHop(danhSachCon, m_combo);
    }

    private static void InToanBo()
    {
        List<MatHang> danhSach = _quanLySieuThi.LayToanBoMatHang();
        Console.WriteLine("--- DANH SÁCH TOÀN BỘ " + danhSach.Count + " MẶT HÀNG ---");
        if (danhSach.Count == 0)
        {
            Console.WriteLine("(Kho rỗng)");
        }
        else
        {
            for (int i = 0; i < danhSach.Count; i++)
            {
                Console.WriteLine(danhSach[i].ToString());
            }
        }
        Console.WriteLine("--- Kết thúc danh sách ---");
    }
}

#endregion