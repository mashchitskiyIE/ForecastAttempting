using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZedGraph;

namespace ForecastAttempting
{
    static class GraphicProcessing
    {
        public static double[] OrdinaryLeastSquares(PointPairList DF, int pd)
        {
            double[,] em = new double[pd + 1, pd + 1];
            double[] ev = new double[pd + 1];
            double am;
            PointPairList ans = new PointPairList();
            for (int i = 0; i <= pd; i++)
            {
                am = 0;
                for (int k = 0; k < DF.Count; k++)
                {
                    am += Math.Pow(DF[k].X, i) * DF[k].Y;
                }
                ev[i] = am;
                for (int j = 0; j <= pd; j++)
                {
                    am = 0;
                    for (int k = 0; k < DF.Count; k++)
                    {
                        am += Math.Pow(DF[k].X, i + j);
                    }
                    em[i, j] = am;
                }
            }
            ev = Algebra.GaussMethod(em, ev);
            return ev;
        }

        public static double[,] CubicSplineInterpolation(PointPairList DF)
        {
            int n = DF.Count - 2;
            double[,] em = new double[n, n];
            double[] fv = new double[n];
            double[] h = new double[n + 1];
            double[,] sp = new double[n + 1, 4];
            PointPairList res = new PointPairList();
            for (int i = 0; i < n; i++) for (int j = 0; j < n; j++) em[i, j] = 0;
            for (int i = 0; i < n + 1; i++) h[i] = DF[i + 1].X - DF[i].X;
            for (int i = 0; i < n; i++)
            {
                em[i, i] = 2 * (h[i] + h[i + 1]);
                fv[i] = 6 * ((DF[i + 2].Y - DF[i + 1].Y) / h[i + 1] - (DF[i + 1].Y - DF[i].Y) / h[i]);
            }
            for (int i = 0; i < n - 1; i++) em[i, i + 1] = h[i + 1];
            for (int i = 1; i < n; i++) em[i, i - 1] = h[i];
            fv = Algebra.ThreeDiagonalMatrixAlgorithm(em, fv);
            sp[n, 1] = 0;
            for (int i = 0; i < n; i++) sp[i, 1] = fv[i];
            sp[0, 0] = sp[0, 1] / h[0];
            for (int i = 1; i < n + 1; i++) sp[i, 0] = (sp[i, 1] - sp[i - 1, 1]) / h[i];
            for (int i = 0; i < n + 1; i++) sp[i, 2] = sp[i, 1] * h[i] / 2 - sp[i, 0] * Math.Pow(h[i], 2) / 6 + (DF[i + 1].Y - DF[i].Y) / h[i];
            for (int i = 0; i < n + 1; i++)
            {
                sp[i, 3] = DF[i + 1].Y;
                sp[i, 0] = sp[i, 0] / 6;
                sp[i, 1] = sp[i, 1] / 2;
            }
            return (sp);
        }

        public static double[] CurveCompletion(double[] x, double[] y, int pd, double[] xm, double[] ym)
        {
            int St = -1, End = y.Length;
            for (int i = 0; i < y.Length; i++) if (!double.IsNaN(y[i])) { St = i; break; }
            for (int i = y.Length - 1; i >= 0; i--) if (!double.IsNaN(y[i])) { End = i; break; }
            PointPairList ppl = new PointPairList();
            List<int> md = new List<int>(); //missing data
            for (int i = St; i <= End; i++) if (!double.IsNaN(y[i])) ppl.Add(x[i], y[i]); else md.Add(i);
            if (md.Count > 0)
            {
                double[,] sp = CubicSplineInterpolation(ppl);
                int sn = 0;
                for (int i = 0; i < md.Count; i++)
                {
                    while (x[md[i]] > ppl[sn + 1].X) sn++;
                    y[md[i]] = sp[sn, 0] * Math.Pow(x[md[i]] - ppl[sn + 1].X, 3) + sp[sn, 1] * Math.Pow(x[md[i]] - ppl[sn + 1].X, 2) + sp[sn, 2] * (x[md[i]] - ppl[sn + 1].X) + sp[sn, 3];
                }
            }
            if (St > 0 || End < y.Length - 1) if (xm != null && ym != null) y = Alignment(x, y, xm, ym, St, End); else y = ExtrapolationByOLSApproximation(ppl, x, y, St, End, pd);
            return y;
        }

        public static double[] ExtrapolationByOLSApproximation(PointPairList DF, double[] x, double[] y, int ySt, int yEnd, int pd)
        {
            double[] pln = OrdinaryLeastSquares(DF, pd);
            if (ySt > 0)
            {
                double k = 0;
                for (int i = 0; i < pln.Length; i++) k += pln[i] * Math.Pow(x[ySt], i);
                k /= y[ySt];
                for (int i = 0; i < ySt; i++) y[i] = Algebra.CalcFromPlnm(x[i], pln) / k;
            }
            if (yEnd < y.Length - 1)
            {
                double k = 0;
                for (int i = 0; i < pln.Length; i++) k += pln[i] * Math.Pow(x[yEnd], i);
                k /= y[yEnd];
                for (int i = yEnd + 1; i < y.Length; i++) y[i] = Algebra.CalcFromPlnm(x[i], pln) / k;
            }
            return y;
        }

        public static double[] Partition(List <double> px, PointPairList DF)
        {
            DF.RemoveAll(a => double.IsNaN(a.Y) == true);
            double[,] sp = CubicSplineInterpolation(DF);
            double[] py = new double[px.Count];
            for (int i = 0; i < py.Length; i++) py[i] = double.NaN;
            int n = 0;
            while (px[n] < DF[0].X) n++;
            int sn = 0;
            while (n < px.Count && px[n] <= DF.Last().X)
            {
                while (px[n] > DF[sn + 1].X) sn++;
                py[n] = sp[sn, 0] * Math.Pow(px[n] - DF[sn + 1].X, 3) + sp[sn, 1] * Math.Pow(px[n] - DF[sn + 1].X, 2) + sp[sn, 2] * (px[n] - DF[sn + 1].X) + sp[sn, 3];
                n++;
            }
            return py;
        }

        public static double[] Alignment(double[] x, double[] y, double[] xm, double[] ym, int St, int End)
        {
            int n; 
            double k, b, delta;
            if (St > 0)
            {
                n = Array.IndexOf(xm, xm.First(a => a >= x[St]));
                k = (ym[n] - ym[n - 1]) / (xm[n] - xm[n - 1]);
                b = ym[n - 1] - k * xm[n - 1];
                delta = k * x[St] + b - y[St];
                for ( St = St - 1; St >= 0; St--)
                {
                    while (n > 1 && xm[n - 1] > x[St]) n--;
                    k = (ym[n] - ym[n - 1]) / (xm[n] - xm[n - 1]);
                    b = ym[n - 1] - k * xm[n - 1];
                    y[St] = k * x[St] + b - delta;
                }
            }
            if (End < x.Length)
            {
                n = Array.IndexOf(xm, xm.First(a => a >= x[End]));
                k = (ym[n] - ym[n - 1]) / (xm[n] - xm[n - 1]);
                b = ym[n - 1] - k * xm[n - 1];
                delta = k * x[End] + b - y[End];
                for (End = End + 1; End < x.Length; End++)
                {
                    while (n < xm.Length && xm[n] < x[End]) n++;
                    k = (ym[n] - ym[n - 1]) / (xm[n] - xm[n - 1]);
                    b = ym[n - 1] - k * xm[n - 1];
                    y[End] = k * x[End] + b - delta;
                }
            }
            return y;
        }
    }
}
