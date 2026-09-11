Namespace Statistics

    ''' <summary>
    ''' 多重检验校正封装，对应 TSSAR 的 ``--mtc`` 选项。
    ''' </summary>
    ''' <remarks>
    ''' 具体算法复用基础库 <c>Microsoft.VisualBasic.Math.Extensions</c> 之中的
    ''' ``p.adjust`` 系列实现（与 R 的 p.adjust 保持一致）。
    ''' </remarks>
    Public Module MultipleTesting

        ''' <summary>
        ''' 对 p 值数组执行多重检验校正。
        ''' </summary>
        ''' <param name="pvalues">原始 p 值；允许包含 <see cref="Double.NaN"/>。</param>
        ''' <param name="method">
        ''' 校正方法名称（不区分大小写）：``fdr``/``BH``、``bonferroni``、``holm``、
        ''' ``hochberg``、``hommel``、``BY``；为空或 ``none`` 时不做校正。
        ''' </param>
        ''' <returns>校正后的 p 值（保持输入顺序，NaN 原样保留）。</returns>
        Public Function Adjust(pvalues As Double(), method As String) As Double()
            If pvalues Is Nothing OrElse pvalues.Length = 0 Then
                Return pvalues
            End If
            If String.IsNullOrWhiteSpace(method) Then
                Return pvalues
            End If

            Dim adjustMethod As Microsoft.VisualBasic.Math.Extensions.PValueAdjustMethod

            Select Case method.Trim().ToLowerInvariant()
                Case "none"
                    Return pvalues
                Case "fdr", "bh"
                    adjustMethod = Microsoft.VisualBasic.Math.Extensions.PValueAdjustMethod.BH
                Case "bonferroni"
                    adjustMethod = Microsoft.VisualBasic.Math.Extensions.PValueAdjustMethod.Bonferroni
                Case "holm"
                    adjustMethod = Microsoft.VisualBasic.Math.Extensions.PValueAdjustMethod.Holm
                Case "hochberg"
                    adjustMethod = Microsoft.VisualBasic.Math.Extensions.PValueAdjustMethod.Hochberg
                Case "hommel"
                    adjustMethod = Microsoft.VisualBasic.Math.Extensions.PValueAdjustMethod.Hommel
                Case "by"
                    adjustMethod = Microsoft.VisualBasic.Math.Extensions.PValueAdjustMethod.BY
                Case Else
                    Throw New System.ArgumentException($"Unknown multiple testing correction method: '{method}'")
            End Select

            ' 剔除 NaN 之后再校正（与 R 的 na.rm 行为一致），最后写回原位
            Dim valid As New List(Of Double)
            Dim indices As New List(Of Integer)

            For i As Integer = 0 To pvalues.Length - 1
                If Not Double.IsNaN(pvalues(i)) Then
                    valid.Add(pvalues(i))
                    indices.Add(i)
                End If
            Next

            Dim result As Double() = DirectCast(pvalues.Clone(), Double())

            If valid.Count = 0 Then
                Return result
            End If

            Dim adjusted As Double() = Microsoft.VisualBasic.Math.Extensions.PValueAdjust(valid, adjustMethod)

            For i As Integer = 0 To adjusted.Length - 1
                result(indices(i)) = adjusted(i)
            Next

            Return result
        End Function
    End Module
End Namespace
