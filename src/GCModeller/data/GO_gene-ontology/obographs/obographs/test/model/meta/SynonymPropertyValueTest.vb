#Region "Microsoft.VisualBasic::1aa226aa905282d4db03c24b222828cc, ..\GCModeller\data\GO_gene-ontology\obographs\obographs\test\model\meta\SynonymPropertyValueTest.vb"

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

Namespace org.geneontology.obographs.model.meta



	Public Class SynonymPropertyValueTest

		Friend Shared synXrefs As String() = {"GOC:go_curators"}
		Friend Shared val As String = "cell nucleus"
		Friend Shared scope As org.geneontology.obographs.model.meta.SynonymPropertyValue.SCOPES = SynonymPropertyValue.SCOPES.EXACT

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			Dim spv As SynonymPropertyValue = build()
			testSyn(spv)
		End Sub

		Public Overridable Sub testSyn(ByVal spv As SynonymPropertyValue)
			assertEquals(val, spv.Val)
		End Sub

		Public Shared Function build() As SynonymPropertyValue
			Return (New SynonymPropertyValue.Builder).val(val).scope(scope).xrefs(java.util.Arrays.asList(synXrefs)).build()
		End Function

	End Class

End Namespace
