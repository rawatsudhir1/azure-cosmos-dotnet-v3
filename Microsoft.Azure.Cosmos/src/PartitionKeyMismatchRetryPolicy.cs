﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Common;
    using Microsoft.Azure.Cosmos.Internal;
    using Microsoft.Azure.Documents;

    internal sealed class PartitionKeyMismatchRetryPolicy : IDocumentClientRetryPolicy
    {
        private readonly CollectionCache clientCollectionCache;
        private readonly IDocumentClientRetryPolicy nextRetryPolicy;

        private const int MaxRetries = 1;

        private int retriesAttempted;

        public PartitionKeyMismatchRetryPolicy(
            CollectionCache clientCollectionCache,
            IDocumentClientRetryPolicy nextRetryPolicy)
        {
            if (clientCollectionCache == null)
            {
                throw new ArgumentNullException("clientCollectionCache");
            }

            if (nextRetryPolicy == null)
            {
                throw new ArgumentNullException("nextRetryPolicy");
            }

            this.clientCollectionCache = clientCollectionCache;
            this.nextRetryPolicy = nextRetryPolicy;
        }

        /// <summary> 
        /// Should the caller retry the operation.
        /// </summary>
        /// <param name="exception">Exception that occured when the operation was tried</param>
        /// <param name="cancellationToken"></param>
        /// <returns>True indicates caller should retry, False otherwise</returns>
        public Task<ShouldRetryResult> ShouldRetryAsync(
            Exception exception, 
            CancellationToken cancellationToken)
        {
            DocumentClientException clientException = exception as DocumentClientException;

            return this.ShouldRetryAsyncInternal(clientException?.StatusCode,
                clientException?.GetSubStatus(),
                clientException?.ResourceAddress,
                () => this.nextRetryPolicy.ShouldRetryAsync(exception, cancellationToken));
        }

        /// <summary> 
        /// Should the caller retry the operation.
        /// </summary>
        /// <param name="cosmosResponseMessage"><see cref="CosmosResponseMessage"/> in return of the request</param>
        /// <param name="cancellationToken"></param>
        /// <returns>True indicates caller should retry, False otherwise</returns>
        public Task<ShouldRetryResult> ShouldRetryAsync(
            CosmosResponseMessage cosmosResponseMessage, 
            CancellationToken cancellationToken)
        {
            return this.ShouldRetryAsyncInternal(cosmosResponseMessage?.StatusCode,
                cosmosResponseMessage?.Headers.SubStatusCode,
                cosmosResponseMessage?.GetResourceAddress(),
                () => this.nextRetryPolicy.ShouldRetryAsync(cosmosResponseMessage, cancellationToken));
        }

        /// <summary>
        /// Method that is called before a request is sent to allow the retry policy implementation
        /// to modify the state of the request.
        /// </summary>
        /// <param name="request">The request being sent to the service.</param>
        public void OnBeforeSendRequest(DocumentServiceRequest request)
        {
            this.nextRetryPolicy.OnBeforeSendRequest(request);
        }

        private Task<ShouldRetryResult> ShouldRetryAsyncInternal(
            HttpStatusCode? statusCode, 
            SubStatusCodes? subStatusCode, 
            string resourceIdOrFullName, 
            Func<Task<ShouldRetryResult>> continueIfNotHandled)
        {
            if (!statusCode.HasValue
                && !subStatusCode.HasValue)
            {
                return continueIfNotHandled();
            }

            if (statusCode == HttpStatusCode.BadRequest
                && subStatusCode == SubStatusCodes.PartitionKeyMismatch
                && this.retriesAttempted < MaxRetries)
            {
                Debug.Assert(resourceIdOrFullName != null);

                if (!string.IsNullOrEmpty(resourceIdOrFullName))
                {
                    this.clientCollectionCache.Refresh(resourceIdOrFullName);
                }

                this.retriesAttempted++;

                return Task.FromResult(ShouldRetryResult.RetryAfter(TimeSpan.Zero));
            }

            return continueIfNotHandled();
        }
    }
}
