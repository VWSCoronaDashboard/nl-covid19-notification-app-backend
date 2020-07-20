﻿// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NL.Rijksoverheid.ExposureNotification.BackEnd.Components.ExposureKeySets;

namespace NL.Rijksoverheid.ExposureNotification.BackEnd.Components.EfDatabase.Configuration.Content
{
    public class ExposureKeySetContentEtc : IEntityTypeConfiguration<ExposureKeySetContentEntity>
    {
        public void Configure(EntityTypeBuilder<ExposureKeySetContentEntity> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.ToTable("ExposureKeySetContent");
            builder.Property(u => u.Id).UseHiLo();
            builder.Property(u => u.PublishingId).HasMaxLength(64);
            builder.Property(x => x.CreatingJobName).IsRequired(false);
            builder.Property(x => x.CreatingJobQualifier).IsRequired(false);
        }
    }
}