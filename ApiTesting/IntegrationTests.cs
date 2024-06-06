namespace TestProject1
{
    using static RestAssured.Dsl;
    using FluentAssertions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.Hosting;
    using Dotnet.AspNetCore.Samples.WebApi;
    using System.Security.AccessControl;
    using System.Runtime.CompilerServices;
    using ApiTesting.Classes;
    using Newtonsoft.Json.Linq;
    using Snapshooter.NUnit;

    public class IntegrationTestsWebApplicationFactory: IDisposable
    {
        public void Dispose() { }

        private readonly WebApplicationFactory<Program> webApplicationFactory;

        private readonly HttpClient httpClient;

        private readonly string baseUrl;

        public IntegrationTestsWebApplicationFactory()
        {
            webApplicationFactory = new WebApplicationFactory<Program>();
            httpClient = webApplicationFactory.CreateClient();
            baseUrl = "https://localhost:7135/api/";
        }

        [TestCase("Players")]
        [TestCase("Players/1")]
        public void StatusCodeIndicatingSuccessCanBeVerifedForGetRequests(string url)
        {
            Given(httpClient)
                .When()
                .Get(baseUrl + url)
                .Then()
                .StatusCode(200);
        }

        [TestCase("Players")]
        [TestCase("Players/1")]
        public void ResponseContentTypeHeaderCanBeVerifiedForGetRequests(string url)
        {
            Given(httpClient)
            .When()
            .Get(baseUrl + url)
            .Then()
            .StatusCode(200)
            .ContentType("application/json; charset=utf-8");
        }

        [Test]
        public void JsonStringResponseBodyCanBeVerifiedForGetRequests()
        {
            JObject testPlayer = (JObject)Given(httpClient)
            .When()
            .Get(baseUrl + "Players/1")
            .Then()
            .StatusCode(200)
            .DeserializeTo(typeof(JObject));

            Snapshot.Match(testPlayer);
        }

        [Test]
        public void responsebodylengthcanbeverified()
        {
            List<object> players = (List<object>)Given(httpClient)
            .When()    
            .Get(baseUrl + "Players")
            .Then().StatusCode(200).Extract().Body("$.[0:]");

            Assert.That(players.Count, NUnit.Framework.Is.EqualTo(11));

        }

        [Test]
        public void ResponseTimeCanBeVerified()
        {
            Given(httpClient)
            .When()
            .Get(baseUrl + "Players")
            .Then()
            .ResponseTime(NHamcrest.Is.LessThan(TimeSpan.FromMilliseconds(200)));
        }

    }
}

