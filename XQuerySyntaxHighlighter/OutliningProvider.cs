﻿using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;

namespace XQuerySyntaxHighlighter
{
	[Export(typeof(ITaggerProvider))]
	[ContentType(XQSH.ContentType)]
	[TagType(typeof(IOutliningRegionTag))]
	internal sealed class OutliningTaggerProvider : ITaggerProvider
	{
		public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
		{
			//create a single tagger for each buffer.
			Func<ITagger<T>> sc = delegate () { return new OutliningTagger(buffer) as ITagger<T>; };
			return buffer.Properties.GetOrCreateSingletonProperty<ITagger<T>>(sc);
		}
	}
}
