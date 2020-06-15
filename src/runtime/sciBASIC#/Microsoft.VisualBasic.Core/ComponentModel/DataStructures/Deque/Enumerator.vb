Namespace ComponentModel.Collection.Deque

    Friend Class Enumerator(Of S) : Implements IEnumerator(Of S)

        ''' <summary>
        ''' initialize with -1 to ensure that InvalidOperationException 
        ''' is thrown when Current is called befor the first call of 
        ''' MoveNext
        ''' </summary>
        Dim curIndex As Integer = -1

        ''' <summary>
        ''' version of Deque(Of T) this Enumerator is enumerating from the moment this enumerator has been created
        ''' </summary>
        ''' 
        Dim version As Long

        ''' <summary>
        ''' Deque(Of T) this enumerator is enumerating
        ''' </summary>
        Dim que As Deque(Of S)

        Public Sub New(ByVal que As Deque(Of S), ByVal version As Long)
            Me.version = version
            Me.que = que
        End Sub

        Public ReadOnly Property Current As S Implements IEnumerator(Of S).Current
            Get
                If curIndex < 0 OrElse curIndex >= que.Count OrElse version <> que.version Then
                    Throw New InvalidOperationException()
                Else
                    Return que(curIndex)
                End If
            End Get
        End Property

        Private ReadOnly Property anyCurrent As Object Implements IEnumerator.Current
            Get
                Return Current
            End Get
        End Property

        Public Sub Dispose() Implements IDisposable.Dispose
            que = Nothing
            curIndex = Nothing
            version = Nothing
        End Sub

        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            If version <> que.version Then
                Throw New InvalidOperationException()
            End If

            curIndex += 1

            If curIndex >= que.Count Then
                Return False
            End If

            Return True
        End Function

        Public Sub Reset() Implements IEnumerator.Reset
            curIndex = 0
        End Sub
    End Class
End Namespace