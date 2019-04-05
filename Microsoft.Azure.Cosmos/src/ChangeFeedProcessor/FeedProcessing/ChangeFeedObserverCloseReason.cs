﻿//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

namespace Microsoft.Azure.Cosmos.ChangeFeed.FeedProcessing
{
    /// <summary>
    /// The reason for the <see cref="ChangeFeedObserver{T}"/> to close.
    /// </summary>
    internal enum ChangeFeedObserverCloseReason
    {
        /// <summary>
        /// Unknown failure. This should never be sent to observers.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The ChangeFeedEventHost is shutting down.
        /// </summary>
        Shutdown,

        /// <summary>
        /// The resource, such as database or collection was removed.
        /// </summary>
        ResourceGone,

        /// <summary>
        /// Lease was lost due to expiration or load-balancing.
        /// </summary>
        LeaseLost,

        /// <summary>
        /// IChangeFeedObserver threw an exception.
        /// </summary>
        ObserverError,

        /// <summary>
        /// The lease is gone. This can be due to partition split.
        /// </summary>
        LeaseGone,
    }
}