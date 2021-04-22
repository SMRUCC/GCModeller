Imports System
Imports System.Collections.Generic

Namespace PdfReader
    Public MustInherit Class PdfObject
        Private _Parent As PdfReader.PdfObject, _ParseObject As PdfReader.ParseObjectBase

        Public Sub New(ByVal parent As PdfObject)
            Me.New(parent, Nothing)
        End Sub

        Public Sub New(ByVal parent As PdfObject, ByVal parse As ParseObjectBase)
            Me.Parent = parent
            ParseObject = parse
        End Sub

        Public Overrides Function ToString() As String
            Return $"({[GetType]().Name})"
        End Function

        Public Overridable Sub Visit(ByVal visitor As IPdfObjectVisitor)
            visitor.Visit(Me)
        End Sub

        Public Property Parent As PdfObject
            Get
                Return _Parent
            End Get
            Private Set(ByVal value As PdfObject)
                _Parent = value
            End Set
        End Property

        Public Property ParseObject As ParseObjectBase
            Get
                Return _ParseObject
            End Get
            Private Set(ByVal value As ParseObjectBase)
                _ParseObject = value
            End Set
        End Property

        Public ReadOnly Property Document As PdfDocument
            Get
                Return TypedParent(Of PdfDocument)()
            End Get
        End Property

        Public ReadOnly Property Decrypt As PdfDecrypt
            Get
                Return TypedParent(Of PdfDocument)().DecryptHandler
            End Get
        End Property

        Public Function TypedParent(Of T As PdfObject)() As T
            Dim parent = Me.Parent

            While parent IsNot Nothing

                If TypeOf parent Is T Then
                    Return TryCast(parent, T)
                Else
                    parent = parent.Parent
                End If
            End While

            Return Nothing
        End Function

        Public Function AsBoolean() As Boolean
            Dim [boolean] As PdfBoolean = Nothing
            If CSharpImpl.__Assign([boolean], TryCast(Me, PdfBoolean)) IsNot Nothing Then Return [boolean].Value
            Throw New ApplicationException($"Unexpected object in content '{[GetType]().Name}', expected a boolean.")
        End Function

        Public Function AsString() As String
            Dim name As PdfName = Nothing, str As PdfString = Nothing

            If CSharpImpl.__Assign(name, TryCast(Me, PdfName)) IsNot Nothing Then
                Return name.Value
            ElseIf CSharpImpl.__Assign(str, TryCast(Me, PdfString)) IsNot Nothing Then
                Return str.Value
            End If

            Throw New ApplicationException($"Unexpected object in content '{[GetType]().Name}', expected a string.")
        End Function

        Public Function AsInteger() As Integer
            Dim [integer] As PdfInteger = Nothing
            If CSharpImpl.__Assign([integer], TryCast(Me, PdfInteger)) IsNot Nothing Then Return [integer].Value
            Throw New ApplicationException($"Unexpected object in content '{[GetType]().Name}', expected an integer.")
        End Function

        Public Function AsNumber() As Single
            Dim [integer] As PdfInteger = Nothing, real As PdfReal = Nothing

            If CSharpImpl.__Assign([integer], TryCast(Me, PdfInteger)) IsNot Nothing Then
                Return [integer].Value
            ElseIf CSharpImpl.__Assign(real, TryCast(Me, PdfReal)) IsNot Nothing Then
                Return real.Value
            End If

            Throw New ApplicationException($"Unexpected object in content '{[GetType]().Name}', expected a number.")
        End Function

        Public Function AsNumberArray() As Single()
            Dim array As PdfArray = Nothing, [integer] As PdfInteger = Nothing, real As PdfReal = Nothing

            If CSharpImpl.__Assign(array, TryCast(Me, PdfArray)) IsNot Nothing Then
                Dim numbers As List(Of Single) = New List(Of Single)()

                For Each item In array.Objects

                    If CSharpImpl.__Assign([integer], TryCast(item, PdfInteger)) IsNot Nothing Then
                        numbers.Add([integer].Value)
                    ElseIf CSharpImpl.__Assign(real, TryCast(item, PdfReal)) IsNot Nothing Then
                        numbers.Add(real.Value)
                    Else
                        Throw New ApplicationException($"Array contains object of type '{[GetType]().Name}', expected only numbers.")
                    End If
                Next

                Return numbers.ToArray()
            End If

            Throw New ApplicationException($"Unexpected object in content '{[GetType]().Name}', expected an integer array.")
        End Function

        Public Function AsArray() As List(Of PdfObject)
            Dim array As PdfArray = Nothing
            If CSharpImpl.__Assign(array, TryCast(Me, PdfArray)) IsNot Nothing Then Return array.Objects
            Throw New ApplicationException($"Unexpected object in content '{[GetType]().Name}', expected an integer array.")
        End Function

        Public Function WrapObject(ByVal obj As ParseObjectBase) As PdfObject
            Dim str As ParseString = Nothing
            If CSharpImpl.__Assign(str, TryCast(obj, ParseString)) IsNot Nothing Then Return New PdfString(Me, str)
            Dim name As ParseName = Nothing, [integer] As ParseInteger = Nothing, real As ParseReal = Nothing, dictionary As ParseDictionary = Nothing, reference As ParseObjectReference = Nothing, stream As ParseStream = Nothing, array As ParseArray = Nothing, identifier As ParseIdentifier = Nothing, [boolean] As ParseBoolean = Nothing

            If CSharpImpl.__Assign(name, TryCast(obj, ParseName)) IsNot Nothing Then
                Return New PdfName(Me, name)
            ElseIf CSharpImpl.__Assign([integer], TryCast(obj, ParseInteger)) IsNot Nothing Then
                Return New PdfInteger(Me, [integer])
            ElseIf CSharpImpl.__Assign(real, TryCast(obj, ParseReal)) IsNot Nothing Then
                Return New PdfReal(Me, real)
            ElseIf CSharpImpl.__Assign(dictionary, TryCast(obj, ParseDictionary)) IsNot Nothing Then
                Return New PdfDictionary(Me, dictionary)
            ElseIf CSharpImpl.__Assign(reference, TryCast(obj, ParseObjectReference)) IsNot Nothing Then
                Return New PdfObjectReference(Me, reference)
            ElseIf CSharpImpl.__Assign(stream, TryCast(obj, ParseStream)) IsNot Nothing Then
                Return New PdfStream(Me, stream)
            ElseIf CSharpImpl.__Assign(array, TryCast(obj, ParseArray)) IsNot Nothing Then
                Return New PdfArray(Me, array)
            ElseIf CSharpImpl.__Assign(identifier, TryCast(obj, ParseIdentifier)) IsNot Nothing Then
                Return New PdfIdentifier(Me, identifier)
            ElseIf CSharpImpl.__Assign([boolean], TryCast(obj, ParseBoolean)) IsNot Nothing Then
                Return New PdfBoolean(Me, [boolean])
            End If

            Dim nul As ParseNull = Nothing
            If CSharpImpl.__Assign(nul, TryCast(obj, ParseNull)) IsNot Nothing Then Return New PdfNull(Me)
            Throw New ApplicationException($"Cannot wrap object '{obj.GetType().Name}' as a pdf object .")
        End Function

        Public Shared Function ArrayToRectangle(ByVal array As PdfArray) As PdfRectangle
            If array IsNot Nothing Then
                Return New PdfRectangle(array.Parent, array.ParseArray)
            Else
                Return Nothing
            End If
        End Function

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function
        End Class
    End Class
End Namespace
