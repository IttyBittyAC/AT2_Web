using MVCApplication.Models.Config;

namespace MVCApplication.XMLServices
{
    /// <summary>
    /// Interface for XML configuration service that provides methods to retrieve features and menus from XML configuration files.
    /// </summary>
    public interface IXMLConfigService
    {
        IReadOnlyList<Feature> GetFeatures();
        Feature? GetFeature(string id);
        IReadOnlyList<Menu> GetMenus();
        Menu? GetMenu(string id);
        IReadOnlyList<Footer> GetFooters();
        Footer? GetFooter(string id);
        IReadOnlyList<Faq> GetFaqs();
        Faq? GetFaq(string id);
    }
}
