using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectricalShop.Common
{
    public class ListPager
    {
        private int pageIndex;
        private int pageSize;
        private int pageTotal;
        private IEnumerable<object> collection;

        public ListPager()
        {
            this.pageIndex = 0;
            this.pageSize = 0;
            this.pageTotal = 0;
        }

        public ListPager(IEnumerable<object> collection, int pageIndex, int pageSize)
        {
            this.collection = collection;
            this.pageIndex = pageIndex;
            this.pageSize = pageSize;

            DoPaging();
        }

        private void DoPaging()
        {
            if (collection != null && collection.Count() > 0 && pageSize > 0)
            {
                // calculate total page
                pageTotal = collection.Count() / pageSize;
                if (collection.Count() % pageSize != 0)
                {
                    pageTotal++;
                }

                if (pageIndex > pageTotal)
                {
                    pageIndex = pageTotal;
                }
            }
            else
            {
                pageIndex = 0;
                pageSize = 0;
                pageTotal = 0;
            }
        }

        public int RowCount
        {
            get
            {
                if (this.collection != null)
                {
                    return this.collection.Count();
                }
                return 0;
            }
        }

        public int PageIndex
        {
            get
            {
                return pageIndex;
            }
            set
            {
                pageIndex = value;
                DoPaging();
            }
        }

        public int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = value;
                DoPaging();
            }
        }

        public IEnumerable<object> DataSource
        {
            get
            {
                return collection;
            }
            set
            {
                collection = value;
                DoPaging();
            }
        }

        public int PageTotal
        {
            get
            {
                return this.pageTotal;
            }
        }

        public IEnumerable<object> PagedData
        {
            get
            {
                if (collection != null)
                {
                    return collection.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }

                return null;
            }
        }
    }
}