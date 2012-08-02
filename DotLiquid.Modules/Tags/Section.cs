using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotLiquid.Exceptions;
using DotLiquid.Util;

namespace DotLiquid.Modules.Tags
{
    public class SectionToken
    {
        public string Name { get; set; }
        public List<object> NodeList { get; set; }
    }

    public class Section : Block
    {
        private string _name; // token from script
        private Condition _cond;
        private static readonly string ExpressionsAndOperators = string.Format(R.Q(@"(?:\b(?:\s?and\s?|\s?or\s?)\b|(?:\s*(?!\b(?:\s?and\s?|\s?or\s?)\b)(?:{0}|\S+)\s*)+)"), Liquid.QuotedFragment);
        private static readonly Regex Syntax = R.B(R.Q(@"({0})(?:\s+({1}))?"), Liquid.QuotedFragment, ExpressionsAndOperators);
        private static readonly Regex ExpressionSyntax = R.B(R.Q(@"({0})\s*([=!<>a-z_]+)?\s*({0})?"), Liquid.QuotedFragment);

        public static Condition GetCondition(string conStr)
        {
            // syntax needs to be copnfirmed at this stage

            List<string> expressions = R.Scan(conStr, ExpressionsAndOperators);
            expressions.Reverse();
            string syntax = expressions.Shift();
            if (string.IsNullOrEmpty(syntax))
                throw new SyntaxException("Condition syntax in Section tag is incorrect.");
            Match syntaxMatch = ExpressionSyntax.Match(syntax);
            if (!syntaxMatch.Success)
                throw new SyntaxException("Condition syntax in Section tag is incorrect.");

            Condition condition = new Condition(syntaxMatch.Groups[1].Value,
                syntaxMatch.Groups[2].Value, syntaxMatch.Groups[3].Value);

            while (expressions.Any())
            {
                string @operator = expressions.Shift().Trim();

                Match expressionMatch = ExpressionSyntax.Match(expressions.Shift());
                if (!expressionMatch.Success)
                    throw new SyntaxException("Condition syntax in Section tag is incorrect.");

                Condition newCondition = new Condition(expressionMatch.Groups[1].Value,
                    expressionMatch.Groups[2].Value, expressionMatch.Groups[3].Value);
                switch (@operator)
                {
                    case "and":
                        newCondition.And(condition);
                        break;
                    case "or":
                        newCondition.Or(condition);
                        break;
                }
                condition = newCondition;
            }
            return condition;
        }

        public string SectionName { get { return _name; } }
        public Condition Condition { get { return _cond; } }
        
        private readonly Dictionary<string, SectionToken> _tokens = new Dictionary<string, SectionToken>();

        public Dictionary<string, SectionToken> Tokens { get { return _tokens; } }

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            Match syntaxMatch = Syntax.Match(markup);

            if (syntaxMatch.Success)
            {
                _name = syntaxMatch.Groups[1].Value; // remember the name

                if (syntaxMatch.Groups[2].Success)
                {
                    _cond = GetCondition(syntaxMatch.Groups[2].Value);
                }
            }
            else
            {
                throw new SyntaxException("Incorrect syntax for the Section tag");
            }

            PushToken("body", null);

            base.Initialize(tagName, markup, tokens);
        }

        public override void UnknownTag(string tag, string markup, List<string> tokens)
        {
            PushToken(tag, markup);
        }

        public override void Render(Context context, TextWriter result)
        {
            // NodeLists from SectionTokens are attached and rendered in WriteSection tags
        }

        public void PushToken(string name, string markup)
        {
            // todo: add condition into tokens, so we can switch only tokens, but not entire sections

            SectionToken tok;
            if (_tokens.ContainsKey(name)) // if one section occurs multiple times, we just concatenate node lists
            {
                tok = _tokens[name];
            } 
            else
            {
                tok = new SectionToken
                {
                    Name = name,
                    NodeList = new List<object>()
                };
                _tokens[name] = tok;
            }

            NodeList = tok.NodeList;
        }
    }
}
