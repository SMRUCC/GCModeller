#Region "Microsoft.VisualBasic::b094dabc520c82458e624feb52db6435, ..\GCModeller\data\GO_gene-ontology\obographs\obographs\model\axiom\EquivalentNodesSet.vb"

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
	''' A set of nodes that all stand in a mutual equivalence or identity relationship to one another
	''' 
	''' Corresponds to Node in the OWLAPI
	''' 
	''' 
	''' @author cjm
	''' 
	''' </summary>
	Public Class EquivalentNodesSet
		Inherits AbstractAxiom

		Private Sub New(builder As Builder)
			MyBase.New(builder)
			representativeNodeId = builder.representativeNodeId
			nodeIds = builder.nodeIds
		End Sub

		Private ReadOnly representativeNodeId As String
		Private ReadOnly nodeIds As java.util.Set(Of String)



		''' <returns> the representativeNodeId </returns>
		Public Overridable Property RepresentativeNodeId As String
			Get
				Return representativeNodeId
			End Get
		End Property



		''' <returns> the nodeIds </returns>
		Public Overridable Property NodeIds As java.util.Set(Of String)
			Get
				Return nodeIds
			End Get
		End Property



		Public Class Builder
			Inherits AbstractAxiom.Builder

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___representativeNodeId As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___nodeIds As java.util.Set(Of String)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private meta As org.geneontology.obographs.model.Meta

			Public Overridable Function representativeNodeId(___representativeNodeId As String) As Builder
				Me.___representativeNodeId = ___representativeNodeId
				Return Me
			End Function

			Public Overridable Function nodeIds(___nodeIds As java.util.Set(Of String)) As Builder
				Me.___nodeIds = ___nodeIds
				Return Me
			End Function

			Public Overridable Function build() As EquivalentNodesSet
				Return New EquivalentNodesSet(Me)
			End Function
		End Class


	End Class

End Namespace
