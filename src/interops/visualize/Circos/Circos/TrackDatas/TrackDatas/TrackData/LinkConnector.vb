Imports Microsoft.VisualBasic.Serialization.JSON

Namespace TrackDatas

    ''' <summary>
    ''' + Finally, links are a special track type which associates two ranges together. The format For links Is analogous To other data types, 
    ''' except now two coordinates are specified.
    ''' 
    ''' ```
    ''' chr12 1000 5000 chr15 5000 7000
    ''' ```
    ''' </summary>
    Public Structure link : Implements ITrackData

        Dim a As TrackData
        Dim b As TrackData

        Public Property comment As String Implements ITrackData.comment

        Public Overrides Function ToString() As String Implements ITrackData.GetLineData
            Return a.ToString & " " & b.ToString
        End Function
    End Structure

    Public Structure Connection
        Implements ITrackData

        Public Property comment As String Implements ITrackData.comment
        Public Property from As Integer
        Public Property [to] As Integer
        Public Property chr As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public ReadOnly Property IsEmpty As Boolean
            Get
                If Not String.IsNullOrEmpty(chr) OrElse from > 0 OrElse [to] > 0 Then
                    Return False
                Else
                    Return True
                End If
            End Get
        End Property

        Public Function GetLineData() As String Implements ITrackData.GetLineData
            Return $"{chr} {from} {[to]}"
        End Function
    End Structure
End Namespace