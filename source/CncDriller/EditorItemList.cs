using ExcellonFormat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CncDriller
{

    class EditorItemList : BindingList<BaseEditorItem>
    {

        private UIElementCollection collection;

        public EditorItemList(UIElementCollection collection)
        {
            this.collection = collection;
            ListChanged += delegate(object sender, ListChangedEventArgs e)
            {
                if (e.ListChangedType == ListChangedType.ItemAdded)
                {
                    collection.Add(this[e.NewIndex]);
                }
            };
        }

        private Point offset = new Point(0, 0);
        public Point Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
                foreach (BaseEditorItem i in this)
                {
                    i.Offset = offset;
                }
            }
        }

        private double scale = 1;
        public double Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;

                foreach (BaseEditorItem i in this)
                {
                    i.Scale = scale;
                }
            }
        }

        protected override void ClearItems()
        {
            collection.Clear();
        }

        

        protected override void RemoveItem(int index)
        {
            BaseEditorItem item = base[index];
            collection.Remove(item);
            base.RemoveItem(index);
        }


        public EditorHoleItem Selected
        {
            get
            {
                foreach (BaseEditorItem h in this)
                {
                    if (h is EditorHoleItem && ((EditorHoleItem)h).Selected)
                    {
                        return (EditorHoleItem)h;
                    }
                }
                return null;
            }
            set
            {
                foreach (BaseEditorItem h in this)
                {
                    if (h is EditorHoleItem)
                    {
                        if (((EditorHoleItem)h) == value)
                        {
                            ((EditorHoleItem)h).Selected = true;
                        }
                        else
                        {
                            ((EditorHoleItem)h).Selected = false;
                        }
                    }
                }
            }
        }

        public EditorHoleItem findBy(Hole hole)
        {
            foreach (BaseEditorItem h in this)
            {
                if (h is EditorHoleItem)
                {
                    if (((EditorHoleItem)h).Hole == hole)
                    {
                        return ((EditorHoleItem)h);
                    }
                }
            }
            return null;
        }
    }
}
