#Region "Microsoft.VisualBasic::7af0dbd4fb93c4bc64b91de5a6955e68, sub-system\BNLearn\IO\IO.vb"

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

'   Total Lines: 178
'    Code Lines: 113 (63.48%)
' Comment Lines: 31 (17.42%)
'    - Xml Docs: 77.42%
' 
'   Blank Lines: 34 (19.10%)
'     File Size: 7.43 KB


'     Module BnIO
' 
'         Function: ReadGeneExpressionMatrix, ReadPriorNetwork
' 
'         Sub: WriteBatchInterventionResults, WriteCPDParameters, WriteInterventionResult, WriteNetworkStructure
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports SMRUCC.genomics.BioSystem.Intervention

Namespace IO

    ''' <summary>
    ''' 数据读写工具
    ''' </summary>
    Public Module BnIO

        ' ==================== 读取单样本外部转录组向量 ====================

        ''' <summary>
        ''' 从外部转录组向量文件读取单样本（或一组均值）的基因表达水平
        ''' 文件格式：两列基因名与表达值，支持 CSV/TSV，首行可为表头（gene / expression，大小写不敏感）
        ''' 例如：
        '''   gene,expression
        '''   codY,12.3
        '''   comK,4.5
        ''' 返回 基因名 → 表达值 的键值对字典。基因名按 OrdinalIgnoreCase 比较去重（后者覆盖前者）。
        ''' 若文件不存在或解析不到任何记录则抛出友好异常。
        ''' </summary>
        <Extension>
        Public Function ReadExpressionVector(path As String) As Dictionary(Of String, Double)
            If Not File.Exists(path) Then
                Throw New Exception(String.Format("外部转录组向量文件不存在: {0}", path))
            End If

            Dim lines As String() = File.ReadAllLines(path)
            If lines.Length = 0 Then
                Throw New Exception(String.Format("外部转录组向量文件为空: {0}", path))
            End If

            Dim delim As Char() = New Char() {","c, vbTab, ";"c}
            Dim result As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

            ' 判断首行是否为表头
            Dim startRow As Integer = 0
            Dim firstTokens As String() = lines(0).Split(delim, StringSplitOptions.RemoveEmptyEntries)
            If firstTokens.Length >= 2 Then
                Dim col0 As String = firstTokens(0).Trim()
                Dim col1 As String = firstTokens(1).Trim()
                If col0.Equals("gene", StringComparison.OrdinalIgnoreCase) OrElse
                   col0.Equals("genes", StringComparison.OrdinalIgnoreCase) Then
                    startRow = 1
                End If
            End If

            For i = startRow To lines.Length - 1
                Dim line As String = lines(i).Trim()
                If String.IsNullOrEmpty(line) Then Continue For

                Dim tokens As String() = line.Split(delim, StringSplitOptions.RemoveEmptyEntries)
                If tokens.Length < 2 Then Continue For

                Dim gene As String = tokens(0).Trim()
                Dim value As Double
                If Not Double.TryParse(tokens(1).Trim(), value) Then Continue For

                ' 按大小写不敏感去重：后者覆盖前者
                result(gene) = value
            Next

            If result.Count = 0 Then
                Throw New Exception(String.Format("外部转录组向量文件未解析到任何有效记录: {0}", path))
            End If

            Return result
        End Function

    End Module

End Namespace

