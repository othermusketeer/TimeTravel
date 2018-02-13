using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

/*  Author: D'Artagnan Palmer
 *  SO User: https://stackoverflow.com/users/3453155/othermusketeer
 *  SU User: https://superuser.com/users/332891/othermusketeer
 *  Date Start: 2018-02-12
 *  Date Updated: 2018-02-12
 *  Copyright: Public Domain
 */

namespace TimeTravel
{
    class Program
    {
        private static bool forceTime;

        static int Main(string[] args)
        {
            // https://superuser.com/users/332891/othermusketeer
            // https://stackoverflow.com/users/3453155/othermusketeer
            string usageStr = "TimeTravel by D'Artagnan Palmer, Public Domain\n" +
                "Usage:\n" +
                "\tTimeTravel\t<YYYY>(/|.|-)<MM>(/|.|-)<DD>\n" +
                "\tTimeTravel\t<YYYY>(/|.|-)<MM>(/|.|-)<DD>( |.|-)<hh>(:|.|-)<mm>(:|.|-)<ss>\n"+
                "\tTimeTravel\t(+|-)<number>(Y|M|D)\n" +
                "\n";
            Regex reDate = new Regex(@"^\s*(?<Year>\d{4})[/\.\-](?<Month>\d+)[/\.\-](?<Day>\d+)\s*$", RegexOptions.IgnoreCase);
            Regex reDateTime = new Regex(@"^\s*(?<Year>\d{4})[/\.\-](?<Month>\d+)[/\.\-](?<Day>\d+)[ \.\-]+(?<Hour>\d+)[\:\.\-](?<Minute>\d+)[\:\.\-](?<Second>\d+)\s*$", RegexOptions.IgnoreCase);
            Regex reDateDiff = new Regex(@"^\s*(?<Sign>[\+\-]?)(?<Amount>\d+)\s*(?<Unit>[ymd])\s*$", RegexOptions.IgnoreCase);

            if (args.Length < 1)
            {
                System.Console.WriteLine("ERROR: Too few parameters!\n\n" + usageStr);
                return 1;
            }

            int iYear, iMonth, iDay, iHour, iMinute, iSecond;
            bool didit;
            string statusStr = "Not Started Yet ";
            string cmdLine = String.Join(" ",args);
            DateTime date;
            forceTime = false;
            SYSTEMTIME systime;

            DateTime nowTime = DateTime.Now;
            DateTime nowUTC = DateTime.UtcNow;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            DateTime wantTime = nowTime;
            DateTime wantUTC = nowUTC;


            System.Console.WriteLine(DateTime.Now.ToString());

            Match m = reDateTime.Match(cmdLine);
            if (m.Success) {
                iYear = Int32.Parse(m.Groups["Year"].Value);
                iMonth = Int32.Parse(m.Groups["Month"].Value);
                iDay = Int32.Parse(m.Groups["Day"].Value);
                iHour = Int32.Parse(m.Groups["Hour"].Value);
                iMinute = Int32.Parse(m.Groups["Minute"].Value);
                iSecond = Int32.Parse(m.Groups["Second"].Value);
                wantTime = new DateTime(iYear,iMonth,iDay,iHour,iMinute,iSecond);
                wantUTC = wantTime.ToUniversalTime();
                forceTime = true;
            } else
            {
                m = reDate.Match(cmdLine);
                if (m.Success) {
                    iYear = Int32.Parse(m.Groups["Year"].Value);
                    iMonth = Int32.Parse(m.Groups["Month"].Value);
                    iDay = Int32.Parse(m.Groups["Day"].Value);
                    iHour = nowTime.Hour;
                    iMinute = nowTime.Minute;
                    iSecond = nowTime.Second;
                    wantTime = new DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond);
                    wantUTC = wantTime.ToUniversalTime();
                    forceTime = true;
                } else
                {
                    m = reDateDiff.Match(cmdLine);
                    if (m.Success) {
                        // Currently only handles one modifier at a time
                        int incValue = Int32.Parse(m.Groups["Amount"].Value);
                        if (m.Groups["Sign"].Value == "-")
                        {
                            incValue = 0 - incValue;
                        }

                        if (m.Groups["Unit"].Value.ToLower() == "y")
                        {
                            wantTime = wantTime.AddYears(incValue);
                        }
                        if (m.Groups["Unit"].Value.ToLower() == "m")
                        {
                            wantTime = wantTime.AddMonths(incValue);
                        }
                        if (m.Groups["Unit"].Value.ToLower() == "d")
                        {
                            wantTime = wantTime.AddDays(incValue);
                        }
                        wantUTC = wantTime.ToUniversalTime();
                        forceTime = true;
                    }
                }
            }

            // Info on setting system date/time from: http://csharp.tips/tip/article/251-How-to-change-system-time-in-csharp-
            // Responding to Ctrl-C is from: https://stackoverflow.com/questions/177856/how-do-i-trap-ctrl-c-in-a-c-sharp-console-app
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                Program.forceTime = false;
            };
            didit = false;

            while (forceTime)
            {
                try
                {

                    System.Console.WriteLine(statusStr + "  | " + nowTime.AddMilliseconds(stopWatch.ElapsedMilliseconds).ToString() + " --> " + DateTime.Now.ToString());
                    System.Threading.Thread.Sleep(1000);
                    // Every second, reset the date-time to the fake one
                    date = wantUTC.AddMilliseconds(stopWatch.ElapsedMilliseconds);
                    systime = new SYSTEMTIME(date);
                    didit = SetSystemTime(ref systime);
                    if (didit)
                    {
                        statusStr = "Successfully Set";
                    } else
                    {
                        statusStr = "Error Setting   ";
                    }
                }
                catch (Exception)
                {
                    date = nowUTC.AddMilliseconds(stopWatch.ElapsedMilliseconds);
                    systime = new SYSTEMTIME(date);
                    SetSystemTime(ref systime);
                    stopWatch.Stop();

                    throw;
                }
            }

            // Attempt to reset the date-time
            date = nowUTC.AddMilliseconds(stopWatch.ElapsedMilliseconds);
            systime = new SYSTEMTIME(date);
            SetSystemTime(ref systime);
            stopWatch.Stop();
            return 0;
        }

        // https://stackoverflow.com/questions/3354893/how-can-i-convert-a-datetime-to-the-number-of-seconds-since-1970
        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        [DllImport("kernel32.dll")]
        static extern bool SetSystemTime(ref SYSTEMTIME time);

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public ushort Year;
            public ushort Month;
            public ushort DayOfWeek;
            public ushort Day;
            public ushort Hour;
            public ushort Minute;
            public ushort Second;
            public ushort Milliseconds;

            public SYSTEMTIME(DateTime dt)
            {
                Year = (ushort)dt.Year;
                Month = (ushort)dt.Month;
                DayOfWeek = (ushort)dt.DayOfWeek;
                Day = (ushort)dt.Day;
                Hour = (ushort)dt.Hour;
                Minute = (ushort)dt.Minute;
                Second = (ushort)dt.Second;
                Milliseconds = (ushort)dt.Millisecond;
            }
        }
    }
}
