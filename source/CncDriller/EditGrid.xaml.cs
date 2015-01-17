using CodeUtils;
using ExcellonFormat;
using ShapeokoDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CncDriller
{
    public delegate void HoleClickEvent(Hole h);

    /// <summary>
    /// Interaction logic for EditGrid.xaml
    /// </summary>
    public partial class EditGrid : UserControl
    {
        private EditorCursorItem cursorItem;
        public EditGrid()
        {
            InitializeComponent();
            holeList = new EditorItemList(drawCanvas.Children);
            cursorItem = new EditorCursorItem();

            ServiceHolder.Instance.OnReplyRecieved += delegate(GReply reply)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (reply is GReplyStatus)
                    {
                        GReplyStatus statusReply = (GReplyStatus)reply;

                        cursorItem.Position = statusReply.MachinePosition.XY;

                    }
                }));
            };
        }

        public event HoleClickEvent OnHoleClick;

        public Hole SelectedHole
        {
            get
            {
                EditorHoleItem gh = holeList.Selected;
                if (gh != null)
                {
                    return gh.Hole;
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    holeList.Selected = null;
                }
                else
                {
                    EditorHoleItem h = holeList.findBy(value);
                    holeList.Selected = h;
                }
            }
        }

        private EditorItemList holeList;

        private ExcellonFile excelonFile = null;
        public ExcellonFile ExcelonFile
        {
            get
            {
                return excelonFile;
            }
            set
            {
                excelonFile = value;
                redrawMe();
            }
        }

        const double ScaleRate = 1.1;
        private Point mouseClickPoint;



        private void drawSpace_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point p = e.GetPosition((IInputElement)sender);

            double scale = holeList.Scale;

            if (e.Delta > 0)
            {
                scale *= ScaleRate;
            }
            else
            {
                scale /= ScaleRate;
            }

            holeList.Scale = scale;
        }

        private void Grid_MouseMove_1(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(this);
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Point off = holeList.Offset;
                off.X -= (mouseClickPoint.X - p.X);
                off.Y += (mouseClickPoint.Y - p.Y);

                holeList.Offset = off;
            }
            mouseClickPoint = p;
        }

        private void Grid_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            mouseClickPoint = e.GetPosition(this);
        }

        private void createHole(Hole h)
        {
            EditorHoleItem el = new EditorHoleItem(h);
            
            el.MouseDown += delegate(object sender, MouseButtonEventArgs e)
            {
                holeList.Selected = el;
                if (e.LeftButton == MouseButtonState.Pressed && OnHoleClick != null)
                {
                    OnHoleClick(el.Hole);
                }
            };

            holeList.Add(el);
        }

        private void createLine(Hole h1, Hole h2)
        {
            EditorPathItem el = new EditorPathItem(h1, h2);
            holeList.Add(el);
        }
        

        public void redrawMe()
        {
            holeList.Clear();

            holeList.Scale = 1;
            holeList.Offset = new Point(0, 0);

            if (ExcelonFile == null)
            {
                return;
            }

            Hole lastHole = null;
            foreach (Hole h in ExcelonFile.Holes)
            {
                createLine(lastHole, h);
                createHole(h);
                lastHole = h;
            }

            holeList.Add(cursorItem);

        }

    }
}
