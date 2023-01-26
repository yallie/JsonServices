using System;
using JsonServices.Exceptions;
using JsonServices.Messages;
using NUnit.Framework;

namespace JsonServices.Tests.Exceptions
{
	[TestFixture]
	public class ExceptionTranslatorTests
	{
		[Test]
		public void DefaultExceptionTranslatorCopyExceptionDataWithoutTranslation()
		{
			var ex = new ParseErrorException();
			var err = new ExceptionTranslator().Translate(ex);

			Assert.NotNull(err);
			Assert.AreEqual(ex.Code, err.Code);
			Assert.AreEqual(ex.Message, err.Message);
			Assert.AreEqual(ex.ToString(), err.Data);
		}

		[Test]
		public void ExceptionTranslatorCanOverrideCode()
		{
			var ex = new ParseErrorException();
			var err = new ExceptionTranslator().Translate(ex, 123);

			Assert.NotNull(err);
			Assert.AreEqual(123, err.Code);
			Assert.AreEqual(ex.Message, err.Message);
			Assert.AreEqual(ex.ToString(), err.Data);
		}

		[Test]
		public void ExceptionTranslatorCanOverrideMessage()
		{
			var ex = new ParseErrorException();
			var err = new ExceptionTranslator().Translate(ex, message: "Nice try");

			Assert.NotNull(err);
			Assert.AreEqual(ex.Code, err.Code);
			Assert.AreEqual("Nice try", err.Message);
			Assert.AreEqual(ex.ToString(), err.Data);
		}

		[Test]
		public void NonJsonServicesExceptionGetsInternalErrorCode()
		{
			var ex = new InvalidOperationException();
			var err = new ExceptionTranslator().Translate(ex);

			Assert.NotNull(err);
			Assert.AreEqual(InternalErrorException.ErrorCode, err.Code);
			Assert.AreEqual(ex.Message, err.Message);
			Assert.AreEqual(ex.ToString(), err.Data);
		}

		[Test]
		public void StubExceptionTranslatorFiresAnEvent()
		{
			var error = default(Error);
			var fired = false;
			var tr = new StubExceptionTranslator();
			tr.ErrorTranslated += (s, e) => fired = (error = e) != null;

			var ex = new InvalidOperationException();
			var err = tr.Translate(ex);

			Assert.IsTrue(fired);
			Assert.NotNull(err);
			Assert.AreSame(err, error);
			Assert.AreEqual(InternalErrorException.ErrorCode, err.Code);
			Assert.AreEqual(ex.Message, err.Message);
			Assert.AreEqual(ex.ToString(), err.Data);
		}
	}
}
