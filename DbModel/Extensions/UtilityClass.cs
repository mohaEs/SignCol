using DbModel.DomainClasses.Enum;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DbModel.Extensions
{
    public static class UtilityClass
    {
        public static int WordTypeToInt(WordType wt)
        {
            int i = 0;
            switch (wt)
            {
                case WordType.Numberslitle10: i = 0; break;
                case WordType.Numberslarger10: i = 1; break;
                case WordType.Letters: i = 2; break;
                case WordType.Words_By_Signs: i = 3; break;
                case WordType.Words_By_Letters: i = 4; break;
                case WordType.Sentences_By_Words: i = 5; break;
                case WordType.Sentences_By_Signs: i = 6; break;
                case WordType.Arbitrary_Sentences: i = 7; break;
            }
            return i;
        }
        public static WordType IntToWordType(int ty)
        {
            WordType i = WordType.Arbitrary_Sentences;
            switch (ty)
            {
                case 0: i = WordType.Numberslitle10; break;
                case 1: i = WordType.Numberslarger10; break;
                case 2: i = WordType.Letters; break;
                case 3: i = WordType.Words_By_Signs; break;
                case 4: i = WordType.Words_By_Letters; break;
                case 5: i = WordType.Sentences_By_Words; break;
                case 6: i = WordType.Sentences_By_Signs; break;
                case 7: i = WordType.Arbitrary_Sentences; break;
            }
            return i;
        }
        public static string IntToWordTypeString(WordType ty)
        {
            string i = "";
            switch (ty)
            {
                case WordType.Numberslitle10: i = "Numberslitle10"; break;
                case WordType.Numberslarger10: i = "Numberslarger10"; break;
                case WordType.Letters: i = "Letters"; break;
                case WordType.Words_By_Signs: i = "WordsBySigns"; break;
                case WordType.Words_By_Letters: i = "WordsByLetters"; break;
                case WordType.Sentences_By_Words: i = "SentencesByWords"; break;
                case WordType.Sentences_By_Signs: i = "SentencesBySigns"; break;
                case WordType.Arbitrary_Sentences: i = "ArbitrarySentences"; break;
            }
            return i;
        }
        public static bool CheckIntHasValue(int v)
        {
            int? vv = v;
            if (vv.HasValue)
                return true;
            return false;
        }
        public static T ParseEnum<T>(string value)
        {
            return (T)System.Enum.Parse(typeof(T), value, true);
        }
        public static string ConnectionStringPath
        {
            get
            {
                string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\Database.mdf");

                return assemblyPath;
            }
        }
        public static bool checkInt(string value)
        {
            int res;
            bool r = false;
            if (int.TryParse(value, out res))
                r = true;
            return r;
        }
        public static bool checkInt2(object value)
        {
            int newint = 0;
            if (value == null)
                return false;
            try
            {
                newint = Convert.ToInt32(value);
                return true;
            }
            catch { }
            return false;
        }

        public static string RemoveSpecialChars(string str, string schar)
        {
            char[] cha = schar.ToCharArray();
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(str) && str.Contains(schar))
            {
                foreach (char c in str)
                {
                    for (int i = 0; i < cha.Length; i++)
                        if (!c.Equals(cha[i]))
                            sb.Append(c);
                }
                return sb.ToString();
            }
            else
                return str;
        }
        public static string ConvertIntToDate(object val)
        {
            if (val != null)
            {
                string res = val.ToString();
                if (res.Length > 1)
                    res = res.Substring(0, 4) + "/" + res.Substring(4, 2) + "/" + res.Substring(6, 2);
                else
                    res = "";
                return res;
            }
            else
                return "";
        }
        public static ObservableCollection<string> ItemSeperator(string val)
        {
            List<string> li = new List<string>();
            char[] ch = val.ToCharArray();

            if (ch.Length > 0)
            {
                for (int i = 0; i < ch.Length; i++)
                {
                    li.Add(ch[i].ToString());
                }
            }
            return li.ToObservableCollection();
        }
        public static string ComposeToString(IList<string> list)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                sb.Append(list[i]);
            }
            return sb.ToString();
        }

        public static string newWriteSelectedAnimalsString(IList<string> list)
        {
            if (list.Count == 0)
                return String.Empty;

            StringBuilder builder = new StringBuilder(list[0]);

            for (int i = 1; i < list.Count; i++)
            {
                builder.Append(", ");
                builder.Append(list[i]);
            }

            return builder.ToString();
        }
        public static string WriteSelectedAnimalsString(IList<string> list)
        {
            if (list.Count == 0)
                return String.Empty;

            StringBuilder builder = new StringBuilder(list[0]);

            for (int i = 1; i < list.Count; i++)
            {
                builder.Append(", ");
                builder.Append(list[i]);
            }

            return builder.ToString();
        }
        public static BitmapImage ConvertToImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
        public static byte[] ConvertToByteArray(BitmapImage image)
        {
            MemoryStream memStream = new MemoryStream();
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(memStream);
            return memStream.GetBuffer();
        }

        public static ImageSource ByteToImage(byte[] imageData)
        {
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(imageData);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();
            ImageSource imgSrc = biImg as ImageSource;
            return imgSrc;
        }
        public static string ImageToByte(FileStream fs)
        {
            byte[] imgBytes = new byte[fs.Length];
            fs.Read(imgBytes, 0, Convert.ToInt32(fs.Length));
            string encodeData = Convert.ToBase64String(imgBytes, Base64FormattingOptions.InsertLineBreaks);

            return encodeData;
        }
        public static ImageSource ByteToStream(string destination)
        {
            //Stream reader = File.OpenRead(destination);
            //MessageBox.Show(destination);
            System.Drawing.Image photo = System.Drawing.Image.FromFile(destination);//.FromStream((Stream)reader);

            MemoryStream finalStream = new MemoryStream();
            photo.Save(finalStream, ImageFormat.Png);

            // translate to image source
            PngBitmapDecoder decoder = new PngBitmapDecoder(finalStream, 
                BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

            //BitmapImage biImg = new BitmapImage(;
            //MemoryStream ms = new MemoryStream(destination);
            //biImg.BeginInit();
            //biImg.StreamSource = ms;
            //biImg.EndInit();
            ImageSource imgSrc = decoder.Frames[0]; //biImg as ImageSource;
            return imgSrc;
        }
    }
}
