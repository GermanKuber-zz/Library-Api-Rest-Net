using System.Collections.Generic;

namespace Library.Data.Services
{
    //TODO : 05 - Implemento la interface en el objeto que se encarga de mapear las propiedades
    public class PropertyMapping<TSource, TDestination> : IPropertyMapping
    {
        public Dictionary<string, PropertyMappingValue> _mappingDictionary { get; private set; }
        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            _mappingDictionary = mappingDictionary;
        }
    }
}
