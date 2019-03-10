Namespace util.concurrent.atomic

    ''' <summary>
    ''' An int value that may be updated atomically. See the java.util.concurrent.atomic package specification for description of the properties of atomic variables. An AtomicInteger is used in applications such as atomically incremented counters, and cannot be used as a replacement for an Integer. However, this class does extend Number to allow uniform access by tools and utilities that deal with numerically-based classes.
    ''' </summary>
    Public Class AtomicInteger : Inherits Java.Lang.Number(Of Integer)

        Private v As Integer

        ''' <summary>
        ''' Creates a New AtomicInteger With the given initial value.
        ''' </summary>
        ''' <param name="initialValue">the initial value</param>
        Sub New(initialValue As Integer)

        End Sub

        ''' <summary>
        ''' Creates a New AtomicInteger With initial value 0.
        ''' </summary>
        Sub New()

        End Sub

        Public Overrides Function [Get]() As Global.System.Int32
            Throw New NotImplementedException()
        End Function

        Public Overrides Sub [Set](newValue As Global.System.Int32)
            Throw New NotImplementedException()
        End Sub

        Public Overrides Sub lazySet(newValue As Global.System.Int32)
            Throw New NotImplementedException()
        End Sub

        Public Overrides Function getAndSet(newValue As Global.System.Int32) As Global.System.Int32
            Throw New NotImplementedException()
        End Function

        Public Overrides Function compareAndSet(expect As Global.System.Int32, update As Global.System.Int32) As Global.System.Boolean
            Throw New NotImplementedException()
        End Function

        Public Overrides Function weakCompareAndSet(expect As Global.System.Int32, update As Global.System.Int32) As Global.System.Boolean
            Throw New NotImplementedException()
        End Function

        Public Overrides Function getAndIncrement() As Global.System.Int32
            Throw New NotImplementedException()
        End Function

        Public Overrides Function getAndDecrement() As Global.System.Int32
            Throw New NotImplementedException()
        End Function

        Public Overrides Function getAndAdd(delta As Global.System.Int32) As Global.System.Int32
            Throw New NotImplementedException()
        End Function

        Public Overrides Function incrementAndGet() As Global.System.Int32
            Throw New NotImplementedException()
        End Function

        Public Overrides Function decrementAndGet() As Global.System.Int32
            Throw New NotImplementedException()
        End Function

        Public Overrides Function addAndGet(delta As Global.System.Int32) As Global.System.Int32
            Throw New NotImplementedException()
        End Function
    End Class

    Public Class AtomicLong : Inherits Java.Lang.Number(Of Long)

        Sub New()
        End Sub

        Sub New(InitValue As Long)
            Call MyBase.New(InitValue)
        End Sub

        Public Overrides Sub lazySet(newValue As Global.System.Int64)
            Throw New NotImplementedException()
        End Sub

        Public Overrides Sub [Set](newValue As Global.System.Int64)
            Throw New NotImplementedException()
        End Sub

        Public Overrides Function addAndGet(delta As Global.System.Int64) As Global.System.Int64
            Throw New NotImplementedException()
        End Function

        Public Overrides Function compareAndSet(expect As Global.System.Int64, update As Global.System.Int64) As Global.System.Boolean
            Throw New NotImplementedException()
        End Function

        Public Overrides Function decrementAndGet() As Global.System.Int64
            Throw New NotImplementedException()
        End Function

        Public Overrides Function [Get]() As Global.System.Int64
            Throw New NotImplementedException()
        End Function

        Public Overrides Function getAndAdd(delta As Global.System.Int64) As Global.System.Int64
            Throw New NotImplementedException()
        End Function

        Public Overrides Function getAndDecrement() As Global.System.Int64
            Throw New NotImplementedException()
        End Function

        Public Overrides Function getAndIncrement() As Global.System.Int64
            Throw New NotImplementedException()
        End Function

        Public Overrides Function getAndSet(newValue As Global.System.Int64) As Global.System.Int64
            Throw New NotImplementedException()
        End Function

        Public Overrides Function incrementAndGet() As Global.System.Int64
            Throw New NotImplementedException()
        End Function

        Public Overrides Function weakCompareAndSet(expect As Global.System.Int64, update As Global.System.Int64) As Global.System.Boolean
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace