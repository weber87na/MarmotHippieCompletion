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
        // 1. 命令識別碼 (必須與 .vsct 檔案一致)
        public static readonly Guid CommandSet = new Guid("20364af1-7375-4f7d-85ce-eecd51338918");
        public const int ForwardCommandId = 0x0100;
        public const int BackwardCommandId = 0x0101;

        private readonly AsyncPackage package;
        private readonly WordCycleService _service = new WordCycleService();

        private CyclicExpandCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));

            if (commandService != null)
            {
                // 註冊正向循環命令 (Alt + /)
                var forwardID = new CommandID(CommandSet, ForwardCommandId);
                var forwardItem = new MenuCommand(this.Execute, forwardID);
                commandService.AddCommand(forwardItem);

                // 註冊反向循環命令 (Alt + Shift + /)
                var backwardID = new CommandID(CommandSet, BackwardCommandId);
                var backwardItem = new MenuCommand(this.Execute, backwardID);
                commandService.AddCommand(backwardItem);
            }
        }

        public static CyclicExpandCommand Instance { get; private set; }

        public static void Initialize(AsyncPackage package, OleMenuCommandService commandService)
        {
            Instance = new CyclicExpandCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // 辨識是哪個命令被觸發
            var menuCommand = sender as MenuCommand;
            if (menuCommand == null) return;

            var textView = GetWpfTextView();
            if (textView == null) return;

            var caret = textView.Caret.Position.BufferPosition;
            SnapshotSpan wordSpan = FindWordBeforeCaret(caret);
            string currentWord = wordSpan.GetText();

            // 1. 初始化檢查：如果不是連續按，或是字串不匹配，就重新搜尋
            if (!_service.IsContinuing(currentWord))
            {
                // 傳入目前的全文、前綴、以及游標位置（用於距離排序）
                _service.Initialize(textView.TextSnapshot.GetText(), currentWord, caret.Position);
            }

            // 2. 根據 Command ID 決定拿「下一個」還是「上一個」
            string replacement;
            if (menuCommand.CommandID.ID == BackwardCommandId)
            {
                replacement = _service.GetPreviousMatch();
            }
            else
            {
                replacement = _service.GetNextMatch();
            }

            // 3. 執行替換文本
            if (replacement != null)
            {
                using (var edit = textView.TextBuffer.CreateEdit())
                {
                    edit.Replace(wordSpan, replacement);
                    edit.Apply();
                }

                // 4. 更新游標位置到單字末尾
                var newPos = new SnapshotPoint(textView.TextSnapshot, wordSpan.Start + replacement.Length);
                textView.Caret.MoveTo(newPos);
            }
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
            // 搜尋條件：字母、數字或底線
            while (start > 0 && (char.IsLetterOrDigit(caret.Snapshot[start - 1]) || caret.Snapshot[start - 1] == '_'))
            {
                start--;
            }
            return new SnapshotSpan(caret.Snapshot, new Span(start, caret.Position - start));
        }
    }
}
