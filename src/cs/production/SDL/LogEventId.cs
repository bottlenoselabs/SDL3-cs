// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL;

internal static class LogEventId
{
    public const int NativeFunctionFailed = 0;

    public const int PoolDisposed = 10;
    public const int PoolObjectCreated = 11;
    public const int PoolObjectLeased = 12;
    public const int PoolObjectReturned = 13;
    public const int PoolObjectLeaked = 14;
}
