using PagedList;
using PROJECT_WEBSITE.Data.EF;
using PROJECT_WEBSITE.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.DAO
{
    public class UserDAO
    {
        DbWebsite db; 
        public UserDAO()
        {
            db = new DbWebsite();
        }
        public int Insert(User entity)
        {
            db.Users.Add(entity);
            db.SaveChanges();
            return entity.UserID;
        }
        public bool Delete(int id)
        {
            try
            {
                var user = db.Users.Find(id);
                db.Users.Remove(user);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(User entity)
        {
            try
            {
                var user = db.Users.Find(entity.UserID);
                user.FullName = entity.FullName;
                user.Email = entity.Email;
                user.Phone = entity.Phone;
                user.Address = entity.Address;
                user.UserStatus = entity.UserStatus;
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        public User ViewDetail(int id)
        {
            return db.Users.Find(id);
        }
        public User GetById(string userName)
        {
            return db.Users.SingleOrDefault(x => x.UserName == userName);
        }
        public int Login(string userName, string passWord)
        {
            var result = db.Users.SingleOrDefault(x => x.UserName == userName);
            if (result == null)
            {
                //Không tồn tại
                return 0;
            }
            else
            {
                if (result.UserStatus == 0)
                {
                    //Bị khóa
                    return -1;
                }
                else
                {
                    if (result.Passsword == passWord)
                    {
                        //Đúng
                        return 1;
                    }
                    else
                    {
                        //Sai mật khẩu
                        return -2;
                    }
                }
            }
        }
        public IEnumerable<User> ListAllAccount()
        {
            return db.Users.ToList();
        }

        public List<int> GetListRoles(int RoleGroup)
        {
            var lst = db.UserRoleGroupDetails.Where(t => t.UserRoleGroupID == RoleGroup);
            var lstrs = new List<int>();
            if (lst.Count() != 0)
            {

                lstrs = lst.Select(t => t.UserRoleID).ToList();
                return lstrs;
            }
            return lstrs;
        }

        public string GetNameRole(int id)
        {
            return db.UserRoleGroups.Find(id).Name;
        }

        public IEnumerable<MUser> GetListUser(int page, int pageSize)
        {

            var lst = db.Users.OrderByDescending(t => t.CreateDate);

            var lstrs = new List<MUser>();

            foreach (var item in lst)
            {
                MUser m = new MUser();
                m.STT = lst.ToList().IndexOf(item) + 1;
                m.UserID = item.UserID;
                m.UserName = item.UserName;
                m.FullName = item.FullName;
                m.Birth = item.Birth;
                m.Address = item.Address;
                m.Phone = item.Phone;
                m.CreateDate = item.CreateDate;
                m.UserRoleGroupID = item.UserRoleGroupID;
                m.UserRoleGroup = db.UserRoleGroups.Find(m.UserRoleGroupID).Name;
                m.UserStatus = item.UserStatus;
                lstrs.Add(m);
            }
            return lstrs.ToPagedList(page, pageSize);
        }

        public bool ChangeUser(int id)
        {
            try
            {
                var product = db.Users.Find(id);
                product.UserStatus = product.UserStatus == 1 ? 0 : 1;

                db.SaveChanges();

                return product.UserStatus == 1 ? true : false;
            }
            catch (Exception)
            {

                return false;
            }
        }
        public static string Md5(string sInput)
        {
            HashAlgorithm algorithmType = default(HashAlgorithm);
            ASCIIEncoding enCoder = new ASCIIEncoding();
            byte[] valueByteArr = enCoder.GetBytes(sInput);
            byte[] hashArray = null;
            // Encrypt Input string 
            algorithmType = new MD5CryptoServiceProvider();
            hashArray = algorithmType.ComputeHash(valueByteArr);
            //Convert byte hash to HEX
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashArray)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
        public MDelete CreateUser(User mUser)
        {
            try
            {
                var check = db.Users.Where(t => t.UserName == mUser.UserName);

                if (check.Count() != 0)
                {
                    MDelete mDelete1 = new MDelete();
                    mDelete1.Check = false;
                    mDelete1.Result = "Trùng tài khoản";
                    return mDelete1;
                }

                mUser.CreateDate = DateTime.Now;
                mUser.Passsword = Md5(mUser.Passsword);
                mUser.UserStatus = 0;
                db.Users.Add(mUser);

                db.SaveChanges();

                MDelete mDelete = new MDelete();
                mDelete.Check = true;
                return mDelete;
            }
            catch (Exception)
            {
                MDelete mDelete1 = new MDelete();
                mDelete1.Check = false;
                return mDelete1;
            }
            
        }


        public IEnumerable<MUserRoleGroup> ListChucVu(SearchNews search, int page, int pageSize)
        {

            var lst = db.UserRoleGroups.OrderByDescending(t => t.CreateDate);

            var lstrs = new List<MUserRoleGroup>();

            foreach (var item in lst)
            {
                MUserRoleGroup m = new MUserRoleGroup();
                m.STT = lst.ToList().IndexOf(item) + 1;
                m.UserRoleGroupID = item.UserRoleGroupID;
                m.Name = item.Name;
                m.CreateDate = item.CreateDate;

                lstrs.Add(m);

            }

            return lstrs.ToPagedList(page, pageSize);

        }

        public MUser GetUserByID(int id)
        {
            var item = db.Users.Find(id);

            MUser m = new MUser();
            m.UserID = item.UserID;
            m.UserName = item.UserName;
            m.FullName = item.FullName;
            m.Birth = item.Birth;
            m.Address = item.Address;
            m.Passsword = item.Passsword;
            m.Phone = item.Phone;
            m.CreateDate = item.CreateDate;
            m.UserRoleGroupID = item.UserRoleGroupID;
            m.UserRoleGroup = db.UserRoleGroups.Find(m.UserRoleGroupID).Name;
            m.UserStatus = item.UserStatus;

            return m;
        }

        public bool UpdateUser(User mUser)
        {
            try
            {
                var m = db.Users.Find(mUser.UserID);

                m.FullName = mUser.FullName;
                m.Birth = mUser.Birth;
                m.Address = mUser.Address;
                m.Phone = mUser.Phone;
                m.UserRoleGroupID = mUser.UserRoleGroupID;

                if (!string.IsNullOrEmpty(mUser.Passsword))
                {
                    m.Passsword = Md5(mUser.Passsword);
                }

                db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public DeleteProduct DeleteUser(int id)
        {
            var checkorrder = db.Orders.Where(t => t.UserID == id).Count();
            if (checkorrder != 0)
            {
                var rs1 = new DeleteProduct();
                rs1.Check = false;
                rs1.Result = "Không thể xóa do nhân viên này sử dụng trong báo cáo đơn hàng";
                return rs1;
            }
            var user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            var rs = new DeleteProduct();
            rs.Check = true;
            return rs;
        }

        public IEnumerable<MRole> GetQuyen(int page, int pageSize)
        {
            var lst = db.UserRoles.Where(t=>t.UserRoleStatus==true).OrderBy(t => t.UserRoleID);

            var lstrs = new List<MRole>();

            foreach (var item in lst)
            {
                MRole m = new MRole();
                m.STT = lst.ToList().IndexOf(item) + 1;
                m.UserRoleID = item.UserRoleID;
                m.Name = item.Name;
                m.CreateDate = item.CreateDate;
                lstrs.Add(m);
            }

            return lstrs.ToPagedList(page, pageSize);
        }

        public int CreateChucVu(string name)
        {

            var check = db.UserRoleGroups.Where(t => t.Name == name);

            if (check.Count() != 0)
            {
                return 0;
            }
            UserRoleGroup m = new UserRoleGroup();
            m.Name = name;
            m.CreateDate = DateTime.Now;
            m.GroupStatus = true;

            db.UserRoleGroups.Add(m);

            db.SaveChanges();

            return m.UserRoleGroupID;
        }

        public bool CheckTrungChucVu(string name,int id)
        {
            var check = db.UserRoleGroups.Where(t => t.Name == name&&t.UserRoleGroupID!=id);

            if (check.Count() != 0)
            {
                return false;
            }
            return true;
        }


        public MDelete CreateChucVuDetail(List<int> role,string name)
        {
            int idchucvu = CreateChucVu(name);
            if (idchucvu != 0)
            {
                foreach (var item in role)
                {
                    var m = new UserRoleGroupDetail();
                    m.UserRoleGroupID = idchucvu;
                    m.UserRoleID = item;
                    m.RoleDetailStatus = true;
                    db.UserRoleGroupDetails.Add(m);
                }
                db.SaveChanges();
                var mdelet = new MDelete();
                mdelet.Check = true;
                return mdelet;
            }
            var mdelet2 = new MDelete();
            mdelet2.Check = false;
            mdelet2.Result = "Chức vụ này đã có";
            return mdelet2;

        }

        public List<int> GetRoleChucVu(int id)
        {
            var lst = new List<int>();

            var role = db.UserRoleGroupDetails.Where(t => t.UserRoleGroupID == id);

            foreach (var item in role)
            {
                lst.Add(item.UserRoleID);
            }
            return lst;
        }


        public MDelete UpdateChucVu(UserRoleGroup userRoleGroup, List<int> role)
        {
            if (CheckTrungChucVu(userRoleGroup.Name,userRoleGroup.UserRoleGroupID))
            {
                var cv = db.UserRoleGroups.Find(userRoleGroup.UserRoleGroupID);
                cv.Name = userRoleGroup.Name;

                db.SaveChanges();

                Update(role, userRoleGroup.UserRoleGroupID);

                var mdelet = new MDelete();
                mdelet.Check = true;
                return mdelet;

                
            }
            var mdelet2 = new MDelete();
            mdelet2.Check = false;
            mdelet2.Result = "Chức vụ này đã có";
            return mdelet2;
        }

        public void Update(List<int> role, int id)
        {
            var lst = db.UserRoleGroupDetails.Where(t => t.UserRoleGroupID == id).ToList();

            db.UserRoleGroupDetails.RemoveRange(lst);

            foreach (var item in role)
            {
                var m = new UserRoleGroupDetail();
                m.UserRoleGroupID = id;
                m.UserRoleID = item;
                m.RoleDetailStatus = true;
                db.UserRoleGroupDetails.Add(m);
            }

            db.SaveChanges();
        }

        public MDelete DelelteChucVu(int id)
        {
            var check = db.UserRoleGroups.Where(t => t.UserRoleGroupID == id);

            if (check.Count() != 0)
            {
                var rs1 = new MDelete();
                rs1.Check = false;
                rs1.Result = "Không thể xóa do chức vụ này đang được sử dụng";
                return rs1;
            }

            var chucvu = db.UserRoleGroups.Find(id);

            db.UserRoleGroups.Remove(chucvu);

            db.SaveChanges();
            var rs = new MDelete();
            rs.Check = true;
            return rs;

        }


    }
}
