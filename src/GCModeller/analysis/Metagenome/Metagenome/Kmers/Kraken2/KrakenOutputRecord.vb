#Region "Microsoft.VisualBasic::2fcb878c59ebfa94b42f625e5a61e75d, analysis\Metagenome\Metagenome\Kmers\Kraken2\KrakenOutputRecord.vb"

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

    '   Total Lines: 116
    '    Code Lines: 63 (54.31%)
    ' Comment Lines: 38 (32.76%)
    '    - Xml Docs: 92.11%
    ' 
    '   Blank Lines: 15 (12.93%)
    '     File Size: 5.16 KB


    '     Class KrakenOutputRecord
    ' 
    '         Properties: LcaMappings, LCASupport, LCATaxids, ReadLength, ReadName
    '                     StatusCode, TaxID, Taxonomy
    ' 
    '         Function: GetTaxID, MakeAnnotationResult, ParseDocument
    ' 
    '         Sub: Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Namespace Kmers.Kraken2

    ''' <summary>
    ''' 用于存储 --output 文件中每一行的数据，这个文件详细列出了每一条序列（read）的分类结果。每一行对应一条 read。
    ''' </summary>
    ''' <remarks>
    ''' --output 文件是 read-centric 的，关注点是“每一条 read 被分到了哪里”。
    ''' </remarks>
    Public Class KrakenOutputRecord : Implements ITaxonomy

        ' [状态码/分类代码]\t[Read名称]\t[TaxID]\t[Read长度]\t[LCA映射详情]

        ''' <summary>
        '''  "C" or "U"
        '''  
        ''' C: Classified (已分类)。表示 Kraken2 成功为该 read 分配了一个分类单元。
        ''' U: Unclassified (未分类)。表示该 read 未能被分配到任何数据库中的分类单元。
        ''' </summary>
        ''' <returns></returns>
        <Column("status_code")> Public Property StatusCode As String
        ''' <summary>
        ''' 输入的 FASTQ 文件中该 read 的序列标识符。
        ''' </summary>
        ''' <returns></returns>
        Public Property ReadName As String
        ''' <summary>
        ''' 这是 Kraken2 最终分配给该 read 的分类单元的 NCBI Taxonomy ID。这个分类单元是该 read 上所有 k-mer 的最低共同祖先。
        ''' </summary>
        ''' <returns></returns>
        <Column("taxid")> Public Property TaxID As UInteger Implements ITaxonomy.ncbi_taxid

        ''' <summary>
        ''' biom style taxonomy string
        ''' </summary>
        ''' <returns></returns>
        <Column("tax_name")> Public Property Taxonomy As String Implements ITaxonomy.taxonomy_string
        <Column("lca_support")> Public Property LCASupport As Double
        <Column("lca_taxids")> Public Property LCATaxids As Integer()

        ''' <summary>
        ''' 该 read 的碱基数量。
        ''' </summary>
        ''' <returns></returns>
        <Column("read_length")> Public Property ReadLength As Integer
        ''' <summary>
        ''' LCA映射详情, 使用字典来存储 k-mer 的分配详情，键是 TaxID，值是 k-mer 数量
        ''' 
        ''' 这是一个由空格分隔的列表，详细描述了该 read 中所有 k-mer 的分配情况。
        ''' 格式为 [TaxID]:[k-mer数量]。
        ''' 0 是一个特殊的 TaxID，通常代表未分类的 k-mer（即数据库中没有匹配的 k-mer）。0:6 表示有 6 个 k-mer 未被分类。
        ''' </summary>
        ''' <returns></returns>
        Public Property LcaMappings As New Dictionary(Of String, Integer)

        Public Shared Function MakeAnnotationResult(reads As IEnumerable(Of KrakenOutputRecord)) As Dictionary(Of String, Integer)
            Dim readsIndex = reads.GroupBy(Function(r) r.ReadName)
            Dim annotation = readsIndex _
                .ToDictionary(Function(r)
                                  Return r.Key
                              End Function,
                              Function(r)
                                  Return GetTaxID(r)
                              End Function)

            Return annotation
        End Function

        Private Shared Function GetTaxID(r As IGrouping(Of String, KrakenOutputRecord)) As Integer
            Dim filter As KrakenOutputRecord() = r.Where(Function(ri) ri.TaxID > 0).ToArray

            If filter.Length = 0 Then
                Return 0
            End If
            If filter.Length = 1 Then
                Return filter.First.TaxID
            Else
                Dim top = filter _
                   .OrderByDescending(Function(ri)
                                          Return ri.LcaMappings(ri.TaxID.ToString)
                                      End Function) _
                   .First

                Return top.TaxID
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseDocument(filepath As String) As IEnumerable(Of KrakenOutputRecord)
            Return Kraken2.KrakenParser.ParseOutputFile(filepath)
        End Function

        Public Shared Sub Save(result As IEnumerable(Of KrakenOutputRecord), file As Stream)
            Using text As New StreamWriter(file) With {.NewLine = ASCII.LF}
                For Each line As KrakenOutputRecord In result.SafeQuery
                    Call text.WriteLine(New String() {
                         line.StatusCode,
                         line.ReadName,
                         line.TaxID,
                         line.ReadLength,
                         (From map As KeyValuePair(Of String, Integer)
                          In line.LcaMappings
                          Select $"{map.Key}:{map.Value}").JoinBy(" ")
                    }.JoinBy(vbTab))
                Next
            End Using
        End Sub

    End Class

End Namespace
