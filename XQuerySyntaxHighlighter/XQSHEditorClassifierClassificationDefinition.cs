//------------------------------------------------------------------------------
// <copyright file="XQSHEditorClassifierClassificationDefinition.cs" company="Hewlett-Packard Company">
//     Copyright (c) Hewlett-Packard Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace XQuerySyntaxHighlighter
{
	/// <summary>
	/// Classification type definition export for XQSHEditorClassifier
	/// </summary>
	internal static class XQSHEditorClassifierClassificationDefinition
	{
		// This disables "The field is never used" compiler's warning. Justification: the field is used by MEF.
#pragma warning disable 169

		/// <summary>
		/// Defines the "XQSHEditorClassifier" classification type.
		/// </summary>
		[Export(typeof(ClassificationTypeDefinition))]
		[Name("XQSHEditorClassifier")]
		private static ClassificationTypeDefinition typeDefinition;

#pragma warning restore 169
	}
}
