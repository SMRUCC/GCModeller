#Region "Microsoft.VisualBasic::5e7bd227c5b1d96fe60da567062576c5, data\GO_gene-ontology\obographs\obographs\test\model\axiom\LogicalDefinitionAxiomTest.vb"

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

    ' 	Class LogicalDefinitionAxiomTest
    ' 
    ' 	    Function: build
    ' 
    ' 	    Sub: test
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports org.junit.Assert

Namespace org.geneontology.obographs.model.axiom




	Public Class LogicalDefinitionAxiomTest

		Private Const DC As String = "A:1"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			Dim lda As LogicalDefinitionAxiom = build()
			assertEquals(DC, lda.DefinedClassId)
		End Sub



		Public Shared Function build() As LogicalDefinitionAxiom
			Dim ids As String() = {"X:1", "X:2"}
			Dim nodeIds As IList(Of String) = New List(Of String)(java.util.Arrays.asList(ids))
			Dim rs As IList(Of ExistentialRestrictionExpression) = java.util.Collections.singletonList(ExistentialRestrictionAxiomTest.build())
			Return (New LogicalDefinitionAxiom.Builder).definedClassId(DC).genusIds(nodeIds).restrictions(rs).build()

		End Function


	End Class

End Namespace
