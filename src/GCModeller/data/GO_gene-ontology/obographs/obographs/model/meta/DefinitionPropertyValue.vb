#Region "Microsoft.VisualBasic::a1b71c9873536eff727c3d066804d950, data\GO_gene-ontology\obographs\obographs\model\meta\DefinitionPropertyValue.vb"

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

    ' 	Class DefinitionPropertyValue
    ' 
    ' 	    Constructor: (+1 Overloads) Sub New
    ' 		Class Builder
    ' 
    ' 		    Function: build, val, xrefs
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic

Namespace org.geneontology.obographs.model.meta




	''' <summary>
	''' A <seealso cref="PropertyValue"/> that represents a textual definition of an ontology class or
	''' property
	''' 
	''' @author cjm
	''' 
	''' </summary>
	Public Class DefinitionPropertyValue
		Inherits AbstractPropertyValue
		Implements PropertyValue

		Private Sub New(builder As org.geneontology.obographs.model.meta.AbstractPropertyValue.Builder)
			MyBase.New(builder)
		End Sub


		Public Class Builder
			Inherits AbstractPropertyValue.Builder

			Public Overridable Function val(___val As String) As org.geneontology.obographs.model.meta.AbstractPropertyValue.Builder
				Return CType(MyBase.val(___val), org.geneontology.obographs.model.meta.AbstractPropertyValue.Builder)
			End Function
			Public Overridable Function xrefs(___xrefs As IList(Of String)) As org.geneontology.obographs.model.meta.AbstractPropertyValue.Builder
				Return CType(MyBase.xrefs(___xrefs), org.geneontology.obographs.model.meta.AbstractPropertyValue.Builder)
			End Function

			Public Overridable Function build() As DefinitionPropertyValue
				Return New DefinitionPropertyValue(Me)
			End Function
		End Class

	End Class

End Namespace
