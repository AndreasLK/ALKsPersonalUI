using Microsoft.AspNetCore.Components.Forms;

namespace ALKsPersonalUI.Inputs;

/// <summary>
/// Represents an image file selected by <see cref="ImageUpload"/> together with its normalized folder-relative path.
/// </summary>
/// <param name="File">The underlying Blazor browser file handle.</param>
/// <param name="RelativePath">
/// Forward-slash normalized relative path (e.g. <c>"Album/Subfolder/photo.png"</c>) when available from folder selection,
/// or <see langword="null"/> when the browser did not provide a relative path.
/// </param>
public sealed record SelectedImageFile(IBrowserFile File, string? RelativePath);

