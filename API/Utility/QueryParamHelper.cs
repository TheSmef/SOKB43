﻿using DocumentFormat.OpenXml.Spreadsheet;
using Models.QuerySupporter;
using System.Linq.Dynamic.Core;

namespace API.Utility
{
    public static class QueryParamHelper
    {

        public static IQueryable<T> SetParams<T>(IQueryable<T> items, QuerySupporter query)
        {
            if (!string.IsNullOrEmpty(query.Filter))
            {
                if (query.FilterParams != null)
                {
                    items = items.Where(query.Filter, query.FilterParams);
                }
                else
                {
                    items = items.Where(query.Filter);
                }
            }
            if (!string.IsNullOrEmpty(query.OrderBy))
            {
                items = items.OrderBy(query.OrderBy);
            }
            return items;
        }
    }
}