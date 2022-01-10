using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForecastAttempting
{
    static class Algebra
    {
        private static void ChangeRows(double[,] m, double[] fv, int i1, int i2)
        {
            double rv;
            for (int j = 0; j < m.GetLength(1); j++) { rv = m[i1, j]; m[i1, j] = m[i2, j]; m[i2, j] = rv; }
            if (fv != null) { rv = fv[i1]; fv[i1] = fv[i2]; fv[i2] = rv; }
        }

        private static void SubtractionMtpRow(double[,] m, double[] fv, int i1, int i2, double mtp)
        {
            for (int j = 0; j < m.GetLength(1); j++) m[i1, j] -= m[i2, j] * mtp;
            if (fv != null) fv[i1] -= fv[i2] * mtp;
        }

        public static double[] GaussMethod(double[,] em, double[] fv)
        {
            int d = em.GetLength(0);
            double[] res = new double[d];
            MatrixToRightUpperTriangularMatrix(em, fv);
            for (int i = d - 1; i >= 0; i--)
            {
                for (int j = d - 1; j > i; j--)
                {
                    fv[i] -= em[i, j] * fv[j];
                }
                fv[i] = fv[i] / em[i, i];
            }
            return fv;
        }

        public static double[] ThreeDiagonalMatrixAlgorithm(double[,] em, double[] fv)
        {
            int n = em.GetLength(0);
            double[] v1 = new double[n - 1];
            double[] v2 = new double[n];
            double[] res = new double[n];
            v1[0] = em[0, 1] / em[0, 0];
            for (int i = 1; i < n - 1; i++)
            {
                v1[i] = em[i, i + 1] / (em[i, i] - v1[i - 1] * em[i, i - 1]);
            }
            v2[0] = fv[0] / em[0, 0];
            for (int i = 1; i < n; i++)
            {
                v2[i] = (fv[i] - v2[i - 1] * em[i, i - 1]) / (em[i, i] - v1[i - 1] * em[i, i - 1]);
            }
            res[n - 1] = v2[n - 1];
            for (int i = n - 2; i >= 0; i--)
            {
                res[i] = v2[i] - v1[i] * res[i + 1];
            }
            return (res);
        }

        public static double CalcFromPlnm(double x, double[] pln)
        {
            double am = 0;
            for (int i = 0; i < pln.Length; i++) am += pln[i] * Math.Pow(x, i);
            return am;
        }

        public static double Determinant(double[,] m)
        {
            double[,] wm = new double[m.GetLength(0), m.GetLength(1)];
            Array.Copy(m, wm, m.Length);
            double ans = MatrixToRightUpperTriangularMatrix(wm, null);
            for (int i = 0; i < wm.GetLength(0); i++) ans *= wm[i, i];
            return ans;
        }

        public static double MatrixToRightUpperTriangularMatrix(double[,] m, double[] fv)
        {
            double dtrmmtp = 1;
            int d = m.GetLength(0);
            for (int j = 0; j < d - 1; j++)
            {
                int ir = j;
                for (int i = j; i < d; i++) if (Math.Abs(m[i, j]) > Math.Abs(m[ir, j]))ir = i;
                if (ir != j) { ChangeRows(m, fv, ir, j); dtrmmtp *= -1; }
                for (int i = d - 1; i > j; i--) SubtractionMtpRow(m, fv, i, j, m[i, j] / m[j, j]);
            }
            return dtrmmtp;
        }

        public static double[,] TransposeMatrix(double[,] m)
        {
            double[,] ans = new double[m.GetLength(1), m.GetLength(0)];
            for (int i = 0; i < m.GetLength(0); i++) for (int j = 0; j < m.GetLength(1); j++) ans[j,i] = m[i,j];
            return ans;
        }

        public static double[,] InverseMatrix(double[,] m)
        {
            return MultiplyMatrixByNumber(TransposeMatrix(CofactorsMatrix(m)), 1 / Determinant(m));
        }

        public static double[,] MultiplyMatrixByNumber(double[,] m, double mtp)
        {
            double[,] ans = new double[m.GetLength(0), m.GetLength(1)];
            Array.Copy(m, ans, m.Length);
            for (int i = 0; i < ans.GetLength(0); i++) for (int j = 0; j < ans.GetLength(1); j++) ans[i, j] *= mtp;
            return ans;
        }

        public static double[,] MultiplyMatrixByMatrix(double[,] ml, double[,] mr)
        {
            double[,] ans = new double[ml.GetLength(0), mr.GetLength(1)];
            for (int i = 0; i < ml.GetLength(0); i++) for (int j = 0; j < mr.GetLength(1); j++) { ans[i, j] = 0; for (int k = 0; k < ml.GetLength(0); k++) ans[i, j] += ml[i, k] * mr[k, j]; }
            return ans;
        }

        public static double[] MultiplyMatrixByVector(double[,] m, double[] v)
        {
            double[] ans = new double[m.GetLength(0)];
            for (int i = 0; i < m.GetLength(0); i++) { ans[i] = 0; for (int j = 0; j < v.Length; j++) ans[i] += m[i, j] * v[j]; }
            return ans;
        }

        public static double[,] CofactorsMatrix(double[,] m)
        {
            int len = m.GetLength(0);
            double[,] ans = new double[len, len];
            for (int i = 0; i < m.GetLength(0); i++)
            {
                for (int j = 0; j < m.GetLength(1); j++)
                {
                    double[,] cfm = new double[len - 1, len - 1];
                    for (int k = 0; k < len; k++) for (int l = 0; l < len; l++) if (k != i && l != j) { int r = k, c = l; if (r > i) r--; if (c > j) c--; cfm[r, c] = m[k, l]; }
                    ans[i, j] = Math.Pow(-1, i + j) * Determinant(cfm);
                }
            }
            return ans;
        }

        public static double[] OrdinaryLeastSquares(double[,] em, double[] ev)
        {
            return MultiplyMatrixByVector(MultiplyMatrixByMatrix(InverseMatrix(MultiplyMatrixByMatrix(TransposeMatrix(em), em)), TransposeMatrix(em)), ev);
        }
    }
}
