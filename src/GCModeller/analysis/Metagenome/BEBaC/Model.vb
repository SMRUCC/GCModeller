Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic

Namespace BEBaC

    Public Class I3merVector : Inherits ClassObject
        Public Property Name As String
        Public Property Vector As Dictionary(Of I3Mers, Integer)
        Public Property Frequency As Dictionary(Of I3Mers, Double)
    End Class

    Public Class Cluster

        Public Property members As List(Of I3merVector)

        Public Function PartitionProbability() As Double
            Return members.PartitionProbability
        End Function
    End Class
End Namespace