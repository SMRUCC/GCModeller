Namespace Lang

    Public MustInherit Class Number(Of T)

        ''' <summary>
        ''' Creates a New AtomicInteger With the given initial value.
        ''' </summary>
        ''' <param name="initialValue">the initial value</param>
        Sub New(initialValue As T)

        End Sub

        ''' <summary>
        ''' Creates a New AtomicInteger With initial value 0.
        ''' </summary>
        Sub New()

        End Sub

        ''' <summary>
        ''' Gets the current value.
        ''' </summary>
        ''' <returns>the current value</returns>
        Public MustOverride Function [Get]() As T

        ''' <summary>
        ''' Sets to the given value.
        ''' </summary>
        ''' <param name="newValue">the New value</param>
        Public MustOverride Sub [Set](newValue As T)

        ''' <summary>
        ''' Eventually sets To the given value.
        ''' </summary>
        ''' <param name="newValue">the New value</param>
        Public MustOverride Sub lazySet(newValue As T)

        ''' <summary>
        ''' Atomically sets To the given value And returns the old value.
        ''' </summary>
        ''' <param name="newValue">the New value</param>
        ''' <returns>the previous value</returns>
        Public MustOverride Function getAndSet(newValue As T) As T

        ''' <summary>
        ''' Atomically sets the value To the given updated value If the current value == the expected value.
        ''' </summary>
        ''' <param name="expect">the expected value</param>
        ''' <param name="update">the New value</param>
        ''' <returns>true if successful. False return indicates that the actual value was Not equal to the expected value.</returns>
        Public MustOverride Function compareAndSet(expect As T,
                     update As T) As Boolean

        ''' <summary>
        ''' Atomically sets the value To the given updated value If the current value == the expected value.
        ''' May fail spuriously And does Not provide ordering guarantees, so Is only rarely an appropriate alternative To compareAndSet.
        ''' </summary>
        ''' <param name="expect">the expected value</param>
        ''' <param name="update">the New value</param>
        ''' <returns>true if successful.</returns>
        Public MustOverride Function weakCompareAndSet(expect As T,
                        update As T) As Boolean

        ''' <summary>
        ''' Atomically increments by one the current value.
        ''' </summary>
        ''' <returns>the previous value</returns>
        Public MustOverride Function getAndIncrement() As T

        ''' <summary>
        ''' Atomically decrements by one the current value.
        ''' </summary>
        ''' <returns>the previous value</returns>
        Public MustOverride Function getAndDecrement() As T

        ''' <summary>
        ''' Atomically adds the given value To the current value.
        ''' </summary>
        ''' <param name="delta">the value to add</param>
        ''' <returns>the previous value</returns>
        Public MustOverride Function getAndAdd(delta As T) As T

        ''' <summary>
        ''' Atomically increments by one the current value.
        ''' </summary>
        ''' <returns>the updated value</returns>
        Public MustOverride Function incrementAndGet() As T

        ''' <summary>
        ''' Atomically decrements by one the current value.
        ''' </summary>
        ''' <returns>the updated value</returns>
        Public MustOverride Function decrementAndGet() As T

        ''' <summary>
        ''' Atomically adds the given value To the current value.
        ''' </summary>
        ''' <param name="delta">the value to add</param>
        ''' <returns>the updated value</returns>
        Public MustOverride Function addAndGet(delta As T) As T

        ''' <summary>
        ''' Returns the String representation Of the current value.
        ''' </summary>
        ''' <returns>the String representation of the current value.</returns>
        Public Overrides Function toString() As String

        End Function

        ''' <summary>
        ''' Returns the value Of the specified number As an int. This may involve rounding Or truncation.
        ''' </summary>
        ''' <returns>the numeric value represented by this Object after conversion To type int.</returns>
        Public Function intValue() As Integer

        End Function

        ''' <summary>
        ''' Returns the value Of the specified number As a Long. This may involve rounding Or truncation.
        ''' </summary>
        ''' <returns>the numeric value represented by this Object after conversion To type Long.</returns>
        Public Function longValue() As Long

        End Function

        ''' <summary>
        ''' Returns the value Of the specified number As a float. This may involve rounding.
        ''' </summary>
        ''' <returns>the numeric value represented by this Object after conversion To type float.</returns>
        Public Function floatValue() As Double

        End Function

        ''' <summary>
        ''' Returns the value Of the specified number As a Double. This may involve rounding.
        ''' </summary>
        ''' <returns>the numeric value represented by this Object after conversion To type Double.</returns>
        Public Function doubleValue() As Double

        End Function
    End Class

    Public Module [Double]

        ''' <summary>
        ''' Returns a representation of the specified floating-point value according to the IEEE 754 floating-point "double format" bit layout.
        ''' 
        ''' Bit 63 (the bit that is selected by the mask 0x8000000000000000L) represents the sign of the floating-point number. Bits 62-52 (the bits that are selected by the mask 0x7ff0000000000000L) represent the exponent. Bits 51-0 (the bits that are selected by the mask 0x000fffffffffffffL) represent the significand (sometimes called the mantissa) of the floating-point number.
        ''' If the argument Is positive infinity, the result Is 0x7ff0000000000000L.
        ''' If the argument Is negative infinity, the result Is 0xfff0000000000000L.
        ''' If the argument Is NaN, the result Is 0x7ff8000000000000L.
        ''' In all cases, the result Is a long integer that, when given to the longBitsToDouble(long) method, will produce a floating-point value the same as the argument to doubleToLongBits (except all NaN values are collapsed to a single "canonical" NaN value).
        ''' </summary>
        ''' <param name="value">a double precision floating-point number.</param>
        ''' <returns>the bits that represent the floating-point number.</returns>
        Public Function doubleToRawLongBits(value As Double) As Long


        End Function

        ''' <summary>
        ''' Returns the double value corresponding to a given bit representation. The argument is considered to be a representation of a floating-point value according to the IEEE 754 floating-point "double format" bit layout.
        '''  If the argument Is 0x7ff0000000000000L, the result Is positive infinity.
        ''' 
        ''' If the argument Is 0xfff0000000000000L, the result Is negative infinity.
        ''' 
        ''' If the argument Is any value In the range 0x7ff0000000000001L through 0x7fffffffffffffffL Or In the range 0xfff0000000000001L through 0xffffffffffffffffL, the result Is a NaN. No IEEE 754 floating-point operation provided by Java can distinguish between two NaN values Of the same type With different bit patterns. Distinct values Of NaN are only distinguishable by use Of the Double.doubleToRawLongBits method.
        ''' 
        ''' In all other cases, let s, e, And m be three values that can be computed from the argument:
        ''' 
        '''  int s = ((bits >> 63) == 0) ? 1 : -1;
        '''  int e = (Int())((bits >> 52) &amp; 0x7ffL);
        '''  Long m = (e == 0) ?
        '''      (bits &amp; 0xfffffffffffffL) &lt;&lt; 1 :
        '''    (bits &amp; 0xfffffffffffffL) | 0x10000000000000L;
        '''  
        ''' Then the floating-point result equals the value of the mathematical expression s·m·2e-1075.
        ''' Note that this method may Not be able To Return a Double NaN With exactly same bit pattern As the Long argument. IEEE 754 distinguishes between two kinds Of NaNs, quiet NaNs And signaling NaNs. The differences between the two kinds Of NaN are generally Not visible In Java. Arithmetic operations On signaling NaNs turn them into quiet NaNs With a different, but often similar, bit pattern. However, On some processors merely copying a signaling NaN also performs that conversion. In particular, copying a signaling NaN To Return it To the calling method may perform this conversion. So longBitsToDouble may Not be able To Return a Double With a signaling NaN bit pattern. Consequently, For some Long values, doubleToRawLongBits(longBitsToDouble(start)) may Not equal start. Moreover, which particular bit patterns represent signaling NaNs Is platform dependent; although all NaN bit patterns, quiet Or signaling, must be In the NaN range identified above.
        ''' </summary>
        ''' <param name="bits"></param>
        ''' <returns></returns>
        Public Function longBitsToDouble(bits As Long) As Double

        End Function

    End Module
End Namespace