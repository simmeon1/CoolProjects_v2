using ClassLibrary;
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
        private ISpotifyTokenWorker TokenWorker { get; set; } = new SpotifyTokenWorker();
        private SpotifyToken FakeToken { get; set; }
        private string SuccessfulTokenResponse { get; set; }

        [TestInitialize]
        public void TestInitialise()
        {
            FakeToken = new SpotifyToken("fakeAccessToken", 3600, "fakeScope", "fakeTokenType", new DateTime(1950, 1, 1));
            SuccessfulTokenResponse = @"{'access_token':'" + "fakeAccessToken" + @"','token_type':'" + "fakeTokenType" + @"','expires_in':" + 3600 + @",'scope':'" + "fakeScope" + @"'}";
        }

        [TestMethod]
        public void TokensHaveTheSameData_ExpectedSuccessfulMatch()
        {
            SpotifyToken tokenOne = FakeToken;
            SpotifyToken tokenTwo = FakeToken;
            Assert.IsTrue(TokenWorker.TokensHaveTheSameData(tokenOne, tokenTwo));
        }

        [TestMethod]
        public void TokensHaveTheSameData_ExpectedUnsuccessfulMatch()
        {
            SpotifyToken tokenOne = FakeToken;
            SpotifyToken tokenTwo = new("asd", 0, "asd", "asd", FakeToken.GetDateTimeCreated().Value.AddSeconds(1));
            Assert.IsFalse(TokenWorker.TokensHaveTheSameData(tokenOne, tokenTwo));
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void TokenIsStillValid_TokenIsFresh(int secondsToAddToDateTimeNowAlongWithExpiresInSeconds)
        {
            SpotifyToken token = FakeToken;
            Mock<IDateTimeProvider> dateTimeProviderMock = GetDateTimeProviderMockThatReturnsFakeDateTimeNow(FakeToken.GetDateTimeCreated().Value.AddSeconds(FakeToken.GetExpiresIn() + secondsToAddToDateTimeNowAlongWithExpiresInSeconds));
            Assert.IsTrue(TokenWorker.TokenIsStillValid(token, dateTimeProviderMock.Object));
        }

        [TestMethod]
        public void TokenIsStillValid_TokenIsOutdated()
        {
            SpotifyToken token = FakeToken;
            Mock<IDateTimeProvider> dateTimeProviderMock = GetDateTimeProviderMockThatReturnsFakeDateTimeNow(FakeToken.GetDateTimeCreated().Value.AddSeconds(FakeToken.GetExpiresIn() + 1));
            Assert.IsFalse(TokenWorker.TokenIsStillValid(token, dateTimeProviderMock.Object));
        }

        [TestMethod]
        public void TokenIsStillValid_TokenIsNull()
        {
            Mock<IDateTimeProvider> dateTimeProviderMock = GetDateTimeProviderMockThatReturnsFakeDateTimeNow(FakeToken.GetDateTimeCreated().Value);
            Assert.IsFalse(TokenWorker.TokenIsStillValid(null, dateTimeProviderMock.Object));
        }

        [TestMethod]
        public void CreateTokenObjectFromTokenInterface_Success()
        {
            SpotifyToken fakeToken = FakeToken;
            ISpotifyToken token = TokenWorker.CreateTokenObject(fakeToken.GetAccessToken(), fakeToken.GetExpiresIn(), fakeToken.GetScope(), fakeToken.GetTokenType(), fakeToken.GetDateTimeCreated());
            Assert.IsTrue(token.GetAccessToken().Equals(fakeToken.GetAccessToken()));
            Assert.IsTrue(token.GetExpiresIn() == fakeToken.GetExpiresIn());
            Assert.IsTrue(token.GetScope().Equals(fakeToken.GetScope()));
            Assert.IsTrue(token.GetTokenType().Equals(fakeToken.GetTokenType()));
            Assert.IsTrue(token.GetDateTimeCreated().Value.CompareTo(fakeToken.GetDateTimeCreated()) == 0);
        }

        [TestMethod]
        public async Task GetAndSetNewAccessToken_ReturnsSuccessfulResponseAndToken()
        {
            Mock<ISpotifyCredentials> spotifyCredentialsMock = GetSpotifyCredentialsMockThatReturnsAFakeRefreshToken();
            Mock<IHttpClient> httpClientMock = new();
            httpClientMock.Setup(x => x.SendRequest(It.IsAny<HttpRequestMessage>()).Result).Returns(GetResponseWithCodeAndMessage(HttpStatusCode.OK, SuccessfulTokenResponse));
            Mock<IDateTimeProvider> dateTimeProviderMock = GetDateTimeProviderMockThatReturnsFakeDateTimeNow(FakeToken.GetDateTimeCreated().Value);

            SpotifyAPIClient spotifyClient = new(httpClientMock.Object, spotifyCredentialsMock.Object, TokenWorker, dateTimeProviderMock.Object);
            await spotifyClient.GetAndSetNewAccessToken();

            SpotifyToken tokenToCompare = FakeToken;
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
            Mock<IHttpClient> httpClientMock = new();
            httpClientMock.Setup(x => x.SendRequest(It.IsAny<HttpRequestMessage>()).Result).Returns(GetResponseWithCodeAndMessage(responseCode, failResponse));
            Mock<IDateTimeProvider> dateTimeProviderMock = GetDateTimeProviderMockThatReturnsFakeDateTimeNow(FakeToken.GetDateTimeCreated().Value);
            SpotifyAPIClient spotifyClient = new(httpClientMock.Object, spotifyCredentialsMock.Object, TokenWorker, dateTimeProviderMock.Object);

            try
            {
                await spotifyClient.GetAndSetNewAccessToken();
                Assert.Fail("Expected token request to fail.");
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Equals($"The request was not successful.{Environment.NewLine}The details are as follows:{Environment.NewLine}" +
                    $"Status code: {responseCode}{Environment.NewLine}" +
                    $"Error details: {failResponse}"));
            }
        }

        [TestMethod]
        public async Task GetPlaylist_PlaylistSuccessfullyReturned()
        {
            const string playlistId = "testPlaylistId";
            const string playlistName = "testPlaylistName";
            const string playlistDesc = "testPlaylistDesc";
            string playlistResponseItem = @"{'items':[{'description':'" + playlistDesc + @"','id':'" + playlistId + @"','name':'" + playlistName + @"'}]}";

            Mock<ISpotifyCredentials> spotifyCredentialsMock = GetSpotifyCredentialsMockThatReturnsAFakeRefreshToken();
            Mock<IHttpClient> httpClientMock = new();
            httpClientMock.SetupSequence(x => x.SendRequest(It.IsAny<HttpRequestMessage>()).Result)
                .Returns(GetResponseWithCodeAndMessage(HttpStatusCode.OK, SuccessfulTokenResponse))
                .Returns(GetResponseWithCodeAndMessage(HttpStatusCode.OK, playlistResponseItem));
            Mock<IDateTimeProvider> dateTimeProviderMock = GetDateTimeProviderMockThatReturnsFakeDateTimeNow(FakeToken.GetDateTimeCreated().Value);
            SpotifyAPIClient spotifyClient = new(httpClientMock.Object, spotifyCredentialsMock.Object, TokenWorker, dateTimeProviderMock.Object);
            List<Playlist> playlists = await spotifyClient.GetPlaylists();

            Assert.IsTrue(playlists.Count == 1);
            Assert.IsTrue(playlists.First().ID.Equals(playlistId));
            Assert.IsTrue(playlists.First().Name.Equals(playlistName));
            Assert.IsTrue(playlists.First().Description.Equals(playlistDesc));
        }

        private static HttpResponseMessage GetResponseWithCodeAndMessage(HttpStatusCode statusCodeOfResponse, string responseText)
        {
            HttpResponseMessage response = new(statusCode: statusCodeOfResponse);
            response.Content = new StringContent(responseText);
            return response;
        }

        private static Mock<ISpotifyCredentials> GetSpotifyCredentialsMockThatReturnsAFakeRefreshToken()
        {
            Mock<ISpotifyCredentials> spotifyCredentialsMock = new();
            spotifyCredentialsMock.Setup(x => x.GetRefreshToken()).Returns("fakeRefreshToken");
            return spotifyCredentialsMock;
        }
    }
}
