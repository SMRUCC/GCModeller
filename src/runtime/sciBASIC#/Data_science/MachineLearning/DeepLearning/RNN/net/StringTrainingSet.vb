' Immutable training set for a character level RNN.
Public Class StringTrainingSet
		Implements TrainingSet
		Private dataField As String ' Data from file.
		Private alphabetField As Alphabet ' Alphabet extracted from data.

		' Constructs from data. Treats null as an empty string.
		Private Sub New(data As String)
			If ReferenceEquals(data, Nothing) Then
				data = ""
			End If

			dataField = data
			alphabetField = Alphabet.fromString(data)
		End Sub

		' Create 

		' Returns a training set with data from file (UTF-8).
		' Requires fileName != null.
		Public Shared Function fromFile(fileName As String) As StringTrainingSet
			If ReferenceEquals(fileName, Nothing) Then
				Throw New NullReferenceException("File path can't be null.")
			End If

			'string data = Files.newBufferedReader(Paths.get(fileName), StandardCharsets.UTF_8).lines().collect(Collectors.joining("\n"));
			'return new StringTrainingSet(data);
			Throw New NotImplementedException()
		End Function

		' Returns a training set created from a string.
		Public Shared Function fromString(data As String) As StringTrainingSet
			Return New StringTrainingSet(data)
		End Function

		' Main functionality 

		' Extracts out.length indices starting at index.
		' ix - input sequence
		' iy - expected output sequence (shifted by 1)
		Public Overridable Sub extract(lowerBound As Integer, ix As Integer(), iy As Integer()) Implements TrainingSet.extract
		Try
			If ix Is Nothing OrElse iy Is Nothing Then
				Throw New NullReferenceException("Output arrays can't be null.")
			End If

			If ix.Length <> iy.Length Then
				Throw New ArgumentException("Arrays must be the same size.")
			End If

			If lowerBound < 0 Then
				Throw New ArgumentException("Illegal lower bound.")
			End If

			' fetch one more symbol than the length.
			Dim upperBound = lowerBound + iy.Length + 1
			If upperBound >= dataField.Length Then
				Throw New Exception("NoMoreTrainingData")
			End If

			' prepare the input/output arrays
			Dim firstCharI As Integer
			Dim secondCharI = alphabetField.charToIndex(dataField(lowerBound))
			Dim t = 0
			Dim j = lowerBound + t + 1

			While j < upperBound
				firstCharI = secondCharI
				secondCharI = alphabetField.charToIndex(dataField(j))
				ix(t) = firstCharI
				iy(t) = secondCharI
				j += 1
				t += 1
			End While
		Catch ex As Exception
			Throw New Exception("Data doesn't match the alphabet.") ' shouldn't happen
			End Try
		End Sub

		' Getters 

		' Returns the loaded data.
		Public Overridable ReadOnly Property Data As String
			Get
				Return dataField
			End Get
		End Property

		' Returns the alphabet.
		Public Overridable ReadOnly Property Alphabet As Alphabet
			Get
				Return alphabetField
			End Get
		End Property

		' Returns data size.
		Public Overridable Function size() As Integer Implements TrainingSet.size
			Return dataField.Length
		End Function

		' Returns the alphabet size.
		Public Overridable Function vocabularySize() As Integer Implements TrainingSet.vocabularySize
			Return alphabetField.size()
		End Function
	End Class

