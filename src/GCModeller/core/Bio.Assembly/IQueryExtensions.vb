#Region "Microsoft.VisualBasic::6fcf105226833feec101b6871f748eff, ..\core\Bio.Assembly\IQueryExtensions.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Text.Similarity
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports System.IO
Imports Microsoft.VisualBasic.Language

''' <summary>
''' Extensions for object query in the GCModeller biological components
''' </summary>
Public Module IQueryExtensions

    ''' <summary>
    ''' 函数会优先按照<paramref name="geneName"/>进行查询，假若查找不到结果，才会使用<paramref name="products"/>列表进行模糊匹配
    ''' </summary>
    ''' <param name="PTT"></param>
    ''' <param name="geneName"></param>
    ''' <param name="products"></param>
    ''' <param name="cut">字符串匹配相似度的最小的阈值</param>
    ''' <returns></returns>
    <ExportAPI("Get.Gene")>
    <Extension>
    Public Function MatchGene(PTT As PTT, geneName As String, products As IEnumerable(Of String), Optional cut As Double = 0.8) As GeneBrief
        Dim gene As GeneBrief = PTT.GetGeneByName(geneName)

        If gene Is Nothing Then
            For Each desc As String In products
                gene = PTT.GetGeneByDescription(
                    Function(productValue) _
                        FuzzyMatching(desc, productValue, cutoff:=cut)).FirstOrDefault

                If Not gene Is Nothing Then
                    Exit For
                End If
            Next
        End If

        Return gene
    End Function

    ''' <summary>
    ''' <see cref="fastatoken.Attributes"/> parsing the first word token as uid.
    ''' </summary>
    ''' <param name="fasta"></param>
    ''' <returns></returns>
    <Extension> Public Function GetLocusTag(fasta As FastaToken) As String
        Dim uid As String = fasta.Attributes.First.Split.First.Trim
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
