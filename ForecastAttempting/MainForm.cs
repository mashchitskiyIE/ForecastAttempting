using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace ForecastAttempting
{
    public partial class MainForm: Form
    {
        public MainForm()
        {
            InitializeComponent();
            MAZGCASB.Click += new EventHandler((object s, EventArgs e) => ZGCResetScale(MAZGC));
            IMAZGCASB.Click += new EventHandler((object s, EventArgs e) => ZGCResetScale(IMAZGC));
            ARIMAZGCASB.Click += new EventHandler((object s, EventArgs e) => ZGCResetScale(ARIMAZGC));
            MAZGCMCB.CheckedChanged += new EventHandler((object s, EventArgs e) => MarkCheckBox_CheckedChanged(MAZGC, MAZGCMCB));
            IMAZGCMCB.CheckedChanged += new EventHandler((object s, EventArgs e) => MarkCheckBox_CheckedChanged(IMAZGC, IMAZGCMCB));
            ARIMAZGCMCB.CheckedChanged += new EventHandler((object s, EventArgs e) => MarkCheckBox_CheckedChanged(ARIMAZGC, ARIMAZGCMCB));
            MAZGC.MouseMove += new MouseEventHandler((object s, MouseEventArgs e) => ZGC_DisplayCoords(MAZGC, MAZGCPNTB, MAZGCCPTB, MAZGCPPTB));
            IMAZGC.MouseMove += new MouseEventHandler((object s, MouseEventArgs e) => ZGC_DisplayCoords(IMAZGC, IMAZGCPNTB, IMAZGCCPTB, IMAZGCPPTB));
            ARIMAZGC.MouseMove += new MouseEventHandler((object s, MouseEventArgs e) => ZGC_DisplayCoords(ARIMAZGC, ARIMAZGCPNTB, ARIMAZGCCPTB, ARIMAZGCPPTB));
            DataInGrid.RowCount = (int)RNU.Value;
            RNU.ValueChanged += new EventHandler((object s, EventArgs e) => { if (RNU.Value >= RFNU.Value) DataInGrid.RowCount = (int)RNU.Value; else RNU.Value = RFNU.Value; });
            RFNU.ValueChanged += new EventHandler((object s, EventArgs e) => { if (RFNU.Value > RNU.Value) RFNU.Value = RNU.Value; });
        }


        double[] Xorg, Yorg, X, Y;

        private void MF_Load(object sender, EventArgs e)
        {
            //Autofill
            //double[] blood = { 5.88, 5.55, 5.33, 5.11, 5, 4.77, 4.66, 4.55, 4.44, 4.33, 4.22, 4.22, 4.11, 3.89, 3.77, 3.66, 3.55, 3.44, 3.33, 3.22, 3.22, 3.11, 3.11, 3, 3.11, 3.11, 3.22, 3.22, 3.44, 3.77, 4, 4.22, 4.44, 4.55, 4.55, 4.55, 4.44, 4.33, 4.22, 4.22, 4.22, 4.22, 4.22, 4.22, 4.33, 4.33, 4.33, 4.44, 4.55, 4.55, 4.55, 4.55, 4.55, 4.44, 4.44, 4.44, 4.44, 4.44, 4.44, 4.44, 4.44, 4.44, 4.55, 4.55, 4.55, 4.55, 4.66, 4.66, 4.66, 4.66, 4.66, 4.66, 4.66, 4.66, 4.66, 4.55, 4.55, 4.66, 4.55, 4.55, 4.55, 4.55, 4.55, 4.66, 4.77, 4.77, 4.77, 4.77, 4.77, 4.88, 4.88, 4.88, 4.88, 4.88, 4.88, 4.88, 4.88, 4.88, 4.88, 5, 5, 4.88, 4.88, 5, 5.11, 5.22, 5.33, 5.44, 5.55, 5.66, 5.88, 5.88, 5.88, 5.99, 5.88, 5.88, 5.88, 5.77, 5.66, 5.44, 5.275, 5.11, 5, 4.88, 4.77, 4.55, 4.44, 4.33, 4.44, 4.66, 4.88, 5.11, 5.33, 5.66, 6.22, 6.88, 7.44, 7.66, 7.66, 7.55, 7.33, 7.1, 6.77, 6.44, 6.11, 5.77, 5.44, 5.11, 4.88, 4.77, 4.55, 4.33, 4.22, 4, 3.77, 3.66, 3.55, 3.44, 3.55, 3.77, 4, 4.22, 4.55, 4.77, 5, 5.11, 5.11, 5.22, 5.22, 5.22, 5.22, 5.22, 5.33, 5.33, 5.33, 5.44, 5.44, 5.55, 5.55, 5.44, 5.33, 5.22, 5.22, 5.11, 5.11, 5, 4.88, 4.77, 4.66, 4.55, 4.44, 4.33, 4.22, 4.11, 4, 3.89, 3.44, 3.55, 3.77, 4.11, 4.44, 4.77, 5.22, 5.55, 5.77, 5.88, 5.99, 5.99, 5.99, 5.99, 5.88, 5.88, 5.77, 5.66, 5.44, 5.44, 5.33, 5.11, 4.88, 4.66, 4.44, 4.33, 4.22, 4.11, 4, 3.89, 3.89, 3.77, 3.66, 3.66, 3.77, 4, 4.11, 4.22, 4.33, 4.33, 4.44, 4.44, 4.33, 4.33, 4.33, 4.33, 4.33, 4.33, 4.44, 4.44, 4.55, 4.55, 4.55, 4.66, 4.66, 4.66, 4.55, 4.55, 4.44, 4.33, 4.33, 4.33, 4.33, 4.44, 4.44, 4.44, 4.44, 4.44, 4.44, 4.44, 4.44, 4.33, 4.33, 4.33, 4.44, 4.66, 5.22, 5.66, 5.99, 6.11, 6.11, 6.11, 6.88, 6.66, 6.44, 6.11, 5.77, 5.55, 5.22, 5, 4.77, 4.66, 4.55 };
            //RNU.Value = blood.Length;
            //RFNU.Value = 250;
            //for (int i = 0; i < blood.Length; i++) DataInGrid.Rows[i].Cells[0].Value = blood[i];
            //Autofill
            GraphPane G = MAZGC.GraphPane, G1 = IMAZGC.GraphPane, G2 = ARIMAZGC.GraphPane;
            G.Title.Text = "Moving Average";
            G1.Title.Text = "Disintegrated Moving Average";
            G2.Title.Text = "Integrated Autoregression of Disintegrated Moving Average";
            G1.XAxis.Title.Text = G.XAxis.Title.Text = "X";
            G1.YAxis.Title.Text = G.YAxis.Title.Text = "Y";
            //G.IsFontsScaled = G1.IsFontsScaled = G2.IsFontsScaled = false;


            //double[][] DI = ARIMA.Desintegration(blood, 2, 5);
            //double[] ARFrc = new double[2] { -0.5, 0.5 };
            //double[] IFrc = ARIMA.Integration(DI, ARFrc, 5);
        }

        private void ReadData()
        {
            int n = DataInGrid.RowCount - 1;
            while (DataInGrid.Rows[n].Cells[0].Value == null || DataInGrid.Rows[n].Cells[0].Value.ToString() == "") n--;
            n++;
            Yorg = new double[n];
            Xorg = new double[n];
            for (int i = 0; i < n; i++) { Xorg[i] = i + 1; Yorg[i] = Convert.ToDouble(DataInGrid.Rows[i].Cells[0].Value.ToString()); }
            if (RFNU.Value > n) RFNU.Value = n;
            n = (int)RFNU.Value;
            X = new double[n];
            Y = new double[n];
            for (int i = 0; i < n; i++) { X[i] = Xorg[i]; Y[i] = Yorg[i]; }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            ReadData();
            EventHandler EH = new EventHandler((object s, EventArgs ea) => { ReadData(); Forecast(); });
            RNU.ValueChanged -= EH; RNU.ValueChanged += EH;
            RFNU.ValueChanged -= EH; RFNU.ValueChanged += EH;
            EH = new EventHandler((object s, EventArgs ea) => Forecast()); 
            MAdNU.ValueChanged -= EH; MAdNU.ValueChanged += EH;
            MAssNU.ValueChanged -= EH; MAssNU.ValueChanged += EH;
            IdNU.ValueChanged -= EH; IdNU.ValueChanged += EH;
            IssNU.ValueChanged -= EH; IssNU.ValueChanged += EH;
            ARssNU.ValueChanged -= EH; ARssNU.ValueChanged += EH;
            PFnNU.ValueChanged -= EH; PFnNU.ValueChanged += EH;
            EH = new EventHandler((object s, EventArgs ea) => { if (ARdNU.Value + 1 > ARNeNU.Value) ARdNU.Value = ARdNU.Value - 1; else Forecast(); });
            ARdNU.ValueChanged -= EH; ARdNU.ValueChanged += EH;
            EH = new EventHandler((object s, EventArgs ea) => { if (ARdNU.Value + 1 > ARNeNU.Value) ARNeNU.Value = ARNeNU.Value + 1; else Forecast(); });
            ARNeNU.ValueChanged -= EH; ARNeNU.ValueChanged += EH;
            Forecast();
        }

        private void Forecast()
        {
            int MAd = (int)MAdNU.Value, MAss = (int)MAssNU.Value, Id = (int)IdNU.Value, Iss = (int)IssNU.Value, ARd = (int)ARdNU.Value, ARss = (int)ARssNU.Value, ARNe = (int)ARNeNU.Value, PFn = (int)PFnNU.Value;

            double[] MA = ARIMA.MovingAverage(Y, MAd, MAss);
            GraphPane G = MAZGC.GraphPane;
            G.CurveList.Clear();
            SymbolType st;
            if (MAZGCMCB.Checked == true) st = SymbolType.Circle; else st = SymbolType.None;
            new LineItem(string.Format("Moving Average({0})({1})", MAd, MAss), X, MA, Color.Red, st);
            G.CurveList.Add(new LineItem(string.Format("Moving Average({0})({1})", MAd, MAss), X, MA, Color.Red, st));
            double Ymin = MA.Where(a => double.IsNaN(a) == false).Min(), Ymax = MA.Where(a => double.IsNaN(a) == false).Max();
            int n = X.Length / MAss + 2;
            for (int i = 0; i < n; i++) G.CurveList.Add(new LineItem("", new double[] { MAss * i, MAss * i }, new double[] { Ymin, Ymax }, Color.LightGray, SymbolType.None));
            G.CurveList.Add(new LineItem("", new double[] { 0, MAss * (n - 1) }, new double[] { Ymax, Ymax }, Color.LightGray, SymbolType.None));
            G.CurveList.Add(new LineItem("", new double[] { 0, MAss * (n - 1) }, new double[] { Ymin, Ymin }, Color.LightGray, SymbolType.None));
            if (MAZGCASCB.Checked == true) ZGCResetScale(MAZGC); else MAZGC.Invalidate();

            double[][] DIMA = ARIMA.Desintegration(MA, Id, Iss);
            G = IMAZGC.GraphPane;
            G.CurveList.Clear();
            if (IMAZGCMCB.Checked == true) st = SymbolType.Circle; else st = SymbolType.None;
            new LineItem(string.Format("Disintegrated Moving Average({0},{1})({2},{3})", MAd, Id, MAss, Iss), X, DIMA.Last(), Color.DarkGreen, st);
            G.CurveList.Add(new LineItem(string.Format("Disintegrated Moving Average({0},{1})({2},{3})", MAd, Id, MAss, Iss), X, DIMA.Last(), Color.DarkGreen, st));
            Ymin = DIMA.Last().Where(a => double.IsNaN(a) == false).Min();
            Ymax = DIMA.Last().Where(a => double.IsNaN(a) == false).Max();
            n = X.Length / Iss + 2;
            for (int i = 0; i < n; i++) G.CurveList.Add(new LineItem("", new double[] { Iss * i, Iss * i }, new double[] { Ymin, Ymax }, Color.LightGray, SymbolType.None));
            G.CurveList.Add(new LineItem("", new double[] { 0, Iss * (n - 1) }, new double[] { Ymax, Ymax }, Color.LightGray, SymbolType.None));
            G.CurveList.Add(new LineItem("", new double[] { 0, Iss * (n - 1) }, new double[] { Ymin, Ymin }, Color.LightGray, SymbolType.None));
            if (IMAZGCASCB.Checked == true) ZGCResetScale(IMAZGC); else IMAZGC.Invalidate();

            double[] Frc = ARIMA.Autoregression(DIMA.Last(), ARd, ARss, ARNe, PFn);
            Frc = ARIMA.Integration(DIMA, Frc, Iss);
            G = ARIMAZGC.GraphPane;
            G.CurveList.Clear();
            if (ARIMAZGCMCB.Checked == true) st = SymbolType.Circle; else st = SymbolType.None;
            G.CurveList.Add(new LineItem("Original", Xorg, Yorg, Color.Blue, st));
            double[] FrcX = new double[(int)PFnNU.Value];
            for (int i = 0; i < FrcX.Length; i++) FrcX[i] = X.Last() + i + 1;
            G.CurveList.Add(new LineItem(string.Format("Integrated Autoregression of Disintegrated Moving Average({0},{1},{2})({3},{4},{5})", ARd, MAd, Id, ARss, MAss, Iss), FrcX, Frc, Color.Violet, st));
            Ymin = Yorg.Min();
            Ymax = Yorg.Max();
            n = (Xorg.Length + FrcX.Length) / ARss + 2;
            for (int i = 0; i < n; i++) G.CurveList.Add(new LineItem("", new double[] { ARss * i, ARss * i }, new double[] { Ymin, Ymax }, Color.LightGray, SymbolType.None));
            G.CurveList.Add(new LineItem("", new double[] { 0, ARss * (n - 1) }, new double[] { Ymax, Ymax }, Color.LightGray, SymbolType.None));
            G.CurveList.Add(new LineItem("", new double[] { 0, ARss * (n - 1) }, new double[] { Ymin, Ymin }, Color.LightGray, SymbolType.None));
            if (ARIMAZGCASCB.Checked == true) ZGCResetScale(ARIMAZGC); else ARIMAZGC.Invalidate();
        }

        public static void ZGCResetScale(ZedGraphControl ZGC)
        {
            GraphPane G = ZGC.GraphPane;
            if (G.CurveList.Count != 0)
            {
                CurveList Curves = new CurveList();
                for (int i = 0; i < G.CurveList.Count; i++)
                {
                    if (G.CurveList[i].Label.Text == "")
                    { Curves.Add(G.CurveList[i]); G.CurveList.RemoveAt(i); i--; }
                }
                double Xmax = G.CurveList.Max(a => (a.Points as PointPairList).Where(b => double.IsNaN(b.Y) == false).Max(c => c.X));
                double Xmin = G.CurveList.Min(a => (a.Points as PointPairList).Where(b => double.IsNaN(b.Y) == false).Min(c => c.X));
                double Xind = (Xmax - Xmin) / 20; //X axis indent
                Xmax += Xind; Xmin -= Xind;
                G.XAxis.Scale.Max = Xmax;
                G.XAxis.Scale.Min = Xmin;
                double Ymax = G.CurveList.Max(a => (a.Points as PointPairList).Where(b => double.IsNaN(b.Y) == false).Max(c => c.Y));
                double Ymin = G.CurveList.Min(a => (a.Points as PointPairList).Where(b => double.IsNaN(b.Y) == false).Min(c => c.Y));
                double Yind = (Ymax - Ymin) / 20;
                Ymax += Yind; Ymin -= Yind;
                G.YAxis.Scale.Max = Ymax;
                G.YAxis.Scale.Min = Ymin;
                G.AxisChange();
                G.CurveList.AddRange(Curves);
                ZGC.Invalidate();
            }
        }

        private void MarkCheckBox_CheckedChanged(ZedGraphControl ZGC, CheckBox CB )
        {
            CurveList CL = ZGC.GraphPane.CurveList;
            SymbolType st;
            if (CB.Checked == true) st = SymbolType.Circle; else st = SymbolType.None;
            for (int i = 0; i < CL.Count; i++) if(CL[i].Label.Text != "") (CL[i] as LineItem).Symbol.Type = st;
            ZGC.Invalidate();
        }
       

        public static void ZGC_DisplayCoords(ZedGraphControl ZGC, TextBox PNTB, TextBox CPTB, TextBox PPTB)
        {
            Point p = ZGC.PointToClient(MousePosition);
            double x, y;
            ZGC.GraphPane.ReverseTransform(p, out x, out y);
            CurveItem crv;
            int n;
            ZGC.GraphPane.FindNearestPoint(new PointF(p.X, p.Y), out crv, out n);
            CPTB.Text = string.Format("({0:F2}; {1:F2})", x, y);
            if (crv != null) { PNTB.ForeColor = PPTB.ForeColor = crv.Color; PNTB.Text = n.ToString(); PPTB.Text = string.Format("({0:F2}; {1:F2})", crv[n].X, crv[n].Y); }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DataGridView DGV = DataInGrid;
            if (DGV != null)
            {
                DataObject d = DGV.GetClipboardContent();
                if (d != null) Clipboard.SetDataObject(d);
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DataGridView DGV = DataInGrid;
            if (DGV != null)
            {
                if (DGV.SelectedCells.Count > 1 || DGV.SelectedCells.Count == 0)
                {
                    MessageBox.Show("Неподдерживаемая вставка: выберите одну ячейку.", "Программное сообщение");
                    DGV.ClearSelection();
                }
                else
                {
                    IDataObject data = Clipboard.GetDataObject();
                    string text = (string)data.GetData(DataFormats.Text);
                    if (text != null)
                    {
                        string[] rows = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        int r = DGV.SelectedCells[0].RowIndex, c = DGV.SelectedCells[0].ColumnIndex, rc = DGV.SelectedCells[0].RowIndex + rows.Length;
                        if (rc > DGV.Rows.Count) RNU.Value = rc;
                        for (int i = 0; i + r < rc; i++)
                        {
                            string[] cols = rows[i].Split('\t');
                            for (int j = 0; j < cols.Length && j + c < DGV.ColumnCount; j++)
                            {
                                if (DGV.Rows[i + r].Cells[j + c].ReadOnly == false)
                                {
                                    double d;
                                    if (double.TryParse(cols[j], out d)) DGV.Rows[i + r].Cells[j + c].Value = d; else DGV.Rows[i + r].Cells[j + c].Value = cols[j];
                                    DGV.Rows[i + r].Cells[j + c].Selected = true;
                                }
                            }
                        }
                    }
                }
                DGV.RefreshEdit();
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            DataGridView DGV = DataInGrid;
            if (DGV != null)
            {
                for (int i = 0; i < DGV.SelectedCells.Count; i++)
                {
                    if (DGV.SelectedCells[i].ReadOnly == false) DGV.SelectedCells[i].Value = "";
                }
                DGV.ClearSelection();
            }
        }


    }
}
