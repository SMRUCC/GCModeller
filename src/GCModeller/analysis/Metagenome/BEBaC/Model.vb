Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.DataMining.Framework.ComponentModel
Imports Microsoft.VisualBasic.DataMining.Framework.KMeans.CompleteLinkage
Imports Microsoft.VisualBasic.Language

Namespace BEBaC

    Public Class I3merVector : Inherits Point
        Public Property Name As String
        Public Property Vector As Dictionary(Of I3Mers, Integer)
        Public Property Frequency As Dictionary(Of I3Mers, Double)
            Get
                Return f
            End Get
            Set(value As Dictionary(Of I3Mers, Double))
                f = value
                Properties = f.Values.ToArray
            End Set
        End Property

        Dim f As Dictionary(Of I3Mers, Double)
    End Class

    Public Class Cluster

        Public Property members As List(Of I3merVector)

        Public Function PartitionProbability() As Double
            Return members.Probability
        End Function
    End Class
End Namespace