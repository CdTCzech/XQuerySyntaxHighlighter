using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace XQuerySyntaxHighlighter
{
	[Export(typeof(EditorFormatDefinition))] // export as EditorFormatDefinition otherwise the syntax coloring does not work
	[ClassificationType(ClassificationTypeNames = "xq_comment")]
	[Name("xq_comment")]  //this should be visible to the end user
	[UserVisible(true)] //set the priority to be after the default classifiers
	[Order(Before = Priority.Default)]
	internal sealed class CommentFormat : ClassificationFormatDefinition
	{
		public CommentFormat()
		{
			DisplayName = "xq_comment"; //human readable version of the name
			ForegroundColor = Color.FromRgb(87, 166, 74);
		}
	}

	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "xq_default_function")]
	[Name("xq_default_function")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class DefaultFunctionFormat : ClassificationFormatDefinition
	{
		public DefaultFunctionFormat()
		{
			DisplayName = "xq_default_function";
			ForegroundColor = Color.FromRgb(191, 210, 249);
		}
	}

	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "xq_keyword")]
	[Name("xq_keyword")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class KeywordFormat : ClassificationFormatDefinition
	{
		public KeywordFormat()
		{
			DisplayName = "xq_keyword";
			ForegroundColor = Color.FromRgb(86, 156, 214);
		}
	}

	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "xq_namespace")]
	[Name("xq_namespace")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class NamespaceFormat : ClassificationFormatDefinition
	{
		public NamespaceFormat()
		{
			DisplayName = "xq_namespace";
			ForegroundColor = Color.FromRgb(78, 201, 176);
		}
	}

	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "xq_number")]
	[Name("xq_number")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class NumberFormat : ClassificationFormatDefinition
	{
		public NumberFormat()
		{
			DisplayName = "xq_number";
			ForegroundColor = Color.FromRgb(184, 215, 163);
		}
	}

	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "xq_string")]
	[Name("xq_string")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class StringFormat : ClassificationFormatDefinition
	{
		public StringFormat()
		{
			DisplayName = "xq_string";
			ForegroundColor = Color.FromRgb(214, 157, 133);
		}
	}

	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "xq_variable")]
	[Name("xq_variable")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class VariableFormat : ClassificationFormatDefinition
	{
		public VariableFormat()
		{
			DisplayName = "xq_variable";
			ForegroundColor = Color.FromRgb(255, 219, 163);
		}
	}
}
