#Region "Microsoft.VisualBasic::1c188a6bc325dce5de9210c809350a18, data\RCSB PDB\PDB\Keywords\Headers\Remark.vb"

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

'   Total Lines: 94
'    Code Lines: 75 (79.79%)
' Comment Lines: 2 (2.13%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 17 (18.09%)
'     File Size: 4.19 KB


'     Enum RemarkCategory
' 
' 
'  
' 
' 
' 
'     Class Remark
' 
'         Properties: Keyword, RemarkText
' 
'         Function: Append, GetRemarkCategory
' 
'         Sub: Flush
' 
' 
' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Keywords

    Public Enum RemarkCategory
        ' 主要分类编码
        GeneralText = 1          ' REMARK 1: 自由文本注释
        ExperimentalMethod = 2   ' REMARK 2: 实验方法（如X射线、NMR）
        Crystallography = 3      ' REMARK 3: 结晶学数据（分辨率、R因子）
        Refinement = 4           ' REMARK 4: 结构修正参数

        DataProcessing = 5

        DataCollection = 200     ' REMARK 200-299: 数据收集参数（探测器、波长等）
        Validation = 300         ' REMARK 300: 模型验证（Ramachandran图）
        Superimposition = 400    ' REMARK 400: 结构叠加或比对

        ProcessingInformation = 100   ' REMARK 100: PDB处理元数据
        Crystallization = 280         ' REMARK 280: 晶体溶剂含量/结晶条件
        Symmetry = 290                ' REMARK 290: 晶体对称性操作
        BiomoleculeAssembly = 350     ' REMARK 350: 生物分子组装变换矩阵
        MissingResiduesAtoms = 465    ' REMARK 465/470: 缺失残基/原子
        GeometryCheck = 500           ' REMARK 500: 几何/立体化学检查
        RelatedEntries = 900          ' REMARK 900: 相关PDB条目

        NoncovalentInteractions = 600
        HydrophobicInteractions = 700
        OtherImportantFeatures = 800

        Other = 999              ' 其他未分类的REMARK
    End Enum

    ''' <summary>
    ''' the pdb remark keyword data
    ''' </summary>
    Public Class Remark : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_REMARK
            End Get
        End Property

        ''' <summary>
        ''' the pdb remark text data with different categories labels
        ''' </summary>
        ''' <returns></returns>
        Public Property RemarkText As Dictionary(Of RemarkCategory, String)

        Dim cache As New List(Of NamedValue(Of String))

        Friend Shared Function Append(ByRef remark As Remark, str As String) As Remark
            If remark Is Nothing Then
                remark = New Remark
            End If

            Dim catNumber = str.Substring(0, 4).Trim
            Dim value As String

            If catNumber.IsInteger Then
                value = str.Substring(4).Trim
            Else
                catNumber = "1"
                value = str.Trim
            End If

            remark.cache.Add(New NamedValue(Of String)(catNumber, value))
            Return remark
        End Function

        Friend Overrides Sub Flush()
            RemarkText = cache _
                .GroupBy(Function(line) GetRemarkCategory(Integer.Parse(line.Name))) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.Select(Function(t) t.Value).JoinBy(vbCrLf)
                              End Function)
        End Sub

        Public Shared Function GetRemarkCategory(code As Integer) As RemarkCategory
            Select Case code
                Case 1 : Return RemarkCategory.GeneralText
                Case 2 : Return RemarkCategory.ExperimentalMethod
                Case 3 : Return RemarkCategory.Crystallography
                Case 4 : Return RemarkCategory.Refinement
                Case 5 : Return RemarkCategory.DataProcessing
                Case 200 To 299
                    Return RemarkCategory.DataCollection
                Case 300
                    ' 根据实际内容覆盖标准分类：此处REMARK 300指向生物组装
                    Return RemarkCategory.BiomoleculeAssembly
                Case 350 : Return RemarkCategory.BiomoleculeAssembly
                Case 400 : Return RemarkCategory.Superimposition
                Case 100 : Return RemarkCategory.ProcessingInformation
                Case 280 : Return RemarkCategory.Crystallization
                Case 290 : Return RemarkCategory.Symmetry
                Case 465, 470
                    Return RemarkCategory.MissingResiduesAtoms
                Case 500 : Return RemarkCategory.GeometryCheck

                Case 600 : Return RemarkCategory.NoncovalentInteractions
                Case 700 : Return RemarkCategory.HydrophobicInteractions
                Case 800 : Return RemarkCategory.OtherImportantFeatures

                Case 900 : Return RemarkCategory.RelatedEntries

                Case Else
                    Return RemarkCategory.Other
            End Select
        End Function

        ''' <summary>
        ''' 生成PDB格式的REMARK部分文本内容
        ''' </summary>
        ''' <param name="remarkObj">Remark对象实例</param>
        ''' <returns>格式化后的REMARK文本</returns>
        Public Shared Function GenerateRemarkText(remarkObj As Remark) As String
            If remarkObj Is Nothing OrElse remarkObj.RemarkText Is Nothing Then
                Return String.Empty
            End If

            Dim sb As New StringBuilder()
            ' 按照REMARK编号顺序处理各个类别
            Dim orderedCategories = remarkObj.RemarkText.Keys.OrderBy(Function(c) CInt(c))

            For Each category As RemarkCategory In orderedCategories
                Dim remarkNumber As Integer = CInt(category)
                Dim textContent As String = remarkObj.RemarkText(category)

                If Not String.IsNullOrEmpty(textContent) Then
                    ' 将文本内容按行分割
                    Dim lines = textContent.Split({vbCrLf, vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries)

                    For Each line In lines
                        ' 格式化REMARK行（符合PDB格式规范）
                        Dim formattedLine = FormatRemarkLine(remarkNumber, line)
                        sb.AppendLine(formattedLine)
                    Next
                End If
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' 格式化单个REMARK行（符合PDB文件格式规范）
        ''' </summary>
        Private Shared Function FormatRemarkLine(remarkNumber As Integer, content As String) As String
            ' PDB格式要求：REMARK记录前6字符为"REMARK"，然后是3字符的编号（右对齐）
            Dim numberPart = remarkNumber.ToString().PadLeft(3)

            ' 确保内容不超过PDB格式限制（通常约70字符）
            Dim contentPart = content
            If contentPart.Length > 70 Then
                contentPart = contentPart.Substring(0, 70)
            End If

            Return $"REMARK {numberPart} {contentPart}"
        End Function
    End Class
End Namespace
