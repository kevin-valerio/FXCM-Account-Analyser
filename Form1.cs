using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace FXCM_Analysis {

    public partial class Form1 : Form {

        public static List<Trade> trades = new List<Trade>();
        public const double LIMITE_TRADE_NON_BREAKEVEN = 1.75;

        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            trades = new List<Trade>();
            string[] splittedText = richTextBox1.Text.Split('\n');
            foreach (string line in splittedText) {
                Trade trade = new Trade();
                string[] tradeTxt = line.Split('\t');

                for (int i = 0; i < tradeTxt.Length; ++i) {

                    switch (i) {
                        case 0:
                            trade.Date = tradeTxt[i];
                            break;
                        case 1:
                            trade.Type = tradeTxt[i];
                            break;
                        case 2:
                            trade.Description = tradeTxt[i];
                            break;
                        case 3:
                            trade.Account = Convert.ToUInt64(tradeTxt[i]);
                            break;
                        case 4:
                            trade.Ticket = tradeTxt[i];
                            break;
                        case 5:
                            trade.GainOrLoss = float.Parse(tradeTxt[i], CultureInfo.InvariantCulture.NumberFormat);
                            break;
                        case 6:
                            trade.FinalBalance = float.Parse(tradeTxt[i], CultureInfo.InvariantCulture.NumberFormat);
                            break;
                        default:
                            break;
                    }             
                }
                trades.Add(trade);
            }
            getTradeInfos();
        }

        private void button2_Click(object sender, EventArgs e) {
            Form2 form = new Form2(); 
            form.Show();
        }
        private void getTradeInfos() {
            int breakEven = 0, tradeNumber = 0, tradeGagnant = 0, tradePerdant = 0;
            foreach(Trade trade in trades) {
               
                if(trade.Type == "PnL") {
                    tradeNumber++;
                    if (trade.GainOrLoss < LIMITE_TRADE_NON_BREAKEVEN && trade.GainOrLoss > -LIMITE_TRADE_NON_BREAKEVEN) {
                        breakEven++;
                    }
                    if (trade.GainOrLoss < -LIMITE_TRADE_NON_BREAKEVEN) {
                        tradePerdant++;
                    }
                    if (trade.GainOrLoss > LIMITE_TRADE_NON_BREAKEVEN) {
                        tradeGagnant++;
                    }
                }
            }
            label1.Text =  breakEven.ToString();
            label2.Text =   tradeNumber.ToString();
            label3.Text =   tradePerdant.ToString();
            label4.Text =   tradeGagnant.ToString();
            label5.Text =   getRR().ToString();
            if (checkBox1.Checked) {
               DrawPieChart(tradeGagnant, tradePerdant, breakEven);
            } else {
                DrawPieChart(tradeGagnant, tradePerdant, 0);
            }

        }

        private double getRR() {
            //R/R = moyenneGagnant / moyennePerdants
            double moyenneGagnante = 0, moyennePerdante = 0, nombreGagnants = 0, nombrePerdant = 0;

            foreach (Trade trade in trades) {
                if (trade.Type == "PnL") {
                    if (!checkBox1.Checked) {
                        if (trade.GainOrLoss < -1.75) {
                            ++nombrePerdant;
                            moyennePerdante += trade.GainOrLoss;
                        }
                        if (trade.GainOrLoss > 1.75) {
                            ++nombreGagnants;
                            moyenneGagnante += trade.GainOrLoss;
                        }
                    } else {
                        if (trade.GainOrLoss < 0) {
                            ++nombrePerdant;
                            moyennePerdante += trade.GainOrLoss;
                        }
                        if (trade.GainOrLoss > 0) {
                            ++nombreGagnants;
                            moyenneGagnante += trade.GainOrLoss;
                        }
                    }
                }
            }
            return -(moyenneGagnante / nombreGagnants) / (moyennePerdante / nombrePerdant);
        }

        private void DrawPieChart(int gagnants, int perdants, int breakeven) {
            chart1.Series.Clear();
            chart1.Legends.Clear();
            chart1.Legends.Add("MyLegend");
            chart1.Legends[0].LegendStyle = LegendStyle.Table;
            chart1.Legends[0].Docking = Docking.Bottom;
            chart1.Legends[0].Alignment = StringAlignment.Center;
            chart1.Legends[0].BorderColor = Color.Black;
            string seriesname = "Trades";
            chart1.Series.Add(seriesname);
            chart1.Series[seriesname].ChartType = SeriesChartType.Pie;
            chart1.Series[seriesname].Points.AddXY("Trades gagnants", gagnants);
            chart1.Series[seriesname].Points.AddXY("Trades perdants", perdants);
            chart1.Series[seriesname].Points.AddXY("Trades breakeven", breakeven);
            foreach (DataPoint p in chart1.Series["Trades"].Points) {
                p.Label = "#PERCENT\n#VALX";
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("chrome.exe", "https://www.myfxcm.com/fxma/account-summary?next=%2Faccount-summary#custAccountSummary");
        }
    }
}
