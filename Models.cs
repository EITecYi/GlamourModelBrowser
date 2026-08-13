namespace GlamourModelBrowser;

// A visual model group is identified by the model-file identity and the item's
// first physical equipment slot. ModelMain contains several packed 16-bit
// values; the whole 64-bit value is not the visual model identity.
public readonly record struct ModelKey(ulong VisualModelIdentity, ulong PrimarySlotMask);

public sealed record ItemModelInfo(
    uint RowId,
    string Name,
    uint IconId,
    ulong ModelMain,
    ulong ModelSub,
    uint EquipSlotCategory,
    ulong EquipSlotMask,
    ulong PrimarySlotMask,
    ulong VisualModelIdentity);

public enum PreviewSource
{
    StandardTryOn,
    GlamourPlate,
    GlamourDresser,
}

public readonly record struct TryOnSlot(
    PreviewSource Source,
    int Index,
    uint ItemId,
    uint EquipSlotCategory,
    uint GlamourId,
    uint IconId);
