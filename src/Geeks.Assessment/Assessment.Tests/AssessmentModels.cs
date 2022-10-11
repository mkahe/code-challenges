using System.Collections.Generic;

namespace Assessment.Tests
{
    public class DataProvider<T> : IDataProvider<T>
    {
        public IDataProvider<T>.IDataProviderResponse GetData(string nextPageToken)
        {
            return new DataProviderResponse<T>();
        }

        public void Dispose()
        {
            //do nothing
        }
        
        public class DataProviderResponse<T> : IDataProvider<T>.IDataProviderResponse
        {
            public string NextPageToken { get; set; }
            public IEnumerable<T> Items { get; set; }
        }
    }

    public class Hierarchy : IHierarchy
    {
        public Hierarchy(string name, IHierarchy parent)
        {
            Name = name;
            Parent = parent;
        }
        public IHierarchy Parent { get; }
        public string Name { get; }
    }
}