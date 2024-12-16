using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Specifications
{
    public static class SpecificationsEvalutor<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> InputQuery,ISpecifications<T> Spec)
        {
            var Query = InputQuery; //_dbContext.Set<T>
            if(Spec.Criteria != null)
            {
                Query = Query.Where(Spec.Criteria);
            }
            Query = Spec.Includes.Aggregate(Query , (CurrentQuery,IncludeExpression) => CurrentQuery.Include(IncludeExpression));

            return Query;
        }
    }
}
