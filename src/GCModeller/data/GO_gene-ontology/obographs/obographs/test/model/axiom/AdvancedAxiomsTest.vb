#Region "Microsoft.VisualBasic::b31ffb18a7844ed87e6c29d679e28161, ..\GCModeller\data\GO_gene-ontology\obographs\obographs\test\model\axiom\AdvancedAxiomsTest.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

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
