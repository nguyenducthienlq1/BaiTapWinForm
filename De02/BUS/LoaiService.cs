using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class LoaiService
    {
        public List<LoaiSP> GetAll()
        {
            SanPhamModels context = new SanPhamModels();
            return context.LoaiSPs.ToList();
        }

        public List<LoaiSP> GetLoaiSPs()
        {
            SanPhamModels context = new SanPhamModels();
            return context.LoaiSPs.ToList();
        }

        public LoaiSP GetById(string id)
        {
            SanPhamModels context = new SanPhamModels();
            return context.LoaiSPs.Find(id);
        }

        public LoaiSP GetLoaiSPByName(string name)
        {
            SanPhamModels context = new SanPhamModels();
            return context.LoaiSPs.FirstOrDefault(l => l.TenLoai == name);
        }
    }
}

