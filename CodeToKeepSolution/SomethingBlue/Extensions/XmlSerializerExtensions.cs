using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SomethingBlue.Extensions
{
    /// <summary>
    ///     Extensions for supporting xml serialization by <see cref="XmlSerializer" />
    /// </summary>
    public static class XmlSerializerExtensions
    {
        private static readonly Dictionary<RuntimeTypeHandle, XmlSerializer> MsSerializers = new Dictionary<RuntimeTypeHandle, XmlSerializer>();

        #region Public methods

        /// <summary>
        ///     Serialize object to xml string by <see cref="XmlSerializer" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToXml<T>(this T value) where T : new()
        {
            XmlSerializer serializer = GetValue(typeof(T));
            using (var stream = new MemoryStream())
            {
                using (var writer = new XmlTextWriter(stream, new UTF8Encoding()))
                {
                    serializer.Serialize(writer, value);
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
        }

        /// <summary>
        ///     Serialize object to stream by <see cref="XmlSerializer" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="stream"></param>
        public static void ToXml<T>(this T value, Stream stream) where T : new()
        {
            XmlSerializer serializer = GetValue(typeof(T));
            serializer.Serialize(stream, value);
        }

        /// <summary>
        ///     Deserialize object from string
        /// </summary>
        /// <typeparam name="T">Type of deserialized object</typeparam>
        /// <param name="srcString">Xml source</param>
        /// <returns></returns>
        public static T FromXml<T>(this string srcString) where T : new()
        {
            XmlSerializer serializer = GetValue(typeof(T));
            using (var stringReader = new StringReader(srcString))
            using (XmlReader reader = new XmlTextReader(stringReader))
                return (T)serializer.Deserialize(reader);
        }

        /// <summary>
        ///     Deserialize object from stream
        /// </summary>
        /// <typeparam name="T">Type of deserialized object</typeparam>
        /// <param name="source">Xml source</param>
        /// <returns></returns>
        public static T FromXml<T>(this Stream source) where T : new()
        {
            XmlSerializer serializer = GetValue(typeof(T));
            return (T)serializer.Deserialize(source);
        }

        #endregion

        #region Private methods

        private static XmlSerializer GetValue(Type type)
        {
            XmlSerializer serializer;
            if (!MsSerializers.TryGetValue(type.TypeHandle, out serializer))
            {
                lock (MsSerializers)
                {
                    if (!MsSerializers.TryGetValue(type.TypeHandle, out serializer))
                    {
                        serializer = new XmlSerializer(type);
                        MsSerializers.Add(type.TypeHandle, serializer);
                    }
                }
            }
            return serializer;
        }

        #endregion
    }
}