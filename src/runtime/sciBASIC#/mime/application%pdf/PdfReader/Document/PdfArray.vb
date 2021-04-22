Imports System.Collections.Generic
Imports System.Text

Namespace PdfReader
    Public Class PdfArray
        Inherits PdfObject

        Private _wrapped As List(Of PdfObject)

        Public Sub New(ByVal parent As PdfObject, ByVal array As ParseArray)
            MyBase.New(parent, array)
        End Sub

        Public Overrides Sub Visit(ByVal visitor As IPdfObjectVisitor)
            visitor.Visit(Me)
        End Sub

        Public ReadOnly Property ParseArray As ParseArray
            Get
                Return TryCast(ParseObject, ParseArray)
            End Get
        End Property

        Public ReadOnly Property Objects As List(Of PdfObject)
            Get

                If _wrapped Is Nothing Then
                    _wrapped = New List(Of PdfObject)()

                    For Each obj In ParseArray.Objects
                        _wrapped.Add(WrapObject(obj))
                    Next
                End If

                Return _wrapped
            End Get
        End Property

        Public Overrides Function ToString() As String
            Dim sb As StringBuilder = New StringBuilder()

            For Each obj In Objects

                If obj.ParseObject.GetType() Is GetType(ParseString) Then
                    sb.Append(obj.ToString())
                End If
            Next

            Return sb.ToString()
        End Function
    End Class
End Namespace
