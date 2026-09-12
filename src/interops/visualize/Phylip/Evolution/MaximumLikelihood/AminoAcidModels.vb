Imports System.Globalization
Imports System.IO
Imports System.Reflection

Namespace Evolution.MaximumLikelihood

    ''' <summary>
    ''' 内置的氨基酸经验替换模型（Dayhoff / JTT / WAG / LG）数据加载器。
    ''' </summary>
    ''' <remarks>
    ''' 速率矩阵数据以嵌入资源（embed resource）的形式存放于
    ''' <c>Evolution/MaximumLikelihood/Models/Data/*.dat</c>，其格式与 PAML 一致：
    ''' 先是 190 个下三角交换率（按行主序，行 i 有 i 个元素），随后是 20 个平衡频率。
    ''' 氨基酸顺序固定为 <c>A R N D C Q E G H I L K M F P S T W Y V</c>（与
    ''' <see cref="Models.CharacterSet.Protein"/> 一致）。
    ''' </remarks>
    Public Module AminoAcidModels

        ''' <summary>
        ''' 氨基酸状态数目（20）
        ''' </summary>
        Public Const States As Integer = 20

        ''' <summary>
        ''' 交换率矩阵的元素数目（20*19/2 = 190）
        ''' </summary>
        Public Const RateCount As Integer = 190

        ''' <summary>
        ''' 读取指定模型的对称交换率矩阵与平衡频率。
        ''' </summary>
        ''' <returns>
        ''' <c>Exchangeability</c>：20x20 的对称矩阵，对角线为 0；
        ''' <c>Pi</c>：归一化之后的平衡频率（和为 1）。
        ''' </returns>
        Public Function LoadRaw(model As AminoAcidModel) As (Exchangeability As Double()(), Pi As Double())
            Dim fileName As String = model.ToString().ToLower() & ".dat"
            Dim tokens As Double() = ReadNumbers(fileName)

            If tokens.Length < RateCount + States Then
                Throw New InvalidDataException($"替换模型数据文件 '{fileName}' 的数值个数不足（{tokens.Length}）。")
            End If

            Dim s(States - 1)() As Double

            For i As Integer = 0 To States - 1
                s(i) = New Double(States - 1) {}
            Next

            Dim k As Integer = 0

            ' 下三角：第 i 行（i 从 1 开始）有 i 个元素
            For i As Integer = 1 To States - 1
                For j As Integer = 0 To i - 1
                    s(i)(j) = tokens(k)
                    s(j)(i) = tokens(k)
                    k += 1
                Next
            Next

            Dim pi(States - 1) As Double
            Dim total As Double = 0

            For i As Integer = 0 To States - 1
                pi(i) = tokens(k)
                total += pi(i)
                k += 1
            Next

            If total > 0 Then
                For i As Integer = 0 To States - 1
                    pi(i) /= total
                Next
            End If

            Return (s, pi)
        End Function

        ''' <summary>
        ''' 从嵌入资源读取指定文件的全部数值（最多 210 个：190 个速率 + 20 个频率）。
        ''' </summary>
        Private Function ReadNumbers(fileName As String) As Double()
            Dim asm As Reflection.Assembly = GetType(AminoAcidModels).Assembly
            Dim suffix As String = "." & fileName
            Dim resourceName As String = asm _
                .GetManifestResourceNames() _
                .FirstOrDefault(Function(name) name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))

            If resourceName Is Nothing Then
                Throw New FileNotFoundException(
                    $"未能在程序集之中找到氨基酸替换模型数据文件 '{fileName}'，请确认其已作为 EmbeddedResource 打包。")
            End If

            Dim values As New List(Of Double)(RateCount + States)

            Using stream = asm.GetManifestResourceStream(resourceName)
                Using reader As New IO.StreamReader(stream)
                    While Not reader.EndOfStream AndAlso values.Count < RateCount + States
                        Dim line As String = reader.ReadLine()

                        If line Is Nothing Then
                            Exit While
                        End If

                        line = line.Trim()

                        If line.StartsWith("//") Then
                            Exit While
                        End If

                        For Each token As String In line.Split({" "c, ControlChars.Tab}, StringSplitOptions.RemoveEmptyEntries)
                            Dim value As Double

                            If Double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, value) Then
                                values.Add(value)

                                If values.Count >= RateCount + States Then
                                    Exit For
                                End If
                            End If
                        Next
                    End While
                End Using
            End Using

            Return values.ToArray()
        End Function
    End Module

End Namespace
