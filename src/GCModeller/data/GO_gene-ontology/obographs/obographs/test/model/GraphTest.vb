#Region "Microsoft.VisualBasic::5aff2fe3ecb58277ed81439114ebf10b, data\GO_gene-ontology\obographs\obographs\test\model\GraphTest.vb"

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

    ' 	Class GraphTest
    ' 
    ' 	    Function: build
    ' 
    ' 	    Sub: test
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
