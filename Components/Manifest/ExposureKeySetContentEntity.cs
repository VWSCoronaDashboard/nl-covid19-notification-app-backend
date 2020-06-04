// Copyright � 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.ComponentModel.DataAnnotations.Schema;
using NL.Rijksoverheid.ExposureNotification.BackEnd.Components.RivmAdvice;

namespace NL.Rijksoverheid.ExposureNotification.BackEnd.Components.Manifest
{
    /// <summary>
    /// TODO incomplete.
    /// </summary>
    [Table("ExposureKeySetContent")]
    public class ExposureKeySetContentEntity : ContentEntity
    {
        /// <summary>
        /// Metadata - Job Id.
        /// </summary>
        public string CreatingJobName { get; set; }
        /// <summary>
        /// Metadata - Job Id.
        /// </summary>
        public int CreatingJobQualifier { get; set; }
    }
}