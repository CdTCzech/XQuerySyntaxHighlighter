using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace XQuerySyntaxHighlighter
{
	public class XQueryTokenTag : ITag
	{
		public TokenType type { get; private set; }

		public XQueryTokenTag(TokenType type)
		{
			this.type = type;
		}
	}

	internal sealed class XQueryTokenTagger : ITagger<XQueryTokenTag>
	{
		private ITextBuffer Buffer;
		private HashSet<string> DefinedKeywords;
		static bool inComment = false;

		internal XQueryTokenTagger(ITextBuffer buffer)
		{
			Buffer = buffer;
			DefinedKeywords = new HashSet<string>();
			DefinedKeywords.Add("declare");
			DefinedKeywords.Add("else");
			DefinedKeywords.Add("for");
			DefinedKeywords.Add("function");
			DefinedKeywords.Add("if");
			DefinedKeywords.Add("then");
		}

		public event EventHandler<SnapshotSpanEventArgs> TagsChanged
		{
			add { }
			remove { }
		}

		public IEnumerable<ITagSpan<XQueryTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			foreach (SnapshotSpan curSpan in spans)
			{
				ITextSnapshotLine containingLine = curSpan.Start.GetContainingLine();
				string line = containingLine.GetText();
				int offset = containingLine.Start.Position;
				int startPos = 0;

				for (int i = 0; i < line.Length; ++i)
				{
					TokenType tokenType = TokenType.xq_unknown;

					if (char.IsLetter(line[i])) // keyword || namespace
					{
						startPos = i;
						while (++i < line.Length && char.IsLetterOrDigit(line[i])) { }

						string temp = line.Substring(startPos, i - startPos);
						if (i + 1 < line.Length && line[i] == ':' && (char.IsLetter(line[i + 1]) || line[i + 1] == '_')) // namespace
						{
							tokenType = TokenType.xq_namespace;
						}
						else if (DefinedKeywords.Contains(temp)) // keyword
						{
							tokenType = TokenType.xq_keyword;
						}
					}
					else if (char.IsDigit(line[i])) // number
					{
						startPos = i;
						while (++i < line.Length && char.IsDigit(line[i])) { } // integer
						if (i < line.Length && line[i] == '.') // floating
						{
							while (++i < line.Length && char.IsDigit(line[i])) { }
						}
						tokenType = TokenType.xq_number;
					}
					else if (line[i] == '(') // comment
					{
						if (i + 1 < line.Length && line[i + 1] == ':')
						{
							startPos = i++;

							while (true)
							{
								while (++i < line.Length && line[i] != ')') { }

								if ((i >= 2 && line[i - 2] == ':') || i == line.Length)
								{
									tokenType = TokenType.xq_comment;
									break;
								}
							}
						}
					}

					if (tokenType != TokenType.xq_unknown)
					{
						var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(startPos + offset, i - startPos));
						yield return new TagSpan<XQueryTokenTag>(tokenSpan, new XQueryTokenTag(tokenType));
					}
				}
			}
		}
	}
}
