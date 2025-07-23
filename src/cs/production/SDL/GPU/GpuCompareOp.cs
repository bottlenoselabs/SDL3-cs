// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace bottlenoselabs.SDL;

/// <summary>
///     Defines a boolean comparison operator for depth-testing, stencil-testing, and sampling operations.
/// </summary>
[PublicAPI]
public enum GpuCompareOp
{
    /// <summary>
    ///     Invalid comparision operator.
    /// </summary>
    Invalid = 0,

    /// <summary>
    ///     The comparison always evaluates <c>false</c>.
    /// </summary>
    Never = 1,

    /// <summary>
    ///     The comparison evaluates <c>reference &lt; test</c>.
    /// </summary>
    Less = 2,

    /// <summary>
    ///     The comparison evaluates <c>reference == test</c>.
    /// </summary>
    Equal = 3,

    /// <summary>
    ///     The comparison evaluates <c>reference &lt;= test</c>.
    /// </summary>
    LessOrEqual = 4,

    /// <summary>
    ///     The comparison evaluates <c>reference &gt; test</c>.
    /// </summary>
    Greater = 5,

    /// <summary>
    ///     The comparison evaluates <c>reference != test</c>.
    /// </summary>
    NotEqual = 6,

    /// <summary>
    ///     The comparison evaluates <c>reference &gt;= test</c>.
    /// </summary>
    GreaterOrEqual = 7,

    /// <summary>
    ///     The comparison always evaluates <c>true</c>.
    /// </summary>
    Always = 8
}
