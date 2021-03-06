﻿//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.FeedProcessing
{
    internal sealed class ChangeFeedObserverFactoryCore<T>: ChangeFeedObserverFactory<T>
    {
        private readonly Func<IReadOnlyList<T>, CancellationToken, Task> onChanges;

        public ChangeFeedObserverFactoryCore(Func<IReadOnlyList<T>, CancellationToken, Task> onChanges)
        {
            this.onChanges = onChanges;
        }

        public override ChangeFeedObserver<T> CreateObserver()
        {
            return new ChangeFeedObserverBase<T>(this.onChanges);
        }
    }
}