using Microsoft.AspNetCore.Http.Features;

namespace VizsgaremekBackend.Dtos;


public class PagedDto<T>
{
   public PagedDto(IEnumerable<T> items, int pageCount)
   {
      this.items = items;
      this.pageCount = pageCount;
   }

   public IEnumerable<T> items { get; set; }
   public int pageCount { get; set; }
}