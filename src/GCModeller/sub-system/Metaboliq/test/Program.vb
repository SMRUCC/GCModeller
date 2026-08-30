Imports System.Text
Imports Microsoft.VisualBasic.DeepLearning.LiquidNeuralNetwork
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

''' <summary>
''' 临时梯度校验：对比解析梯度（反向模式 AD）与中心差分数值梯度
''' </summary>
Module Program

    Sub Main(args As String())
        Call CheckMode(LiquidMode.CT_RNN, "rk4")
        Call CheckMode(LiquidMode.LTC, "rk4")
        Call CheckMode(LiquidMode.LTC, "heun")
        Call CheckMode(LiquidMode.LTC, "euler")
        Call CheckMode(LiquidMode.CFC, "cfc")
    End Sub

    Private Sub CheckMode(mode As LiquidMode, solver As String)
        Dim seed As Integer = 42
        Dim T = 6, N = 3, H = 4, O = 2
        Dim dt = 0.1

        Dim net As New LiquidNeuralNetwork(N, H, O, 1, "tanh", "none", seed, mode)
        net.SolverType = solver

        Dim inputs = Tensor.Random({T, N}, -0.5F, 0.5F, seed + 7)
        Dim targets = Tensor.Random({T, O}, -0.5F, 0.5F, seed + 9)

        ' ---------- 解析梯度 ----------
        Dim analyticLoss = RollOut(net, inputs, targets, dt, backward:=True)

        Dim pairs = net.GetParameterPairs()
        Dim analytic(pairs.Count - 1)() As Double

        For p = 0 To pairs.Count - 1
            Dim g = pairs(p).Gradient
            analytic(p) = New Double(g.Length - 1) {}
            For i = 0 To g.Length - 1
                analytic(p)(i) = g(i)
            Next
        Next

        ' ---------- 数值梯度 ----------
        Dim eps = 0.00001
        Dim worstRel As Double = 0
        Dim worstName As String = ""
        Dim sb As New StringBuilder()

        For p = 0 To pairs.Count - 1
            Dim param = pairs(p).Value
            Dim maxAbs As Double = 0
            Dim sumRel As Double = 0

            For i = 0 To param.Length - 1
                Dim old = param(i)

                param(i) = old + eps
                Dim lp = RollOut(net, inputs, targets, dt, backward:=False)
                param(i) = old - eps
                Dim lm = RollOut(net, inputs, targets, dt, backward:=False)
                param(i) = old

                Dim fd As Double = (lp - lm) / (2 * eps)
                Dim an As Double = analytic(p)(i)
                Dim denom As Double = Math.Max(1.0, Math.Abs(fd) + Math.Abs(an))
                Dim rel As Double = Math.Abs(fd - an) / denom

                If rel > maxAbs Then maxAbs = rel
                sumRel += rel
            Next

            Dim avg = sumRel / Math.Max(1, param.Length)

            sb.AppendLine($"   {pairs(p).Name,-46} maxRel={maxAbs:E3}  avgRel={avg:E3}")

            If maxAbs > worstRel Then
                worstRel = maxAbs
                worstName = pairs(p).Name
            End If
        Next

        Console.WriteLine($"[{mode}/{solver}] loss={analyticLoss:F6}  参数组={pairs.Count}  最差相对误差={worstRel:E3} ({worstName})")
        Console.Write(sb.ToString())
        Console.WriteLine()
    End Sub

    ''' <summary>
    ''' 前向整段序列；backward=True 时额外做完整 BPTT 并保留梯度
    ''' </summary>
    Private Function RollOut(net As LiquidNeuralNetwork, inputs As Tensor, targets As Tensor,
                             dt As Double, backward As Boolean) As Double
        Dim T = inputs.Shape(0)
        Dim outputs(T - 1) As Tensor
        Dim loss As Double = 0

        net.ResetState()
        net.Training = backward

        For t = 0 To T - 1
            Dim u = New Tensor(net.InputSize)
            For i = 0 To net.InputSize - 1
                u(i) = inputs(t, i)
            Next

            outputs(t) = net.Forward(u, dt)
            loss += LNNTrainer.MSE(outputs(t), Row(targets, t, net.OutputSize))
        Next

        If Not backward Then
            net.Training = False
            Return loss / T
        End If

        Dim carry As Tensor = Nothing

        For t = T - 1 To 0 Step -1
            Dim dOut = LNNTrainer.MSEGradient(outputs(t), Row(targets, t, net.OutputSize))
            Dim adjH = net.BackwardOutput(dOut)

            If carry IsNot Nothing Then
                For i = 0 To adjH.Length - 1
                    adjH(i) += carry(i)
                Next
            End If

            carry = net.BackwardLiquid(adjH)
        Next

        net.Training = False

        Return loss / T
    End Function

    Private Function Row(m As Tensor, r As Integer, width As Integer) As Tensor
        Dim v = New Tensor(width)
        For i = 0 To width - 1
            v(i) = m(r, i)
        Next
        Return v
    End Function

End Module
