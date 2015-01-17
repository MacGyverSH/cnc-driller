using CodeUtils;
using ExcellonFormat;
using ShapeokoDriver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CncDriller
{


    enum DrillerReport
    {
        ChangeTool,
        Done
    }

    class ReportProgressState
    {
        public ReportProgressState(DrillingState st, int i)
        {
            State = st;
            Index = i;
        }

        public ReportProgressState(DrillerReport r, int i)
        {
            Index = i;
            Report = r;
        }

        public DrillingState State { get; set; }
        public int Index { get; set; }
        public DrillerReport Report { get; set; }
    }

    

    /// <summary>
    /// Interaction logic for DrillingWindow.xaml
    /// </summary>
    public partial class DrillingWindow : Window
    {
        
        private BindingList<DrillingHole> list;
        private ExcellonFile file;

        private BackgroundWorker driller;

        private double homeZ = 0;

        private GVector MachinePosition;
        private GStatus Status;

        private double pcbThickness = 0;
        private double drillSpeed = 0;
        private double moveSpeed = 0;

        private GSender gsender;

        public DrillingWindow(ExcellonFile file)
        {
            this.file = file;
            InitializeComponent();

            gsender = ServiceHolder.Instance.Sender;

            list = new BindingList<DrillingHole>();
            foreach(Hole h in file.Holes)
            {
                list.Add(new DrillingHole(h));
            }


            dgDrills.ItemsSource = list;

            driller = new BackgroundWorker();

            driller.WorkerReportsProgress = true;
            driller.WorkerSupportsCancellation = true;

            driller.DoWork += driller_DoWork; 
            driller.ProgressChanged += driller_ProgressChanged;
            driller.RunWorkerCompleted += driller_RunWorkerCompleted;

            

            pgProgress.Maximum = 100;

            ServiceHolder.Instance.OnReplyRecieved += delegate(GReply reply)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (reply is GReplyStatus)
                    {
                        GReplyStatus statusReply = (GReplyStatus)reply;

                        MachinePosition = statusReply.MachinePosition;
                        Status = statusReply.Status;
                    }
                }));
            };

        }

        private void driller_DoWork(object sender, DoWorkEventArgs e)
        {
    
            int index = 0;
            int max = list.Count;
            Tool lastTool = null;

            gsender.WaitToIdle();
            GCommandChain ch = new GCommandChain("G17 G21 G90 G94 G53 G0"); // Init
            GSender.appendVectorToChain(ch, new GVector(0, 0, homeZ + 2));
            gsender.SendDataBlocking(ch);
            gsender.WaitToIdle();

            Thread.Sleep(1000);

            e.Result = null;
            foreach (DrillingHole h in list)
            {
                if (driller.CancellationPending)
                {
                    return;
                }

                if (!h.IsDone)
                {
                    int percent = (index * 100) / max;

                    if (lastTool == null)
                    {
                        lastTool = h.Hole.ActiveTool;
                    }

                    if (lastTool != h.Hole.ActiveTool)
                    {
                        e.Result = new ReportProgressState(DrillerReport.ChangeTool, index);
                        return;
                    }

                    driller.ReportProgress(percent, new ReportProgressState(DrillingState.Drilling, index));

                    // TODO: Drill
                    //Thread.Sleep(100);
                    Drill(h);

                    driller.ReportProgress(percent, new ReportProgressState(DrillingState.Done, index));

                }
                index++;
            }

            e.Result = new ReportProgressState(DrillerReport.Done, 0);

        }

        private void sendInstruction(GVector vect, double z, string name, double speed)
        {
            GCommandChain ch = new GCommandChain(name); // run in mm, machine coordinates, rapid positioning
            GSender.appendVectorToChain(ch, new GVector(vect, homeZ + z), true);
            ch.Add(String.Format(CultureInfo.InvariantCulture.NumberFormat, "F{0:0.##}", speed));

            gsender.SendDataBlocking(ch);
            //Thread.Sleep(4000);
        }

        private void Drill(DrillingHole hole)
        {
            
            //gsender.MoveToMachineCoords(new GVector(hole.Hole.Coords, homeZ + 2), moveSpeed);
            sendInstruction(hole.Hole.Coords, 2, "G0", moveSpeed);
            sendInstruction(hole.Hole.Coords, 0.2, "G0", moveSpeed);
            
            sendInstruction(hole.Hole.Coords, -pcbThickness, "G1", drillSpeed);
            sendInstruction(hole.Hole.Coords, 0.2, "G1", drillSpeed);
            
            sendInstruction(hole.Hole.Coords, 2, "G0", moveSpeed);
            
        }

        private void driller_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgProgress.Value = e.ProgressPercentage;

            if (e.UserState is ReportProgressState)
            {
                ReportProgressState report = (ReportProgressState)e.UserState;
                list[report.Index].State = report.State;
            }
            
        }

        private void driller_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
            if (e.Result is ReportProgressState)
            {
                ReportProgressState report = (ReportProgressState)e.Result;
                
                if (report.Report == DrillerReport.ChangeTool)
                {
                    if (MessageBox.Show("Change tool to " + list[report.Index].Diameter + " and click OK", "Drilling", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                    {
                        driller.RunWorkerAsync();
                    }
                    else
                    {
                        btn_stop_Click_1(null, null);
                    }
                }
                else if (report.Report == DrillerReport.Done)
                {
                    pgProgress.Value = 100;
                    btn_stop.IsEnabled = false;
                }

            }
        }

        private void btn_start_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                pcbThickness = Double.Parse(tbPcbThickness.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat);
                drillSpeed = Double.Parse(tbDrillSpeed.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat);
                moveSpeed = Double.Parse(tbMoveSpeed.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat);
            }
            catch (FormatException)
            {
                MessageBox.Show("Bad number format.", "Drilling", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            driller.RunWorkerAsync();
            btn_start.IsEnabled = false;
            btn_stop.IsEnabled = true;
            btn_setZ.IsEnabled = false;

            tbDrillSpeed.IsEnabled = false;
            tbMoveSpeed.IsEnabled = false;
            tbPcbThickness.IsEnabled = false;

            btn_start.Content = "Restart";
        }

        private void Window_Closing_1(object sender, CancelEventArgs e)
        {
            driller.CancelAsync();
        }

        private void btn_stop_Click_1(object sender, RoutedEventArgs e)
        {
            btn_start.IsEnabled = true;
            btn_stop.IsEnabled = false;
            btn_setZ.IsEnabled = true;

            tbDrillSpeed.IsEnabled = true;
            tbMoveSpeed.IsEnabled = true;
            tbPcbThickness.IsEnabled = true;

            driller.CancelAsync();
        }

        private void btn_setZ_Click_1(object sender, RoutedEventArgs e)
        {
            if (Status != GStatus.Idle)
            {
                MessageBox.Show("CNC moving, or in other state! Please wait to iddle mode.", "Drilling", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            homeZ = MachinePosition.Z;
            lbZHome.Content = String.Format(CultureInfo.InvariantCulture.NumberFormat, "{0:0.##}", homeZ);
        }


    }
}
