using Centralite.Common.Models;
using Centralite.Ember.Ezsp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Centralite.Services.Tests
{
    [TestClass]
    public class TestMessageQueueService
    {
        private MessageQueueService messageQueueService = new MessageQueueService();

        [TestMethod]
        public void TestAddMessage()
        {
            bool messageAddedEventCalled = false;
            messageQueueService.MessageAddedEvent += delegate ()
            {
                messageAddedEventCalled = true;
            };

            var message = new EzspIncomingMessageHandlerResponse();
            var device = new DemoZigbeeDeviceBase();
            messageQueueService.AddMessage(message, device);

            var messages = (Queue<Tuple<EzspIncomingMessageHandlerResponse, ZigbeeDeviceBase>>)typeof(MessageQueueService).GetField("Messages", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(messageQueueService);

            Assert.IsTrue(messageAddedEventCalled);
            Assert.AreEqual(messages.Peek().Item1, message);
            Assert.AreEqual(messages.Peek().Item2, device);
        }

        [TestMethod]
        public void TestClearConsumerEvents()
        {
            bool messageAddedEventCalled = false;
            messageQueueService.MessageAddedEvent += delegate ()
            {
                messageAddedEventCalled = true;
            };

            messageQueueService.ClearConsumerEvents();

            var message = new EzspIncomingMessageHandlerResponse();
            var device = new DemoZigbeeDeviceBase();
            messageQueueService.AddMessage(message, device);

            Assert.IsFalse(messageAddedEventCalled);
        }

        [TestMethod]
        public void TestRetrieveMessage()
        {
            var message = new EzspIncomingMessageHandlerResponse();
            var device = new DemoZigbeeDeviceBase();
            messageQueueService.AddMessage(message, device);

            var tuple = messageQueueService.RetrieveMessage();

            Assert.AreEqual(tuple.Item1, message);
            Assert.AreEqual(tuple.Item2, device);

            tuple = messageQueueService.RetrieveMessage();

            Assert.AreEqual(tuple, null);
        }
    }
}
