// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace NL.Rijksoverheid.ExposureNotification.IccPortalAuthorizer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://*:5004", "https://*:5005"); //TODO WHY IS THIS NOT IN THE CONFIG!
                    webBuilder.UseStartup<Startup>();
                });
    }
}