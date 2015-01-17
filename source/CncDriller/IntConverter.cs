using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CncDriller
{
    class IntConverter : IValueConverter
    {

        public IntConverter()
        {
        }

        public string Category { get; set; }
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is Int32))
            {
                throw new NotImplementedException("IntConverter can only convert from int");
            }

            return ((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int res = 0;

            if (!(value is string) || !Int32.TryParse((string)value, out res))
            {
                throw new NotImplementedException("IntConverter can only convert back from numeric string");
            }

            return res;
        }


    }
}
