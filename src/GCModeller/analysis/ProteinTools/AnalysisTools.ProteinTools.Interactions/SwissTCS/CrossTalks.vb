Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataVisualization.Network.Abstract
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace SwissTCS

    ''' <summary>
    ''' 双组分系统的蛋白质之间的互作的可能性的高低
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CrossTalks : Implements IInteraction, INetworkEdge
        <Column("kinase")> Public Property Kinase As String Implements IInteraction.target
        <Column("regulator")> Public Property Regulator As String Implements IInteraction.source
        <Column("probability")> Public Property Probability As Double Implements INetworkEdge.Confidence
        Public Property InteractionType As String Implements INetworkEdge.InteractionType

        Public Shared Function Trim(sId As String) As String
            Dim p As Integer = InStr(sId, "(")

            If p > 0 Then
                Return Mid(sId, 1, p - 1)
            Else
                Return sId
            End If
        End Function

        Public Function TrimedKinaseId() As String
            Return Trim(Kinase)
        End Function

        Public Function TrimedRegulatorId() As String
            Return Trim(Regulator)
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0} -> {1};  {2}", Kinase, Regulator, Probability)
        End Function
    End Class

    Public Enum TCSComponentTypes
        kinase
        receiver
    End Enum
End Namespace