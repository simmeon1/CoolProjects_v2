using Common_ClassLibrary;
using FlightConnectionsDotCom_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FlightConnectionsDotCom_Tests.UnitTests
{
    [TestClass]
    public class GoogleFlightsWorker_UnitTests
    {
        Mock<IWebDriver> driverMock;
        Mock<ILogger> logger;

        private void InitialiseMockObjects()
        {
            driverMock = new();
            logger = new();
        }

        [TestMethod]
        public async Task OpenFlights_ExpectedResultsWithStandartProcess()
        {
            InitialiseMockObjects();
            List<string> path1 = new() { "ABZ", "LTN", "VAR" };
            List<string> path2 = new() { "EDI", "LTN", "VAR" };
            List<Path> paths = new() { new Path(path1), new Path(path2) };

            driverMock.Setup(x => x.Navigate()).Returns(new Mock<INavigation>().Object);
            driverMock.Setup(x => x.SwitchTo()).Returns(new Mock<ITargetLocator>().Object);
            driverMock.Setup(x => x.FindElement(By.CssSelector("header"))).Returns(new Mock<IWebElement>().Object);

            Mock<IWebElement> flightListMock1 = new();
            Mock<IWebElement> flightListMock2 = new();
            Mock<IWebElement> flightListMock3 = new();
            Mock<IWebElement> flightListMock4 = new();
            Mock<IWebElement> flightListMock5 = new();
            Mock<IWebElement> flightListMock6 = new();
            Mock<IWebElement> flightMock1 = new();
            Mock<IWebElement> flightMock2 = new();
            Mock<IWebElement> flightMock3 = new();
            Mock<IWebElement> flightMock4 = new();
            Mock<IWebElement> flightMock5 = new();
            Mock<IWebElement> flightMock6 = new();
            Mock<IWebElement> flightMock7 = new();

            string flightText1 = "08:00 AM\n- \n08:30 AM\nWizz Air\n30 min\nABZ-LTN\nNonstop\nPrice £10";
            string flightText2 = "09:30 AM\n- \n10:30 AM\nWizz Air\n1 hr\nABZ-LTN\nNonstop\nPrice £15";
            string flightText3 = "9:45 PM\n- \n2:55 AM+1\nWizz Air\n3 hr 10 min\nLTN-VAR\nNonstop\n£27";
            string flightText4 = "9:45 PM\n- \n11:45 PM\nWizz Air\n2 hr\nLTN-VAR\nNonstop\nPrice unavailable";
            string flightText5 = "06:00 AM\n- \n07:00 AM\neasyJet\n1 hr\nEDI-LTN\nNonstop\n£25";
            string flightText6 = "13:45 PM\n- \n14:45 PM\neasyJet\n1 hr\nEDI-LTN\nNonstop\n£30";
            string flightText7 = "20:00 PM\n- \n21:00 PM\neasyJet\n1 hr\nEDI-LTN\nNonstop\n£40";

            flightMock1.SetupSequence(x => x.GetAttribute("innerText")).Returns("more flights").Returns(flightText1).Returns(flightText1);
            flightMock2.SetupSequence(x => x.GetAttribute("innerText")).Returns(flightText2).Returns(flightText2);
            flightMock3.SetupSequence(x => x.GetAttribute("innerText")).Returns(flightText3).Returns(flightText3);
            flightMock4.SetupSequence(x => x.GetAttribute("innerText")).Returns(flightText4).Returns(flightText4);
            flightMock5.SetupSequence(x => x.GetAttribute("innerText")).Returns(flightText5).Returns(flightText5);
            flightMock6.SetupSequence(x => x.GetAttribute("innerText")).Returns(flightText6).Returns(flightText6);
            flightMock7.SetupSequence(x => x.GetAttribute("innerText")).Returns(flightText7).Returns(flightText7);

            flightListMock1.Setup(x => x.FindElements(By.CssSelector("[role=listitem]"))).Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement> { flightMock1.Object }));
            flightListMock2.Setup(x => x.FindElements(By.CssSelector("[role=listitem]"))).Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement> { flightMock2.Object }));
            flightListMock3.Setup(x => x.FindElements(By.CssSelector("[role=listitem]"))).Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement> { flightMock3.Object }));
            flightListMock4.Setup(x => x.FindElements(By.CssSelector("[role=listitem]"))).Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement> { flightMock4.Object }));
            flightListMock5.Setup(x => x.FindElements(By.CssSelector("[role=listitem]"))).Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement> { flightMock5.Object, flightMock6.Object }));
            flightListMock6.Setup(x => x.FindElements(By.CssSelector("[role=listitem]"))).Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement> { flightMock7.Object }));
            driverMock.SetupSequence(x => x.FindElements(By.CssSelector("[role=list]")))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement> { flightListMock1.Object }))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement> { flightListMock1.Object }))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement> { flightListMock2.Object }))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement> { flightListMock3.Object }))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement> { flightListMock4.Object }))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement> { flightListMock5.Object }))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement> { flightListMock6.Object }));

            Mock<IWebElement> consentButton1 = new();
            Mock<IWebElement> consentButton2 = new();
            consentButton1.Setup(x => x.Text).Throws(new StaleElementReferenceException());
            consentButton2.Setup(x => x.Text).Returns("I agree");

            Mock<IWebElement> searchButton = new();
            searchButton.Setup(x => x.GetAttribute("aria-label")).Returns("Done. Search for");

            Mock<IWebElement> stopsButton = new();
            stopsButton.Setup(x => x.GetAttribute("aria-label")).Returns("Stops");

            ReadOnlyCollection<IWebElement> radioGroupChildren = new(new List<IWebElement>() { new Mock<IWebElement>().Object, new Mock<IWebElement>().Object });
            Mock<IWebElement> radioGroup = new();
            radioGroup.Setup(x => x.FindElements(By.CssSelector("input"))).Returns(radioGroupChildren);
            driverMock.Setup(x => x.FindElement(By.CssSelector("[role=radiogroup]"))).Returns(radioGroup.Object);

            driverMock.SetupSequence(x => x.FindElements(By.CssSelector("button")))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { consentButton1.Object, consentButton2.Object }))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { searchButton.Object }))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { searchButton.Object }))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { searchButton.Object }))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { searchButton.Object }))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { stopsButton.Object }))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { consentButton1.Object, consentButton2.Object }))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { consentButton1.Object, consentButton2.Object }))
             ;

            Mock<IWebElement> roundTripSpan = new();
            roundTripSpan.Setup(x => x.Text).Returns("Round trip");
            driverMock.Setup(x => x.FindElements(By.CssSelector("span")))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { roundTripSpan.Object }));

            Mock<IWebElement> oneWayLi = new();
            oneWayLi.Setup(x => x.Text).Returns("One way");
            driverMock.Setup(x => x.FindElements(By.CssSelector("li"))).Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { oneWayLi.Object }));

            ReadOnlyCollection<IWebElement> inputs = new(new List<IWebElement>() {
                    new Mock<IWebElement>().Object, new Mock<IWebElement>().Object, new Mock<IWebElement>().Object,
                    new Mock<IWebElement>().Object, new Mock<IWebElement>().Object, new Mock<IWebElement>().Object,
                    new Mock<IWebElement>().Object });
            driverMock.Setup(x => x.FindElements(By.CssSelector("input"))).Returns(inputs);

            GoogleFlightsWorker chromeWorker = new(logger.Object, new Mock<IDelayer>().Object, driverMock.Object);
            GoogleFlightsWorkerResults results = await chromeWorker.ProcessPaths(paths, new DateTime(2000, 10, 10), new DateTime(2000, 10, 11));

            Dictionary<string, JourneyCollection> workerFlights = results.PathsAndJourneys;
            Assert.IsTrue(workerFlights.Count == 3);
            Assert.IsTrue(workerFlights["ABZ-LTN"].GetCount() == 2);
            Assert.IsTrue(workerFlights["LTN-VAR"].GetCount() == 2);
            Assert.IsTrue(workerFlights["EDI-LTN"].GetCount() == 3);

            List<FullPathAndListOfPathsAndJourneyCollections> workerPaths = results.FullPathsAndJourneyCollections;
            Assert.IsTrue(workerPaths.Count == 2);
            Assert.IsTrue(workerPaths[0].Path.ToString().Equals("ABZ-LTN-VAR"));
            Assert.IsTrue(workerPaths[0].PathsAndJourneyCollections.Count == 2);
            Assert.IsTrue(workerPaths[0].PathsAndJourneyCollections[0].Path.ToString().Equals("ABZ-LTN"));
            Assert.IsTrue(workerPaths[0].PathsAndJourneyCollections[1].Path.ToString().Equals("LTN-VAR"));
            Assert.IsTrue(workerPaths[1].Path.ToString().Equals("EDI-LTN-VAR"));
            Assert.IsTrue(workerPaths[1].PathsAndJourneyCollections.Count == 2);
            Assert.IsTrue(workerPaths[1].PathsAndJourneyCollections[0].Path.ToString().Equals("EDI-LTN"));
            Assert.IsTrue(workerPaths[1].PathsAndJourneyCollections[1].Path.ToString().Equals("LTN-VAR"));

            Assert.IsTrue(workerPaths[0].PathsAndJourneyCollections[0].JourneyCollection.GetCount() == 2);
            Assert.IsTrue(workerPaths[0].PathsAndJourneyCollections[0].JourneyCollection[0].ToString().Equals(@"ABZ-LTN - 10/10/2000 08:00:00 - 10/10/2000 08:30:00 - Wizz Air - 00:30:00 - 10 - Flight"));
            Assert.IsTrue(workerPaths[0].PathsAndJourneyCollections[0].JourneyCollection[1].ToString().Equals(@"ABZ-LTN - 11/10/2000 09:30:00 - 11/10/2000 10:30:00 - Wizz Air - 01:00:00 - 15 - Flight"));
            Assert.IsTrue(workerPaths[0].PathsAndJourneyCollections[1].JourneyCollection[0].ToString().Equals(@"LTN-VAR - 10/10/2000 21:45:00 - 11/10/2000 02:55:00 - Wizz Air - 03:10:00 - 27 - Flight"));
            Assert.IsTrue(workerPaths[0].PathsAndJourneyCollections[1].JourneyCollection[1].ToString().Equals(@"LTN-VAR - 11/10/2000 21:45:00 - 11/10/2000 23:45:00 - Wizz Air - 02:00:00 - 0 - Flight"));

            Assert.IsTrue(workerPaths[1].PathsAndJourneyCollections[0].JourneyCollection[0].ToString().Equals(@"EDI-LTN - 10/10/2000 06:00:00 - 10/10/2000 07:00:00 - easyJet - 01:00:00 - 25 - Flight"));
            Assert.IsTrue(workerPaths[1].PathsAndJourneyCollections[0].JourneyCollection[1].ToString().Equals(@"EDI-LTN - 10/10/2000 13:45:00 - 10/10/2000 14:45:00 - easyJet - 01:00:00 - 30 - Flight"));
            Assert.IsTrue(workerPaths[1].PathsAndJourneyCollections[0].JourneyCollection[2].ToString().Equals(@"EDI-LTN - 11/10/2000 20:00:00 - 11/10/2000 21:00:00 - easyJet - 01:00:00 - 40 - Flight"));
            Assert.IsTrue(workerPaths[1].PathsAndJourneyCollections[1].JourneyCollection[0].ToString().Equals(@"LTN-VAR - 10/10/2000 21:45:00 - 11/10/2000 02:55:00 - Wizz Air - 03:10:00 - 27 - Flight"));
            Assert.IsTrue(workerPaths[1].PathsAndJourneyCollections[1].JourneyCollection[1].ToString().Equals(@"LTN-VAR - 11/10/2000 21:45:00 - 11/10/2000 23:45:00 - Wizz Air - 02:00:00 - 0 - Flight"));
        }
        
        [TestMethod]
        public async Task asd()
        {
            InitialiseMockObjects();
            GoogleFlightsWorker chromeWorker = new(logger.Object, new Mock<IDelayer>().Object, driverMock.Object);
            Dictionary<string, JourneyCollection> collectedPathFlights = new();
            collectedPathFlights.Add("ABZ-LTN", new());
            GoogleFlightsWorkerResults results = await chromeWorker.ProcessPaths(new List<Path>() { new Path(new List<string>() { "ABZ", "LTN" }) }, new DateTime(2000, 10, 10), new DateTime(2000, 10, 11), collectedPathJourneys: collectedPathFlights);
            logger.Verify(x => x.Log("An exception was thrown while collecting flights and the results have been returned early."), Times.Once());
            Assert.IsTrue(results.PathsAndJourneys.SerializeObject().Equals(collectedPathFlights.SerializeObject()));
            Assert.IsTrue(results.FullPathsAndJourneyCollections.Count == 0);
        }
    }
}
