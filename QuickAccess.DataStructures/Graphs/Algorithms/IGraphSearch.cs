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
// Project: QuickAccess.DataStructures
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com

#endregion

using System.Diagnostics.Contracts;
using QuickAccess.DataStructures.Graphs.Model;

namespace QuickAccess.DataStructures.Graphs.Algorithms
{
	/// <summary>
	/// The interface of the graph search algorithm where the edges are not weighted.
	/// <seealso cref="IWeightedEdgesGraphSearch"/>
	/// </summary>
	public interface IGraphSearch
	{
		/// <summary>Gets the search map from the specified start vertex.</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="graph">The graph connectivity.</param>
		/// <param name="startVertexIndex">The index of the start vertex.</param>
		/// <param name="filterAdjacentVerticesCallback">
		/// The filter adjacent vertices callback, it returns sequence of indexes of adjacent (destination) vertices.
		/// If <c>null</c> it selects all adjacent vertices (no filtering).
		/// </param>
		/// <returns>The graph search map from the specified start vertex.
		/// </returns>
		[Pure]
		VertexSearchMap<int> GetSearchMapFrom<TEdgeData>(
			GraphConnectivityDefinition<TEdgeData> graph,
			int startVertexIndex,
			FilterAdjacentVerticesCallback<TEdgeData> filterAdjacentVerticesCallback = null);
	}
}