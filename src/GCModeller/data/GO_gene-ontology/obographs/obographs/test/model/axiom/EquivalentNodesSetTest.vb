Imports System.Collections.Generic
Imports org.junit.Assert

Namespace org.geneontology.obographs.model.axiom




	Public Class EquivalentNodesSetTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			Dim ens As org.geneontology.obographs.model.axiom.EquivalentNodesSet = build()
			assertEquals(2, ens.NodeIds.size())
		End Sub



		Public Shared Function build() As org.geneontology.obographs.model.axiom.EquivalentNodesSet
			Dim ids As String() = {"X:1", "X:2"}
			Dim nodeIds As java.util.Set(Of String) = New HashSet(Of String)(java.util.Arrays.asList(ids))
			Return (New org.geneontology.obographs.model.axiom.EquivalentNodesSet.Builder).nodeIds(nodeIds).representativeNodeId(ids(0)).build()

		End Function


	End Class

End Namespace