using System;
using System.Collections.Generic;

namespace SimpleLineParser.Parser
{
    using Data;
    public static class Scheme
    {

        private static readonly int Buffer = 8192;

        private enum ParserState : byte
        {
            Prefix = 0,
            Name = 1,
            InBrackets = 2,
            NextLine = 3
        }



        public static List<SetParse> Parse(string raw)
        {
            return ParseSpan(raw.AsSpan());
        }


        private static int GetOptionFromChar(char c)
        {
            switch (c)
            {
                case '!': return 1;
                case '@': return 2;
                case '*': return 3;
                case '+': return 4;
                case '#': return 5;
                default: return 0;
            }
        }

        private static void SetValueParse(ref ValueParse currentValueParse,ref SegmentParse segment,bool addToLeft ,string prefix,string name, string value, int option)
        {

            currentValueParse.Prefix = prefix;
            currentValueParse.Name = name;
            currentValueParse.Value = value;
            currentValueParse.AddOptions(option);

            if (addToLeft)
            {
                segment.AddLeft(currentValueParse);
            }
            else
            {
                segment.AddRight(currentValueParse);
            }

            currentValueParse = new ValueParse(string.Empty, string.Empty, string.Empty, 0);
        }

        private static List<SetParse> ParseSpan(ReadOnlySpan<char> raw)
        {
            if (raw.IsEmpty)
            {
                return new List<SetParse>();
            }

            int start = 0;
            List<SetParse> results = new List<SetParse>();
            ValueParse currentValueParse = new ValueParse(string.Empty, string.Empty, string.Empty, 0);
            SetParse currentSet = new SetParse();
            SegmentParse currentSegment = new SegmentParse();

            Span<char> currentPrefix = stackalloc char[Buffer];
            Span<char> currentName = stackalloc char[Buffer];
            Span<char> currentValue = stackalloc char[Buffer];


            int prefixLength = 0, nameLength = 0, valueLength = 0;

            ParserState state = ParserState.Prefix;
            int bracketDepth = 0;
            bool leftColon = true;
            bool reset = false;
            bool hasRest = false;
            int currentOption = 0;

            for (int i = 0; i < raw.Length; i++)
            {
                hasRest = true;

                if (state == ParserState.Prefix && char.IsUpper(raw[i]))
                {
                    state = ParserState.Name;
                }

                switch (raw[i])
                {
                    case '\r':
                    case '\n':
                        state = ParserState.NextLine;

                        if (bracketDepth == 0)
                        {

                            if (i > start)
                            {
                                raw.Slice(start, i - start).Trim().CopyTo(currentName.Slice(nameLength));
                                nameLength += i - start;
                            }

                            if (nameLength > 0)
                            {

                                SetValueParse(ref currentValueParse, ref currentSegment, leftColon,
                                    currentPrefix.Slice(0, prefixLength).ToString(),
                                    currentName.Slice(0, nameLength).ToString(),
                                    currentValue.Slice(0, valueLength).ToString(),
                                    currentOption);
                            }


                            if (currentSegment.IsValid)
                            {
                                currentSet.AddSegment(currentSegment);
                                currentSegment = new SegmentParse();
                            }

                            start = i + 1;
                            leftColon = true;
                            reset = true;
                        }

                        break;

                    case ':':
                        if (bracketDepth == 0 && leftColon)
                        {
                            if (i > start)
                            {
                                raw.Slice(start, i - start).Trim().CopyTo(currentName.Slice(nameLength));
                                nameLength += i - start;
                            }

                            SetValueParse(ref currentValueParse, ref currentSegment, leftColon,
                                currentPrefix.Slice(0, prefixLength).ToString(),
                                currentName.Slice(0, nameLength).ToString(),
                                currentValue.Slice(0, valueLength).ToString(),
                                currentOption);


                            //currentSegment.AddLeft(currentPrefix.Slice(0, prefixLength).ToString(), currentName.Slice(0, nameLength).ToString(), currentValue.Slice(0, valueLength).ToString(), currentOption);

                            leftColon = false;
                            reset = true;
                            start = i + 1;
                        }

                        break;

                    case ',':
                        if (bracketDepth == 0)
                        {
                            if (i > start)
                            {
                                raw.Slice(start, i - start).Trim().CopyTo(currentName.Slice(nameLength));
                                nameLength += i - start;
                            }

                            SetValueParse(ref currentValueParse, ref currentSegment, leftColon,
                                currentPrefix.Slice(0, prefixLength).ToString(),
                                currentName.Slice(0, nameLength).ToString(),
                                currentValue.Slice(0, valueLength).ToString(),
                                currentOption);

                            reset = true;
                            start = i + 1;
                        }

                        break;

                    case '(':
                        if (state == ParserState.Name || state == ParserState.Prefix)
                        {
                            if (i > start)
                            {
                                raw.Slice(start, i - start).Trim().CopyTo(currentName.Slice(nameLength));
                                nameLength += i - start;
                            }

                            state = ParserState.InBrackets;
                            start = i + 1;
                        }

                        bracketDepth++;

                        break;

                    case ')':
                        bracketDepth--;

                        if (bracketDepth == 0 && state == ParserState.InBrackets)
                        {
                            raw.Slice(start, i - start).Trim().CopyTo(currentValue.Slice(valueLength));
                            valueLength += i - start;
                            state = ParserState.Prefix;
                            start = i + 1;
                        }

                        break;

                    case '.':
                        if (bracketDepth == 0 && state == ParserState.Prefix)
                        {
                            state = ParserState.Name;
                            raw.Slice(start, i - start).Trim().CopyTo(currentPrefix.Slice(prefixLength));
                            prefixLength += i - start;
                            start = i + 1;
                        }

                        break;

                    case '\t':
                    case ' ':
                        if (bracketDepth == 0 && i == start)
                        {
                            start++;
                        }

                        break;
                    case '#':
                    case '@':
                    case '!':
                    case '*':
                    case '+':
                        if (bracketDepth == 0)
                        {
                            currentOption = GetOptionFromChar(raw[i]);
                            start = i + 1;
                        }

                        break;

                }




                if (state == ParserState.NextLine)
                {
                    if (nameLength == 0)
                    {
                        raw.Slice(start).Trim().CopyTo(currentName.Slice(nameLength));
                        nameLength += raw.Length - start;
                    }

                    state = ParserState.Prefix;
                    leftColon = true;
                    reset = true;
                    bracketDepth = 0;
                }

                if (!reset)
                {
                    continue;
                }

                prefixLength = 0;
                nameLength = 0;
                valueLength = 0;
                currentOption = 0;
                reset = false;
                state = ParserState.Prefix;
                hasRest = false;
                bracketDepth = 0;
            }

            if (!hasRest)
            {
                if (currentSegment.IsValid)
                {
                    currentSet.AddSegment(currentSegment);
                }

                results.Add(currentSet);

                return results;
            }

            if (nameLength == 0)
            {
                raw.Slice(start).Trim().CopyTo(currentName.Slice(nameLength));
                nameLength += raw.Length - start;
            }


            SetValueParse(ref currentValueParse, ref currentSegment, leftColon,
                currentPrefix.Slice(0, prefixLength).ToString(),
                currentName.Slice(0, nameLength).ToString(),
                currentValue.Slice(0, valueLength).ToString(),
                currentOption);


            if (currentSegment.IsValid)
            {
                currentSet.AddSegment(currentSegment);
            }

            results.Add(currentSet);

            return results;
        }

    }
}


namespace SimpleLineParser.Data
{
    using System.Linq;

    public interface IParsable
    {
        bool IsValid { get; set; }
    }


    public class SegmentParse : IParsable
    {

        public SegmentParse()
        {
            IsValid = false;
        }



        public void AddRight(ValueParse right)
        {
            IsValid = true;
            Right.Add(right);
        }
        public void AddLeft(ValueParse left)
        {
            IsValid = true;
            Left.Add(left);
        }
        public void AddRight(string prefix, string name, string value, int option)
        {
            IsValid = true;
            Right.Add(new ValueParse(prefix, name, value, option));
        }

        public void AddLeft(string prefix, string name, string value, int option)
        {
            IsValid = true;
            Left.Add(new ValueParse(prefix, name, value, option));
        }

        public void Clear()
        {
            Right.Clear();
            Left.Clear();

        }

        public bool IsValid { get; set; }

        public List<ValueParse> Left { get; } = new List<ValueParse>();
        public List<ValueParse> Right { get; } = new List<ValueParse>();
    }


    public class SetParse : IParsable
    {
        public List<SegmentParse> Segments { get; } = new List<SegmentParse>();
        public List<ValueParse> Left => Segments[^1].Left;
        public List<ValueParse> Right => Segments[^1].Right;

        public SetParse()
        {
            IsValid = false;
        }


        public void AddSegment(SegmentParse segment)
        {
            IsValid = true;
            Segments.Add(segment);
        }


        public bool IsValid { get; set; }
    }


    public class ValueParse : IParsable
    {
        public ValueParse(string prefix, string name, string value, int option) : this(prefix,name,value,new int[]{option})
        {

        }
        public ValueParse(string prefix, string name, string value, int[] option)
        {
            Prefix = prefix;
            Name = name;
            Value = value;
            if (option.Length == 1 && option[0] == 0)
            {
                OnlyZero = true;
            }
            else
            {
                OnlyZero = false;
            }
            OptionSelected = new HashSet<int>(option);
        }

        public void AddOptions(int option)
        {
            if(option == 0)return;
            if (OnlyZero)
            {
                OptionSelected = new HashSet<int>();
                OnlyZero = false;
            }
            OptionSelected.Add(option);

        }

        private bool OnlyZero { get; set; }
        public string Prefix { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        private HashSet<int> OptionSelected { get; set; }
        public int[] Options => OptionSelected.ToArray();


        public bool IsValid { get; set; }
    }
}

namespace SimpleLineParser.Data.Enums
{
    public enum BracketType
    {
        Round,      // ()
        Square,     // []
        Curly,      // {}
        Angle       // <>
    }

}



namespace SimpleLineParser.Extensions
{
    using Data.Enums;
    public static class BracketTypeExtensions
    {
        public static (char Open, char Close) GetBracketChars(this BracketType bracketType)
        {
            return bracketType switch
            {
                BracketType.Round => ('(', ')'),
                BracketType.Square => ('[', ']'),
                BracketType.Curly => ('{', '}'),
                BracketType.Angle => ('<', '>'),
                _ => ('(', ')'),
            };
        }
    }

}





