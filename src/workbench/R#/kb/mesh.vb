#Region "Microsoft.VisualBasic::268fdd3eefc38dc70137f4e015b2c1df, R#\kb\mesh.vb"

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


' Code Statistics:

'   Total Lines: 49
'    Code Lines: 40 (81.63%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 9 (18.37%)
'     File Size: 1.68 KB


' Module meshTools
' 
'     Function: loadMeshTree, loadMeshXml, mesh_category
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.MeSH
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.MeSH.Tree
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' The National Center For Biotechnology Information (NCBI) Medical Subject Headings (MeSH) 
''' system Is a comprehensive controlled vocabulary used For the indexing And cataloging Of 
''' scientific literature In the field Of biomedicine. Developed by the National Library Of 
''' Medicine (NLM), MeSH serves As a thesaurus that provides a consistent way To organize And 
''' retrieve information from the vast biomedical literature.
''' 
''' Here 's an introduction to the NCBI MeSH system:
''' 
''' 1. **Purpose**: The primary purpose Of MeSH Is To enable the efficient retrieval Of information from
'''      the PubMed database And other NLM databases. It helps researchers, healthcare professionals, And 
'''      the general Public find relevant articles And resources.
''' 2. **Structure**: MeSH Is structured hierarchically, with a tree-Like organization. It consists of 
'''      Descriptors (main headings), Qualifiers (subheadings), And Entry Terms (synonyms Or related terms). 
'''      Descriptors are organized into 16 main categories, known as Trees.
''' 3. **Descriptors**: Descriptors are the main indexing terms used To describe the subject Of an 
'''      article. Each Descriptor Is assigned a unique MeSH ID And can have multiple Entry Terms associated
'''      With it.
''' 4. **Trees**: The 16 main Trees in MeSH are: 
'''     - Anatomy
'''     - Organisms
'''     - Diseases
'''     - Chemicals And Drugs
'''     - Analytical, Diagnostic And Therapeutic Techniques And Equipment
'''     - Psychiatry And Psychology
'''     - Phenomena And Processes
'''     - Disciplines And Occupations
'''     - Anthropology, Education, Sociology, And Social Phenomena
'''     - Technology, Industry, And Agriculture
'''     - Humanities
'''     - Information Science
'''     - Named Groups
'''     - Health Care
'''     - Publication Characteristics
'''     - Geographicals
''' 5. **Qualifiers**: These are used To describe the specific aspects Of a Descriptor. For example, "Drug 
'''      Therapy" might be a Qualifier For the Descriptor "Hypertension."
''' 6. **Entry Terms**: These are synonyms Or closely related terms To the Descriptors. They help ensure that
'''      users can find relevant information even If they use different terminology.
''' 7. **Updates**: MeSH Is updated annually To reflect the evolving nature Of biomedical research. New 
'''      Descriptors, Qualifiers, And Entry Terms are added, while outdated terms are removed Or revised.
''' 8. **Search And Navigation**: Users can search For MeSH terms directly In the MeSH Browser Or use them 
'''      To refine searches In PubMed. The hierarchical Structure Of MeSH allows users To navigate from broad 
'''      To more specific topics.
''' 9. **Integration with PubMed**: MeSH terms are used To index articles In PubMed. When users search For a 
'''      specific topic, they can use MeSH terms To ensure they retrieve the most relevant And comprehensive 
'''      results.
''' </summary>
<Package("mesh")>
<RTypeExport("ncbi_mesh", GetType(MeSH.Tree.Term))>
Module meshTools

    Sub Main()

    End Sub

    ''' <summary>
    ''' Parse the ncbi pubmed mesh term dataset
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' the pubmed mesh term dataset could be download from:
    ''' 
    ''' https://nlmpubs.nlm.nih.gov/projects/mesh/MESH_FILES/xmlmesh/desc2024.zip
    ''' </remarks>
    <ExportAPI("read.mesh_xml")>
    Public Function loadMeshXml(file As String) As DescriptorRecord()
        Return DescriptorRecordSet.ReadTerms(file).ToArray
    End Function

    ''' <summary>
    ''' get mesh category values that assign to current mesh term
    ''' </summary>
    ''' <param name="term"></param>
    ''' <returns></returns>
    <ExportAPI("mesh_category")>
    Public Function mesh_category(term As MeSH.Tree.Term) As MeshCategory()
        Return term.category.ToArray
    End Function

    ''' <summary>
    ''' read the tree of mesh terms
    ''' </summary>
    ''' <param name="file">
    ''' the file path to the ncbi mesh term file, usually be the ``data/mtrees2024.txt``.
    ''' </param>
    ''' <param name="as_tree"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("read.mesh_tree")>
    <RApiReturn(GetType(MeSH.Tree.Term))>
    Public Function loadMeshTree(<RRawVectorArgument> file As Object,
                                 Optional as_tree As Boolean = True,
                                 Optional env As Environment = Nothing) As Object

        Dim s = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If

        Using buf As Stream = s.TryCast(Of Stream)
            If as_tree Then
                Return MeSH.Tree.ParseTree(buf)
            Else
                Return MeSH.Tree.ReadTerms(buf).ToArray
            End If
        End Using
    End Function

    ''' <summary>
    ''' build background model for enrichment based on ncbi mesh terms
    ''' </summary>
    ''' <param name="tree"></param>
    ''' <param name="category"></param>
    ''' <returns></returns>
    <ExportAPI("mesh_background")>
    Public Function convertMeshBackground(tree As Tree(Of MeSH.Tree.Term), category As MeshCategory, Optional levels As Integer = 1) As Background
        Dim root_terms = tree.Childs.Values _
            .Where(Function(n)
                       Return n.Data.category.Contains(category)
                   End Function) _
            .ToArray
        Dim clusters As New List(Of Cluster)

        For Each root As Tree(Of MeSH.Tree.Term) In root_terms
            For Each terms In root.ClusterTermByLevel(levels, category.Description & "->" & root.label)
                Dim cluster As New Cluster With {
                    .description = terms.category,
                    .ID = terms.cluster.Data.accessionID,
                    .names = terms.cluster.Data.term,
                    .members = terms.cluster _
                        .ClusterMembers _
                        .ToArray
                }

                Call clusters.Add(cluster)
            Next
        Next

        Return New Background With {
            .build = Now,
            .clusters = clusters.ToArray,
            .id = category.ToString,
            .name = category.Description,
            .size = .clusters.BackgroundSize
        }
    End Function

    <Extension>
    Private Iterator Function ClusterMembers(root As Tree(Of MeSH.Tree.Term)) As IEnumerable(Of BackgroundGene)
        Yield New BackgroundGene With {
            .name = root.label,
            .accessionID = root.Data.tree.First & root.Data.tree.Last
        }

        For Each node In root.Childs.SafeQuery.Select(Function(t) t.Value)
            For Each term In node.ClusterMembers
                Yield term
            Next
        Next
    End Function

    ''' <summary>
    ''' get cluster term in specific level
    ''' </summary>
    ''' <param name="root"></param>
    ''' <param name="level"></param>
    ''' <returns></returns>
    <Extension>
    Private Iterator Function ClusterTermByLevel(root As Tree(Of MeSH.Tree.Term), level As Integer, label As String) As IEnumerable(Of (category As String, cluster As Tree(Of MeSH.Tree.Term)))
        If level > 0 Then
            For Each term In root.Childs.Values
                For Each item In term.ClusterTermByLevel(level - 1, $"{label}->{term.label}")
                    Yield item
                Next
            Next
        Else
            ' current tree node is a cluster member
            Yield (label, root)
        End If
    End Function
End Module
