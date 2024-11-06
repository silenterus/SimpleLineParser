using System;
using System.Collections.Generic;
using System.Linq;
using SimpleLineParser.Data;
using SimpleLineParser.Data.Enums;

namespace SimpleLineParser.Parser
{
    public static class Scheme
    {
        private static readonly int Buffer = 8192;

        readonly static Dictionary<char, BracketType> OpenBrackets = new Dictionary<char, BracketType>
        {
            { '(', BracketType.Round },
            { '[', BracketType.Square },
            { '{', BracketType.Curly },
            { '<', BracketType.Angle }
        };

        readonly static Dictionary<char, BracketType> CloseBrackets = new Dictionary<char, BracketType>
        {
            { ')', BracketType.Round },
            { ']', BracketType.Square },
            { '}', BracketType.Curly },
            { '>', BracketType.Angle }
        };

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
                case '@': return 6;
                case '*': return 3;
                case '+': return 4;
                case '#': return 5;
                case '>': return 7;
                case '<': return 2;
                default: return 0;
            }
        }

        private static void SetValueParse(ref ValueParse currentValueParse, ref SegmentParse segment, bool addToLeft, string prefix, string name, string value, int option, BracketType bracketType)
        {
            currentValueParse.Prefix = prefix;
            currentValueParse.Name = name;
            currentValueParse.Value = value;
            currentValueParse.AddOptions(option);
            currentValueParse.ValueBracket = bracketType;

            if (addToLeft)
            {
                segment.AddLeft(currentValueParse);
            }
            else
            {
                segment.AddRight(currentValueParse);
            }

            currentValueParse = new ValueParse(string.Empty, string.Empty, string.Empty, 0, BracketType.Round);
        }

        private static int CopyWithoutSpaces(ReadOnlySpan<char> source, Span<char> destination)
        {
            int destIndex = 0;
            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];
                if (c != ' ')
                {
                    destination[destIndex++] = c;
                }
            }
            return destIndex;
        }

        private static int CopyWithoutSpacesAndNewLines(ReadOnlySpan<char> source, Span<char> destination)
        {
            int destIndex = 0;
            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];
                if (c != ' ' && c != '\n' && c != '\r')
                {
                    destination[destIndex++] = c;
                }
            }
            return destIndex;
        }

        private static List<SetParse> ParseSpan(ReadOnlySpan<char> raw)
        {
            if (raw.IsEmpty)
            {
                return new List<SetParse>();
            }

            int start = 0;
            List<SetParse> results = new List<SetParse>();

            ValueParse currentValueParse = new ValueParse(string.Empty, string.Empty, string.Empty, 0, BracketType.Round);
            SetParse currentSet = new SetParse();
            SegmentParse currentSegment = new SegmentParse();

            Span<char> currentPrefix = stackalloc char[Buffer];
            Span<char> currentName = stackalloc char[Buffer];
            Span<char> currentValue = stackalloc char[Buffer];

            int prefixLength = 0, nameLength = 0, valueLength = 0;

            ParserState state = ParserState.Prefix;

            Stack<BracketType> bracketStack = new Stack<BracketType>();

            bool leftColon = true;
            bool reset = false;
            bool hasRest = false;
            int currentOption = 0;

            // Variables to track bracket nesting
            BracketType currentBracketType = BracketType.Round; // default
            int nestingLevel = 0;
            int maxNestingLevel = 0;

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
                        // Check if the next non-space character is not a semicolon or comma
                        int lookaheadIndex = i + 1;
                        while (lookaheadIndex < raw.Length && char.IsWhiteSpace(raw[lookaheadIndex]))
                        {
                            lookaheadIndex++;
                        }
                        if (lookaheadIndex < raw.Length && (raw[lookaheadIndex] == ':' || raw[lookaheadIndex] == ','))
                        {
                            // Do not trigger ParserState.NextLine
                            break;
                        }

                        state = ParserState.NextLine;

                        if (bracketStack.Count == 0)
                        {
                            if (i > start)
                            {
                                nameLength += CopyWithoutSpacesAndNewLines(raw.Slice(start, i - start).Trim(), currentName.Slice(nameLength));
                            }

                            if (nameLength > 0)
                            {
                                SetValueParse(ref currentValueParse, ref currentSegment, leftColon,
                                    currentPrefix.Slice(0, prefixLength).ToString(),
                                    currentName.Slice(0, nameLength).ToString(),
                                    currentValue.Slice(0, valueLength).ToString(),
                                    currentOption,
                                    currentValueParse.ValueBracket);
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
                        if (bracketStack.Count == 0 && leftColon)
                        {
                            if (i > start)
                            {
                                nameLength += CopyWithoutSpacesAndNewLines(raw.Slice(start, i - start).Trim(), currentName.Slice(nameLength));
                            }

                            SetValueParse(ref currentValueParse, ref currentSegment, leftColon,
                                currentPrefix.Slice(0, prefixLength).ToString(),
                                currentName.Slice(0, nameLength).ToString(),
                                currentValue.Slice(0, valueLength).ToString(),
                                currentOption,
                                currentValueParse.ValueBracket);

                            leftColon = false;
                            reset = true;
                            start = i + 1;
                        }

                        break;

                    case ',':
                        if (bracketStack.Count == 0)
                        {
                            if (i > start)
                            {
                                nameLength += CopyWithoutSpacesAndNewLines(raw.Slice(start, i - start).Trim(), currentName.Slice(nameLength));
                            }

                            SetValueParse(ref currentValueParse, ref currentSegment, leftColon,
                                currentPrefix.Slice(0, prefixLength).ToString(),
                                currentName.Slice(0, nameLength).ToString(),
                                currentValue.Slice(0, valueLength).ToString(),
                                currentOption,
                                currentValueParse.ValueBracket);

                            reset = true;
                            start = i + 1;
                        }

                        break;

                    case '(':
                    case '[':
                    case '{':
                    case '<':
                        var bracketTypeOpen = OpenBrackets[raw[i]];
                        bracketStack.Push(bracketTypeOpen);

                        if (state == ParserState.Name || state == ParserState.Prefix)
                        {
                            if (i > start)
                            {
                                nameLength += CopyWithoutSpacesAndNewLines(raw.Slice(start, i - start).Trim(), currentName.Slice(nameLength));
                            }

                            state = ParserState.InBrackets;
                            start = i + 1;

                            // Initialize the current bracket type and nesting level
                            currentBracketType = bracketTypeOpen;
                            nestingLevel = 1;
                            maxNestingLevel = 1;
                        }
                        else if (state == ParserState.InBrackets)
                        {
                            // Check if the bracket type is the same
                            if (bracketTypeOpen == currentBracketType)
                            {
                                nestingLevel++;
                                if (nestingLevel > maxNestingLevel)
                                {
                                    maxNestingLevel = nestingLevel;
                                }
                            }
                            else
                            {
                                // Handle different bracket type if necessary
                                // For now, we ignore or you can implement nested different brackets handling
                            }
                        }

                        break;

                    case ')':
                    case ']':
                    case '}':
                    case '>':
                        var bracketTypeClose = CloseBrackets[raw[i]];
                        if (bracketStack.Count > 0 && bracketTypeClose == bracketStack.Peek())
                        {
                            bracketStack.Pop();

                            if (state == ParserState.InBrackets)
                            {
                                if (bracketTypeClose == currentBracketType)
                                {
                                    nestingLevel--;

                                    if (nestingLevel == 0)
                                    {
                                        // We have closed all brackets of the current type
                                        var valueSlice = raw.Slice(start, i - start).Trim();
                                        valueLength += valueSlice.Length;
                                        valueSlice.CopyTo(currentValue.Slice(0, valueLength));

                                        // Determine BracketType based on maxNestingLevel
                                        BracketType assignedBracketType = currentBracketType;
                                        switch (currentBracketType)
                                        {
                                            case BracketType.Round:
                                                switch (maxNestingLevel)
                                                {
                                                    case 1:
                                                        assignedBracketType = BracketType.Round;
                                                        break;
                                                    case 2:
                                                        assignedBracketType = BracketType.DoubleRound;
                                                        break;
                                                    case 3:
                                                        assignedBracketType = BracketType.TripleRound;
                                                        break;
                                                    case 4:
                                                        assignedBracketType = BracketType.QuadrupleRound;
                                                        break;
                                                }
                                                break;
                                            case BracketType.Square:
                                                switch (maxNestingLevel)
                                                {
                                                    case 1:
                                                        assignedBracketType = BracketType.Square;
                                                        break;
                                                    case 2:
                                                        assignedBracketType = BracketType.DoubleSquare;
                                                        break;
                                                    case 3:
                                                        assignedBracketType = BracketType.TripleSquare;
                                                        break;
                                                    case 4:
                                                        assignedBracketType = BracketType.QuadrupleSquare;
                                                        break;
                                                }
                                                break;
                                            case BracketType.Curly:
                                                switch (maxNestingLevel)
                                                {
                                                    case 1:
                                                        assignedBracketType = BracketType.Curly;
                                                        break;
                                                    case 2:
                                                        assignedBracketType = BracketType.DoubleCurly;
                                                        break;
                                                    case 3:
                                                        assignedBracketType = BracketType.TripleCurly;
                                                        break;
                                                    case 4:
                                                        assignedBracketType = BracketType.QuadrupleCurly;
                                                        break;
                                                }
                                                break;
                                            case BracketType.Angle:
                                                switch (maxNestingLevel)
                                                {
                                                    case 1:
                                                        assignedBracketType = BracketType.Angle;
                                                        break;
                                                    case 2:
                                                        assignedBracketType = BracketType.DoubleAngle;
                                                        break;
                                                    case 3:
                                                        assignedBracketType = BracketType.TripleAngle;
                                                        break;
                                                    case 4:
                                                        assignedBracketType = BracketType.QuadrupleAngle;
                                                        break;
                                                }
                                                break;
                                        }

                                        currentValueParse.ValueBracket = assignedBracketType; // Set the BracketType

                                        state = ParserState.Prefix;
                                        start = i + 1;
                                    }
                                }
                                else
                                {
                                    // Handle closing a different type of bracket if necessary
                                }
                            }
                        }
                        else
                        {
                            // Handle unmatched closing bracket, error or ignore
                        }
                        break;

                    case '.':
                        if (bracketStack.Count == 0 && state == ParserState.Prefix)
                        {
                            state = ParserState.Name;
                            prefixLength += CopyWithoutSpacesAndNewLines(raw.Slice(start, i - start).Trim(), currentPrefix.Slice(prefixLength));
                            start = i + 1;
                        }

                        break;

                    case '#':
                    case '@':
                    case '!':
                    case '*':
                    case '+':
                        if (bracketStack.Count == 0)
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
                        nameLength += CopyWithoutSpacesAndNewLines(raw.Slice(start).Trim(), currentName.Slice(nameLength));
                    }

                    state = ParserState.Prefix;
                    leftColon = true;
                    reset = true;
                    bracketStack.Clear();
                    nestingLevel = 0;
                    maxNestingLevel = 0;
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
                bracketStack.Clear();
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
                nameLength += CopyWithoutSpacesAndNewLines(raw.Slice(start).Trim(), currentName.Slice(nameLength));
            }

            SetValueParse(ref currentValueParse, ref currentSegment, leftColon,
                currentPrefix.Slice(0, prefixLength).ToString(),
                currentName.Slice(0, nameLength).ToString(),
                currentValue.Slice(0, valueLength).ToString(),
                currentOption,
                currentValueParse.ValueBracket);

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
        public void AddRight(string prefix, string name, string value, int option, BracketType bracketType)
        {
            IsValid = true;
            Right.Add(new ValueParse(prefix, name, value, option, bracketType));
        }

        public void AddLeft(string prefix, string name, string value, int option, BracketType bracketType)
        {
            IsValid = true;
            Left.Add(new ValueParse(prefix, name, value, option, bracketType));
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
        public ValueParse(string prefix, string name, string value, int option, BracketType bracketType) : this(prefix, name, value, new int[] { option }, bracketType)
        {

        }
        public ValueParse(string prefix, string name, string value, int[] option, BracketType bracketType)
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
            ValueBracket = bracketType;
            OptionSelected = new HashSet<int>(option);
        }

        public void AddOptions(int option)
        {
            if (option == 0) return;
            if (OnlyZero)
            {
                OptionSelected = new HashSet<int>();
                OnlyZero = false;
            }
            OptionSelected.Add(option);

        }

        public void SetOption(int option)
        {
            OptionSelected.Clear();
            OptionSelected.Add(option);
        }
        private bool OnlyZero { get; set; }
        public string Prefix { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public BracketType ValueBracket { get; set; }

        private HashSet<int> OptionSelected { get; set; }
        public int[] Options => OptionSelected.ToArray();

        public bool IsValid { get; set; }
    }
}

namespace SimpleLineParser.Data.Enums
{
    public enum BracketType
    {
        Round,           // ()
        DoubleRound,     // (())
        TripleRound,     // ((()))
        QuadrupleRound,  // (((())))

        Square,          // []
        DoubleSquare,    // [[]]
        TripleSquare,    // [[[]]]
        QuadrupleSquare, // [[[[]]]]

        Curly,           // {}
        DoubleCurly,     // {{}}
        TripleCurly,     // {{{}}}
        QuadrupleCurly,  // {{{{}}}}

        Angle,           // <>
        DoubleAngle,     // <<>>
        TripleAngle,     // <<<>>>
        QuadrupleAngle   // <<<<>>>>
    }

}
