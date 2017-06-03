namespace Library.Api.Helper
{
    //TODO : 03 - Declaro una clae con las propiedades y las validaciones para el QueryString
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
        //TODO : 14 - Agrego el género de la película para poder filtrarlo.
        public string Genre { get; set; }

        public string SearchQuery { get; set; }

    }
}
