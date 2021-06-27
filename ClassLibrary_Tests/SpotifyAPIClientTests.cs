using ClassLibrary;
using ClassLibrary.SpotifyClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClassLibrary_Tests
{
    [TestClass]
    public class SpotifyAPIClientTests
    {
        private const string fakeAccessToken = "fakeAccessToken";
        private const string fakeRefreshToken = "fakeRefreshToken";
        private const string fakeTokenType = "fakeTokenType";
        private const int fakeExpiresIn = 3600;
        private const string fakeScope = "fakeScope";
        private DateTime fakeDateTime1950 = new(1950, 1, 1);
        private string successfulResponse = @"{""access_token"":" + $"{fakeAccessToken}" + @",""token_type"":" + $"{fakeTokenType}" + @",""expires_in"":" + $"{fakeExpiresIn}" + @",""scope"":" + $"{fakeScope}" + "}";
        private ISpotifyTokenWorker tokenWorker = new SpotifyTokenWorker();

        [TestMethod]
        public void TokensHaveTheSameData_ExpectedSuccessfulMatch()
        {
            SpotifyToken tokenOne = new(fakeAccessToken, fakeExpiresIn, fakeScope, fakeTokenType, fakeDateTime1950);
            SpotifyToken tokenTwo = new(fakeAccessToken, fakeExpiresIn, fakeScope, fakeTokenType, fakeDateTime1950);
            Assert.IsTrue(tokenWorker.TokensHaveTheSameData(tokenOne, tokenTwo));
        }

        [TestMethod]
        public void TokensHaveTheSameData_ExpectedUnsuccessfulMatch()
        {
            SpotifyToken tokenOne = new(fakeAccessToken, fakeExpiresIn, fakeScope, fakeTokenType, fakeDateTime1950);
            SpotifyToken tokenTwo = new("asd", 0, "asd", "asd", fakeDateTime1950.AddSeconds(1));
            Assert.IsFalse(tokenWorker.TokensHaveTheSameData(tokenOne, tokenTwo));
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void TokenIsStillValid_TokenIsFresh(int secondsToAddToDateTimeNowAlongWithExpiresInSeconds)
        {
            SpotifyToken token = new(fakeAccessToken, fakeExpiresIn, fakeScope, fakeTokenType, fakeDateTime1950);
            Mock<IDateTimeProvider> dateTimeProviderMock = GetDateTimeProviderMockThatReturnsFakeDateTimeNow(fakeDateTime1950.AddSeconds(fakeExpiresIn + secondsToAddToDateTimeNowAlongWithExpiresInSeconds));
            Assert.IsTrue(tokenWorker.TokenIsStillValid(token, dateTimeProviderMock.Object));
        }

        [TestMethod]
        public void TokenIsStillValid_TokenIsOutdated()
        {
            SpotifyToken token = new(fakeAccessToken, fakeExpiresIn, fakeScope, fakeTokenType, fakeDateTime1950);
            Mock<IDateTimeProvider> dateTimeProviderMock = GetDateTimeProviderMockThatReturnsFakeDateTimeNow(fakeDateTime1950.AddSeconds(fakeExpiresIn + 1));
            Assert.IsFalse(tokenWorker.TokenIsStillValid(token, dateTimeProviderMock.Object));
        }
        
        [TestMethod]
        public void TokenIsStillValid_TokenIsNull()
        {
            Mock<IDateTimeProvider> dateTimeProviderMock = GetDateTimeProviderMockThatReturnsFakeDateTimeNow(fakeDateTime1950);
            Assert.IsFalse(tokenWorker.TokenIsStillValid(null, dateTimeProviderMock.Object));
        }

        [TestMethod]
        public async Task GetAndSetNewAccessToken_ReturnsSuccessfulResponseAndToken()
        {
            Mock<ISpotifyCredentials> spotifyCredentialsMock = GetSpotifyCredentialsMockThatReturnsAFakeRefreshToken();
            Mock<IHttpClient> httpClientMock = GetHttpClientMockThatReturnsAGivenStatusCodeAndMessage(HttpStatusCode.OK, successfulResponse);
            Mock<IJsonParser> jsonParserMock = GetJsonParserMockThatReturnsFakeTokenData();
            Mock<IDateTimeProvider> dateTimeProviderMock = GetDateTimeProviderMockThatReturnsFakeDateTimeNow(fakeDateTime1950);

            SpotifyAPIClient spotifyClient = new(spotifyCredentialsMock.Object, httpClientMock.Object, jsonParserMock.Object, dateTimeProviderMock.Object, tokenWorker);
            await spotifyClient.GetAndSetNewAccessToken();

            SpotifyToken tokenToCompare = new(fakeAccessToken, fakeExpiresIn, fakeScope, fakeTokenType, fakeDateTime1950);
            Assert.IsTrue(spotifyClient.ConfirmTokenAgainstCurrentToken(tokenToCompare));
        }

        private Mock<IDateTimeProvider> GetDateTimeProviderMockThatReturnsFakeDateTimeNow(DateTime dateToReturn)
        {
            Mock<IDateTimeProvider> dateTimeProviderMock = new();
            dateTimeProviderMock.Setup(x => x.GetDateTimeNow()).Returns(dateToReturn);
            return dateTimeProviderMock;
        }

        [TestMethod]
        public async Task GetAndSetNewAccessToken_ThrowsExceptionForBadRequest()
        {
            string failResponse = "failResponse";
            Mock<ISpotifyCredentials> spotifyCredentialsMock = GetSpotifyCredentialsMockThatReturnsAFakeRefreshToken();
            HttpStatusCode responseCode = HttpStatusCode.BadRequest;
            Mock<IHttpClient> httpClientMock = GetHttpClientMockThatReturnsAGivenStatusCodeAndMessage(responseCode, failResponse);
            Mock<IJsonParser> jsonParserMock = new();
            Mock<IDateTimeProvider> dateTimeProviderMock = GetDateTimeProviderMockThatReturnsFakeDateTimeNow(fakeDateTime1950);
            SpotifyAPIClient spotifyClient = new(spotifyCredentialsMock.Object, httpClientMock.Object, jsonParserMock.Object, dateTimeProviderMock.Object, tokenWorker);

            try
            {
                await spotifyClient.GetAndSetNewAccessToken();
                Assert.Fail("Expected token request to fail.");
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Equals($"The token request was not successful.{Environment.NewLine}The details are as follows:{Environment.NewLine}" +
                    $"Status code: {responseCode}{Environment.NewLine}" +
                    $"Error details: {failResponse}"));
            }
        }

        [TestMethod]
        public async Task GetPlaylist_PlaylistSuccessfullyReturned()
        {
            string playlistId = "testPlaylistId";
            string playlistName = "testPlaylistName";
            string playlistDesc = "testPlaylistDesc";
            string successfulPlaylistResponse = @"{""href"":"""",""items"":[{""collaborative"":false,""description"":""" + $"{playlistDesc}" + @""",""external_urls"":{""spotify"":""""},""href"":"""",""id"":""" + $"{playlistId}" + @""",""images"":[],""name"":""" + $"{playlistName}" + @""",""owner"":{""display_name"":"""",""external_urls"":{""spotify"":""""},""href"":"""",""id"":"""",""type"":"""",""uri"":""""},""primary_color"":null,""public"":false,""snapshot_id"":"""",""tracks"":{""href"":"""",""total"":0},""type"":"""",""uri"":""""}],""limit"":20,""next"":null,""offset"":0,""previous"":null,""total"":8}";

            Mock<ISpotifyCredentials> spotifyCredentialsMock = GetSpotifyCredentialsMockThatReturnsAFakeRefreshToken();
            Mock<IHttpClient> httpClientMock = GetHttpClientMockThatReturnsAGivenStatusCodeAndMessage(HttpStatusCode.OK, successfulPlaylistResponse);
            Mock<IJsonParser> jsonParserMock = GetJsonParserMockThatReturnsFakeTokenData();

            Mock<IDateTimeProvider> dateTimeProviderMock = GetDateTimeProviderMockThatReturnsFakeDateTimeNow(fakeDateTime1950);
            SpotifyAPIClient spotifyClient = new(spotifyCredentialsMock.Object, httpClientMock.Object, jsonParserMock.Object, dateTimeProviderMock.Object, tokenWorker);
            List<Playlist> playlists = await spotifyClient.GetPlaylists();

            Assert.IsTrue(playlists.Count == 1);
            Assert.IsTrue(playlists.First().ID.Equals(playlistId));
            Assert.IsTrue(playlists.First().Name.Equals(playlistName));
            Assert.IsTrue(playlists.First().Description.Equals(playlistDesc));
        }

        private static Mock<IJsonParser> GetJsonParserMockThatReturnsFakeTokenData()
        {
            Mock<IJsonParser> jsonParserMock = new();
            jsonParserMock.Setup(x => x.GetPropertyValue<string>("access_token")).Returns(fakeAccessToken);
            jsonParserMock.Setup(x => x.GetPropertyValue<string>("token_type")).Returns(fakeTokenType);
            jsonParserMock.Setup(x => x.GetPropertyValue<int>("expires_in")).Returns(fakeExpiresIn);
            jsonParserMock.Setup(x => x.GetPropertyValue<string>("scope")).Returns(fakeScope);
            return jsonParserMock;
        }

        private static Mock<IHttpClient> GetHttpClientMockThatReturnsAGivenStatusCodeAndMessage(HttpStatusCode statusCodeOfResponse, string responseToReturn)
        {
            Mock<IHttpClient> httpClientMock = new();
            HttpResponseMessage responseFromTokenRequest = new(statusCode: statusCodeOfResponse);
            responseFromTokenRequest.Content = new StringContent(responseToReturn);
            httpClientMock.Setup(x => x.SendRequest((It.IsAny<HttpRequestMessage>())).Result).Returns(responseFromTokenRequest);
            return httpClientMock;
        }

        private static Mock<ISpotifyCredentials> GetSpotifyCredentialsMockThatReturnsAFakeRefreshToken()
        {
            Mock<ISpotifyCredentials> spotifyCredentialsMock = new();
            spotifyCredentialsMock.Setup(x => x.GetRefreshToken()).Returns(fakeRefreshToken);
            return spotifyCredentialsMock;
        }
    }
}
