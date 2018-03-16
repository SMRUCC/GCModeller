#Region "Microsoft.VisualBasic::5a9f6a50c82459bf93b1fdd27c10d437, data\GO_gene-ontology\obographs\obographs\model\Graph.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' 	Class Graph
    ' 
    ' 	    Properties: Edges, EquivalentNodesSets, Id, Lbl, LogicalDefinitionAxioms
    '                  Meta, Nodes
    ' 
    ' 	    Constructor: (+1 Overloads) Sub New
    ' 		Class Builder
    ' 
    ' 		    Function: build, edges, equivalentNodesSet, id, lbl
    '                 logicalDefinitionAxioms, meta, nodes
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic

Namespace org.geneontology.obographs.model




	''' <summary>
	''' A graph object holds a collection of nodes and edges
	''' 
	''' Corresponds to a Named Graph in RDF, and an Ontology in OWL
	''' 
	''' Note: there is no assumption that either nodes or edges are unique to a graph
	''' 
	''' ## Basic OBO Graphs
	''' 
	''' ![Node UML](node-bog.png)
	'''  * 
	''' @startuml node-bog.png
	''' class Graph
	''' class Node
	''' class Edge
	''' 
	''' Graph-->Node : 0..*
	''' Graph-->Edge : 0..*
	''' @enduml
	''' 
	''' 
	''' @author cjm
	''' 
	''' </summary>
	Public Class Graph

		Private Sub New(builder As Builder)
			id = builder.id
			lbl = builder.lbl
			meta = builder.meta
			nodes = builder.nodes
			edges = builder.edges
			equivalentNodesSets = builder.equivalentNodesSets
			logicalDefinitionAxioms = builder.logicalDefinitionAxioms
		End Sub

		Private ReadOnly nodes As IList(Of Node)
		Private ReadOnly edges As IList(Of Edge)
		Private ReadOnly id As String
		Private ReadOnly lbl As String
		Private ReadOnly meta As Meta
		Private ReadOnly equivalentNodesSets As IList(Of org.geneontology.obographs.model.axiom.EquivalentNodesSet)
		Private ReadOnly logicalDefinitionAxioms As IList(Of org.geneontology.obographs.model.axiom.LogicalDefinitionAxiom)



		''' <returns> the nodes </returns>
		Public Overridable Property Nodes As IList(Of Node)
			Get
				Return nodes
			End Get
		End Property



		''' <returns> the edges </returns>
		Public Overridable Property Edges As IList(Of Edge)
			Get
				Return edges
			End Get
		End Property



		''' <returns> the id </returns>
		Public Overridable Property Id As String
			Get
				Return id
			End Get
		End Property



		''' <returns> the lbl </returns>
		Public Overridable Property Lbl As String
			Get
				Return lbl
			End Get
		End Property



		''' <returns> the meta </returns>
		Public Overridable Property Meta As Meta
			Get
				Return meta
			End Get
		End Property



		''' <returns> the equivalentNodesSet </returns>
		Public Overridable Property EquivalentNodesSets As IList(Of org.geneontology.obographs.model.axiom.EquivalentNodesSet)
			Get
				Return equivalentNodesSets
			End Get
		End Property



		''' <returns> the logicalDefinitionAxioms </returns>
		Public Overridable Property LogicalDefinitionAxioms As IList(Of org.geneontology.obographs.model.axiom.LogicalDefinitionAxiom)
			Get
				Return logicalDefinitionAxioms
			End Get
		End Property



		Public Class Builder

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___id As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___lbl As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___meta As Meta
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___nodes As IList(Of Node)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___edges As IList(Of Edge)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private equivalentNodesSets As IList(Of org.geneontology.obographs.model.axiom.EquivalentNodesSet)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___logicalDefinitionAxioms As IList(Of org.geneontology.obographs.model.axiom.LogicalDefinitionAxiom)

			Public Overridable Function id(___id As String) As Builder
				Me.___id = ___id
				Return Me
			End Function

			Public Overridable Function lbl(___lbl As String) As Builder
				Me.___lbl = ___lbl
				Return Me
			End Function

			Public Overridable Function meta(___meta As Meta) As Builder
				Me.___meta = ___meta
				Return Me
			End Function

			' TODO: test for uniqueness
			Public Overridable Function nodes(___nodes As IList(Of Node)) As Builder
				Me.___nodes = ___nodes
				Return Me
			End Function
			Public Overridable Function edges(___edges As IList(Of Edge)) As Builder
				Me.___edges = ___edges
				Return Me
			End Function
			Public Overridable Function equivalentNodesSet(equivalentNodesSets As IList(Of org.geneontology.obographs.model.axiom.EquivalentNodesSet)) As Builder
				Me.equivalentNodesSets = equivalentNodesSets
				Return Me
			End Function
			Public Overridable Function logicalDefinitionAxioms(___logicalDefinitionAxioms As IList(Of org.geneontology.obographs.model.axiom.LogicalDefinitionAxiom)) As Builder
				Me.___logicalDefinitionAxioms = ___logicalDefinitionAxioms
				Return Me
			End Function

			Public Overridable Function build() As Graph
				Return New Graph(Me)
			End Function
		End Class

	End Class

End Namespace
