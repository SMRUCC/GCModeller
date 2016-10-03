#Region "Microsoft.VisualBasic::e725a2194427256e902ae95cd1544d9e, ..\GCModeller\data\GO_gene-ontology\obographs\obographs\test\model\NodeTest.vb"

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

Imports org.junit.Assert

Namespace org.geneontology.obographs.model


	Public Class NodeTest

		Friend Shared id As String = "GO:0005634"
		Friend Shared lbl As String = "nucleus"

		Friend Shared parent_id As String = "GO:0005623"
		Friend Shared parent_lbl As String = "cell"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			Dim n As org.geneontology.obographs.model.Node = build()
			Dim p As org.geneontology.obographs.model.Node = buildParent()
			assertEquals(id, n.Id)
			assertEquals(lbl, n.Label)
			assertEquals(parent_id, p.Id)
			assertEquals(parent_lbl, p.Label)

			Dim m As org.geneontology.obographs.model.Meta = MetaTest.build()
			MetaTest.testMeta(m)
			assertNull(p.Meta)
		End Sub

		Public Shared Function build() As org.geneontology.obographs.model.Node
			Dim m As org.geneontology.obographs.model.Meta = MetaTest.build()
			Return (New org.geneontology.obographs.model.Node.Builder).id(id).label(lbl).meta(m).build()
		End Function

		Public Shared Function buildParent() As org.geneontology.obographs.model.Node
			Dim m As org.geneontology.obographs.model.Meta = MetaTest.build()
			Return (New org.geneontology.obographs.model.Node.Builder).id(parent_id).label(parent_lbl).build()
		End Function

	End Class

End Namespace
