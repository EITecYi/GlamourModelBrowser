using Dalamud.Configuration;
using Dalamud.Plugin;

namespace GlamourModelBrowser;

public sealed class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    public bool ShowTryOnButton { get; set; } = true;

    public int MaxResults { get; set; } = 200;

    public void Save(IDalamudPluginInterface pluginInterface) => pluginInterface.SavePluginConfig(this);
}
