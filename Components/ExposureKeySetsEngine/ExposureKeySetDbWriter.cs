﻿// Copyright © 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Linq;
using EFCore.BulkExtensions;
using NL.Rijksoverheid.ExposureNotification.BackEnd.Components.EfDatabase;
using NL.Rijksoverheid.ExposureNotification.BackEnd.Components.EfDatabase.Contexts;
using NL.Rijksoverheid.ExposureNotification.BackEnd.Components.ExposureKeySets;
using NL.Rijksoverheid.ExposureNotification.BackEnd.Components.Manifest;
using NL.Rijksoverheid.ExposureNotification.BackEnd.Components.Services;

namespace NL.Rijksoverheid.ExposureNotification.BackEnd.Components.ExposureKeySetsEngine
{

    /// <summary>
    /// Write to a Db instead of a queue
    /// </summary>
    public class ExposureKeySetDbWriter : IExposureKeySetWriter
    {
        public ExposureKeySetDbWriter(IDbContextProvider<ExposureContentDbContext> dbContext, IPublishingId publishingId)
        {
            _DbContext = dbContext;
            _PublishingId = publishingId;
        }

        private readonly IDbContextProvider<ExposureContentDbContext> _DbContext;
        private readonly IPublishingId _PublishingId;

        public void Write(ExposureKeySetEntity[] things)
        {
            var entities = things.Select(x => new ExposureKeySetContentEntity
            {
                Content = x.GaenContent,
                CreatingJobName = x.CreatingJobName,
                CreatingJobQualifier = x.CreatingJobQualifier,
                Region = x.Region,
                Release = x.Created,
            }).ToList();

            foreach (var i in entities)
            {
                i.PublishingId = _PublishingId.Create(i);
            }

            using (_DbContext.BeginTransaction())
            {
                _DbContext.Current.BulkInsertAsync(entities);
                _DbContext.SaveAndCommit();
            }
        }
    }
}