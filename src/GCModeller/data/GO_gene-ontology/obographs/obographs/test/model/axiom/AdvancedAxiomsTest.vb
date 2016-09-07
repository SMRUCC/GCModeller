Imports System
Imports System.Collections.Generic
Imports org.junit.Assert

Namespace org.geneontology.obographs.model.axiom




	Public Class AdvancedAxiomsTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			Dim d As org.geneontology.obographs.model.GraphDocument = buildGD()

			Dim g As org.geneontology.obographs.model.Graph = d.Graphs(0)
			assertEquals(1,g.LogicalDefinitionAxioms.Count)

			Console.WriteLine(org.geneontology.obographs.io.OgJsonGenerator.render(d))
			Console.WriteLine(org.geneontology.obographs.io.OgYamlGenerator.render(d))
		End Sub

		Public Shared Function buildGD() As org.geneontology.obographs.model.GraphDocument
			Dim g As org.geneontology.obographs.model.Graph = buildGraph()
			Dim graphs As IList(Of org.geneontology.obographs.model.Graph) = CType(java.util.Collections.singletonList(g), IList(Of org.geneontology.obographs.model.Graph))

			Dim context As IDictionary(Of Object, Object) = New Dictionary(Of Object, Object)
			context("GO") = "http://purl.obolibrary.org/obo/GO_"

			Dim d As org.geneontology.obographs.model.GraphDocument = (New org.geneontology.obographs.model.GraphDocument.Builder).context(context).graphs(graphs).build()
			Return d
		End Function



		Public Shared Function buildGraph() As org.geneontology.obographs.model.Graph
			Dim n As org.geneontology.obographs.model.Node = org.geneontology.obographs.model.NodeTest.build()
			Dim e As org.geneontology.obographs.model.Edge = org.geneontology.obographs.model.EdgeTest.build()

			Dim nodes As IList(Of org.geneontology.obographs.model.Node) = CType(java.util.Collections.singletonList(n), IList(Of org.geneontology.obographs.model.Node))
			Dim edges As IList(Of org.geneontology.obographs.model.Edge) = CType(java.util.Collections.singletonList(e), IList(Of org.geneontology.obographs.model.Edge))
			Dim enss As IList(Of org.geneontology.obographs.model.axiom.EquivalentNodesSet) = java.util.Collections.singletonList(EquivalentNodesSetTest.build())
			Dim ldas As IList(Of LogicalDefinitionAxiom) = java.util.Collections.singletonList(LogicalDefinitionAxiomTest.build())
			Dim g As org.geneontology.obographs.model.Graph = (New org.geneontology.obographs.model.Graph.Builder).nodes(nodes).edges(edges).equivalentNodesSet(enss).logicalDefinitionAxioms(ldas).build()
			Return g
		End Function




	End Class

End Namespace