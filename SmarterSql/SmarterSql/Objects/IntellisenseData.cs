// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics;
using GlacialComponents.Controls;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Objects {
	public abstract class IntellisenseData : IComparable<IntellisenseData> {
		#region Member variables

		private GLItem glItem;
		protected string strSubItem = string.Empty;
		protected string strTypePrefix = string.Empty;
		protected string strUpperCaseLetters = string.Empty;

		#region Enums

		#region enSortOrder enum

		public enum enSortOrder {
			None = 0,
			SysObject,
			SysObjectColumn,
			SysObjectParameter,
			Database,
			DataType,
			Permission,
			SqlCommand,
			Table,
			User,
			Variable,
			MethodParameter,
			LiveTemplate,
			Other,
			SysObjectSchema,
			SysServer
		}

		#endregion

		#region ImageKeys enum

		public enum ImageKeys {
			None = 0,
			Table,
			TableColumn,
			Sproc,
			LocalVariable,
			GlobalVariable,
			LiveTemplate,
			View,
			TableValuedFunction,
			ScalarValuedFunction,
			SqlCommand,
			Permission,
			User,
			DataType,
			DataBase,
		}

		#endregion

		#endregion

		#endregion

		/// <summary>
		/// Public constructor
		/// </summary>
		/// <param name="strVariableName"></param>
		protected IntellisenseData(string strVariableName) {
			strUpperCaseLetters = Common.SplitCamelCasing(strVariableName);
		}

		#region Public properties

		protected abstract enSortOrder SortLevel { get; }

		/// <summary>
		/// Returns the text shown in the main column
		/// </summary>
		public abstract string MainText { get; }

		/// <summary>
		/// Returns the image key
		/// </summary>
		public abstract int ImageKey { get; }

		/// <summary>
		/// Returns the tooltip
		/// </summary>
		public abstract string GetToolTip { get; }

		/// <summary>
		/// Returns the data which is returned to the user after he makes a selection
		/// </summary>
		public abstract string GetSelectedData { get; }

		/// <summary>
		/// Returns the type prefix, for example "@@" for global variables
		/// </summary>
		public string TypePrefix {
			[DebuggerStepThrough]
			get { return strTypePrefix; }
		}

		/// <summary>
		/// Returns the upper case letters in the main text column
		/// </summary>
		public string UpperCaseLetters {
			[DebuggerStepThrough]
			get { return strUpperCaseLetters; }
		}

		/// <summary>
		/// Returns a list of subitems
		/// </summary>
		public string SubItem {
			[DebuggerStepThrough]
			get { return strSubItem; }
		}

		public bool StartsWithAttAtt {
			[DebuggerStepThrough]
			get { return false; }
		}

		///<summary>
		///Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</returns>
		///<filterpriority>2</filterpriority>
		[DebuggerStepThrough]
		public override string ToString() {
			return MainText + ", " + SubItem;
		}

		#endregion

		#region IComparable<IntellisenseData> Members

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.Zero This object is equal to other. Greater than zero This object is greater than other.</returns>
		public int CompareTo(IntellisenseData other) {
			if (StartsWithAttAtt && other.StartsWithAttAtt) {
				return MainText.CompareTo(other.MainText);
			}
			if (StartsWithAttAtt) {
				return 1;
			}
			if (other.StartsWithAttAtt) {
				return -1;
			}

			int compareTo = MainText.CompareTo(other.MainText);
			if (0 != compareTo) {
				return compareTo;
			}
			return SortLevel - other.SortLevel;
		}

		#endregion

		/// <summary>
		/// Returns a ListViewItem containing the data for this IntellisenseData object
		/// </summary>
		/// <returns></returns>
		public GLItem GetItem() {
			if (null == glItem) {
				glItem = new GLItem {
					Tag = this
				};

				// Main item
				GLSubItem subItem1 = new GLSubItem {
					ImageIndex = ImageKey,
					Text = MainText
				};

				// Sub item
				GLSubItem subItem2 = new GLSubItem {
					Text = strSubItem
				};

				// Add items
				glItem.SubItems.AddRange(new[] {
					subItem1, subItem2
				});
			}
			return glItem;
		}
	}
}