using System;
using System.IO;
using System.Net;

namespace GetOpenDataFromMoz
{
    class Program
    {
        static void Main()
        {
            var assets = new DataType(28, 5, 2020, "assets");
            var tests = new DataType(1, 6, 2020, "tests");
            var assetsEmerg = new DataType(18, 4, 2020, "assets-emerg");
        
            TimeSpanFileGetter.GetFilesFromStartTillToday(assets);
            TimeSpanFileGetter.GetFilesFromStartTillToday(tests);
            TimeSpanFileGetter.GetFilesFromStartTillToday(assetsEmerg);
        }
    }

    static class FileGetter
    {
        public static void GetFile(string fileUrl, string fileName)
        {
            WebClient client = new WebClient();

            client.DownloadFile(fileUrl, fileName);
        }
    }


    class DataType
    {
        public string BaseUrl { get; set; }
        public string Folder { get; set; }
        public Date StartDate { get; set; }

        public class Date
        {
            public int Day { get; set; }
            public int Month { get; set; }
            public int Year { get; set; }
        }

        public void CreateDataFolder()
        {

            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }
        }

        public DataType(int day, int month, int year, string folder)
        {
            StartDate = new Date();

            StartDate.Day = day;
            StartDate.Month = month;
            StartDate.Year = year;

            BaseUrl = Consts.BaseUrl;
            Folder = Path.Combine(Consts.BaseFolder, folder);

            CreateDataFolder();
        }
    }

    static class TimeSpanFileGetter
    {
        public static void GetFilesFromStartTillToday(DataType dataType)
        {
            DateTime today = DateTime.Now;
            DateTime startDay = new DateTime(dataType.StartDate.Year, dataType.StartDate.Month, dataType.StartDate.Day);

            TimeSpan timeSpan = today - startDay;

            for (int i = 0; i < timeSpan.Days; i++)
            {
                var day = startDay.AddDays(i);

                string fileName = new DirectoryInfo(dataType.Folder).Name + "_" + DateToString(day) + Consts.CsvFileExt;

                try
                {
                    if (File.Exists(Path.Combine(dataType.Folder, fileName)))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("File exsists, skipping: " + fileName);
                    }

                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        FileGetter.GetFile(dataType.BaseUrl + fileName, Path.Combine(dataType.Folder, fileName));
                        Console.WriteLine("Saved file: " + fileName);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Could not download file: " + fileName);
                    Console.WriteLine(ex);
                }
            }
        }

        static string DateToString(DateTime dateTime)
        {
            return string.Join('_', dateTime.Day.ToString().Length == 1 ? "0" + dateTime.Day.ToString() : dateTime.Day.ToString(), dateTime.Month.ToString().Length == 1 ? "0" + dateTime.Month.ToString() : dateTime.Month.ToString(), dateTime.Year.ToString().Substring(dateTime.Year.ToString().Length - 2));
        }
    }

    static class Consts
    {
        public const string BaseFolder = @"..\..\..\data";
        public const string CsvFileExt = ".csv";
        public const string BaseUrl = @"https://covid19.gov.ua/csv/";
    }

}
