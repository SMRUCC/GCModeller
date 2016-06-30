Imports System.Text
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.ComponentModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace ProteinDomainArchitecture.MPAlignment

    Public Class DomainAlignment
        Public Property ProteinQueryDomainDs As DomainDistribution
        Public Property ProteinSbjctDomainDs As DomainDistribution
        <XmlAttribute> Public Property Score As Double

        Public ReadOnly Property IsMatch As Boolean
            Get
                Return Score > 0
            End Get
        End Property

        Public Function FormatPlantTextOutput(QueryMaxLength As Integer, SbjctMaxLength As Integer) As String
            Dim array As String() = {
                String.Format("{0}{1}", ProteinQueryDomainDs.DomainId, New String(" ", QueryMaxLength - Len(ProteinQueryDomainDs.DomainId))),
                String.Format("{0}{1}", ProteinSbjctDomainDs.DomainId, New String(" ", SbjctMaxLength - Len(ProteinSbjctDomainDs.DomainId))),
                Score
            }
            Return String.Join(" ", array)
        End Function

        Public Overrides Function ToString() As String
            Return String.Join(vbTab, {ProteinQueryDomainDs.DomainId, ProteinSbjctDomainDs.DomainId, Score})
        End Function
    End Class
End Namespace