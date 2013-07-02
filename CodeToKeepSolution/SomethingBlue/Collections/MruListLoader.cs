using System.Collections.Generic;
using SomethingBlue.Extensions;

namespace SomethingBlue.Collections
{
    public class DocumentType
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class DocumentTypeEqualityComparer : IEqualityComparer<DocumentType>
    {
        public bool Equals(DocumentType dt1, DocumentType dt2)
        {
            return (dt1.Id == dt2.Id);
        }

        public int GetHashCode(DocumentType dt)
        {
            return dt.GetHashCode();
        }
    }

    public static class MruListLoader
    {
        public static void Save(ObservableMruList<DocumentType> mruList)
        {
            if (mruList.Count == 0) return;

            //Settings.Default.MRUList = mruList.ToXml();
            //Settings.Default.Save();
        }

        public static ObservableMruList<DocumentType> Load()
        {
            var mruList = new ObservableMruList<DocumentType>(9, new DocumentTypeEqualityComparer());
            string xml = string.Empty;
            //xml = Settings.Default.MRUList;
            if (string.IsNullOrWhiteSpace(xml) == false)
            {
                var listFromXml = xml.FromXml<List<DocumentType>>();
                mruList.AddRange(listFromXml);
            }
            return mruList;
        }
    }
}