' ============================================================================
' TrainingLog.vb - 训练过程观察者
'
' 目标：让训练可观测且可替换输出目标，但**保持既有的控制台文案不变**，
' 从而让 verbose:=True 的用户可见行为与重构前一致。
'
' 既有文案（重构前）：
'   GMVAE.vb L344
'     "  Epoch {epoch,4}/{epochs} | Loss=.. | Recon=.. | KL(y)=.. | KL(z)=.."
'   GraphVAE.vb L903
'     "  Epoch {epoch,4:D}/{epochs}: loss = ......"
' ============================================================================

Imports System.Globalization

Namespace Core

    ''' <summary>
    ''' 训练过程观察者。
    ''' </summary>
    Public Interface ITrainingObserver

        Sub OnEpochBegin(epoch As Integer, totalEpochs As Integer)

        Sub OnBatch(epoch As Integer, batchIndex As Integer, losses As IReadOnlyDictionary(Of String, Double))

        Sub OnEpochEnd(epoch As Integer, losses As IReadOnlyDictionary(Of String, Double), elapsedMilliseconds As Double)

        Sub OnConverged(epoch As Integer, reason As String)

    End Interface

    ''' <summary>不做任何输出的观察者。</summary>
    Public Class NullTrainingObserver
        Implements ITrainingObserver

        Public Sub OnEpochBegin(epoch As Integer, totalEpochs As Integer) Implements ITrainingObserver.OnEpochBegin
        End Sub

        Public Sub OnBatch(epoch As Integer, batchIndex As Integer, losses As IReadOnlyDictionary(Of String, Double)) Implements ITrainingObserver.OnBatch
        End Sub

        Public Sub OnEpochEnd(epoch As Integer, losses As IReadOnlyDictionary(Of String, Double), elapsedMilliseconds As Double) Implements ITrainingObserver.OnEpochEnd
        End Sub

        Public Sub OnConverged(epoch As Integer, reason As String) Implements ITrainingObserver.OnConverged
            ' 空观察者保持静默
        End Sub

    End Class

    ''' <summary>
    ''' 控制台观察者。输出格式由 <see cref="Formatter"/> 决定，
    ''' 通过 <see cref="ForGmvae"/> / <see cref="ForGraphVae"/> 复用重构前既有文案。
    ''' </summary>
    Public Class ConsoleTrainingObserver
        Implements ITrainingObserver

        Private ReadOnly _formatter As Func(Of Integer, Integer, IReadOnlyDictionary(Of String, Double), String)
        Private _totalEpochs As Integer = 0

        ''' <summary>每隔多少轮输出一次；第 1 轮与最后一轮总是输出。</summary>
        Public Property PrintInterval As Integer = 10

        Public Sub New(formatter As Func(Of Integer, Integer, IReadOnlyDictionary(Of String, Double), String))
            _formatter = formatter
        End Sub

        ''' <summary>复现 GMVAE 既有文案。</summary>
        Public Shared Function ForGmvae() As ConsoleTrainingObserver
            Dim formatter As Func(Of Integer, Integer, IReadOnlyDictionary(Of String, Double), String) =
                Function(epoch, total, losses)
                    Return String.Format(CultureInfo.InvariantCulture,
                                         "  Epoch {0,4}/{1} | Loss={2:F4} | Recon={3:F4} | KL(y)={4:F4} | KL(z)={5:F4}",
                                         epoch, total,
                                         Lookup(losses, "loss"), Lookup(losses, "recon"),
                                         Lookup(losses, "kl-y"), Lookup(losses, "kl-z"))
                End Function

            Return New ConsoleTrainingObserver(formatter) With {.PrintInterval = 10}
        End Function

        ''' <summary>复现 GraphVAE 既有文案。</summary>
        Public Shared Function ForGraphVae() As ConsoleTrainingObserver
            Dim formatter As Func(Of Integer, Integer, IReadOnlyDictionary(Of String, Double), String) =
                Function(epoch, total, losses)
                    Return String.Format(CultureInfo.InvariantCulture,
                                         "  Epoch {0,4:D}/{1}: loss = {2:F6}",
                                         epoch, total, Lookup(losses, "loss"))
                End Function

            Return New ConsoleTrainingObserver(formatter) With {.PrintInterval = 50}
        End Function

        Private Shared Function Lookup(losses As IReadOnlyDictionary(Of String, Double), key As String) As Double
            Dim value As Double

            If losses IsNot Nothing AndAlso losses.TryGetValue(key, value) Then
                Return value
            End If

            Return Double.NaN
        End Function

        Public Sub OnEpochBegin(epoch As Integer, totalEpochs As Integer) Implements ITrainingObserver.OnEpochBegin
            _totalEpochs = totalEpochs
        End Sub

        Public Sub OnBatch(epoch As Integer, batchIndex As Integer, losses As IReadOnlyDictionary(Of String, Double)) Implements ITrainingObserver.OnBatch
            ' 控制台观察者不输出批次级信息
        End Sub

        Public Sub OnEpochEnd(epoch As Integer, losses As IReadOnlyDictionary(Of String, Double), elapsedMilliseconds As Double) Implements ITrainingObserver.OnEpochEnd
            If _formatter Is Nothing Then
                Return
            End If

            Dim shouldPrint As Boolean =
                epoch = 1 OrElse
                (_totalEpochs > 0 AndAlso epoch = _totalEpochs) OrElse
                (PrintInterval > 0 AndAlso epoch Mod PrintInterval = 0)

            If shouldPrint Then
                Console.WriteLine(_formatter(epoch, _totalEpochs, losses))
            End If
        End Sub

        Public Sub OnConverged(epoch As Integer, reason As String) Implements ITrainingObserver.OnConverged
            Console.WriteLine($"  已收敛于第 {epoch} 轮: {reason}")
        End Sub

    End Class

End Namespace
