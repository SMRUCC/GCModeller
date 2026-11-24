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

Namespace Assembly.NCBI.GenBank.GBFF

    ''' <summary>
    ''' Genbank 平文件(GBFF)的文本格式常量以及行分类工具。
    ''' </summary>
    ''' <remarks>
    ''' NCBI 平文件之中所有的列位置都是固定的:
    ''' 
    ''' ```
    ''' LOCUS       NC_000001          248956422 bp    DNA     linear   CON 06-AUG-2025
    ''' FEATURES             Location/Qualifiers
    '''      source          1..248956422
    '''                      /organism="Homo sapiens"
    ''' ORIGIN
    '''         1 aacaatgctg aacgtgagac agtgaaagaa ...
    ''' //
    ''' ```
    ''' 
    ''' 字段名占用第1列到第10列，字段的值从第13列开始；``FEATURES``段的位点类型名从第6列开始，
    ''' 位点的位置或者qualifier从第22列开始。
    ''' </remarks>
    Public Module GbkFormat

        ''' <summary>
        ''' 头部字段名所占用的列宽度(字段的值从这一列之后开始)
        ''' </summary>
        Public Const HEADER_INDENT As Integer = 12
        ''' <summary>
        ''' 平文件的标准行宽
        ''' </summary>
        Public Const LINE_WIDTH As Integer = 80
        ''' <summary>
        ''' ``FEATURES``段之中位点类型名所在的列(下标从0开始，即第6列)
        ''' </summary>
        Public Const FEATURE_KEY_COLUMN As Integer = 5
        ''' <summary>
        ''' ``FEATURES``段之中位点的位置以及qualifier所在的列(下标从0开始，即第22列)
        ''' </summary>
        Public Const QUALIFIER_COLUMN As Integer = 21
        ''' <summary>
        ''' 位点类型名所占用的列宽度(第6列到第21列)
        ''' </summary>
        Public Const FEATURE_KEY_WIDTH As Integer = QUALIFIER_COLUMN - FEATURE_KEY_COLUMN
        ''' <summary>
        ''' ``ORIGIN``段每一行的碱基数量
        ''' </summary>
        Public Const SEQUENCE_PER_LINE As Integer = 60
        ''' <summary>
        ''' ``ORIGIN``段之中碱基的分组大小(每10个碱基之间一个空格)
        ''' </summary>
        Public Const SEQUENCE_GROUP_SIZE As Integer = 10
        ''' <summary>
        ''' ``ORIGIN``段行首位置号所占用的列宽度(最大支持9位数，即1亿以上的序列)
        ''' </summary>
        Public Const POSITION_WIDTH As Integer = 9
        ''' <summary>
        ''' 记录之间的分隔符
        ''' </summary>
        Public Const RECORD_DELIMITER As String = "//"
        ''' <summary>
        ''' ``FEATURES``段的标题行
        ''' </summary>
        Public Const FEATURES_HEADER As String = "FEATURES             Location/Qualifiers"
        ''' <summary>
        ''' ``ORIGIN``段的标题行
        ''' </summary>
        Public Const ORIGIN_HEADER As String = "ORIGIN"

        ''' <summary>
        ''' 判断目标行是否为一个新的字段(section)的起始行。
        ''' 
        ''' 字段名是从第1列开始的字母，并且字段名的末尾(第11列之后)是空白字符；
        ''' 而字段的续行总是以空白字符开始的。
        ''' </summary>
        Public Function IsSectionHeader(line As String) As Boolean
            If String.IsNullOrEmpty(line) Then
                Return False
            End If
            If Not Char.IsLetter(line(0)) Then
                Return False
            End If
            If line.Length >= HEADER_INDENT Then
                Return line(HEADER_INDENT - 2) = " "c
            Else
                ' 长度不足12列的短行只可能是单独的 ORIGIN 字段
                Return String.Equals(line.Trim(), ORIGIN_HEADER, StringComparison.OrdinalIgnoreCase)
            End If
        End Function

        ''' <summary>
        ''' 将字段行拆分为字段名以及字段值
        ''' </summary>
        ''' <param name="line">字段的原始文本行</param>
        ''' <param name="keyword">字段名(例如``LOCUS``/``DEFINITION``)</param>
        ''' <param name="value">字段值(第13列之后的内容)</param>
        Public Sub SplitKeyword(line As String, ByRef keyword As String, ByRef value As String)
            keyword = Nothing
            value = Nothing

            If String.IsNullOrEmpty(line) Then
                Return
            End If

            Dim i As Integer = line.IndexOf(" "c)

            If i < 0 Then
                keyword = line.Trim()
                Return
            End If

            keyword = line.Substring(0, i)

            If line.Length > HEADER_INDENT Then
                value = line.Substring(HEADER_INDENT).Trim()
            Else
                value = ""
            End If
        End Sub

        ''' <summary>
        ''' 取出字段行的值部分(第13列之后的内容)
        ''' </summary>
        Public Function GetValue(line As String) As String
            If String.IsNullOrEmpty(line) Then
                Return ""
            ElseIf line.Length <= HEADER_INDENT Then
                Return ""
            Else
                Return line.Substring(HEADER_INDENT).Trim()
            End If
        End Function

        ''' <summary>
        ''' 判断目标行是否为一个新的位点(feature)的起始行。
        ''' 
        ''' 位点类型名从第6列开始，而位点的位置续行以及qualifier都是从第22列开始的，
        ''' 因此第6列上面不是空白字符的行就是新的位点的起始行。
        ''' </summary>
        Public Function IsFeatureHeader(line As String) As Boolean
            If String.IsNullOrEmpty(line) OrElse line.Length <= FEATURE_KEY_COLUMN Then
                Return False
            End If
            If line(0) <> " "c Then
                Return False
            End If
            Return line(FEATURE_KEY_COLUMN) <> " "c
        End Function

        ''' <summary>
        ''' 判断目标行是否为一个qualifier的起始行(第22列为<c>/</c>)
        ''' </summary>
        Public Function IsQualifierStart(line As String) As Boolean
            If String.IsNullOrEmpty(line) OrElse line.Length <= QUALIFIER_COLUMN Then
                Return False
            End If
            Return line(QUALIFIER_COLUMN) = "/"c
        End Function

        ''' <summary>
        ''' 判断分子的拓扑结构是否为环状
        ''' </summary>
        ''' <param name="topology">``LOCUS``行之中的拓扑结构字段(``linear``/``circular``/``both``)</param>
        Public Function IsCircular(topology As String) As Boolean
            If String.IsNullOrEmpty(topology) Then
                Return False
            End If
            Return topology.IndexOf("circular", StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                String.Equals(topology.Trim(), "both", StringComparison.OrdinalIgnoreCase)
        End Function

        ''' <summary>
        ''' 判断分子的拓扑结构是否为线性
        ''' </summary>
        Public Function IsLinear(topology As String) As Boolean
            If String.IsNullOrEmpty(topology) Then
                Return True
            End If
            Return topology.IndexOf("linear", StringComparison.OrdinalIgnoreCase) >= 0
        End Function

        ''' <summary>
        ''' 判断``LOCUS``行的分类代码是否表示一个``CON``(constructed)类型的记录
        ''' </summary>
        Public Function IsCON(division As String) As Boolean
            Return String.Equals(If(division, "").Trim(), "CON", StringComparison.OrdinalIgnoreCase)
        End Function
    End Module
End Namespace
