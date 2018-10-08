using System;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Threading;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DbModel.Context;
using DbModel.Services.Interfaces;
using System.Diagnostics.Contracts;
using DbModel.Services;
using DbModel.DomainClasses.Enum;

namespace DbModel.ViewModel.WordsVM
{
    public class ChartVM
    {
        IUnitOfWork _uow;
        private IWords word { set; get; }

        public ChartVM()
        {

        }
        public ChartVM(IUnitOfWork uw)
        {
            //Contract.Requires(model != null);
            _uow = uw;
            word = new WordsService(_uow);
        }
        public double wt1()
        {
            return word.WordCount(WordType.Numberslitle10);
        }
        public double wt2()
        {
            return word.WordCount(WordType.Numberslarger10);
        }
        public double wt3()
        {
            return word.WordCount(WordType.Letters);
        }
        public double wt4()
        {
            return word.WordCount(WordType.Words_By_Signs);
        }
        public double wt5()
        {
            return word.WordCount(WordType.Words_By_Letters);
        }
        public double wt6()
        {
            return word.WordCount(WordType.Sentences_By_Words);
        }
        public double wt7()
        {
            return word.WordCount(WordType.Sentences_By_Signs);
        }
        public double wt8()
        {
            return word.WordCount(WordType.Arbitrary_Sentences);
        }

    }
}