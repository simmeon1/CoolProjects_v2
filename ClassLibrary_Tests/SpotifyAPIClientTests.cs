using ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClassLibrary_Tests
{
    [TestClass]
    public class SpotifyAPIClientTests
    {

        [TestMethod]
        public async Task GetToken_ReturnsSuccessfulResponseAndToken()
        {
            string successfulResponse = @"{""access_token"":""fakeAccessToken"",""token_type"":""fakeTokenType"",""expires_in"":3600,""scope"":""fakeScope""}";

            Mock<ISpotifyCredentials> spotifyCredentialsMock = new();
            spotifyCredentialsMock.Setup(x => x.GetRefreshToken()).Returns("fakeRefreshToken");

            Mock<IHttpClient> httpClientMock = new();
            HttpResponseMessage responseFromTokenRequest = new(statusCode: HttpStatusCode.OK);
            responseFromTokenRequest.Content = new StringContent(successfulResponse);
            httpClientMock.Setup(x => x.SendRequest((It.IsAny<HttpRequestMessage>())).Result).Returns(responseFromTokenRequest);

            Mock<IJsonParser> jsonParserMock = new();
            jsonParserMock.Setup(x => x.GetPropertyValue<string>("access_token")).Returns("fakeAccessToken");
            jsonParserMock.Setup(x => x.GetPropertyValue<string>("token_type")).Returns("fakeTokenType");
            jsonParserMock.Setup(x => x.GetPropertyValue<int>("expires_in")).Returns(3600);
            jsonParserMock.Setup(x => x.GetPropertyValue<string>("scope")).Returns("fakeScope");

            SpotifyAPIClient spotifyClient = new(spotifyCredentialsMock.Object, httpClientMock.Object, jsonParserMock.Object);
            await spotifyClient.GetAndSetNewAccessToken();

            SpotifyToken tokenToCompare = new("fakeAccessToken", 3600, "fakeScope", "fakeTokenType");
            Assert.IsTrue(spotifyClient.ConfirmTokenAgainstCurrentToken(tokenToCompare));
        }
    }
}
