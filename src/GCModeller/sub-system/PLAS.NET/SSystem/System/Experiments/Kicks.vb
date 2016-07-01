Imports System.Xml.Serialization
Imports LANS.SystemsBiology.AnalysisTools.CellPhenotype.SSystem.Kernel.ObjectModels
Imports Microsoft.VisualBasic

Namespace Kernel

    Public Class Kicks

        ''' <summary>
        ''' Standing by
        ''' </summary>
        ''' <remarks></remarks>
        ReadOnly __pendingKicks As List(Of Disturb)
        ''' <summary>
        ''' Active object.
        ''' </summary>
        ''' <remarks></remarks>
        ReadOnly __runningKicks As List(Of Disturb)
        ReadOnly __kernel As Kernel

        Sub New(kernel As Kernel)
            __kernel = kernel
            __pendingKicks = kernel.get_Model.Experiments.ToList(Function(x) New Disturb(x))
            __runningKicks = New List(Of Disturb)

            For i As Integer = 0 To __pendingKicks.Count - 1
                __pendingKicks(i).Set(kernel)
            Next
        End Sub

        Public Sub Tick()
            For Each x As Disturb In __getPendings()
                Call __pendingKicks.Remove(x)
                Call __runningKicks.Add(x)
            Next

            For i As Integer = 0 To __runningKicks.Count - 1
                __runningKicks(i).Tick()
            Next

            For Each x In __getExpireds()
                Call __runningKicks.Remove(x)
            Next
        End Sub

        Private Function __getExpireds() As Disturb()
            Return (From x As Disturb
                    In __runningKicks
                    Where x.LeftKicks = 0
                    Select x).ToArray ' Removed pending
        End Function

        Private Function __getPendings() As Disturb()
            Return (From x As Disturb In __pendingKicks
                    Where x.IsReady  ' Quene pending
                    Select x).ToArray
        End Function
    End Class
End Namespace
