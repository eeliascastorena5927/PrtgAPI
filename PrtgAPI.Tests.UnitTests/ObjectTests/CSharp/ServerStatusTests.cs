﻿using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrtgAPI.Tests.UnitTests.ObjectTests.TestItems;
using PrtgAPI.Tests.UnitTests.ObjectTests.TestResponses;

namespace PrtgAPI.Tests.UnitTests.ObjectTests
{
    [TestClass]
    public class ServerStatusTests : BaseTest
    {
        [TestMethod]
        public void ServerStatus_CanExecute()
        {
            var client = Initialize_Client(new ServerStatusResponse(GetItem()));

            var result = client.GetStatus();

            AssertEx.AllPropertiesAreNotDefault(result);
        }

        [TestMethod]
        public async Task ServerStatus_CanExecuteAsync()
        {
            var client = Initialize_Client(new ServerStatusResponse(GetItem()));

            var result = await client.GetStatusAsync();

            AssertEx.AllPropertiesAreNotDefault(result);
        }

        [TestMethod]
        public void ServerStatus_CanDeserializeEmpty()
        {
            var item = new ServerStatusItem(
                newMessages: "0",
                newAlarms: "0",
                alarms: "",
                partialAlarms: "",
                ackAlarms: "",
                unusualSens: "",
                upSens: "",
                warnSens: "",
                pausedSens: "",
                unknownSens: "",
                newTickets: "",
                userId: "0",
                userTimeZone: "",
                toDos: "",
                favs: "0",
                clock: "10.11.2017 04:27:34 PM",
                version: "1.2.3.4+",
                backgroundTasks: "0",
                correlationTasks: "0",
                autoDiscoTasks: "0",
                reportTasks: "0",
                editionType: "",
                prtgUpdateAvailable: "false",
                maintExpiryDays: "??",
                trialExpiryDays: "-999999",
                commercialExpiryDays: "",
                overloadProtection: "false",
                clusterType: "",
                clusterNodeName: "",
                isAdminUser: "false",
                readOnlyUser: "",
                ticketUser: "",
                readOnlyAllowAcknowledge: "",
                lowMem: "false",
                activationAlert: "",
                prtgHost: "",
                maxSensorCount: "",
                activated: ""
            );

            var client = Initialize_Client(new ServerStatusResponse(item));

            var result = client.GetStatus();

            //Test accessing each property

            foreach (var prop in result.GetType().GetProperties())
            {
                var value = prop.GetValue(result);
            }
        }

        [TestMethod]
        public void ServerStatus_AlternateValues()
        {
            var item = new ServerStatusItem(commercialExpiryDays: "-999999", clusterNodeName: "Cluster Node \\\"PRTG Network Monitor (Failover)\\\" (Failover Node)");

            var client = Initialize_Client(new ServerStatusResponse(item));

            var result = client.GetStatus();

            Assert.AreEqual(null, result.CommercialExpiryDays);
            Assert.AreEqual("PRTG Network Monitor (Failover)", result.ClusterNodeName);
        }

        public ServerStatusItem GetItem() => new ServerStatusItem();
    }
}
