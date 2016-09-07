Imports System
Imports System.Collections.Generic
Imports org.junit.Assert

Namespace org.geneontology.obographs.model




	Public Class GraphDocumentTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			Dim d As org.geneontology.obographs.model.GraphDocument = build()
			Dim g As org.geneontology.obographs.model.Graph = d.Graphs(0)
			assertEquals(2, g.Nodes.Count)
			assertEquals(1, g.Edges.Count)
			assertEquals(1, d.Graphs.Count)

			Console.WriteLine(org.geneontology.obographs.io.OgJsonGenerator.render(d))
			Console.WriteLine(org.geneontology.obographs.io.OgYamlGenerator.render(d))
		End Sub

		Public Shared Function build() As org.geneontology.obographs.model.GraphDocument
			Dim g As org.geneontology.obographs.model.Graph = GraphTest.build()
			Dim graphs As IList(Of org.geneontology.obographs.model.Graph) = CType(java.util.Collections.singletonList(g), IList(Of org.geneontology.obographs.model.Graph))

			Dim context As IDictionary(Of Object, Object) = New Dictionary(Of Object, Object)
			context("GO") = "http://purl.obolibrary.org/obo/GO_"

			Dim d As org.geneontology.obographs.model.GraphDocument = (New org.geneontology.obographs.model.GraphDocument.Builder).context(context).graphs(graphs).build()
			Return d
		End Function



	End Class

End Namespace