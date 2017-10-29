using System;
using System.Linq.Expressions;

namespace VOffice.Core.Queries
{
    /// <summary>
    /// BaseQuery class is used to implement the Query Object Pattern    
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseQuery<T> : IPaginationQuery
    {
        public BaseQuery()
        {
            PageSize = 0;
            SortAscending = true;
        }

        public bool ComputePagesCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber
        {
            get; set;
        }
        public string SortColumn { get; set; }
        public bool SortAscending { get; set; }

        public bool PagingEnabled
        {
            get
            {
                return PageSize > 0;
            }
            set
            {
                if (value)
                {
                    if (PageSize == 0)
                        throw new Exception("PageSize should be set to a value grather than 0");
                }
                else
                    PageSize = 0;
            }
        }


        /// <summary>
        /// Offset for paging in SQL
        /// </summary>
        public int Start
        {
            get
            {
                return (PageNumber - 1) * PageSize;
            }
        }

        /// <summary>
        /// Take dùng trong phân trang SQL
        /// </summary>
        public int Limit
        {
            get
            {
                return PageSize;
            }
        }
        public virtual Expression<Func<T, bool>> ToExpression()
        {
            return x => true;
        }
    }
}
