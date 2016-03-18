// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using Textamina.Markdig.Helpers;

namespace Textamina.Markdig.Extensions.Diagrams
{

    public enum DiagramCharType
    {
        None,

        Text,

        Line,

        Rectangle,

        Join,

        Fixed,
    }



    public class DiagramParser
    {
        private StringLineGroup lines;

        public delegate void ArrowHandler();

        private readonly Dictionary<ArrowKey, ArrowHandler> arrowHandlers = new Dictionary<ArrowKey, ArrowHandler>();



        public void Parse()
        {

            int maxHeight = lines.Count;
            int maxWidth = 0;
            for (int y = 0; y < lines.Count; y++)
            {
                var line = lines.Lines[y];
                var width = line.Slice.Length;
                if (width > maxWidth)
                {
                    maxWidth = width;
                }
            }

            var types = new DiagramCharType[maxHeight, maxWidth];

            for (int y = 0; y < lines.Count; y++)
            {
                var line = lines.Lines[y];

                for (int x = 0; x < line.Slice.Length; x++)
                {
                    if (types[y, x] != DiagramCharType.None)
                    {
                        continue;
                    }

                    var c = line.Slice[x + line.Slice.Start];

                    switch (c)
                    {
                        case '-':
                            ParseHorizontalLine(x, y, false);
                            break;
                        case '=':
                            ParseHorizontalLine(x, y, true);
                            break;
                        case '|':
                            ParseVerticalLine(x, y);
                            break;
                        case '~':
                            ParseUpperHorizontalLine(x, y);
                            break;
                        case '_':
                            ParseLowerHorizontalLine(x, y);
                            break;
                        case '\\':
                        case '/':
                            ParseRoundEdge(x, y);
                            break;
                        case '+':
                            ParseJoin(x, y);
                            break;
                        case 'o':
                        case 'O':
                            ParseJoinOrAnchor(x, y);
                            break;
                    }
                }
            }
        }

        private void ParseJoinOrAnchor(int i, int i1)
        {
            throw new System.NotImplementedException();
        }

        private void ParseJoin(int i, int i1)
        {
            throw new System.NotImplementedException();
        }

        private void ParseRoundEdge(int i, int i1)
        {
            throw new System.NotImplementedException();
        }

        private void ParseLowerHorizontalLine(int i, int i1)
        {
            throw new System.NotImplementedException();
        }

        private void ParseUpperHorizontalLine(int i, int i1)
        {
            throw new System.NotImplementedException();
        }

        private void ParseVerticalLine(int i, int i1)
        {
            throw new System.NotImplementedException();
        }

        private void ParseHorizontalLine(int x, int y, bool thick)
        {
            var c = thick ? '=' : '-';


            int startX = x;
            int startY = y;
            var startArrowHandler = FollowLine(ref startX, ref startY, -1, 0, c, true);

            int endX = x;
            int endY = y;
            var endArrowHandler = FollowLine(ref endX, ref endY, 1, 0, c, true);



            throw new System.NotImplementedException();
        }

        private ArrowHandler FollowLine(ref int x, ref int y, int dx, int dy, char c, bool arrow = false)
        {
            bool checkArrow = false;
            char currentChar = c;
            ArrowHandler arrowHandler = null;
            while (y >= 0 && x >= 0 && y < lines.Count)
            {
                var slice = lines.Lines[y].Slice;
                if (x >= slice.Length)
                {
                    break;
                }
                currentChar = slice[slice.Start + x];
                if (currentChar != c)
                {
                    checkArrow = true;
                    break;
                }
                x += dx;
                y += dy;
            }

            if (arrow && checkArrow)
            {
                var arrowKey = new ArrowKey(currentChar, dx, dy);
                if (arrowHandlers.TryGetValue(arrowKey, out arrowHandler))
                {
                    x += dx;
                    y += dy;
                }
            }

            return arrowHandler;
        }


        private struct ArrowKey : IEquatable<ArrowKey>
        {
            public readonly char Character;

            public readonly int Dx;

            public readonly int Dy;

            public ArrowKey(char character, int dx, int dy)
            {
                Character = character;
                Dx = dx;
                Dy = dy;
            }

            public bool Equals(ArrowKey other)
            {
                return Character == other.Character && Dx == other.Dx && Dy == other.Dy;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is ArrowKey && Equals((ArrowKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = Character.GetHashCode();
                    hashCode = (hashCode*397) ^ Dx;
                    hashCode = (hashCode*397) ^ Dy;
                    return hashCode;
                }
            }

            public static bool operator ==(ArrowKey left, ArrowKey right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(ArrowKey left, ArrowKey right)
            {
                return !left.Equals(right);
            }
        }


    }
}