Imports System.Text
Imports System.Xml.Serialization
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace SequenceModel.Patterns.Clustal

    Public Class SRChain

        <XmlElement> Public Property lstSR As SR()
        <XmlAttribute>
        Public Property Name As String

        Public ReadOnly Property IsEmpty As Boolean
            Get
                Dim l As Integer = (From x As SR In lstSR
                                    Where x.Residue = "-"c
                                    Select 1).Sum
                Return l = lstSR.Length
            End Get
        End Property

        Public ReadOnly Property Hits As Integer
            Get
                Dim l As Integer = (From x As SR In lstSR
                                    Where x.Residue <> "-"c
                                    Select 1).Sum
                Return l
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return New String(lstSR.ToArray(Function(x) x.Residue))
        End Function
    End Class
End Namespace