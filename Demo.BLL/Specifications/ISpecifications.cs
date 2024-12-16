﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Specifications
{
    public interface ISpecifications<T> where T : class
    {
        Expression<Func<T,bool>> Criteria { get; set; }
        List<Expression<Func<T,object>>> Includes { get; set; }
    }
}
