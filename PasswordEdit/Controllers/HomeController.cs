using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using YUN;
namespace PasswordEdit.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {

            string ticket = Request["ticket"];//"fb82c715df4f8bb8ded6737bb868a257";// 
            if (string.IsNullOrEmpty(ticket))
            {
                return View("Error");
            }

            //获取AppSettings的节点 

            string appID = ConfigurationManager.AppSettings["appID"];//轻应用注册到云之家时生成
            string appSecret = ConfigurationManager.AppSettings["appSecret"];//轻应用注册到云之家时生成
            YUNAPI.YUNXT = ConfigurationManager.AppSettings["yunxt"];
            
            try
            {
                YunUser user = await YUNAPI.GetYunUser(ticket, appID, appSecret);

                ViewData["userName"] = user.mobile;

                ViewData["msg"] = "";
            }
            catch (Exception ex)
            {
                return View("Error");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Index(FormCollection f)
        {
            string oldPassword = f["oldPw"];
            string newPassword = f["newPw"];
            string newPassword_twice = f["newPw_twice"];
            string userName = f["userName"];
            ViewData["userName"] = userName;
            ResetPw(userName, oldPassword, newPassword, newPassword_twice);
            return View("Index");
        }
        public Boolean ResetPw(string userName, String oldPassword, String newPassword, String newPassword_twice)
        {

            if (newPassword_twice != newPassword)
            {
                ViewData["msg"] = "两次密码输入不一致,请重新填写.";
                return false;
            }

            if (oldPassword==newPassword)
            {
                ViewData["msg"] = "新密码与原密码一致,请重新填写.";
                return false;
            }
            

            DirectoryEntry de = new DirectoryEntry("LDAP://DC.weigaogroup.com", userName, oldPassword);//域的根路径
            de.UsePropertyCache = true;

            DirectorySearcher searcher = new DirectorySearcher();
            searcher.SearchRoot = de;
            searcher.SearchScope = SearchScope.Subtree;
            searcher.Filter = string.Format("(&(objectClass=user)(samAccountName={0}))", userName);

            try
            {
                SearchResult result = searcher.FindOne();

                if (result == null)
                {
                    ViewData["msg"] = "该用户不存在,无法进行密码修改.";
                    return false;
                }

                result.GetDirectoryEntry().Invoke("ChangePassword", new object[] { oldPassword, newPassword });
                ViewData["msg"] = "密码修改成功.";
                System.Diagnostics.Debug.WriteLine("密码修改成功");
                return true;

            }
            catch (Exception ex)
            {
                ViewData["msg"] =  "密码修改失败,请检测旧密码是否正确";
                return false;
            }

        }
        
    }
}