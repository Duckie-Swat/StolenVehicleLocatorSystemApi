namespace StolenVehicleLocatorSystem.Contracts
{
    public class PagedModel<TModel>
    {
        private const int _maxPageSize = 50;
        private int _pageSize;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > _maxPageSize) ? _maxPageSize : value;
        }

        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public IList<TModel> Items { get; set; }

        public PagedModel()
        {
            Items = new List<TModel>();
        }
    }
}