using DbModel.Extensions;
using DbModel.Infrastructure;
using DbModel.Services.Search;
using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace DbModel.ViewModel
{
    public class ListItems : ValidatableViewModelBase
    {

        public ObservableCollection<SearchOptions> GetSearchOption()
        {
            var searchop = new ObservableCollection<SearchOptions>();
            searchop.Add(new SearchOptions() { ID = 0, Value = "ID" });
            searchop.Add(new SearchOptions() { ID = 1, Value = "Name" });
            searchop.Add(new SearchOptions() { ID = 2, Value = "Date" });
            searchop.Add(new SearchOptions() { ID = 3, Value = "Gender" });
            searchop.Add(new SearchOptions() { ID = 4, Value = "Family History" });
            searchop.Add(new SearchOptions() { ID = 5, Value = "Age" });
            searchop.Add(new SearchOptions() { ID = 6, Value = "Cansanguinity" });
            return searchop;
        }
        public ObservableCollection<SearchOptions> GetFexamSearchOption()
        {
            var searchop = new ObservableCollection<SearchOptions>();
            searchop.Add(new SearchOptions() { ID = 0, Value = "ID" });
            searchop.Add(new SearchOptions() { ID = 1, Value = "Baseline" });
            searchop.Add(new SearchOptions() { ID = 2, Value = "Diagnosis" });
            searchop.Add(new SearchOptions() { ID = 3, Value = "Iris" });
            searchop.Add(new SearchOptions() { ID = 4, Value = "Tube" });
            searchop.Add(new SearchOptions() { ID = 5, Value = "Lens" });
            searchop.Add(new SearchOptions() { ID = 6, Value = "Bleb" });
            searchop.Add(new SearchOptions() { ID = 7, Value = "Corneal clarity" });
            searchop.Add(new SearchOptions() { ID = 8, Value = "Surgeries" });
            searchop.Add(new SearchOptions() { ID = 9, Value = "AC" });
            searchop.Add(new SearchOptions() { ID = 10, Value = "Patient" });
            searchop.Add(new SearchOptions() { ID = 11, Value = "Medications" });
            searchop.Add(new SearchOptions() { ID = 12, Value = "Corneal diameter" });
            searchop.Add(new SearchOptions() { ID = 13, Value = "CCT" });
            searchop.Add(new SearchOptions() { ID = 14, Value = "Fundus" });
            searchop.Add(new SearchOptions() { ID = 15, Value = "CD" });
            searchop.Add(new SearchOptions() { ID = 16, Value = "AL" });
            searchop.Add(new SearchOptions() { ID = 17, Value = "Reflection" });
            searchop.Add(new SearchOptions() { ID = 18, Value = "Date" });
            return searchop;
        }

        public ObservableCollection<Operator> GetSearchOperationInt()
        {
            var sop = new ObservableCollection<Operator>();
            sop.Add(new Operator("=", System.Linq.Expressions.Expression.Equal, Operator.TypesToApply.Both));
            sop.Add(new Operator("<>", System.Linq.Expressions.Expression.NotEqual, Operator.TypesToApply.Both));
            sop.Add(new Operator("<", System.Linq.Expressions.Expression.LessThan, Operator.TypesToApply.Numeric));
            sop.Add(new Operator(">", System.Linq.Expressions.Expression.GreaterThan, Operator.TypesToApply.Numeric));
            sop.Add(new Operator("<=", System.Linq.Expressions.Expression.LessThanOrEqual, Operator.TypesToApply.Numeric));
            sop.Add(new Operator(">=", System.Linq.Expressions.Expression.GreaterThanOrEqual, Operator.TypesToApply.Numeric));
            return sop;
        }
        private Operator _selectedSearchOperationString;
        public Operator SelectedSearchOperationString
        {
            get { return _selectedSearchOperationString; }
            set
            {
                _selectedSearchOperationString = value;
                //RaisePropertyChanged("SelectedBaselineIOP");
            }
        }


        public ObservableCollection<Operator> GetSearchOperationBool()
        {
            var sop = new ObservableCollection<Operator>();
            sop.Add(new Operator("=", System.Linq.Expressions.Expression.Equal, Operator.TypesToApply.Both));
            sop.Add(new Operator("<>", System.Linq.Expressions.Expression.NotEqual, Operator.TypesToApply.Both));
            return sop;
        }
        private Operator _selectedSearchOperationBool;
        public Operator SelectedSearchOperationBool
        {
            get { return _selectedSearchOperationBool; }
            set
            {
                _selectedSearchOperationBool = value;
            }
        }
        public ObservableCollection<Operator> GetSearchOperationString()
        {
            var sop = new ObservableCollection<Operator>();
            sop.Add(new Operator("شامل باشد", (expression, expression1) => System.Linq.Expressions.Expression.Call(expression,
                typeof(string).GetMethod("Contains"), expression1), Operator.TypesToApply.String));
            sop.Add(new Operator("شامل نباشد", (expression, expression1) =>
            {
                var contain = System.Linq.Expressions.Expression.Call(expression, typeof(string).GetMethod("Contains"), expression1);
                return System.Linq.Expressions.Expression.Not(contain);
            }, Operator.TypesToApply.String));
            return sop;
        }
        private Operator _selectedSearchOperationInt;
        public Operator SelectedSearchOperationInt
        {
            get { return _selectedSearchOperationInt; }
            set
            {
                _selectedSearchOperationInt = value;
            }
        }

        public ObservableCollection<AndOr> GetOperationAndOrs()
        {
            var sop = new ObservableCollection<AndOr>();
            sop.Add(new AndOr("And", "و", System.Linq.Expressions.Expression.AndAlso));
            sop.Add(new AndOr("Or", "یا", System.Linq.Expressions.Expression.OrElse));
            sop.Add(new AndOr("Suppress", "نادیده", (expression, expression1) => expression));
            return sop;
        }
        private AndOr _selectedAndOr;
        public AndOr SelectedAndOr
        {
            get { return _selectedAndOr; }
            set
            {
                _selectedAndOr = value;
                //RaisePropertyChanged("SelectedBaselineIOP");
            }
        }
        private SearchOptions _selectedSearchOption;
        public SearchOptions SelectedSearchOption
        {
            get { return _selectedSearchOption; }
            set
            {
                _selectedSearchOption = value;
                //RaisePropertyChanged("SelectedBaselineIOP");
            }
        }

        // BaselineIOP
        private ObservableCollection<DropDownItems> wordtypes = new ObservableCollection<DropDownItems>();
        public ObservableCollection<DropDownItems> WordTypes
        {
            get { return wordtypes; }
            set
            {
                wordtypes = value;
                RaisePropertyChanged("WordTypes");
            }
        }
        public ObservableCollection<DropDownItems> GetWordType()
        {
            var baselines = new ObservableCollection<DropDownItems>();
            baselines.Add(new DropDownItems() { ID = 0, Value = "Number < 10" });
            baselines.Add(new DropDownItems() { ID = 1, Value = "Number > 10" });
            baselines.Add(new DropDownItems() { ID = 2, Value = "Letter" });
            baselines.Add(new DropDownItems() { ID = 3, Value = "Word by Sign" });
            baselines.Add(new DropDownItems() { ID = 4, Value = "Word by letters" });
            baselines.Add(new DropDownItems() { ID = 5, Value = "Sentence by Words" });
            baselines.Add(new DropDownItems() { ID = 6, Value = "Sentence by Signs" });
            baselines.Add(new DropDownItems() { ID = 7, Value = "Arbitrary Sentence" });
            return baselines;
        }


        public Tuple<ObservableCollection<string>, string> addtolist(ObservableCollection<string> mylist, string checkedstring)
        {
            ObservableCollection<string> newo = new ObservableCollection<string>();
            if (!string.IsNullOrEmpty(checkedstring))
            {
                if (checkedstring.Contains(","))
                {
                    string[] m = checkedstring.Split(',');
                    for (int i = 0; i < m.Length; i++)
                    {
                        newo.Add(m[i]);
                    }
                }
                else
                {
                    newo.Add(checkedstring);
                }
            }
            return Tuple.Create(newo, checkedstring);
        }








        public ObservableCollection<string> SetListValues(string values)
        {
            ObservableCollection<string> mydata = new ObservableCollection<string>();/*new ObservableCollection<string>{"Timolol", "Betaxolol", "Dorzolamide", "Azopt", "Cobiosopt", "Zilomol",
                "Latanoprost", "Brimonidine", "Diamox" };*/
            if (!string.IsNullOrEmpty(values))
            {
                if (values.Contains(","))
                {
                    string[] ss = values.Split(',');
                    for (int i = 0; i < ss.Length; i++)
                        mydata.Add(ss[i]);
                }
                if (!values.Contains(","))
                {
                    mydata.Add(values);
                }
            }
            return mydata;
        }
        public string SumListContains(ObservableCollection<string> mylist)
        {
            string res = "";
            if (mylist != null && mylist.Count > 0)
            {
                for (int i = 0; i < mylist.Count; i++)
                {
                    res += mylist[i] + ",";
                }
                res = res.Remove(res.Length - 1, 1);
            }
            return res;
        }
        

    }

    public class DropDownItems
    {
        public int? ID { get; set; }
        public string Value { get; set; }
        public override string ToString()
        {
            return Value;
        }
    }

}
