using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.ComponentModelHost;

namespace MarmotHippieCompletion
{
    internal sealed class CyclicExpandCommand
    {
        // 1. 這些 GUID 必須與你的 .vsct 檔案一致
        public static readonly Guid CommandSet = new Guid("20364af1-7375-4f7d-85ce-eecd51338918");
        public const int CommandId = 0x0100;

        private readonly AsyncPackage package;
        private readonly WordCycleService _service = new WordCycleService();

        // 私有建構子：由 Initialize 呼叫
        private CyclicExpandCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));

            // 將命令註冊到選單系統
            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static CyclicExpandCommand Instance { get; private set; }

        // --- 這是你目前遺漏的核心方法 ---
        public static void Initialize(AsyncPackage package, OleMenuCommandService commandService)
        {
            // ThreadHelper.ThrowIfNotOnUIThread(); // 如果是在 AsyncPackage 初始化，通常已經切換過執行緒
            Instance = new CyclicExpandCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var textView = GetWpfTextView();
            if (textView == null) return;

            var caret = textView.Caret.Position.BufferPosition;
            SnapshotSpan wordSpan = FindWordBeforeCaret(caret);
            string currentWord = wordSpan.GetText();

            // 如果不是連續按，就重新初始化循環搜尋
            if (!_service.IsContinuing(currentWord))
            {
                _service.Initialize(textView.TextSnapshot.GetText(), currentWord);
            }

            string replacement = _service.GetNextMatch();

            // 執行替換
            using (var edit = textView.TextBuffer.CreateEdit())
            {
                edit.Replace(wordSpan, replacement);
                edit.Apply();
            }

            // 將游標移動到新單字的末尾
            var newPos = new SnapshotPoint(textView.TextSnapshot, wordSpan.Start + replacement.Length);
            textView.Caret.MoveTo(newPos);
        }

        private IWpfTextView GetWpfTextView()
        {
            var textManager = (IVsTextManager)ServiceProvider.GlobalProvider.GetService(typeof(SVsTextManager));
            if (textManager == null) return null;

            textManager.GetActiveView(1, null, out IVsTextView view);
            if (view == null) return null;

            var componentModel = (IComponentModel)ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel));
            var editorAdapterFactory = componentModel.GetService<IVsEditorAdaptersFactoryService>();

            return editorAdapterFactory.GetWpfTextView(view);
        }

        private SnapshotSpan FindWordBeforeCaret(SnapshotPoint caret)
        {
            int start = caret.Position;
            while (start > 0 && (char.IsLetterOrDigit(caret.Snapshot[start - 1]) || caret.Snapshot[start - 1] == '_'))
            {
                start--;
            }
            return new SnapshotSpan(caret.Snapshot, new Span(start, caret.Position - start));
        }
    }
}
