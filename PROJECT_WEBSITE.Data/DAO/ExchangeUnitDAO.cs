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
    public class ExchangeUnitDAO
    {
        DbWebsite db;

        public ExchangeUnitDAO()
        {
            db = new DbWebsite();
        }



        public IEnumerable<MDsExchangeUnit> GetAll(SearchNews search, int page, int pageSize)
        {
            var lst = db.ExchangeUnits.Select(t => t).OrderByDescending(x => x.Createdate).AsQueryable();

            var lstrs = new List<MDsExchangeUnit>();
           
            foreach (var item in lst)
            {
                MDsExchangeUnit m = new MDsExchangeUnit();
                var Result = "";

                if (db.ProductUnits.Find(db.Products.Find(item.ProductIDIn).UnitID).ValueUnit.Value > db.ProductUnits.Find(db.Products.Find(item.ProductIDOut).UnitID).ValueUnit.Value)
                {
                    Result = $"{item.ValueCountUnit} " +
                    $"{db.ProductUnits.Find(db.Products.Find(item.ProductIDIn).UnitID).Name} " +
                    $"=> {(db.ProductUnits.Find(db.Products.Find(item.ProductIDIn).UnitID).ValueUnit.Value * item.ValueCountUnit.Value)}" +
                    $" {db.ProductUnits.Find(db.Products.Find(item.ProductIDOut).UnitID).Name}";
                }
                else
                {
                    int rs = ((int)item.ValueCountUnit.Value * (int)db.ProductUnits.Find(db.Products.Find(item.ProductIDIn).UnitID).ValueUnit.Value) / (int)db.ProductUnits.Find(db.Products.Find(item.ProductIDOut).UnitID).ValueUnit.Value;
                    Result = $"{item.ValueCountUnit} " +
                    $"{db.ProductUnits.Find(db.Products.Find(item.ProductIDIn).UnitID).Name} " +
                    $"=> {rs}" +
                    $" {db.ProductUnits.Find(db.Products.Find(item.ProductIDOut).UnitID).Name}";
                }
                m.STT = lst.ToList().IndexOf(item)+1;
                m.ExchangeUnitID = item.ExchangeUnitID;
                m.Createdate = item.Createdate;
                m.ProductNameIn = db.Products.Find(item.ProductIDIn).Name;
                m.ProductNameOut = db.Products.Find(item.ProductIDOut).Name;
                m.Result = Result;
                m.UserName = db.UserRoleGroups.Where(t => t.UserRoleGroupID == db.Users.Where(e => e.UserID == item.UserID).FirstOrDefault().UserRoleGroupID).FirstOrDefault().Name + " - " + db.Users.Where(t => t.UserID == item.UserID).FirstOrDefault().FullName;
                lstrs.Add(m);
            }

            var lstrs1 = lstrs.Select(t => t).AsQueryable();

            if (!string.IsNullOrEmpty(search.SearchName))
            {

                var ExchangeID = CutExchangeUnitID(search.SearchName);

                if (ExchangeID.HasValue)
                {
                    lstrs1 = lstrs1.Where(t => t.ExchangeUnitID == ExchangeID.Value);
                }
                else
                {
                    lstrs1 = lstrs1.Where(t => t.ProductNameIn.Contains(search.SearchName)|| t.ProductNameOut.Contains(search.SearchName)||t.UserName.Contains(search.SearchName)||t.Result.Contains(search.SearchName));
                }

            }
            if (search.TuNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(search.TuNgay.Value, t.Createdate.Value) < 0);
            }
            if (search.DenNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(t.Createdate.Value, search.DenNgay.Value) < 0);
            }


            return lstrs1.ToPagedList(page,pageSize);
        }

        public int? CutExchangeUnitID(string input)
        {
            try
            {
                var rs = int.Parse(input.Substring(2));

                return rs;
            }
            catch (Exception)
            {

                return null;
            }

        }

        public MExchangeUnit GetExchangeByProductID(int ProductID)
        {
            var Rs = new MExchangeUnit();
            var product = db.Products.Find(ProductID);
            Rs.ProductID = ProductID;
            Rs.ProductName = product.Name;
            Rs.UnitName = db.ProductUnits.Find(product.UnitID).Name;

            decimal CountReceiptProduct = db.ReceiptDetails.Where(t => t.ProductID == ProductID).Count();

            int TotalPriceProductInCheck = db.ReceiptDetails.Where(t => t.ProductID == ProductID).Count();

            decimal TotalPriceProductIn = 0;

            if (TotalPriceProductInCheck != 0)
            {
                TotalPriceProductIn = db.ReceiptDetails.Where(t => t.ProductID == ProductID).Sum(t => t.PriceIput.Value);
                Rs.PriceInput = TotalPriceProductIn / CountReceiptProduct;
            }else
            {
                var priceinput = db.Products.Where(t => t.ParentProductID == product.ParentProductID && t.ProductID != product.ProductID).FirstOrDefault();

                decimal a = GetExchangeByProductID(priceinput.ProductID).PriceInput.Value;

                decimal b = (decimal)db.ProductUnits.Find(product.UnitID).ValueUnit.Value;

                Rs.PriceInput = (b*a)/ (decimal)db.ProductUnits.Find(priceinput.UnitID).ValueUnit.Value;
            }

            Rs.ValueUnit = db.ProductUnits.Find(product.UnitID).ValueUnit.Value;

            Rs.CountProduct = product.CountProduct.Value;

            Rs.CategoryName = db.ProductCategories.Where(e => e.ProductCategoryID == product.ProductCategoryID).FirstOrDefault().Name;

            Rs.KyTuCaterory = getnamecategorykytu(Rs.CategoryName);
            return Rs;
        }
        
        public string getnamecategorykytu(string input)
        {
            string[] arraystring = input.Split(' ');
            string KyTu1 = "";
            string KyTu2 = "";

            if (arraystring.Length > 2)
            {
                KyTu1 = arraystring[0].Substring(0, 1);
                KyTu2 = arraystring[1].Substring(0, 1);
            }
            else
            {
                KyTu1 = arraystring[0].Substring(0, 1);
            }

            return KyTu1.ToUpper() + KyTu2.ToUpper();
        }

        public bool AddExchangeUnit(MExchangeUnit mExchangeUnit)
        {
            try
            {

                var exchangeUnit = new ExchangeUnit();
                exchangeUnit.ProductIDIn = mExchangeUnit.ProductID;
                exchangeUnit.ProductIDOut = GetExchangeByID(mExchangeUnit.UnitOut.Value,mExchangeUnit.ProductID).ProductID;
                exchangeUnit.Createdate = DateTime.Now;
                exchangeUnit.ValueCountUnit = mExchangeUnit.ValueCountUnit;
                exchangeUnit.UserID = mExchangeUnit.UserID;
                exchangeUnit.ExchangeStatus = true;

                db.ExchangeUnits.Add(exchangeUnit);

                AddCOuntProductExchangeUnit(exchangeUnit.ProductIDIn.Value, exchangeUnit.ProductIDOut.Value, exchangeUnit.ValueCountUnit.Value);

                db.SaveChanges();

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public Product GetExchangeByID(int idunit, int idprudct)
        {
            var produduct = db.Products.Find(idprudct);

            var productbase = db.Products.Find(produduct.ParentProductID);

            if (productbase.UnitID == idunit)
            {
                return productbase;
            }else
            {
                var lst = db.Products.Where(t => t.ParentProductID == productbase.ProductID).ToList();

                foreach (var item in lst)
                {
                    if (item.UnitID == idunit)
                    {
                        return item;
                    }
                }
            }
            return null;    
        }

        public void AddCOuntProductExchangeUnit(int ProductInput, int ProductOutput, int Count)
        {

            var ProductIn = db.Products.Find(ProductInput);

            ProductIn.CountProduct = ProductIn.CountProduct - Count;

            var UnitValue = db.ProductUnits.Find(ProductIn.UnitID).ValueUnit;

            var ProductOut = db.Products.Find(ProductOutput);

            ProductOut.CountProduct = ProductOut.CountProduct + (int?)((int?)(UnitValue * Count)/db.ProductUnits.Find(ProductOut.UnitID).ValueUnit);

            db.SaveChanges();

        }
    }
}
