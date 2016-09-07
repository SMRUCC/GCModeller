Imports System.Collections.Generic

Namespace org.geneontology.obographs.model.meta





	''' <summary>
	''' A <seealso cref="PropertyValue"/> that represents a an alternative term for a node
	''' 
	''' @author cjm
	''' 
	''' </summary>

	Public Class SynonymPropertyValue
		Inherits AbstractPropertyValue
		Implements PropertyValue

		''' <summary>
		''' OBO-style synonym scopes
		''' 
		''' @author cjm
		''' 
		''' </summary>
		Public Enum SCOPES
			EXACT
			NARROW
			BROAD
			RELATED
		End Enum

		''' <summary>
		''' properties from oboInOwl vocabulary that represent scopes
		''' 
		''' @author cjm
		''' 
		''' </summary>
		Public Enum PREDS
			hasExactSynonym
			hasNarrowSynonym
			hasBroadSynonym
			hasRelatedSynonym
		End Enum

		Private Sub New(builder As org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder)
			MyBase.New(builder)
		End Sub

		''' <returns> true is scope equals EXACT -- convenience predicate </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property Exact As Boolean
			Get
				Return Pred.Equals(PREDS.hasExactSynonym.ToString())
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property Types As IList(Of String)
			Get
				If Meta IsNot Nothing Then Return Meta.Subsets
				Return New List(Of )
			End Get
		End Property


		Public Class Builder
			Inherits AbstractPropertyValue.Builder

			Public Overrides Function val(___val As String) As org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder
				Return CType(MyBase.val(___val), org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder)
			End Function

			Public Overrides Function xrefs(___xrefs As IList(Of String)) As org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder
				Return CType(MyBase.xrefs(___xrefs), org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder)
			End Function

			Public Overridable Function addType(type As String) As org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder
				' TODO: decide on pattern for nested builders
				MyBase.meta((New org.geneontology.obographs.model.Meta.Builder).subsets(java.util.Collections.singletonList(type)).build())
				Return Me
			End Function

			Public Overridable Function scope(___scope As SCOPES) As org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder
				Dim pred As PREDS = PREDS.hasRelatedSynonym
				Select Case ___scope
				Case SCOPES.EXACT
					pred = PREDS.hasExactSynonym
				Case SCOPES.RELATED
					pred = PREDS.hasRelatedSynonym
				Case SCOPES.BROAD
					pred = PREDS.hasBroadSynonym
				Case SCOPES.NARROW
					pred = PREDS.hasNarrowSynonym

				End Select
				MyBase.pred(pred.ToString())
				Return Me

			End Function

			Public Overridable Function build() As SynonymPropertyValue
				Return New SynonymPropertyValue(Me)
			End Function
		End Class

	End Class

End Namespace