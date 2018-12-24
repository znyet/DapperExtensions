using System.Collections.Generic;

namespace DapperExtensions
{
    public class PageEntity<T>
    {
        public IEnumerable<T> Data { get; set; }
        public object Total { get; set; }

    }
}
