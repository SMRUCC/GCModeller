Imports System.Collections.Generic

Namespace org.geneontology.obographs.model.meta




	Public MustInherit Class AbstractPropertyValue
		Implements PropertyValue



		Protected Friend Sub New(builder As Builder)
			pred = builder.pred
			val = builder.val
			meta = builder.meta
			xrefs = builder.xrefs
		End Sub

		Private ReadOnly pred As String
		Private ReadOnly val As String
		Private ReadOnly xrefs As IList(Of String)
		Private ReadOnly meta As org.geneontology.obographs.model.Meta






		''' <returns> the pred </returns>
		Public Overridable Property Pred As String Implements PropertyValue.getPred
			Get
				Return pred
			End Get
		End Property



		''' <returns> the xrefs </returns>
		Public Overridable Property Xrefs As IList(Of String) Implements PropertyValue.getXrefs
			Get
				Return xrefs
			End Get
		End Property



		''' <returns> the val </returns>
		Public Overridable Property Val As String Implements PropertyValue.getVal
			Get
				Return val
			End Get
		End Property



		''' <returns> the meta </returns>
		Public Overridable Property Meta As org.geneontology.obographs.model.Meta Implements PropertyValue.getMeta
			Get
				Return meta
			End Get
		End Property



		Public Class Builder

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___pred As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___val As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___meta As org.geneontology.obographs.model.Meta
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___xrefs As IList(Of String)

			Public Overridable Function pred(___pred As String) As Builder
				Me.___pred = ___pred
				Return Me
			End Function

			Public Overridable Function val(___val As String) As Builder
				Me.___val = ___val
				Return Me
			End Function
			Public Overridable Function xrefs(___xrefs As IList(Of String)) As Builder
				Me.___xrefs = ___xrefs
				Return Me
			End Function

			Public Overridable Function meta(___meta As org.geneontology.obographs.model.Meta) As Builder
				Me.___meta = ___meta
				Return Me
			End Function



		End Class

	End Class

End Namespace