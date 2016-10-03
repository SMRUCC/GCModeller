#Region "Microsoft.VisualBasic::7ff22672eac5f8b8df6d814f7501846c, ..\GCModeller\data\GO_gene-ontology\obographs\obographs\test\model\GraphDocumentTest.vb"

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
