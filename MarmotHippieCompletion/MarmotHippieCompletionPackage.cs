using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design; // 必須引用，為了 IMenuCommandService
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace MarmotHippieCompletion
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(MarmotHippieCompletionPackage.PackageGuidString)]
    // 修正點 1: 務必解除註解，否則 VS 找不到 .vsct 定義的按鈕與快捷鍵
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class MarmotHippieCompletionPackage : AsyncPackage
    {
        public const string PackageGuidString = "10364af1-7375-4f7d-85ce-eecd51338917";

        public MarmotHippieCompletionPackage()
        {
            // 建構子通常留空
        }

        #region Package Members

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // 在背景執行緒初始化，完成後切換到 UI 執行緒
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // 修正點 2: 獲取功能表服務並初始化 Command
            // 注意：GetServiceAsync 必須在 UI 執行緒呼叫
            var commandService = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (commandService != null)
            {
                // 初始化你的 CyclicExpandCommand
                CyclicExpandCommand.Initialize(this, commandService);
            }
        }

        #endregion
    }
}
