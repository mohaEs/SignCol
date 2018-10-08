using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using DbModel.Extensions;
using DbModel.DomainClasses.Enum;

namespace DbModel.ValueConvert
{
    public class GridItemConverter : IValueConverter
    {
        private const string NewItemPlaceholderName = "{NewItemPlaceholder}";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string res = "";
            if (value is int)
            {
                res = value.ToString();
                res = res.Substring(0, 4) + "/" + res.Substring(4, 2) + "/" + res.Substring(6, 2);
            }
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString() == NewItemPlaceholderName)
            {
                value = DependencyProperty.UnsetValue;
            }
            return value;
        }
    }
    public class ToIntConverter : IValueConverter
    {
        private const string NewItemPlaceholderName = "{NewItemPlaceholder}";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? res =null;
            if (value!=null)
            {
                res = (int?)int.Parse(UtilityClass.RemoveSpecialChars(value.ToString(), "/"));
            }
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString() == NewItemPlaceholderName)
            {
                value = DependencyProperty.UnsetValue;
            }
            return value;
        }
    }
    public class GenderConverter : IValueConverter
    {
        private const string NewItemPlaceholderName = "{NewItemPlaceholder}";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string res = "";
            if (value != null)
            {
                if (value.ToString().Equals("0"))
                    res = "Female";
                if (value.ToString().Equals("1"))
                    res = "Male";
            }
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString() == NewItemPlaceholderName)
            {
                value = DependencyProperty.UnsetValue;
            }
            return value;
        }
    }
    public class WordTypeConverter : IValueConverter
    {
        private const string NewItemPlaceholderName = "{NewItemPlaceholder}";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //return (from n in _word where n.WordType == wt select n).Count();
            string res = "";
            if (value != null)
            {
                if ((WordType)value == WordType.Numberslitle10)
                    res = "Number <10";
                if ((WordType)value == WordType.Numberslarger10)
                    res = "Number > 10";
                if ((WordType)value == WordType.Letters)
                    res = "Letter";
                if ((WordType)value == WordType.Words_By_Signs)
                    res = "Word by Sign";
                if ((WordType)value == WordType.Words_By_Letters)
                    res = "Word by letters";
                if ((WordType)value == WordType.Sentences_By_Words)
                    res = "Sentence by Words";
                if ((WordType)value == WordType.Sentences_By_Signs)
                    res = "Sentence by Signs";
                if ((WordType)value == WordType.Arbitrary_Sentences)
                    res = "Arbitrary Sentence";

            }
            return res;// value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString() == NewItemPlaceholderName)
            {
                value = DependencyProperty.UnsetValue;
            }
            return value;
        }
    }
    public class FamilyHistoryConverter : IValueConverter
    {
        private const string NewItemPlaceholderName = "{NewItemPlaceholder}";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string res = "";
            if (value != null)
            {
                if (value.ToString().Equals("0"))
                    res = "No";
                if (value.ToString().Equals("1"))
                    res = "Yes";
            }
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString() == NewItemPlaceholderName)
            {
                value = DependencyProperty.UnsetValue;
            }
            return value;
        }
    }
}
