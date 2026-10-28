Imports System.Globalization
Imports System.Runtime.CompilerServices
Imports System.Text
Imports SMRUCC.genomics.Analysis.SGrid.GRN

Namespace IO

    ''' <summary>
    ''' The export of the fitted coefficient matrix (the net regulatory elasticity g - h).
    ''' (拟合得到的系数矩阵导出：行为目标基因、列为调控因子)
    ''' </summary>
    ''' <remarks>
    ''' 两种输出格式：
    ''' 1. <see cref="SaveCoefficientMatrix"/> 输出宽表形式的矩阵，便于人工与其它脚本直接读取；
    ''' 2. <see cref="SaveFormulaTable"/> 输出与弹性网络模型（elastic net）一致的
    '''    「每个靶基因一行、调控因子与系数用分号分隔」的长表，
    '''    这样 G:\vcc2026\analysis 之中的 R 分析脚本可以不加修改地读取本模型的系数矩阵。
    ''' 
    ''' 矩阵元素的语义是净调控弹性 <c>g_ij - h_ij</c>：
    ''' 正值表示调控因子 j 激活靶基因 i，负值表示抑制，零表示不存在调控关系。
    ''' </remarks>
    Public Module CoefficientMatrix

        ''' <summary>
        ''' 把拟合得到的系数矩阵（净调控弹性）导出为宽表 CSV
        ''' </summary>
        ''' <param name="network">拟合完成的网络模型</param>
        ''' <param name="file">目标 csv 文件</param>
        ''' <param name="useOriginal">
        ''' 是否使用原始的基因符号（如 <c>NKX3-1</c>）而不是规整化之后的编号（<c>NKX3_1</c>）；
        ''' 面向生物学注释（GO/KEGG 富集、先验知识比对）的导出应当使用原始符号
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function SaveCoefficientMatrix(network As SSystemNetwork,
                                              file As String,
                                              Optional useOriginal As Boolean = False) As Boolean
            If network Is Nothing Then
                Throw New ArgumentNullException(NameOf(network))
            End If

            Dim parameters As SSystemParameters = network.Parameters
            Dim names As String() = displayNames(parameters, useOriginal)
            Dim net As Double()() = parameters.NetElasticity()
            Dim sb As New StringBuilder()

            Call sb.Append("target")

            For Each name As String In names
                Call sb.Append(","c)
                Call sb.Append(name)
            Next

            Call sb.AppendLine()

            For i As Integer = 0 To names.Length - 1
                Call sb.Append(names(i))

                For j As Integer = 0 To names.Length - 1
                    Call sb.Append(","c)

                    If net(i)(j) = 0 Then
                        Call sb.Append("0")
                    Else
                        Call sb.Append(num(net(i)(j)))
                    End If
                Next

                Call sb.AppendLine()
            Next

            Call sb.SaveTo(file)

            Return True
        End Function

        ''' <summary>
        ''' 把拟合结果导出为弹性网络模型格式的系数表
        ''' </summary>
        ''' <param name="network">拟合完成的网络模型</param>
        ''' <param name="file">目标 csv 文件</param>
        ''' <param name="useOriginal">是否使用原始的基因符号（见 <see cref="SaveCoefficientMatrix"/>）</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 列的语义（与 elastic net 模型的输出一致，故 R 分析脚本可以直接复用）：
        ''' <list type="bullet">
        ''' <item><c>target</c>：靶基因</item>
        ''' <item><c>regulators</c>：该靶基因的候选调控因子（分号分隔）</item>
        ''' <item><c>weights</c>：与候选调控因子一一对应的净调控弹性（未被选中的为 0）</item>
        ''' <item><c>r2</c>：该基因斜率拟合的决定系数</item>
        ''' <item><c>nonZeroWeights</c>：非零系数的数量</item>
        ''' <item><c>IsValid</c>：该基因的参数求解是否成功</item>
        ''' </list>
        ''' </remarks>
        <Extension>
        Public Function SaveFormulaTable(network As SSystemNetwork,
                                         file As String,
                                         Optional useOriginal As Boolean = False) As Boolean
            If network Is Nothing Then
                Throw New ArgumentNullException(NameOf(network))
            End If

            Dim parameters As SSystemParameters = network.Parameters
            Dim names As String() = displayNames(parameters, useOriginal)
            Dim net As Double()() = parameters.NetElasticity()
            Dim sb As New StringBuilder()

            Call sb.AppendLine("target,regulators,weights,r2,nonZeroWeights,IsValid")

            For i As Integer = 0 To names.Length - 1
                Dim candidates As Integer() = parameters.CandidateOf(i)
                Dim regulators As New StringBuilder()
                Dim weights As New StringBuilder()
                Dim nonZero As Integer = 0

                For k As Integer = 0 To candidates.Length - 1
                    Dim j As Integer = candidates(k)
                    Dim value As Double = If(j >= 0 AndAlso j < names.Length, net(i)(j), 0)

                    If k > 0 Then
                        Call regulators.Append(";"c)
                        Call weights.Append(";"c)
                    End If

                    Call regulators.Append(names(j))
                    Call weights.Append(num(value))

                    If value <> 0 Then
                        nonZero += 1
                    End If
                Next

                Dim valid As Boolean = parameters.Converged(i) AndAlso Not Double.IsNaN(parameters.RSquared(i))

                Call sb.Append(names(i))
                Call sb.Append(","c)
                Call sb.Append(regulators.ToString())
                Call sb.Append(","c)
                Call sb.Append(weights.ToString())
                Call sb.Append(","c)
                Call sb.Append(num(parameters.RSquared(i)))
                Call sb.Append(","c)
                Call sb.Append(nonZero.ToString())
                Call sb.Append(","c)
                Call sb.AppendLine(valid.ToString())
            Next

            Call sb.SaveTo(file)

            Return True
        End Function

        ''' <summary>
        ''' 导出所使用的基因符号：规整化编号或者原始符号
        ''' </summary>
        ''' <param name="parameters"></param>
        ''' <param name="useOriginal"></param>
        ''' <returns></returns>
        Private Function displayNames(parameters As SSystemParameters, useOriginal As Boolean) As String()
            If useOriginal AndAlso Not parameters.OriginalNames.IsNullOrEmpty AndAlso parameters.OriginalNames.Length = parameters.nGenes Then
                Return parameters.OriginalNames
            Else
                Return parameters.GeneNames
            End If
        End Function

        ''' <summary>
        ''' 以不变文化格式化一个双精度数
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Private Function num(x As Double) As String
            If Double.IsNaN(x) OrElse Double.IsInfinity(x) Then
                Return "0"
            Else
                Return x.ToString("G10", CultureInfo.InvariantCulture)
            End If
        End Function
    End Module
End Namespace
