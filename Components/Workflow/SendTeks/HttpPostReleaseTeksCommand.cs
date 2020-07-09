﻿// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NL.Rijksoverheid.ExposureNotification.BackEnd.Components.EfDatabase;
using NL.Rijksoverheid.ExposureNotification.BackEnd.Components.EfDatabase.Contexts;
using NL.Rijksoverheid.ExposureNotification.BackEnd.Components.Mapping;
using NL.Rijksoverheid.ExposureNotification.BackEnd.Components.Services.AuthorisationTokens;
using Serilog;

namespace NL.Rijksoverheid.ExposureNotification.BackEnd.Components.Workflow.SendTeks
{
    public class HttpPostReleaseTeksCommand
    {
        private readonly IReleaseTeksValidator _KeyValidator;
        private readonly ISignatureValidator _SignatureValidator;
        private readonly ITekWriter _Writer;
        private readonly WorkflowDbContext _DbContextProvider;
        private readonly IJsonSerializer _JsonSerializer;

        public HttpPostReleaseTeksCommand(
            IReleaseTeksValidator keyValidator, 
            ISignatureValidator signatureValidator, 
            ITekWriter writer, 
            WorkflowDbContext dbContextProvider,
            IJsonSerializer jsonSerializer
            )
        {
            _KeyValidator = keyValidator;
            _SignatureValidator = signatureValidator;
            _Writer = writer;
            _DbContextProvider = dbContextProvider;
            _JsonSerializer = jsonSerializer;
        }

        public async Task Execute(byte[] signature, HttpRequest request)
        {
            using var reader = new StreamReader(request.Body);
            var payload = await reader.ReadToEndAsync();
            var args = _JsonSerializer.Deserialize<ReleaseTeksArgs>(payload);

            var workflow = _DbContextProvider
                .KeyReleaseWorkflowStates
                .FirstOrDefault(x => x.BucketId == args.BucketId);

            if (workflow == null)
            {
                Log.Warning("Matching workflow not found.");
                return;
            }

            if (!_KeyValidator.Validate(args, workflow))
            {
                Log.Warning("Keys args not valid.");
                return;
            }


            if (!_SignatureValidator.Valid(signature, workflow, Encoding.UTF8.GetBytes(payload)))
            {
                Log.Warning("Signature not valid.");
                return;
            }

            Log.Information("Writing start.");
            await _Writer.Execute(args);
            Log.Information("Committing.");
            _DbContextProvider.SaveAndCommit();
            Log.Information("Committed.");
        }
    }
}