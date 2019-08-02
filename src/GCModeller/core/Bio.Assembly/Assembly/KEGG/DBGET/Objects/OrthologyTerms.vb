Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports XmlProperty = Microsoft.VisualBasic.Text.Xml.Models.Property

Namespace Assembly.KEGG.DBGET.bGetObject

    <XmlType("Orthology-terms", [Namespace]:=OrthologyTerms.Xmlns)>
    Public Class OrthologyTerms : Inherits ListOf(Of XmlProperty)

        Public Const Xmlns$ = "http://GCModeller.org/core/KEGG/Model/OrthologyTerm.xsd"

        <XmlIgnore>
        Public ReadOnly Property EntityList As String()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Terms.Keys
            End Get
        End Property

        ''' <summary>
        ''' The KO terms?
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("terms")>
        Public Property Terms As XmlProperty()

        Public Overrides Function ToString() As String
            Return EntityList.GetJson
        End Function

        Protected Overrides Function getSize() As Integer
            If Terms.IsNullOrEmpty Then
                Return 0
            Else
                Return Terms.Length
            End If
        End Function

        Protected Overrides Function getCollection() As IEnumerable(Of XmlProperty)
            Return Terms.SafeQuery
        End Function
    End Class
End Namespace