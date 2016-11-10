using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

namespace ConstructorIO
{
    using System.Data;
    using HashArgs = Dictionary<string, object>;

    public class Util
    {
        /// <summary>
        /// Serializes url params in a rudimentary way, and you must write other helper methods to serialize other things.
        /// </summary>
        /// <param name="paramDict"> params HashMap of the parameters to encode.</param>
        /// <returns> The encoded parameters, as a String.</returns>
        public static string SerializeParams(IDictionary<string, object> paramDict)
        {
            var list = new List<string>();
            foreach (var item in paramDict)
            {
                list.Add(System.Uri.EscapeDataString(item.Key) + "=" + System.Uri.EscapeDataString((string)item.Value));
            }
            return string.Join("&", list);
        }

        /// <summary>
        /// Merge two Hash tables (Dictionary)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="into"></param>
        public static void Merge(HashArgs from, HashArgs into)
        {
            foreach(var kvp in from)
                into[kvp.Key] = kvp.Value;
        }

        /// <summary>
        /// Load a CSV file, read all entries and return ListItem objects
        /// </summary>
        /// <param name="filename">CSV file</param>
        /// <returns>Enumarator of ListItem objects</returns>
        static public IEnumerable<ListItem> LoadCSV(string filename)
        {
            char valuedelimiter = ',';
            char keywordDelimiter = ';';
            string[] lineParts;
            List<ListItem> listItems = new List<ListItem>();
            List<string> headers = new List<string>() ;
            int lineCounter = 0;

            try
            {
                using (var textReader = new System.IO.StreamReader(filename))
                {
                    while (!textReader.EndOfStream)
                    {
                        lineCounter++;
                        lineParts = textReader.ReadLine().Split(valuedelimiter);
                        
                        // The first line should be a header, read that to know the index of the columns
                        if (lineCounter==1)
                        {
                            for (int h = 0; h < lineParts.Length; h++)
                                headers.Add(lineParts[h]);
                            continue;
                        }

                        // Do some validation on this line
                        if (lineParts.Length != headers.Count())
                            throw new FormatException("Number of values doesn't match number of headers.");

                        ListItem newListItem = new ListItem();
                        // Get the all the values, whatever they are
                        for (int i=0;i<lineParts.Length;i++)
                        {
                            // Keywords need special handling 
                            if (headers[i]=="keywords")
                            {
                                string[] keywords = lineParts[i].Split(keywordDelimiter);
                                foreach (string keyword in keywords)
                                    newListItem.AddKeyword(keyword);
                            }
                            else if (isString(lineParts[i]))
                                newListItem[headers[i]] = lineParts[i];
                            else
                                newListItem[headers[i]] = int.Parse(lineParts[i]);

                        }

                        listItems.Add(newListItem);
                    }
                    return listItems;
                }
            }
            catch (FormatException ex)
            {
                throw new FormatException("Error parsing CSV file " + filename + " at line " + lineCounter, ex);
            }
        }

        /// <summary>
        /// Check if a value is string or not
        /// </summary>
        /// <param name="value">Value to check</param>
        static bool isString(string value)
        {
            return value.StartsWith("\"") && value.EndsWith("\"");
        }
    }

    /// <summary>
    ///  StringValueAttribute
    /// </summary>

    public class StringValueAttribute : Attribute
    {
        private string m_sValue;

        public StringValueAttribute(string value)
        {
            m_sValue = value;
        }

        public string Value
        {
            get { return m_sValue; }
        }
    }

    

    /// <summary>
    /// StringEnum
    /// </summary>
    public class StringEnum
    {
        private static Hashtable m_hsStringValues = new Hashtable();

        public static string GetStringValue(Enum value)
        {
            string output = null;

            Type type = value.GetType();

            if (m_hsStringValues.ContainsKey(value))
                output = (m_hsStringValues[value] as StringValueAttribute).Value;
            else
            {
                FieldInfo fi = type.GetField(value.ToString());
                StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
                if (attrs.Length > 0)
                {
                    m_hsStringValues.Add(value, attrs[0]);
                    output = attrs[0].Value;
                }
            }

            return output;
        }
    }
}