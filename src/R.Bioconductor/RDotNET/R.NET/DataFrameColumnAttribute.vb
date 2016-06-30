
''' <summary>
''' Represents a column of certain data frames.
''' </summary>
<AttributeUsage(AttributeTargets.[Property], Inherited := True, AllowMultiple := False)> _
Public Class DataFrameColumnAttribute
	Inherits Attribute
	Private Shared ReadOnly Empty As String() = New String(-1) {}

	Private ReadOnly m_index As Integer

	''' <summary>
	''' Gets the index.
	''' </summary>
	Public ReadOnly Property Index() As Integer
		Get
			Return Me.m_index
		End Get
	End Property

	Private m_name As String

	''' <summary>
	''' Gets or sets the name.
	''' </summary>
	Public Property Name() As String
		Get
			Return Me.m_name
		End Get
		Set
			If Me.m_index < 0 AndAlso value Is Nothing Then
				Throw New ArgumentNullException("value", "Name must not be null when Index is not defined.")
			End If
			Me.m_name = value
		End Set
	End Property

	''' <summary>
	''' Initializes a new instance by name.
	''' </summary>
	''' <param name="name">The name.</param>
	Public Sub New(name As String)
		If name Is Nothing Then
			Throw New ArgumentNullException("name")
		End If
		Me.m_name = name
		Me.m_index = -1
	End Sub

	''' <summary>
	''' Initializes a new instance by index.
	''' </summary>
	''' <param name="index">The index.</param>
	Public Sub New(index As Integer)
		If index < 0 Then
			Throw New ArgumentOutOfRangeException("index")
		End If
		Me.m_name = Nothing
		Me.m_index = index
	End Sub

	Friend Function GetIndex(names As String()) As Integer
		Return If(Index >= 0, Index, Array.IndexOf(If(names, Empty), Name))
	End Function
End Class
