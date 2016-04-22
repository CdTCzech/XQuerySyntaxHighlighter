using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace XQuerySyntaxHighlighter
{
	internal static class XQueryClassificationDefinition
	{
		[Export(typeof(ClassificationTypeDefinition))]
		[Name("xq_comment")]
		internal static ClassificationTypeDefinition xq_comment = null;

		[Export(typeof(ClassificationTypeDefinition))]
		[Name("xq_default_function")]
		internal static ClassificationTypeDefinition xq_default_function = null;

		[Export(typeof(ClassificationTypeDefinition))]
		[Name("xq_keyword")]
		internal static ClassificationTypeDefinition xq_keyword = null;

		[Export(typeof(ClassificationTypeDefinition))]
		[Name("xq_namespace")]
		internal static ClassificationTypeDefinition xq_namespace = null;

		[Export(typeof(ClassificationTypeDefinition))]
		[Name("xq_number")]
		internal static ClassificationTypeDefinition xq_number = null;

		[Export(typeof(ClassificationTypeDefinition))]
		[Name("xq_string")]
		internal static ClassificationTypeDefinition xq_string = null;

		[Export(typeof(ClassificationTypeDefinition))]
		[Name("xq_variable")]
		internal static ClassificationTypeDefinition xq_variable = null;
	}

	[Export(typeof(ITaggerProvider))]
	[ContentType(XQSH.ContentType)]
	[TagType(typeof(ClassificationTag))]
	internal sealed class XQueryClassifierProvider : ITaggerProvider
	{
		[Export]
		[Name("xquery")]
		[DisplayName("XQuery")]
		[BaseDefinition("code")]
		internal static ContentTypeDefinition XQueryContentType = null;

		[Export]
		[FileExtension(".xq")]
		[ContentType(XQSH.ContentType)]
		internal static FileExtensionToContentTypeDefinition AsmFileType = null;

		[Import]
		internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

		[Import]
		internal IBufferTagAggregatorFactoryService AggregatorFactory = null;

		public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
		{
			ITagAggregator<XQueryTokenTag> asmTagAggregator = AggregatorFactory.CreateTagAggregator<XQueryTokenTag>(buffer);
			return new XQueryClassifier(buffer, asmTagAggregator, ClassificationTypeRegistry) as ITagger<T>;
		}
	}


	class XQueryClassifier : ITagger<ClassificationTag>
	{
		ITextBuffer Buffer;
		ITagAggregator<XQueryTokenTag> Aggregator;
		IDictionary<TokenType, IClassificationType> XQueryTypes;

		internal XQueryClassifier(ITextBuffer buffer,
							   ITagAggregator<XQueryTokenTag> asmTagAggregator,
							   IClassificationTypeRegistryService typeService)
		{
			Buffer = buffer;
			Aggregator = asmTagAggregator;
			XQueryTypes = new Dictionary<TokenType, IClassificationType>();
			XQueryTypes[TokenType.xq_comment] = typeService.GetClassificationType("xq_comment");
			XQueryTypes[TokenType.xq_default_function] = typeService.GetClassificationType("xq_default_function");
			XQueryTypes[TokenType.xq_keyword] = typeService.GetClassificationType("xq_keyword");
			XQueryTypes[TokenType.xq_namespace] = typeService.GetClassificationType("xq_namespace");
			XQueryTypes[TokenType.xq_number] = typeService.GetClassificationType("xq_number");
			XQueryTypes[TokenType.xq_string] = typeService.GetClassificationType("xq_string");
			XQueryTypes[TokenType.xq_variable] = typeService.GetClassificationType("xq_variable");
		}

		public event EventHandler<SnapshotSpanEventArgs> TagsChanged
		{
			add { }
			remove { }
		}

		public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			foreach (var tagSpan in Aggregator.GetTags(spans))
			{
				NormalizedSnapshotSpanCollection tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
				IClassificationType xqueryType = XQueryTypes[tagSpan.Tag.type];
				if (xqueryType != null)
				{
					yield return new TagSpan<ClassificationTag>(tagSpans[0], new ClassificationTag(xqueryType));
				}
			}
		}
	}
}
