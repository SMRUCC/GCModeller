#Region "Microsoft.VisualBasic::b64686a7ed7cab09be047359fe0bdab5, data\GO_gene-ontology\obographs\test\DAGtest.vb"

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

    ' Module DAGtest
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Data.GeneOntology.obographs

Module DAGtest

    Sub Main()
        Dim obo As GO_OBO = GO_OBO.LoadDocument("P:\go.obo")
        Dim twocomponentSystem_genes As Index(Of String) = "E:\GCModeller\src\GCModeller\annotations\GSEA\data\xcb_TCS.txt".ReadAllLines.Select(Function(line) line.StringSplit("\s+").First).ToArray
        Dim proteins = UniProtXML.EnumerateEntries("E:\GCModeller\src\GCModeller\annotations\GSEA\data\uniprot-taxonomy_314565.XML") _
            .Where(Function(prot)
                       If Not prot.xrefs.ContainsKey("KEGG") Then
                           Return False
                       End If

                       Dim geneID = prot.xrefs("KEGG").First.id.Split(":"c).Last

                       Return geneID Like twocomponentSystem_genes
                   End Function).ToArray
        Dim terms = proteins.GoTermsFromUniProt.ToArray
        Dim DAG = obo.CreateGraph(terms)

        Call DAG.DAGasTabular.Save("./test.network/")
    End Sub
End Module
