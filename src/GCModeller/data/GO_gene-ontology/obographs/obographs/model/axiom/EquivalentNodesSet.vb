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