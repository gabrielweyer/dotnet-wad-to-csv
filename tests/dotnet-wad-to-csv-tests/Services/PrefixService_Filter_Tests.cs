using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.BlobToCsv.Services;
using FluentAssertions;
using Microsoft.WindowsAzure.Storage.Blob;
using Xunit;

namespace DotNet.WadToCsv.Tests.Services
{
    public class PrefixService_Filter_Tests
    {
        [Fact]
        public void GivenSameHour_ThenSingleResultAllTheWayToHours()
        {
            // Actual

            var blobs = new List<CloudBlockBlob>
            {
                new CloudBlockBlob(new Uri("https://domain.blob.core.windows.net/container/prefix/2018/06/20/04/e872fe-35920.applicationLog.csv")),
                new CloudBlockBlob(new Uri("https://domain.blob.core.windows.net/container/prefix/2018/06/20/05/e872fe-35921.applicationLog.csv")),
                new CloudBlockBlob(new Uri("https://domain.blob.core.windows.net/container/prefix/2018/06/20/05/e872fe-35922.applicationLog.csv")),
                new CloudBlockBlob(new Uri("https://domain.blob.core.windows.net/container/prefix/2018/06/20/05/e872fe-35923.applicationLog.csv")),
                new CloudBlockBlob(new Uri("https://domain.blob.core.windows.net/container/prefix/2018/06/20/06/e872fe-35924.applicationLog.csv")),
                new CloudBlockBlob(new Uri("https://domain.blob.core.windows.net/container/prefix/2018/06/20/07/e872fe-35925.applicationLog.csv"))
            };

            var from = new DateTime(2018, 6, 20, 5, 4, 3, DateTimeKind.Utc);
            var to = new DateTime(2018, 6, 20, 6, 6, 5, DateTimeKind.Utc);

            // Act

            var actualBlobs = PrefixService.Filter(blobs, from, to, "prefix/");

            // Assert

            var expectedNames = new List<string>
            {
                "prefix/2018/06/20/05/e872fe-35921.applicationLog.csv",
                "prefix/2018/06/20/05/e872fe-35922.applicationLog.csv",
                "prefix/2018/06/20/05/e872fe-35923.applicationLog.csv",
                "prefix/2018/06/20/06/e872fe-35924.applicationLog.csv"
            };

            actualBlobs.Select(q => q.Name).Should().BeEquivalentTo(expectedNames);
        }
    }
}
