﻿using ABCLearn.Models;
using System.Data;

namespace ABCLearn.DataContext
{
    public class LecturerDAO
    {
        private List<Lecturer> _lecturers = new List<Lecturer>();
        private static LecturerDAO insctance;
        public static LecturerDAO Instance
        {
            get
            {
                if (insctance == null)
                {
                    insctance = new LecturerDAO();
                }
                return insctance;
            }
        }

        public List<Lecturer> Lecturers()
        {
            getLecturer();
            return _lecturers;
        }

        public void getLecturer()
        {
            if (_lecturers.Count == 0)
            {
                string query = "SELECT * FROM tblLecturer";
                DataTable dataTable = ConectionData.ExecuteQuery(query);
                foreach (DataRow row in dataTable.Rows)
                {
                    Lecturer obj = new Lecturer()
                    {
                        Id = Convert.ToInt32(row["IDLecturer"].ToString().Trim()),
                        FirstName = row["FirstName"].ToString().Trim(),
                        LastName = row["LastName"].ToString().Trim(),
                        RoleID = row["RoleID"].ToString().Trim(),
                        Email = row["Email"].ToString().Trim(),
                        Avatar = row["Avatar"].ToString().Trim(),
                        Courses = getCourse(Convert.ToInt32(row["IDLecturer"].ToString().Trim())),
                        Quizs = GetQuizzes(Convert.ToInt32(row["IDLecturer"].ToString().Trim())),
                        Password = row["Password"].ToString().Trim(),
                        Phone = row["Phone"].ToString().Trim()
                        // Gán các giá trị khác của object từ các cột trong DataTable
                    };

                    _lecturers.Add(obj);
                }
            }
            _lecturers.Reverse();
        }
        public bool login(AccountLogin acc)
        {
            string query = "SELECT * FROM tblLecturer WHERE Email = @Email AND Password = @pass";
            DataTable tb = ConectionData.ExecuteQuery(query, new object[] { acc.Email, acc.Password });
            foreach (DataRow row in tb.Rows)
            {
                UserLogin obj = new UserLogin()
                {
                    Id = Convert.ToInt32(row["IDLecturer"].ToString().Trim()),
                    FirstName = row["FirstName"].ToString().Trim(),
                    LastName = row["LastName"].ToString().Trim(),
                    RoleID = row["RoleID"].ToString().Trim(),
                    Password = row["Password"].ToString().Trim(),
                    Email = row["Email"].ToString().Trim(),
                    Phone = row["Phone"].ToString().Trim(),
                    Avatar = row["Avatar"].ToString().Trim(),
                    Courses = getCourse(Convert.ToInt32(row["IDLecturer"].ToString().Trim())),
                };
                UserLogin.Instance = obj;
            }
            if (tb.Rows.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public List<Course> getCourse(int id)
        {
            List<Course> courses = new List<Course>();
            string query = "SELECT * FROM tblCourse WHERE IDLecturer = @id";
            DataTable dataTable = ConectionData.ExecuteQuery(query, new object[] { id });
            foreach (DataRow row in dataTable.Rows)
            {
                Course obj = new Course()
                {
                    Id = Convert.ToInt32(row["IDCourse"].ToString().Trim()),
                    Title = row["Title"].ToString().Trim(),
                    Detail = row["Detail"].ToString().Trim(),
                    Status = bool.Parse(row["Status"].ToString().Trim()),
                    Price = float.Parse(row["Price"].ToString().Trim()),
                    Sale = float.Parse(row["Sale"].ToString().Trim())
                };

                courses.Add(obj);
            }
            return courses;
        }
        public List<Quiz> GetQuizzes(int id)
        {
            List<Quiz> quizzes = new List<Quiz>();
            string query = "SELECT * FROM tblLecturer";
            DataTable dataTable = ConectionData.ExecuteQuery(query);
            return quizzes;
        }
        public bool editprofile(Profile pro)
        {
            int ID = UserLogin.Instance.Id;
            string query = "UPDATE tblLecturer " +
                " \nSET FirstName = @FirstName , LastName = @LastName , Password = @Password , Phone = @Phone , Email = @Email" +
                " \nWHERE IDlecturer = @IDlecturer";
            bool result = ConectionData.ExecuteUpdate(query, new object[] { pro.FirstName, pro.LastName, pro.Password, pro.Phone, pro.Email, ID });
            return result;
        }
        public void Update()
        {
            _lecturers.Clear();
            getLecturer();
        }
        public bool resetPassword(Profile pro)
        {
            bool result = false;
            string query = "UPDATE tblLecturer SET Password = @pass WHERE Email = @Email ";
            result = ConectionData.ExecuteUpdate(query, new object[] { pro.Password, pro.Email });
            return result;
        }
        public bool removeLecturer(int id)
        {
            string query = "DELETE FROM tblLecturer WHERE IDLecturer = @IDlecturer ";
            bool result = ConectionData.ExecuteUpdate(query, new object[] { id });
            return result;
        }
    }
}
