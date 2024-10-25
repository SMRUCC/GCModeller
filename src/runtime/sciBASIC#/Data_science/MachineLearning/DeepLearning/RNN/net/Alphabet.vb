Namespace RNN

	' Immutable set of symbols mapped indices.
	<Serializable>
	Public Class Alphabet
		Private m_indexToChar As Char()
		Private m_charToIndex As Dictionary(Of Char, Integer)

		' Constructs an alphabet containing symbols extracted from the string.
		' Treats null as an empty string.
		Private Sub New(data As String)
			If ReferenceEquals(data, Nothing) Then
				m_indexToChar = New Char(-1) {}
				m_charToIndex = New Dictionary(Of Char, Integer)()
				Return
			End If

			' find the alphabet
			Dim chars As SortedSet(Of Char) = New SortedSet(Of Char)()
			m_charToIndex = New Dictionary(Of Char, Integer)()
			Dim i = 0

			For i = 0 To data.Length - 1
				chars.Add(data(i))
			Next

			m_indexToChar = New Char(chars.Count - 1) {}

			i = 0
			For Each c In chars
				m_indexToChar(i) = c
				m_charToIndex(c) = System.Math.Min(Threading.Interlocked.Increment(i), i - 1)
			Next
		End Sub

		' Returns alphabet containing symbols extracted from the string.
		' Treats null as an empty string.
		Public Shared Function fromString(data As String) As Alphabet
			Return New Alphabet(data)
		End Function

		' Returns the alphabet size.
		Public Overridable Function size() As Integer
			Return m_indexToChar.Length
		End Function

		' Converts a character to the corresponding index.
		Public Overridable Function charToIndex(c As Char) As Integer
			Dim index As Integer? = m_charToIndex(c)
			If index Is Nothing Then
				Throw New Exception("Character is not a part of the alphabet.")
			End If

			Return index.Value
		End Function

		' Converts an index to the corresponding character.
		' Index must be an index returned by charToIndex.
		Public Overridable Function indexToChar(index As Integer) As Char
			If Not (index >= 0 AndAlso index < size()) Then
				Throw New IndexOutOfRangeException("Index does not correspond to a character.")
			End If
			Return m_indexToChar(index)
		End Function

		' Converts all indices to chars using indexToChar.
		Public Overridable Function indicesToChars(indices As Integer()) As Char()
			If indices Is Nothing Then
				Throw New NullReferenceException("Indices can't be null.")
			End If

			Dim out = New Char(indices.Length - 1) {}

			For i = 0 To indices.Length - 1
				out(i) = indexToChar(indices(i))
			Next

			Return out
		End Function

		' Converts the string to indices using charToIndex.
		Public Overridable Function charsToIndices(chars As String) As Integer()
			If ReferenceEquals(chars, Nothing) Then
				Throw New NullReferenceException("Array can't be null.")
			End If

			Dim out = New Integer(chars.Length - 1) {}

			For i = 0 To chars.Length - 1
				out(i) = charToIndex(chars(i))
			Next

			Return out
		End Function
	End Class
End Namespace