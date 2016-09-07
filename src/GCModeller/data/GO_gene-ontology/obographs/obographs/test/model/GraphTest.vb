Imports System
Imports System.Collections.Generic
Imports org.junit.Assert

Namespace org.geneontology.obographs.model




	Public Class GraphTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			Dim g As org.geneontology.obographs.model.Graph = build()
			assertEquals(2, g.Nodes.Count)
			assertEquals(1, g.Edges.Count)

			Console.WriteLine(org.geneontology.obographs.io.OgJsonGenerator.render(g))
			Console.WriteLine(org.geneontology.obographs.io.OgYamlGenerator.render(g))
		End Sub

		Public Shared Function build() As org.geneontology.obographs.model.Graph
			Dim n As org.geneontology.obographs.model.Node = NodeTest.build()
			Dim p As org.geneontology.obographs.model.Node = NodeTest.buildParent()
			Dim e As org.geneontology.obographs.model.Edge = EdgeTest.build()

			Dim nodes As IList(Of org.geneontology.obographs.model.Node) = New List(Of org.geneontology.obographs.model.Node)
			nodes.Add(n)
			nodes.Add(p)
			Dim edges As IList(Of org.geneontology.obographs.model.Edge) = CType(java.util.Collections.singletonList(e), IList(Of org.geneontology.obographs.model.Edge))
			Dim g As org.geneontology.obographs.model.Graph = (New org.geneontology.obographs.model.Graph.Builder).nodes(nodes).edges(edges).build()
			Return g
		End Function



	End Class

End Namespace