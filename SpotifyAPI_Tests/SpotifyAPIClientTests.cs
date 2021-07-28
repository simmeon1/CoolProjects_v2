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
        private const string playlistId = "testPlaylistId";
        private const string playlistName = "testPlaylistName";
        private const string playlistDesc = "testPlaylistDesc";
        private string successfulTokenResponse = @"{""access_token"":" + $"{fakeAccessToken}" + @",""token_type"":" + $"{fakeTokenType}" + @",""expires_in"":" + $"{fakeExpiresIn}" + @",""scope"":" + $"{fakeScope}" + "}";

        private string playlistResponseItem = @"[{""collaborative"":false,""description"":""" + $"{playlistDesc}" + @""",""external_urls"":{""spotify"":""""},""href"":"""",""id"":""" + $"{playlistId}" + @""",""images"":[],""name"":""" + $"{playlistName}" + @""",""owner"":{""display_name"":"""",""external_urls"":{""spotify"":""""},""href"":"""",""id"":"""",""type"":"""",""uri"":""""},""primary_color"":null,""public"":false,""snapshot_id"":"""",""tracks"":{""href"":"""",""total"":0},""type"":"""",""uri"":""""}]";

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
        public void CreateTokenObjectFromTokenInterface_Success()
        {
            ISpotifyToken token = tokenWorker.CreateTokenObject(fakeAccessToken, fakeExpiresIn, fakeScope, fakeTokenType, fakeDateTime1950);
            Assert.IsTrue(token.GetAccessToken().Equals(fakeAccessToken));
            Assert.IsTrue(token.GetDateTimeCreated().Value.CompareTo(fakeDateTime1950) == 0);
            Assert.IsTrue(token.GetExpiresIn() == fakeExpiresIn);
            Assert.IsTrue(token.GetScope().Equals(fakeScope));
            Assert.IsTrue(token.GetTokenType().Equals(fakeTokenType));
        }

        [TestMethod]
        public async Task GetAndSetNewAccessToken_ReturnsSuccessfulResponseAndToken()
        {
            Mock<ISpotifyCredentials> spotifyCredentialsMock = GetSpotifyCredentialsMockThatReturnsAFakeRefreshToken();
            Mock<IHttpClient> httpClientMock = GetHttpClientMockThatReturnsAGivenStatusCodeAndMessage(HttpStatusCode.OK, successfulTokenResponse);
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
            Mock<ISpotifyCredentials> spotifyCredentialsMock = GetSpotifyCredentialsMockThatReturnsAFakeRefreshToken();
            string successfulPlaylistResponse = @"{""href"":"""",""items"":" + $"{playlistResponseItem}" + @",""limit"":20,""next"":null,""offset"":0,""previous"":null,""total"":8}";
            Mock<IHttpClient> httpClientMock = GetHttpClientMockThatReturnsAGivenStatusCodeAndMessage(HttpStatusCode.OK, successfulPlaylistResponse);
            Mock<IJsonParser> jsonParserMock = GetJsonParserMockThatReturnsFakeTokenData();
            jsonParserMock.Setup(x => x.GetArrayJsons(It.IsAny<string>(), "items")).Returns(new List<string>() { @"{""collaborative"":false,""description"":""" + $"{playlistDesc}" + @""",""external_urls"":{""spotify"":""""},""href"":"""",""id"":""" + $"{playlistId}" + @""",""images"":[],""name"":""" + $"{playlistName}" + @""",""owner"":{""display_name"":"""",""external_urls"":{""spotify"":""""},""href"":"""",""id"":"""",""type"":"""",""uri"":""""},""primary_color"":null,""public"":false,""snapshot_id"":"""",""tracks"":{""href"":"""",""total"":0},""type"":"""",""uri"":""""}" });
            jsonParserMock.Setup(x => x.GetPropertyValue<string>(It.IsAny<string>(), "id")).Returns(playlistId);
            jsonParserMock.Setup(x => x.GetPropertyValue<string>(It.IsAny<string>(), "name")).Returns(playlistName);
            jsonParserMock.Setup(x => x.GetPropertyValue<string>(It.IsAny<string>(), "description")).Returns(playlistDesc);

            Mock<IDateTimeProvider> dateTimeProviderMock = GetDateTimeProviderMockThatReturnsFakeDateTimeNow(fakeDateTime1950);
            SpotifyAPIClient spotifyClient = new(spotifyCredentialsMock.Object, httpClientMock.Object, jsonParserMock.Object, dateTimeProviderMock.Object, tokenWorker);
            List<Playlist> playlists = await spotifyClient.GetPlaylists();

            Assert.IsTrue(playlists.Count == 1);
            Assert.IsTrue(playlists.First().ID.Equals(playlistId));
            Assert.IsTrue(playlists.First().Name.Equals(playlistName));
            Assert.IsTrue(playlists.First().Description.Equals(playlistDesc));
        }

        [TestMethod]
        public void GetPlaylistsFromJsonResponse()
        {
            JsonParser jsonParser = new();
            SpotifyAPIClient spotifyClient = new(null, null, jsonParser, null, null);
            string successfulPlaylistResponse = @"{""href"":"""",""items"":" + $"{playlistResponseItem}" + @",""limit"":20,""next"":null,""offset"":0,""previous"":null,""total"":8}";
            List<Playlist> playlists = spotifyClient.GetPlaylistsFromJson(successfulPlaylistResponse);

            Assert.IsTrue(playlists.Count == 1);
            Assert.IsTrue(playlists.First().ID.Equals(playlistId));
            Assert.IsTrue(playlists.First().Name.Equals(playlistName));
            Assert.IsTrue(playlists.First().Description.Equals(playlistDesc));
        }

        [TestMethod]
        public void JsonParser_GetArrayJsons()
        {
            JsonParser jsonParser = new();
            const string json = @"{""items"":[{""name"":1},{""name"":2}]}";
            List<string> result = jsonParser.GetArrayJsons(json, "items");
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(jsonParser.GetPropertyValue<int>(result[0], "name") == 1);
            Assert.IsTrue(jsonParser.GetPropertyValue<int>(result[1], "name") == 2);
        }

        private static Mock<IJsonParser> GetJsonParserMockThatReturnsFakeTokenData()
        {
            Mock<IJsonParser> jsonParserMock = new();
            jsonParserMock.Setup(x => x.GetPropertyValue<string>(It.IsAny<string>(), "access_token")).Returns(fakeAccessToken);
            jsonParserMock.Setup(x => x.GetPropertyValue<string>(It.IsAny<string>(), "token_type")).Returns(fakeTokenType);
            jsonParserMock.Setup(x => x.GetPropertyValue<int>(It.IsAny<string>(), "expires_in")).Returns(fakeExpiresIn);
            jsonParserMock.Setup(x => x.GetPropertyValue<string>(It.IsAny<string>(), "scope")).Returns(fakeScope);
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
