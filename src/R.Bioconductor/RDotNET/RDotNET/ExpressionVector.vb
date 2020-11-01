Imports System
Imports System.Runtime.InteropServices
Imports System.Security.Permissions


    ''' <summary>
    ''' A vector of S expressions
    ''' </summary>
    <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
    Public Class ExpressionVector
        Inherits Vector(Of Expression)
        ''' <summary>
        ''' Creates a new instance for an expression vector.
        ''' </summary>
        ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        ''' <param name="coerced">The pointer to an expression vector.</param>
        Friend Sub New(ByVal engine As REngine, ByVal coerced As IntPtr)
            MyBase.New(engine, coerced)
        End Sub

        ''' <summary>
        ''' Gets an array representation of a vector of SEXP in R. Note that the implementation cannot be particularly "fast" in spite of the name.
        ''' </summary>
        ''' <returns></returns>
        Protected Overrides Function GetArrayFast() As Expression()
            Dim res = New Expression(Length - 1) {}
            Dim useAltRep = Engine.Compatibility = REngine.CompatibilityMode.ALTREP

            For i = 0 To res.Length - 1
                res(i) = If(useAltRep, GetValueAltRep(i), GetValue(i))
            Next

            Return res
        End Function

        ''' <summary>
        ''' Gets the element at the specified index.
        ''' </summary>
        ''' <remarks>Used for pre-R 3.5 </remarks>
        ''' <param name="index">The zero-based index of the element to get.</param>
        ''' <returns>The element at the specified index.</returns>
        Protected Overrides Function GetValue(ByVal index As Integer) As Expression
            Dim offset = GetOffset(index)
            Dim pointer = Marshal.ReadIntPtr(DataPointer, offset)
            Return New Expression(Engine, pointer)
        End Function

        ''' <summary>
        ''' Gets the element at the specified index.
        ''' </summary>
        ''' <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
        ''' <param name="index">The zero-based index of the element to get.</param>
        ''' <returns>The element at the specified index.</returns>
        Protected Overrides Function GetValueAltRep(ByVal index As Integer) As Expression
            Return GetValue(index)
        End Function

        ''' <summary>
        ''' Sets the element at the specified index.
        ''' </summary>
        ''' <remarks>Used for pre-R 3.5 </remarks>
        ''' <param name="index">The zero-based index of the element to set.</param>
        ''' <param name="value">The value to set</param>
        Protected Overrides Sub SetValue(ByVal index As Integer, ByVal value As Expression)
            Dim offset = GetOffset(index)
            Marshal.WriteIntPtr(DataPointer, offset, If(value, Engine.NilValue).DangerousGetHandle())
        End Sub

        ''' <summary>
        ''' Sets the element at the specified index.
        ''' </summary>
        ''' <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
        ''' <param name="index">The zero-based index of the element to set.</param>
        ''' <param name="value">The value to set</param>
        Protected Overrides Sub SetValueAltRep(ByVal index As Integer, ByVal value As Expression)
            SetValue(index, value)
        End Sub

        ''' <summary>
        ''' Efficient initialisation of R vector values from an array representation in the CLR
        ''' </summary>
        Protected Overrides Sub SetVectorDirect(ByVal values As Expression())
            Dim useAltRep = Engine.Compatibility = REngine.CompatibilityMode.ALTREP

            For i = 0 To values.Length - 1

                If useAltRep Then
                    SetValueAltRep(i, values(i))
                End If

                If True Then
                    SetValue(i, values(i))
                End If
            Next
        End Sub

        ''' <summary>
        ''' Gets the size of a pointer in byte.
        ''' </summary>
        Protected Overrides ReadOnly Property DataSize As Integer
            Get
                Return Marshal.SizeOf(GetType(IntPtr))
            End Get
        End Property
    End Class

