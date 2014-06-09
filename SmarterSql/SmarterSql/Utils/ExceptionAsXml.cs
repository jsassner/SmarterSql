// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using System.Collections;
using System.Linq;
using System.Xml.Linq;

namespace Sassner.SmarterSql.Utils {
	public class ExceptionAsXml {
		#region Member variables

		/// <summary>The root node element of the serialized exception.</summary>
		public readonly XElement Exception;

		#endregion

		/// <summary>Represent an Exception as XML data.</summary>
		/// <param name="exception">The Exception to serialize.</param>
		/// Whether or not to serialize the Exception.StackTrace member if it's not null.
		/// </param>
		public ExceptionAsXml(Exception exception) : this(exception, false) {
		}

		/// <summary>Represent an Exception as XML data.</summary>
		/// <param name="exception">The Exception to serialize.</param>
		/// <param name="omitStackTrace">
		/// Whether or not to serialize the Exception.StackTrace member if it's not null.
		/// </param>
		public ExceptionAsXml(Exception exception, bool omitStackTrace) {
			// Allow null to allow new ExceptionAsXml(exception.InnerException)
			// where InnerException may be null
			if (null == exception) {
				return;
			}

			// The root element is the Exception's type
			Exception = new XElement(exception.GetType().ToString());
			if (null != exception.Message) {
				Exception.Add(new XElement("Message", exception.Message));
			}

			// StackTrace can be null, e.g.: new ExceptionAsXml(new Exception())
			if (!omitStackTrace && null != exception.StackTrace) {
				Exception.Add(new XElement("StackTrace",
				                           from frame in exception.StackTrace.Split('\n')
				                           select new XElement("Frame", frame.Substring(6).Trim()))
					);
			}

			// Data is never null; it's empty if there is no data
			if (exception.Data.Count > 0) {
				Exception.Add(new XElement("Data",
				                           from entry in exception.Data.Cast<DictionaryEntry>()
				                           select new XElement((string)entry.Key, entry.Value))
					);
			}

			// Add the InnerException if it exists
			if (exception.InnerException != null) {
				Exception.Add(new ExceptionAsXml(exception.InnerException).Exception);
			}
		}

		public override int GetHashCode() {
			return (Exception == null ? 0 : Exception.ToString().GetHashCode());
		}

		/// <summary>
		/// Get the XML markup for this Exception with indenting and new lines.
		/// </summary>
		public override string ToString() {
			return (Exception == null ? null : Exception.ToString());
		}

		/// <summary>
		/// Allow: string s = new ExceptionAsXml(exception);
		/// </summary>
		public static implicit operator string(ExceptionAsXml exception) {
			return (exception == null ? null : exception.ToString());
		}
	}
}
