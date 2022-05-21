using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PointOfSale.Utility
{
    public static class ExportExcelReport
    {
        

        public static ExcelWorksheet GetExcelWorksheet<T>(ExcelPackage package, List<T> genericdataList)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet");
            int cellIndex = 1;// create table head
            if (genericdataList.Count > 0)
            {
                var cellsHead = genericdataList.FirstOrDefault().GetType().GetProperties();
                foreach (var prop in cellsHead)
                {
                    worksheet.Cells[1, cellIndex].Value = PropertyName(prop.Name);
                    if ((prop.PropertyType.Name.ToLower() == "string") && (prop.Name == "Cod" || prop.Name == "Payment" || prop.Name == "Total" || prop.Name == "TotalPayment" || prop.Name == "Amount" || prop.Name == "TotalAmount" || prop.Name == "UnitPrice" || prop.Name == "Price" || prop.Name == "SalePrice"))
                    {
                        worksheet.Column(cellIndex).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    cellIndex++;
                }
                worksheet.Cells["A1:Z1"].Style.Font.Bold = true;
                worksheet.View.FreezePanes(2, 1);
                //
                // Inserting data
                //  worksheet.Cells["A2"].LoadFromCollection(genericdataList);
                DataInsertion(worksheet, genericdataList);
                worksheet.Cells.AutoFitColumns();
            }
            else
            {
                worksheet.Cells["A2:D2"].Value = "No data available in table";
                worksheet.Cells["A2:D2"].Merge = true;
                worksheet.Cells["A2:D2"].Style.Font.Bold = true;
                worksheet.Cells["A2:D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            return worksheet;
        }

        public static ExcelWorksheet GetExcelWorksheet<T>(ExcelPackage package, List<T> genericdataList, string workSheetName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(workSheetName);
            int cellIndex = 1;// create table head
            if (genericdataList.Count > 0)
            {
                var cellsHead = genericdataList.FirstOrDefault().GetType().GetProperties();
                foreach (var prop in cellsHead)
                {
                    worksheet.Cells[1, cellIndex].Value = PropertyName(prop.Name);
                    if ((prop.PropertyType.Name.ToLower() == "string") && (prop.Name == "Cod" || prop.Name == "Payment" || prop.Name == "Total" || prop.Name == "TotalPayment" || prop.Name == "Amount" || prop.Name == "TotalAmount" || prop.Name == "UnitPrice" || prop.Name == "Price" || prop.Name == "SalePrice"))
                    {
                        worksheet.Column(cellIndex).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    cellIndex++;
                }
                worksheet.Cells["A1:Z1"].Style.Font.Bold = true;
                worksheet.View.FreezePanes(2, 1);
                //
                //Inserting data
                worksheet.Cells["A2"].LoadFromCollection(genericdataList);
                worksheet.Cells.AutoFitColumns();
            }
            else
            {
                worksheet.Cells["A2:D2"].Value = "No data available in table";
                worksheet.Cells["A2:D2"].Merge = true;
                worksheet.Cells["A2:D2"].Style.Font.Bold = true;
                worksheet.Cells["A2:D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            return worksheet;
        }

        private static string PropertyName(string name)
        {
            if (name.ToLower() == "SNO".ToLower()) return name = "S.No";

            // split the string by upper-case and provide space
            return Regex.Replace(name, "([A-Z0-9])", " $1").Trim();
        }

        public static Byte[] GetExcelFile<T>(List<T> ts)
        {
            byte[] fileContents;
            using (var package = new ExcelPackage())
            {
                ExportExcelReport.GetExcelWorksheet(package, ts);
                fileContents = package.GetAsByteArray();
            }
            return fileContents;
        }

        private static ExcelWorksheet DataInsertion<T>(ExcelWorksheet worksheet, List<T> list)
        {
            int row = 2;
            string[] dataTypeInt = { "int", "int32", "int64" };
            string[] dataTypeDecimal = { "decimal", "double", "float" };
            string[] headName = { "id", "num", "number", "s.no", "sno", "rno", "awb" };
            for (int i = 0; i < list.Count; i++)
            {
                var dataProperties = list[i].GetType().GetProperties();
                int colIndex = 1;
                foreach (var prop in dataProperties)
                {
                    string propertyTypeName = prop.PropertyType.Name.ToLower();

                    if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var propInfo = prop.PropertyType.GetGenericArguments()[0];
                        propertyTypeName = propInfo.Name.ToLower();
                    }

                    if (headName.Any(x => prop.Name.ToLower().Contains(x)) || propertyTypeName == "string" || propertyTypeName == "bool")
                    {
                        worksheet.Cells[row, colIndex].Value = prop.GetValue(list[i]);
                    }
                    else
                    {
                        if (dataTypeInt.Contains(propertyTypeName))
                        {
                            worksheet.Cells[row, colIndex].Value = prop.GetValue(list[i]);
                            worksheet.Cells[row, colIndex].Style.Numberformat.Format = "#,##0";
                        }
                        else if (dataTypeDecimal.Contains(propertyTypeName))
                        {
                            worksheet.Cells[row, colIndex].Value = prop.GetValue(list[i]);
                            worksheet.Cells[row, colIndex].Style.Numberformat.Format = "#,##0.00";                                                                                         // worksheet.Cells[row, colIndex].Value = //worksheet.Cells[row, colIndex].Value;
                        }
                        else
                        {
                            worksheet.Cells[row, colIndex].Value = prop.GetValue(list[i]);
                        }
                    }
                    colIndex++;
                }
                row++;
            }
            return worksheet;
        }
    }
}
