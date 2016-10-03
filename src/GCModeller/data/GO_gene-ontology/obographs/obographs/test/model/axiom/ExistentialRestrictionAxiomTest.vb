#Region "Microsoft.VisualBasic::af647ef882644caca8689d431dd61c6a, ..\GCModeller\data\GO_gene-ontology\obographs\obographs\test\model\axiom\ExistentialRestrictionAxiomTest.vb"

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

Namespace org.geneontology.obographs.model.axiom




	Public Class ExistentialRestrictionAxiomTest

		Private Const REL As String = "part_of"
		Private Const FILLER As String = "Z:1"


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			Dim r As ExistentialRestrictionExpression = build()
			assertEquals(REL, r.PropertyId)
			assertEquals(FILLER, r.FillerId)
		End Sub



		Public Shared Function build() As ExistentialRestrictionExpression

			Return (New ExistentialRestrictionExpression.Builder).propertyId(REL).fillerId(FILLER).build()

		End Function


	End Class

End Namespace
