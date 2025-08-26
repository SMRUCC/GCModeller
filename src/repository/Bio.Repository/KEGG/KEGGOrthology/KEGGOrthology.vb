#Region "Microsoft.VisualBasic::2f345e393055d85b87375b071dc7a89c, Bio.Repository\KEGG\KEGGOrthology\KEGGOrthology.vb"

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

    '   Total Lines: 158
    '    Code Lines: 128 (81.01%)
    ' Comment Lines: 3 (1.90%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 27 (17.09%)
    '     File Size: 5.58 KB


    ' Class KEGGOrthology
    ' 
    '     Properties: Repository
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: __copy, __ko_genes_index, BuildLocus2KOIndex, EnumerateKO, FileCopy
    '               IndexSubMatch
    ' 
    '     Sub: __locus2KO, BuildLocusIndex
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
#If DEBUG Then
Imports Microsoft.VisualBasic.Serialization.JSON
#End If
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports Microsoft.VisualBasic.Data.Repository

Public Class KEGGOrthology

    Public ReadOnly Property Repository As String

    Sub New(DIR$)
        Repository = DIR
        __locus2KO()
    End Sub

    Dim locus2KO As New Dictionary(Of String, String())

    Private Function __ko_genes_index() As String
        Return (Repository & "/ko00001_genes_index.csv")
    End Function

    Private Sub __locus2KO()
        Dim package As IEnumerable(Of KO_gene) = __ko_genes_index().LoadCsv(Of KO_gene)
        Dim locus_tags = package.GroupBy(Function(kg) kg.sp_code & ":" & kg.gene)

        For Each gene As IGrouping(Of String, KO_gene) In locus_tags
            Dim k$() = gene _
                .Select(Function(g) g.ko) _
                .Distinct _
                .ToArray

            Call locus2KO.Add(gene.Key, k)
        Next
    End Sub

    ''' <summary>
    ''' 重新构建基因编号的索引
    ''' </summary>
    Public Sub BuildLocusIndex()
        Dim source As IEnumerable(Of KO_gene()) = BuildLocus2KOIndex(Repository & "/ko00001/")

        Using writer As New WriteStream(Of KO_gene)(__ko_genes_index)
            For Each block As KO_gene() In source
                Call writer.Flush(block)
            Next
        End Using
    End Sub

    Public Shared Iterator Function BuildLocus2KOIndex(DIR$) As IEnumerable(Of KO_gene())
        For Each xml As String In ls - l - r - "*.xml" <= DIR
            Dim ko As Orthology = xml.LoadXml(Of Orthology)

            If ko.Genes.IsNullOrEmpty Then
                Continue For
            End If

            Dim genes As KO_gene() = LinqAPI.Exec(Of KO_gene) <=
 _
                From gene As QueryEntry
                In ko.Genes
                Let desc = If(gene.description Is Nothing, "", gene.description)
                Let og = New KO_gene With {
                    .gene = gene.locusID,
                    .ko = ko.Entry,
                    .name = desc,
                    .sp_code = gene.speciesID,
                    .url = ""
                }
                Select og

            Yield genes
        Next
    End Function

    Public Shared Iterator Function FileCopy(source$, target$) As IEnumerable(Of String)
        Dim ko00001 As htext = htext.ko00001
        Dim sourcefiles As Dictionary(Of String, String) =
            source.LoadSourceEntryList("*.xml")

        For Each A As BriteHText In ko00001.Hierarchical.CategoryItems
            Dim parent As String = "/"

            For Each failure$ In __copy(sourcefiles, target, parent, A)
                Yield failure
            Next
        Next
    End Function

    Private Shared Iterator Function __copy(sourcefiles As Dictionary(Of String, String), target$, parents$, htext As BriteHText) As IEnumerable(Of String)
        If htext.CategoryItems.IsNullOrEmpty Then
            If htext.EntryId Is Nothing Then  ' 会出现空节点的情况
                Call htext.ToString.Warning
                Return
            End If

            If sourcefiles.ContainsKey(htext.EntryId) Then
                target &= parents & $"/{htext.EntryId}.xml"
                If Not sourcefiles(htext.EntryId).FileCopy(target) Then
                    Yield htext.EntryId
                End If
            Else
                Yield htext.EntryId
            End If
        Else
            parents &= (htext.ClassLabel.Split(":\/*?".ToCharArray).JoinBy("_")) & "/"

            For Each [sub] In htext.CategoryItems
                For Each failure$ In __copy(sourcefiles, target, parents, [sub])
                    Yield failure
                Next
            Next
        End If
    End Function

    Public Iterator Function EnumerateKO(locus_tag$) As IEnumerable(Of Orthology)
        Dim KOlist$() = If(
            locus2KO.ContainsKey(locus_tag), locus2KO(locus_tag), New String() {})

        For Each KO$ In KOlist
            Dim xml$ = $"{Repository}/ko00001/{KO}.xml"
            Dim o As Orthology = xml.LoadXml(Of Orthology)

            Yield o
        Next
    End Function

    Public Shared Iterator Function IndexSubMatch(blasthits As IEnumerable(Of Map(Of String, String)), index$) As IEnumerable(Of KO_gene)
        Using reader As New DataStream(index)
            Dim kegg_locus$() = blasthits _
                .Select(Function(h) h.Maps.ToLower) _
                .Distinct _
                .ToArray
            Dim maps As New Index(Of String)(kegg_locus)
            Dim i As Integer

            For Each gene As KO_gene In reader.AsLinq(Of KO_gene)
                If maps($"{gene.sp_code}:{gene.gene}".ToLower) > -1 Then
                    Yield gene
#If DEBUG Then
                    Call gene.GetJson.debug
#End If
                End If

                i += 1
            Next

            Call $"Index file iterates {i} gene lines...".debug
        End Using
    End Function
End Class
