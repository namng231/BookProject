using Microsoft.EntityFrameworkCore;

namespace BookManagement.Dtos
{
    public class PagingListDto<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalItems { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public PagingListDto(List<T> items, int totalItems, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalItems = totalItems;
            TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            this.AddRange(items);
        }

        public static async Task<PagingListDto<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            int skipCount = (pageIndex - 1) * pageSize;
            var items = await source.Skip(skipCount).Take(pageSize).ToListAsync();
            var count = await source.CountAsync();

            return new PagingListDto<T>(items, count, pageIndex, pageSize);
        }
    }
}
