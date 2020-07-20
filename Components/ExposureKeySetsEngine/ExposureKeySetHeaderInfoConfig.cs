﻿// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.Configuration;
using NL.Rijksoverheid.ExposureNotification.BackEnd.Components.Configuration;

namespace NL.Rijksoverheid.ExposureNotification.BackEnd.Components.ExposureKeySetsEngine
{
    public class ExposureKeySetHeaderInfoConfig : AppSettingsReader, IExposureKeySetHeaderInfoConfig
    {
        public ExposureKeySetHeaderInfoConfig(IConfiguration config, string? prefix = "ExposureKeySets:SignatureHeader") : base(config, prefix) { }
        public string AppBundleId => GetConfigValue(nameof(VerificationKeyId), "nl.rijksoverheid.en");
        public string VerificationKeyId => GetConfigValue(nameof(VerificationKeyId), "204");
        public string VerificationKeyVersion => GetConfigValue(nameof(VerificationKeyVersion), "v1");
    }
}