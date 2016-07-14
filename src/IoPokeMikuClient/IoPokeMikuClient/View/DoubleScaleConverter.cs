using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace IoPokeMikuClient.View
{
    public class DoubleScaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            var dValue = value as Double?;
            if(dValue == null)
            {
                return value;
            }

            var strParam = parameter as string;
            if(strParam == null)
            {
                return value;
            }

            double dParam;
            if(!Double.TryParse(strParam, out dParam))
            {
                return value;
            }

            return dValue * dParam;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return null;
        }
    }
}
