Imports System
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.Framework
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Namespace Data

    ''' <summary>
    ''' 细胞级元数据：barcode → 患者 / 期别 / 组织类型 / 分组。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 元数据表用 <see cref="SampleInfo"/> 承载（与 ExperimentDesigner 的模板一致），
    ''' 通过 <c>LoadCsv(Of SampleInfo)</c> 反序列化：<c>ID</c>/<c>sample_name</c>/<c>sample_info</c>
    ''' 落到强类型属性，其余列（<c>patient_id</c>、<c>stage</c>、<c>type</c>、<c>group</c> 等）
    ''' 由读取器自动收进 <see cref="SampleInfo.metadata"/> 字典。
    ''' </para>
    ''' <para>
    ''' <b>为什么保留原始 <see cref="SampleInfo"/> 而不是先拍平成 POCO</b>：不同数据集的元数据列并不统一，
    ''' 拍平会丢掉未知列；保留下字典之后，<see cref="ValueOf"/> 可以按名字取任意列，
    ''' 而强类型字段（ID / sample_name）仍然可用。
    ''' </para>
    ''' </remarks>
    Public Class CellMetadata

        ''' <summary>元数据表的原始记录（顺序与 CSV 行序一致）。</summary>
        Public Property Samples As SampleInfo()

        ''' <summary>barcode → 记录下标。</summary>
        Private m_index As Dictionary(Of String, Integer)

        ''' <summary>记录数。</summary>
        Public ReadOnly Property Count As Integer
            Get
                Return If(Samples Is Nothing, 0, Samples.Length)
            End Get
        End Property

        ''' <summary>从 CSV 反序列化元数据表。</summary>
        ''' <param name="path">元数据 CSV 路径（首行为表头）</param>
        Public Shared Function Load(path As String) As CellMetadata
            If String.IsNullOrWhiteSpace(path) Then
                Throw New ArgumentException("the metadata path is empty", NameOf(path))
            End If

            Dim records As List(Of SampleInfo) = path.LoadCsv(Of SampleInfo)()
            Dim metadata As New CellMetadata With {
                .Samples = records.ToArray
            }

            metadata.BuildIndex()

            Return metadata
        End Function

        Private Sub BuildIndex()
            m_index = New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For i As Integer = 0 To Count - 1
                Dim key As String = Samples(i).ID

                If String.IsNullOrEmpty(key) Then
                    Continue For
                End If
                If Not m_index.ContainsKey(key) Then
                    m_index(key) = i
                End If
            Next
        End Sub

        ''' <summary>按 barcode 查记录；找不到返回 <c>Nothing</c>。</summary>
        Public Function Find(barcode As String) As SampleInfo
            Dim i As Integer = -1

            If barcode Is Nothing OrElse m_index Is Nothing Then
                Return Nothing
            End If
            If Not m_index.TryGetValue(barcode, i) Then
                Return Nothing
            End If

            Return Samples(i)
        End Function

        ''' <summary>取某条记录的某个字段（先查强类型，再查 <c>metadata</c>）。</summary>
        Public Shared Function ValueOf(info As SampleInfo, name As String) As String
            If info Is Nothing OrElse String.IsNullOrEmpty(name) Then
                Return Nothing
            End If

            Select Case name.ToLowerInvariant()
                Case "id"
                    Return info.ID
                Case "sample_name"
                    Return info.sample_name
                Case "sample_info"
                    Return info.sample_info
            End Select

            Dim value As String = Nothing

            If info.metadata IsNot Nothing Then
                Call info.metadata.TryGetValue(name, value)
            End If

            Return value
        End Function

        ''' <summary>某个 barcode 的字段值。</summary>
        Public Function Field(barcode As String, name As String) As String
            Return ValueOf(Find(barcode), name)
        End Function

        ''' <summary>barcode 的期别标签（缺失返回空串）。</summary>
        Public Function StageOf(barcode As String) As String
            Return If(Field(barcode, "stage"), "")
        End Function

        ''' <summary>barcode 的组织类型（<c>tumor</c> / <c>normal</c>，缺失返回空串）。</summary>
        Public Function TissueOf(barcode As String) As String
            Return If(Field(barcode, "type"), "")
        End Function

        ''' <summary>某个期别下的全部 barcode（保持表内顺序）。</summary>
        Public Function BarcodesOfStage(stage As String) As String()
            Dim result As New List(Of String)

            For i As Integer = 0 To Count - 1
                Dim info As SampleInfo = Samples(i)

                If String.Equals(ValueOf(info, "stage"), stage, StringComparison.OrdinalIgnoreCase) Then
                    Call result.Add(info.ID)
                End If
            Next

            Return result.ToArray
        End Function

        ''' <summary>
        ''' "正常对照"的判定：优先用 <c>stage</c> 的空/NA 标签，其次退回 <c>type=normal</c>。
        ''' </summary>
        ''' <remarks>
        ''' <para>
        ''' 观察性数据里"健康"通常没有期别标签（本数据集的 <c>stage=NA</c>），
        ''' 但也存在"无分期肿瘤"的可能，因此不能只看标签。
        ''' </para>
        ''' <para>
        ''' 判定顺序：若 <c>NA</c> 组的 <c>type</c> 全部是 <c>normal</c>，则把 <c>NA</c> 当作正常对照；
        ''' 否则认为 <c>NA</c> 不可信，改用 <c>type=normal</c> 的子集。判定依据由
        ''' <see cref="CrossTab"/> 输出，需随模型清单一起持久化。
        ''' </para>
        ''' </remarks>
        Public ReadOnly Property NormalSelector As String
            Get
                Dim naTypes As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
                Dim total As Integer = 0

                For i As Integer = 0 To Count - 1
                    Dim stage As String = If(ValueOf(Samples(i), "stage"), "")

                    If Not IsMissingStage(stage) Then
                        Continue For
                    End If

                    Dim category As String = If(ValueOf(Samples(i), "type"), "NA")

                    naTypes(category) = 1 + CountOf(naTypes, category)
                    total += 1
                Next

                If total = 0 Then
                    Return "type=normal"
                End If
                If naTypes.Count = 1 AndAlso naTypes.ContainsKey("normal") Then
                    Return "stage=NA"
                End If

                Return "type=normal"
            End Get
        End Property

        ''' <summary>期别标签是否等价于"未标注"。</summary>
        Public Shared Function IsMissingStage(stage As String) As Boolean
            If String.IsNullOrWhiteSpace(stage) Then
                Return True
            End If

            Select Case stage.Trim().ToUpperInvariant()
                Case "NA", "N/A", "NAN", "NULL", "UNKNOWN", "-"
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        ''' <summary>正常对照的 barcode 集合（按 <see cref="NormalSelector"/> 的定义）。</summary>
        Public Function NormalBarcodes() As String()
            Dim result As New List(Of String)
            Dim selector As String = NormalSelector

            For i As Integer = 0 To Count - 1
                Dim info As SampleInfo = Samples(i)

                If selector = "stage=NA" Then
                    If IsMissingStage(ValueOf(info, "stage")) Then
                        Call result.Add(info.ID)
                    End If
                ElseIf String.Equals(ValueOf(info, "type"), "normal", StringComparison.OrdinalIgnoreCase) Then
                    Call result.Add(info.ID)
                End If
            Next

            Return result.ToArray
        End Function

        ''' <summary>
        ''' 期别 × 组织类型 × 分组的交叉统计（用于审计"NA 是否等于正常"的判定）。
        ''' </summary>
        Public Function CrossTab() As String
            Dim table As New SortedDictionary(Of String, Integer)(StringComparer.Ordinal)

            For i As Integer = 0 To Count - 1
                Dim info As SampleInfo = Samples(i)
                Dim stage As String = If(ValueOf(info, "stage"), "")
                Dim key As String = $"{stage}\t{ValueOf(info, "type")}\t{ValueOf(info, "group")}"

                table(key) = 1 + CountOf(table, key)
            Next

            Dim lines As New List(Of String) From {"stage" & vbTab & "type" & vbTab & "group" & vbTab & "cells"}

            For Each pair As KeyValuePair(Of String, Integer) In table
                Call lines.Add($"{pair.Key}{vbTab}{pair.Value}")
            Next

            Return String.Join(Environment.NewLine, lines)
        End Function

        ''' <summary>字典取值，缺失返回 0（VB 的 <c>TryGetValue</c> 需要 <c>ByRef</c> 目标，不能直接参与算术）。</summary>
        Private Shared Function CountOf(table As IDictionary(Of String, Integer), key As String) As Integer
            Dim value As Integer = 0

            Call table.TryGetValue(key, value)

            Return value
        End Function

        ''' <summary>缺失元数据的 barcode（按细胞轴核对时使用）。</summary>
        Public Function MissingBarcodes(cells As IEnumerable(Of String)) As String()
            Dim result As New List(Of String)

            For Each barcode As String In cells
                If Find(barcode) Is Nothing Then
                    Call result.Add(barcode)
                End If
            Next

            Return result.ToArray
        End Function

    End Class
End Namespace
