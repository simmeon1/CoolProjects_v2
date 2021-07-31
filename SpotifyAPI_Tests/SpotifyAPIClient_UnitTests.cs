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
    public class SpotifyAPIClient_UnitTests
    {
        private const string fakeAccessToken = "fakeAccessToken";
        private const string fakeRefreshToken = "fakeRefreshToken";
        private const string fakeTokenType = "fakeTokenType";
        private const string fakeScope = "fakeScope";
        private const int fakeExpiresIn = 3600;
        private readonly DateTime fakeDateTime1950 = new(1950, 1, 1);
        private readonly string successfulTokenResponse = @"{'access_token':'" + fakeAccessToken + @"','token_type':'" + fakeTokenType + @"','expires_in':" + fakeExpiresIn + @",'scope':'" + fakeScope + @"'}";

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
            Mock<IDateTimeProvider> dateTimeProviderMock = GetDateTimeProviderMockThatReturnsFakeDateTimeNow(fakeDateTime1950);

            SpotifyAPIClient spotifyClient = new(httpClientMock.Object, spotifyCredentialsMock.Object, tokenWorker, dateTimeProviderMock.Object);
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
            Mock<IDateTimeProvider> dateTimeProviderMock = GetDateTimeProviderMockThatReturnsFakeDateTimeNow(fakeDateTime1950);
            SpotifyAPIClient spotifyClient = new(httpClientMock.Object, spotifyCredentialsMock.Object, tokenWorker, dateTimeProviderMock.Object);

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
        public async Task GetPlaylist_PlaylistSuccessfullyReturnedAsync()
        {
            const string playlistId = "testPlaylistId";
            const string playlistName = "testPlaylistName";
            const string playlistDesc = "testPlaylistDesc";
            string playlistResponseItem = @"[{""collaborative"":false,""description"":""" + $"{playlistDesc}" + @""",""external_urls"":{""spotify"":""""},""href"":"""",""id"":""" + $"{playlistId}" + @""",""images"":[],""name"":""" + $"{playlistName}" + @""",""owner"":{""display_name"":"""",""external_urls"":{""spotify"":""""},""href"":"""",""id"":"""",""type"":"""",""uri"":""""},""primary_color"":null,""public"":false,""snapshot_id"":"""",""tracks"":{""href"":"""",""total"":0},""type"":"""",""uri"":""""}]";

            Mock<ISpotifyCredentials> spotifyCredentialsMock = GetSpotifyCredentialsMockThatReturnsAFakeRefreshToken();
            string successfulPlaylistResponse = @"{""href"":"""",""items"":" + $"{playlistResponseItem}" + @",""limit"":20,""next"":null,""offset"":0,""previous"":null,""total"":8}";
            Mock<IHttpClient> httpClientMock = GetHttpClientMockThatReturnsAGivenStatusCodeAndMessage(HttpStatusCode.OK, successfulTokenResponse, successfulPlaylistResponse);
            Mock<IDateTimeProvider> dateTimeProviderMock = GetDateTimeProviderMockThatReturnsFakeDateTimeNow(fakeDateTime1950);
            SpotifyAPIClient spotifyClient = new(httpClientMock.Object, spotifyCredentialsMock.Object, tokenWorker, dateTimeProviderMock.Object);
            List<Playlist> playlists = await spotifyClient.GetPlaylists();

            Assert.IsTrue(playlists.Count == 1);
            Assert.IsTrue(playlists.First().ID.Equals(playlistId));
            Assert.IsTrue(playlists.First().Name.Equals(playlistName));
            Assert.IsTrue(playlists.First().Description.Equals(playlistDesc));
        }

        private static Mock<IHttpClient> GetHttpClientMockThatReturnsAGivenStatusCodeAndMessage(HttpStatusCode statusCodeOfResponse, string firstResponseToReturn, string secondResponseToReturn = "")
        {
            Mock<IHttpClient> httpClientMock = new();

            HttpResponseMessage firstResponseFromTokenRequest = new(statusCode: statusCodeOfResponse);
            firstResponseFromTokenRequest.Content = new StringContent(firstResponseToReturn);

            HttpResponseMessage secondResponseFromTokenRequest = new(statusCode: statusCodeOfResponse);
            secondResponseFromTokenRequest.Content = new StringContent(secondResponseToReturn);

            httpClientMock.SetupSequence(x => x.SendRequest((It.IsAny<HttpRequestMessage>())).Result)
                .Returns(firstResponseFromTokenRequest)
                .Returns(secondResponseFromTokenRequest);
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
