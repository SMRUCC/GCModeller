Imports System.Linq

Namespace StatTests

    ''' <summary>
    ''' 多重检验校正。
    ''' </summary>
    Public Module MultipleTesting

        ''' <summary>
        ''' Benjamini-Hochberg FDR 校正，返回与输入同序的 q 值。
        ''' </summary>
        ''' <param name="pvalues">原始 p 值（允许包含 NaN，NaN 会被原样保留）</param>
        ''' <remarks>
        ''' q(i) = min_{j >= i} ( p(j) * n / j )，其中 p(j) 为升序排列后的第 j 个 p 值，
        ''' n 为有效（非 NaN）检验数。
        ''' </remarks>
        Public Function BenjaminiHochberg(pvalues As IEnumerable(Of Double)) As Double()
            Dim values As Double() = pvalues.ToArray
            Dim result As Double() = New Double(values.Length - 1) {}

            For i As Integer = 0 To values.Length - 1
                result(i) = Double.NaN
            Next

            Dim valid As Integer() = Enumerable.Range(0, values.Length) _
                .Where(Function(i) Not Double.IsNaN(values(i))) _
                .ToArray

            If valid.Length = 0 Then
                Return result
            End If

            Dim order As Integer() = valid _
                .OrderBy(Function(i) values(i)) _
                .ToArray
            Dim n As Integer = order.Length
            Dim running As Double = Double.MaxValue

            For rank As Integer = n To 1 Step -1
                Dim idx As Integer = order(rank - 1)
                Dim q As Double = values(idx) * n / rank

                If q > 1 Then
                    q = 1
                End If
                If q < running Then
                    running = q
                End If

                result(idx) = running
            Next

            Return result
        End Function

    End Module

End Namespace
