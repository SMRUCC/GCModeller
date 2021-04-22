Imports System
Imports System.Collections.Generic

Namespace PdfReader
    Public Class PdfContents
        Inherits PdfObject

        Private _Streams As System.Collections.Generic.List(Of PdfReader.PdfStream)

        Public Sub New(ByVal parent As PdfObject, ByVal obj As PdfObject)
            MyBase.New(parent)
            Streams = New List(Of PdfStream)()
            ResolveToStreams(obj)
        End Sub

        Public Property Streams As List(Of PdfStream)
            Get
                Return _Streams
            End Get
            Private Set(ByVal value As List(Of PdfStream))
                _Streams = value
            End Set
        End Property

        Public Overrides Sub Visit(ByVal visitor As IPdfObjectVisitor)
            visitor.Visit(Me)
        End Sub

        Public Function CreateParser() As PdfContentsParser
            Return New PdfContentsParser(Me)
        End Function

        Private Sub ResolveToStreams(ByVal obj As PdfObject)
            Dim stream As PdfStream = Nothing, reference As PdfObjectReference = Nothing, array As PdfArray = Nothing

            If CSharpImpl.__Assign(stream, TryCast(obj, PdfStream)) IsNot Nothing Then
                Streams.Add(stream)
            ElseIf CSharpImpl.__Assign(reference, TryCast(obj, PdfObjectReference)) IsNot Nothing Then
                ResolveToStreams(Document.ResolveReference(reference))
            ElseIf CSharpImpl.__Assign(array, TryCast(obj, PdfArray)) IsNot Nothing Then

                For Each entry In array.Objects
                    ResolveToStreams(entry)
                Next
            End If
        End Sub

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function
        End Class
    End Class
End Namespace
