﻿#region LICENSE [BSD-2-Clause]
// This code is distributed under the BSD-2-Clause license.
// =====================================================================
// 
// Copyright ©2019 by Kamil Piotr Kaczorek
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
// Project: QuickAccess.Parser
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com
#endregion

using System;
using QuickAccess.DataStructures.CodeOperatorAlgebra;
using QuickAccess.DataStructures.Common.RegularExpression;

namespace QuickAccess.Parser.SmartExpressions
{
	public abstract class SmartExpressionBrick
		: IExpressionParser,
		  IRegularExpressionRepresentable,
		  ICodeOperatorAlgebraicDomain<SmartExpressionBrick, ISmartExpressionAlgebra>,
		  IEquatable<SmartExpressionBrick>
	{
		protected SmartExpressionBrick(ISmartExpressionAlgebra algebra)
		{
			Algebra = algebra ?? SX.DefaultAlgebra;
		}

		public virtual string Name => GetType().Name;


		/// <inheritdoc />
		public ISmartExpressionAlgebra Algebra { get; }

		public SmartExpressionBrick this[long minCount, long maxCount] =>
			Algebra.CreateQuantifierBrick(this, minCount, maxCount);

		public SmartExpressionBrick this[long count] => Algebra.CreateQuantifierBrick(this, count, count);

		public virtual bool IsEmpty => false;

		public static SmartExpressionBrick operator *(SmartExpressionBrick left, SmartExpressionBrick right)
		{
			return SX.DefaultAlgebra.GetOperatorResultOfHighestPrioritizedAlgebra(left, OverloadableCodeBinarySymmetricOperator.Mul, right);
		}

		public static SmartExpressionBrick operator /(SmartExpressionBrick left, SmartExpressionBrick right)
		{
			return SX.DefaultAlgebra.GetOperatorResultOfHighestPrioritizedAlgebra(left, OverloadableCodeBinarySymmetricOperator.Div, right);
		}

		public static SmartExpressionBrick operator %(SmartExpressionBrick left, SmartExpressionBrick right)
		{
			return SX.DefaultAlgebra.GetOperatorResultOfHighestPrioritizedAlgebra(left, OverloadableCodeBinarySymmetricOperator.Mod, right);
		}

		public static SmartExpressionBrick operator +(SmartExpressionBrick left, SmartExpressionBrick right)
		{
			return SX.DefaultAlgebra.GetOperatorResultOfHighestPrioritizedAlgebra(left, OverloadableCodeBinarySymmetricOperator.Sum, right);
		}

		public static SmartExpressionBrick operator -(SmartExpressionBrick left, SmartExpressionBrick right)
		{
			return SX.DefaultAlgebra.GetOperatorResultOfHighestPrioritizedAlgebra(left, OverloadableCodeBinarySymmetricOperator.Sub, right);
		}

		public static SmartExpressionBrick operator &(SmartExpressionBrick left, SmartExpressionBrick right)
		{
			return SX.DefaultAlgebra.GetOperatorResultOfHighestPrioritizedAlgebra(left, OverloadableCodeBinarySymmetricOperator.And, right);
		}

		public static SmartExpressionBrick operator ^(SmartExpressionBrick left, SmartExpressionBrick right)
		{
			return SX.DefaultAlgebra.GetOperatorResultOfHighestPrioritizedAlgebra(left, OverloadableCodeBinarySymmetricOperator.XOr, right);
		}

		public static SmartExpressionBrick operator |(SmartExpressionBrick left, SmartExpressionBrick right)
		{
			return SX.DefaultAlgebra.GetOperatorResultOfHighestPrioritizedAlgebra(left, OverloadableCodeBinarySymmetricOperator.Or, right);
		}

		public static SmartExpressionBrick operator ++(SmartExpressionBrick arg)
		{
			return SX.DefaultAlgebra.EvaluateOperatorResult(OverloadableCodeUnarySymmetricOperator.Increment, arg);
		}

		public static SmartExpressionBrick operator --(SmartExpressionBrick arg)
		{
			return SX.DefaultAlgebra.EvaluateOperatorResult(OverloadableCodeUnarySymmetricOperator.Decrement, arg);
		}

		public static SmartExpressionBrick operator +(SmartExpressionBrick arg)
		{
			return SX.DefaultAlgebra.EvaluateOperatorResult(OverloadableCodeUnarySymmetricOperator.Plus, arg);
		}

		public static SmartExpressionBrick operator -(SmartExpressionBrick arg)
		{
			return SX.DefaultAlgebra.EvaluateOperatorResult(OverloadableCodeUnarySymmetricOperator.Minus, arg);
		}

		public static SmartExpressionBrick operator !(SmartExpressionBrick arg)
		{
			return SX.DefaultAlgebra.EvaluateOperatorResult(OverloadableCodeUnarySymmetricOperator.LogicalNegation, arg);
		}

		public static SmartExpressionBrick operator ~(SmartExpressionBrick arg)
		{
			return SX.DefaultAlgebra.EvaluateOperatorResult(OverloadableCodeUnarySymmetricOperator.BitwiseComplement, arg);
		}

		public static implicit operator SmartExpressionBrick(string x)
		{
			return SX.ToTextSequence(x);
		}

		public static implicit operator SmartExpressionBrick(char x)
		{
			return SX.ToCharacter(x);
		}

		public void ApplyCustomRule(string name, SmartExpressionBrick content)
		{
			ApplyRuleDefinition(name, content, recursion: false, freeze: false);
		}

		public void ApplyCustomSealedRule(string name, SmartExpressionBrick content)
		{
			ApplyRuleDefinition(name, content, recursion: false, freeze: true);
		}

		protected abstract void ApplyRuleDefinition(string name, SmartExpressionBrick content, bool recursion, bool freeze);

		protected static void ApplyRuleDefinition(SmartExpressionBrick target,
		                                          string name,
		                                          SmartExpressionBrick content,
		                                          bool recursion,
		                                          bool freeze)
		{
			target.ApplyRuleDefinition(name, content, recursion, freeze);
		}


		public abstract string ExpressionId { get; }

		public virtual string ToRegularExpressionString(RegularExpressionBuildingContext ctx)
		{
			var matchingLevel = RegularExpressionMatchingLevel;
			if (matchingLevel != MatchingLevel.None)
			{
				throw new InvalidOperationException(
					$"{nameof(RegularExpressionMatchingLevel)} returns {matchingLevel} but {nameof(ToRegularExpressionString)} method is not overloaded.");
			}

			throw new NotSupportedException($"Conversion to regular expression is not supported for {GetType()}.");
		}

		/// <inheritdoc />
		public virtual MatchingLevel RegularExpressionMatchingLevel => MatchingLevel.None;

		/// <inheritdoc />
		public abstract bool Equals(SmartExpressionBrick other);

		/// <inheritdoc />
		public IParsedExpressionNode TryParse(ISourceCode sourceCode)
		{
			using (var ctx = sourceCode.GetFurtherContext())
			{
				var res = TryParseInternal(ctx);

				if (res != null)
				{
					ctx.Accept();
				}
				
				return res;
			}
		}

		/// <summary>Tries the parse internal.</summary>
		/// <param name="parsingContext">The source.</param>
		/// <returns></returns>
		protected abstract IParsedExpressionNode TryParseInternal(IParsingContextStream parsingContext);

	}
}