using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectricalShop.Common
{
    public static class StringExtensions
    {
        public static String Repeat(string s, int count)
        {
            String strResult = "";
            for (int i=0; i<count; i++)
            {
                strResult += s;
            }
            return strResult;
        }

        public static string convertToUnSign(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}