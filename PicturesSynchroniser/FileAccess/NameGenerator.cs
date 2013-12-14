using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PicturesSynchroniser.Models;

namespace PicturesSynchroniser.FileAccess
{
    internal class NameGenerator
    {
        internal static void SetNewDateTime(PictureModel picture, CameraTimeSynchroniserModel pictureTimeSynchroniser)
        {
            var newDateTime = new DateTime(picture.Year, picture.Month, picture.Day, picture.Hour, picture.Minute, picture.Second);
            newDateTime.AddYears(pictureTimeSynchroniser.YearDifference);
            newDateTime.AddMonths(pictureTimeSynchroniser.MonthDifference);
            newDateTime.AddDays(pictureTimeSynchroniser.DayDifference);
            newDateTime.AddHours(pictureTimeSynchroniser.HourDifference);
            newDateTime.AddMinutes(pictureTimeSynchroniser.MinuteDifference);
            newDateTime.AddSeconds(pictureTimeSynchroniser.SecondDifference);

            picture.NewDateTime = newDateTime;
        }

        internal static void SetPicturesNewFileName(List<PictureModel> pictures, FileNameTemplateModel fileNameTemplate)
        {
            int day = 1;
            int enumeration = 1;
            int picturesCount = pictures.Count();
            int namingErrorCount = 0;

            for (int i = 0; i < picturesCount; i++)
            {
                StringBuilder newFileName = new StringBuilder();
                newFileName.Append(fileNameTemplate.Keyword);

                AddDayToName(pictures, i, newFileName, ref day, fileNameTemplate);
                AddDateToName(pictures, i, newFileName, fileNameTemplate);
                AddTimeToName(pictures, i, newFileName, fileNameTemplate);
                AddEnumerationToName(newFileName, picturesCount, ref enumeration, fileNameTemplate);
                AddFileType(pictures, i, newFileName);

                pictures[i].NewFileName = newFileName.ToString();
                bool isNewNameUnique = CheckIfNameIsUnique(pictures, pictures[i]);
                while (!isNewNameUnique)
                {
                    namingErrorCount++;
                    ChangeNewNameAgain(pictures[i], namingErrorCount);
                    isNewNameUnique = CheckIfNameIsUnique(pictures, pictures[i]);
                }
            }
        }

        internal static void SetPicturesUserDefinedFileName(IEnumerable<PictureModel> sortedPicturesByDateTime)
        {
            foreach (var picture in sortedPicturesByDateTime)
            {
                if (picture.NewFileName.Contains("."))
                {
                    picture.UserDefinedFileName = picture.NewFileName.Substring(
                        0, picture.NewFileName.LastIndexOf('.'));
                }
                else
                {
                    picture.UserDefinedFileName = picture.NewFileName;
                }
            }
        }

        internal static void SetNewNameToInvalidCameraPictures(
            IEnumerable<PictureModel> invalidCameraPictures,
            List<PictureModel> sortedPicturesByDateTime)
        {
            int namingErrorCount = 0;

            foreach (var picture in invalidCameraPictures)
            {
                picture.IsNamedAsUserDefined = false;
                picture.NewFileName = picture.OriginalFileName;

                bool isNewNameUniqueInInvalid = CheckIfNameIsUnique(invalidCameraPictures, picture);
                bool isNewNameUniqueInSorted = CheckIfNameIsUnique(sortedPicturesByDateTime, picture);
                while (!(isNewNameUniqueInInvalid == true && isNewNameUniqueInSorted == false))
                {
                    namingErrorCount++;
                    ChangeNewNameAgain(picture, namingErrorCount);
                    isNewNameUniqueInInvalid = CheckIfNameIsUnique(invalidCameraPictures, picture);
                    isNewNameUniqueInSorted = CheckIfNameIsUnique(sortedPicturesByDateTime, picture);
                }
            }
        }
        
        internal static string GetLongestKeyword(string[] pathParts)
        {
            var matches = Regex.Matches(
                pathParts[pathParts.Length - 2], @"([a-z]+)",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);

            if (matches.Count == 0)
            {
                return "Picture";
            }
            else if (matches.Count == 1)
            {
                return matches[0].Value;
            }
            else
            {
                string longestMatch = string.Empty;
                foreach (var match in matches)
                {
                    if (match.ToString().Length > longestMatch.Length)
                    {
                        longestMatch = match.ToString();
                    }
                }

                return longestMatch;
            }
        }

        private static void AddDayToName(List<PictureModel> pictures, int index,
            StringBuilder newFileName, ref int day, FileNameTemplateModel fileNameTemplate)
        {
            if (fileNameTemplate.DayIncluded == FileNameTemplateStrings.DayYes)
            {
                newFileName.Append(" Day ");
                if (index > 0)
                {
                    if (pictures[index - 1].NewDateTime.Day != pictures[index].NewDateTime.Day)
                    {
                        day += (int)(pictures[index].NewDateTime - pictures[index - 1].NewDateTime).TotalDays;
                    }

                    newFileName.Append(day);
                }
                else
                {
                    newFileName.Append(day);
                }
            }
        }

        private static void AddDateToName(List<PictureModel> pictures, int index,
            StringBuilder newFileName, FileNameTemplateModel fileNameTemplate)
        {
            if (fileNameTemplate.DateIncluded == FileNameTemplateStrings.DateYearMonthDay)
            {
                newFileName.Append(" ");
                newFileName.Append(pictures[index].Year.ToString("D4"));
                newFileName.Append(".");
                newFileName.Append(pictures[index].Month.ToString("D2"));
                newFileName.Append(".");
                newFileName.Append(pictures[index].Day.ToString("D2"));
            }
            else if (fileNameTemplate.DateIncluded == FileNameTemplateStrings.DateMonthDay)
            {
                newFileName.Append(" ");
                newFileName.Append(pictures[index].Month.ToString("D2"));
                newFileName.Append(".");
                newFileName.Append(pictures[index].Day.ToString("D2"));
            }
        }

        private static void AddTimeToName(List<PictureModel> pictures, int index,
            StringBuilder newFileName, FileNameTemplateModel fileNameTemplate)
        {
            if (fileNameTemplate.TimeIncluded == FileNameTemplateStrings.TimeHourMinuteSecondMillisecond)
            {
                newFileName.Append(" ");
                newFileName.Append(pictures[index].Hour.ToString("D2"));
                newFileName.Append(".");
                newFileName.Append(pictures[index].Minute.ToString("D2"));
                newFileName.Append(".");
                newFileName.Append(pictures[index].Second.ToString("D2"));
                newFileName.Append(" ");
                newFileName.Append(pictures[index].Millisecond.ToString("D2"));
            }
            else if (fileNameTemplate.TimeIncluded == FileNameTemplateStrings.TimeHourMinuteSecond)
            {
                newFileName.Append(" ");
                newFileName.Append(pictures[index].Hour.ToString("D2"));
                newFileName.Append(".");
                newFileName.Append(pictures[index].Minute.ToString("D2"));
                newFileName.Append(".");
                newFileName.Append(pictures[index].Second.ToString("D2"));
            }
            else if (fileNameTemplate.TimeIncluded == FileNameTemplateStrings.TimeHourMinute)
            {
                newFileName.Append(" ");
                newFileName.Append(pictures[index].Hour.ToString("D2"));
                newFileName.Append(".");
                newFileName.Append(pictures[index].Minute.ToString("D2"));
            }
        }

        private static void AddEnumerationToName(StringBuilder newFileName, int picturesCount,
            ref int enumeration, FileNameTemplateModel fileNameTemplate)
        {
            if (fileNameTemplate.EnumerationIncluded == "Yes")
            {
                newFileName.Append(" ");
                if (picturesCount < 1000)
                {
                    newFileName.Append(enumeration.ToString("D3"));
                }
                else if (picturesCount < 10000)
                {
                    newFileName.Append(enumeration.ToString("D4"));
                }
                else if (picturesCount < 100000)
                {
                    newFileName.Append(enumeration.ToString("D5"));
                }
                else if (picturesCount < 1000000)
                {
                    newFileName.Append(enumeration.ToString("D6"));
                }

                enumeration++;
            }
        }

        private static void AddFileType(List<PictureModel> pictures, int index, StringBuilder newFileName)
        {
            newFileName.Append(pictures[index].StorageFile.FileType.ToLower());
        }

        private static bool CheckIfNameIsUnique(IEnumerable<PictureModel> pictures, PictureModel picture)
        {
            var occurrences = pictures.Where(p => p.NewFileName.ToLower() == picture.NewFileName.ToLower());

            if (occurrences == null)
            {
                return false;
            }

            if (occurrences.Count() == 1)
            {
                return true;
            }

            return false;
        }

        private static void ChangeNewNameAgain(PictureModel picture, int namingErrorCount)
        {
            picture.IsNamedAsUserDefined = false;
            string pictureName = picture.NewFileName;
            var splittedName = pictureName.Split('.');
            splittedName[splittedName.Length - 2] += " collision ";
            splittedName[splittedName.Length - 2] += namingErrorCount.ToString("D3");

            var random = new Random();
            for (int i = 0; i < 3; i++)
            {
                splittedName[splittedName.Length - 2] += (char)('a' + random.Next(0, 26));
            }

            picture.NewFileName = string.Join(".", splittedName);
        }
    }
}