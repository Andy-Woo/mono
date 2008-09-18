//
// UrlRoutingModuleTest.cs
//
// Author:
//	Atsushi Enomoto <atsushi@ximian.com>
//
// Copyright (C) 2008 Novell Inc. http://novell.com
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Web;
using System.Web.Routing;
using NUnit.Framework;

namespace MonoTests.System.Web.Routing
{
	[TestFixture]
	public class UrlRoutingModuleTest
	{
		[SetUp]
		public void SetUp ()
		{
			RouteTable.Routes.Clear ();
		}

		[Test]
		public void SetRouteCollectionNull ()
		{
			var m = new UrlRoutingModule ();
			RouteTable.Routes.Add (new Route (null, null));
			Assert.IsNotNull (m.RouteCollection, "#1");
			Assert.AreEqual (RouteTable.Routes, m.RouteCollection, "#1-2");
			m.RouteCollection = null;
			Assert.IsNotNull (m.RouteCollection, "#2");
			Assert.AreEqual (RouteTable.Routes, m.RouteCollection, "#2-2");
		}

		// PostMapRequestHandler

		[Test]
		public void PostMapRequestHandlerNoMatch ()
		{
			var m = new UrlRoutingModule ();
			RouteTable.Routes.Add (new MyRoute (null, null));
			m.PostMapRequestHandler (new HttpContextStub2 ());
		}

		[Test]
		public void PostMapRequestHandlerNoPath ()
		{
			var m = new UrlRoutingModule ();
			RouteTable.Routes.Add (new MyRoute ("foo/bar", new MyRouteHandler ()));
			// ... huh? no NIE? what does it do then?
			m.PostMapRequestHandler (new HttpContextStub2 ("~/foo/bar", null));
		}

		[Test]
		[Ignore ("It fails at #1. Isn't it expected to do something?")]
		public void PostMapRequestHandlerModifiedPath ()
		{
			var m = new UrlRoutingModule ();
			RouteTable.Routes.Add (new MyRoute ("{foo}/{bar}", new MyRouteHandler ()));
			var hc = new HttpContextStub2 ("~/x/y", String.Empty, "apppath");
			hc.SetResponse (new HttpResponseStub (2));
			Assert.IsNotNull (m.RouteCollection.GetRouteData (hc), "#0");
			try {
				m.PostMapRequestHandler (hc);
				Assert.Fail ("#1");
			} catch (ApplicationException ex) {
				Assert.AreEqual ("~/UrlRouting.axd", ex.Message, "#2");
			}
		}

		// PostResolveRequestCache

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void PostResolveRequestCacheNullRouteHandler ()
		{
			var m = new UrlRoutingModule ();
			RouteTable.Routes.Add (new MyRoute ("foo/bar", null));
			m.PostResolveRequestCache (new HttpContextStub2 ("~/foo/bar", null));
		}

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void PostResolveRequestCacheNullHttpHandler ()
		{
			var m = new UrlRoutingModule ();
			RouteTable.Routes.Add (new MyRoute ("foo/bar", new NullRouteHandler ()));
			m.PostResolveRequestCache (new HttpContextStub2 ("~/foo/bar", null));
		}

		[Test]
		[ExpectedException (typeof (NotImplementedException))]
		public void PostResolveRequestCacheNoPath ()
		{
			var m = new UrlRoutingModule ();
			RouteTable.Routes.Add (new MyRoute ("foo/bar", new MyRouteHandler ()));
			// it tries to get HttpContextBase.Request.Path and causes NIE.
			m.PostResolveRequestCache (new HttpContextStub2 ("~/foo/bar", null));
		}

		[Test]
		public void PostResolveRequestCacheErrorHttpHandler ()
		{
			var m = new UrlRoutingModule ();
			RouteTable.Routes.Add (new MyRoute ("{foo}/{bar}", new MyRouteHandler ()));
			var hc = new HttpContextStub2 ("~/x/y", "z");
			try {
				m.PostResolveRequestCache (hc);
				Assert.Fail ("#1");
			} catch (ApplicationException ex) {
				Assert.AreEqual ("~/UrlRouting.axd", ex.Message, "#2");
			}
		}

		[Test]
		public void PostResolveRequestCacheModifiedPath ()
		{
			var m = new UrlRoutingModule ();
			RouteTable.Routes.Add (new MyRoute ("{foo}/{bar}", new MyRouteHandler ()));
			var hc = new HttpContextStub2 ("~/x/y", "z", "apppath");
			hc.SetResponse (new HttpResponseStub (2));
			try {
				m.PostResolveRequestCache (hc);
				Assert.Fail ("#1");
			} catch (ApplicationException ex) {
				Assert.AreEqual ("~/UrlRouting.axd", ex.Message, "#2");
			}
		}

		[Test]
		[Ignore ("looks like RouteExistingFiles ( = false) does not affect... so this test needs more investigation")]
		public void PathToExistingFile ()
		{
			var m = new UrlRoutingModule ();
			RouteTable.Routes.Add (new MyRoute ("{foo}/{bar}", new MyRouteHandler ()));
			var hc = new HttpContextStub2 ("~/Test/test.html", String.Empty, ".");
			// it tries to get HttpContextBase.Response, so set it.
			hc.SetResponse (new HttpResponseStub (3));
			try {
				m.PostResolveRequestCache (hc);
				Assert.Fail ("#1");
			} catch (ApplicationException ex) {
				Assert.AreEqual ("~/UrlRouting.axd", ex.Message, "#2");
			}
		}
	}
}
