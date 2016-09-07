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