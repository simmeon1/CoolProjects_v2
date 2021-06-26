using ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
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
        private string successfulResponse = @"{""access_token"":" + $"{fakeAccessToken}" + @",""token_type"":" + $"{fakeTokenType}" + @",""expires_in"":" + $"{fakeExpiresIn}" + @",""scope"":" + $"{fakeScope}" + "}";

        [TestMethod]
        public async Task TokensHaveTheSameData_ExpectedSuccessfulMatch()
        {
            SpotifyToken tokenOne = new(fakeAccessToken, fakeExpiresIn, fakeScope, fakeTokenType);
            SpotifyToken tokenTwo = new(fakeAccessToken, fakeExpiresIn, fakeScope, fakeTokenType);
            Assert.IsTrue(ISpotifyTokenWorker.TokensHaveTheSameData(tokenOne, tokenTwo));
        }
        
        [TestMethod]
        public async Task TokensHaveTheSameData_ExpectedUnsuccessfulMatch()
        {
            SpotifyToken tokenOne = new(fakeAccessToken, fakeExpiresIn, fakeScope, fakeTokenType);
            SpotifyToken tokenTwo = new("asd", 0, "asd", "asd");
            Assert.IsFalse(ISpotifyTokenWorker.TokensHaveTheSameData(tokenOne, tokenTwo));
        }

        [TestMethod]
        public async Task GetAndSetNewAccessToken_ReturnsSuccessfulResponseAndToken()
        {

            Mock<ISpotifyCredentials> spotifyCredentialsMock = GetSpotifyCredentialsMockThatReturnsAFakeRefreshToken();
            Mock<IHttpClient> httpClientMock = GetHttpClientMockThatReturnsAGivenStatusCodeAndMessage(HttpStatusCode.OK, successfulResponse);
            Mock<IJsonParser> jsonParserMock = GetJsonParserMockThatReturnsFakeTokenData();

            SpotifyAPIClient spotifyClient = new(spotifyCredentialsMock.Object, httpClientMock.Object, jsonParserMock.Object);
            await spotifyClient.GetAndSetNewAccessToken();

            SpotifyToken tokenToCompare = new(fakeAccessToken, fakeExpiresIn, fakeScope, fakeTokenType);
            Assert.IsTrue(spotifyClient.ConfirmTokenAgainstCurrentToken(tokenToCompare));
        }

        [TestMethod]
        public async Task GetAndSetNewAccessToken_ThrowsExceptionForBadRequest()
        {
            string failResponse = "failResponse";
            Mock<ISpotifyCredentials> spotifyCredentialsMock = GetSpotifyCredentialsMockThatReturnsAFakeRefreshToken();
            HttpStatusCode responseCode = HttpStatusCode.BadRequest;
            Mock<IHttpClient> httpClientMock = GetHttpClientMockThatReturnsAGivenStatusCodeAndMessage(responseCode, failResponse);
            Mock<IJsonParser> jsonParserMock = new();
            SpotifyAPIClient spotifyClient = new(spotifyCredentialsMock.Object, httpClientMock.Object, jsonParserMock.Object);

            try
            {
                await spotifyClient.GetAndSetNewAccessToken();
                Assert.Fail("Expected token request to fail.");
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Equals($"The token request was not successful. The details are as follows:{Environment.NewLine}" +
                    $"Status code: {responseCode}{Environment.NewLine}" +
                    $"Error details: {failResponse}"));
            }
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
