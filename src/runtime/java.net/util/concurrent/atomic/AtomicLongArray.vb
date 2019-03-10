Namespace util.concurrent.atomic

    Public MustInherit Class AtomicNumericArray(Of T) : Inherits AtomicArray(Of T)

        Sub New(length As Integer)
            Call MyBase.New(length)
        End Sub

        Sub New(array As T())
            Call MyBase.New(array)
        End Sub

        ''' <summary>
        ''' Atomically increments by one the element at index i.
        ''' </summary>
        ''' <param name="i">the index</param>
        ''' <returns>the previous value</returns>
        Public MustOverride Function getAndIncrement(i As Integer) As T
        ''' <summary>
        ''' Atomically decrements by one the element at index i.
        ''' </summary>
        ''' <param name="i">the index</param>
        ''' <returns>the previous value</returns>
        Public MustOverride Function getAndDecrement(i As Integer) As T
        ''' <summary>
        ''' Atomically adds the given value To the element at index i.
        ''' </summary>
        ''' <param name="i">the index</param>
        ''' <param name="delta">the value to add</param>
        ''' <returns>the previous value</returns>
        Public MustOverride Function getAndAdd(i As Integer, delta As T) As T
        ''' <summary>
        ''' Atomically increments by one the element at index i.
        ''' </summary>
        ''' <param name="i">the index</param>
        ''' <returns>the updated value</returns>
        Public MustOverride Function incrementAndGet(i As Integer) As T
        ''' <summary>
        ''' Atomically decrements by one the element at index i.
        ''' </summary>
        ''' <param name="i">the index</param>
        ''' <returns>the updated value</returns>
        Public MustOverride Function decrementAndGet(i As Integer) As T
        ''' <summary>
        ''' Atomically adds the given value To the element at index i.
        ''' </summary>
        ''' <param name="i">the index</param>
        ''' <param name="delta">the value to add</param>
        ''' <returns>the updated value</returns>
        Public MustOverride Function addAndGet(i As Integer, delta As T) As T
    End Class

    ''' <summary>
    ''' A long array in which elements may be updated atomically. See the java.util.concurrent.atomic package specification for description of the properties of atomic variables.
    ''' </summary>
    <Serializable> Public Class AtomicLongArray : Inherits AtomicNumericArray(Of Long)

        ''' <summary>
        ''' Creates a New AtomicLongArray Of the given length, With all elements initially zero.
        ''' </summary>
        ''' <param name="length">length - the length of the array</param>
        Sub New(length As Integer)
            Call MyBase.New(length)
        End Sub

        ''' <summary>
        ''' Creates a New AtomicLongArray With the same length As, And all elements copied from, the given array.
        ''' </summary>
        ''' <param name="array">array - the array to copy elements from</param>
        Sub New(array As Long())
            Call MyBase.New(array)
        End Sub

        Public Overrides Function addAndGet(i As Integer, delta As Long) As Long
            Throw New NotImplementedException()
        End Function

        Public Overrides Function decrementAndGet(i As Integer) As Long
            Throw New NotImplementedException()
        End Function

        Public Overrides Function getAndAdd(i As Integer, delta As Long) As Long
            Throw New NotImplementedException()
        End Function

        Public Overrides Function getAndDecrement(i As Integer) As Long
            Throw New NotImplementedException()
        End Function

        Public Overrides Function getAndIncrement(i As Integer) As Long
            Throw New NotImplementedException()
        End Function

        Public Overrides Function incrementAndGet(i As Integer) As Long
            Throw New NotImplementedException()
        End Function
    End Class

    <Serializable> Public Class AtomicIntegerArray : Inherits AtomicNumericArray(Of Integer)

        ''' <summary>
        ''' Creates a New AtomicLongArray Of the given length, With all elements initially zero.
        ''' </summary>
        ''' <param name="length">length - the length of the array</param>
        Sub New(length As Integer)
            Call MyBase.New(length)
        End Sub

        ''' <summary>
        ''' Creates a New AtomicLongArray With the same length As, And all elements copied from, the given array.
        ''' </summary>
        ''' <param name="array">array - the array to copy elements from</param>
        Sub New(array As Integer())
            Call MyBase.New(array)
        End Sub

        Public Overrides Function addAndGet(i As Integer, delta As Integer) As Integer
            Throw New NotImplementedException()
        End Function

        Public Overrides Function decrementAndGet(i As Integer) As Integer
            Throw New NotImplementedException()
        End Function

        Public Overrides Function getAndAdd(i As Integer, delta As Integer) As Integer
            Throw New NotImplementedException()
        End Function

        Public Overrides Function getAndDecrement(i As Integer) As Integer
            Throw New NotImplementedException()
        End Function

        Public Overrides Function getAndIncrement(i As Integer) As Integer
            Throw New NotImplementedException()
        End Function

        Public Overrides Function incrementAndGet(i As Integer) As Integer
            Throw New NotImplementedException()
        End Function
    End Class

    <Serializable> Public Class AtomicDoubleArray : Inherits AtomicNumericArray(Of Double)

        ''' <summary>
        ''' Creates a New AtomicLongArray Of the given length, With all elements initially zero.
        ''' </summary>
        ''' <param name="length">length - the length of the array</param>
        Sub New(length As Integer)
            Call MyBase.New(length)
        End Sub

        ''' <summary>
        ''' Creates a New AtomicLongArray With the same length As, And all elements copied from, the given array.
        ''' </summary>
        ''' <param name="array">array - the array to copy elements from</param>
        Sub New(array As Double())
            Call MyBase.New(array)
        End Sub

        Public Overrides Function addAndGet(i As Integer, delta As Double) As Double
            Throw New NotImplementedException()
        End Function

        Public Overrides Function decrementAndGet(i As Integer) As Double
            Throw New NotImplementedException()
        End Function

        Public Overrides Function getAndAdd(i As Integer, delta As Double) As Double
            Throw New NotImplementedException()
        End Function

        Public Overrides Function getAndDecrement(i As Integer) As Double
            Throw New NotImplementedException()
        End Function

        Public Overrides Function getAndIncrement(i As Integer) As Double
            Throw New NotImplementedException()
        End Function

        Public Overrides Function incrementAndGet(i As Integer) As Double
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace