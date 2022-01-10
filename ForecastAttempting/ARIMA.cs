using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForecastAttempting
{
    static class ARIMA
    {
        //d - degree, ss - season step
        public static double[] MovingAverage(double[] src, int d, int ss)
        {
            double[] MA = new double[src.Length];
            if (d > 1)
            {
                int n = ss * (d - 1);
                for (int i = 0; i < n; i++) MA[i] = double.NaN;
                for (int i = n; i < MA.Length; i++) { MA[i] = 0; for (int j = 0; j < d; j++) MA[i] += src[i - ss * j]; MA[i] /= (double)d; }
            }
            else src.CopyTo(MA, 0);
            return MA;
        }

        public static double[][] Desintegration(double[] src, int d, int ss)
        {
            double[][] ans = new double[d + 1][];
            for (int i = 0; i < ans.Length; i++) ans[i] = new double[src.Length];
            src.CopyTo(ans[0], 0);
            for (int i = 1; i <= d; i++)
            {
                double[] dI = new double[src.Length];
                for (int j = 0; j < i * ss; j++) dI[j] = double.NaN;
                for (int j = i * ss; j < src.Length; j++) dI[j] = ans[i - 1][j] - ans[i - 1][j - ss];
                dI.CopyTo(ans[i], 0);
            }
            return ans;
        }

        public static double[] Integration(double[][] DI, double[] ARFrc, int ss)
        {
            int n = DI[0].Length;
            for (int i = 0; i < DI.Length - 1; i++) Array.Resize(ref DI[i], n + ARFrc.Length);
            if (DI.Length > 1)
            {
                ARFrc.CopyTo(DI[DI.Length - 2], n);
                for (int i = DI.Length - 2; i >= 0; i--)
                {
                    for (int j = n; j < DI[i].Length; j++) DI[i][j] = DI[i][j - ss] + DI[i][j];
                    if (i > 0) for (int j = n; j < DI[i].Length; j++) DI[i - 1][j] = DI[i][j];
                }
                return DI[0].Skip(n).ToArray();
            }
            else return ARFrc;
        }

        public static double[] Autoregression(double[] src, int d, int ss, int Ne, int Pn) // Ne - number of equations, Pn - Points number
        {
            double[] wa = new double[src.Length];
            src.CopyTo(wa,0);
            double[] Frc = new double[Pn];
            for (int i = 0; i < Frc.Length; i++ )
            {
                double[,] em = new double[Ne, d + 1];
                double[] ev = new double[Ne];
                for (int j = 0; j < Ne; j++)
                {
                    ev[j] = wa[wa.Length - 1 - j];
                    em[j, 0] = 1;
                    for (int k = 1; k <= d; k++) em[j, k] = wa[wa.Length - 1 - j - ss * k];
                }
                double[] AR = Algebra.OrdinaryLeastSquares(em, ev);
                Frc[i] = AR[0];
                for (int j = 1; j <= d; j++) Frc[i] += AR[j] * wa[wa.Length - j * ss];
                Array.Resize(ref wa, wa.Length + 1);
                wa[wa.Length - 1] = Frc[i];
            }
            return Frc;
        }
    }
}
