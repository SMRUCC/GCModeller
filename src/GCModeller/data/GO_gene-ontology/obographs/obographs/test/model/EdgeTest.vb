Imports org.junit.Assert

Namespace org.geneontology.obographs.model


	Public Class EdgeTest

		Friend Shared pred As String = "part_of"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			Dim e As org.geneontology.obographs.model.Edge = build()
			assertEquals(NodeTest.id, e.Sub)
			assertEquals(pred, e.Pred)
			assertEquals(NodeTest.parent_id, e.Obj)
		End Sub

		Public Shared Function build() As org.geneontology.obographs.model.Edge
			Return (New org.geneontology.obographs.model.Edge.Builder).sub(NodeTest.id).pred(pred).obj(NodeTest.parent_id).build()
		End Function

	End Class

End Namespace