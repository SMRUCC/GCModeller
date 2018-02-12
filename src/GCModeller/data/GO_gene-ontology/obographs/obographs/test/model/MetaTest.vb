#Region "Microsoft.VisualBasic::105dd1cc0d6eee2d7e1889907d486b7e, data\GO_gene-ontology\obographs\obographs\test\model\MetaTest.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' 	Class MetaTest
    ' 
    ' 	    Function: build
    ' 
    ' 	    Sub: test, testMeta
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
