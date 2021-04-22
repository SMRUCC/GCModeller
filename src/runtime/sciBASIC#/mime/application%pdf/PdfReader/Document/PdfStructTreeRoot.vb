Imports System
Imports System.Collections.Generic

Namespace PdfReader
    Public Class PdfStructTreeRoot
        Inherits PdfDictionary

        Private _elements As List(Of PdfStructTreeElement)
        Private _IdTree As PdfNameTree
        Private _parentTree As PdfNumberTree

        Public Sub New(ByVal parent As PdfObject, ByVal dictionary As ParseDictionary)
            MyBase.New(parent, dictionary)
        End Sub

        Public ReadOnly Property K As List(Of PdfStructTreeElement)
            Get
                Dim dictionary As PdfDictionary = Nothing, array As PdfArray = Nothing, reference As PdfObjectReference = Nothing

                If _elements Is Nothing Then
                    _elements = New List(Of PdfStructTreeElement)()
                    Dim lK = OptionalValueRef(Of PdfObject)("K")

                    If CSharpImpl.__Assign(dictionary, TryCast(lK, PdfDictionary)) IsNot Nothing Then
                        _elements.Add(New PdfStructTreeElement(dictionary))
                    ElseIf CSharpImpl.__Assign(array, TryCast(lK, PdfArray)) IsNot Nothing Then

                        For Each item In array.Objects
                            dictionary = TryCast(item, PdfDictionary)

                            If dictionary Is Nothing Then
                                If CSharpImpl.__Assign(reference, TryCast(item, PdfObjectReference)) IsNot Nothing Then
                                    dictionary = Document.IndirectObjects.MandatoryValue(Of PdfDictionary)(reference)
                                Else
                                    Throw New ApplicationException($"PdfStructTreeRoot property K with array must contain dictionary or object reference and not '{item.GetType().Name}'.")
                                End If
                            End If

                            _elements.Add(New PdfStructTreeElement(dictionary))
                        Next
                    End If
                End If

                Return _elements
            End Get
        End Property

        Public ReadOnly Property IDTree As PdfNameTree
            Get
                If _IdTree Is Nothing Then _IdTree = New PdfNameTree(MandatoryValueRef(Of PdfDictionary)("IDTree"))
                Return _IdTree
            End Get
        End Property

        Public ReadOnly Property ParentTree As PdfNumberTree
            Get
                If _parentTree Is Nothing Then _parentTree = New PdfNumberTree(MandatoryValueRef(Of PdfDictionary)("ParentTree"))
                Return _parentTree
            End Get
        End Property

        Public ReadOnly Property ParentTreeNextKey As PdfInteger
            Get
                Return OptionalValue(Of PdfInteger)("ParentTreeNextKey")
            End Get
        End Property

        Public ReadOnly Property RoleMap As PdfDictionary
            Get
                Return OptionalValueRef(Of PdfDictionary)("RoleMap")
            End Get
        End Property

        Public ReadOnly Property ClassMap As PdfDictionary
            Get
                Return OptionalValueRef(Of PdfDictionary)("ClassMap")
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
