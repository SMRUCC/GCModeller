#Region "Microsoft.VisualBasic::642216446385ab87ea1007b78005e7ef, ..\GCModeller\data\GO_gene-ontology\obographs\obographs\model\meta\XrefPropertyValue.vb"

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




	Public Class XrefPropertyValue
		Inherits AbstractPropertyValue
		Implements PropertyValue

		Private ReadOnly lbl As String

		Private Sub New(builder As org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder)
			MyBase.New(builder)
			lbl = builder.lbl
		End Sub



		''' <returns> the lbl </returns>
		Public Overridable Property Lbl As String
			Get
				Return lbl
			End Get
		End Property



		Public Class Builder
			Inherits AbstractPropertyValue.Builder

			Private ___lbl As String

			Public Overrides Function val(___val As String) As org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder
				Return CType(MyBase.val(___val), org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder)
			End Function
			Public Overridable Function lbl(___lbl As String) As org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder
				Me.___lbl= ___lbl
				Return Me
			End Function

			Public Overrides Function xrefs(___xrefs As IList(Of String)) As org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder
				Return CType(MyBase.xrefs(___xrefs), org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder)
			End Function

			Public Overridable Function build() As XrefPropertyValue
				Return New XrefPropertyValue(Me)
			End Function
		End Class

	End Class

End Namespace
