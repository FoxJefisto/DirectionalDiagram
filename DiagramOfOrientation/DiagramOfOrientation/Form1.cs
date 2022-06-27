using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using OxyPlot;
using OxyPlot.Series;
using MathNet.Numerics.Distributions;
using System.Numerics;
using OxyPlot.Axes;
using OxyPlot.Legends;
using System.Reflection;

namespace DiagramOfOrientation
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            double lbd = 0.25;
            int l = 8;
            int n = 64;
            double[] d = Linspace(0, 8, n);
            DrawRuler(this, n, d);
            var Dssh = Shum(6, d, lbd);
            DrawShums(this, n, Dssh);
            double[] xTheta = Linspace(-90, 90, 180);
            var yAM1 = AM1Vector(xTheta);
            DrawAM1(this, xTheta, yAM1);
            xTheta = Linspace(-90, 90, 4000);
            var DNL = DN1LReal(d, lbd, xTheta, 10);
            var DNLShum = DN1LShumReal(d, lbd, xTheta, 10, Dssh);
            DrawDiagramOfOrientation(this, xTheta, DNL, new Point(50, 800), "Диаграмма направленности без шумов");
            DrawDiagramOfOrientation(this, xTheta, DNLShum, new Point(50, 1200), "Диаграмма направленности c шумами");
        }

        public static void DrawRuler(Form form, int n, double[] d)
        {
            var pv = new OxyPlot.WindowsForms.PlotView();
            pv.Location = new Point(50, 0);
            pv.Size = new Size(1000, 200);
            form.Controls.Add(pv);
            pv.Model = new PlotModel { Title = "Линейка" };
            DataPoint[] dp = new DataPoint[n];
            for (int i = 0; i < n; i++)
            {
                dp[i] = new DataPoint(d[i], 0);
            }
            FunctionSeries fs = new FunctionSeries();
            fs.Points.AddRange(dp);
            fs.MarkerType = MarkerType.Diamond;
            pv.Model.Series.Add(fs);
        }

        public static void DrawShums(Form form, int n, Complex[] Dssh)
        {
            var pv = new OxyPlot.WindowsForms.PlotView();
            pv.Location = new Point(50, 200);
            pv.Size = new Size(1000, 200);
            form.Controls.Add(pv);
            pv.Model = new PlotModel { Title = "Шумы" };
            var bi1 = new DataPoint[64];
            var bi2 = new DataPoint[64];
            for (int i = 0; i < n; i++)
            {
                bi1[i] = new DataPoint(i, Dssh[i].Real);
                bi2[i] = new DataPoint(i, Dssh[i].Imaginary);
            }
            var fs1 = new FunctionSeries();
            var fs2 = new FunctionSeries();
            fs1.Points.AddRange(bi1);
            fs2.Points.AddRange(bi2);
            fs1.MarkerType = MarkerType.Diamond;
            fs1.Title = "Re(Dssh)";
            fs2.MarkerType = MarkerType.Diamond;
            fs2.Title = "Im(Dssh)";
            pv.Model.Legends.Add(new Legend()
            {
                LegendPosition = LegendPosition.RightTop,
                LegendPlacement = LegendPlacement.Outside
            });
            var YAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = AxisPosition.Left,
                PositionAtZeroCrossing = true,
                Minimum = -7,
                Maximum = 7,
                MajorStep = 2,
                MinorStep = 1,
                ExtraGridlines = new double[] { 0 },
                ExtraGridlineStyle = LineStyle.Dot
            };
            var XAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = -5,
                Maximum = 64,
                MajorStep = 10,
                MinorStep = 1,
                ExtraGridlines = new double[] { 0 },
                ExtraGridlineStyle = LineStyle.Dot
            };
            fs1.YAxisKey = YAxis.Key;
            fs2.YAxisKey = YAxis.Key;
            fs1.XAxisKey = XAxis.Key;
            fs2.XAxisKey = XAxis.Key;
            pv.Model.Series.Add(fs1);
            pv.Model.Series.Add(fs2);
            pv.Model.Axes.Add(YAxis);
            pv.Model.Axes.Add(XAxis);
        }

        public static void DrawAM1(Form form, double[] x, double[] y)
        {
            var pv = new OxyPlot.WindowsForms.PlotView();
            pv.Location = new Point(50, 400);
            pv.Size = new Size(600, 400);
            form.Controls.Add(pv);
            pv.Model = new PlotModel { Title = "Диаграмма направленности одного излучателя" };
            var bs = new DataPoint[x.Length];
            for (int i = 0; i < bs.Length; i++)
            {
                bs[i] = new DataPoint(x[i], y[i]);
            }
            var fs = new FunctionSeries();
            var YAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = AxisPosition.Left,
                PositionAtZeroCrossing = true,
                Minimum = 0,
                Maximum = 1.1,
                MajorStep = 0.2,
                MinorStep = 0.1,
                ExtraGridlines = new double[] { 0 },
                ExtraGridlineStyle = LineStyle.Dot
            };
            var XAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = -100,
                Maximum = 100,
                MajorStep = 25,
                MinorStep = 5,
                ExtraGridlines = new double[] { 0 },
                ExtraGridlineStyle = LineStyle.Dot
            };
            fs.XAxisKey = XAxis.Key;
            fs.YAxisKey = YAxis.Key;
            fs.Points.AddRange(bs);
            pv.Model.Series.Add(fs);
            pv.Model.Axes.Add(XAxis);
            pv.Model.Axes.Add(YAxis);
        }

        public static void DrawDiagramOfOrientation(Form form, double[] x, double[] y, Point location, string title)
        {
            var pv = new OxyPlot.WindowsForms.PlotView();
            pv.Location = location;
            pv.Size = new Size(600, 400);
            form.Controls.Add(pv);
            pv.Model = new PlotModel { Title = title };
            var bs = new DataPoint[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                bs[i] = new DataPoint(x[i], y[i]);
            }
            var fs = new FunctionSeries();
            var YAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = -80,
                Maximum = 0,
                MajorStep = 10,
                MinorStep = 5,
                ExtraGridlines = new double[] { 0 },
                ExtraGridlineStyle = LineStyle.Dot
            };
            var XAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = -100,
                Maximum = 100,
                MajorStep = 25,
                MinorStep = 5,
                ExtraGridlines = new double[] { 0 },
                ExtraGridlineStyle = LineStyle.Dot
            };
            fs.XAxisKey = XAxis.Key;
            fs.YAxisKey = YAxis.Key;
            fs.Points.AddRange(bs);
            pv.Model.Series.Add(fs);
            pv.Model.Axes.Add(XAxis);
            pv.Model.Axes.Add(YAxis);
        }

        public static double[] Linspace(double start, double stop, int num)
        {
            double step = (stop - start) / (num - 1);
            double[] y = new double[num];
            for (int i = 0; i < num; i++)
            {
                y[i] = start + step * i;
            }

            return y;
        }

        public static Complex[] Shum(int N, double[] d, double lbd)
        {
            var GSHUp = new List<double[]>();
            for (int i = 0; i < N; i++)
            {
                var y = new double[64];
                Normal.Samples(y, 0, 1.0);
                GSHUp.Add(y);
            }
            var ReASh = new double[64];
            var ImASh = new double[64];
            for (int i = 0; i < 64; i++)
            {
                ReASh[i] = GSHUp[0][i] + GSHUp[1][i] + GSHUp[2][i];
                ImASh[i] = GSHUp[3][i] + GSHUp[4][i] + GSHUp[5][i];
            }
            var ASh = new Complex[64];
            for (int i = 0; i < 64; i++)
            {
                ASh[i] = new Complex(ReASh[i], ImASh[i]);
            }
            double k = 2 * Math.PI / lbd;
            var DRL = new Complex[64];
            for (int i = 0; i < 64; i++)
            {
                DRL[i] = ASh[i] + Complex.Exp(Complex.ImaginaryOne * k * 0.01 * d[i] * Complex.Sin(10 * Math.PI / 180));
            }
            return DRL;
        }

        public static double AM1(double theta, double Theta = 90)
        {
            double k = Math.Pow((2.78 * theta / Theta), 2);
            if (k != 0)
            {
                return Math.Sin(Math.Sqrt(k)) / Math.Sqrt(k);
            }
            else
            {
                return 1;
            }
        }

        public static double[] AM1Vector(double[] theta, double Theta = 90)
        {
            return theta.Select(x => AM1(x)).ToArray();
        }

        public static Complex[] AR1(double[] d, double lbd, double[] theta, double theta1)
        {
            double k = 2 * Math.PI / lbd;
            var x = new Complex[theta.Length];
            for(int j = 0; j < theta.Length; j++)
            {
                for (int i = 0; i < d.Length; i++)
                {
                    x[j] += Complex.Exp(-Complex.ImaginaryOne * k * d[i] * Complex.Sin((theta[j] - theta1) * Math.PI / 180));
                }
            }
            return x;
        }

        public static Complex[] DN1L(double[] d, double lbd, double[] theta, double theta1)
        {
            var Ar = AR1(d, lbd, theta, theta1);
            var Am = AM1Vector(theta);
            var DNL = new Complex[theta.Length];
            for (int i = 0; i < theta.Length; i++)
            {
                DNL[i] = 10 * Complex.Log10(Am[i] * Ar[i] * Complex.Conjugate(Ar[i]) / Complex.Pow(d.Length, 2));
            }
            return DNL;
        }

        public static double[] DN1LReal(double[] d, double lbd, double[] theta, double theta1)
        {
            return DN1L(d, lbd, theta, theta1).Select(s => s.Real).ToArray();
        }

        public static double[] DN1LShumReal(double[] d, double lbd, double[] theta, double theta1, Complex[] Dssh)
        {
            return DN1LShum(d, lbd, theta, theta1, Dssh).Select(s => s.Real).ToArray();
        }
        public static Complex[] AR1Shum(double[] d, double lbd, double[] theta, double theta1, Complex[] Dssh)
        {
            double k = 2 * Math.PI / lbd;
            var x = new Complex[theta.Length];
            for (int j = 0; j < theta.Length; j++)
            {
                for (int i = 0; i < d.Length; i++)
                {
                    x[j] += (1 + Dssh[i]) * Complex.Exp(-Complex.ImaginaryOne * k * d[i] * Complex.Sin((theta[j] - theta1) * Math.PI / 180));
                }
            }
            return x;
        }

        public static Complex[] DN1LShum(double[] d, double lbd, double[] theta, double theta1, Complex[] Dssh)
        {
            var Ar = AR1Shum(d, lbd, theta, theta1, Dssh);
            var Am = AM1Vector(theta);
            var DNL = new Complex[theta.Length];
            for (int i = 0; i < theta.Length; i++)
            {
                DNL[i] = 10 * Complex.Log10(Am[i] * Ar[i] * Complex.Conjugate(Ar[i]) / Complex.Pow(d.Length + 60, 2));
            }
            return DNL;
        }

        public static void PrintArray<T>(T[] array)
        {
            foreach (var x in array)
            {
                Debug.Write($"{x} ");
            }
            Debug.WriteLine("");
        }
    }
}
