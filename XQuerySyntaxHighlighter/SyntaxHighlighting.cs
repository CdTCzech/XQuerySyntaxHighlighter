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

		private static readonly HashSet<string> DefinedKeywords = new HashSet<string>()
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

		// TODO: upozorneni na vic jak 16380 znaku, folding
		public IEnumerable<ITagSpan<XQueryTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			SnapshotSpan curSpan = spans[0];
			string text = curSpan.Snapshot.TextBuffer.CurrentSnapshot.GetText();
			int startPos = 0;
			bool inString = false;

			int i = 0;
			int end = text.Length;
			if (text.Length >= 3 && text.Substring(0, 3) == "R\"(") i = 3;
			if (text.Length >= 2 && text.Substring(text.Length - 2, 2) == ")\"") end = text.Length - 2;

			while (i < end)
			{
				TokenType tokenType = TokenType.xq_unknown;
				startPos = i;

				if (char.IsLetter(text[i]) || text[i] == '_') // keyword || namespace
				{
					while (++i < end && (char.IsLetterOrDigit(text[i]) || text[i] == '-')) { }

					string temp = text.Substring(startPos, i - startPos);
					if (i + 1 < end && text[i] == ':' && (char.IsLetter(text[i + 1]) || text[i + 1] == '_')) // namespace
					{
						var tokenSpanNamespace = new SnapshotSpan(curSpan.Snapshot, new Span(startPos, i - startPos));
						yield return new TagSpan<XQueryTokenTag>(tokenSpanNamespace, new XQueryTokenTag(TokenType.xq_namespace));
						startPos = ++i;
						while (++i < end && (char.IsLetterOrDigit(text[i]) || text[i] == '-')) { }

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
					else if (i + 1 < end && text[i] == '(')
					{
						tokenType = TokenType.xq_default_function;
					}
				}
				else if (char.IsDigit(text[i])) // number
				{
					while (++i < end && char.IsDigit(text[i])) { } // integer
					if (i < end && text[i] == '.') // floating
					{
						while (++i < end && char.IsDigit(text[i])) { }
					}
					tokenType = TokenType.xq_number;
				}
				else if (text[i] == '(' && i + 1 < end && text[i + 1] == ':') // comment
				{
					while (true)
					{
						while (++i < end && text[i] != ')') { }
						if (i == end || text[i - 1] == ':') break;
					}
					if (i + 1 <= end) ++i;
					tokenType = TokenType.xq_comment;
				}
				else if (text[i] == '$') // variable
				{
					while (++i < end && (char.IsLetterOrDigit(text[i]) || text[i] == '-')) { }
					tokenType = TokenType.xq_variable;
				}
				else if (text[i] == '\"') // string
				{
					while (++i < end && char.IsWhiteSpace(text[i])) { }
					if (i < end && text[i] == '{')
					{
						inString = true;
					}
					else if (i < end && text[i] != '\"')
					{
						while (++i < end && text[i] != '\"') { }
					}
					if (i + 1 <= end) ++i;
					tokenType = TokenType.xq_string;
				}
				else if (inString && text[i] == '}')
				{
					while (++i < end && char.IsWhiteSpace(text[i])) { }
					if (i < end && text[i] == '\"')
					{
						inString = false;
						if (i + 1 <= end) ++i;
						tokenType = TokenType.xq_string;
					}
				}
				else if (text[i] == '\'') // char
				{
					while (++i < end && text[i] != '\'') { }
					if (i + 1 <= end) ++i;
					tokenType = TokenType.xq_string;
				}
				else
				{
					++i;
				}

				/*
				using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\hrncir\Documents\Visual Studio 2015\Projects\Out.txt", true))
				{
					file.WriteLine("Start: " + startPos + "End: " + Math.Max(1, i - startPos) + "Type: " + tokenType.ToString());
				}
				*/

				var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(startPos, i - startPos));
				yield return new TagSpan<XQueryTokenTag>(tokenSpan, new XQueryTokenTag(tokenType));
			}
		}
	}
}
