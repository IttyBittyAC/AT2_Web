using MVCApplication.Models.Config;

namespace MVCApplication.XMLServices
{
    public class XMLConfigService : IXMLConfigService
    {
        /// <summary>
        /// Retrieves a read-only list of all features defined in the XML configuration.
        /// </summary>
        /// <returns>A read-only list of <see cref="Feature"/> objects representing the features available in the configuration.
        /// The list is empty if no features are defined.</returns>
        /// <exception cref="NotImplementedException">The method is not implemented.</exception>
        IReadOnlyList<Feature> IXMLConfigService.GetFeatures()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the feature with the specified identifier from the configuration source.
        /// </summary>
        /// <param name="id">The unique identifier of the feature to retrieve. Cannot be null or empty.</param>
        /// <returns>A <see cref="Feature"/> instance representing the requested feature if found; otherwise, <see
        /// langword="null"/>.</returns>
        /// <exception cref="NotImplementedException">The method is not implemented.</exception>
        Feature? IXMLConfigService.GetFeature(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a read-only list of menus defined in the XML configuration.
        /// </summary>
        /// <returns>A read-only list of <see cref="Menu"/> objects representing the available menus. The list is empty if no
        /// menus are defined.</returns>
        /// <exception cref="NotImplementedException">Thrown if the method is not implemented.</exception>
        IReadOnlyList<Menu> IXMLConfigService.GetMenus()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the menu configuration associated with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the menu to retrieve. Cannot be null or empty.</param>
        /// <returns>A <see cref="Menu"/> object representing the menu configuration if found; otherwise, <see langword="null"/>.</returns>
        /// <exception cref="NotImplementedException">The method is not implemented.</exception>
        Menu? IXMLConfigService.GetMenu(string id)
        {
            throw new NotImplementedException();
        }
    }
}
