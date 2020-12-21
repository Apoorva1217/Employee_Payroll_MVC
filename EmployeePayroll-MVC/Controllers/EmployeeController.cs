using EmployeePayroll_MVC.Models;
using EmployeePayroll_MVC.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeePayroll_MVC.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Ability to add multiple Employees to the Employee Payroll DB
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RegisterEmployee(RegisterEmpRequestModel employee)
        {
            bool result = false;
            if (ModelState.IsValid)
            {
                result = this.RegisterEmployeeService(employee);
            }
            ModelState.Clear();

            if (result == true)
            {
                return RedirectToAction("EmployeeList");
            }
            return View("Register", employee);
        }
        public bool RegisterEmployeeService(RegisterEmpRequestModel employee)
        {
            try
            {
                Employee validEmployee = db.Employees.Where(x => x.Name == employee.Name && x.Gender == employee.Gender).FirstOrDefault();
                if (validEmployee == null)
                {
                    int departmentId = db.Departments.Where(x => x.DeptName == employee.Department).Select(x => x.DeptId).FirstOrDefault();
                    Employee newEmployee = new Employee()
                    {
                        Name = employee.Name,
                        Gender = employee.Gender,
                        DepartmentId = departmentId,
                        SalaryId = Convert.ToInt32(employee.SalaryId),
                        StartDate = employee.StartDate,
                        Description = employee.Description
                    };
                    Employee returnData = db.Employees.Add(newEmployee);
                }
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Retrieve single Employee in Employee Payroll DB
        /// </summary>
        /// <returns></returns>
        public ActionResult EmployeeList()
        {
            List<EmployeeDetailModel> list = GetAllEmployee();
            return View(list);
        }
        public List<EmployeeDetailModel> GetAllEmployee()
        {
            try
            {
                List<EmployeeDetailModel> list = (from e in db.Employees
                                                  join d in db.Departments on e.DepartmentId equals d.DeptId
                                                  join s in db.Salaries on e.SalaryId equals s.SalaryId
                                                  select new EmployeeDetailModel
                                                  {
                                                      EmpId = e.EmpId,
                                                      Name = e.Name,
                                                      Gender = e.Gender,
                                                      DepartmentId = d.DeptId,
                                                      Department = d.DeptName,
                                                      SalaryId = s.SalaryId,
                                                      Amount = s.Amount,
                                                      StartDate = e.StartDate,
                                                      Description = e.Description
                                                  }).ToList<EmployeeDetailModel>();

                return list;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Ability to Edit Employee in Employee Payroll DB
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult Edit(EmployeeDetailModel data)
        {
            RegisterEmpRequestModel emp = new RegisterEmpRequestModel
            {
                EmpId = data.EmpId,
                Name = data.Name,
                Gender = data.Gender,
                Department = data.Department,
                SalaryId = data.SalaryId.ToString(),
                StartDate = data.StartDate,
                Description = data.Description
            };

            return View(emp);
        }
        public ActionResult EditEmployee(RegisterEmpRequestModel employee)
        {
            bool result = EditEmployeeService(employee);
            if (result == true)
            {
                List<EmployeeDetailModel> list = GetAllEmployee();
                return View("EmployeeList", list);
            }
            else
            {
                return View("Error");
            }
        }
        public bool EditEmployeeService(RegisterEmpRequestModel employee)
        {
            try
            {
                int departmentId = db.Departments.Where(x =>
                                                     x.DeptName == employee.Department)
                                                    .Select(x => x.DeptId).FirstOrDefault();

                Employee emp = db.Employees.Find(employee.EmpId);
                emp.Name = employee.Name;
                emp.SalaryId = Convert.ToInt32(employee.SalaryId);
                emp.StartDate = employee.StartDate;
                emp.Description = employee.Description;
                emp.Gender = employee.Gender;
                emp.DepartmentId = departmentId;

                int result = db.SaveChanges();
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}