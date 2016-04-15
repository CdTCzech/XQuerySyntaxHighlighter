//------------------------------------------------------------------------------
// <copyright file="XQSHEditorClassifierFormat.cs" company="Hewlett-Packard Company">
//     Copyright (c) Hewlett-Packard Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace XQuerySyntaxHighlighter
{
	/// <summary>
	/// Defines an editor format for the XQSHEditorClassifier type that has a purple background
	/// and is underlined.
	/// </summary>
	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "XQSHEditorClassifier")]
	[Name("XQSHEditorClassifier")]
	[UserVisible(true)] // This should be visible to the end user
	[Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
	internal sealed class XQSHEditorClassifierFormat : ClassificationFormatDefinition
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XQSHEditorClassifierFormat"/> class.
		/// </summary>
		public XQSHEditorClassifierFormat()
		{
			this.DisplayName = "XQSHEditorClassifier"; // Human readable version of the name
			this.BackgroundColor = Colors.BlueViolet;
			this.TextDecorations = System.Windows.TextDecorations.Underline;
		}
	}
}
