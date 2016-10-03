#Region "Microsoft.VisualBasic::00d3f15106d816953fd14994a5da7593, ..\R.Bioconductor\RDotNET\R.NET\DataFrameColumnAttribute.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

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
