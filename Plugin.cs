using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using GlamourModelBrowser.Services;
using GlamourModelBrowser.Windows;

namespace GlamourModelBrowser;

public sealed class Plugin : IDalamudPlugin
{
    private const string CommandName = "/gmb";
    private readonly IDalamudPluginInterface pluginInterface;
    private readonly ICommandManager commandManager;
    private readonly IPluginLog log;
    private readonly Configuration configuration;
    private readonly WindowSystem windowSystem = new("GlamourModelBrowser");
    private readonly ModelBrowserWindow browserWindow;
    private readonly TryOnButtonOverlay buttonOverlay;

    public Plugin(
        IDalamudPluginInterface pluginInterface,
        ICommandManager commandManager,
        IDataManager dataManager,
        IChatGui chatGui,
        IGameGui gameGui,
        ITextureProvider textureProvider,
        IPluginLog log)
    {
        this.pluginInterface = pluginInterface;
        this.commandManager = commandManager;
        this.log = log;
        this.configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        this.configuration.Version = 1;
        this.configuration.MaxResults = Math.Clamp(this.configuration.MaxResults, 20, 1000);
        this.configuration.Save(pluginInterface);

        var index = new GlamourModelIndex(dataManager, log);
        var reader = new TryOnReader();
        var printer = new EchoItemPrinter(chatGui);
        this.browserWindow = new ModelBrowserWindow(this.configuration, reader, index, printer, textureProvider, log);
        this.buttonOverlay = new TryOnButtonOverlay(gameGui, this.configuration, this.browserWindow);
        this.windowSystem.AddWindow(this.browserWindow);

        pluginInterface.UiBuilder.Draw += this.Draw;
        pluginInterface.UiBuilder.OpenMainUi += this.OpenWindow;
        pluginInterface.UiBuilder.OpenConfigUi += this.OpenWindow;

        this.commandManager.AddHandler(CommandName, new CommandInfo(this.OnCommand)
        {
            HelpMessage = "打开同模幻化浏览器。",
            ShowInHelp = true,
        });
    }

    public string Name => "Glamour Model Browser";

    private void Draw()
    {
        this.buttonOverlay.Draw();
        this.windowSystem.Draw();
    }

    private void OnCommand(string command, string arguments)
    {
        this.browserWindow.Refresh();
        this.browserWindow.IsOpen = true;
    }

    private void OpenWindow() => this.OnCommand(CommandName, string.Empty);

    public void Dispose()
    {
        this.pluginInterface.UiBuilder.Draw -= this.Draw;
        this.pluginInterface.UiBuilder.OpenMainUi -= this.OpenWindow;
        this.pluginInterface.UiBuilder.OpenConfigUi -= this.OpenWindow;
        this.windowSystem.RemoveAllWindows();
        this.commandManager.RemoveHandler(CommandName);
    }
}
