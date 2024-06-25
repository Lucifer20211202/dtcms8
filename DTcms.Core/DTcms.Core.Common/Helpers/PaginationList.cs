using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.Common.Helpers
{
    /// <summary>
    /// 分页帮助类
    /// </summary>
    public class PaginationList<T>
    {
        public List<T> Items { get; set; } = [];
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPrevious => PageIndex > 1;
        public bool HasNext => PageIndex < TotalPages;
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public PaginationList(int totalCount, int currentPage, int pageSize, List<T> items)
        {
            PageIndex = currentPage;
            PageSize = pageSize;
            Items.AddRange(items);
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        public static async Task<PaginationList<T>> CreateAsync(int currentPage, int pageSize, IQueryable<T> result)
        {
            var totalCount = await result.CountAsync();
            var skip = (currentPage - 1) * pageSize;
            result = result.Skip(skip);
            result = result.Take(pageSize);

            var items = await result.ToListAsync();

            return new PaginationList<T>(totalCount, currentPage, pageSize, items);
        }

        public static PaginationList<T> Create(int pageIndex, int pageSize, int totalCount, List<T> result)
        {
            return new PaginationList<T>(totalCount, pageIndex, pageSize, result);
        }
    }
}