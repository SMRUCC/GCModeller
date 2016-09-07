Imports System.Collections.Generic

Namespace org.geneontology.obographs.model.meta




	''' <summary>
	''' A generic <seealso cref="PropertyValue"/> that is not explicitly modeled
	''' 
	''' @author cjm
	''' 
	''' </summary>
	Public Class BasicPropertyValue
		Inherits AbstractPropertyValue
		Implements PropertyValue

		Private Sub New(builder As Builder)
			MyBase.New(builder)
		End Sub


		Public Class Builder
			Inherits AbstractPropertyValue.Builder

			Public Overridable Function pred(___pred As String) As Builder
				Return CType(MyBase.pred(___pred), Builder)
			End Function
			Public Overridable Function val(___val As String) As Builder
				Return CType(MyBase.val(___val), Builder)
			End Function
			Public Overridable Function xrefs(___xrefs As IList(Of String)) As Builder
				Return CType(MyBase.xrefs(___xrefs), Builder)
			End Function


			Public Overridable Function build() As BasicPropertyValue
				Return New BasicPropertyValue(Me)
			End Function
		End Class

	End Class

End Namespace