using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JourneyPlanner_Tests.UnitTests
{
    [TestClass]
    public class JourneyRetrieverInstanceCreator_UnitTests
    {
        [TestMethod]
        public void AllClassesThatImplementInterfaceBuildOKWithExpectedConstructor()
        {
            Type interfaceType = typeof(IJourneyRetriever);
            IEnumerable<Type> types = interfaceType.Assembly.GetTypes()
                .Where(p => interfaceType.IsAssignableFrom(p))
                .Where(p => !p.Name.Equals(interfaceType.Name));

            JourneyRetrieverComponents components = new(
                null,
                null,
                null,
                null
            );

            JourneyRetrieverInstanceCreator instanceCreator = new();
            List<IJourneyRetriever> instances = new();
            foreach (Type type in types) instances.Add(instanceCreator.CreateInstance(type.FullName, components));
            Assert.IsTrue(instances.Count == 3);
            Assert.IsTrue(instances.Any(i => i.GetType().Name.Equals(nameof(GoogleFlightsWorker))));
            Assert.IsTrue(instances.Any(i => i.GetType().Name.Equals(nameof(NationalExpressWorker))));
            Assert.IsTrue(instances.Any(i => i.GetType().Name.Equals(nameof(MegaBusWorker))));
        }
    }
}
