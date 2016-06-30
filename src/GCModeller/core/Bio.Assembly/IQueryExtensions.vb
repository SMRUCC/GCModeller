#Region "Microsoft.VisualBasic::1b1b942f6968add5641f0eb19bff8e40, ..\Bio.Assembly\IQueryExtensions.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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
                        FuzzyMatchString.Equals(desc, productValue, cut:=cut)).FirstOrDefault

                If Not gene Is Nothing Then
                    Exit For
                End If
            Next
        End If

        Return gene
    End Function
End Module
