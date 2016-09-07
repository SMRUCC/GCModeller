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