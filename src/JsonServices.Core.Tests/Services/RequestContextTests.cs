﻿using System;
using System.Threading.Tasks;
using JsonServices.Services;
using NUnit.Framework;

namespace JsonServices.Tests.Services
{
	[TestFixture]
	public class RequestContextTests
	{
		class NonDisposable
		{
			public bool IsDisposed { get; set; }
			public void Dispose()
			{
				IsDisposed = true;
			}
		}

		class Disposable : NonDisposable, IDisposable
		{
		}

		[Test]
		public void RequestContextDisposesOfItsProperties()
		{
			var ctx = new RequestContext();
			ctx.Properties["NonDisposable"] = new NonDisposable();
			ctx.Properties["Disposable"] = new Disposable();
			ctx.Dispose();

			var nd = ctx.Properties["NonDisposable"] as NonDisposable;
			Assert.NotNull(nd);
			Assert.IsFalse(nd.IsDisposed);

			var d = ctx.Properties["Disposable"] as Disposable;
			Assert.NotNull(d);
			Assert.IsTrue(d.IsDisposed);
		}

		[Test]
		public async Task TestCurrentRequestContextProperty()
		{
			var ctx = new RequestContext();
			ctx.Properties["Hello"] = "World";
			RequestContext.CurrentContextHolder.Value = ctx;

			await Task.Yield();
			Assert.AreSame(ctx, RequestContext.Current);

			await Task.Delay(10).ConfigureAwait(false);
			Assert.AreSame(ctx, RequestContext.Current);
		}
	}
}
