using ExcellonFormat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CncDriller
{

    enum DrillingState
    {
        Waiting,
        Drilling,
        Done
    }

    class DrillingHole : INotifyPropertyChanged
    {
        private Hole hole;

        public DrillingHole(Hole h)
        {
            this.hole = h;
        }

        public Hole Hole
        {
            get
            {
                return hole;
            }
        }

        public string Coordinates
        {
            get
            {
                return hole.Coords.Text;
            }
        }

        public string Diameter
        {
            get
            {
                return hole.ActiveTool.Diameter.Text;
            }
        }

        private DrillingState state = DrillingState.Waiting;
        public DrillingState State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                OnPropertyChanged("State");
                OnPropertyChanged("IsDrilling");
                OnPropertyChanged("IsDone");
            }
        }

        public bool IsDrilling
        {
            get
            {
                return state == DrillingState.Drilling;
            }
        }

        public bool IsDone
        {
            get
            {
                return state == DrillingState.Done;
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
