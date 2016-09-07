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