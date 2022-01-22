using Dapper;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.DTOs.Models;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Extensions;
using Mvp24Hours.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAPI.Infrastructure.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class DapperExtensions
    {
        /// <summary>
        /// Perform query using dapper with pagination
        /// </summary>
        public static async Task<IPagingResult<IEnumerable<T>>> QueryPagingResultAsync<T>(this IDbConnection connection, PagingCriteriaRequest pagingCriteria = null, string whereSql = null, dynamic whereParams = null, string orderBySql = "id asc")
            where T : class
        {
            int limit = ConfigurationPropertiesHelper.MaxQtyByQueryPage;
            int offset = 0;

            // set pagination pattern
            if (pagingCriteria != null)
            {
                limit = pagingCriteria.Limit > 0 ? pagingCriteria.Limit : limit;
                offset = pagingCriteria.Offset;
            }

            return await QueryPagingResultAsync<T>(connection, limit, offset, whereSql, whereParams, orderBySql);
        }

        /// <summary>
        /// Perform query using dapper with pagination
        /// </summary>
        public static async Task<IPagingResult<IEnumerable<T>>> QueryPagingResultAsync<T>(this IDbConnection connection, int limit, int offset, string whereSql = null, dynamic whereParams = null, string orderBySql = "id asc")
            where T : class
        {
            // get number of rows with filter
            var sqlCountBuilder = new SqlBuilder();
            var sqlCount = sqlCountBuilder.AddTemplate($"select count(0) from {typeof(T).Name} /**where**/");

            // get list with pagination
            var sqlQueryBuilder = new SqlBuilder();
            var sqlQuery = sqlQueryBuilder.AddTemplate($"select * from {typeof(T).Name} /**where**/ /**orderby**/ limit @limit offset @offset");
            sqlQueryBuilder.AddParameters(new { limit, offset });

            // add filter
            if (whereSql.HasValue())
            {
                sqlCountBuilder.Where(whereSql, whereParams);
                sqlQueryBuilder.Where(whereSql, whereParams);
            }

            // add ordering
            sqlQueryBuilder.OrderBy(orderBySql);

            // get total lines and pages
            int totalCount = await connection.QuerySingleAsync<int>(sqlCount.RawSql, sqlCount.Parameters);
            var totalPages = (int)Math.Ceiling((double)totalCount / limit);

            // get paginated records
            var result = await connection.QueryAsync<T>(sqlQuery.RawSql, sqlQuery.Parameters);

            return result.ToBusinessPaging(
                new PageResult(limit, offset, result.Count()),
                new SummaryResult(totalCount, totalPages)
            );
        }
    }
}
