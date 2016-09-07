Imports System.Collections.Generic
Imports org.junit.Assert

Namespace org.geneontology.obographs.model.axiom




	Public Class LogicalDefinitionAxiomTest

		Private Const DC As String = "A:1"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			Dim lda As LogicalDefinitionAxiom = build()
			assertEquals(DC, lda.DefinedClassId)
		End Sub



		Public Shared Function build() As LogicalDefinitionAxiom
			Dim ids As String() = {"X:1", "X:2"}
			Dim nodeIds As IList(Of String) = New List(Of String)(java.util.Arrays.asList(ids))
			Dim rs As IList(Of ExistentialRestrictionExpression) = java.util.Collections.singletonList(ExistentialRestrictionAxiomTest.build())
			Return (New LogicalDefinitionAxiom.Builder).definedClassId(DC).genusIds(nodeIds).restrictions(rs).build()

		End Function


	End Class

End Namespace