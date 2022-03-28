using PROJECT_WEBSITE.Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace PROJECT_WEBSITE.Data.DAO
{
    public class SildeDAO
    {

        DbWebsite db;

        public SildeDAO()
        {
            db = new DbWebsite();
        }

        public List<Slide> getall()
        {
            return db.Slides.Select(t => t).ToList();
        }

        public Slide GetID(int id)
        {
            return db.Slides.Find(id);
        }

        public bool Update(Slide slide)
        {
            try
            {
                var slide1 = db.Slides.Find(slide.SlideID);
                if (slide.ImageMore != null)
                {
                    JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();

                    var lstImageMore = scriptSerializer.Deserialize<List<string>>(slide.ImageMore);

                    XElement xElement = new XElement("Images");

                    foreach (var item in lstImageMore)
                    {
                        xElement.Add(new XElement("Image", item));
                    }

                    slide1.ImageMore = xElement.ToString();

                }
                else
                {
                    slide1.ImageMore = string.Empty;
                }

                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

