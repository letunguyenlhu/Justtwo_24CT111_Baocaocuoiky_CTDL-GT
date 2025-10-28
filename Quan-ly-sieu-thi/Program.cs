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
        _khoHang = new Dictionary<string, MatHang>(StringComparer.OrdinalIgnoreCase);
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
/// Lớp 'static' (tĩnh) chứa thuật toán liệt kê tổ hợp C(n, m).
/// </summary>
public static class ThuVienToHop
{
    // Biến đếm (nằm ngoài) để theo dõi
    private static long _demToHop;

    /// <summary>
    /// (Public) Hàm chính, sẽ gọi hàm đệ quy và IN TRỰC TIẾP ra Console.
    /// </summary>
    public static void LietKeVaInToHop(List<MatHang> danhSachNguon, int m)
    {
        int n_tapCon = danhSachNguon.Count;

        if (danhSachNguon == null || m <= 0 || m > n_tapCon)
        {
            Console.WriteLine("Dữ liệu không hợp lệ để tạo tổ hợp.");
            return;
        }

        List<MatHang> comboHienTai = new List<MatHang>();
        _demToHop = 0;

        Console.WriteLine("--- Bắt đầu liệt kê combo C(" + n_tapCon + ", " + m + ") ---");

        LietKeRecursive(danhSachNguon, m, 0, comboHienTai);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("--- Hoàn thành. Tìm thấy " + _demToHop + " tổ hợp. ---");
        Console.ResetColor();
    }

    private static void LietKeRecursive(
        List<MatHang> danhSachNguon,
        int m,
        int indexBatDau,
        List<MatHang> comboHienTai)
    {
        // 1. Điều kiện dừng: Đã tìm đủ m phần tử
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

            return; // Dừng nhánh đệ quy này
        }

        // 2. Bước đệ quy
        for (int i = indexBatDau; i < danhSachNguon.Count; i++)
        {
            comboHienTai.Add(danhSachNguon[i]);
            LietKeRecursive(danhSachNguon, m, i + 1, comboHienTai);
            // 3. Quay lui (Backtrack)
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
        List<MatHang> danhSach = new List<MatHang>(soLuong);
        for (int i = 0; i < soLuong; i++)
        {
            try
            {
                string ma = "MH" + i.ToString("D8");
                string ten = TenTienTo[_random.Next(TenTienTo.Length)] + " " + TenHauTo[_random.Next(TenHauTo.Length)] + " #" + i;
                string dvt = DonVi[_random.Next(DonVi.Length)];
                decimal gia = _random.Next(5000, 500000);

                int tonKho;
               
                int coHoi = _random.Next(1, 11); 

                if (coHoi == 1) 
                {
                    tonKho = _random.Next(1, 10);
                }
                else 
                {
                 
                    tonKho = _random.Next(10, 1000);
                }

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
            int soLuongTrongKho = _quanLySieuThi.DemSoLuong();
            Console.WriteLine("Hiện có " + soLuongTrongKho + " mặt hàng (N_kho = " + soLuongTrongKho + ").");
            Console.WriteLine("1. Thêm mặt hàng mới ");
            Console.WriteLine("2. Tìm mặt hàng theo Mã ");
            Console.WriteLine("3. Tìm mặt hàng theo Tên");
            Console.WriteLine("4. Cập nhật mặt hàng (theo Mã)");
            Console.WriteLine("5. Xóa mặt hàng (theo Mã)");
            Console.WriteLine("6. Chức năng Combo (Tùy chọn)");
            Console.WriteLine("7. In toàn bộ danh sách");
            Console.WriteLine("8. Tạo dữ liệu ngẫu nhiên (test)");
            Console.WriteLine("9. Cảnh báo tồn kho thấp");
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
                    MenuChucNangCombo();
                    break;
                case "7":
                    InToanBo();
                    break;
                case "8":
                    TaoNgauNhienDuLieu();
                    break;
                case "9":
                    KiemTraTonKhoThap();
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
    /// Chuẩn hóa input của người dùng (ví dụ: "123") thành mã (ví dụ: "MH00000123")
    /// </summary>
    private static string ChuanHoaMa(string input)
    {
        int soThuTu;
        if (int.TryParse(input, out soThuTu))
        {
            return "MH" + soThuTu.ToString("D8");
        }
        else
        {
            return input;
        }
    }

    /// <summary>
    /// Chức năng 8: Tạo N dữ liệu ngẫu nhiên
    /// </summary>
    private static void TaoNgauNhienDuLieu()
    {
        Console.Write("Bạn muốn tạo mới bao nhiêu (N) mặt hàng? (Sẽ xóa kho cũ!): ");
        int n;

        if (int.TryParse(Console.ReadLine(), out n) == false || n <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Số lượng N không hợp lệ.");
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

    /// <summary>
    /// Chức năng 1: Thêm mặt hàng 
    /// </summary>
    private static void ThemMatHang()
    {
        Console.Write("Bạn muốn thêm bao nhiêu mặt hàng?: ");
        int soLuongCanThem;
        if (int.TryParse(Console.ReadLine(), out soLuongCanThem) == false || soLuongCanThem <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Số lượng không hợp lệ.");
            Console.ResetColor();
            return;
        }

        int soLuongThanhCong = 0; // Biến đếm

        // Bắt đầu vòng lặp for
        for (int i = 0; i < soLuongCanThem; i++)
        {
            Console.WriteLine("\n--- Đang thêm mặt hàng thứ " + (i + 1) + "/" + soLuongCanThem + " ---");

            // <<< THAY ĐỔI 1: Thêm vòng lặp while(true) bên trong for >>>
            // Vòng lặp này dùng để hỏi mã, nó chỉ thoát ra khi
            // 1. Nhập được mã hợp lệ
            // 2. Người dùng chọn "Bỏ qua"

            string ma; // Biến lưu mã
            bool daBoQua = false; // Cờ (flag) để biết người dùng có chọn bỏ qua không

            while (true) // Vòng lặp hỏi mã
            {
                Console.Write("Nhập mã mặt hàng (duy nhất): ");
                ma = Console.ReadLine();

                string maChuan = ChuanHoaMa(ma);
                MatHang mhTimChuan = _quanLySieuThi.TimTheoMa(maChuan);
                MatHang mhTimGoc = _quanLySieuThi.TimTheoMa(ma);

                if (mhTimChuan.MaMatHang == null && mhTimGoc.MaMatHang == null)
                {
                    // Mã hợp lệ, không trùng
                    break; // Thoát khỏi vòng lặp `while(true)` để hỏi tên, giá...
                }

                // Nếu mã bị trùng:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Lỗi: Mã '" + ma + "' (mã chuẩn '" + maChuan + "') đã tồn tại.");
                Console.ResetColor();
                Console.Write("Bạn muốn: [1] Nhập lại mã | [2] Bỏ qua mặt hàng này?: ");

                string luaChon = Console.ReadLine();
                if (luaChon == "2")
                {
                    daBoQua = true; 
                    break; 
                }

                // Nếu không chọn "2" (hoặc chọn "1", hoặc nhập lung tung)
                // thì vòng lặp 'while(true)' sẽ tự động lặp lại, hỏi lại mã.
                Console.WriteLine("...Vui lòng nhập lại mã...");
            }
            if (daBoQua == true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Đã bỏ qua mặt hàng thứ " + (i + 1) + ".");
                Console.ResetColor();
                continue; 
            }

            try
            {
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
                    Console.WriteLine("Thêm thành công: " + ten);
                    Console.ResetColor();
                    soLuongThanhCong++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Lỗi: Không thể thêm mặt hàng (lỗi không xác định).");
                    Console.ResetColor();
                    continue; // Bỏ qua
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Lỗi nhập liệu (Tên/Giá/SL): " + ex.Message);
                Console.WriteLine("Bỏ qua mặt hàng này.");
                Console.ResetColor();
                continue; // Bỏ qua
            }
        } // Kết thúc vòng lặp for

        // In kết quả
        if (soLuongThanhCong > 0)
        {
            Console.WriteLine("\nĐã thêm thành công " + soLuongThanhCong + " mặt hàng. Danh sách mặt hàng hiện có:");
            InToanBo();
        }
        else
        {
            Console.WriteLine("\nKhông có mặt hàng nào được thêm.");
        }
    }

    /// <summary>
    /// Chức năng 2: Tìm theo mã 
    /// </summary>
    private static void TimTheoMa()
    {
        Console.Write("Nhập mã mặt hàng (hoặc chỉ SỐ) cần tìm: ");
        string input = Console.ReadLine();

        string maCanTim = ChuanHoaMa(input);

        Console.WriteLine("...Đang tìm kiếm mã chuẩn: " + maCanTim);

        MatHang mhTimDuoc = _quanLySieuThi.TimTheoMa(maCanTim);

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
            Console.WriteLine("Không tìm thấy mặt hàng có mã '" + maCanTim + "'.");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Chức năng 3: Tìm theo tên
    /// </summary>
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

    /// <summary>
    /// Chức năng 4: Cập nhật
    /// </summary>
    private static void CapNhatMatHang()
    {
        Console.Write("Nhập mã mặt hàng (hoặc số) cần cập nhật: ");
        string input = Console.ReadLine();

        string maCanTim = ChuanHoaMa(input);

        MatHang mhCanSua = _quanLySieuThi.TimTheoMa(maCanTim);

        if (mhCanSua.MaMatHang == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Không tìm thấy mặt hàng có mã '" + maCanTim + "'.");
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

            // Dùng mã gốc (mhCanSua.MaMatHang) để cập nhật
            if (_quanLySieuThi.CapNhatMatHang(mhCanSua.MaMatHang, mhCanSua))
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

    /// <summary>
    /// Chức năng 5: Xóa
    /// </summary>
    private static void XoaMatHang()
    {
        Console.Write("Nhập mã mặt hàng (hoặc số) cần XÓA: ");
        string input = Console.ReadLine();

        string maCanTim = ChuanHoaMa(input);

        MatHang mh = _quanLySieuThi.TimTheoMa(maCanTim);
        if (mh.MaMatHang == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Không tìm thấy mặt hàng có mã '" + maCanTim + "'.");
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
            // Dùng mã gốc (mh.MaMatHang) để xóa
            if (_quanLySieuThi.XoaMatHang(mh.MaMatHang))
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

    /// <summary>
    /// Chức năng 9: Tồn kho thấp
    /// </summary>
    private static void KiemTraTonKhoThap()
    {
        Console.Write("Nhập mức tồn kho tối thiểu để cảnh báo (mặc định: 10): ");
        string input = Console.ReadLine();
        int mucTonKho;
        if (int.TryParse(input, out mucTonKho) == false || input == "")
        {
            mucTonKho = 10;
        }

        Console.WriteLine("...Đang tìm các mặt hàng có số lượng tồn <= " + mucTonKho);
        List<MatHang> danhSach = _quanLySieuThi.LayToanBoMatHang();
        int dem = 0;

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("--- CÁC MẶT HÀNG CÓ TỒN KHO THẤP ---");

        foreach (MatHang mh in danhSach)
        {
            if (mh.SoLuongTon <= mucTonKho)
            {
                Console.WriteLine(mh.ToString());
                dem = dem + 1;
            }
        }

        Console.ResetColor();
        if (dem == 0)
        {
            Console.WriteLine("Tốt! Không tìm thấy mặt hàng nào có tồn kho thấp.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- Tổng cộng có " + dem + " mặt hàng cần chú ý ---");
            Console.ResetColor();
        }
    }


    // <<< NHÓM HÀM CHO CHỨC NĂNG 6 >>>

    /// <summary>
    /// (Menu chính của Chức năng 6)
    /// </summary>
    private static void MenuChucNangCombo()
    {
        Console.WriteLine("--- CHỨC NĂNG COMBO ---");
        Console.WriteLine("1. Tạo Combo thủ công (Bạn tự chọn sản phẩm)");
        Console.WriteLine("2. Tìm Combo theo mức giá (Ngẫu nhiên)");
        Console.WriteLine("3. Tạo combo tự động (Liệt kê TẤT CẢ tổ hợp có thể với số sản phẩm bạn chọn)");
        Console.WriteLine("0. Quay lại menu chính");
        Console.Write("Vui lòng chọn: ");

        string luaChon = Console.ReadLine();

        switch (luaChon)
        {
            case "1":
                TaoComboThuCong();
                break;
            case "2":
                TimComboTheoGia();
                break;
            case "3":
                LietKeToHopNangCao();
                break;
            case "0":
                break;
            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Lựa chọn không hợp lệ.");
                Console.ResetColor();
                break;
        }
    }

    /// <summary>
    /// (Logic 6.1) Người dùng tự xây dựng combo 
    /// </summary>
    private static void TaoComboThuCong()
    {
        List<MatHang> comboHienTai = new List<MatHang>();
        decimal tongGia = 0;

        Console.WriteLine("--- BẮT ĐẦU XÂY DỰNG COMBO ---");
        Console.WriteLine("Nhập mã SP, SỐ, hoặc TÊN (hoặc 'xong' để kết thúc).");

        while (true) // Vòng lặp thêm sản phẩm
        {
            Console.Write("Nhập SP [" + comboHienTai.Count + "] (hoặc 'xong'): ");
            string input = Console.ReadLine();

            if (input.ToUpper() == "XONG")
            {
                break; // Thoát (Không hỏi lại)
            }

            // --- Logic tìm kiếm ---
            MatHang mhTimDuoc = new MatHang();
            bool daTimThay = false;

            string maCanTim = ChuanHoaMa(input);
            MatHang mhTimBangMa = _quanLySieuThi.TimTheoMa(maCanTim);

            if (mhTimBangMa.MaMatHang != null)
            {
                mhTimDuoc = mhTimBangMa;
                daTimThay = true;
            }
            else
            {
                List<MatHang> ketQuaTimTen = _quanLySieuThi.TimTheoTen(input);

                if (ketQuaTimTen.Count == 1)
                {
                    mhTimDuoc = ketQuaTimTen[0];
                    daTimThay = true;
                }
                else if (ketQuaTimTen.Count > 1)
                {
                    // In ra danh sách bị trùng tên
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  Tìm thấy " + ketQuaTimTen.Count + " SP chứa tên '" + input + "'. Vui lòng dùng MÃ (hoặc số) để chọn:");
                    foreach (MatHang mh_trung in ketQuaTimTen)
                    {
                        Console.WriteLine("    -> " + mh_trung.ToString());
                    }
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  Lỗi: Không tìm thấy sản phẩm với mã HOẶC tên '" + input + "'");
                    Console.ResetColor();
                }
            }
            // --- Kết thúc logic tìm kiếm ---

            if (daTimThay == true)
            {
                // Kiểm tra xem đã có trong combo chưa
                bool daCoTrongCombo = false;
                foreach (MatHang mh_co in comboHienTai)
                {
                    if (mh_co.MaMatHang == mhTimDuoc.MaMatHang)
                    {
                        daCoTrongCombo = true;
                        break;
                    }
                }

                if (daCoTrongCombo)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  Sản phẩm này đã có trong combo.");
                    Console.ResetColor();
                }
                else
                {
                    comboHienTai.Add(mhTimDuoc);
                    tongGia = tongGia + mhTimDuoc.DonGia;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("  Đã thêm: " + mhTimDuoc.Ten);
                    Console.WriteLine("  Tổng tạm tính: " + tongGia.ToString("N0") + " VNĐ");
                    Console.ResetColor();
                }
            }
        } // Kết thúc vòng lặp while

        // In kết quả
        Console.WriteLine("\n--- COMBO CỦA BẠN ĐÃ HOÀN TẤT ---");
        if (comboHienTai.Count > 0)
        {
            foreach (MatHang mh in comboHienTai)
            {
                Console.WriteLine(mh.ToString());
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("--- TỔNG CỘNG: " + tongGia.ToString("N0") + " VNĐ ---");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine("(Bạn chưa chọn sản phẩm nào)");
        }
    }

    /// <summary>
    /// (Logic 6.2) Tìm combo ngẫu nhiên theo mức giá
    /// </summary>
    private static void TimComboTheoGia()
    {
        try
        {
            Console.Write("Nhập mức giá bạn muốn (ví dụ: 500000): ");
            decimal mucGia = decimal.Parse(Console.ReadLine());

            Console.Write("Cho phép chênh lệch (dưới mức giá) (ví dụ: 50000): ");
            decimal chenhLech = decimal.Parse(Console.ReadLine());

            decimal giaMax = mucGia;
            decimal giaMin = mucGia - chenhLech;

            List<MatHang> khoHang = _quanLySieuThi.LayToanBoMatHang();
            if (khoHang.Count == 0)
            {
                Console.WriteLine("Kho rỗng, không thể tìm.");
                return;
            }

            // --- Xáo trộn (Shuffle) kho hàng ---
            Random rng = new Random();
            int n = khoHang.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                MatHang temp = khoHang[k];
                khoHang[k] = khoHang[n];
                khoHang[n] = temp;
            }
            // --- Kết thúc xáo trộn ---

            List<MatHang> comboTimDuoc = new List<MatHang>();
            decimal tongGia = 0;

            foreach (MatHang mh in khoHang)
            {
                if (tongGia + mh.DonGia <= giaMax)
                {
                    comboTimDuoc.Add(mh);
                    tongGia = tongGia + mh.DonGia;
                }
            }

            if (tongGia >= giaMin)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n--- ĐÃ TÌM THẤY 1 COMBO PHÙ HỢP ---");
                foreach (MatHang mh in comboTimDuoc)
                {
                    Console.WriteLine(mh.ToString());
                }
                Console.WriteLine("--- TỔNG CỘNG: " + tongGia.ToString("N0") + " VNĐ ---");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nKhông tìm thấy combo phù hợp sau 1 lần thử ngẫu nhiên.");
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

    /// <summary>
    /// (Logic 6.3) Liệt kê TẤT CẢ tổ hợp 
    /// </summary>
    private static void LietKeToHopNangCao()
    {
        int n_kho = _quanLySieuThi.DemSoLuong();
        if (n_kho == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Kho rỗng. Vui lòng dùng chức năng '8' để tạo dữ liệu trước.");
            Console.ResetColor();
            return;
        }

        // --- Bước 1: Hỏi mục tiêu (n) ---
        List<MatHang> danhSachCon = new List<MatHang>();

        Console.WriteLine("--- Bước 1: Xây dựng tập sản phẩm (n) ---");
        Console.Write("Bạn muốn chọn bao nhiêu sản phẩm để tạo combo (n)? (Bỏ trống để không giới hạn): ");
        string input_n = Console.ReadLine();
        int n_mucTieu; // Đây là số 'n' MỤC TIÊU
        if (int.TryParse(input_n, out n_mucTieu) == false || n_mucTieu <= 0)
        {
            n_mucTieu = 0; // 0 nghĩa là không giới hạn
        }

        Console.WriteLine("Nhập mã SP, SỐ, hoặc TÊN để thêm vào tập (n).");
        Console.WriteLine("Nhập 'xong' để tiếp tục.");


        while (true) // Vòng lặp xây dựng tập n
        {
            // Hiển thị tiến độ (ví dụ: [3/5])
            string thongBaoTienDo;
            if (n_mucTieu > 0)
            {
                thongBaoTienDo = "SP [" + danhSachCon.Count + "/" + n_mucTieu + "]";
            }
            else
            {
                thongBaoTienDo = "SP [" + danhSachCon.Count + "]";
            }

            Console.Write("Nhập " + thongBaoTienDo + " (hoặc 'xong'): ");
            string input = Console.ReadLine();

            // --- Xử lý thoát ---
            if (input.ToUpper() == "XONG")
            {
                // Hỏi xác nhận nếu chưa đủ số lượng n
                if (n_mucTieu > 0 && danhSachCon.Count < n_mucTieu)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Bạn mới chọn " + danhSachCon.Count + "/" + n_mucTieu + " SP. Bạn có chắc muốn dùng " + danhSachCon.Count + " SP này? (YES/NO): ");
                    Console.ResetColor();
                    if (Console.ReadLine().ToUpper() != "YES")
                    {
                        Console.WriteLine("...Vui lòng nhập thêm...");
                        continue; // Tiếp tục vòng lặp, không thoát
                    }
                }

                break; // Thoát khỏi vòng lặp
            }

            // --- Logic tìm kiếm ---
            MatHang mhTimDuoc = new MatHang();
            bool daTimThay = false;

            string maCanTim = ChuanHoaMa(input);
            MatHang mhTimBangMa = _quanLySieuThi.TimTheoMa(maCanTim);

            if (mhTimBangMa.MaMatHang != null)
            {
                mhTimDuoc = mhTimBangMa;
                daTimThay = true;
            }
            else
            {
                List<MatHang> ketQuaTimTen = _quanLySieuThi.TimTheoTen(input);

                if (ketQuaTimTen.Count == 1)
                {
                    mhTimDuoc = ketQuaTimTen[0];
                    daTimThay = true;
                }
                else if (ketQuaTimTen.Count > 1)
                {
                    // In ra danh sách bị trùng tên
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  Tìm thấy " + ketQuaTimTen.Count + " SP chứa tên '" + input + "'. Vui lòng dùng MÃ (hoặc số) để chọn:");
                    foreach (MatHang mh_trung in ketQuaTimTen)
                    {
                        Console.WriteLine("    -> " + mh_trung.ToString());
                    }
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  Lỗi: Không tìm thấy sản phẩm với mã HOẶC tên '" + input + "'");
                    Console.ResetColor();
                }
            }
            // --- Kết thúc logic tìm kiếm ---

            if (daTimThay == true)
            {
                // Kiểm tra xem đã thêm chưa
                bool daThem = false;
                foreach (MatHang mh_co in danhSachCon)
                {
                    if (mh_co.MaMatHang == mhTimDuoc.MaMatHang)
                    {
                        daThem = true;
                        break;
                    }
                }

                if (daThem == false)
                {
                    danhSachCon.Add(mhTimDuoc);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("  Đã thêm: " + mhTimDuoc.Ten);
                    Console.ResetColor();

                    // Nếu đã đủ số lượng n mục tiêu, tự động thoát
                    if (n_mucTieu > 0 && danhSachCon.Count == n_mucTieu)
                    {
                        Console.WriteLine("...Bạn đã chọn đủ " + n_mucTieu + " sản phẩm...");
                        break;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  Sản phẩm này đã có trong danh sách (n).");
                    Console.ResetColor();
                }
            }
        } // Kết thúc vòng lặp while

        // --- Bước 2: Nhập m (combo) ---
        int n_thucTe = danhSachCon.Count; // n thực tế
        if (n_thucTe == 0)
        {
            Console.WriteLine("Bạn chưa chọn sản phẩm nào (n=0). Đã hủy.");
            return;
        }

        Console.WriteLine("\n--- Bước 2: Chọn số lượng sản phẩm trong từng combo (m) ---");
        int m_combo;

        while (true) // Vòng lặp hỏi m
        {
            Console.Write("Bạn đã chọn " + n_thucTe + " SP. Bạn muốn mỗi combo chứa bao nhiêu sản phẩm (m)? (m <= " + n_thucTe + "): ");
            if (int.TryParse(Console.ReadLine(), out m_combo) == true && m_combo > 0 && m_combo <= n_thucTe)
            {
                break; // Nhập đúng -> thoát vòng lặp
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Giá trị 'm' không hợp lệ. Vui lòng nhập m > 0 và m <= n.");
                Console.ResetColor();
            }
        }

        // --- Bước 3: Cảnh báo ---
        if (n_thucTe > 20)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("CẢNH BÁO: Số lượng n (số lượng sản phẩm tạo combo = " + n_thucTe + ") là lớn.");
            Console.WriteLine("Việc liệt kê tổ hợp C(" + n_thucTe + ", " + m_combo + ") có thể mất nhiều thời gian.");
            Console.Write("Bạn có chắc muốn tiếp tục? (YES/NO): ");

            if (Console.ReadLine().ToUpper() != "YES")
            {
                Console.WriteLine("Đã hủy.");
                return;
            }
            Console.ResetColor();
        }

        // --- Bước 4: Chạy thuật toán ---
        ThuVienToHop.LietKeVaInToHop(danhSachCon, m_combo);
    }


    /// <summary>
    /// Chức năng 7: In toàn bộ
    /// </summary>
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