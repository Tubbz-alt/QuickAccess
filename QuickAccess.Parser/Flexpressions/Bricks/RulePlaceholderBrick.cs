﻿#region LICENSE [BSD-2-Clause]
// This code is distributed under the BSD-2-Clause license.
// =====================================================================
// 
// Copyright ©2020 by Kamil Piotr Kaczorek
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
using QuickAccess.Infrastructure.Freezable;
using QuickAccess.Infrastructure.RegularExpression;
using QuickAccess.Infrastructure.ValueContract;
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser.Flexpressions.Bricks
{
	public sealed class RulePlaceholderBrick : FlexpressionBrick, ICanBeReadOnly
	{
		private readonly FreezableValue<Tuple<FlexpressionBrick, bool>> _rule;

		public string RuleName { get; }
		/// <inheritdoc />
		public override string Name => RuleName;
		public FlexpressionBrick Content => _rule.IsDefined ? _rule.Value.Item1 : null;
		public bool IsRecursion => _rule.IsDefined && _rule.Value.Item2;
		public bool IsReadOnly => _rule.IsReadOnly;

		public RulePlaceholderBrick(IFlexpressionAlgebra algebra, string ruleName)
		: this(algebra, ruleName, null)
		{
		}

		public RulePlaceholderBrick(IFlexpressionAlgebra algebra, string ruleName, FlexpressionBrick defaultRule)
		: base(algebra.GetHighestPrioritizedAlgebra(defaultRule))
		{
			RuleName = ruleName;
			_rule = defaultRule != null ? new FreezableValue<Tuple<FlexpressionBrick, bool>>(Tuple.Create(defaultRule, false)) : new FreezableValue<Tuple<FlexpressionBrick, bool>>();
		}

        /// <inheritdoc />
        protected override void ApplyRuleDefinition(
            string name,
            FlexpressionBrick content,
            bool recursion,
            bool freeze)
        {
            if (name == RuleName && _rule.TrySet(content, recursion))
            {
                if (freeze)
                {
                    _rule.Freeze();
                }
            }
        }

        /// <inheritdoc />
		public override bool Equals(FlexpressionBrick other)
		{
			if (IsEmpty && (other?.IsEmpty ?? false))
			{
				return true;
			}

			return other is RulePlaceholderBrick cb && RuleName.Equals(cb.RuleName);
		}

		/// <inheritdoc />
		protected override IParsingProduct TryParseInternal(IParsingContextStream ctx, ParsingOptions options)
		{
			return Content.TryParse(ctx, options);
		}

        /// <inheritdoc />
		public override string ToRegularExpressionString(RegularExpressionBuildingContext  ctx)
		{
			if (!_rule.IsDefined)
			{
				throw new InvalidOperationException($"Rule is not defined for this placeholder. Rule name={RuleName}");
			}

			return IsRecursion ? ctx.Factory.CreateRecursiveGroupCall(ctx.Context, RuleName) : Content.ToRegularExpressionString(ctx);
		}

		/// <inheritdoc />
		public override MatchingLevel RegularExpressionMatchingLevel =>
			IsRecursion ? MatchingLevel.Exact : Content?.RegularExpressionMatchingLevel ?? MatchingLevel.None;

		/// <inheritdoc />
		public override string ToString()
		{
			return IsRecursion ? RuleName : Content?.ToString() ?? RuleName;
		}
	}
}