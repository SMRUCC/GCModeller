Imports LANS.SystemsBiology.AnalysisTools.CellPhenotype.SSystem.Kernel.ObjectModels
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream

Namespace Kernel

    Public Class DataAcquisition
        Dim _dataPackage As New File
        Dim Kernel As Kernel

        Public Sub Tick()
            Dim Row As New RowObject

            Call Row.Add(Kernel.RuntimeTicks)
            For Each Var As Var In Kernel.Vars
                Row.Add(Var.Value)
            Next

            _dataPackage.AppendLine(Row)
        End Sub

        Public Sub Save(Path As String)
            Call _dataPackage.Save(Path, False)
        End Sub

        Public Sub [Set](Kernel As Kernel)
            Dim Row As New RowObject

            Me.Kernel = Kernel
            Call Row.Add("Elapsed Time")
            For Each Var As Var In Kernel.Vars
                If Not String.IsNullOrEmpty(Var.Title) Then
                    Call Row.Add(String.Format("""{0}""", Var.Title))
                Else
                    Call Row.Add(Var.UniqueId)
                End If
            Next

            Call _dataPackage.AppendLine(Row)
        End Sub

        Public Shared Function [Get](e As DataAcquisition) As File
            Return e._dataPackage
        End Function
    End Class
End Namespace
