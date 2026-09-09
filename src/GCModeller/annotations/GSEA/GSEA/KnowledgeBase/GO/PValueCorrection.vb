Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Statistics

Namespace GO

    ''' <summary>
    ''' 富集分析的多重假设检验校正
    ''' </summary>
    ''' <remarks>
    ''' 富集分析会对成百上千个功能词条同时进行假设检验，所以必须对
    ''' 得到的p值做多重假设检验校正，否则假阳性会非常高。
    ''' </remarks>
    Public Module PValueCorrection

        ''' <summary>
        ''' Benjamini-Hochberg (BH) FDR校正
        ''' </summary>
        ''' <param name="pvalues">原始的p值向量</param>
        ''' <returns>
        ''' 与输入向量等长的校正之后的q值向量，结果保证单调不减
        ''' (即p值越小，校正之后的q值也越小)
        ''' </returns>
        ''' <remarks>
        ''' BH的计算方式为：将p值从小到大排序之后，第``i``个p值对应的
        ''' ``q = p * n / i``；然后从最大的rank开始反向取累计最小值，
        ''' 从而保证q值的单调性。
        ''' </remarks>
        Public Function BHCorrection(pvalues As Double()) As Double()
            Dim n As Integer = pvalues.Length
            Dim q As Double() = New Double(n - 1) {}

            If n = 0 Then
                Return q
            End If

            Dim index As Integer() = New Integer(n - 1) {}

            For i As Integer = 0 To n - 1
                index(i) = i
            Next

            Array.Sort(index, Function(a As Integer, b As Integer) pvalues(a).CompareTo(pvalues(b)))

            Dim prev As Double = 1.0

            For rank As Integer = n To 1 Step -1
                Dim i As Integer = index(rank - 1)
                Dim value As Double = pvalues(i) * n / rank

                If value < prev Then
                    prev = value
                End If
                If prev > 1.0 Then
                    prev = 1.0
                End If

                q(i) = prev
            Next

            Return q
        End Function

        ''' <summary>
        ''' Bonferroni校正
        ''' </summary>
        ''' <param name="pvalues"></param>
        ''' <returns></returns>
        Public Function Bonferroni(pvalues As Double()) As Double()
            Dim n As Integer = pvalues.Length
            Dim q As Double() = New Double(n - 1) {}

            If n = 0 Then
                Return q
            End If

            For i As Integer = 0 To n - 1
                Dim value As Double = pvalues(i) * n

                q(i) = If(value > 1.0, 1.0, value)
            Next

            Return q
        End Function

        ''' <summary>
        ''' 对富集计算的结果做BH校正，并且将校正之后的q值写入
        ''' <see cref="IStatFDR.adjPVal"/>属性之中
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="result"></param>
        ''' <returns>
        ''' 按照原始的p值从小到大排序之后的结果
        ''' </returns>
        <Extension>
        Public Function FDRCorrection(Of T As IStatFDR)(result As IEnumerable(Of T)) As T()
            Dim array As T() = result.SafeQuery.ToArray
            Dim pvalues As Double() = array _
                .Select(Function(term) term.pValue) _
                .ToArray
            Dim q As Double() = BHCorrection(pvalues)

            For i As Integer = 0 To array.Length - 1
                array(i).adjPVal = q(i)
            Next

            Return array _
                .OrderBy(Function(term) term.pValue) _
                .ToArray
        End Function

        ''' <summary>
        ''' 对富集计算的结果做Bonferroni校正
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="result"></param>
        ''' <returns></returns>
        <Extension>
        Public Function BonferroniCorrection(Of T As IStatFDR)(result As IEnumerable(Of T)) As T()
            Dim array As T() = result.SafeQuery.ToArray
            Dim pvalues As Double() = array _
                .Select(Function(term) term.pValue) _
                .ToArray
            Dim q As Double() = Bonferroni(pvalues)

            For i As Integer = 0 To array.Length - 1
                array(i).adjPVal = q(i)
            Next

            Return array _
                .OrderBy(Function(term) term.pValue) _
                .ToArray
        End Function
    End Module
End Namespace
