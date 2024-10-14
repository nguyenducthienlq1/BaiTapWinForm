using BUS;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace De02
{
    public partial class frmSanPham : Form
    {
        private readonly SanPhamService sanPhamService = new SanPhamService();  
        private readonly LoaiService loaiService = new LoaiService();
        SanPhamModels db = new SanPhamModels();
        public frmSanPham()
        {
            InitializeComponent();
            dgvSanPham.CellClick += dgvSanPham_CellClick;
            btnThoat.Click += btnThoat_Click;
        }
        private void FillFalcultyCombobox(List<LoaiSP> listSanPham)
        {
            listSanPham.Insert(0, new LoaiSP { MaLoai = "", TenLoai = "--Chọn loại sản phẩm--" });

            // Gán dữ liệu vào ComboBox
            cboLoaiSP.DataSource = listSanPham;
            cboLoaiSP.DisplayMember = "TenLoai";  // Hiển thị tên loại
            cboLoaiSP.ValueMember = "MaLoai";     // 

        }
        private void BindGrid(List<Sanpham>listSanPham)
        {
            dgvSanPham.Rows.Clear();
            foreach(var item in listSanPham)
            {
                int index = dgvSanPham.Rows.Add();
                dgvSanPham.Rows[index].Cells[0].Value = item.MaSP;
                dgvSanPham.Rows[index].Cells[1].Value = item.TenSP;
                dgvSanPham.Rows[index].Cells[2].Value = item.Ngaynhap;
                dgvSanPham.Rows[index].Cells[3].Value = item.LoaiSP.TenLoai;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            var listSanPham = sanPhamService.GetAll();
            var listLoai = loaiService.GetAll();
            BindGrid(listSanPham);
            FillFalcultyCombobox(listLoai);
        }
        private void dgvSanPham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Lấy dòng được chọn
                DataGridViewRow row = dgvSanPham.Rows[e.RowIndex];

                // Đổ dữ liệu từ dòng vào các TextBox
                txtMaSP.Text = row.Cells[0].Value.ToString();   
                txtTenSP.Text = row.Cells[1].Value.ToString();  
                dtNgayNhap.Text = row.Cells[2].Value.ToString();   
                cboLoaiSP.Text = row.Cells[3].Value.ToString();    
            }
        }
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string searchValue = txtTimKiem.Text.Trim(); // Lấy giá trị tìm kiếm

            if (string.IsNullOrEmpty(searchValue)) // Nếu txtFind rỗng, tải lại dữ liệu ban đầu
            {
                LoadData();// Tải lại toàn bộ dữ liệu
            }
            else
            {
                // Tìm kiếm theo cả MaSP hoặc TenSP
                var filteredItems = sanPhamService.GetAll()
                    .Where(sp => sp.MaSP.Contains(searchValue) || sp.TenSP.Contains(searchValue))
                    .ToList();

                if (filteredItems.Count == 0) // Nếu không tìm thấy kết quả
                {
                    MessageBox.Show("Không tìm thấy sản phẩm nào phù hợp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Hiển thị dữ liệu đã lọc lên dgv_items
                    BindGrid(filteredItems);
                }
            }
        }
        private void ClearData()
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            dtNgayNhap.Text = "";
            cboLoaiSP.SelectedIndex = 0;
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(txtTenSP.Text) || cboLoaiSP.SelectedIndex == 0)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tạo đối tượng sản phẩm mới
                Sanpham sanpham = new Sanpham
                {
                    MaSP = txtMaSP.Text, // Tạo mã sản phẩm ngẫu nhiên
                    TenSP = txtTenSP.Text,
                    Ngaynhap = dtNgayNhap.Value
                };

                // Lấy mã loại sản phẩm
                var selectedLoai = cboLoaiSP.SelectedItem as LoaiSP;
                if (selectedLoai != null)
                {
                    sanpham.MaLoai = selectedLoai.MaLoai;
                }

                // Thêm vào cơ sở dữ liệu
                using (var db = new SanPhamModels())
                {
                    db.Sanphams.Add(sanpham);
                    db.SaveChanges();
                }

                // Cập nhật lại DataGridView
                var listSanPham = sanPhamService.GetAll();
                BindGrid(listSanPham);

                // Làm sạch dữ liệu nhập
                ClearData();

                MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSanPham.CurrentRow != null)
            {
                // Lấy mã sản phẩm từ dòng được chọn
                string maSP = dgvSanPham.CurrentRow.Cells[0].Value.ToString();

                // Xác nhận với người dùng trước khi xóa
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        // Xóa sản phẩm trong cơ sở dữ liệu
                        using (var db = new SanPhamModels())
                        {
                            var sanPham = db.Sanphams.SingleOrDefault(sp => sp.MaSP == maSP);
                            if (sanPham != null)
                            {
                                db.Sanphams.Remove(sanPham);
                                db.SaveChanges();

                                // Xóa dòng trong DataGridView
                                dgvSanPham.Rows.RemoveAt(dgvSanPham.CurrentRow.Index);

                                MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Làm sạch các trường nhập liệu
                                ClearData();
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy sản phẩm để xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadData()
        {
            var listItems = sanPhamService.GetAll();
            BindGrid(listItems);
        }
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvSanPham.SelectedRows.Count > 0)
            {
                // Lấy ID của sinh viên từ dòng được chọn
                string IdSanPham = dgvSanPham.SelectedRows[0].Cells[0].Value.ToString();

                using (var db = new SanPhamModels())
                {
                    var sanpham = db.Sanphams.FirstOrDefault(s => s.MaSP == IdSanPham);
                    if (sanpham != null)
                    {
                        sanpham.TenSP = txtTenSP.Text;
                        string selectedLoaiSP = cboLoaiSP.Text;
                        var loai = db.LoaiSPs.FirstOrDefault(f => f.TenLoai == selectedLoaiSP);
                        if (loai != null)
                        {
                            sanpham.MaLoai = loai.MaLoai;
                        }
                        else
                        {
                            MessageBox.Show("Loại không tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        sanpham.Ngaynhap = dtNgayNhap.Value;
                        db.SaveChanges();

                        var selectedRow = dgvSanPham.SelectedRows[0];
                        selectedRow.Cells[1].Value = sanpham.TenSP;
                        selectedRow.Cells[2].Value = loai.TenLoai;
                        selectedRow.Cells[3].Value = sanpham.Ngaynhap;
                        MessageBox.Show("Cập nhật sản phẩm thành công!");

                        ClearData();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sản phẩm.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để chỉnh sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Đóng ứng dụng
                this.Close();
            }
        }
    }
}
