using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalFinanceApp.Data
{
    public class Pagination<T>
    {
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 5;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public List<T> CurrentPageItems { get; set; } = new List<T>();

        public void SetPagination(List<T> items)
        {
            TotalItems = items.Count;
            CurrentPageItems = items.Skip(PageIndex * PageSize).Take(PageSize).ToList();
        }
        public void NextPage()
        {
            if (PageIndex < TotalPages - 1)
            {
                PageIndex++;
            }
        }
        public void PreviousPage()
        {
            if (PageIndex > 0)
            {
                PageIndex--;
            }
        }
        public void ResetToFirstPage()
        {
            PageIndex = 0;
        }

        public bool HasNextPage()
        {
            return PageIndex < TotalPages - 1;
        }

        public bool HasPreviousPage()
        {
            return PageIndex > 0;
        }
    }
}
