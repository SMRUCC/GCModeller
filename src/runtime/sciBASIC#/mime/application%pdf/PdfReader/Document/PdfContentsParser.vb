Imports System
Imports System.IO

Namespace PdfReader
    Public Class PdfContentsParser
        Inherits PdfObject

        Private _index As Integer = 0
        Private _parser As Parser

        Public Sub New(ByVal parent As PdfContents)
            MyBase.New(parent)
        End Sub

        Public ReadOnly Property Contents As PdfContents
            Get
                Return TypedParent(Of PdfContents)()
            End Get
        End Property

        Public Function GetObject() As PdfObject
            ' First time around we setup the parser to the first stream
            If _parser Is Nothing AndAlso _index < Contents.Streams.Count Then _parser = New Parser(New MemoryStream(Contents.Streams(Math.Min(Threading.Interlocked.Increment(_index), _index - 1)).ValueAsBytes), True)

            ' Keep trying to get a parsed object as long as there is a parser for a stream
            While _parser IsNot Nothing
                Dim obj = _parser.ParseObject(True)
                If obj IsNot Nothing Then Return WrapObject(obj)
                _parser.Dispose()
                _parser = Nothing

                ' Is there another stream we can continue parsing with
                If _index < Contents.Streams.Count Then _parser = New Parser(New MemoryStream(Contents.Streams(Math.Min(Threading.Interlocked.Increment(_index), _index - 1)).ValueAsBytes), True)
            End While

            Return Nothing
        End Function
    End Class
End Namespace
