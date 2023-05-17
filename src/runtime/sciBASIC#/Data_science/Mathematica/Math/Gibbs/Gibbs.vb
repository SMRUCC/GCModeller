Imports randf2 = Microsoft.VisualBasic.Math.RandomExtensions

''' <summary>
''' Gibb's Sampling Steps:<br>
''' 
''' 1. Set every variable to a random value.<br>
''' 2. Choose a variable to update. <br>
''' 3. Randomly Select (aka "Sample") a new value for the variable based on the
'''    current conditions. <br>
''' 4. Repeat from Step 2.
''' </summary>  
Public Class Gibbs

    Private Shared sequences As List(Of String) = New List(Of String)()
    Friend start As Dictionary(Of String, Integer?)
    Private motifLength As Integer

    ''' <summary>
    ''' Constructs and performs Gibb's Sampling in order to find repeated motifs.
    ''' </summary>
    ''' <paramname="seq">
    '''            A String array of the sequences that will be used. </param>
    ''' <paramname="motifLength">
    '''            An Integer that shows the length of the motif or pattern we
    '''            are trying to find, this value is given. </param>
    Public Sub New(ByVal seqArray As String(), ByVal motifLength As Integer)
        Gibbs.sequences.AddRange(seqArray)
        Me.motifLength = motifLength
        start = generateRandomValue()
        sample()
        Console.WriteLine(start)
    End Sub

    ''' <summary>
    ''' This method is repeated 2000 times.
    ''' </summary>
    ''' <paramname="start">
    '''            A HashTable containing the sequence as a key, and the random
    '''            integer to be used as the value. </param>
    Private Sub sample()
        Dim rand As Random = randf2.seeds

        For j = 0 To 1999
            Dim chosenSeqIndex = rand.Next(Gibbs.sequences.Count)
            Dim chosenSequence As String = Gibbs.sequences(chosenSeqIndex)
            Dim scores As List(Of Double?) = New List(Of Double?)()
            ' i = possibleStart
            For i = 0 To chosenSequence.Length - motifLength + 1 - 1
                Dim tempMotif = chosenSequence.Substring(i, motifLength)
                Dim p = calculateP(tempMotif, chosenSeqIndex)
                Dim q = calculateQ(tempMotif, chosenSeqIndex, i)
                scores.Add(q / p)
            Next
            Dim sum As Double = 0
            For Each d As Double In scores
                sum += d
            Next
            For i = 0 To scores.Count - 1
                scores(0) = scores(i) / sum
            Next

            Dim random As Double = rand.NextDouble()
            Dim dubsum As Double = 0
            For Each d As Double In scores
                dubsum += d
                If random = dubsum Then
                    start(chosenSequence) = scores.IndexOf(d)
                End If
            Next
        Next

    End Sub

    ''' <summary>
    ''' Calculates the probability of a letter in this position.
    ''' </summary>
    ''' <paramname="tempMotif">
    '''            The motif being used for this calculation. </param>
    ''' <paramname="chosenSeqIndex">
    '''            The index of the sequence being used for this calculation,
    '''            useful for skipping all of this sequences calculations and
    '''            focusing on the other ones. </param>
    ''' <returns> A double of the probability of a letter in this position. </returns>
    Private Function calculateQ(ByVal tempMotif As String, ByVal chosenSeqIndex As Integer, ByVal possibleStart As Integer) As Double
        Dim q As Double = 1
        Dim start = possibleStart
        Dim [end] = possibleStart + tempMotif.Length
        Dim denominator As Double = Gibbs.sequences.Count - 1
        For Each s As String In Gibbs.sequences
            Dim numerator As Double = 0
            If s.Equals(Gibbs.sequences(chosenSeqIndex)) Then
                Continue For
            End If
            If [end] > s.Length Then
                q *= 0.01
                Continue For
            End If
            Dim thisMotif = s.Substring(start, [end] - start)
            Dim letters As Char() = tempMotif.ToCharArray()
            Dim seqLetters As Char() = thisMotif.ToCharArray()

            For i = 0 To tempMotif.Length - 1
                If letters(i) = seqLetters(i) Then
                    numerator += 1
                End If
            Next
            If numerator = 0 Then
                q *= 0.01
            Else
                q *= numerator / denominator
            End If
        Next
        Return q
    End Function

    ''' <summary>
    ''' Calculates the probability of a letter randomly selected.
    ''' 
    ''' To find this value, the method loops through each letter of the selected
    ''' temporary motif, and loops through the other sequences. While looping
    ''' through the other sequences, we find the amount of same letters in each
    ''' other sequence, along with the total length of all other sequences. The
    ''' value P is a product of every result, each result being the amount of
    ''' letters of the same kind over the total amount of letters.
    ''' </summary>
    ''' <paramname="tempMotif">
    '''            The motif being used for this calculation. </param>
    ''' <paramname="chosenSeqIndex">
    '''            The index of the sequence being used for this calculation,
    '''            useful for skipping all of this sequences calculations and
    '''            focusing on the other ones. </param>
    ''' <returns> A double of the probability of a letter randomly selected. </returns>
    Private Function calculateP(ByVal tempMotif As String, ByVal chosenSeqIndex As Integer) As Double
        Dim p As Double = 1
        For Each c As Char In tempMotif.ToCharArray()
            Dim sameLetters As Double = 0
            Dim totalLength As Double = 0
            For Each s As String In Gibbs.sequences
                If s.Equals(Gibbs.sequences(chosenSeqIndex)) Then
                    Continue For
                End If
                Dim seqLetters As Char() = s.ToCharArray()
                For Each x As Char In seqLetters
                    If c = x Then
                        sameLetters += 1
                    End If
                Next
                totalLength += s.Length
            Next
            p *= sameLetters / totalLength
        Next
        Return p
    End Function

    ''' <summary>
    ''' Calculates and stores every random value. Generates a random from 0 to a
    ''' value of each individual sequences length subtracted by the motif length.
    ''' </summary>
    ''' <returns> A HashTable containing the sequence as a key, and the random
    '''         integer to be used as the value. </returns>
    Private Function generateRandomValue() As Dictionary(Of String, Integer?)
        Dim rand As Random = randf2.seeds
        Dim randomValues As New Dictionary(Of String, Integer?)()
        For Each seq As String In Gibbs.sequences
            Dim randomVal = rand.Next(seq.Length - motifLength)
            randomValues(seq) = randomVal
        Next
        Return randomValues
    End Function

    Public Shared Sub Main(ByVal args As String())
        Dim data = New String() {"ABCDAAAABDB", "AAAADCBBCA", "DDBCABAAAACBBD", "AABAAAACCDD"}
        Dim length = 4
        Dim gibbs As Gibbs = New Gibbs(data, length)
    End Sub

End Class
