using System;
using System.Collections.Generic;
using System.Linq;
using JourneyPlanner_ClassLibrary;
using JourneyPlanner_ClassLibrary.Interfaces;
using JourneyPlanner_ClassLibrary.JourneyRetrievers;
using JourneyPlanner_ClassLibrary.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JourneyPlanner_Tests.UnitTests.Workers
{
    [TestClass]
    public class JourneyRetrieverInstanceCreatorUnitTests
    {
        [TestMethod]
        public void AllClassesThatImplementInterfaceBuildOkWithExpectedConstructor()
        {
            Type interfaceType = typeof(IJourneyRetriever);
            IEnumerable<Type> types = interfaceType.Assembly.GetTypes()
                .Where(p => interfaceType.IsAssignableFrom(p))
                .Where(p => !p.Name.Equals(interfaceType.Name));

            JourneyRetrieverComponents components = new(
                null,
                null,
                null,
                null,
                null,
                null);

            JourneyRetrieverInstanceCreator instanceCreator = new();
            List<IJourneyRetriever> instances = new();
            foreach (Type type in types) instances.Add(instanceCreator.CreateInstance(type.FullName, components));
            Assert.IsTrue(instances.Count == 4);
            Assert.IsTrue(instances.Any(i => i.GetType().Name.Equals(nameof(GoogleFlightsWorker))));
            // Assert.IsTrue(instances.Any(i => i.GetType().Name.Equals(nameof(NationalExpressWorker))));
            Assert.IsTrue(instances.Any(i => i.GetType().Name.Equals(nameof(MegaBusWorker))));
            Assert.IsTrue(instances.Any(i => i.GetType().Name.Equals(nameof(MegaBusScheduledWorker))));
        }
    }
}
