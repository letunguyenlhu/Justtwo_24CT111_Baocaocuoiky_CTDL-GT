using System;
using System.Collections.Generic; // Cho Dictionary, List
using System.Text;            // Cho Console.OutputEncoding
using System.Globalization; // Cho việc xử lý Ngày/Tháng (DateTime)

// ===== Bắt đầu Định nghĩa các Kiểu Dữ liệu và Class =====

#region 1. Kiểu Dữ liệu 

/// <summary>
/// Định nghĩa kiểu Mặt Hàng bằng 'struct'.
/// </summary>
public struct MatHang
{
    public string MaMatHang;
    public string Ten;
    public string DonViTinh;
    public decimal DonGia;
    public int SoLuongTon;
    public DateTime HanSuDung; 

    /// <summary>
    /// Constructor (hàm khởi tạo) để gán giá trị và kiểm tra (validate)
    /// </summary>
    public MatHang(string ma, string ten, string dvt, decimal gia, int sl, DateTime hsd) 
    {
        if (string.IsNullOrWhiteSpace(ma))
            throw new Exception("Lỗi: Mã mặt hàng không được để trống.");
        if (string.IsNullOrWhiteSpace(ten))
            throw new Exception("Lỗi: Tên mặt hàng không được để trống.");
        if (gia < 0)
            throw new Exception("Lỗi: Đơn giá không được âm.");
        if (sl < 0)
            throw new Exception("Lỗi: Số lượng tồn không được âm.");
        if (hsd.Year < 2000) 
            throw new Exception("Lỗi: Hạn sử dụng không hợp lệ.");

        this.MaMatHang = ma;
        this.Ten = ten;
        this.DonViTinh = dvt;
        this.DonGia = gia;
        this.SoLuongTon = sl;
        this.HanSuDung = hsd; 
    }

    public override string ToString()
    {
        string hsdString = HanSuDung.ToString("dd/MM/yyyy");

        return "[Mã: " + MaMatHang + "] - Tên: " + Ten +
               " (ĐVT: " + DonViTinh + ") - Giá: " + DonGia +
               " VNĐ - Tồn: " + SoLuongTon + " - HSD: " + hsdString; 
    }
}

#endregion

#region 2. Lớp Nghiệp vụ (Service/Logic)

/// <summary>
/// Lớp 'QuanLySieuThi' (dùng class) để quản lý kho hàng.
/// <<< Sử dụng StringComparer để không phân biệt hoa/thường >>>
/// </summary>
public class QuanLySieuThi
{
    private Dictionary<string, MatHang> _khoHang;
    public QuanLySieuThi()
    {
        _khoHang = new Dictionary<string, MatHang>(StringComparer.OrdinalIgnoreCase);
    }
    public bool ThemMatHang(MatHang mh)
    {
        if (_khoHang.ContainsKey(mh.MaMatHang))
        {
            return false;
        }
        else
        {
            _khoHang.Add(mh.MaMatHang, mh);
            return true;
        }
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
        if (_khoHang.TryGetValue(ma, out MatHang mh))
        {
            return mh; 
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
public static class ThuVienToHop
{
    private static long _demToHop;

    /// <summary>
    /// (Public) Hàm chính, sẽ gọi hàm đệ quy và IN TRỰC TIẾP ra Console.
    /// </summary>
    public static void LietKeVaInToHop(List<MatHang> danhSachNguon, int m)
    {
        int n_tapCon = danhSachNguon.Count;

        if (danhSachNguon == null || m <= 0 || m > n_tapCon)
        {
            Console.WriteLine("Dữ liệu không hợp lệ để tính tổ hợp.");
            return;
        }

        List<MatHang> comboHienTai = new List<MatHang>();
        _demToHop = 0;

        Console.WriteLine("--- Bắt đầu liệt kê C(" + n_tapCon + ", " + m + ") ---");

        LietKeRecursive_Public(danhSachNguon, m, 0, comboHienTai);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("--- Hoàn thành. Tìm thấy " + _demToHop + " tổ hợp. ---");
        Console.ResetColor();
    }

    /// <summary>
    /// Hàm đệ quy, dùng để tìm tất cả tổ hợp.
    /// </summary>
    public static void LietKeRecursive_Public(
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
            LietKeRecursive_Public(danhSachNguon, m, i + 1, comboHienTai);
            comboHienTai.RemoveAt(comboHienTai.Count - 1);
        }
    }
}

#endregion

#region 4. Lớp Tạo Dữ liệu Mẫu (Utility)

/// <summary>
/// Lớp để tạo dữ liệu mẫu ngẫu nhiên.
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
                    tonKho = _random.Next(1, 10); // 10% tồn kho thấp
                }
                else
                {
                    tonKho = _random.Next(10, 1000); // 90% tồn kho bình thường
                }

                // Thêm 10% cơ hội hàng đã hết hạn 
                DateTime hsd;
                if (coHoi == 2) // 10% cơ hội hết hạn
                {
                    hsd = DateTime.Now.AddDays(_random.Next(-30, 0)); // Đã hết hạn 30 ngày
                }
                else // 90% cơ hội còn hạn
                {
                    hsd = DateTime.Now.AddDays(_random.Next(1, 365)); // Hết hạn trong 1 năm tới
                }

                danhSach.Add(new MatHang(ma, ten, dvt, gia, tonKho, hsd)); 
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
/// Lớp 'Program' chứa hàm 'Main' 
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

        // <<< Set culture để xử lý ngày tháng dễ hơn >>>
        // Giúp C# hiểu ngày "25/10/2025"
        CultureInfo ci = new CultureInfo("vi-VN");
        System.Threading.Thread.CurrentThread.CurrentCulture = ci;
        System.Threading.Thread.CurrentThread.CurrentUICulture = ci;


        Console.WriteLine("Chào mừng bạn đến với chương trình Quản lý Siêu thị!");
        Console.WriteLine("Kho hàng hiện đang rỗng. Hãy dùng chức năng '8' để tạo dữ liệu.");

        bool dangChay = true;
        while (dangChay)
        {
            Console.WriteLine("");
            Console.WriteLine("===== MENU QUẢN LÝ SIÊU THỊ ABC =====");
            int soLuongTrongKho = _quanLySieuThi.DemSoLuong();
            Console.WriteLine("Hiện có " + soLuongTrongKho + " mặt hàng (N_kho = " + soLuongTrongKho + ").");
            Console.WriteLine("1. Thêm mặt hàng mới");
            Console.WriteLine("2. Tìm mặt hàng theo Mã (hoặc số)");
            Console.WriteLine("3. Tìm mặt hàng theo Tên");
            Console.WriteLine("4. Cập nhật mặt hàng (theo Mã)");
            Console.WriteLine("5. Xóa mặt hàng (theo Mã)");
            Console.WriteLine("6. Tạo Combo");
            Console.WriteLine("7. In toàn bộ danh sách");
            Console.WriteLine("8. Tạo dữ liệu ngẫu nhiên (test)");
            Console.WriteLine("9. Cảnh báo tồn kho thấp");
            Console.WriteLine("10. Thanh toán Giỏ hàng");
            Console.WriteLine("11. Báo cáo & Thống kê ");
            Console.WriteLine("12. Kiểm tra Hàng sắp hết hạn ");
            Console.WriteLine("0. Thoát");
            Console.Write("Vui lòng chọn chức năng: ");

            string luaChon = Console.ReadLine();
            Console.Clear();

            switch (luaChon)
            {
                case "1": ThemMatHang(); break;
                case "2": TimTheoMa(); break;
                case "3": TimTheoTen(); break;
                case "4": CapNhatMatHang(); break;
                case "5": XoaMatHang(); break;
                case "6": MenuChucNangCombo(); break;
                case "7": InToanBo(); break;
                case "8": TaoNgauNhienDuLieu(); break;
                case "9": KiemTraTonKhoThap(); break;
                case "10": ThanhToanGioHang(); break;
                case "11": BaoCaoThongKe(); break;
                case "12": KiemTraHanSuDung(); break;
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
//Hàm xử lý Menu
    public static string ChuanHoaMa(string input)
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
    public static void TaoNgauNhienDuLieu()
    {
        Console.Write("Bạn muốn tạo mới bao nhiêu mặt hàng? (Sẽ xóa kho cũ!): ");
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
    /// Chức năng 1: Thêm thủ công 
    public static void ThemMatHang()
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

            string ma; // Biến lưu mã NHẬP VÀO
            string maChuan; // Biến lưu mã ĐÃ CHUẨN HÓA
            bool daBoQua = false;

            while (true) // Vòng lặp hỏi mã
            {
                Console.Write("Nhập mã mặt hàng (hoặc chỉ SỐ): ");
                ma = Console.ReadLine();

                maChuan = ChuanHoaMa(ma);
                Console.WriteLine("-> Mã chuẩn sẽ là: " + maChuan); // Thông báo cho người dùng

                // Kiểm tra mã chuẩn có tồn tại không
                MatHang mhTimChuan = _quanLySieuThi.TimTheoMa(maChuan);

                if (mhTimChuan.MaMatHang == null)
                {
                    // Mã hợp lệ, không trùng
                    ma = maChuan; 
                    break; // Thoát khỏi vòng lặp `while(true)`
                }

                // Nếu mã bị trùng:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Lỗi: Mã chuẩn '" + maChuan + "' đã tồn tại.");
                Console.ResetColor();
                Console.Write("Bạn muốn: [1] Nhập lại mã | [2] Bỏ qua mặt hàng này?: ");

                string luaChon = Console.ReadLine();
                if (luaChon == "2")
                {
                    daBoQua = true;
                    break;
                }
                Console.WriteLine("...Vui lòng nhập lại mã...");
            } // Kết thúc vòng lặp while(true) hỏi mã

            if (daBoQua == true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Đã bỏ qua mặt hàng thứ " + (i + 1) + ".");
                Console.ResetColor();
                continue; // Bỏ qua phần còn lại của vòng `for`, chuyển sang (i+1)
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
                DateTime hsd;
                while (true)
                {
                    Console.Write("Nhập Hạn Sử Dụng (ví dụ: 31/12/2025): ");
                    string hsdString = Console.ReadLine();
                    if (DateTime.TryParseExact(hsdString, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out hsd))
                    {
                        break;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Lỗi: Định dạng ngày không đúng. Vui lòng nhập theo dạng Ngày/Tháng/Năm.");
                        Console.ResetColor();
                    }
                }

                MatHang mh = new MatHang(ma, ten, dvt, gia, sl, hsd);

                if (_quanLySieuThi.ThemMatHang(mh))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Thêm thành công: " + ten + " (Mã: " + ma + ")"); 
                    Console.ResetColor();
                    soLuongThanhCong++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Lỗi: Không thể thêm mặt hàng (lỗi không xác định).");
                    Console.ResetColor();
                    continue;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Lỗi nhập liệu (Tên/Giá/SL/HSD): " + ex.Message);
                Console.WriteLine("Bỏ qua mặt hàng này.");
                Console.ResetColor();
                continue;
            }
        } // Kết thúc vòng lặp for

        // In kết quả
        if (soLuongThanhCong > 0)
        {
            Console.WriteLine("\nĐã thêm thành công " + soLuongThanhCong + " mặt hàng. In lại danh sách:");
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
    public static void TimTheoMa()
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
    public static void TimTheoTen()
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
    public static void CapNhatMatHang()
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

            Console.Write("HSD mới (cũ: " + mhCanSua.HanSuDung.ToString("dd/MM/yyyy") + ") (Nhập dd/MM/yyyy): ");
            string hsdString = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(hsdString) == false)
            {
                DateTime hsdMoi;
                if (DateTime.TryParseExact(hsdString, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out hsdMoi))
                {
                    mhCanSua.HanSuDung = hsdMoi;
                }
                else
                {
                    Console.WriteLine("...Định dạng ngày sai, giữ HSD cũ...");
                }
            }

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
    public static void XoaMatHang()
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
    public static void KiemTraTonKhoThap()
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


    // <<< NHÓM HÀM CHO CHỨC NĂNG 6 (COMBO) >>>

    /// <summary>
    /// (Menu chính của Chức năng 6)
    /// </summary>
    public static void MenuChucNangCombo()
    {
        Console.WriteLine("--- CHỨC NĂNG COMBO ---");
        Console.WriteLine("1. Tạo Combo thủ công");
        Console.WriteLine("2. Tìm Combo theo mức giá");
        Console.WriteLine("3. Liệt kê TẤT CẢ tổ hợp với mặt hàng bạn chọn");
        Console.WriteLine("0. Quay lại menu chính");
        Console.Write("Vui lòng chọn: ");

        string luaChon = Console.ReadLine();

        switch (luaChon)
        {
            case "1": TaoComboThuCong(); break;
            case "2": TimComboTheoGia(); break;
            case "3": LietKeToHopNangCao(); break;
            case "0": break;
            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Lựa chọn không hợp lệ.");
                Console.ResetColor();
                break;
        }
    }
    /// <summary>
    ///  6.1 Người dùng tự xây dựng combo 
    /// </summary>
    public static void TaoComboThuCong() 
    {
        List<MatHang> comboHienTai = new List<MatHang>();
        decimal tongGia = 0;

        Console.WriteLine("--- BẮT ĐẦU XÂY DỰNG COMBO (Yêu cầu >= 2 SP) ---");
        Console.WriteLine("Nhập mã SP, SỐ, hoặc TÊN (hoặc 'xong' để kết thúc).");

        while (true) 
        {
            Console.Write("Nhập SP [" + comboHienTai.Count + "] (hoặc 'xong'): ");
            string input = Console.ReadLine();

            if (input.ToUpper() == "XONG")
            {
                break; 
            }
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

            if (daTimThay == true)
            {
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
        } 

        Console.WriteLine("\n--- COMBO CỦA BẠN ĐÃ HOÀN TẤT ---");
        if (comboHienTai.Count >= 2) 
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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Lỗi: Combo phải có ít nhất 2 sản phẩm.");
            Console.WriteLine("Đã hủy combo (bạn có " + comboHienTai.Count + " sản phẩm).");
            Console.ResetColor();
        }
    }

    
    /// <summary>
    /// 6.2 Tìm combo ngẫu nhiên theo mức giá
    /// </summary>
    public static void TimComboTheoGia()
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
            if (khoHang.Count < 2) 
            {
                Console.WriteLine("Kho không đủ sản phẩm (dưới 2) để tạo combo.");
                return;
            }
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
            if (tongGia >= giaMin && comboTimDuoc.Count >= 2)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n--- ĐÃ TÌM THẤY 1 COMBO PHÙ HỢP (>= 2 SP) ---");
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
                Console.WriteLine("\nKhông tìm thấy combo phù hợp với giá đã chọn trong danh sách sản phẩm hiện tại.");
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
    /// 6.3 Liệt kê TẤT CẢ tổ hợp có thể C(n, m) 
    /// </summary>
    public static void LietKeToHopNangCao() 
    {
        int n_kho = _quanLySieuThi.DemSoLuong();
        if (n_kho == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Kho rỗng. Vui lòng dùng chức năng '8' để tạo dữ liệu trước.");
            Console.ResetColor();
            return;
        }

      
        List<MatHang> danhSachCon = new List<MatHang>();
        Console.WriteLine("--- Bước 1: Xây dựng tập sản phẩm (n) ---");
        Console.Write("Bạn muốn chọn bao nhiêu sản phẩm trong danh sách (n) để tạo combo? (Bỏ trống để không giới hạn): ");
        string input_n = Console.ReadLine();
        int n_mucTieu;
        if (int.TryParse(input_n, out n_mucTieu) == false || n_mucTieu <= 0)
        {
            n_mucTieu = 0;
        }

        Console.WriteLine("Nhập mã SP, SỐ, hoặc TÊN để thêm vào danh sách (n).");
        Console.WriteLine("Nhập 'xong' để tiếp tục.");

        while (true) 
        {
            string thongBaoTienDo;
            int soHienTai = danhSachCon.Count + 1; 

            if (n_mucTieu > 0)
            {
                if (danhSachCon.Count >= n_mucTieu)
                {
                    Console.WriteLine("...Bạn đã chọn đủ " + n_mucTieu + " sản phẩm...");
                    break; 
                }

                thongBaoTienDo = "SP [" + soHienTai + "/" + n_mucTieu + "]";
            }
            else
            {
                thongBaoTienDo = "SP [" + soHienTai + "]";
            }

            Console.Write("Nhập " + thongBaoTienDo + " (hoặc 'xong'): ");
            string input = Console.ReadLine();

            if (input.ToUpper() == "XONG")
            {
                if (n_mucTieu > 0 && danhSachCon.Count < n_mucTieu)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Bạn mới chọn " + danhSachCon.Count + "/" + n_mucTieu + " SP. Bạn có chắc muốn dùng " + danhSachCon.Count + " SP này? (YES/NO): ");
                    Console.ResetColor();
                    if (Console.ReadLine().ToUpper() != "YES")
                    {
                        Console.WriteLine("...Vui lòng nhập thêm...");
                        continue;
                    }
                }
                break;
            }

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

            if (daTimThay == true)
            {
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
        } 
        int n_thucTe = danhSachCon.Count;
        if (n_thucTe < 2) 
        {
            Console.WriteLine("Bạn đã chọn " + n_thucTe + " sản phẩm. Không đủ để tạo combo (yêu cầu >= 2). Đã hủy.");
            return;
        }

        Console.WriteLine("\n--- Bước 2: Chọn số lượng (m) ---");
        int m_combo;

        while (true) 
        {
            Console.Write("Bạn đã chọn " + n_thucTe + " SP. Bạn muốn mỗi combo chứa bao nhiêu sản phẩm (m)? (2 <= m <= " + n_thucTe + "): ");

            if (int.TryParse(Console.ReadLine(), out m_combo) == true && m_combo >= 2 && m_combo <= n_thucTe)
            {
                break; 
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Giá trị 'm' không hợp lệ. Vui lòng nhập (2 <= m <= n).");
                Console.ResetColor();
            }
        }

        if (n_thucTe > 20)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("CẢNH BÁO: Số lượng n (danh sách sản phẩm tạo Combo = " + n_thucTe + ") là lớn.");
            Console.WriteLine("Việc liệt kê tổ hợp C(" + n_thucTe + ", " + m_combo + ") có thể mất nhiều thời gian.");
            Console.Write("Bạn có chắc muốn tiếp tục? (YES/NO): ");

            if (Console.ReadLine().ToUpper() != "YES")
            {
                Console.WriteLine("Đã hủy.");
                return;
            }
            Console.ResetColor();
        }
        ThuVienToHop.LietKeVaInToHop(danhSachCon, m_combo);
    }

    /// <summary>
    /// Chức năng 7: In toàn bộ
    /// </summary>
    public static void InToanBo()
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

    /// <summary>
    /// Chức năng 10: Thanh toán Giỏ hàng 
    /// </summary>
    public static void ThanhToanGioHang()
    {
        // 1. List các sản phẩm (struct) trong giỏ
        List<MatHang> gioHang = new List<MatHang>();
        // 2. List số lượng mua (vì struct MatHang chỉ lưu TỒN KHO)
        List<int> soLuongMuaList = new List<int>();

        decimal tongTien = 0;

        Console.WriteLine("--- BẮT ĐẦU THANH TOÁN ---");
        Console.WriteLine("Nhập mã SP, SỐ, hoặc TÊN (hoặc 'xong' để thanh toán).");

        while (true)
        {
            Console.Write("\nNhập SP (hoặc 'xong'): ");
            string input = Console.ReadLine();

            if (input.ToUpper() == "XONG")
            {
                if (gioHang.Count == 0)
                {
                    Console.WriteLine("Giỏ hàng rỗng. Đã hủy thanh toán.");
                    return;
                }
                break; // Thoát để tính tiền
            }

            // --- 1. Tìm sản phẩm ---
            MatHang mhTrongKho = new MatHang();
            bool daTimThay = false;

            string maCanTim = ChuanHoaMa(input);
            MatHang mhTimBangMa = _quanLySieuThi.TimTheoMa(maCanTim);

            if (mhTimBangMa.MaMatHang != null)
            {
                mhTrongKho = mhTimBangMa;
                daTimThay = true;
            }
            else
            {
                List<MatHang> ketQuaTimTen = _quanLySieuThi.TimTheoTen(input);
                if (ketQuaTimTen.Count == 1)
                {
                    mhTrongKho = ketQuaTimTen[0];
                    daTimThay = true;
                }
                else if (ketQuaTimTen.Count > 1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  Tìm thấy nhiều SP. Vui lòng dùng MÃ (hoặc số) để chọn.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  Lỗi: Không tìm thấy sản phẩm này.");
                    Console.ResetColor();
                }
            }

            if (daTimThay == false)
            {
                continue; // Quay lại vòng lặp, hỏi SP tiếp
            }

            // --- 2. Hỏi số lượng ---
            Console.WriteLine("  Đã tìm thấy: " + mhTrongKho.Ten + " (Tồn: " + mhTrongKho.SoLuongTon + ")");
            if (mhTrongKho.SoLuongTon == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Sản phẩm này đã hết hàng!");
                Console.ResetColor();
                continue;
            }

            int soLuongMua;
            while (true)
            {
                Console.Write("  Nhập số lượng mua: ");
                if (int.TryParse(Console.ReadLine(), out soLuongMua) && soLuongMua > 0)
                {
                    if (soLuongMua <= mhTrongKho.SoLuongTon)
                    {
                        break; // Số lượng hợp lệ
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("  Lỗi: Kho chỉ còn " + mhTrongKho.SoLuongTon + ". Vui lòng nhập ít hơn.");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  Lỗi: Số lượng không hợp lệ.");
                    Console.ResetColor();
                }
            }

            // --- 3. Thêm vào giỏ hàng ---
            gioHang.Add(mhTrongKho);
            soLuongMuaList.Add(soLuongMua);
            tongTien = tongTien + (mhTrongKho.DonGia * soLuongMua);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  Đã thêm " + soLuongMua + " " + mhTrongKho.DonViTinh + " " + mhTrongKho.Ten);
            Console.WriteLine("  Tổng tạm tính: " + tongTien.ToString("N0") + " VNĐ");
            Console.ResetColor();
        }

        // --- 4. In Hóa Đơn ---
        Console.Clear();
        Console.WriteLine("--- HÓA ĐƠN THANH TOÁN ---");
        for (int i = 0; i < gioHang.Count; i++)
        {
            MatHang mh = gioHang[i];
            int sl = soLuongMuaList[i];
            decimal thanhTien = mh.DonGia * sl;
            Console.WriteLine((i + 1) + ". " + mh.Ten + " (SL: " + sl + " x " + mh.DonGia.ToString("N0") + ") = " + thanhTien.ToString("N0") + " VNĐ");
        }
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("------------------------------");
        Console.WriteLine("--- TỔNG CỘNG: " + tongTien.ToString("N0") + " VNĐ ---");
        Console.ResetColor();

        // --- 5. Cập nhật Kho  ---
        for (int i = 0; i < gioHang.Count; i++)
        {
            MatHang mh_da_mua = gioHang[i];
            int sl_mua = soLuongMuaList[i];

            MatHang mh_trong_kho = _quanLySieuThi.TimTheoMa(mh_da_mua.MaMatHang);

            mh_trong_kho.SoLuongTon = mh_trong_kho.SoLuongTon - sl_mua;

            _quanLySieuThi.CapNhatMatHang(mh_trong_kho.MaMatHang, mh_trong_kho);
        }

        Console.WriteLine("Đã cập nhật kho. Cảm ơn quý khách!");
    }

    /// <summary>
    /// Chức năng 11: Báo cáo & Thống kê 
    /// </summary>
    public static void BaoCaoThongKe()
    {
        Console.WriteLine("--- BÁO CÁO & THỐNG KÊ KHO HÀNG ---");

        List<MatHang> danhSach = _quanLySieuThi.LayToanBoMatHang();
        if (danhSach.Count == 0)
        {
            Console.WriteLine("Kho rỗng, không có gì để báo cáo.");
            return;
        }

        decimal tongGiaTriKho = 0;

        // Khởi tạo biến tạm để tìm max/min
        // Gán bằng sản phẩm đầu tiên
        MatHang tonNhieuNhat = danhSach[0];
        MatHang tonItNhat = danhSach[0];
        MatHang giaTriNhat = danhSach[0];

        // Dùng vòng lặp foreach để tính toán
        foreach (MatHang mh in danhSach)
        {
            decimal giaTriMatHang = mh.DonGia * mh.SoLuongTon;

            // 1. Tính tổng giá trị kho
            tongGiaTriKho = tongGiaTriKho + giaTriMatHang;

            // 2. Tìm tồn nhiều nhất
            if (mh.SoLuongTon > tonNhieuNhat.SoLuongTon)
            {
                tonNhieuNhat = mh;
            }

            // 3. Tìm tồn ít nhất
            if (mh.SoLuongTon < tonItNhat.SoLuongTon)
            {
                tonItNhat = mh;
            }

            // 4. Tìm giá trị nhất
            decimal giaTriMax = giaTriNhat.DonGia * giaTriNhat.SoLuongTon;
            if (giaTriMatHang > giaTriMax)
            {
                giaTriNhat = mh;
            }
        }

        // In kết quả
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n[THỐNG KÊ TỔNG QUAN]");
        Console.ResetColor();
        Console.WriteLine("Tổng số loại mặt hàng (SKU): " + danhSach.Count);
        Console.WriteLine("Tổng giá trị kho hàng: " + tongGiaTriKho.ToString("N0") + " VNĐ");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n[CHI TIẾT MẶT HÀNG]");
        Console.ResetColor();
        Console.WriteLine("Tồn kho nhiều nhất: " + tonNhieuNhat.Ten + " (Tồn: " + tonNhieuNhat.SoLuongTon + ")");
        Console.WriteLine("Tồn kho ít nhất:   " + tonItNhat.Ten + " (Tồn: " + tonItNhat.SoLuongTon + ")");
        Console.WriteLine("Giá trị nhất:      " + giaTriNhat.Ten + " (Tổng giá trị: " + (giaTriNhat.DonGia * giaTriNhat.SoLuongTon).ToString("N0") + " VNĐ)");
    }

    /// <summary>
    /// Chức năng 12: Kiểm tra Hàng sắp hết hạn 
    /// </summary>
    public static void KiemTraHanSuDung()
    {
        Console.Write("Kiểm tra hàng hết hạn trong bao nhiêu ngày tới? (ví dụ: 7): ");
        int soNgay;
        if (int.TryParse(Console.ReadLine(), out soNgay) == false || soNgay < 0)
        {
            soNgay = 7; // Mặc định 7 ngày
        }

        DateTime homNay = DateTime.Now;
        // Chỉ lấy phần Ngày (bỏ qua Giờ, Phút, Giây) để so sánh cho chính xác
        DateTime homNayLuc0h = homNay.Date;

        DateTime ngayCanhBao = homNayLuc0h.AddDays(soNgay);

        Console.WriteLine("...Đang tìm các mặt hàng có HSD từ hôm nay đến " + ngayCanhBao.ToString("dd/MM/yyyy") + "...");

        List<MatHang> danhSach = _quanLySieuThi.LayToanBoMatHang();
        int demDaHetHan = 0;
        int demSapHetHan = 0;

        // In danh sách ĐÃ HẾT HẠN (Màu Đỏ)
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n--- 1. DANH SÁCH ĐÃ HẾT HẠN ---");
        foreach (MatHang mh in danhSach)
        {
            if (mh.HanSuDung < homNayLuc0h)
            {
                Console.WriteLine(mh.ToString());
                demDaHetHan++;
            }
        }
        if (demDaHetHan == 0) Console.WriteLine("(Không có)");

        // In danh sách SẮP HẾT HẠN (Màu Vàng)
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n--- 2. DANH SÁCH SẮP HẾT HẠN (trong " + soNgay + " ngày tới) ---");
        foreach (MatHang mh in danhSach)
        {
            // Điều kiện: (HSD >= hôm nay) VÀ (HSD <= ngày cảnh báo)
            if (mh.HanSuDung >= homNayLuc0h && mh.HanSuDung <= ngayCanhBao)
            {
                Console.WriteLine(mh.ToString());
                demSapHetHan++;
            }
        }
        if (demSapHetHan == 0) Console.WriteLine("(Không có)");

        Console.ResetColor();
        Console.WriteLine("\n--- Tổng kết: " + demDaHetHan + " đã hết hạn, " + demSapHetHan + " sắp hết hạn. ---");
    }
}

#endregion