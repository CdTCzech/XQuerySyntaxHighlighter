using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace XQuerySyntaxHighlighter
{
	[Export(typeof(ITaggerProvider))]
	[ContentType(XQSH.ContentType)]
	[TagType(typeof(XQueryTokenTag))]
	internal sealed class SyntaxHighlightingProvider : ITaggerProvider
	{
		public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
		{
			return new XQueryTokenTagger(buffer) as ITagger<T>;
		}
	}
}
