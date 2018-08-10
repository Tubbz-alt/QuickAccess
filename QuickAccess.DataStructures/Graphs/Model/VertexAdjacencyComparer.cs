#region LICENSE [BSD-2-Clause]

// This code is distributed under the BSD-2-Clause license.
// =====================================================================
// 
// Copyright �2018 by Kamil Piotr Kaczorek
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
// Project: QuickAccess.DataStructures
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com

#endregion

using System.Collections.Generic;
using System.Linq;
using QuickAccess.DataStructures.Common;

namespace QuickAccess.DataStructures.Graphs.Model
{
	/// <summary> 
	/// Static factory of instances of <see cref="VertexAdjacencyComparer{TEdgeData}"/> type.
	/// </summary>
	public static class VertexAdjacencyComparer
	{

		/// <summary>Gets the empty edge vertex comparer.</summary>
		/// <value>The empty edge vertex comparer.</value>
		public static VertexAdjacencyComparer<EmptyValue> EmptyEdgeComparer => VertexAdjacencyComparer<EmptyValue>.Default;

		/// <summary>Creates the vertex comparer with specified equality comparer of edge data.</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="edgeDataEqualityComparer">The equality comparer of edge data, if <c>null</c> default comparer will be used.</param>
		/// <returns>Comparer instance.</returns>
		public static VertexAdjacencyComparer<TEdgeData> Create<TEdgeData>(
			IEqualityComparer<TEdgeData> edgeDataEqualityComparer)
		{
			if (edgeDataEqualityComparer == null || EmptyValue.IsEmptyValueType<TEdgeData>())
			{
				return VertexAdjacencyComparer<TEdgeData>.Default;
			}

			return new VertexAdjacencyComparer<TEdgeData>(edgeDataEqualityComparer);
		}		
	}

	/// <summary>
	/// The comparer of instances of <see cref="VertexAdjacency{TEdgeData}"/> type.
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	public sealed class VertexAdjacencyComparer<TEdgeData>
		: IEqualityComparer<VertexAdjacency<TEdgeData>>, IComparer<VertexAdjacency<TEdgeData>>
	{
		public static readonly VertexAdjacencyComparer<TEdgeData> Default = new VertexAdjacencyComparer<TEdgeData>(null);
		public readonly IEqualityComparer<TEdgeData> EdgeDataEqualityComparer;

		/// <summary>Initializes a new instance of the <see cref="VertexAdjacencyComparer{TEdgeData}"/> class.</summary>
		/// <param name="edgeDataEqualityComparer">The edge data equality comparer.</param>
		public VertexAdjacencyComparer(IEqualityComparer<TEdgeData> edgeDataEqualityComparer)
		{
			if (EmptyValue.IsEmptyValueType<TEdgeData>())
			{
				return;
			}

			EdgeDataEqualityComparer = edgeDataEqualityComparer ?? EqualityComparer<TEdgeData>.Default;
		}


		/// <summary>
		/// Compares two specified vertices applying following rules:
		/// - defined instance is smaller than <c>null</c>.
		/// - when both instances are defined, the instance with higher number of edges is smaller.
		/// - when both instances have the same number of edges - returns the comparison of hash codes 
		///		(calculated with <see cref="GetHashCode"/> method).
		/// <remarks>
		/// This method is used by <see cref="GraphConnectivityDefinition.ToCompacted{TEdgeData}"/> method to build optimized
		/// graph connectivity structure.
		/// </remarks>
		/// </summary>
		/// <param name="x">The first vertex.</param>
		/// <param name="y">The second vertex.</param>
		/// <returns>Comparison result.</returns>
		public int Compare(VertexAdjacency<TEdgeData> x, VertexAdjacency<TEdgeData> y)
		{
			if (ReferenceEquals(x, y))
			{
				return 0;
			}

			if (x == null || y == null)
			{
				return x == null ? 1 : -1;
			}

			var cmp = y.EdgesCount.CompareTo(x.EdgesCount);

			if (cmp != 0)
			{
				return cmp;
			}

			if (x.Count <= 0)
			{
				return 0;
			}

			cmp = GetHashCode(x).CompareTo(GetHashCode(y));

			return cmp;
		}

		/// <summary>
		/// Determines whether the specified vertices are equal.
		/// The vertices are equal if references are equal or if both vertices have the same number of adjacent edges and the second vertex
		/// contains all edges of the first one.
		/// Two edges are equal if destination vertices are equal and execution of <see cref=" IEqualityComparer{T}.Equals(T,T)"/> 
		/// method for edges data of <see cref="EdgeDataEqualityComparer"/> returns <c>true</c>.
		/// <remarks>
		/// This method is used in conjunction with the <see cref="GraphConnectivityDefinition.ToCompactedWithSharedVertexInstances{TEdgeData}(GraphConnectivityDefinition{TEdgeData},IEqualityComparer{TEdgeData},Factory.IVertexAdjacencyFactory{TEdgeData})"/>
		/// to reuse vertex instance for equivalent vertices.
		/// </remarks>
		/// </summary>
		/// <param name="x">The first vertex to compare.</param>
		/// <param name="y">The second vertex to compare.</param>
		/// <returns><c>true</c> if the specified vertices are equal; otherwise, <c>false</c>.</returns>
		public bool Equals(VertexAdjacency<TEdgeData> x, VertexAdjacency<TEdgeData> y)
		{
			return x.IsEquivalent(y, EdgeDataEqualityComparer);
		}

		/// <inheritdoc />
		public int GetHashCode(VertexAdjacency<TEdgeData> obj)
		{
			unchecked
			{
				return (obj.HasEmptyEdgeData()
						? obj.AdjacentIndexes.MultiplyEachItemBy(23)
						: obj.Select(e => (e.Destination * 23) ^ EdgeDataEqualityComparer.GetHashCode(e.Data))
					)
					.XOr(obj.EdgesCount);
			}
		}
	}
}