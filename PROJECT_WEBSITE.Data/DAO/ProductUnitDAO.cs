using PagedList;
using PROJECT_WEBSITE.Data.EF;
using PROJECT_WEBSITE.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.DAO
{
    public class ProductUnitDAO
    {
        DbWebsite db;
        public ProductUnitDAO()
        {
            db = new DbWebsite();
        }

        /// <summary>
        /// Danh sách đơn vị tính cơ bản
        /// </summary>
        /// <returns></returns>
        public List<MProductUnit> ListProductUnitBase()
        {
            var lstUnit = db.ProductUnits.Where(t => t.ValueUnit == 1).Select(t => new MProductUnit() { 
                UnitID = t.UnitID,
                Name = t.Name,
                ValueUnit = t.ValueUnit           
            }).ToList();

            return lstUnit;
        }

        /// <summary>
        /// Danh sách đơn vị tính != cơ bản
        /// </summary>
        /// <returns></returns>
        public List<MProductUnit> ListProductUnit()
        {
            var lstUnit = db.ProductUnits.Where(t => t.ValueUnit != 1).Select(t => new MProductUnit()
            {
                UnitID = t.UnitID,
                Name = t.Name,
                ValueUnit = t.ValueUnit
            }).ToList();

            return lstUnit;
        }

        public IEnumerable<MProductUnit> ListProductUnitAll(SearchUnit search,int page, int pageSize)
        {
            var lstRs = new List<MProductUnit>();

            var lstUnit = db.ProductUnits.OrderByDescending(t=>t.UnitID).AsQueryable();

            if (!string.IsNullOrEmpty(search.query))
            {
                var madvt = CutMaDVT(search.query);

                if (madvt.HasValue)
                {
                    lstUnit = lstUnit.Where(t => t.UnitID == madvt.Value);
                }else
                {
                    lstUnit = lstUnit.Where(t => t.Name.Contains(search.query));
                }
            }
            if (search.LoaiDVT.HasValue)
            {
                if (search.LoaiDVT.Value == true)
                {
                    lstUnit = lstUnit.Where(t => t.ValueUnit == 1);
                }else
                {
                    lstUnit = lstUnit.Where(t => t.ValueUnit != 1);
                }
            }
            if (search.GiaTri.HasValue)
            {
                lstUnit = lstUnit.Where(t => t.ValueUnit == search.GiaTri.Value);
            }

            foreach (var item in lstUnit)
            {
                var m = new MProductUnit();
                m.STT = lstUnit.ToList().IndexOf(item) + 1;
                m.UnitID = item.UnitID;
                m.Name = item.Name;
                m.ValueUnit = item.ValueUnit;

                lstRs.Add(m);
            }

            return lstRs.ToPagedList(page, pageSize);
        }

        public int? CutMaDVT(string input)
        {
            try
            {
                var rs = int.Parse(input.Substring(4));

                return rs;
            }
            catch (Exception)
            {

                return null;
            }

        }

        public MProductUnit GetUnitByID(int id)
        {
            var unit = db.ProductUnits.Find(id);

            var rs = new MProductUnit();
            rs.UnitID = id;
            rs.Name = unit.Name;
            rs.ValueUnit = unit.ValueUnit;

            return rs;
        }


        public bool CreateUnit(MProductUnit mProductUnit)
        {
            try
            {
                var productunit = new ProductUnit();
                productunit.Name = mProductUnit.Name;
                productunit.ValueUnit = mProductUnit.ValueUnit;

                db.ProductUnits.Add(productunit);

                db.SaveChanges();

                return true;


            }
            catch (Exception)
            {

                return false;
            }
        }


        public bool UpdateUnit(MProductUnit mProductUnit)
        {
            try
            {
                var Unit = db.ProductUnits.Find(mProductUnit.UnitID);
                Unit.Name = mProductUnit.Name;
                Unit.ValueUnit = mProductUnit.ValueUnit;

                db.SaveChanges();

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public DeleteUnit DeleteUnitByID(int id)
        {
            var check = db.Products.Where(t => t.UnitID == id).Count();

            if (check != 0)
            {
                var rs1 = new DeleteUnit();
                rs1.Check = false;
                rs1.Result = "Không thể xóa do có sản phẩm đang sử dụng đơn vị tính này";
                return rs1;
            }
            var unit = db.ProductUnits.Find(id);

            db.ProductUnits.Remove(unit);

            db.SaveChanges();

            var rs = new DeleteUnit();
            rs.Check = true;
            return rs;
        }

        public List<MProductUnit> GetExchangeByID(int id)
        {
            //Tìm sản phẩm chính nó
            var productbase = db.Products.Find(id).ParentProductID;

            List<MProductUnit> mProductUnits = new List<MProductUnit>();

            var product = db.Products.Find(productbase);

            var Unit = db.ProductUnits.Find(product.UnitID);

            MProductUnit m = new MProductUnit();
            m.UnitID = Unit.UnitID;
            m.Name = Unit.Name;
            m.ValueUnit = Unit.ValueUnit;

            mProductUnits.Add(m);


            var lstparent = db.Products.Where(t => t.ParentProductID == product.ProductID && t.ProductID != id);

            if (lstparent.Count() != 0)
            {
                foreach (var item in lstparent)
                {
                    var Unit1 = db.ProductUnits.Find(item.UnitID);
                    MProductUnit m1 = new MProductUnit();
                    m1.UnitID = Unit1.UnitID;
                    m1.Name = Unit1.Name;
                    m1.ValueUnit = Unit1.ValueUnit;

                    mProductUnits.Add(m1);
                }
            }
           
            return mProductUnits;
        }
    }
}
