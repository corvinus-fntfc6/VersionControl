using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        RealEstateEntities context = new RealEstateEntities();
        List<Flat> Flats;

        Excel.Application xlApp;
        Excel.Workbook xlWB;
        Excel.Worksheet xlSheet;

        public Form1()
        {            
            InitializeComponent();
            LoadData();
        }

        public void LoadData()
        {
            Flats = context.Flats.ToList();
        }

        public void CreateExcel()
        {
            try
            {
                xlApp = new Excel.Application();
                xlWB = xlApp.Workbooks.Add(Missing.Value);
                xlSheet = xlWB.ActiveSheet;
                CreateTable();
                xlApp.Visible = true;
                xlApp.UserControl = true;
            }
            catch (Exception ex)
            {
                string hiba = string.Format("Error:¸{0}\nLine:{1}", ex.Message, ex.Source);
                xlWB.Close(false, Type.Missing, Type.Missing);
                xlApp.Quit();
                xlWB = null;
                xlApp = null;
            }
        }

        public void CreateTable()
        {
            string[] headers = new string[]
            {
                "kód",
                "Eladó",
                 "Oldal",
                "Kerület",
                "Lift",
                 "Szobák száma",
                "Alapterület (m2)",
                "Ár (mFt)",
                "Négyzetméter ár (Ft/m2)"
            };
            for (int i = 0; i < headers.Length; i++)
            {
                xlSheet.Cells[1, i + 1] = headers[i];

            }
            object[,] values = new object[Flats.Count, headers.Length];
            int counter = 0;
            foreach (Flat f in Flats)
            {
                values[counter, 0] = f.Code;
                values[counter, 1] = f.Vendor;
                values[counter, 2] = f.Side;
                values[counter, 3] = f.District;
                values[counter, 4] = f.Elevator;
                values[counter, 5] = f.NumberOfRooms;
                values[counter, 6] = f.FloorArea;
                values[counter, 7] = f.Price;
                values[counter, 8] = "=" + GetCell(counter + 2, 8) + "*1000000/" + GetCell(counter + 2, 7);
                counter++;

            }
            xlSheet.get_Range(
              GetCell(2, 1),
              GetCell(1 + values.GetLength(0), values.GetLength(1))).Value2 = values;
            Excel.Range headerRange = xlSheet.get_Range(GetCell(1, 1), GetCell(1, headers.Length));
            headerRange.Font.Bold = true;
            headerRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            headerRange.EntireColumn.AutoFit();
            headerRange.RowHeight = 40;
            headerRange.Interior.Color = Color.LightBlue;
            headerRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);

            Excel.Range tableRange = xlSheet.get_Range(GetCell(2, 1), GetCell(1 + values.GetLength(0), values.GetLength(1)));
            tableRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);

            Excel.Range firstColumn = xlSheet.get_Range(GetCell(2, 1), GetCell(1 + values.GetLength(0), 1));
            firstColumn.Font.Bold = true;
            firstColumn.Interior.Color = Color.LightYellow;

            Excel.Range lastColumn = xlSheet.get_Range(GetCell(2, values.GetLength(1)), GetCell(1 + values.GetLength(0), values.GetLength(1)));
            lastColumn.Interior.Color = Color.Green;
            lastColumn.NumberFormat = "###,###.00";
        }

        private string GetCell(int x, int y)
        {
            string ExcelKoordinata = "";
            int divident = y;
            int modulo;
            while (divident > 0)
            {
                modulo = (divident - 1) % 26;
                ExcelKoordinata = Convert.ToChar(65 + modulo).ToString() + ExcelKoordinata;
                divident = (int)((divident - modulo) / 26);

            }
            ExcelKoordinata += x.ToString();
            return ExcelKoordinata;
        }
    }
}
