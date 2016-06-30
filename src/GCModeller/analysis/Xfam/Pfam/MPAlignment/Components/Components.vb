Imports System.Text
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.ComponentModel
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace ProteinDomainArchitecture.MPAlignment

    ''' <summary>
    ''' Domain position specifc distributions
    ''' </summary>
    Public Class DomainDistribution

        <XmlAttribute> Public Property DomainId As String
        ''' <summary>
        ''' Position collection for this <see cref="DomainId">domain object item</see>
        ''' </summary>
        ''' <returns></returns>
        <XmlElement>
        Public Property Distribution As Position()

        Public Shared ReadOnly Property EmptyDomain As DomainDistribution
            Get
                Dim nullDistr As Position() = {
                    New Position With {
                        .Left = 1,
                        .Right = 1
                    }
                }
                Dim empty As New DomainDistribution With {
                    .DomainId = EmptyId,
                    .Distribution = nullDistr
                }
                Return empty
            End Get
        End Property

        Public Const EmptyId As String = "*****"

        Public Overrides Function ToString() As String
            Return DomainId
        End Function
    End Class
End Namespace