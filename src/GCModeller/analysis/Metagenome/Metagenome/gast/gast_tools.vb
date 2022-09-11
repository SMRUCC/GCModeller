#Region "Microsoft.VisualBasic::1e63df59d6de75e34f60b4b97f60138d, GCModeller\analysis\Metagenome\Metagenome\gast\gast_tools.vb"

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

    '   Total Lines: 249
    '    Code Lines: 182
    ' Comment Lines: 22
    '   Blank Lines: 45
    '     File Size: 10.06 KB


    '     Module gast_tools
    ' 
    '         Function: __gi, ExportNt, ExportSILVA, FillTaxonomy, gastTaxonomyInternal
    '                   NamesClusterOut, ParseNames
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace gast

    ' http://www.cnblogs.com/leezx/p/6972226.html

    Public Module gast_tools

        <Extension>
        Friend Iterator Function gastTaxonomyInternal(blastn As IEnumerable(Of Query),
                                                      getTaxonomy As Func(Of String, Taxonomy),
                                                      getOTU As Dictionary(Of String, NamedValue(Of Integer)),
                                                      min_pct#) As IEnumerable(Of gastOUT)
            For Each query As Query In blastn
                If Not getOTU.ContainsKey(query.QueryName) Then
                    Continue For
                End If

                ' Create an array Of taxonomy objects For all the associated refssu_ids.
                Dim hits = query _
                    .SubjectHits _
                    .Select(Function(h) getTaxonomy(h.Name)) _
                    .ToArray
                Dim counts = getOTU(query.QueryName)

                ' Lookup the consensus taxonomy For the array
                Dim taxReturn = gast.Taxonomy.consensus(hits, majority:=min_pct)
                ' 0=taxObj, 1=winning vote, 2=minrank, 3=rankCounts, 4=maxPcts, 5=naPcts;
                Dim taxon = taxReturn(0).TaxonomyString
                Dim rank = taxReturn(0).depth.ToString

                'If r.Matches(taxon, "g__", RegexICSng).Count = 2 Then
                '    taxReturn = gast.Taxonomy.consensus(hits, majority:=min_pct)
                '    Pause()
                'End If

                If (taxon Is Nothing) Then
                    taxon = "Unknown"
                Else
                    ' 因为分类在中间不可能出现NA，NA只会出现在末尾部分
                    ' 所以在这里就直接使用字符串替换来删除多余的NA了
                    ' 在这里还需要删除[]这类的字符串
                    taxon = taxon _
                        .Replace(";NA", "") _
                        .Replace("[", "") _
                        .Replace("]", "") _
                        .Trim(";"c) _
                        .Trim
                End If

                ' (taxonomy, distance, rank, refssu_count, vote, minrank, taxa_counts, max_pcts, na_pcts)
                Dim gastOut As New gastOUT With {
                    .taxonomy = taxon,
                    .rank = rank,
                    .refssu_count = hits.Length,
                    .vote = taxReturn(1).TaxonomyString.Trim(";"c).Trim,
                    .minrank = taxReturn(2).TaxonomyString.Trim(";"c).Trim,
                    .taxa_counts = taxReturn(3).TaxonomyString,
                    .max_pcts = taxReturn(4).TaxonomyString,
                    .na_pcts = taxReturn(5).TaxonomyString,
                    .read_id = counts.Name,
                    .refhvr_ids = query.QueryName,
                    .counts = counts.Value
                }

                Yield gastOut
            Next
        End Function

        <Extension>
        Public Function ExportSILVA([in] As String, EXPORT As String) As Boolean
            Dim reader As New StreamIterator([in])
            Dim out As String = EXPORT & "/" & [in].BaseName & ".fasta"
            Dim tax As String = out.TrimSuffix & ".tax"

            Call "".SaveTo(out)
            Call "".SaveTo(tax)

            Using ref As New StreamWriter(New FileStream(out, FileMode.OpenOrCreate)),
                taxon As New StreamWriter(New FileStream(tax, FileMode.OpenOrCreate))

                ref.NewLine = vbLf
                taxon.NewLine = vbLf

                For Each fa As FastaSeq In reader.ReadStream
                    Dim title As String = fa.Title
                    Dim uid As String = title.Split.First
                    Dim taxnomy As String = Mid(title, uid.Length + 1).Trim

                    uid = uid.Replace(".", "_")
                    fa = New FastaSeq({uid}, fa.SequenceData)
                    title = {uid, taxnomy, "1"}.JoinBy(vbTab)

                    Call ref.WriteLine(fa.GenerateDocument(60))
                    Call taxon.WriteLine(title)
                Next
            End Using

            Return True
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="[in]">file path of *.names</param>
        ''' <returns></returns>
        Public Iterator Function NamesClusterOut([in] As String) As IEnumerable(Of NamedValue(Of String()))
            Dim lines As String() = [in].ReadAllLines

            For Each line As String In lines
                Dim tokens As String() = Strings.Split(line, vbTab)

                Yield New NamedValue(Of String()) With {
                    .Name = tokens(Scan0),
                    .Value = tokens(1).Split(","c)
                }
            Next
        End Function

        Public Function ExportNt(nt As String, gi2taxid As String, taxi_dmp As String, out As String) As Boolean
            Dim giTaxid As BucketDictionary(Of Integer, Integer) =
                NCBI.Taxonomy.AcquireAuto(gi2taxid)
            Dim tree As New NcbiTaxonomyTree(taxi_dmp)

            Using ref As StreamWriter = out.OpenWriter(Encodings.ASCII,),
                tax As StreamWriter = (out.TrimSuffix & ".tax").OpenWriter(Encodings.ASCII)

                For Each seq As FastaSeq In New StreamIterator(nt).ReadStream
                    Dim title As String = seq.Title
                    Dim gi As Integer = __gi(title)
                    Dim taxid As Integer = giTaxid(gi)
                    Dim nodes As TaxonomyNode() = tree.GetAscendantsWithRanksAndNames(taxid, True)

                    If nodes.Length = 0 Then
                        Continue For
                    End If

                    Dim taxonomy As String = TaxonomyNode.Taxonomy(nodes, ";")

                    seq = New FastaSeq({title}, seq.SequenceData)
                    title = {title, taxonomy, "1"}.JoinBy(vbTab)

                    Call ref.WriteLine(seq.GenerateDocument(120))
                    Call tax.WriteLine(title)
                Next
            End Using

            Return True
        End Function

        Private Function __gi(ByRef title As String) As Integer
            Dim gi As String = Regex.Match(title, "gi\|\S+", RegexICSng).Value

            title = gi
            title = title.NormalizePathString().Trim("_"c)
            gi = Regex.Match(title, "gi(\||_)\d+", RegexICSng).Value
            gi = Regex.Match(gi, "\d+").Value

            Return CInt(Val(gi))
        End Function

        Public Function ParseNames(path As String) As Names()
            Dim lines As IEnumerable(Of String) = path.IterateAllLines
            Dim LQuery = LinqAPI.MakeList(Of Names) <=
                From line As String
                In lines
                Let tokens As String() = Strings.Split(line, vbTab)
                Let uid As String = tokens(Scan0)
                Let members As String() = tokens(1).Split(","c)
                Select New Names With {
                    .members = members,
                    .numOfSeqs = members.Length,
                    .unique = uid
                }

            'Dim tags As String() = LinqAPI.Exec(Of String) <=
            '    From s As String
            '    In LQuery.Select(Function(x) x.members).MatrixAsIterator
            '    Select Regex.Replace(s, "_\d+", "")
            '    Distinct

            For Each x As Names In LQuery
                Dim myTags = From s As String
                             In x.members
                             Select sid = Regex.Replace(s, "\d+", "")
                             Group By sid Into Count

                x.composition = myTags.ToDictionary(
                    Function(s) s.sid,
                    Function(s)
                        Return (s.Count * 100) / x.numOfSeqs
                    End Function)
            Next

            Return New Names With {
                .unique = TagTotal,
                .numOfSeqs = LQuery.Sum(Function(x) x.numOfSeqs)
            } + LQuery
        End Function

        Public Const TagTotal As String = "Total"
        Public Const TagNoAssign As String = "Not-Assign"

        <Extension>
        Public Iterator Function FillTaxonomy(source As IEnumerable(Of Names), gast_out As String) As IEnumerable(Of Names)
            Dim hash As New Dictionary(Of gastOUT)(gast_out.Imports(Of gastOUT)(vbTab))
            Dim notAssigned As Integer

            For Each x As Names In source
                If Not hash.ContainsKey(x.unique) Then
                    Yield x
                    Continue For
                Else

                End If

                Dim taxi As gastOUT = hash(x.unique)
                x.taxonomy = taxi.taxonomy
                x.distance = taxi.distance
                x.refs = taxi.refhvr_ids

                If taxi.refhvr_ids = "NA" Then
                    notAssigned += x.numOfSeqs
                End If

                Yield x
            Next

            Yield New Names With {
                .distance = -1,
                .numOfSeqs = notAssigned,
                .refs = "N/A",
                .taxonomy = "null",
                .unique = TagNoAssign
            }
        End Function
    End Module
End Namespace
