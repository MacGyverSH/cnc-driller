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
using System.Windows.Shapes;

namespace CncDriller
{

    enum CollimationSteps
    {
        SelectFirst,
        SelectSecond,
        Summary,
        Preview
    }

    /// <summary>
    /// Interaction logic for Collimation.xaml
    /// </summary>
    public partial class Collimation : Window
    {
        private ExcellonFile excellonFile;

        private CollimationSteps step = CollimationSteps.SelectFirst;


        private Hole HoleA = null;
        private GVector HoleA_coords;

        private Hole HoleB = null;
        private GVector HoleB_coords;

        private double angle = 0;
        private GVector translation;
        

        public Collimation(ExcellonFile excellonFile)
        {
            translation = GVector.empty(2);
            this.excellonFile = excellonFile;
            InitializeComponent();
            updateUI();
            excellonEditor.ExcelonFile = excellonFile;

            excellonEditor.OnHoleClick += delegate(Hole h)
            {
                Console.WriteLine(h);
                if (step == CollimationSteps.Preview)
                {
                    if (controller.IsInIdleMode)
                    {
                        controller.MoveTo(new GVector(h.Coords.X, h.Coords.Y, controller.MachineZ));
                    }
                }
            };

        }

        private void updateUI()
        {
            
            switch (step)
            {
                case CollimationSteps.SelectFirst:
                    excellonEditor.SelectedHole = HoleA;

                    if (HoleA != null)
                    {
                        btn_move.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        btn_move.Visibility = System.Windows.Visibility.Hidden;
                    }

                    btn_next.Content = "Next";
                    btn_back.IsEnabled = false;
                    lbHelp.Content = "Select first hole and move with tool above the drill";

                    holeSelectionGrid.Visibility = System.Windows.Visibility.Visible;
                    summaryGrid.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case CollimationSteps.SelectSecond:
                    excellonEditor.SelectedHole = HoleB;

                    if (HoleB != null)
                    {
                        btn_move.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        btn_move.Visibility = System.Windows.Visibility.Hidden;
                    }

                    btn_next.Content = "Next";
                    btn_back.IsEnabled = true;
                    lbHelp.Content = "Select second hole and move with tool above the drill";

                    holeSelectionGrid.Visibility = System.Windows.Visibility.Visible;
                    summaryGrid.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case CollimationSteps.Summary:

                    lb_distance.Content = String.Format("{0:0.#} mm", HoleA_coords.XY.DistanceTo(HoleB_coords.XY));
                    lb_drillA_diameter.Content = String.Format("{0:0.#} mm", HoleA.ActiveTool.Diameter.X);
                    lb_drillA_position.Content = HoleA_coords.XY.Text;
                    lb_drillB_diameter.Content = String.Format("{0:0.#} mm", HoleB.ActiveTool.Diameter.X);
                    lb_drillB_position.Content = HoleB_coords.XY.Text;

                    lb_angle.Content = String.Format("{0:0.##} °", (angle * 180) / Math.PI);
                    lb_translation.Content = translation.XY.Text;

                    btn_next.Content = "Collimate";
                    btn_back.IsEnabled = true;
                    lbHelp.Content = "Check if values are correct";

                    btn_move.Visibility = System.Windows.Visibility.Hidden;

                    holeSelectionGrid.Visibility = System.Windows.Visibility.Collapsed;
                    summaryGrid.Visibility = System.Windows.Visibility.Visible;
                    break;
                case CollimationSteps.Preview:

                    btn_next.Content = "Done";
                    btn_back.IsEnabled = false;
                    lbHelp.Content = "Click on hole to preview";

                    holeSelectionGrid.Visibility = System.Windows.Visibility.Visible;
                    summaryGrid.Visibility = System.Windows.Visibility.Collapsed;
                    break;
            }

        }

        private void colimate()
        {
            //translation = HoleA_coords.XY - HoleA.Coords;
            
            GVector ExcellonVector = HoleB.Coords - HoleA.Coords;
            GVector CncVector = HoleB_coords.XY - HoleA_coords.XY;

            angle = CncVector.Angle(ExcellonVector);
        }

        private void btn_next_Click(object sender, RoutedEventArgs e)
        {
            if (step == CollimationSteps.SelectFirst || step == CollimationSteps.SelectSecond)
            {
                Hole h = excellonEditor.SelectedHole;
                if (h == null)
                {
                    MessageBox.Show("Please select one drill!", "Collimation", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!controller.IsInIdleMode)
                {
                    MessageBox.Show("CNC moving, or in other state! Please wait to iddle mode.", "Collimation", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            

                if (step == CollimationSteps.SelectFirst)
                {

                    if (HoleA != null)
                    {
                        if (MessageBox.Show("Override coordinates of first hole with current CNC position?", "Collimation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            HoleA_coords = controller.MachinePosition;
                        }
                    }
                    else
                    {
                        HoleA_coords = controller.MachinePosition;
                    }
                    HoleA = h;
                    

                    step = CollimationSteps.SelectSecond;
                }
                else if (step == CollimationSteps.SelectSecond)
                {
                    if (HoleB != null)
                    {
                        if (MessageBox.Show("Override coordinates of second hole with current CNC position?", "Collimation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            HoleB_coords = controller.MachinePosition;
                        }
                    }
                    else
                    {
                        HoleB_coords = controller.MachinePosition;
                    }
                    HoleB = h;

                    colimate();

                    step = CollimationSteps.Summary;
                }
            }
            else if (step == CollimationSteps.Summary)
            {
                

                ExcellonFile ex = excellonFile;

                ex.Translate(HoleA.Coords.XY * -1);
                ex.Rotate(angle);
                ex.Translate(HoleA_coords.XY);
                
                excellonEditor.ExcelonFile = ex;

                step = CollimationSteps.Preview;


            }
            else if (step == CollimationSteps.Preview)
            {
                DialogResult = true;
                Close();
            }

            updateUI();
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            if (step == CollimationSteps.SelectSecond)
            {
                step = CollimationSteps.SelectFirst;
            }
            else if (step == CollimationSteps.Summary)
            {
                step = CollimationSteps.SelectSecond;
            }
            
            updateUI();
        }

        private void btn_move_Click(object sender, RoutedEventArgs e)
        {
            switch (step)
            {
                case CollimationSteps.SelectFirst:
                    if (HoleA != null)
                    {
                        controller.MoveTo(HoleA_coords);
                    }
                    break;
                case CollimationSteps.SelectSecond:
                    if (HoleB != null)
                    {
                        controller.MoveTo(HoleB_coords);
                    }
                    break;
            }
        }
    }
}
