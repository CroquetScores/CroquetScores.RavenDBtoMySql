// ReSharper disable UnusedMember.Global

using System;
using System.Linq;
using Newtonsoft.Json;
using OpenMagic.Extensions;

namespace CroquetScores.RavenDB.Documents.Types
{
    public struct Score
    {
        private static readonly string[] WinLetters;

        static Score()
        {
            WinLetters = new[] { "w" };
        }

        public static implicit operator Score(string value)
        {
            return new Score(value);
        }

        public static bool operator >(Score left, Score right)
        {
            return left.Hoops > right.Hoops || (left.Hoops == right.Hoops && left.IsWin());
        }

        public static bool operator <(Score left, Score right)
        {
            return !(left > right);
        }

        private Score(string score)
        {
            if (score.IsNullOrWhiteSpace())
            {
                throw new FormatException("Score is required.");
            }

            score = score.Trim();

            if (!score[0].IsInteger())
            {
                throw new FormatException($"'{score}' is not a valid score. The first character must be a number.");
            }

            _Score = score;
        }

        // ReSharper disable once InconsistentNaming
        // Property is required for JSON serializer to work
        public string _Score { get; set; }

        [JsonIgnore]
        public int Hoops => throw
            // return Convert.ToInt32(Conversion.Val(_Score));
            //
            //
            new NotImplementedException();

        [JsonIgnore]
        public string Peeling
        {
            get
            {
                var hoopCount = Hoops.ToString();

                return _Score.Length == hoopCount.Length ? null : _Score.Substring(hoopCount.Length).Trim();
            }
        }

        public string Result(Score opponent)
        {
            var difference = Hoops - opponent.Hoops;
            var value = $"{difference}{Peeling}{opponent.Peeling}";

            if (difference > 0)
            {
                return "+" + value;
            }

            return value;
        }

        public override string ToString()
        {
            return _Score;
        }

        private bool IsWin()
        {
            var peel = Peeling;

            return peel != null && (PeelingEqualsWinLetters(peel) || PeelingStartsWithWinLetters(peel));
        }

        private static bool PeelingStartsWithWinLetters(string peel)
        {
            return WinLetters.Any(letters => peel.StartsWith(letters + " ", StringComparison.OrdinalIgnoreCase));
        }

        private static bool PeelingEqualsWinLetters(string peel)
        {
            return WinLetters.Any(letters => peel.Equals(letters, StringComparison.OrdinalIgnoreCase));
        }
    }
}