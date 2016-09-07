Imports System.Collections.Generic

Namespace org.geneontology.obographs.model.axiom




	''' <summary>
	''' Corresponds to an axiom of the form C = X1 and ... and Xn,
	''' Where X_i is either a named class or OWL Restriction
	''' 
	''' 
	''' @author cjm
	''' 
	''' </summary>
	Public Class LogicalDefinitionAxiom
		Inherits AbstractAxiom

		Private Sub New(builder As Builder)
			MyBase.New(builder)
			definedClassId = builder.definedClassId
			genusIds = builder.genusIds
			restrictions = builder.restrictions
		End Sub

		Private ReadOnly definedClassId As String
		Private ReadOnly genusIds As IList(Of String)
		Private ReadOnly restrictions As IList(Of ExistentialRestrictionExpression)



		''' <returns> the representativeNodeId </returns>
		Public Overridable Property DefinedClassId As String
			Get
				Return definedClassId
			End Get
		End Property



		''' <returns> the nodeIds </returns>
		Public Overridable Property GenusIds As IList(Of String)
			Get
				Return genusIds
			End Get
		End Property



		''' <returns> the restrictions </returns>
		Public Overridable Property Restrictions As IList(Of ExistentialRestrictionExpression)
			Get
				Return restrictions
			End Get
		End Property


		Public Class Builder
			Inherits AbstractAxiom.Builder

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___definedClassId As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___genusIds As IList(Of String)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___restrictions As IList(Of ExistentialRestrictionExpression)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private meta As org.geneontology.obographs.model.Meta

			Public Overridable Function definedClassId(___definedClassId As String) As Builder
				Me.___definedClassId = ___definedClassId
				Return Me
			End Function

			Public Overridable Function genusIds(___genusIds As IList(Of String)) As Builder
				Me.___genusIds = ___genusIds
				Return Me
			End Function

			Public Overridable Function restrictions(___restrictions As IList(Of ExistentialRestrictionExpression)) As Builder
				Me.___restrictions = ___restrictions
				Return Me
			End Function

			Public Overridable Function build() As LogicalDefinitionAxiom
				Return New LogicalDefinitionAxiom(Me)
			End Function
		End Class


	End Class

End Namespace