using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace FFTWithNAudio
{
    public partial class ucFFTChartPanel : UserControl
    {
        public ucFFTChartPanel()
        {
            InitializeComponent();

            this._yColors.AddRange(new Color[]
            {
                Color.Red,
                Color.Blue,
                Color.Olive,
                Color.Teal,
                Color.YellowGreen,
                Color.DarkSeaGreen,
                Color.SandyBrown,
                Color.LightCyan
            });
            this._y2Colors.AddRange(new Color[]
            {
                Color.BlueViolet,
                Color.Orange,
                Color.BurlyWood,
                Color.Aquamarine,
                Color.Yellow
            });
            this._backupColors = new Dictionary<CurveItem, Color>();
            this._hintLine = new LineObj();
            this._hintLine.Line.Color = Color.Green;
            this._toolTip = new TextObj();
            this._toolTip.Location.AlignH = AlignH.Left;
            this._toolTip.Location.AlignV = AlignV.Top;
            this._toolTip.FontSpec.Fill.Color = Color.FromArgb(127, Color.Silver);
            this._toolTip.FontSpec.StringAlignment = StringAlignment.Near;
            this.zedGraphControl1.GraphPane.Title.Text = "标题";
            this.zedGraphControl1.GraphPane.YAxis.MajorTic.IsOpposite = false;
            this.zedGraphControl1.GraphPane.YAxis.MinorTic.IsOpposite = false;
            this.zedGraphControl1.GraphPane.Y2Axis.MajorTic.IsOpposite = false;
            this.zedGraphControl1.GraphPane.Y2Axis.MinorTic.IsOpposite = false;
            this.zedGraphControl1.GraphPane.XAxis.Title.Text = "频率";
            this.zedGraphControl1.GraphPane.XAxis.Type = AxisType.Text;
            this.zedGraphControl1.GraphPane.IsFontsScaled = false;
            this.zedGraphControl1.GraphPane.BarSettings.MinClusterGap = 0f;
            this._frequencyList = new List<int>();
        }

        public event EventHandler<ChartMouseMoveEventArgs> ChartMouseMoveEvent;

        public event EventHandler<ChartZoomEventArgs> ChartZoomEvent;

        public void GetYAxisScale(bool isY2, out double min, out double max)
        {
            if (isY2)
            {
                min = this.zedGraphControl1.GraphPane.Y2Axis.Scale.Min;
                max = this.zedGraphControl1.GraphPane.Y2Axis.Scale.Max;
                return;
            }
            min = this.zedGraphControl1.GraphPane.YAxis.Scale.Min;
            max = this.zedGraphControl1.GraphPane.YAxis.Scale.Max;
        }

        public void SetYAxisScale(bool isY2, double min, double max)
        {
            if (isY2)
            {
                this.zedGraphControl1.GraphPane.Y2Axis.Scale.Min = min;
                this.zedGraphControl1.GraphPane.Y2Axis.Scale.Max = max;
            }
            else
            {
                this.zedGraphControl1.GraphPane.YAxis.Scale.Min = min;
                this.zedGraphControl1.GraphPane.YAxis.Scale.Max = max;
            }
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();
        }

        public void RestoreYAxisScale(bool isY2)
        {
            if (isY2)
            {
                if (this._y2Min != null && this._y2Max != null)
                {
                    this.zedGraphControl1.GraphPane.Y2Axis.Scale.Min = this._y2Min.Value;
                    this.zedGraphControl1.GraphPane.Y2Axis.Scale.Max = this._y2Max.Value;
                }
            }
            else if (this._yMin != null && this._yMax != null)
            {
                this.zedGraphControl1.GraphPane.YAxis.Scale.Min = this._yMin.Value;
                this.zedGraphControl1.GraphPane.YAxis.Scale.Max = this._yMax.Value;
            }
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();
        }

        public void ClearData()
        {
            this.zedGraphControl1.GraphPane.CurveList.Clear();
            this.zedGraphControl1.Invalidate();
        }

        public void ClearYData()
        {
            this._yMin = null;
            this._yMax = null;
            this.zedGraphControl1.GraphPane.CurveList.RemoveAll((CurveItem r) => !r.IsY2Axis);
            this.zedGraphControl1.Invalidate();
        }

        public void ClearY2Data()
        {
            this._y2Min = null;
            this._y2Max = null;
            this.zedGraphControl1.GraphPane.Y2Axis.IsVisible = false;
            this.zedGraphControl1.GraphPane.CurveList.RemoveAll((CurveItem r) => r.IsY2Axis);
            this.zedGraphControl1.Invalidate();
        }

        public void SetTitle(string title)
        {
            this.zedGraphControl1.GraphPane.Title.Text = title;
            this.zedGraphControl1.Invalidate();
        }

        public void HideTitle()
        {
            this.zedGraphControl1.GraphPane.Title.IsVisible = false;
            this.zedGraphControl1.Invalidate();
        }

        public void HideXAxisTitle()
        {
            this.zedGraphControl1.GraphPane.XAxis.Title.IsVisible = false;
        }

        public void InitFrequencyList(List<int> times, int start_frequency)
        {
            this._start_frequency = start_frequency;
            this._frequencyList = times;
            List<string> list = new List<string>();
            foreach (int time in this._frequencyList)
            {
                list.Add(time.ToString());
            }
            this.zedGraphControl1.GraphPane.XAxis.Scale.TextLabels = list.ToArray();
        }

        public void SetYAxisTitle(string property)
        {
            this.zedGraphControl1.GraphPane.YAxis.Title.Text = property;
        }

        public void AddDisplayData(string property, string id, List<double> results)
        {
            PointPairList pointPairList = new PointPairList();
            for (int i = 0; i < results.Count; i++)
            {
                PointPair point = new PointPair((double)i, results[i]);
                pointPairList.Add(point);
            }
            Color yaxisColor = this.GetYAxisColor();
            string label = string.Format("{0}-{1}", id, property);
            LineItem lineItem = this.zedGraphControl1.GraphPane.AddCurve(label, pointPairList, yaxisColor, SymbolType.None);
            lineItem.Line.Width = 2f;
            this._backupColors.Add(lineItem, yaxisColor);
            this.zedGraphControl1.GraphPane.YAxis.Title.Text = property;
            this.zedGraphControl1.GraphPane.YAxis.Scale.IsReverse = false;
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();
            if (!this._xInitialized)
            {
                this._xMin = this.zedGraphControl1.GraphPane.XAxis.Scale.Min;
                this._xMax = this.zedGraphControl1.GraphPane.XAxis.Scale.Max;
                this._xInitialized = true;
            }
            this.UpdateYScaleRange();
        }

        public void AddDisplayDataAsBar(string property, string id, List<double> results)
        {
            PointPairList pointPairList = new PointPairList();
            for (int i = 0; i < results.Count; i++)
            {
                PointPair point = new PointPair((double)i, results[i]);
                pointPairList.Add(point);
            }
            string label = string.Format("{0}-{1}", id, property);
            BarItem barItem = this.zedGraphControl1.GraphPane.AddBar(label, pointPairList, Color.Blue);
            Color color = Color.FromArgb(215, 0, 0, 255);
            barItem.Bar.Fill = new Fill(color);
            this._backupColors.Add(barItem, color);
            this.zedGraphControl1.GraphPane.YAxis.Title.Text = property;
            this.zedGraphControl1.GraphPane.YAxis.Scale.IsReverse = true;
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();
            if (!this._xInitialized)
            {
                this._xMin = this.zedGraphControl1.GraphPane.XAxis.Scale.Min;
                this._xMax = this.zedGraphControl1.GraphPane.XAxis.Scale.Max;
                this._xInitialized = true;
            }
            this.UpdateYScaleRange();
        }

        private void UpdateYScaleRange()
        {
            if (this._yMin != null)
            {
                double? num = this._yMin;
                double num2 = this.zedGraphControl1.GraphPane.YAxis.Scale.Min;
                if (!(num.GetValueOrDefault() > num2 & num != null))
                {
                    goto IL_68;
                }
            }
            this._yMin = new double?(this.zedGraphControl1.GraphPane.YAxis.Scale.Min);
            IL_68:
            if (this._yMax != null)
            {
                double? num = this._yMax;
                double num2 = this.zedGraphControl1.GraphPane.YAxis.Scale.Max;
                if (!(num.GetValueOrDefault() < num2 & num != null))
                {
                    return;
                }
            }
            this._yMax = new double?(this.zedGraphControl1.GraphPane.YAxis.Scale.Max);
        }

        private void UpdateY2ScaleRange()
        {
            if (this._y2Min != null)
            {
                double? num = this._y2Min;
                double num2 = this.zedGraphControl1.GraphPane.Y2Axis.Scale.Min;
                if (!(num.GetValueOrDefault() > num2 & num != null))
                {
                    goto IL_68;
                }
            }
            this._y2Min = new double?(this.zedGraphControl1.GraphPane.Y2Axis.Scale.Min);
            IL_68:
            if (this._y2Max != null)
            {
                double? num = this._y2Max;
                double num2 = this.zedGraphControl1.GraphPane.Y2Axis.Scale.Max;
                if (!(num.GetValueOrDefault() < num2 & num != null))
                {
                    return;
                }
            }
            this._y2Max = new double?(this.zedGraphControl1.GraphPane.Y2Axis.Scale.Max);
        }

        public void AddDisplayData(string label, List<double> results)
        {
            PointPairList pointPairList = new PointPairList();
            for (int i = 0; i < results.Count; i++)
            {
                PointPair point = new PointPair((double)i, results[i]);
                pointPairList.Add(point);
            }
            Color yaxisColor = this.GetYAxisColor();
            LineItem lineItem = this.zedGraphControl1.GraphPane.AddCurve(label, pointPairList, yaxisColor, SymbolType.None);
            lineItem.Line.Width = 2f;
            this._backupColors.Add(lineItem, yaxisColor);
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();
            if (!this._xInitialized)
            {
                this._xMin = this.zedGraphControl1.GraphPane.XAxis.Scale.Min;
                this._xMax = this.zedGraphControl1.GraphPane.XAxis.Scale.Max;
                this._xInitialized = true;
            }
            this.UpdateYScaleRange();
        }

        public void InvalidateGraphControl()
        {
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();
        }

        public void AddDisplayDataAsPoint(string property, string id, List<double> results)
        {
            PointPairList pointPairList = new PointPairList();
            for (int i = 0; i < results.Count; i++)
            {
                PointPair point = new PointPair((double)i, results[i]);
                pointPairList.Add(point);
            }
            Color yaxisColor = this.GetYAxisColor();
            string label = string.Format("{0}-{1}", id, property);
            LineItem lineItem = this.zedGraphControl1.GraphPane.AddCurve(label, pointPairList, yaxisColor, SymbolType.Square);
            lineItem.Symbol.Size = 5f;
            lineItem.Symbol.Fill = new Fill(yaxisColor);
            lineItem.Symbol.Border.IsVisible = false;
            lineItem.Line.IsVisible = false;
            this._backupColors.Add(lineItem, yaxisColor);
            this.zedGraphControl1.GraphPane.YAxis.Title.Text = property;
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();
            this.UpdateYScaleRange();
        }

        public void SetY2AxisTitle(string property)
        {
            this.zedGraphControl1.GraphPane.Y2Axis.Title.Text = property;
        }

        public void AddDisplayDataToY2(string property, string id, List<double> results)
        {
            PointPairList pointPairList = new PointPairList();
            for (int i = 0; i < results.Count; i++)
            {
                PointPair point = new PointPair((double)i, results[i]);
                pointPairList.Add(point);
            }
            Color y2AxisColor = this.GetY2AxisColor();
            string label = string.Format("{0}-{1}", id, property);
            LineItem lineItem = this.zedGraphControl1.GraphPane.AddCurve(label, pointPairList, y2AxisColor, SymbolType.None);
            lineItem.Line.Width = 2f;
            this._backupColors.Add(lineItem, y2AxisColor);
            lineItem.IsY2Axis = true;
            this.zedGraphControl1.GraphPane.Y2Axis.Title.Text = property;
            this.zedGraphControl1.GraphPane.Y2Axis.IsVisible = true;
            this.zedGraphControl1.GraphPane.Y2Axis.MajorTic.IsOpposite = false;
            this.zedGraphControl1.GraphPane.Y2Axis.MinorTic.IsOpposite = false;
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();
            this.UpdateY2ScaleRange();
        }

        public void AddDisplayDataToY2AsPoint(string property, string id, List<double> results)
        {
            PointPairList pointPairList = new PointPairList();
            for (int i = 0; i < results.Count; i++)
            {
                PointPair point = new PointPair((double)i, results[i]);
                pointPairList.Add(point);
            }
            Color y2AxisColor = this.GetY2AxisColor();
            string label = string.Format("{0}-{1}", id, property);
            LineItem lineItem = this.zedGraphControl1.GraphPane.AddCurve(label, pointPairList, y2AxisColor, SymbolType.Square);
            lineItem.Symbol.Size = 5f;
            lineItem.Symbol.Fill = new Fill(y2AxisColor);
            lineItem.Symbol.Border.IsVisible = false;
            lineItem.Line.IsVisible = false;
            this._backupColors.Add(lineItem, y2AxisColor);
            lineItem.IsY2Axis = true;
            this.zedGraphControl1.GraphPane.Y2Axis.Title.Text = property;
            this.zedGraphControl1.GraphPane.Y2Axis.IsVisible = true;
            this.zedGraphControl1.GraphPane.Y2Axis.MajorTic.IsOpposite = false;
            this.zedGraphControl1.GraphPane.Y2Axis.MinorTic.IsOpposite = false;
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();
            this.UpdateY2ScaleRange();
        }

        public DataTable GetYDataTable(bool isY2)
        {
            Dictionary<string, List<double>> ydata = this.GetYData(isY2);
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Times");
            foreach (string columnName in ydata.Keys)
            {
                dataTable.Columns.Add(columnName, typeof(double));
            }
            int count = dataTable.Columns.Count;
            for (int i = 0; i < this._frequencyList.Count; i++)
            {
                DataRow dataRow = dataTable.Rows.Add(Array.Empty<object>());

                dataRow[0] = this._frequencyList[i];
                int num = 1;
                foreach (List<double> list in ydata.Values)
                {
                    if (i < list.Count)
                    {
                        dataRow[num] = list[i];
                    }
                    num++;
                }
            }
            return dataTable;
        }

        public Dictionary<string, List<double>> GetYData(bool isY2)
        {
            Dictionary<string, List<double>> dictionary = new Dictionary<string, List<double>>();
            foreach (CurveItem curveItem in this.zedGraphControl1.GraphPane.CurveList)
            {
                if ((isY2 && curveItem.IsY2Axis) || (!isY2 && !curveItem.IsY2Axis))
                {
                    List<double> list = new List<double>();
                    for (int i = 0; i < curveItem.Points.Count; i++)
                    {
                        PointPair pointPair = curveItem.Points[i];
                        if (pointPair.Y == 1.7976931348623157E+308)
                        {
                            list.Add(0.0);
                        }
                        else
                        {
                            list.Add(pointPair.Y);
                        }
                    }
                    dictionary.Add(curveItem.Label.Text, list);
                }
            }
            return dictionary;
        }

        public List<List<double?>> GetYDataList(bool isY2)
        {
            List<List<double?>> list = new List<List<double?>>();
            foreach (CurveItem curveItem in this.zedGraphControl1.GraphPane.CurveList)
            {
                if ((isY2 && curveItem.IsY2Axis) || (!isY2 && !curveItem.IsY2Axis))
                {
                    List<double?> list2 = new List<double?>();
                    for (int i = 0; i < curveItem.Points.Count; i++)
                    {
                        PointPair pointPair = curveItem.Points[i];
                        if (pointPair.Y == 1.7976931348623157E+308)
                        {
                            list2.Add(null);
                        }
                        else
                        {
                            list2.Add(new double?(pointPair.Y));
                        }
                    }
                    list.Add(list2);
                }
            }
            return list;
        }

        private void zedGraphControl1_MouseMove(object sender, MouseEventArgs e)
        {
            int tooltipXIndex = this.GetTooltipXIndex(e.X, e.Y);
            this.HandleOnMouseMove(tooltipXIndex, e.X, e.Y);
            EventHandler<ChartMouseMoveEventArgs> chartMouseMoveEvent = this.ChartMouseMoveEvent;
            if (chartMouseMoveEvent == null)
            {
                return;
            }
            chartMouseMoveEvent(this, new ChartMouseMoveEventArgs(tooltipXIndex, e.X, e.Y));
        }

        private int GetTooltipXIndex(int mouseX, int mouseY)
        {
            GraphPane graphPane = this.zedGraphControl1.GraphPane;
            if (graphPane == null)
            {
                return -1;
            }
            PointF ptF = new PointF((float)mouseX, (float)mouseY);
            double num;
            double num2;
            graphPane.ReverseTransform(ptF, out num, out num2);
            if (num < graphPane.XAxis.Scale.Max && num > graphPane.XAxis.Scale.Min && num2 < graphPane.YAxis.Scale.Max && num2 > graphPane.YAxis.Scale.Min)
            {
                return this.FindNearestXIndex(num - 1.0);
            }
            return -1;
        }

        public void HandleOnMouseMove(int xIndex, int mouseX, int mouseY)
        {
            if (xIndex < 0)
            {
                return;
            }
            GraphPane graphPane = this.zedGraphControl1.GraphPane;
            if (graphPane == null || graphPane.CurveList.Count == 0)
            {
                return;
            }
            bool flag = false;
            if (this.zedGraphControl1.GraphPane.GraphObjList.Contains(this._hintLine))
            {
                this.zedGraphControl1.GraphPane.GraphObjList.Remove(this._hintLine);
                this.zedGraphControl1.GraphPane.GraphObjList.Remove(this._toolTip);
                flag = true;
            }
            if (xIndex < graphPane.CurveList[0].Points.Count)
            {
                PointF ptF = new PointF((float)mouseX, (float)mouseY);
                double num;
                double y;
                graphPane.ReverseTransform(ptF, out num, out y);
                double x = graphPane.CurveList[0].Points[xIndex].X;
                this._hintLine.Location.X = x + 1.0;
                this._hintLine.Location.Y = graphPane.YAxis.Scale.Min;
                this._hintLine.Location.Width = 0.0;
                this._hintLine.Location.Height = graphPane.YAxis.Scale.Max - graphPane.YAxis.Scale.Min;
                string tooltip = this.GetTooltip(xIndex);
                this._toolTip.Location.X = x + 1.0;
                this._toolTip.Location.Y = y;
                this._toolTip.Text = tooltip;
                int num2 = graphPane.CurveList[0].Points.Count / 5;
                if (num2 < 3)
                {
                    num2 = 3;
                }
                if (xIndex >= graphPane.CurveList[0].Points.Count - num2)
                {
                    this._toolTip.Location.AlignH = AlignH.Right;
                }
                else
                {
                    this._toolTip.Location.AlignH = AlignH.Left;
                }
                this.zedGraphControl1.GraphPane.GraphObjList.Add(this._hintLine);
                this.zedGraphControl1.GraphPane.GraphObjList.Add(this._toolTip);
                this.zedGraphControl1.Invalidate();
                return;
            }
            if (flag)
            {
                this.zedGraphControl1.Invalidate();
            }
        }

        private int FindNearestXIndex(double x)
        {
            GraphPane graphPane = this.zedGraphControl1.GraphPane;
            if (graphPane == null || graphPane.CurveList.Count == 0)
            {
                return -1;
            }
            CurveItem curveItem = graphPane.CurveList[0];
            int i = 0;
            while (i < curveItem.Points.Count - 1)
            {
                double x2 = curveItem.Points[i].X;
                double x3 = curveItem.Points[i + 1].X;
                if (x <= x2)
                {
                    return i;
                }
                if (x < x3)
                {
                    if (x - x2 <= x3 - x)
                    {
                        return i;
                    }
                    return i + 1;
                }
                else
                {
                    i++;
                }
            }
            return curveItem.Points.Count - 1;
        }

        private string GetTooltip(int index)
        {
            if (this._frequencyList.Count <= index || index < 0)
            {
                return "";
            }
            GraphPane graphPane = this.zedGraphControl1.GraphPane;
            double x = graphPane.CurveList[0].Points[index].X;
            double y = graphPane.CurveList[0].Points[index].Y;
            string text = string.Format("频率: {0}\n", this._frequencyList[index]);
            foreach (CurveItem curveItem in graphPane.CurveList)
            {
                if (curveItem.IsVisible && !curveItem.IsY2Axis && index < curveItem.Points.Count)
                {
                    string str = string.Format("{0}: {1:0.####}", curveItem.Label.Text, curveItem.Points[index].Y);
                    if (curveItem.Points[index].Y == 1.7976931348623157E+308)
                    {
                        str = string.Format("{0}:  -", curveItem.Label.Text);
                    }
                    text = text + "\n" + str;
                }
            }
            if (graphPane.Y2Axis.IsVisible)
            {
                text += "\n  --  ";
                foreach (CurveItem curveItem2 in graphPane.CurveList)
                {
                    if (curveItem2.IsVisible && curveItem2.IsY2Axis && index < curveItem2.Points.Count)
                    {
                        string str2 = string.Format("{0}: {1:0.####}", curveItem2.Label.Text, curveItem2.Points[index].Y);
                        if (curveItem2.Points[index].Y == 1.7976931348623157E+308)
                        {
                            str2 = string.Format("{0}:  -", curveItem2.Label.Text);
                        }
                        text = text + "\n" + str2;
                    }
                }
            }
            return text;
        }

        private bool zedGraphControl1_MouseDownEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            int num = -1;
            Graphics g = base.CreateGraphics();
            GraphPane graphPane = this.zedGraphControl1.GraphPane;
            object obj;
            if (graphPane.FindNearestObject(e.Location, g, out obj, out num) && obj is Legend && num >= 0 && num < graphPane.CurveList.Count<CurveItem>())
            {
                CurveItem curveItem = graphPane.CurveList[num];
                curveItem.IsVisible = !curveItem.IsVisible;
                if (curveItem.IsVisible)
                {
                    curveItem.Color = this._backupColors[curveItem];
                }
                else
                {
                    curveItem.Color = Color.Gray;
                }
                this.zedGraphControl1.Invalidate();
            }
            return true;
        }

        private Color GetYAxisColor()
        {
            int num = 0;
            using (List<CurveItem>.Enumerator enumerator = this.zedGraphControl1.GraphPane.CurveList.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (!enumerator.Current.IsY2Axis)
                    {
                        num++;
                    }
                }
            }
            if (num >= this._yColors.Count)
            {
                return this.GetRandomColor();
            }
            return this._yColors[num];
        }

        private Color GetY2AxisColor()
        {
            int num = 0;
            using (List<CurveItem>.Enumerator enumerator = this.zedGraphControl1.GraphPane.CurveList.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.IsY2Axis)
                    {
                        num++;
                    }
                }
            }
            if (num >= this._y2Colors.Count)
            {
                return this.GetRandomColor();
            }
            return this._y2Colors[num];
        }

        private Color GetRandomColor()
        {
            Random random = new Random();
            return Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        }

        private void zedGraphControl1_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            double num = this.zedGraphControl1.GraphPane.XAxis.Scale.Min;
            double num2 = this.zedGraphControl1.GraphPane.XAxis.Scale.Max;
            if (num < this._xMin)
            {
                this.zedGraphControl1.GraphPane.XAxis.Scale.Min = this._xMin;
                num = this._xMin;
            }
            if (num2 > this._xMax)
            {
                this.zedGraphControl1.GraphPane.XAxis.Scale.Max = this._xMax;
                num2 = this._xMax;
            }
            EventHandler<ChartZoomEventArgs> chartZoomEvent = this.ChartZoomEvent;
            if (chartZoomEvent == null)
            {
                return;
            }
            chartZoomEvent(this, new ChartZoomEventArgs(num, num2));
        }

        public void HandleOnZoomEvent(double xMin, double xMax)
        {
            this.zedGraphControl1.GraphPane.XAxis.Scale.Min = xMin;
            this.zedGraphControl1.GraphPane.XAxis.Scale.Max = xMax;
            this.zedGraphControl1.Invalidate();
        }

        private TextObj _toolTip;

        private LineObj _hintLine;

        private List<int> _frequencyList;

        private int _start_frequency = 0;

        private Dictionary<CurveItem, Color> _backupColors;

        private List<Color> _yColors = new List<Color>();

        private List<Color> _y2Colors = new List<Color>();

        private double _xMin;

        private double _xMax = 100.0;

        private bool _xInitialized;

        private double? _yMin;

        private double? _yMax;

        private double? _y2Min;

        private double? _y2Max;
    }

    public class ChartMouseMoveEventArgs : EventArgs
    {
        public ChartMouseMoveEventArgs(int xIndex, int x, int y)
        {
            this.XIndex = xIndex;
            this.X = x;
            this.Y = y;
        }
        public int XIndex { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
    }


    public class ChartZoomEventArgs : EventArgs
    {
        public ChartZoomEventArgs(double xMin, double xMax)
        {
            this.XMin = xMin;
            this.XMax = xMax;
        }
        public double XMin { get; private set; }
        public double XMax { get; private set; }
    }
}
