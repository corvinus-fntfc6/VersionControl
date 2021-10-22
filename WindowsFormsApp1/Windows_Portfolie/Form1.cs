using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Windows_Portfolie
{
    public partial class Form1 : Form
    {
        PortfolioEntities context = new PortfolioEntities();
        List<Tick> Ticks;
        List<PortfolioItem> Portfolio = new List<PortfolioItem>();
        List<decimal> profitsOrdered = new List<decimal>();

        public Form1()
        {
            InitializeComponent();
            Ticks = context.Ticks.ToList();
            dataGridView1.DataSource = Ticks;
            CreatePortfolio();

            List<decimal> profits = new List<decimal>();
            int interval = 30;
            DateTime startingDate = (from x in Ticks select x.TradingDay).Min();
            DateTime endDate = new DateTime(2016, 12, 30);
            TimeSpan z = endDate - startingDate;

            for (int i = 0; i < z.Days - interval; i++)
            {
                decimal ny = GetPortfolioValue(startingDate.AddDays(i + interval))
                           - GetPortfolioValue(startingDate.AddDays(i));
                profits.Add(ny);
                Console.WriteLine(i + " " + ny);
            }

            this.profitsOrdered = (from x in profits orderby x select x).ToList();
            MessageBox.Show(this.profitsOrdered[this.profitsOrdered.Count() / 5].ToString());
        }

        private void CreatePortfolio()
        {
            Portfolio.Add(new PortfolioItem() { Index = "OTP", Volume = 10 });
            Portfolio.Add(new PortfolioItem() { Index = "ZWACK", Volume = 10 });
            Portfolio.Add(new PortfolioItem() { Index = "ELMU", Volume = 10 });

            dataGridView2.DataSource = Portfolio;
        }
        private decimal GetPortfolioValue(DateTime date)
        {
            decimal value = 0;
            foreach (var item in Portfolio)
            {
                var last = (from x in Ticks
                            where item.Index == x.Index.Trim()
                            && date <= x.TradingDay
                            select x)
                            .First();
                value += (decimal)last.Price + item.Volume;
            }
            return value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Szöveges file|*.txt";
            dialog.Title = "Profitok elmentése";
            dialog.FileName = "profits.txt";

            if (dialog.ShowDialog() == DialogResult.OK && dialog.FileName != "")
            {
                using (System.IO.StreamWriter w = new System.IO.StreamWriter(dialog.FileName))
                {
                    foreach (decimal d in this.profitsOrdered)
                    {
                        w.WriteLine(d);
                    }
                }

                MessageBox.Show("Mentés sikeres!");
            }
        }
    }
}
