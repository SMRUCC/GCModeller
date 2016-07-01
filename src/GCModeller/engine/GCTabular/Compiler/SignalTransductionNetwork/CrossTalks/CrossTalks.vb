Imports System.Text.RegularExpressions
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat
Imports LANS.SystemsBiology.Assembly
Imports LANS.SystemsBiology
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DataVisualization.Network.FileStream
Imports Microsoft.VisualBasic.DataVisualization.Network.Abstract

Namespace Compiler.Components

    ''' <summary>
    ''' 双组分系统的蛋白质之间的互作的可能性的高低
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CrossTalks : Implements IInteraction

        <Column("kinase")> Public Property Kinase As String Implements IInteraction.source
        <Column("regulator")> Public Property Regulator As String Implements IInteraction.target
        <Column("probability")> Public Property Probability As Double

        Public Shared Function Trimed(strId As String) As String
            Dim p As Integer = InStr(strId, "(")

            If p > 0 Then
                Return Mid(strId, 1, p - 1)
            Else
                Return strId
            End If
        End Function

        Public Function TrimedKinaseId() As String
            Return Trimed(Kinase)
        End Function

        Public Function TrimedRegulatorId() As String
            Return Trimed(Regulator)
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0} -> {1};  {2}", Kinase, Regulator, Probability)
        End Function

        Public Function get_InternalGUID() As String
            Return String.Format("{0} ==> {1}", Kinase, Regulator)
        End Function
    End Class
End Namespace
