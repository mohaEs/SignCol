using System;
using System.ComponentModel.DataAnnotations;

namespace DbModel.DomainClasses.Enum
{
    public enum WordType : byte
    {
        [Display(Name = "Number < 10")]
        Numberslitle10,
        [Display(Name = "Number > 10")]
        Numberslarger10,
        [Display(Name = "Letter")]
        Letters,
        [Display(Name = "Word by Sign")]
        Words_By_Signs,
        [Display(Name = "Word by letters")]
        Words_By_Letters,
        [Display(Name = "Sentence by Words")]
        Sentences_By_Words,
        [Display(Name = "Sentence by Signs")]
        Sentences_By_Signs,
        [Display(Name = "Arbitrary Sentence")]
        Arbitrary_Sentences
    }
    public enum LeapKinnectType : byte
    {
        [Display(Name = "کینکت")]
        Kinnect,
        [Display(Name = "لیپ")]
        Leap
    }
}
