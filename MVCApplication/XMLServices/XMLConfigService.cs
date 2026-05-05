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
        private List<Footer> _footers = new List<Footer>();
        private List<Faq> _faqs = new List<Faq>();

        /// <summary>
        /// Construct the service and load all XML configuration from the Config folder.
        /// The IHostEnvironment is used to resolve the application's ContentRootPath so files
        /// are read from the deployed content root (e.g., MVCApplication/Config).
        /// </summary>
        public XMLConfigService(IHostEnvironment env)
        {
            _configFolder = Path.Combine(env.ContentRootPath, "Config");
            LoadAll();
        }

        /// <summary>
        /// Create secure XmlReaderSettings to avoid XML External Entity (XXE) attacks.
        /// We prohibit DTD processing and disable XmlResolver so external resources cannot be resolved.
        /// </summary>
        private XmlReaderSettings SafeSettings()
        {
            return new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null
            };
        }

        /// <summary>
        /// Load all configuration files into memory. Each loader returns a list of POCOs which
        /// are cached on the service instance. This method is safe to call on startup or when
        /// you implement a reload/watch mechanism.
        /// </summary>
        private void LoadAll()
        {
            _features = LoadFeatures();
            _menus = LoadMenus();
            _footers = LoadFooters();
            _faqs = LoadFaqs();
        }

        /// <summary>
        /// Load features from features.xml. The parser is case-insensitive to element
        /// and attribute names to accommodate lowercase XML. Uses XmlReader with SafeSettings
        /// for secure parsing. On parse errors, the method returns the previous cached value
        /// to avoid replacing a working configuration with an empty list.
        /// </summary>
        private List<Feature> LoadFeatures()
        {
            string file = Path.Combine(_configFolder, "features.xml");
            if (!File.Exists(file)) return new List<Feature>();

            try
            {
                // Use XmlReader to apply SafeSettings (prevents DTDs/XXE), then load into XDocument
                using var reader = XmlReader.Create(file, SafeSettings());
                XDocument doc = XDocument.Load(reader);

                // We use Name.LocalName and case-insensitive comparisons so XML can use lowercase names.
                return doc.Root?
                    .Elements()
                    .Where(x => string.Equals(x.Name.LocalName, "feature", StringComparison.OrdinalIgnoreCase))
                    .Select(x => new Feature
                    {
                        Id = (string?)x.Attribute("id") ?? string.Empty,
                        // Parse boolean attributes safely
                        Enabled = bool.TryParse((string?)x.Attribute("enabled"), out var e) ? e : false
                    })
                    .ToList()
                    ?? new List<Feature>();
            }
            catch
            {
                // Swallow exceptions intentionally to preserve previous values; consider logging.
                return _features;
            }
        }

        /// <summary>
        /// Load menus from menus.xml. Each <menu> contains multiple <item> elements.
        /// Items are ordered by the numeric "order" attribute. Attribute and element
        /// lookups are case-insensitive to match lowercase XML.
        /// </summary>
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
                                // Parse numeric order attribute with fallback
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
                // Preserve existing menu cache on error
                return _menus;
            }
        }

        /// <summary>
        /// Load footer layout from footer.xml. The expected structure is:
        /// <Footer>
        ///   <Columns>
        ///     <Column title="...">
        ///       <Link text="..." url="..." />
        ///     </Column>
        ///   </Columns>
        /// </Footer>
        /// The method returns a list to support multiple footer sets in the future, but
        /// the current XML typically contains a single footer section.
        /// </summary>
        private List<Footer> LoadFooters()
        {
            string file = Path.Combine(_configFolder, "footer.xml");
            if (!File.Exists(file)) return new List<Footer>();

            try
            {
                using var reader = XmlReader.Create(file, SafeSettings());
                XDocument doc = XDocument.Load(reader);

                var footer = new Footer();

                // Find the <Columns> element (case-insensitive)
                var columnsNode = doc.Root?.Elements()
                    .FirstOrDefault(x => string.Equals(x.Name.LocalName, "columns", StringComparison.OrdinalIgnoreCase));

                if (columnsNode != null)
                {
                    // Iterate each <Column> and read its title attribute and child <Link> elements
                    foreach (var colEl in columnsNode.Elements().Where(x => string.Equals(x.Name.LocalName, "column", StringComparison.OrdinalIgnoreCase)))
                    {
                        var col = new Column
                        {
                            Title = (string?)colEl.Attribute("title") ?? string.Empty
                        };

                        foreach (var linkEl in colEl.Elements().Where(x => string.Equals(x.Name.LocalName, "link", StringComparison.OrdinalIgnoreCase)))
                        {
                            var link = new Link
                            {
                                Text = (string?)linkEl.Attribute("text") ?? string.Empty,
                                Url = (string?)linkEl.Attribute("url") ?? string.Empty
                            };
                            col.Links.Add(link);
                        }

                        footer.Columns.Add(col);
                    }
                }

                return new List<Footer> { footer };
            }
            catch
            {
                // Preserve previous footer cache on error
                return _footers;
            }
        }

        /// <summary>
        /// Load FAQs from faqs.xml. Each <Faq> should contain an id attribute and
        /// child <Question> and <Answer> elements. Element name comparisons are
        /// case-insensitive to support lowercase XML files.
        /// </summary>
        private List<Faq> LoadFaqs()
        {
            string file = Path.Combine(_configFolder, "faqs.xml");
            if (!File.Exists(file)) return new List<Faq>();

            try
            {
                using var reader = XmlReader.Create(file, SafeSettings());
                XDocument doc = XDocument.Load(reader);

                return doc.Root?
                    .Elements()
                    .Where(x => string.Equals(x.Name.LocalName, "faq", StringComparison.OrdinalIgnoreCase))
                    .Select(x => new Faq
                    {
                        Id = (string?)x.Attribute("id") ?? string.Empty,
                        // Find child elements <question> and <answer> in a case-insensitive way
                        Question = (string?)x.Elements().FirstOrDefault(e => string.Equals(e.Name.LocalName, "question", StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty,
                        Answer = (string?)x.Elements().FirstOrDefault(e => string.Equals(e.Name.LocalName, "answer", StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty
                    })
                    .ToList()
                    ?? new List<Faq>();
            }
            catch
            {
                // Preserve previous FAQ cache on parse error
                return _faqs;
            }
        }

        /// <summary>
        /// Return the cached list of features. Caller should treat the returned list as readonly.
        /// </summary>
        public IReadOnlyList<Feature> GetFeatures()
        {
            return _features;
        }

        /// <summary>
        /// Find a feature by id (case-insensitive). Returns null if not found.
        /// </summary>
        public Feature? GetFeature(string id)
        {
            return _features.FirstOrDefault(f => f.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Return all parsed menus.
        /// </summary>
        public IReadOnlyList<Menu> GetMenus()
        {
            return _menus;
        }

        /// <summary>
        /// Find a menu by id (case-insensitive). Returns null if not found.
        /// </summary>
        public Menu? GetMenu(string id)
        {
            return _menus.FirstOrDefault(m => m.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Return the parsed footer sections. Currently this typically contains a single Footer.
        /// </summary>
        public IReadOnlyList<Footer> GetFooters()
        {
            return _footers;
        }

        /// <summary>
        /// Return a footer by id. The current lookup attempts to match the provided id to the
        /// first column title (case-insensitive). If no match is found the first footer is returned.
        /// Adjust lookup logic if you prefer an explicit footer id attribute.
        /// </summary>
        public Footer? GetFooter(string id)
        {
            return _footers.FirstOrDefault(f => f.Columns.Any() && string.Equals(id, f.Columns.First().Title, StringComparison.OrdinalIgnoreCase)) ?? _footers.FirstOrDefault();
        }

        /// <summary>
        /// Return all parsed FAQs.
        /// </summary>
        public IReadOnlyList<Faq> GetFaqs()
        {
            return _faqs;
        }

        /// <summary>
        /// Find a faq by id (case-insensitive). Returns null if not found.
        /// </summary>
        public Faq? GetFaq(string id)
        {
            return _faqs.FirstOrDefault(f => string.Equals(f.Id, id, StringComparison.OrdinalIgnoreCase));
        }
    }
}