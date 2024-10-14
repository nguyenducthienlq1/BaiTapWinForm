using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class SanPhamService
    {
        private readonly SanPhamModels context = new SanPhamModels();
        public List<Sanpham> GetAll()
        {
            return context.Sanphams.ToList();
        }
        public Sanpham timKiemSP(string name)
        {
            return context.Sanphams.Where(s => s.TenSP == name).FirstOrDefault();
        }

    }
}
