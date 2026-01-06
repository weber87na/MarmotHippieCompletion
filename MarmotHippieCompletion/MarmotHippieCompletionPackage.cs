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
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // 修正建議：VS 2022 建議使用 UIContextGuid 來減少不必要的載入
    // 使用「編輯器開啟時才載入」會比「無條件載入」效能更好
    [ProvideAutoLoad(UIContextGuids80.CodeWindow, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class MarmotHippieCompletionPackage : AsyncPackage
    {
        public const string PackageGuidString = "10364af1-7375-4f7d-85ce-eecd51338917";

        public MarmotHippieCompletionPackage()
        {
        }

        #region Package Members

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // 1. 初始化時先不要切換執行緒，先進行非同步的服務獲取
            // 這樣可以最大化利用 AsyncPackage 的優勢
            var commandServiceTask = GetServiceAsync(typeof(IMenuCommandService));

            // 2. 切換到 UI 執行緒以註冊命令
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // 3. 獲取服務結果並轉型
            var commandService = await commandServiceTask as OleMenuCommandService;

            if (commandService != null)
            {
                // 初始化 CyclicExpandCommand
                // 確保 CyclicExpandCommand 內部使用的 CommandSet GUID 
                // 與 .vsct 中的 guidMarmotPackageCmdSet 一致
                CyclicExpandCommand.Initialize(this, commandService);
            }
        }

        #endregion
    }
}
