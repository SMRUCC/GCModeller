Imports System


    ''' <summary>
    ''' Represents a column of certain data frames.
    ''' </summary>
    <AttributeUsage(AttributeTargets.Property, Inherited:=True, AllowMultiple:=False)>
    Public Class DataFrameColumnAttribute
        Inherits Attribute

        Private Shared ReadOnly Empty As String() = New String(-1) {}
        Private ReadOnly indexField As Integer

        ''' <summary>
        ''' Gets the index.
        ''' </summary>
        Public ReadOnly Property Index As Integer
            Get
                Return indexField
            End Get
        End Property

        Private nameField As String

        ''' <summary>
        ''' Gets or sets the name.
        ''' </summary>
        Public Property Name As String
            Get
                Return nameField
            End Get
            Set(ByVal value As String)

                If indexField < 0 AndAlso Equals(value, Nothing) Then
                    Throw New ArgumentNullException("value", "Name must not be null when Index is not defined.")
                End If

                nameField = value
            End Set
        End Property

        ''' <summary>
        ''' Initializes a new instance by name.
        ''' </summary>
        ''' <param name="name">The name.</param>
        Public Sub New(ByVal name As String)
            If Equals(name, Nothing) Then
                Throw New ArgumentNullException("name")
            End If

            nameField = name
            indexField = -1
        End Sub

        ''' <summary>
        ''' Initializes a new instance by index.
        ''' </summary>
        ''' <param name="index">The index.</param>
        Public Sub New(ByVal index As Integer)
            If index < 0 Then
                Throw New ArgumentOutOfRangeException("index")
            End If

            nameField = Nothing
            indexField = index
        End Sub

        Friend Function GetIndex(ByVal names As String()) As Integer
            Return If(Index >= 0, Index, Array.IndexOf(If(names, Empty), Name))
        End Function
    End Class

