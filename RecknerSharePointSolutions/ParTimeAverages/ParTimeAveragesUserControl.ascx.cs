﻿using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using RecknerSharePointSolutions.ExtensionMethods;

namespace RecknerSharePointSolutions.ParTimeAverages
{
    public partial class ParTimeAveragesUserControl : UserControl
    {

        public ParTimeAverages ThisWebPart { get; set; }

        decimal cellValue = 0;
                            
        protected void Page_Load(object sender, EventArgs e)
        {
            this.ThisWebPart = this.Parent as ParTimeAverages;
            
            if (!Page.IsPostBack) {
 

                if (ThisWebPart.BackToMonths > 0) {

                    ThisWebPart.StartDate = DateTime.Now.AddMonths(ThisWebPart.BackToMonths * -1);
                    
                }
 
                var sourceTable = GetData("proback", ThisWebPart.StartDate);
                var pivotTable = GeneratePivotTable(sourceTable, ThisWebPart.StartDate);
                   pivotTable.Sort = ThisWebPart.SortExpression;
                   pivotTable.RowFilter = ThisWebPart.FilterExpression;
                
                   GridView1.DataSource = pivotTable;
                   GridView1.DataBind();
                 
             }
        }

        public static DataView GeneratePivotTable(DataTable sourceTable, DateTime startDate)
        {
            var pivotTable = new DataTable();
            GeneratePivotColumns(pivotTable, startDate);
            GeneratePivotRows(sourceTable, pivotTable);
            return pivotTable.AsDataView();

        }

        private static void GeneratePivotColumns(DataTable pivotTable,  DateTime startDate)
        {
            pivotTable.Columns.Add("Location");
            pivotTable.Columns.Add("Employee");
            pivotTable.Columns.Add("Averages");

            foreach (var weekNumbers in  startDate.EnumarateWeekNumbers(DateTime.Now.AddDays(-5)))
            {
                pivotTable.Columns.Add(weekNumbers);
            }
 
        
        }

        private static void GeneratePivotRows(DataTable sourceTable, DataTable pivotTable)
        {
            var employeesWithLocation = (from employee in sourceTable.AsEnumerable()
                                         select new
                                         {
                                             Location = employee.Field<string>("Location").Trim(),
                                             Name = employee.Field<string>("Name").Trim()

                                         }).Distinct();

            var columnName = "";


            foreach (var employee in employeesWithLocation.OrderBy(x => x.Location))
            {

                var pivotRow = pivotTable.NewRow();
                pivotRow["Location"] = employee.Location;
                pivotRow["Employee"] = employee.Name;

                for (var i = 3; i < pivotTable.Columns.Count  ; i++)
                {
                    columnName = pivotTable.Columns[i].ColumnName;
                     
                    pivotRow[columnName] = GetEmployeeHoursByWeek(sourceTable, employee.Name.Trim(), columnName.Trim()); ;

                }

               
               
                pivotRow.Compute("avg", "Averages", 3, pivotTable.Columns.Count, 2);

                pivotTable.Rows.Add(pivotRow);

            }

        }

        private static DataTable GetData(string connectrionStringName, DateTime startDate)
        {
            var cnString = System.Configuration.ConfigurationManager.ConnectionStrings[connectrionStringName].ToString();
            var ds = new DataSet();

            using (var cn = new SqlConnection(cnString))
            {
                using (var cmd = new SqlCommand("SP_Get_Parttime_Employee_Hours", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@StartDate", SqlDbType.SmallDateTime);
                    cmd.Parameters[0].Value = startDate;
                    
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }

                }
            }

            return ds.Tables[0];

        }
        
        private static double GetEmployeeHoursByWeek(DataTable sourceTable, string employeeName, string weekNum)
        {
            double employeeHour = 0;

            var r =
              sourceTable.AsEnumerable().SingleOrDefault(x => x.Field<string>("Name").Trim() == employeeName.Trim() &&
                                                       x.Field<string>("WeekNum").Trim() == weekNum.Trim());

            if (r != null)
            {
                
                 double.TryParse(r["hours"].ToString(), out employeeHour);

            }

            return Math.Round(employeeHour, 2);
        }

        int cellIndex = 0;

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            foreach (TableCell item in e.Row.Cells)
            {
                cellIndex += 1;

                if (cellIndex == 3)
                {

                    item.BackColor = System.Drawing.Color.SteelBlue;
                    item.ForeColor = System.Drawing.Color.White;
                    
                }


                Decimal.TryParse(item.Text, out cellValue);
                 
                if (cellValue >= ThisWebPart.TotalHoursTreshold) {

                    item.BackColor = System.Drawing.Color.Red;
                    item.ForeColor = System.Drawing.Color.White;
                    item.Wrap = false;

                    
                                    
                }

             
                                
            }

            cellIndex = 0;

        }

   

     



    }
}
