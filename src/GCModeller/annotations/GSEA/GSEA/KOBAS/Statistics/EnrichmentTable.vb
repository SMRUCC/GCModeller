Imports System.Globalization
Imports System.Text
Imports Microsoft.VisualBasic.Linq

Namespace KOBAS

    ''' <summary>
    ''' 富集计算的结果表（KOBAS <c>discover.TestResult</c> 的等价物）。
    ''' </summary>
    ''' <remarks>
    ''' 与 KOBAS 原实现的差异：KOBAS 以「字符串列表 + 动态列插入」的方式携带结果，
    ''' 这里改为强类型的 <see cref="EnrichmentRow"/> 集合，排序与 FDR 校正直接作用
    ''' 在对象之上，导出时再序列化为制表符分隔文本。
    ''' </remarks>
    Public Class EnrichmentTable

        ''' <summary>
        ''' 默认的结果表列名（不含 FDR 校正列）。
        ''' </summary>
        Public Shared ReadOnly DefaultColumns As String() = {
            "Term",
            "Database",
            "ID",
            "Input number",
            "Background number",
            "P-Value",
            "Input"
        }

        ''' <summary>
        ''' 当前结果表的列名。
        ''' </summary>
        ''' <returns></returns>
        Public Property Title As New List(Of String)

        ''' <summary>
        ''' 结果行。
        ''' </summary>
        ''' <returns></returns>
        Public Property Rows As New List(Of EnrichmentRow)

        ''' <summary>
        ''' 创建一个空的结果表。
        ''' </summary>
        Public Sub New()
            Call Title.AddRange(DefaultColumns)
        End Sub

        ''' <summary>
        ''' 使用给定的结果行创建一个结果表。
        ''' </summary>
        ''' <param name="rows">结果行集合。</param>
        Public Sub New(rows As IEnumerable(Of EnrichmentRow))
            Call Me.New()

            If rows IsNot Nothing Then
                Call Me.Rows.AddRange(rows)
            End If
        End Sub

        ''' <summary>
        ''' 对当前结果表执行多重检验校正，并写入 <see cref="EnrichmentRow.CorrectedPValue"/>。
        ''' </summary>
        ''' <param name="method">校正方法，默认 BH。</param>
        Public Sub AddFdr(Optional method As FdrMethod = FdrMethod.BH)
            If Rows.Count = 0 Then
                Return
            End If

            Dim adjusted As Double() = FdrAdjust.Adjust(Rows.Select(Function(row) row.PValue), method)

            For i As Integer = 0 To Rows.Count - 1
                Rows(i).CorrectedPValue = adjusted(i)
            Next

            If Not Title.Contains("Corrected P-Value") Then
                Title.Insert(Title.IndexOf("Input"), "Corrected P-Value")
            End If
        End Sub

        ''' <summary>
        ''' 按 p 值升序排序（NaN 视为最大值，排在最后）。
        ''' </summary>
        Public Sub SortByPValue()
            Rows = Rows _
                .OrderBy(Function(row) If(Double.IsNaN(row.PValue), Double.MaxValue, row.PValue)) _
                .ToList
        End Sub

        ''' <summary>
        ''' 按前景命中基因数量降序排序（KOBAS 的 <c>res.sort(key = -2, order = 1)</c> 频数模式）。
        ''' </summary>
        Public Sub SortByCount()
            Rows = Rows _
                .OrderByDescending(Function(row) row.InputNumber) _
                .ToList
        End Sub

        ''' <summary>
        ''' 导出为制表符分隔的文本（第一行为列名）。
        ''' </summary>
        ''' <returns></returns>
        Public Function ToTsv() As String
            Dim text As New StringBuilder

            Call text.AppendLine(String.Join(ControlChars.Tab, Title))

            For Each row As EnrichmentRow In Rows
                Dim cells As New List(Of String) From {
                    row.Term,
                    row.Database,
                    row.ID,
                    row.InputNumber.ToString(CultureInfo.InvariantCulture),
                    row.BackgroundNumber.ToString(CultureInfo.InvariantCulture),
                    row.PValue.ToString("G8", CultureInfo.InvariantCulture)
                }

                If Title.Contains("Corrected P-Value") Then
                    cells.Add(row.CorrectedPValue.ToString("G8", CultureInfo.InvariantCulture))
                End If

                cells.Add(row.Input.JoinBy("|"))

                Call text.AppendLine(String.Join(ControlChars.Tab, cells))
            Next

            Return text.ToString()
        End Function

        Public Overrides Function ToString() As String
            Return ToTsv()
        End Function
    End Class
End Namespace
