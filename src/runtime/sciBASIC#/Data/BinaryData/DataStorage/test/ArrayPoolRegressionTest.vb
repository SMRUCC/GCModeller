Imports System
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Data.IO.Numpy

''' <summary>
''' <c>ArrayPool</c> 池化改造的回归测试（numpy 写出路径）。
'''
''' 池化改造把 numpy 的整列写出缓冲换成了从 <c>ArrayPool</c> 租用的复用缓冲，
''' 因此必须验证「写出的字节序列与改造前完全一致」。这里通过
''' <see cref="NpyFile.Write(String, Double(), Integer(), Boolean)"/> 落盘再由
''' <see cref="NpyFile.Read(String)"/> 读回，逐字节比对数据区。
''' </summary>
Module ArrayPoolRegressionTest

    ''' <summary>
    ''' 运行全部回归用例，并把结果通过 <see cref="DebugAudit"/> 写入报告。
    ''' </summary>
    Public Function Run() As Integer
        Dim workDir As String = IO.Path.Combine(IO.Path.GetTempPath(), "npy-pool-regression")

        Call Directory.CreateDirectory(workDir)
        Call DebugAudit.Reset()

        Console.WriteLine("ArrayPool 池化改造回归测试")

        Call RoundTripDoubles(workDir)
        Call RoundTripBooleans(workDir)
        Call RoundTripStrings(workDir)

        Dim reportFile As String = IO.Path.Combine(workDir, "arraypool-regression-report.json")

        Console.WriteLine()
        Console.WriteLine(DebugAudit.Dump())

        Call DebugAudit.SaveReport(reportFile)
        Console.WriteLine($"测试报告：{reportFile}")

        If DebugAudit.FailureCount = 0 Then
            Console.WriteLine("全部断言通过。")
            Return 0
        Else
            Console.WriteLine($"共 {DebugAudit.FailureCount} 项断言失败。")
            Return 1
        End If
    End Function

    ''' <summary>&lt;f8 数组的往返一致性。</summary>
    Private Sub RoundTripDoubles(workDir As String)
        Dim values As Double() = {1.5, -2.25, 0.0, 3.75E+300, -1.0E-200}
        Dim file As String = IO.Path.Combine(workDir, "pool-f8.npy")

        Call NpyFile.Write(file, values)

        Dim array As NpyArray = NpyFile.Read(file)
        Dim raw As Byte() = If(array.RawData, New Byte() {})
        Dim expected As Byte() = New Byte(values.Length * 8 - 1) {}

        Call Buffer.BlockCopy(values, 0, expected, 0, expected.Length)

        Call DebugAudit.AreEqual(expected.Length, raw.Length, "f8：数据区长度")
        Call DebugAudit.Check(BytesEqual(expected, raw), "f8：数据区逐字节一致")
    End Sub

    ''' <summary>|b1 数组的往返一致性（池化缓冲必须被显式清零后再写入）。</summary>
    Private Sub RoundTripBooleans(workDir As String)
        Dim values As Boolean() = {True, False, True, True, False, False, True, False, True}
        Dim file As String = IO.Path.Combine(workDir, "pool-b1.npy")

        Call NpyFile.Write(file, values)

        Dim array As NpyArray = NpyFile.Read(file)
        Dim raw As Byte() = If(array.RawData, New Byte() {})

        Call DebugAudit.AreEqual(values.Length, raw.Length, "b1：数据区长度")

        For i As Integer = 0 To values.Length - 1
            Call DebugAudit.AreEqual(If(values(i), CByte(1), CByte(0)), raw(i), $"b1：第 {i} 个元素")
        Next
    End Sub

    ''' <summary>&lt;U[n] 字符串数组的往返一致性（尾部必须是零填充）。</summary>
    Private Sub RoundTripStrings(workDir As String)
        Dim values As String() = {"ab", "cde", ""}
        Dim capacity As Integer = 3
        Dim itemSize As Integer = capacity * 4
        Dim file As String = IO.Path.Combine(workDir, "pool-U3.npy")

        Call NpyFile.Write(file, values)

        Dim array As NpyArray = NpyFile.Read(file)
        Dim raw As Byte() = If(array.RawData, New Byte() {})
        Dim expected As Byte() = New Byte(values.Length * itemSize - 1) {}

        For i As Integer = 0 To values.Length - 1
            Dim encoded As Byte() = EncodeUtf32(values(i), capacity)

            Call Buffer.BlockCopy(encoded, 0, expected, i * itemSize, encoded.Length)
        Next

        Call DebugAudit.AreEqual(expected.Length, raw.Length, "U3：数据区长度")
        Call DebugAudit.Check(BytesEqual(expected, raw), "U3：数据区逐字节一致（含零填充）")
    End Sub

    ''' <summary>按 numpy &lt;U 的定长 UTF-32 规则编码一个字符串。</summary>
    Private Function EncodeUtf32(value As String, capacity As Integer) As Byte()
        Dim buffer As Byte() = New Byte(capacity * 4 - 1) {}

        If String.IsNullOrEmpty(value) Then
            Return buffer
        End If

        Dim i As Integer = 0
        Dim at As Integer = 0

        While i < value.Length AndAlso at \ 4 < capacity
            Dim codePoint As Integer

            If Char.IsHighSurrogate(value(i)) AndAlso i + 1 < value.Length AndAlso Char.IsLowSurrogate(value(i + 1)) Then
                codePoint = Char.ConvertToUtf32(value(i), value(i + 1))
                i += 2
            Else
                codePoint = AscW(value(i))

                If codePoint < 0 Then
                    codePoint += 65536
                End If

                i += 1
            End If

            Call BitConverter.TryWriteBytes(buffer.AsSpan(at, 4), codePoint)
            at += 4
        End While

        Return buffer
    End Function

    Private Function BytesEqual(expected As Byte(), actual As Byte()) As Boolean
        If expected Is Nothing OrElse actual Is Nothing Then
            Return expected Is actual
        End If
        If expected.Length <> actual.Length Then
            Return False
        End If

        For i As Integer = 0 To expected.Length - 1
            If expected(i) <> actual(i) Then
                Return False
            End If
        Next

        Return True
    End Function

End Module
