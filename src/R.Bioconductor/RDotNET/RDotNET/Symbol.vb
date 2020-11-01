Imports RDotNet.Utilities
Imports System
Imports System.Runtime.InteropServices


''' <summary>
''' A symbol object.
''' </summary>
Public Class Symbol : Inherits SymbolicExpression

    ''' <summary>
    ''' Creates a symbol.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="pointer">The pointer.</param>
    Protected Friend Sub New(ByVal engine As REngine, ByVal pointer As IntPtr)
        MyBase.New(engine, pointer)
    End Sub

    ''' <summary>
    ''' Gets and sets the name.
    ''' </summary>
    Public Property PrintName As String
        Get
            Dim sexp As Object = GetInternalStructure()
            Return New InternalString(Engine, DirectCast(sexp.symsxp.pname, IntPtr)).ToString()
        End Get
        Set(ByVal value As String)
            Dim pointer As IntPtr = (If(Equals(value, Nothing), Engine.NilValue, New InternalString(Engine, value))).DangerousGetHandle()
            Dim offset = GetOffsetOf("pname")
            Marshal.WriteIntPtr(handle, offset, pointer)
        End Set
    End Property

    ''' <summary>
    ''' Gets the internal function.
    ''' </summary>
    Public ReadOnly Property Internal As SymbolicExpression
        Get
            Dim sexp As Object = GetInternalStructure()

            If Engine.EqualsRNilValue(CType(sexp.symsxp.value, IntPtr)) Then
                Return Nothing
            End If

            Return New SymbolicExpression(Engine, sexp.symsxp.internal)
        End Get
    End Property

    ''' <summary>
    ''' Gets the symbol value.
    ''' </summary>
    Public ReadOnly Property Value As SymbolicExpression
        Get
            Dim sexp As Object = GetInternalStructure()

            If Engine.EqualsRNilValue(CType(sexp.symsxp.value, IntPtr)) Then
                Return Nothing
            End If

            Return New SymbolicExpression(Engine, sexp.symsxp.value)
        End Get
    End Property

    Private Function GetOffsetOf(ByVal fieldName As String) As Integer
        Return Marshal.OffsetOf(Engine.GetSEXPRECType(), "u").ToInt32() + Marshal.OffsetOf(Engine.GetSymSxpType(), fieldName).ToInt32()
    End Function
End Class

