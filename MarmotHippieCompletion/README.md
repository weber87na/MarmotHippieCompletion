# Marmot Hippie Completion

**A tribute to the classic `Hippie Completion`!** This is a fast completion tool built for Visual Studio 2022, designed to replicate the beloved **"Cyclic Expand Word"** feature found in the IntelliJ IDEA / JetBrains series.

It automatically scans the words that have appeared in your current file, allowing you to quickly complete text with a simple keystroke—no heavy IntelliSense indexing required.

---

## 🚀 Key Features

* **Cyclic Expansion**: Automatically extracts words from the text buffer and cycles through matches in order.
* **Blazing Fast**: Powered by pure string-matching logic with zero CPU overhead. Instant results every time you press the key.
* **Muscle Memory**: Perfectly aligns with IntelliJ / Emacs operation logic to double your development efficiency.

## ⌨️ Default Shortcut

* **Shortcut**: `Alt + /`

---

## ⚠️ Resolving Shortcut Conflicts (GitHub Copilot)

By default, GitHub Copilot’s **`Ask.Copilot`** also occupies `Alt + /`, which may prevent this extension from working. If the shortcut does not respond, please follow these steps to manually reassign it:

### 1. Remove the Copilot Binding
1. Go to **Tools** > **Options**.
2. Navigate to **Environment** > **Keyboard**.
3. In the "Show commands containing" search box, enter: `Ask.Copilot`.
4. Select the command and click the **Remove** button on the right.



### 2. Assign the Shortcut to Marmot
1. While still in the **Keyboard** settings page.
2. Search for: `Marmot`.
3. Select `Edit.MarmotCyclicExpand` (or the name you defined in your `.vsct` file).
4. Set **"Use new shortcut in"** to **Text Editor**.
5. Click inside the **"Press shortcut keys"** box and press **`Alt + /`**.
6. Click **Assign**, then click **OK** to save.


---


# Marmot Hippie Completion

**致敬經典的 `Hippie Completion`！** 這是一款為 Visual Studio 2022 打造的快速補全工具，旨在復刻 IntelliJ IDEA / JetBrains 系列中深受開發者喜愛的「循環單字展開 (Cyclic Expand Word)」功能。

它可以自動掃描當前檔案中出現過的文字，讓您透過簡單的按鍵快速補全，無需依賴繁重的 IntelliSense 索引。

---

## 🚀 主要功能

* **循環補全 (Cyclic Expand)**：自動提取文字緩衝區中的單字，並依序進行循環替換。
* **極速體驗**：純字串匹配邏輯，不佔用 CPU 資源，即按即出。
* **肌肉記憶**：完美對標 IntelliJ / Emacs 的操作邏輯，讓您的開發效率翻倍。

## ⌨️ 預設快捷鍵

* **快捷鍵**：`Alt + /`

---

## ⚠️ 解決快捷鍵衝突 (GitHub Copilot)

由於 GitHub Copilot 的 **`Ask.Copilot`** 預設也佔用了 `Alt + /`，這會導致插件失效。若您按下快捷鍵沒有反應，請依照以下步驟手動重新指派：

### 1. 移除 Copilot 的佔用
1. 前往 **工具 (Tools)** > **選項 (Options)**。
2. 導航至 **環境 (Environment)** > **鍵盤 (Keyboard)**。
3. 在「顯示命令包含」搜尋框輸入：`Ask.Copilot`。
4. 選中該命令，點擊右側的 **移除 (Remove)** 按鈕。



### 2. 指派給 Marmot
1. 仍在 **鍵盤 (Keyboard)** 設定頁面中。
2. 在搜尋框輸入：`Marmot`。
3. 選中 `Edit.MarmotCyclicExpand` (或您在 vsct 中定義的名稱)。
4. 將「新快速鍵用於」設定為 **文字編輯器 (Text Editor)**。
5. 在「按快速鍵」框中按下 **`Alt + /`**。
6. 點擊 **指派 (Assign)**，最後按下確定。
