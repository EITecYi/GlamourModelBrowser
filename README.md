# Glamour Model Browser

## 同模型数据与算法

- 插件只读取玩家本机客户端的 `Item` 表，不访问、不抓取、不缓存 `garlandtools.cn` 或任何 Wiki 的在线物品数据。
- 同模型分组使用“游戏模型文件实际使用的模型号 + 第一个实际装备槽位”，而不是直接比较完整的 64 位 `ModelMain`。头部、身体、手部、腿部、脚部和饰品会忽略 `ModelMain` 中不参与模型文件路径的变体分段；武器保留“武器号 + 机体号”。因此“身体”与“身体+手部”等分类行不同、但外观相同的装备会进入同一组，职业或材质变体也不会再因完整编码不同而被错误排除。
- 同模型结果始终包含当前正在试穿的本装备，便于直接与同组的其他装备一起查看和点击对比。
- 算法思路参考 Garland Tools 的公开 MIT 源码实现；版权与许可说明见 `THIRD-PARTY-NOTICES.md`。
- 这是本地游戏数据推断，不能承诺与任意第三方网站的结果逐项完全一致。

这是一个独立的 Dalamud 插件项目。

## 功能

- 打开普通试穿、投影模板或幻化柜时，在窗口上方显示“同模”按钮。
- 读取普通试穿、投影模板预览和幻化柜试穿/当前选择中的装备。
- 按 `ModelMain`、`ModelSub` 与实际装备槽位交集查找同模型物品；可识别“躯干”和“躯干+手部”等分类行不同但外观相同的变体。
- 用游戏物品图标显示同模结果。
- 点击图标后，在 `/e` 默语频道显示可点击的物品链接。
- 也可以使用 `/gmb` 手动打开窗口。

## 构建

```powershell
$env:DOTNET_CLI_HOME = 'F:\[ACG]\[GAME]\FF14\[Plugin]\GlamourModelBrowser\.dotnet-home'
$env:NUGET_PACKAGES = 'F:\[ACG]\[GAME]\FF14\[Plugin]\GlamourModelBrowser\.dotnet-home\.nuget\packages'
dotnet build '.\GlamourModelBrowser.csproj' -c Release -p:DalamudLibPath='C:\Users\CHARLIE\AppData\Roaming\XIVLauncherCN\addon\Hooks\dev' --no-restore --nologo
```

编译输出位于 `build\GlamourModelBrowser.dll`。

## 在 Dalamud CN 中加载开发插件

1. 启动游戏并注入 Dalamud。
2. 打开 Dalamud 设置，进入“开发插件”或“Dev Plugin”页面。
3. 确认已开启开发插件模式。
4. 添加下面这个文件作为开发插件加载位置：

   `F:\[ACG]\[GAME]\FF14\[Plugin]\GlamourModelBrowser\build\GlamourModelBrowser.dll`

5. 在插件列表中找到“Glamour Model Browser”，点击加载。

如果看不到新插件，先重新扫描开发插件；仍然看不到时，重启一次游戏和 Dalamud。不要把 DLL 放进 `addon\Hooks\dev`，那个目录是开发库，不是插件目录。

## 游戏内测试

1. 打开一个可以试穿的物品，进入原生“试穿”窗口；也可以打开投影模板或幻化柜。
2. 在聊天栏输入 `/gmb`，确认插件窗口可以打开。
3. 点击“刷新当前试穿”，确认窗口中出现当前试穿装备槽位。
4. 选择一个装备槽位，等待首次建立本地 Item 模型索引。
5. 检查同模型结果是否出现图标和名称。
6. 点击任意图标，确认 `/e` 默语频道出现可点击的物品链接。
7. 在投影模板中选择一个模板，确认“读取来源”显示为“投影模板”，并能列出该模板的装备槽位。
8. 在幻化柜中对一个物品执行试穿；确认“读取来源”显示为“幻化柜”。若未打开独立试穿窗口，则先右键或选中该物品，再点击刷新读取当前选择。
9. 关闭并重新打开普通试穿窗口，确认窗口上方出现“同模”按钮；点击它应当直接打开插件窗口并刷新数据。

插件运行时不访问 Wiki，数据来自当前客户端的本地 `Item` 表。试穿记录同时可能包含原始装备 ID 和实际渲染的幻化 ID；当 `GlamourId` 存在时，插件会优先按它查找同模，以保证比较的是玩家眼前显示的幻化，而不是被投影覆盖的原装备。

## 常见测试结果

- `/gmb` 无反应：插件没有成功加载，先看 Dalamud 插件列表和日志。
- 能打开窗口但没有装备：必须先打开原生试穿窗口，再点击“刷新当前试穿”。
- 有装备但没有同模：记录界面显示的“读取 ID”“幻化 ID”和“装备槽分类”，这能判断是试穿内存布局变化还是 Item 表匹配问题。
- 有结果但点击没有聊天链接：检查是否在游戏内 `/e` 默语频道，且没有关闭聊天输出；插件发送的是完整的游戏物品链接 payload，不是普通文本。
