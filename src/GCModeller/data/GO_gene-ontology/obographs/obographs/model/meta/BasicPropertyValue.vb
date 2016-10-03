#Region "Microsoft.VisualBasic::e85e807698c1321dbd49ceaf0a587e5f, ..\GCModeller\data\GO_gene-ontology\obographs\obographs\model\meta\BasicPropertyValue.vb"

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

Imports System.Collections.Generic

Namespace org.geneontology.obographs.model.meta




	''' <summary>
	''' A generic <seealso cref="PropertyValue"/> that is not explicitly modeled
	''' 
	''' @author cjm
	''' 
	''' </summary>
	Public Class BasicPropertyValue
		Inherits AbstractPropertyValue
		Implements PropertyValue

		Private Sub New(builder As Builder)
			MyBase.New(builder)
		End Sub


		Public Class Builder
			Inherits AbstractPropertyValue.Builder

			Public Overridable Function pred(___pred As String) As Builder
				Return CType(MyBase.pred(___pred), Builder)
			End Function
			Public Overridable Function val(___val As String) As Builder
				Return CType(MyBase.val(___val), Builder)
			End Function
			Public Overridable Function xrefs(___xrefs As IList(Of String)) As Builder
				Return CType(MyBase.xrefs(___xrefs), Builder)
			End Function


			Public Overridable Function build() As BasicPropertyValue
				Return New BasicPropertyValue(Me)
			End Function
		End Class

	End Class

End Namespace
