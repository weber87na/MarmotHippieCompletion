using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MarmotHippieCompletion
{
    public class WordCycleService
    {
        private string _originalPrefix;
        private List<string> _matches = new List<string>();
        private int _currentIndex = -1;
        private string _lastReplacedWord;

        // 判斷是否為連續觸發（如果是連續觸發，就繼續循環，否則重新掃描）
        public bool IsContinuing(string currentWordUnderCaret)
        {
            return _currentIndex != -1 && currentWordUnderCaret == _lastReplacedWord;
        }

        public void Initialize(string text, string prefix)
        {
            _originalPrefix = prefix;
            // 找出所有以 prefix 開頭的單字，排除掉 prefix 本身，並保持出現順序
            _matches = Regex.Matches(text, @"\b" + Regex.Escape(prefix) + @"\w+\b")
                            .Cast<Match>()
                            .Select(m => m.Value)
                            .Where(w => w != prefix)
                            .Distinct()
                            .ToList();

            _currentIndex = -1;
        }

        public string GetNextMatch()
        {
            if (_matches.Count == 0) return _originalPrefix;
            _currentIndex = (_currentIndex + 1) % _matches.Count;
            _lastReplacedWord = _matches[_currentIndex];
            return _lastReplacedWord;
        }

        public void Reset()
        {
            _currentIndex = -1;
            _matches.Clear();
        }
    }

}
