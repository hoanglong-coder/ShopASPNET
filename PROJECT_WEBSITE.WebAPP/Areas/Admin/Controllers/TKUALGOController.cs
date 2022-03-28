using PROJECT_WEBSITE.Data.ModelCustom;
using PROJECT_WEBSITE.Data.TKU_Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using PROJECT_WEBSITE.Data.EF;
using PROJECT_WEBSITE.Data.DAO;
using PROJECT_WEBSITE.WebAPP.Common;

namespace PROJECT_WEBSITE.WebAPP.Areas.Admin.Controllers
{
    public class TKUALGOController : RoleTOPKController
    {
        DbWebsite db = new DbWebsite();
        // GET: Admin/TKUALGO
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;


            return View();
        }


        [HttpGet]
        public JsonResult CreateTXT()
        {
            MainTKU mainTKU = new MainTKU();

            var check = mainTKU.GhiFileTxt();

            if (check != null)
            {
                return Json(new { code = 200, data = true, NameFile = check }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { code = 200, data = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult StartTKU(string path, int topk)
        {
            MainTKU mainTKU = new MainTKU();

            string[] rs = path.Split('\\');

            string fileLPath = Server.MapPath(@"~/Data/"+rs[2]);

            var check = mainTKU.Main(fileLPath, topk);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult ReadFile(string output)
        {
            try
            {
                var lst = new List<MTopK>();

                string[] rs = output.Split('\\');

                string fileLPath = Server.MapPath(@"~/Data/" + rs[2]);

                List<string> ListRecords = new List<string>();

                using (StreamReader tempReader = new StreamReader(fileLPath))
                {
                    string tempLine = string.Empty;
                    while ((tempLine = tempReader.ReadLine()) != null)
                    {
                        ListRecords.Add(tempLine);
                    }
                }

                foreach (var item in ListRecords)
                {
                    MTopK mTopK = new MTopK();

                    string[] temp = item.Split(':');

                    string[] temp2 = temp[0].Split(' ');

                    mTopK.STT = ListRecords.IndexOf(item) + 1;

                    mTopK.PU = int.Parse(temp[1]);

                    List<MProduct> lstproduct = new List<MProduct>();
                    foreach (var item1 in temp2)
                    {
                        var product = db.Products.Find(int.Parse(item1));

                        MProduct mProduct = new MProduct();
                        mProduct.ProductID = product.ProductID;
                        mProduct.Name = product.Name;

                        lstproduct.Add(mProduct);
                    }
                    mTopK.MProduct = lstproduct;
                    lst.Add(mTopK);
                }


                return Json(new { code = 200, data = lst.OrderByDescending(t=>t.PU), status =true   }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { code = 200,status=false }, JsonRequestBehavior.AllowGet);
            }
            
        }


        [HttpPost]
        public JsonResult GetProducts(List<int> id)
        {
            var dao = new ProductDAO();
            var lst = dao.ProductTOPK(id);

            return Json(new { code = 200, data = lst }, JsonRequestBehavior.AllowGet);
        }
    }
}