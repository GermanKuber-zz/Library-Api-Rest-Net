namespace Library.Api.Helper
{
    public class AuthorsResourceParameters
    {
        const int maxPageSize = 20;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
        public string Genre { get; set; }

        public string SearchQuery { get; set; }

        //TODO : 01 - Agrego propieadad mediante la cual ordenar mis Autores
        public string OrderBy { get; set; } = "Name";
        public string Fields { get; set; }
    }
}
