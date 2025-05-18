// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL;

#pragma warning disable CA1815

/// <summary>
///     Parameters for creating a <see cref="Window" />.
/// </summary>
[PublicAPI]
public sealed class WindowOptions : BaseOptions
{
    internal CString TitleCString;

    private string _title = string.Empty;

    /// <summary>
    ///     Gets or sets the text title of the window. Default is <see cref="string.Empty" />.
    /// </summary>
    public string Title
    {
        get => _title;
        set
        {
            TitleCString = Allocator.AllocateCString(value);
            _title = value;
        }
    }

    /// <summary>
    ///     Gets or sets the width of the window. Default is <c>800</c>.
    /// </summary>
    public int Width { get; set; } = 800;

    /// <summary>
    ///     Gets or sets the height of the window. Default is <c>600</c>.
    /// </summary>
    public int Height { get; set; } = 600;

    /// <summary>
    ///     Gets or sets a value indicating whether the window is resizable. Default is <c>false</c>.
    /// </summary>
    public bool IsResizable { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether the window starts maximized. Default is <c>false</c>.
    /// </summary>
    public bool IsStartingMaximized { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the window's surface should be created and managed. Default is
    ///     <c>false</c>.
    /// </summary>
    public bool IsEnabledCreateSurface { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the window's renderer should be created and managed. Default is
    ///     <c>false</c>.
    /// </summary>
    public bool IsEnabledCreateRenderer { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowOptions" /> class.
    /// </summary>
    /// <param name="allocator">
    ///     The allocator to use for temporary interoperability allocations. If <c>null</c>, a
    ///     <see cref="ArenaNativeAllocator" /> is used with a capacity of 1 KB.
    /// </param>
    public WindowOptions(INativeAllocator? allocator = null)
        : base(allocator)
    {
    }

    /// <inheritdoc />
    protected override void OnReset()
    {
        Title = string.Empty;
        Width = 800;
        Height = 600;
        IsResizable = true;
        IsStartingMaximized = false;
        IsEnabledCreateSurface = false;
    }
}
