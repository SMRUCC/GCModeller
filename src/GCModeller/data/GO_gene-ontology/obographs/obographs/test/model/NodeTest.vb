Imports org.junit.Assert

Namespace org.geneontology.obographs.model


	Public Class NodeTest

		Friend Shared id As String = "GO:0005634"
		Friend Shared lbl As String = "nucleus"

		Friend Shared parent_id As String = "GO:0005623"
		Friend Shared parent_lbl As String = "cell"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			Dim n As org.geneontology.obographs.model.Node = build()
			Dim p As org.geneontology.obographs.model.Node = buildParent()
			assertEquals(id, n.Id)
			assertEquals(lbl, n.Label)
			assertEquals(parent_id, p.Id)
			assertEquals(parent_lbl, p.Label)

			Dim m As org.geneontology.obographs.model.Meta = MetaTest.build()
			MetaTest.testMeta(m)
			assertNull(p.Meta)
		End Sub

		Public Shared Function build() As org.geneontology.obographs.model.Node
			Dim m As org.geneontology.obographs.model.Meta = MetaTest.build()
			Return (New org.geneontology.obographs.model.Node.Builder).id(id).label(lbl).meta(m).build()
		End Function

		Public Shared Function buildParent() As org.geneontology.obographs.model.Node
			Dim m As org.geneontology.obographs.model.Meta = MetaTest.build()
			Return (New org.geneontology.obographs.model.Node.Builder).id(parent_id).label(parent_lbl).build()
		End Function

	End Class

End Namespace