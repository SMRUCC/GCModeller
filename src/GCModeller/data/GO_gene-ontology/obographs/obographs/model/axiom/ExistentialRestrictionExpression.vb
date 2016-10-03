#Region "Microsoft.VisualBasic::767f555fd87c85ccb5cc9919ae24ae37, ..\GCModeller\data\GO_gene-ontology\obographs\obographs\model\axiom\ExistentialRestrictionExpression.vb"

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

Namespace org.geneontology.obographs.model.axiom




	''' <summary>
	''' Corresponds to an axiom of the form C = X1 and ... and Xn,
	''' Where X_i is either a named class or OWL Restriction
	''' 
	''' 
	''' @author cjm
	''' 
	''' </summary>
	Public Class ExistentialRestrictionExpression
		Inherits AbstractExpression

		Private Sub New(builder As Builder)
			MyBase.New(builder)
			fillerId = builder.fillerId
			propertyId = builder.propertyId
		End Sub

		Private ReadOnly propertyId As String
		Private ReadOnly fillerId As String



		''' <returns> the representativeNodeId </returns>
		Public Overridable Property FillerId As String
			Get
				Return fillerId
			End Get
		End Property




		''' <returns> the propertyId </returns>
		Public Overridable Property PropertyId As String
			Get
				Return propertyId
			End Get
		End Property




		Public Class Builder
			Inherits AbstractExpression.Builder

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___propertyId As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___fillerId As String

			Public Overridable Function propertyId(___propertyId As String) As Builder
				Me.___propertyId = ___propertyId
				Return Me
			End Function

			Public Overridable Function fillerId(___fillerId As String) As Builder
				Me.___fillerId = ___fillerId
				Return Me
			End Function

			Public Overridable Function build() As ExistentialRestrictionExpression
				Return New ExistentialRestrictionExpression(Me)
			End Function

		End Class


	End Class

End Namespace
