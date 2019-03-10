Imports System.Numerics

Namespace Utilities
	''' <summary>
	''' An internal helper class to convert types of arrays, primarily for data operations necessary for .NET types to/from R concepts.
	''' </summary>
	Friend NotInheritable Class RTypesUtil
		Private Sub New()
		End Sub
		Friend Shared Function SerializeComplexToDouble(values As Complex()) As Double()
			Dim data = New Double(2 * values.Length - 1) {}
			For i As Integer = 0 To values.Length - 1
				data(2 * i) = values(i).Real
				data(2 * i + 1) = values(i).Imaginary
			Next
			Return data
		End Function

		Friend Shared Function DeserializeComplexFromDouble(data As Double()) As Complex()
			Dim dblLen As Integer = data.Length
			If dblLen Mod 2 <> 0 Then
				Throw New ArgumentException("Serialized definition of complexes must be of even length")
			End If
			Dim n As Integer = dblLen \ 2
			Dim res = New Complex(n - 1) {}
			For i As Integer = 0 To n - 1
				res(i) = New Complex(data(2 * i), data(2 * i + 1))
			Next
			Return res
		End Function
	End Class
End Namespace
