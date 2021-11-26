using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Filter_Helpers
{
    public static class FilterHelper<TEntity> where TEntity : class
    {
        public static PropertyDescriptor SortProperty(string sort, out bool isDesc)
        {
            Type type = typeof(TEntity);
            List<string> ls = new List<string>();
            type.GetProperties().ToList().ForEach(i => ls.Add(i.Name.ToString()));
            isDesc = false;
            if (sort.Contains("-"))
            {
                string field = sort.Replace("-", "");
                bool isExist = ls.Any(i => i.ToLower() == field.ToLower());
                if (isExist)
                {
                    PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TEntity)).Find(field, true);
                    isDesc = true;
                    return property;
                }
            }
            else
            {
                bool isExist = ls.Any(i => i.ToLower() == sort.ToLower());
                if (isExist)
                {
                    PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TEntity)).Find(sort, true);
                    return property;
                }
            }
            return null;
        }
        public static string FilterCondition(string filter)
        {
            Type type = typeof(TEntity);
            List<string> columnNames = new List<string>();
            type.GetProperties().ToList().ForEach(i => columnNames.Add(i.Name.ToString()));
            StringBuilder builder = new StringBuilder();
            if (filter.Contains("&&") && filter.Contains("||"))
            {
                string[] multiConditions = filter.Split("&&");
                int multiCnt = multiConditions.Count();
                foreach (var multiCondition in multiConditions)
                {
                    if (multiCondition.Contains("||"))
                    {
                        string[] conditions = multiCondition.Split("||");
                        int cnt = conditions.Count();
                        foreach (var condition in conditions)
                        {
                            Filters(condition, columnNames.ToArray(), builder);
                            if (conditions[cnt - 1] != condition)
                                builder.Append(" OR ");
                        }
                    }
                    else if(!multiCondition.Contains("||") && !multiCondition.Contains("&&"))
                    {
                        Filters(multiCondition, columnNames.ToArray(), builder);
                    }
                    if ((multiConditions[multiCnt - 1] != multiCondition))
                        builder.Append(" AND ");
                }
            }
            if (filter.Contains("&&") && !filter.Contains("||"))
            {
                string[] conditions = filter.Split("&&");
                int cnt = conditions.Count();
                foreach (var condition in conditions)
                {
                    Filters(condition,columnNames.ToArray(), builder);
                    if(conditions[cnt-1] != condition)
                        builder.Append(" AND ");
                }
            }
            else if (!filter.Contains("&&") && filter.Contains("||"))
            {
                string[] conditions = filter.Split("||");
                int cnt = conditions.Count();
                foreach (var condition in conditions)
                {
                    Filters(condition, columnNames.ToArray(), builder);
                    if (conditions[cnt - 1] != condition)
                        builder.Append(" OR ");
                }
            }
            else
            {
                Filters(filter, columnNames.ToArray(), builder);
            }
            return builder.ToString();
        }
        public static void Filters(string condition,string[] columnNames,StringBuilder builder)
        {
            if (condition.Contains(">="))
            {
                string[] prop = condition.Split(">=");
                bool isExist = columnNames.Any(i => i.ToLower() == prop[0].Trim().ToLower());
                if (isExist)
                {
                    PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TEntity)).Find(prop[0].Trim(), true);
                    if (IsInteger(prop[1].Trim())) builder.Append($"{property.Name} >= {prop[1].Trim()}");
                }
            }
            else if (condition.Contains("<="))
            {
                string[] prop = condition.Split("<=");
                bool isExist = columnNames.Any(i => i.ToLower() == prop[0].Trim().ToLower());
                if (isExist)
                {
                    PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TEntity)).Find(prop[0].Trim(), true);
                    if (IsInteger(prop[1].Trim())) builder.Append($"{property.Name} <= {prop[1].Trim()}");
                }
            }
            else if (condition.Contains(">"))
            {
                string[] prop = condition.Split(">");
                bool isExist = columnNames.Any(i => i.ToLower() == prop[0].Trim().ToLower());
                if (isExist)
                {
                    PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TEntity)).Find(prop[0].Trim(), true);
                    if (IsInteger(prop[1].Trim())) builder.Append($"{property.Name} > {prop[1].Trim()}");
                }
            }
            else if (condition.Contains("<"))
            {
                string[] prop = condition.Split("<");
                bool isExist = columnNames.Any(i => i.ToLower() == prop[0].Trim().ToLower());
                if (isExist)
                {
                    PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TEntity)).Find(prop[0].Trim(), true);
                    if (IsInteger(prop[1].Trim())) builder.Append($"{property.Name} < {prop[1].Trim()}");
                }
            }
        
            else if (condition.Contains("="))
            {
                string[] prop = condition.Split("=");
                bool isExist = columnNames.Any(i => i.ToLower() == prop[0].Trim().ToLower());
                if (isExist)
                {
                    PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TEntity)).Find(prop[0].Trim(), true);
                    if (property.PropertyType == typeof(string) || property.PropertyType == typeof(char)) builder.Append($"{property.Name} = '{prop[1].Trim()}'");
                    else builder.Append($"{property.Name} = {prop[1].Trim()}");
                }
            }
            else if (condition.Contains("*"))
            {
                string[] prop = condition.Split("*");
                bool isExist = columnNames.Any(i => i.ToLower() == prop[0].Trim().ToLower());
                if (isExist)
                {
                    PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TEntity)).Find(prop[0].Trim(), true);
                    builder.Append($"{property.Name} Like('%{prop[1].Trim()}%')");
                }
            }
        }
        public static string TextSearchSortName(string sort, out bool isDesc)
        {
            Type type = typeof(TEntity);
            List<string> columnNames = new List<string>();
            type.GetProperties().ToList().ForEach(i => columnNames.Add(i.Name.ToString()));
            isDesc = false;
            if (sort.Contains("-"))
            {
                string field = sort.Replace("-", "");
                bool isExist = columnNames.Any(i => i.ToLower() == field.Trim().ToLower());
                if (isExist)
                {
                    PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TEntity)).Find(field.Trim(), true);
                    isDesc = true;
                    return property.Name;
                }
            }
            else
            {
                bool isExist = columnNames.Any(i => i.ToLower() == sort.ToLower());
                if (isExist)
                {
                    PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TEntity)).Find(sort.Trim(), true);
                    return property.Name;
                }
            }
            return null;
        }
        public static object Select(List<TEntity> entities, string fields)
        {

            Type type = typeof(TEntity);
            List<string> columnNames = new List<string>();
            type.GetProperties().ToList().ForEach(i => columnNames.Add(i.Name.ToString()));
            List<IDictionary<string, dynamic>> keyValues = new List<IDictionary<string, dynamic>>();
            PropertyInfo[] Props = type.GetProperties();
            if (entities == null) return null;
            foreach (var item in entities)
            {
                IDictionary<string, dynamic> value = new Dictionary<string, dynamic>();
                if (string.IsNullOrEmpty(fields))
                {
                    foreach (string field in columnNames)
                    {
                        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TEntity)).Find(field, true);
                        value.Add(property.Name, property.GetValue(item));
                    }
                }
                else
                {
                    foreach (string field in fields.Split(","))
                    {
                        bool isExist = columnNames.Any(i => i.ToLower() == field.Trim().ToLower());
                        if (isExist)
                        {
                            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(TEntity)).Find(field.Trim(), true);
                            value.Add(property.Name, property.GetValue(item));
                        }
                    }
                }

                keyValues.Add(value);
            }
            return keyValues;
        }
        public static bool IsInteger(string val)
        {
            try
            {
                Convert.ToInt64(val);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }

}
