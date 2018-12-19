using Centralite.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reflection;

namespace Centralite.Services.Tests
{
    [TestClass]
    public class TestErrorMessageService
    {
        private ErrorMessageService errorMessageService = new ErrorMessageService();

        [TestMethod]
        public void TestAddMessage()
        {
            bool messageAddedEventCalled = false;
            errorMessageService.ErrorMessageAddedEvent += delegate ()
            {
                messageAddedEventCalled = true;
            };

            var message = new ErrorMessage();
            errorMessageService.AddMessage(message);

            var messages = (Queue<ErrorMessage>)typeof(ErrorMessageService).GetField("ErrorMessages", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(errorMessageService);

            Assert.IsTrue(messageAddedEventCalled);
            Assert.AreEqual(messages.Peek(), message);
        }

        [TestMethod]
        public void TestClearConsumerEvents()
        {
            bool messageAddedEventCalled = false;
            errorMessageService.ErrorMessageAddedEvent += delegate ()
            {
                messageAddedEventCalled = true;
            };

            errorMessageService.ClearConsumerEvents();

            var message = new ErrorMessage();
            errorMessageService.AddMessage(message);

            Assert.IsFalse(messageAddedEventCalled);
        }

        [TestMethod]
        public void TestRetrieveMessage()
        {
            var message = new ErrorMessage();
            errorMessageService.AddMessage(message);

            var retrievedMessage = errorMessageService.RetrieveMessage();

            Assert.AreEqual(retrievedMessage, message);

            retrievedMessage = errorMessageService.RetrieveMessage();

            Assert.AreEqual(retrievedMessage, null);
        }
    }
}
