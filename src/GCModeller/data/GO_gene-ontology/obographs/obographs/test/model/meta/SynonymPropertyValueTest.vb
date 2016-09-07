Imports org.junit.Assert

Namespace org.geneontology.obographs.model.meta



	Public Class SynonymPropertyValueTest

		Friend Shared synXrefs As String() = {"GOC:go_curators"}
		Friend Shared val As String = "cell nucleus"
		Friend Shared scope As org.geneontology.obographs.model.meta.SynonymPropertyValue.SCOPES = SynonymPropertyValue.SCOPES.EXACT

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			Dim spv As SynonymPropertyValue = build()
			testSyn(spv)
		End Sub

		Public Overridable Sub testSyn(ByVal spv As SynonymPropertyValue)
			assertEquals(val, spv.Val)
		End Sub

		Public Shared Function build() As SynonymPropertyValue
			Return (New SynonymPropertyValue.Builder).val(val).scope(scope).xrefs(java.util.Arrays.asList(synXrefs)).build()
		End Function

	End Class

End Namespace