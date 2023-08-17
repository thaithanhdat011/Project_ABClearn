using ABCLearn.DataContext;
using ABCLearn.Extend;
using ABCLearn.Models;
using ABCLearn.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ABCLearn.Controllers
{
	public class LoginController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult forgotPass()
		{
			if (HttpContext.Session.GetObject<UserLogin>("User") == null)
			{
				UserLogin.Instance = null;
			}
			return View();
		}
		public IActionResult Login(AccountLogin acc, string submitButton)
		{
			if (submitButton == "Sutdent")
			{
				if (StudentDAO.Instance.login(acc))
                //Gọi phương thức login từ đối tượng StudentDAO.Instance để xác thực đăng nhập của sinh viên, với tham số là acc.
                {
                    DateTime nowUtc = DateTime.UtcNow;
					TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // Múi giờ của Việt Nam
					DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, vietnamTimeZone);
					var check = (vietnamTime - UserLogin.Instance.DateCreated); //Tính toán khoảng thời gian giữa thời điểm đăng nhập và thời điểm tạo tài khoản.
                    if (check.TotalDays >= 30 || !UserLogin.Instance.IsConfirmEmail)
					//Kiểm tra nếu khoảng thời gian đã trôi qua từ lần đăng nhập cuối cùng lớn hơn hoặc bằng 30 ngày hoặc người dùng chưa xác nhận email
                    {
						return RedirectToAction("ConfirmAcceptAccout", "Register", new { email = UserLogin.Instance.Email, forgot = UserLogin.Instance.Id });
                        // Chuyển hướng người dùng đến hành động "ConfirmAcceptAccout" trong controller "Register" với các tham số email và forgot.
                    }
                    UserLogin.Instance.Islogin = true; // đánh dấu người dùng bằng cách gán giá trị true
					UserLogin.Instance.RoleID = "Student"; // gán student cho roleID
					foreach (var course in UserLogin.Instance.Courses) //Lặp qua danh sách các khóa học 
                    {
						course.Calendars = CourseDAO.Instance.getCalendar(course.Id);
                        // //Gán danh sách lịch học của khóa học vào thuộc tính Calendars của đối tượng course.
                    }
                    HttpContext.Session.SetObject("User", UserLogin.Instance);
                    // Lưu đối tượng UserLogin.Instance vào Session với khóa là "User"
                    return RedirectToAction("Index", "Home");
				}
				else
				{
					TempData["MessError"] = "Pass Or Email was Wrong!!";
                    //Gán giá trị "Pass Or Email was Wrong!!" cho khóa "MessError" trong TempData
                    return RedirectToAction("Index", "Login");
				}
			}
			else
			{
				if (LecturerDAO.Instance.login(acc))
				{
					UserLogin.Instance.Islogin = true; // Đánh dấu người dùng đã đăng nhập bằng cách gán giá trị true
                    UserLogin.Instance.RoleID = "Lecturer"; //Gán giá trị "Lecturer" cho thuộc tính RoleID của đối tượng UserLogin.Instance
                    UserLogin.Instance.TimeLogin = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
					foreach (var course in UserLogin.Instance.Courses) //Lặp qua danh sách các khóa học của người dùng
                    {
						course.Calendars = CourseDAO.Instance.getCalendar(course.Id);
                        //Gán danh sách lịch học của khóa học vào thuộc tính Calendars của đối tượng course
                    }
                    HttpContext.Session.SetObject("User", UserLogin.Instance);
                    //Lưu đối tượng UserLogin.Instance vào Session với khóa là "User".
                    return RedirectToAction("Profile", "Home");
				}
				else
				{
					TempData["MessError"] = "Pass Or Email was Wrong!!";
                    ////Gán giá trị "Pass Or Email was Wrong!!" cho khóa "MessError" trong TempData
                    return RedirectToAction("Index", "Login");
				}
			}
		}

		public IActionResult Logout() //Khai báo phương thức Logout trả về một đối tượng IActionResult.
        {
			UserLogin.Instance.Islogin = false;
            //Đánh dấu người dùng đã đăng xuất bằng cách gán giá trị false cho thuộc tính Islogin của đối tượng UserLogin.Instance.
            UserLogin.Instance.RoleID = "Guest";
            //Gán giá trị "Guest" cho thuộc tính RoleID của đối tượng UserLogin.Instance, đại diện cho vai trò của người dùng sau khi đăng xuất.
            HttpContext.Session.Clear();
            //Xóa toàn bộ dữ liệu trong Session, bao gồm cả đối tượng UserLogin.Instance.
            return RedirectToAction("Index", "Home");
		}
		private void renderData()
		{
			LecturerDAO.Instance.GetAll();
			StudentDAO.Instance.GetAll();
			CourseDAO.Instance.GetAll();
			QuizDAO.Instance.GetAll();
		}
		public IActionResult ResetPassword(string Password)
		{
			var role = HttpContext.Session.GetString("role");
			if (role == "Student")
			{
				Profile.Instance.Password = Password;
				if (StudentDAO.Instance.resetPassword(Profile.Instance))
				{
					Profile.Instance = null;
					TempData["MessSS"] = "Success!!";
					StudentDAO.Instance.SaveChange();
					return RedirectToAction("Index", "Login");
				}
			}
			else
			{
				Profile.Instance.Password = Password;
				if (LecturerDAO.Instance.resetPassword(Profile.Instance))
				{
					Profile.Instance = null;
					TempData["MessSS"] = "Success!!";
					LecturerDAO.Instance.SaveChange();
					return RedirectToAction("Index", "Login");
				}
			}
			TempData["MessError"] = "ERROR in reset password!!";
			return RedirectToAction("Index", "Login");
		}
	}
}
