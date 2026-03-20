Imports System.ComponentModel.DataAnnotations.Schema
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Language

Public Class KOFamScan

    Public Property gene_name As String
    Public Property KO As String
    Public Property thrshld As Double
    Public Property score As Double

    <Column("E-value")>
    Public Property Evalue As Double

    ''' <summary>
    ''' the KO definition
    ''' </summary>
    ''' <returns></returns>
    Public Property definition As String

    Public Property flag As Boolean

    Public Shared Iterator Function ParseTable(s As Stream) As IEnumerable(Of KOFamScan)
        Dim line As Value(Of String) = ""

        Using text As New StreamReader(s)
            Dim readLine = TqdmWrapper.StreamReader(text)
            Dim header As String = readLine()

            Do While (line = readLine()) IsNot Nothing
                ' 跳过空行
                If String.IsNullOrWhiteSpace(line) Then
                    Continue Do
                End If

                ' 跳过注释行（以#开头）
                If line.Trim.StartsWith("#") Then
                    Continue Do
                End If

                Dim hit As KOFamScan = ParseTableLine(line)

                If Not hit Is Nothing Then
                    Yield hit
                End If
            Loop
        End Using
    End Function

    Private Shared Function ParseTableLine(line As String) As KOFamScan
        ' 使用制表符分割行
        Dim parts As String() = line.Split(New Char() {ControlChars.Tab}, StringSplitOptions.RemoveEmptyEntries)

        ' 根据是否以*开头判断数据格式
        Dim startIndex As Integer = 0
        Dim hasFlag As Boolean = False

        If parts.Length > 0 AndAlso parts(0).Trim() = "*" Then
            ' 数据行以*开头，设置flag为True
            hasFlag = True
            startIndex = 1
        End If

        ' 验证字段数量是否足够（至少需要5个字段：gene_name, KO, thrshld, score, E-value）
        ' definition可能为空，所以最少需要5个字段
        Dim dataLength As Integer = parts.Length - startIndex
        If dataLength < 5 Then
            ' 记录错误但继续处理下一行
            Return Nothing
        End If

        ' 创建KOFamScan对象
        Dim item As New KOFamScan() With {
            .gene_name = parts(startIndex).Trim(),
            .KO = parts(startIndex + 1).Trim(),
            .flag = hasFlag
        }

        ' 解析数值字段
        Double.TryParse(parts(startIndex + 2).Trim(), item.thrshld)
        Double.TryParse(parts(startIndex + 3).Trim(), item.score)

        ' 处理E-value（可能使用科学计数法）
        Dim evalueStr As String = parts(startIndex + 4).Trim()
        Double.TryParse(evalueStr, Globalization.NumberStyles.Float Or Globalization.NumberStyles.AllowExponent,
                       Globalization.CultureInfo.InvariantCulture, item.Evalue)

        ' 处理definition字段（可能包含空格，是最后一个字段）
        If dataLength >= 6 Then
            item.definition = parts(startIndex + 5).Trim()
        End If

        Return item
    End Function
End Class
