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

		private HashSet<string> DefinedKeywords = new HashSet<string>()
		{
			"and", "as", "ascending", "at", "attribute",
			"base-uri", "boundary-space", "by",
			"case", "cast", "castable", "collation", "construction", "copy-namespaces",
			"declare", "default", "descending", "div",
			"element", "else", "empty", "encoding", "eq", "every", "except", "external",
			"for", "function",
			"ge", "greatest", "gt",
			"idiv", "if", "import", "in", "inherit", "instance", "intersect", "is",
			"lax", "le", "least", "let", "lt",
			"mod", "module", "namespace",
			"ne", "no-inherit", "no-preserve",
			"of", "option", "or", "order", "ordered", "ordering",
			"preserve",
			"return",
			"satisfies", "schema", "some", "stable", "strict", "strip",
			"then", "to", "treat", "typeswitch",
			"union", "unordered",
			"validate", "variable", "version",
			"where",
			"xquery"
		};

		internal XQueryTokenTagger(ITextBuffer buffer)
		{
			Buffer = buffer;
		}

		public event EventHandler<SnapshotSpanEventArgs> TagsChanged
		{
			add { }
			remove { }
		}

		// TODO: multiline comments/strings, XQuery in string, regiony, upozorneni na vic jak 16380 znaku, folding
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

					if (char.IsLetter(line[i]) || line[i] == '_') // keyword || namespace
					{
						startPos = i;
						while (++i < line.Length && (char.IsLetterOrDigit(line[i]) || line[i] == '-')) { }

						string temp = line.Substring(startPos, i - startPos);
						if (i + 1 < line.Length && line[i] == ':' && (char.IsLetter(line[i + 1]) || line[i + 1] == '_')) // namespace
						{
							var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(startPos + offset, i - startPos));
							yield return new TagSpan<XQueryTokenTag>(tokenSpan, new XQueryTokenTag(TokenType.xq_namespace));
							startPos = ++i;
							while (++i < line.Length && (char.IsLetterOrDigit(line[i]) || line[i] == '-')) { }

							if (temp == "xs" || temp == "fn")
							{
								tokenType = TokenType.xq_default_function;
								// tokenType = (temp == "xs") ? TokenType.xq_keyword : TokenType.xq_default_function;
							}
							else
							{
								tokenType = TokenType.xq_unknown;
							}
						}
						else if (DefinedKeywords.Contains(temp.ToLower())) // keyword
						{
							tokenType = TokenType.xq_keyword;
						}
						else if (i + 1 < line.Length && line[i] == '(')
						{
							tokenType = TokenType.xq_default_function;
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
							startPos = i;
							ITextSnapshot textSnapshot = curSpan.Snapshot.TextBuffer.CurrentSnapshot;
							while (true)
							{
								while (++i < textSnapshot.Length && textSnapshot[i] != ')') { }
								if (textSnapshot[i - 1] == ':') break;
							}
							tokenType = TokenType.xq_comment;
						}
					}
					else if (line[i] == '$') // variable
					{
						startPos = i;
						while (++i < line.Length && (char.IsLetterOrDigit(line[i]) || line[i] == '-')) { }
						tokenType = TokenType.xq_variable;
					}
					else if (line[i] == '\"' || line[i] == '\'') // text
					{
						startPos = i;
						char tempChar = line[i];
						while (++i < line.Length && line[i] != tempChar) { }
						++i;
						tokenType = TokenType.xq_string;
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
