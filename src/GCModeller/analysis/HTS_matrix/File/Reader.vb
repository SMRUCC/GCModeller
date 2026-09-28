#Region "Microsoft.VisualBasic::6e6a44fa15668b72ef478f9926694d40, analysis\HTS_matrix\File\Reader.vb"

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

    '   Total Lines: 57
    '    Code Lines: 46 (80.70%)
    ' Comment Lines: 3 (5.26%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (14.04%)
    '     File Size: 2.01 KB


    ' Module Reader
    ' 
    '     Function: loadGeneMatrix, ParseGeneRowTokens
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text
Imports r = System.Text.RegularExpressions.Regex

''' <summary>
''' the parser of the matrix text document
''' </summary>
Module Reader

    Const quot$ = """.+""[,\t]"

    ''' <summary>
    ''' parse csv/tsv line
    ''' </summary>
    ''' <param name="line"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 在这里假设基因表达矩阵是由基因id加表达值的向量构成的，不会有太复杂的基因id字符串（例如存在逗号的基因id字符串），所以在这里为了字符串的解析性能，直接通过Split(ASCII.TAB, ","c)来拆分csv/tsv文本行
    ''' </remarks>
    Friend Function ParseGeneRowTokens(line As String) As NamedValue(Of Double())
        Dim quot As String = r.Match(line, Reader.quot).Value

        If Not quot.StringEmpty Then
            Return New NamedValue(Of Double()) With {
                .Name = quot.Trim(""""c, ","c),
                .Value = line _
                    .Substring(quot.Length) _
                    .Split(ASCII.TAB, ","c) _
                    .Select(AddressOf ParseDouble) _
                    .ToArray
            }
        Else
            Dim tokens As String() = line.Split(ASCII.TAB, ","c)

            Return New NamedValue(Of Double()) With {
                .Name = tokens(Scan0),
                .Value = tokens _
                    .Skip(1) _
                    .Select(AddressOf ParseDouble) _
                    .ToArray
            }
        End If
    End Function

    ''' <summary>
    ''' Parse the matrix rows as expression data
    ''' </summary>
    ''' <param name="text"></param>
    ''' <param name="excludes"></param>
    ''' <param name="takeIndex"></param>
    ''' <returns></returns>
    <Extension>
    Friend Iterator Function loadGeneMatrix(text As IEnumerable(Of String), excludes As Index(Of String), takeIndex As Integer()) As IEnumerable(Of DataFrameRow)
        For Each line As String In text
            Dim tokens As NamedValue(Of Double()) = ParseGeneRowTokens(line)
            Dim data As Double() = tokens.Value

            If Not excludes Is Nothing Then
                data = takeIndex _
                    .Select(Function(i) data(i)) _
                    .ToArray
            End If

            Yield New DataFrameRow With {
                .experiments = data,
                .geneID = tokens.Name.Trim(""""c, " "c, ASCII.TAB)
            }
        Next
    End Function
End Module
