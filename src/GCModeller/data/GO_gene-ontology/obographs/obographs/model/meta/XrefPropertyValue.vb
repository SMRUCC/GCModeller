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