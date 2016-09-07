Imports org.junit.Assert

Namespace org.geneontology.obographs.model



	Public Class MetaTest

		Friend Shared defval As String = "A membrane-bounded organelle of eukaryotic cells in which chromosomes are housed and replicated. In most cells, the nucleus contains all of the cell's chromosomes except the organellar chromosomes, and is the site of RNA synthesis and processing. In some species, or in specialized cell types, RNA metabolism or DNA replication may be absent."
		Friend Shared defXrefs As String() = {"GOC:go_curators"}
		Friend Shared subsets As String() = {"goslim_yeast", "goslim_plant"}
		Friend Shared xrefVal As String = "Wikipedia:Cell_nucleus"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			Dim m As org.geneontology.obographs.model.Meta = build()
			testMeta(m)
		End Sub

		Public Shared Sub testMeta(ByVal m As org.geneontology.obographs.model.Meta)
			assertEquals(defval, m.Definition.Val)
			assertEquals(1, m.Definition.Xrefs.Count)
			assertEquals(2, m.Subsets.Count)
			assertEquals(1, m.Xrefs.Count)
			assertEquals(org.geneontology.obographs.model.meta.XrefPropertyValueTest.val, m.Xrefs(0).Val)
			assertEquals(org.geneontology.obographs.model.meta.XrefPropertyValueTest.lbl, m.Xrefs(0).Lbl)
		End Sub

		Public Shared Function build() As org.geneontology.obographs.model.Meta

			Dim s As org.geneontology.obographs.model.meta.SynonymPropertyValue = org.geneontology.obographs.model.meta.SynonymPropertyValueTest.build()
			Dim xref As org.geneontology.obographs.model.meta.XrefPropertyValue = org.geneontology.obographs.model.meta.XrefPropertyValueTest.build()
			Dim def As org.geneontology.obographs.model.meta.DefinitionPropertyValue = (New org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder).val(defval).xrefs(java.util.Arrays.asList(defXrefs)).build()
			Return (New org.geneontology.obographs.model.Meta.Builder).definition(def).synonyms(java.util.Collections.singletonList(s)).xrefs(java.util.Collections.singletonList(xref)).subsets(subsets).build()
		End Function

	End Class

End Namespace