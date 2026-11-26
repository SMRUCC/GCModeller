Namespace Randomizer

    ''' <summary>
    ''' 扰动种子派生：<c>CRC32(扰动名)</c>（可选与任务种子组合），保证"同名扰动永远得到同一随机流"。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 为什么不用"第几个扰动"当种子：并行合成时扰动之间的完成顺序不确定，
    ''' 用序号会让结果依赖调度；用名字派生的种子与顺序、并行度都无关，
    ''' 因此"同一次任务重跑两遍"的计数矩阵必然逐元素相等。
    ''' </para>
    ''' <para>
    ''' 任务级种子与名字种子组合时用乘加混合（而不是异或），避免 <c>seed=1</c> 与
    ''' <c>CRC32="x"</c> 这类低比特巧合导致相邻任务共用同一条流。
    ''' </para>
    ''' </remarks>
    Public Module PerturbationSeed

        Private Const Polynomial As UInteger = &HEDB88320UI

        Private ReadOnly Table As UInteger() = BuildTable()

        ''' <summary>扰动名 → 32 位种子。</summary>
        Public Function FromName(name As String) As Long
            If name Is Nothing Then
                Return 0
            End If

            Dim crc As UInteger = &HFFFFFFFFUI

            For Each ch As Char In name
                Dim code As Integer = AscW(ch)

                ' 非 ASCII 字符按 UTF-16 的两个字节处理，保证确定性
                crc = (crc >> 8) Xor Table((crc Xor CUInt(code And &HFF)) And &HFFUI)

                If code > &HFF Then
                    crc = (crc >> 8) Xor Table((crc Xor CUInt((code >> 8) And &HFF)) And &HFFUI)
                End If
            Next

            Return CLng(crc Xor &HFFFFFFFFUI)
        End Function

        ''' <summary>任务种子与扰动名组合成一个有效种子（先截到 32 位，避免乘法溢出）。</summary>
        Public Function Combine(taskSeed As Long, name As String) As Long
            Dim value As Long = FromName(name)
            Dim mixed As Long = ((taskSeed And &HFFFFFFFFL) * 1000003L) And &HFFFFFFFFL

            Return mixed Xor value Xor &H5DEECE66DL
        End Function

        ''' <summary>为某个扰动构造独立的随机源。</summary>
        Public Function Create(taskSeed As Long, name As String) As Pcg64
            Return New Pcg64(Combine(taskSeed, name))
        End Function

        Private Function BuildTable() As UInteger()
            Dim table(255) As UInteger

            For i As Integer = 0 To 255
                Dim value As UInteger = CUInt(i)

                For bit As Integer = 1 To 8
                    If (value And 1UI) = 1UI Then
                        value = (value >> 1) Xor Polynomial
                    Else
                        value >>= 1
                    End If
                Next

                table(i) = value
            Next

            Return table
        End Function

    End Module
End Namespace
