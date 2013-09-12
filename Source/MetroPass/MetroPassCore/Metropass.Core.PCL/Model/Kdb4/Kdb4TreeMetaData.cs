using System;
using System.Linq;
using System.Xml.Linq;


namespace Metropass.Core.PCL.Model.Kdb4
{
    public class Kdb4TreeMetaData
    {
        private readonly XElement _metaElement;
        private const string RecycledBinEnabledElementName = "RecycleBinEnabled";
        private const string RecycledBinUUIDElementName = "RecycleBinUUID";
        private const string RecycleBinChangedElementName = "RecycleBinChanged";

        public Kdb4TreeMetaData(XDocument kdbDocument)
        {
            var metaElement = kdbDocument.Descendants("Meta").FirstOrDefault();
            if (metaElement == null)
            {
                metaElement = new XElement("Meta");
                kdbDocument.Add(metaElement);
            }
            _metaElement = metaElement;
        }

        public bool RecycleBinEnabled
        {
            get
            {
                var enabledElement = _metaElement.Descendants(RecycledBinEnabledElementName).FirstOrDefault();
                if (enabledElement != null)
                {
                    return bool.Parse(enabledElement.Value);
                }
                return false;
            }
            set
            {
                var enabledElement = _metaElement.Descendants(RecycledBinEnabledElementName).FirstOrDefault();
                if (enabledElement == null)
                {
                    enabledElement = new XElement(RecycledBinEnabledElementName);
                    _metaElement.Add(enabledElement);
                }
                enabledElement.Value = value.ToString();
            }
        }

        public string RecycleBinUUID
        {
            get
            {
                var uuidElement = _metaElement.Descendants(RecycledBinUUIDElementName).FirstOrDefault();
                if (uuidElement != null)
                {
                    return uuidElement.Value;
                }
                return "";
            }
            set
            {
                var uuidElement = _metaElement.Descendants(RecycledBinUUIDElementName).FirstOrDefault();
                if (uuidElement == null)
                {
                    uuidElement = new XElement(RecycledBinEnabledElementName);
                    _metaElement.Add(uuidElement);
                }
                uuidElement.Value = value.ToString();
            }
        }

        public string RecycleBinChanged
        {
            get
            {
                var changedElement = _metaElement.Descendants(RecycleBinChangedElementName).FirstOrDefault();
                if (changedElement != null)
                {
                    return changedElement.Value;
                }
                return DateTime.Now.ToFormattedUtcTime();
            }
            set
            {
                var changedElement = _metaElement.Descendants(RecycleBinChangedElementName).FirstOrDefault();
                if (changedElement == null)
                {
                    changedElement = new XElement(RecycleBinChangedElementName);
                    _metaElement.Add(changedElement);
                }
                changedElement.Value = value;
            }
        }
    }
}