using Models.Dto.StatsModels.ParamModels;
using Models.QuerySupporter;
using System.Text.Json.Serialization;

namespace WEB.Utility
{
    public static class QueryMapper
    {
        public static Dictionary<string, string> MapToQuery(QuerySupporter query)
        {
            var uriquery = new Dictionary<string, string>
                {
                        { "Filter", query.Filter! },
                        { "OrderBy", query.OrderBy! },
                        { "Top", query.Top!.ToString() },
                        { "Skip", query.Skip!.ToString() },
                };
            if (query.FilterParams != null)
            {
                foreach (string param in query.FilterParams)
                {
                    uriquery.Add("FilterParam", param);
                }
            }
            return uriquery;
        }

        public static Dictionary<string, string> MapToQuery(DateQuery query)
        {
            var uriquery = new Dictionary<string, string>
                {
                        { "StartDate", query.StartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                        { "EndDate", query.EndDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                };
            return uriquery;
        }
    }
}
