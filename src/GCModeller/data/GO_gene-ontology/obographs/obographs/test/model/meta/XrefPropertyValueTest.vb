#Region "Microsoft.VisualBasic::91f094ff7b96ac541aed59524c7c21ed, data\GO_gene-ontology\obographs\obographs\test\model\meta\XrefPropertyValueTest.vb"

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

    ' 	Class XrefPropertyValueTest
    ' 
    ' 	    Function: build
    ' 
    ' 	    Sub: test, testSyn
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports org.junit.Assert

Namespace org.geneontology.obographs.model.meta



	Public Class XrefPropertyValueTest

		Public Shared val As String = "ICD10:111"
		Public Shared lbl As String = "foo disease"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			Dim spv As XrefPropertyValue = build()
			testSyn(spv)
		End Sub

		Public Overridable Sub testSyn(ByVal spv As XrefPropertyValue)
			assertEquals(val, spv.Val)
		End Sub

		Public Shared Function build() As XrefPropertyValue
			Return (New XrefPropertyValue.Builder).val(val).lbl(lbl).build()
		End Function

	End Class

End Namespace
