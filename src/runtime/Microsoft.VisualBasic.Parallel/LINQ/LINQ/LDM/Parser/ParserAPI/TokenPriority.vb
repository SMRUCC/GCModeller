Namespace LDM.Parser

    ''' <summary>
    ''' Indicates priority in order of operations.
    ''' </summary>
    Public Enum TokenPriority
        ''' <summary>
        ''' Default
        ''' </summary>
        None

        ''' <summary>
        ''' Bitwise or
        ''' </summary>
        [Or]

        ''' <summary>
        ''' Bitwise and
        ''' </summary>
        [And]

        ''' <summary>
        ''' Bitwise not
        ''' </summary>
        [Not]

        ''' <summary>
        ''' Equality comparisons like &gt;, &lt;=, ==, etc.
        ''' </summary>
        Equality

        ''' <summary>
        ''' Plus or minus
        ''' </summary>
        PlusMinus

        ''' <summary>
        ''' Modulus
        ''' </summary>
        [Mod]

        ''' <summary>
        ''' Multiply or divide
        ''' </summary>
        MulDiv

        ''' <summary>
        ''' Unary minus
        ''' </summary>
        UnaryMinus
    End Enum
End Namespace