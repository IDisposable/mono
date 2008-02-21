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
//
// Authors:
//		Federico Di Gregorio <fog@initd.org>

using System;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace MonoTests.System.Linq.Expressions
{
	[TestFixture]
	public class ExpressionTest_Or
	{
		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void Arg1Null ()
		{
			Expression.Or (null, Expression.Constant (1));
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void Arg2Null ()
		{
			Expression.Or (Expression.Constant (1), null);
		}

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void NoOperatorClass ()
		{
			Expression.Or (Expression.Constant (new NoOpClass ()), Expression.Constant (new NoOpClass ()));
		}

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void ArgTypesDifferent ()
		{
			Expression.Or (Expression.Constant (1), Expression.Constant (true));
		}

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void Double ()
		{
			Expression.Or (Expression.Constant (1.0), Expression.Constant (2.0));
		}

		[Test]
		public void Integer ()
		{
			BinaryExpression expr = Expression.Or (Expression.Constant (1), Expression.Constant (2));
			Assert.AreEqual (ExpressionType.Or, expr.NodeType, "Or#01");
			Assert.AreEqual (typeof (int), expr.Type, "Or#02");
			Assert.IsNull (expr.Method, "Or#03");
			Assert.AreEqual ("(1 | 2)", expr.ToString(), "Or#04");
		}

		[Test]
		public void Boolean ()
		{
			BinaryExpression expr = Expression.Or (Expression.Constant (true), Expression.Constant (false));
			Assert.AreEqual (ExpressionType.Or, expr.NodeType, "Or#05");
			Assert.AreEqual (typeof (bool), expr.Type, "Or#06");
			Assert.IsNull (expr.Method, "Or#07");
			Assert.AreEqual ("(True Or False)", expr.ToString(), "Or#08");
		}

		[Test]
		public void UserDefinedClass ()
		{
			// We can use the simplest version of GetMethod because we already know only one
			// exists in the very simple class we're using for the tests.
			MethodInfo mi = typeof (OpClass).GetMethod ("op_BitwiseOr");

			BinaryExpression expr = Expression.Or (Expression.Constant (new OpClass ()), Expression.Constant (new OpClass ()));
			Assert.AreEqual (ExpressionType.Or, expr.NodeType, "Or#09");
			Assert.AreEqual (typeof (OpClass), expr.Type, "Or#10");
			Assert.AreEqual (mi, expr.Method, "Or#11");
			Assert.AreEqual ("op_BitwiseOr", expr.Method.Name, "Or#12");
			Assert.AreEqual ("(value(MonoTests.System.Linq.Expressions.OpClass) | value(MonoTests.System.Linq.Expressions.OpClass))",
				expr.ToString(), "Or#13");
		}

		[Test]
		public void OrTest ()
		{
			Expression<Func<bool, bool, bool>> e = (bool a, bool b) => a | b;

			Func<bool,bool,bool> c = e.Compile ();

			Assert.AreEqual (true,  c (true, true), "o1");
			Assert.AreEqual (true, c (true, false), "o2");
			Assert.AreEqual (true, c (false, true), "o3");
			Assert.AreEqual (false, c (false, false), "o4");
		}

		[Test]
		public void OrNullableTest ()
		{
			Expression<Func<bool?, bool?, bool?>> e = (bool? a, bool? b) => a | b;

			Func<bool?,bool?,bool?> c = e.Compile ();

			Assert.AreEqual (true,  c (true, true),   "o1");
			Assert.AreEqual (true,  c (true, false),  "o2");
			Assert.AreEqual (true,  c (false, true),  "o3");
			Assert.AreEqual (false, c (false, false), "o4");

			Assert.AreEqual (true, c (true, null),  "o5");
			Assert.AreEqual (null, c (false, null), "o6");
			Assert.AreEqual (null, c (null, false), "o7");
			Assert.AreEqual (true, c (true, null),  "o8");
			Assert.AreEqual (null, c (null, null),  "o9");
		}

	}
}
