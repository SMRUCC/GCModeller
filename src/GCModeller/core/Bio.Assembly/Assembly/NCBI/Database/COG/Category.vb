Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace Assembly.NCBI.COG

    Public Class Category

        <XmlAttribute> Public Property [Class] As COGCategories
        <XmlAttribute> Public Property Description As String
        <XmlElement> Public Property SubClasses As KeyValuePair()
            Get
                Return __innerArray
            End Get
            Set(value As KeyValuePair())
                __innerArray = value
                If __innerArray Is Nothing Then
                    __innerArray = New KeyValuePair() {}
                End If

                Dim Full = __innerArray.Join(__innerArray.ToArray(
                                             Function(obj) New KeyValuePair With {
                                                 .Key = obj.Key.ToLower,
                                                 .Value = obj.Value}))
                __innerHash = New SortedDictionary(Of Char, String)(
                    Full.ToDictionary(Function(obj) obj.Key(Scan0),
                                      Function(obj) obj.Value))
            End Set
        End Property

        Protected Friend __innerHash As SortedDictionary(Of Char, String)
        Protected Friend __innerArray As KeyValuePair()

        Public Overrides Function ToString() As String
            Return Description
        End Function

        Public Function ToArray() As COGFunc()
            Return SubClasses.ToArray(
                Function(x) New COGFunc With {
                    .Category = [Class],
                    .COG = x.Key,
                    .Func = x.Value
            })
        End Function

        Public Function GetDescription(COG As Char, ByRef description As String) As Boolean
            If __innerHash.ContainsKey(COG) Then
                description = __innerHash(COG)
                Return True
            Else
                description = ""
                Return False
            End If
        End Function
    End Class
End Namespace