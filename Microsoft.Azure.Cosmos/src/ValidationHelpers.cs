﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos
{
    using System;
    using Microsoft.Azure.Documents;

    internal static class ValidationHelpers
    {
        public static bool ValidateConsistencyLevel(Cosmos.ConsistencyLevel backendConsistency, Cosmos.ConsistencyLevel desiredConsistency)
        {
            return ValidationHelpers.ValidateConsistencyLevel((Documents.ConsistencyLevel)backendConsistency, (Documents.ConsistencyLevel)desiredConsistency);
        }

        public static bool ValidateConsistencyLevel(Documents.ConsistencyLevel backendConsistency, Documents.ConsistencyLevel desiredConsistency)
        {
            switch (backendConsistency)
            {
                case Documents.ConsistencyLevel.Strong:
                    return desiredConsistency == Documents.ConsistencyLevel.Strong ||
                        desiredConsistency == Documents.ConsistencyLevel.BoundedStaleness ||
                        desiredConsistency == Documents.ConsistencyLevel.Session ||
                        desiredConsistency == Documents.ConsistencyLevel.Eventual ||
                        desiredConsistency == Documents.ConsistencyLevel.ConsistentPrefix;

                case Documents.ConsistencyLevel.BoundedStaleness:
                    return desiredConsistency == Documents.ConsistencyLevel.BoundedStaleness ||
                        desiredConsistency == Documents.ConsistencyLevel.Session ||
                        desiredConsistency == Documents.ConsistencyLevel.Eventual ||
                        desiredConsistency == Documents.ConsistencyLevel.ConsistentPrefix;

                case Documents.ConsistencyLevel.Session:
                case Documents.ConsistencyLevel.Eventual:
                case Documents.ConsistencyLevel.ConsistentPrefix:
                    return desiredConsistency == Documents.ConsistencyLevel.Session ||
                        desiredConsistency == Documents.ConsistencyLevel.Eventual ||
                        desiredConsistency == Documents.ConsistencyLevel.ConsistentPrefix;

                default:
                    throw new ArgumentException("backendConsistency");
            }
        }
    }
}
