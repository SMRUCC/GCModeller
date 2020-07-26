#Region "Microsoft.VisualBasic::6cb928dd8276e3d28357f2a7a5706e34, Bio.Assembly\Query\IQueryExtensions.vb"

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

    ' Module IQueryExtensions
    ' 
    '     Function: ContainsMultipleSequence, GetLocusTag, MatchGene
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace ObjectQuery

    ''' <summary>
    ''' Extensions for object query in the GCModeller biological components
    ''' </summary>
    Public Module IQueryExtensions

        ''' <summary>
        ''' 函数会优先按照<paramref name="geneName"/>进行查询，假若查找不到结果，才会使用<paramref name="products"/>列表进行模糊匹配
        ''' </summary>
        ''' <param name="PTT"></param>
        ''' <param name="geneName"></param>
        ''' <param name="products">大小写不敏感</param>
        ''' <param name="cut">字符串匹配相似度的最小的阈值</param>
        ''' <param name="First">
        ''' 是直接返回匹配到的第一个结果还是先对所有基因匹配，然后再返回相似度最高的基因对象？默认是直接返回第一个匹配结果
        ''' </param>
        ''' <returns></returns>
        <ExportAPI("Get.Gene")>
        <Extension>
        Public Function MatchGene(PTT As PTT, geneName$, products As IEnumerable(Of String), Optional cut# = 0.8, Optional First As Boolean = True) As GeneBrief
            Dim gene As GeneBrief = PTT.GetGeneByName(geneName)

            If Not gene Is Nothing Then
                Return gene
            End If

            ' 对功能描述或者代谢产物进行字符串模糊匹配
            For Each desc$ In products.Select(AddressOf LCase)
                If First Then
                    Dim match = Function(productValue$) As Boolean
                                    Return FuzzyMatching(desc, LCase(productValue), tokenbased:=False, cutoff:=cut)
                                End Function

                    gene = PTT _
                        .GetGeneByDescription(match) _
                        .FirstOrDefault

                    If Not gene Is Nothing Then
                        Return gene
                    End If
                Else
                    ' 可能会没有任何匹配，所以不直接使用First
                    Dim genes = PTT._innerList _
                        .Select(Function(g)
                                    Return New Binding(Of DistResult, GeneBrief)(LevenshteinDistance.ComputeDistance(desc, LCase(g.Product)), g)
                                End Function) _
                        .Where(Function(d) Not d.Bind Is Nothing) _
                        .OrderByDescending(Function(d) d.Bind.MatchSimilarity) _
                        .FirstOrDefault

                    If genes.Bind.MatchSimilarity >= cut Then
                        Return genes.Target
                    End If
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' <see cref="FastaSeq.Headers"/> parsing the first word token as uid.
        ''' </summary>
        ''' <param name="fasta"></param>
        ''' <returns></returns>
        <Extension> Public Function GetLocusTag(fasta As FastaSeq) As String
            Dim uid As String = fasta.Headers.First.Split.First.Trim
            Return uid
        End Function

        ''' <summary>
        ''' 判断目标fasta文件是否包含有多条序列？
        ''' </summary>
        ''' <param name="path$">fasta file path</param>
        ''' <returns></returns>
        <Extension>
        Public Function ContainsMultipleSequence(path$) As Boolean
            Using reader As StreamReader = path.OpenReader
                Dim s As New Value(Of String)

                ' fasta文件的第一行肯定带有 > 所以必须要跳过
                Call reader.ReadLine()

                Do While Not (s = reader.ReadLine) Is Nothing
                    If (+s).FirstOrDefault = ">"c Then
                        Return True
                    End If
                Loop
            End Using

            Return False
        End Function
    End Module
End Namespace