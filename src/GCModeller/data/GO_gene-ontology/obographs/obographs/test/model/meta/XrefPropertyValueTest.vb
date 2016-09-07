Imports org.junit.Assert

Namespace org.geneontology.obographs.model.meta



	Public Class XrefPropertyValueTest

		Public Shared val As String = "ICD10:111"
		Public Shared lbl As String = "foo disease"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			Dim spv As XrefPropertyValue = build()
			testSyn(spv)
		End Sub

		Public Overridable Sub testSyn(ByVal spv As XrefPropertyValue)
			assertEquals(val, spv.Val)
		End Sub

		Public Shared Function build() As XrefPropertyValue
			Return (New XrefPropertyValue.Builder).val(val).lbl(lbl).build()
		End Function

	End Class

End Namespace