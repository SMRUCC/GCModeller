#Region "Microsoft.VisualBasic::a5ef32ce206c57b8b870c5d548bbd9f7, data\GO_gene-ontology\obographs\obographs\test\model\EdgeTest.vb"

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

    ' 	Class EdgeTest
    ' 
    ' 	    Function: build
    ' 
    ' 	    Sub: test
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports org.junit.Assert

Namespace org.geneontology.obographs.model


	Public Class EdgeTest

		Friend Shared pred As String = "part_of"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			Dim e As org.geneontology.obographs.model.Edge = build()
			assertEquals(NodeTest.id, e.Sub)
			assertEquals(pred, e.Pred)
			assertEquals(NodeTest.parent_id, e.Obj)
		End Sub

		Public Shared Function build() As org.geneontology.obographs.model.Edge
			Return (New org.geneontology.obographs.model.Edge.Builder).sub(NodeTest.id).pred(pred).obj(NodeTest.parent_id).build()
		End Function

	End Class

End Namespace
