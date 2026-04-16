#Region "Microsoft.VisualBasic::f49f7bc8d6bb8264d80ae0c63245bdc2, localblast\LocalBLAST\Pipeline\GenomeVector.vb"

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

    '   Total Lines: 173
    '    Code Lines: 112 (64.74%)
    ' Comment Lines: 35 (20.23%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 26 (15.03%)
    '     File Size: 7.44 KB


    '     Class GenomeVector
    ' 
    '         Properties: assembly_id, size, taxonomy, terms
    ' 
    '         Function: CreateVectors, GetHierarchicalECNumberTerms, groupByAssembly, GroupByTaxonomy, streamGroupByAssembly
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace Pipeline

    ''' <summary>
    ''' 基因组代谢酶注释结果
    ''' </summary>
    Public Class GenomeVector : Implements INamedValue

        ''' <summary>
        ''' the unique reference id of the genome, usually is the genbank assembly id, but it can be any string that can uniquely 
        ''' identify this genome in the dataset, and it should be consistent with the assembly id used in the annotation result, 
        ''' so that we can link the annotation result to the taxonomy information of this genome
        ''' </summary>
        ''' <returns></returns>
        Public Property assembly_id As String Implements INamedValue.Key
        ''' <summary>
        ''' the genome taxonomy information, it can be the taxonomic name of this genome, or the taxonomic id of this genome, 
        ''' but it should be consistent with the taxonomy information used in the annotation result, so that we can link the 
        ''' annotation result to the taxonomy information of this genome
        ''' </summary>
        ''' <returns></returns>
        Public Property taxonomy As String
        ''' <summary>
        ''' annotated terms inside this genome, the key is the term name, and the value is the count of this term in this genome
        ''' </summary>
        ''' <returns></returns>
        Public Property terms As Dictionary(Of String, Integer)

        ''' <summary>
        ''' annotated gene count
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property size As Integer
            Get
                If terms Is Nothing Then
                    Return 0
                Else
                    Return terms.Values.Sum
                End If
            End Get
        End Property

        ''' <summary>
        ''' used for processing of the ec number terms, make the count of the
        ''' hierarchical ec number terms by summing the count of the specific 
        ''' ec number terms
        ''' </summary>
        ''' <returns></returns>
        Public Function GetHierarchicalECNumberTerms() As Dictionary(Of String, Integer)
            Dim hierarchical As New Dictionary(Of String, Integer)

            For Each ec_number As KeyValuePair(Of String, Integer) In terms.SafeQuery
                Dim ec As ECNumber = ECNumber.ValueParser(ec_number.Key)

                For Each ec_term As String In ec.HierarchicalECTerms
                    If Not hierarchical.ContainsKey(ec_term) Then
                        hierarchical(ec_term) = ec_number.Value
                    Else
                        hierarchical(ec_term) += ec_number.Value
                    End If
                Next
            Next

            Return hierarchical
        End Function

        Public Overrides Function ToString() As String
            Return taxonomy
        End Function

        Private Shared Function groupByAssembly(terms As IEnumerable(Of RankTerm)) As IEnumerable(Of NamedCollection(Of RankTerm))
            Return From term As RankTerm
                   In terms
                   Let tag As NamedValue(Of String) = term.queryName.GetTagValue(".")
                   Group By tag.Name Into Group
                   Select New NamedCollection(Of RankTerm)(Name, Group.Select(Function(a) a.term))
        End Function

        Private Shared Iterator Function streamGroupByAssembly(terms As IEnumerable(Of RankTerm)) As IEnumerable(Of NamedCollection(Of RankTerm))
            Dim asm_id As String = ""
            Dim group As New List(Of RankTerm)

            For Each term As RankTerm In terms
                Dim tag As NamedValue(Of String) = term.queryName.GetTagValue(".")

                If tag.Name <> asm_id Then
                    If group.Count > 0 Then
                        Yield New NamedCollection(Of RankTerm)(asm_id, group)
                        group.Clear()
                    End If

                    asm_id = tag.Name
                End If

                Call group.Add(term)
            Next

            If group.Count > 0 Then
                Yield New NamedCollection(Of RankTerm)(asm_id, group)
            End If
        End Function

        ''' <summary>
        ''' Make union of the taxonomy assembly contig result
        ''' </summary>
        ''' <param name="vectors"></param>
        ''' <param name="size_cutoff">test of the contigby gene size cutoff.</param>
        ''' <returns></returns>
        Public Shared Iterator Function GroupByTaxonomy(vectors As IEnumerable(Of GenomeVector), Optional size_cutoff As Integer = 1000) As IEnumerable(Of GenomeVector)
            Call $"Make union of the taxonomy assembly contig result via annotated gene size cutoff {size_cutoff}".debug

            For Each taxonomy As IGrouping(Of String, GenomeVector) In vectors.GroupBy(Function(vec) vec.taxonomy)
                Dim contigs As New List(Of GenomeVector)

                For Each asm As GenomeVector In taxonomy
                    If asm.size < size_cutoff Then
                        Call contigs.Add(asm)
                    Else
                        Yield asm
                    End If
                Next

                If Not contigs.Any Then
                    Continue For
                End If

                Dim union As New Dictionary(Of String, Integer)

                For Each part As GenomeVector In contigs
                    For Each term As KeyValuePair(Of String, Integer) In part.terms
                        If union.ContainsKey(term.Key) Then
                            union(term.Key) += term.Value
                        Else
                            union.Add(term.Key, term.Value)
                        End If
                    Next
                Next

                Yield New GenomeVector With {
                    .taxonomy = taxonomy.Key,
                    .assembly_id = contigs.Keys.JoinBy(","),
                    .terms = union
                }
            Next
        End Function

        Public Shared Iterator Function CreateVectors(terms As IEnumerable(Of RankTerm), Optional stream As Boolean = False) As IEnumerable(Of GenomeVector)
            For Each asm As NamedCollection(Of RankTerm) In If(stream, streamGroupByAssembly(terms), groupByAssembly(terms))
                Dim id As String = asm.name
                Dim title As String = asm.First.queryName
                Dim taxon As String = title.GetTagValue(" ").Value
                Dim counts As Dictionary(Of String, Integer) = asm _
                    .GroupBy(Function(t) t.term) _
                    .ToDictionary(Function(a) a.Key,
                                  Function(a)
                                      Return a.Count
                                  End Function)

                Yield New GenomeVector With {
                    .assembly_id = id,
                    .taxonomy = taxon,
                    .terms = counts
                }
            Next
        End Function

    End Class
End Namespace
