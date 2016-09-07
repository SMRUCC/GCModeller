Imports org.junit.Assert

Namespace org.geneontology.obographs.model.axiom




	Public Class ExistentialRestrictionAxiomTest

		Private Const REL As String = "part_of"
		Private Const FILLER As String = "Z:1"


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			Dim r As ExistentialRestrictionExpression = build()
			assertEquals(REL, r.PropertyId)
			assertEquals(FILLER, r.FillerId)
		End Sub



		Public Shared Function build() As ExistentialRestrictionExpression

			Return (New ExistentialRestrictionExpression.Builder).propertyId(REL).fillerId(FILLER).build()

		End Function


	End Class

End Namespace