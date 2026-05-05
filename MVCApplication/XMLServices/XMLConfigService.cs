using MVCApplication.Models.Config;
using System.Xml;
using System.Xml.Linq;

namespace MVCApplication.XMLServices
{
    public class XMLConfigService : IXMLConfigService
    {
        private readonly string _configFolder;
        private List<Feature> _features = new List<Feature>();
        private List<Menu> _menus = new List<Menu>();

        public XMLConfigService(IHostEnvironment env)
        {
            _configFolder = Path.Combine(env.ContentRootPath, "Config");
             LoadAll();
        }

        private XmlReaderSettings SafeSettings()
        {
            return new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null
            };
        }

        private void LoadAll()
        {
            _features = LoadFeatures();
            _menus = LoadMenus();
        }

        private List<Feature> LoadFeatures()
        {
            string file = Path.Combine(_configFolder, "features.xml");
            if (!File.Exists(file)) return new List<Feature>();

            try
            {
                using var reader = XmlReader.Create(file, SafeSettings());
                XDocument doc = XDocument.Load(reader);

                return doc.Root?
                    .Elements()
                    .Where(x => string.Equals(x.Name.LocalName, "feature", StringComparison.OrdinalIgnoreCase))
                    .Select(x => new Feature
                    {
                        Id = (string?)x.Attribute("id") ?? string.Empty,
                        Enabled = bool.TryParse((string?)x.Attribute("enabled"), out var e) ? e : false
                    })
                    .ToList()
                    ?? new List<Feature>();
            }
            catch
            {
                return _features;
            }
        }

        private List<Menu> LoadMenus()
        {
            string file = Path.Combine(_configFolder, "menus.xml");
            if (!File.Exists(file)) return new List<Menu>();
            try
            {
                using var reader = XmlReader.Create(file, SafeSettings());
                XDocument doc = XDocument.Load(reader);

                return doc.Root?
                    .Elements()
                    .Where(x => string.Equals(x.Name.LocalName, "menu", StringComparison.OrdinalIgnoreCase))
                    .Select(x => new Menu
                    {
                        Id = (string?)x.Attribute("id") ?? string.Empty,
                        Items = x.Elements()
                            .Where(i => string.Equals(i.Name.LocalName, "item", StringComparison.OrdinalIgnoreCase))
                            .Select(i => new MenuItem
                            {
                                 Text = (string?)i.Attribute("text") ?? string.Empty,
                                 Url = (string?)i.Attribute("url") ?? string.Empty,
                                 Order = int.TryParse((string?)i.Attribute("order"), out var o) ? o : 0
                            })
                            .OrderBy(i => i.Order)
                            .ToList()
                    })
                    .ToList()
                    ?? new List<Menu>();
            }
            catch
            {
                return _menus;
            }
        }

        public IReadOnlyList<Feature> GetFeatures()
        {
            return _features;
        }

        public Feature? GetFeature(string id)
        {
            return _features.FirstOrDefault(f => f.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public IReadOnlyList<Menu> GetMenus()
        {
            return _menus;
        }

        public Menu? GetMenu(string id)
        {
            return _menus.FirstOrDefault(m => m.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }
    }
}
