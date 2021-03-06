﻿// Copyright  De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

namespace NL.Rijksoverheid.ExposureNotification.BackEnd.Components.Workflow.SendTeks
{
    public interface ITekValidPeriodFilter
    {
        /// <summary>
        /// Assumes future TEKs already killed.
        /// Filters out ones too old that would not get into the EKS
        /// </summary>
        FilterResult<Tek> Execute(Tek[] values);
    }
}