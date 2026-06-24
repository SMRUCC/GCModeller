' ============================================================================
' Module 1: Genome Annotation & Feature Extraction
' File: GenomeAnnotation/GffParser.vb
'
' 功能: 解析 GFF3 格式的基因组注释文件，提取基因/CDS 的坐标与属性信息。
'       对应论文中 "基因组注释与特征化模块" 的基因预测结果读取部分。
'
' 论文对应: Traitar 使用 Prodigal 进行基因预测，输出 GFF 格式。
'           本模块负责读取已有的 GFF 文件（由 Prodigal 或其他工具生成）。
' ============================================================================

Imports System.IO
Imports System.Collections.Generic

Namespace Traitar.GenomeAnnotation

    ''' <summary>
    ''' 表示 GFF3 文件中的一条记录（通常是一个 CDS / gene）
    ''' </summary>
    Public Class GffRecord
        ''' <summary>序列ID（如 contig_1 / chromosome）</summary>
        Public Property SeqId As String
        ''' <summary>来源（如 Prodigal）</summary>
        Public Property Source As String
        ''' <summary>类型（gene / CDS / mRNA ...）</summary>
        Public Property FeatureType As String
        ''' <summary>起始坐标（1-based）</summary>
        Public Property Start As Integer
        ''' <summary>终止坐标（1-based, inclusive）</summary>
        Public Property [End] As Integer
        ''' <summary>得分（可为 '.'）</summary>
        Public Property Score As String
        ''' <summary>正负链（+/-）</summary>
        Public Property Strand As Char
        ''' <summary>相位（0/1/2 或 '.'）</summary>
        Public Property Phase As String
        ''' <summary>属性键值对（ID=...;Name=...;product=...）</summary>
        Public Property Attributes As Dictionary(Of String, String)

        Public Overrides Function ToString() As String
            Return $"{SeqId}:{Start}-{[End]}({Strand}) [{FeatureType}]"
        End Function
    End Class

    ''' <summary>
    ''' GFF3 文件解析器
    ''' </summary>
    Public Class GffParser

        ''' <summary>
        ''' 解析 GFF3 文件，返回所有记录
        ''' </summary>
        Public Shared Function Parse(filePath As String) As List(Of GffRecord)
            Dim records As New List(Of GffRecord)
            For Each line As String In File.ReadAllLines(filePath)
                If String.IsNullOrEmpty(line) Then Continue For
                If line.StartsWith("#") Then Continue For

                Dim fields = line.Split(ControlChars.Tab)
                If fields.Length < 9 Then Continue For

                Dim rec As New GffRecord With {
                    .SeqId = fields(0),
                    .Source = fields(1),
                    .FeatureType = fields(2),
                    .Start = Integer.Parse(fields(3)),
                    .[End] = Integer.Parse(fields(4)),
                    .Score = fields(5),
                    .Strand = If(fields(6).Length > 0, fields(6)(0), "."c),
                    .Phase = fields(7),
                    .Attributes = ParseAttributes(fields(8))
                }
                records.Add(rec)
            Next
            Return records
        End Function

        ''' <summary>
        ''' 仅提取 CDS 记录（蛋白质编码基因）
        ''' </summary>
        Public Shared Function ParseCds(filePath As String) As List(Of GffRecord)
            Return Parse(filePath).FindAll(Function(r) r.FeatureType = "CDS")
        End Function

        ''' <summary>
        ''' 解析第9列属性字段: key1=value1;key2=value2
        ''' </summary>
        Private Shared Function ParseAttributes(attrStr As String) As Dictionary(Of String, String)
            Dim dict As New Dictionary(Of String, String)
            If String.IsNullOrEmpty(attrStr) Then Return dict
            For Each pair As String In attrStr.Split(";"c)
                If String.IsNullOrWhiteSpace(pair) Then Continue For
                Dim eqIdx = pair.IndexOf("="c)
                If eqIdx > 0 Then
                    Dim key = pair.Substring(0, eqIdx).Trim()
                    Dim val = pair.Substring(eqIdx + 1).Trim()
                    dict(key) = val
                End If
            Next
            Return dict
        End Function
    End Class
End Namespace
