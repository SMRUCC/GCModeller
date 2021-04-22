Imports System
Imports System.Collections.Generic

Namespace PdfReader
    Public Class PdfStructTreeElement
        Inherits PdfDictionary

        Private _elements As List(Of PdfStructTreeElement)

        Public Sub New(ByVal dictionary As PdfDictionary)
            MyBase.New(dictionary.Parent, dictionary.ParseDictionary)
        End Sub

        Public ReadOnly Property S As PdfName
            Get
                Return MandatoryValue(Of PdfName)("S")
            End Get
        End Property

        Public ReadOnly Property ID As PdfString
            Get
                Return OptionalValue(Of PdfString)("ID")
            End Get
        End Property

        Public ReadOnly Property Pg As PdfObjectReference
            Get
                Return OptionalValue(Of PdfObjectReference)("Pg")
            End Get
        End Property

        Public ReadOnly Property K As List(Of PdfStructTreeElement)
            Get
                Dim dictionary As PdfDictionary = Nothing, array As PdfArray = Nothing, reference As PdfObjectReference = Nothing

                If _elements Is Nothing Then
                    _elements = New List(Of PdfStructTreeElement)()
                    Dim lK = OptionalValueRef(Of PdfObject)("K")

                    If CSharpImpl.__Assign(dictionary, TryCast(lK, PdfDictionary)) IsNot Nothing Then
                        _elements.Add(New PdfStructTreeElement(dictionary))
                    ElseIf CSharpImpl.__Assign(array, TryCast(lK, PdfArray)) IsNot Nothing Then

                        For Each item As PdfObject In array.Objects
                            dictionary = TryCast(item, PdfDictionary)

                            If dictionary Is Nothing Then
                                If CSharpImpl.__Assign(reference, TryCast(item, PdfObjectReference)) IsNot Nothing Then
                                    dictionary = Document.IndirectObjects.MandatoryValue(Of PdfDictionary)(reference)
                                Else
                                    Throw New ApplicationException($"PdfStructTreeElement property K has unrecognized content of type '{item.GetType().Name}'.")
                                End If
                            End If

                            _elements.Add(New PdfStructTreeElement(dictionary))
                        Next
                    End If
                End If

                Return _elements
            End Get
        End Property

        Public ReadOnly Property A As PdfObject
            Get
                Return OptionalValue(Of PdfObject)("A")
            End Get
        End Property

        Public ReadOnly Property C As PdfObject
            Get
                Return OptionalValue(Of PdfObject)("C")
            End Get
        End Property

        Public ReadOnly Property R As PdfInteger
            Get
                Return OptionalValue(Of PdfInteger)("R")
            End Get
        End Property

        Public ReadOnly Property T As PdfString
            Get
                Return OptionalValue(Of PdfString)("T")
            End Get
        End Property

        Public ReadOnly Property Lang As PdfString
            Get
                Return OptionalValue(Of PdfString)("Lang")
            End Get
        End Property

        Public ReadOnly Property Alt As PdfString
            Get
                Return OptionalValue(Of PdfString)("Alt")
            End Get
        End Property

        Public ReadOnly Property E As PdfString
            Get
                Return OptionalValue(Of PdfString)("E")
            End Get
        End Property

        Public ReadOnly Property ActualText As PdfString
            Get
                Return OptionalValue(Of PdfString)("ActualText")
            End Get
        End Property

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function
        End Class
    End Class
End Namespace
