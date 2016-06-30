Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.Linq
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace FBA_OUTPUT

    ''' <summary>
    ''' RXN  --> flux result.
    ''' </summary>
    Public Class TabularOUT : Implements sIdEnumerable

        Public Property Rxn As String Implements sIdEnumerable.Identifier
        Public Property Flux As Double

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    ''' <summary>
    ''' 记录不同的样品之间的FBA目标方程的计算结果的对象
    ''' </summary>
    Public Class ObjectiveFunction

        Public Property Result As KeyValuePairObject(Of String, Double)()
            Get
                If __innerResult Is Nothing Then
                    __innerResult = New List(Of KeyValuePairObject(Of String, Double))
                End If
                Return __innerResult.ToArray
            End Get
            Set(value As KeyValuePairObject(Of String, Double)())
                If value Is Nothing Then
                    __innerResult = New List(Of KeyValuePairObject(Of String, Double))
                Else
                    __innerResult = value.ToList
                End If
            End Set
        End Property

        Public Property Factors As String()
        <XmlAttribute> Public Property Coefficient As Double()
        Public Property Associates As String()
        Public Property Comments As String
        Public Property Name As String
        Public Property Info As String

        Dim __innerResult As List(Of KeyValuePairObject(Of String, Double))

        Public Sub Add(sample As String, result As Double)
            Call __innerResult.Add(sample, result)
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace