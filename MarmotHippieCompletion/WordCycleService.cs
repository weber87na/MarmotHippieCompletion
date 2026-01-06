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
        private List<string> _matches = new List<string>();
        private int _currentIndex = -1;
        private string _initialWord;
        private string _lastReplacement;

        /// <summary>
        /// 初始化搜尋結果，加入距離排序邏輯
        /// </summary>
        /// <param name="documentText">文件全文</param>
        /// <param name="currentWord">目前游標前的單字（前綴）</param>
        /// <param name="caretPosition">目前游標位置，用於計算距離</param>
        public void Initialize(string documentText, string currentWord, int caretPosition)
        {
            _initialWord = currentWord;
            _lastReplacement = currentWord;

            if (string.IsNullOrWhiteSpace(currentWord))
            {
                _matches = new List<string>();
                return;
            }

            // 1. 找出所有單字及其位置
            // \b\w+\b 匹配完整單字
            var allMatches = Regex.Matches(documentText, @"\b\w+\b")
                                  .Cast<Match>()
                                  .Where(m => m.Value.StartsWith(currentWord) && m.Value != currentWord)
                                  .ToList();

            // 2. 依照與游標的「距離」排序
            // 離游標越近的單字，越有可能是使用者想要的
            var sortedWords = allMatches
                .OrderBy(m => Math.Abs(m.Index - caretPosition))
                .Select(m => m.Value)
                .Distinct() // 去除重複，但保留最近的順序
                .ToList();

            // 3. 將「原始輸入」加入循環的最末端
            // 這樣使用者如果按太多次，可以回到最初的狀態
            sortedWords.Add(_initialWord);

            _matches = sortedWords;
            _currentIndex = -1;
        }

        public bool IsContinuing(string currentWord)
        {
            return !string.IsNullOrEmpty(_lastReplacement) && currentWord == _lastReplacement;
        }

        public string GetNextMatch()
        {
            if (_matches.Count == 0) return _initialWord;

            _currentIndex++;
            if (_currentIndex >= _matches.Count) _currentIndex = 0;

            _lastReplacement = _matches[_currentIndex];
            return _lastReplacement;
        }

        public string GetPreviousMatch()
        {
            if (_matches.Count == 0) return _initialWord;

            _currentIndex--;
            if (_currentIndex < 0) _currentIndex = _matches.Count - 1;

            _lastReplacement = _matches[_currentIndex];
            return _lastReplacement;
        }
    }

}
