Namespace util.concurrent.atomic

    <Serializable> Public MustInherit Class AtomicArray(Of T)

        Protected _InternalChunkBuffer As T()

        Sub New(length As Integer)
            _InternalChunkBuffer = New T(length - 1) {}
        End Sub

        Sub New(array As T())
            _InternalChunkBuffer = array
        End Sub

        Default Public Property Element(i As Integer) As T
            Get
                Return _InternalChunkBuffer(i)
            End Get
            Set(value As T)
                _InternalChunkBuffer(i) = value
            End Set
        End Property

        ''' <summary>
        ''' Returns the length Of the array.
        ''' </summary>
        ''' <returns>the length Of the array</returns>
        Public Function length() As Integer
            Return _InternalChunkBuffer.Count
        End Function

        ''' <summary>
        ''' Gets the current value at position i.
        ''' </summary>
        ''' <param name="i">the index</param>
        ''' <returns>the current value</returns>
        Public Function [Get](i As Integer) As T
            Return _InternalChunkBuffer(i)
        End Function

        ''' <summary>
        ''' Sets the element at position i To the given value.
        ''' </summary>
        ''' <param name="i">the index</param>
        ''' <param name="newValue">the New value</param>
        Public Sub [Set](i As Integer, newValue As T)
            _InternalChunkBuffer(i) = newValue
        End Sub

        ''' <summary>
        ''' Eventually sets the element at position i To the given value.
        ''' </summary>
        ''' <param name="i">the index</param>
        ''' <param name="newValue">the New value</param>
        Public Sub lazySet(i As Integer, newValue As T)

        End Sub

        ''' <summary>
        ''' Atomically sets the element at position i To the given value And returns the old value.
        ''' </summary>
        ''' <param name="i">the index</param>
        ''' <param name="newValue">the New value</param>
        ''' <returns>the previous value</returns>
        Public Function getAndSet(i As Integer, newValue As T) As T
            Dim old As T = _InternalChunkBuffer(i)
            _InternalChunkBuffer(i) = newValue
            Return old
        End Function

        ''' <summary>
        ''' Atomically sets the element at position i To the given updated value If the current value == the expected value.
        ''' </summary>
        ''' <param name="i">the index</param>
        ''' <param name="expect">the expected value</param>
        ''' <param name="update">the New value</param>
        ''' <returns>true if successful. False return indicates that the actual value was Not equal to the expected value.</returns>
        Public Function compareAndSet(i As Integer, expect As T, update As T) As Boolean

        End Function

        ''' <summary>
        ''' Atomically sets the element at position i To the given updated value If the current value == the expected value.
        ''' May fail spuriously And does Not provide ordering guarantees, so Is only rarely an appropriate alternative To compareAndSet.
        ''' </summary>
        ''' <param name="i">the index</param>
        ''' <param name="expect">the expected value</param>
        ''' <param name="update">the New value</param>
        ''' <returns>true if successful</returns>
        Public Function weakCompareAndSet(i As Integer, expect As T, update As T) As Boolean

        End Function

        ''' <summary>
        ''' Returns the String representation Of the current values Of array.
        ''' </summary>
        ''' <returns>the String representation of the current values of array</returns>
        Public Overrides Function toString() As String
            Return Global.System.String.Join(", ", _InternalChunkBuffer)
        End Function
    End Class
End Namespace