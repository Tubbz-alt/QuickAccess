﻿#region LICENSE [BSD-2-Clause]

// This code is distributed under the BSD-2-Clause license.
// =====================================================================
// 
// Copyright ©2018 by Kamil Piotr Kaczorek
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, 
//     this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, 
//     this list of conditions and the following disclaimer in the documentation and/or 
//     other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
// IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES 
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF 
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// =====================================================================
// 
// Project: QuickAccess.Parser.Tests
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuickAccess.Parser.Tests
{
	[TestClass]
	public class MathExpressionCompilerIntergraionTest
	{
		[TestMethod]
		public void ON_Compile_WHEN_Expression_Is_Correct_SHOULD_Return_Executable_Expression_Node_That_Gives_Proper_Result()
		{
			// Arrange
			var sourceCode = "sin(90*PI/180.0)*2^(1+1)*-1e-1";
			var compiler = new MathExpressionCompiler(CharComparer.CaseSensitive, new MathExpressionParserFactory());
			compiler.DefineOperator<double>("+", (x, y) => x + y, 0);
			compiler.DefineOperator<double>("-", (x, y) => x - y, 0);
			compiler.DefineOperator<double>("*", (x, y) => x * y, 10);
			compiler.DefineOperator<double>("/", (x, y) => x / y, 10);
			compiler.DefineOperator<double>("^", Math.Pow, 20);
			compiler.DefineOperator<double>("+", x => x);
			compiler.DefineOperator<double>("-", x => -x);
			compiler.DefineFunction<double>("pow", Math.Pow);
			compiler.DefineFunction<double>("sin", Math.Sin);
			compiler.DefineFunction<double>("cos", Math.Cos);
			compiler.DefineFunction<double>("ceiling", Math.Ceiling);
			compiler.DefineFunction<double>("floor", Math.Floor);
			compiler.DefineFunction<double>("abs", Math.Abs);
			compiler.DefineVariable("PI", () => Math.PI);

			var source = new StringSourceCode(new ParsingContextStreamFactory(), new SourceCodeFragmentFactory(), sourceCode);
			// Act
			var res = compiler.Compile(source);
			// Assert
			Assert.IsNotNull(res);
			var error = source.GetError();
			Assert.IsNull(error);
			var calcRes = (double) res.Execute();
			Assert.AreEqual(-0.4, calcRes, 0.0000001);
		}
	}
}