using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGL_CMS_Domain.Models;

namespace CursorBasedPagination_Helpers
{
    public static class PageNationHelper<T> where T : class
    {
        public static IEnumerable<T> ToPagedList(IQueryable<T> values, int after, int before)
        {
            if (values != null && before > 0 && (after > before)) return values.Skip(before).Take(after).ToList();
            else return null;
        }
        public static IEnumerable<T> ToPagedList(List<T> values, int after, int before)
        {
            if (values != null && before > 0 && (after > before)) return values.Skip(before).Take(after).ToList();
            else return null;
        }
    }
    public static class PageNationHelper
    {
        public static PageNationParams GetPageNationParams(long limit, long beforeVal, long afterVal, Uri uri, string filter = null, string fields = null,string sort= null,string textSearch = null) // text: optional parameter
        {
            PageNationParams paging = new PageNationParams();
            paging.Limit = limit;
            string textSearchQueryStr = String.IsNullOrEmpty(textSearch) ? string.Empty : "&search=" + textSearch;
            string filterQueryStr = String.IsNullOrEmpty(filter) ? string.Empty : "&filter=" + filter;
            string fieldsQueryStr = String.IsNullOrEmpty(fields) ? "" : "&fields=" + fields.Replace(" ", string.Empty);
            string sortsQueryStr = String.IsNullOrEmpty(sort) ? "" : "&sort=" + sort;
            var before = (beforeVal - limit) <= 0 ? 0 : (beforeVal - limit);
            var after = (afterVal - limit) <= 0 ? limit : (afterVal - limit);
            paging.Cursors = new Cursors()
            {
                Before = EncodeValue($"{before}"),
                After = EncodeValue($"{after}")
            };
            paging.Previous = new Uri($"{uri.AbsoluteUri}?paging=" + $"{limit}%%{paging.Cursors.Before}%%{paging.Cursors.After}{filterQueryStr}{fieldsQueryStr}{sortsQueryStr}{textSearchQueryStr}");

            var maxBefore = (beforeVal + limit);
            var maxAfter = (afterVal + limit);

            paging.Cursors = new Cursors()
            {
                Before = EncodeValue($"{maxBefore}"),
                After = EncodeValue($"{maxAfter}")
            };
            paging.Next = new Uri($"{uri.AbsoluteUri}?paging=" + $"{limit}%%{paging.Cursors.Before}%%{paging.Cursors.After}{filterQueryStr}{fieldsQueryStr}{sortsQueryStr}{textSearchQueryStr}");

            return paging;
        }
        public static void GetDecodedValues(string paging, out long beforeVal, out long afterVal, out long limit)
        {
            string[] values = new string[3];
            if (paging != null) values = paging.Split("%%").ToArray();
            limit = Convert.ToInt64(values[0]);
            beforeVal = 0;
            afterVal = 0;

            if (values.Count() > 1)
            {
                beforeVal = Convert.ToInt64(PageNationHelper.DecodeValue(values[1].ToString()));
                afterVal = Convert.ToInt64(PageNationHelper.DecodeValue(values[2].ToString()));
            }
            if (afterVal <= 0) afterVal = limit;
            if (beforeVal <= 0) beforeVal = 0;
        }
        public static string EncodeValue(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        public static long DecodeValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            return Convert.ToInt64(Encoding.UTF8.GetString(Convert.FromBase64String(value)));
        }
    }
}
