Imports RDotNet.Dynamic
Imports RDotNet.Internals
Imports System
Imports System.Collections.Generic
Imports System.Dynamic
Imports System.Runtime.InteropServices
Imports System.Security.Permissions


    ''' <summary>
    ''' A generic list. This is also known as list in R.
    ''' </summary>
    <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
    Public Class GenericVector
        Inherits Vector(Of SymbolicExpression)
        ''' <summary>
        ''' Creates a new empty GenericVector with the specified length.
        ''' </summary>
        ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        ''' <param name="length">The length.</param>
        Public Sub New(ByVal engine As REngine, ByVal length As Integer)
            MyBase.New(engine, engine.GetFunction(Of Rf_allocVector)()(SymbolicExpressionType.List, length))
        End Sub

        ''' <summary>
        ''' Creates a new GenericVector with the specified values.
        ''' </summary>
        ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        ''' <param name="list">The values.</param>
        Public Sub New(ByVal engine As REngine, ByVal list As IEnumerable(Of SymbolicExpression))
            MyBase.New(engine, SymbolicExpressionType.List, list)
        End Sub

        ''' <summary>
        ''' Creates a new instance for a list.
        ''' </summary>
        ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        ''' <param name="coerced">The pointer to a list.</param>
        Protected Friend Sub New(ByVal engine As REngine, ByVal coerced As IntPtr)
            MyBase.New(engine, coerced)
        End Sub

        ''' <summary>
        ''' Gets the element at the specified index.
        ''' </summary>
        ''' <remarks>Used for pre-R 3.5 </remarks>
        ''' <param name="index">The zero-based index of the element to get.</param>
        ''' <returns>The element at the specified index.</returns>
        Protected Overrides Function GetValue(ByVal index As Integer) As SymbolicExpression
            Dim offset = GetOffset(index)
            Dim pointer = Marshal.ReadIntPtr(DataPointer, offset)
            Return New SymbolicExpression(Engine, pointer)
        End Function

        ''' <summary>
        ''' Gets the element at the specified index.
        ''' </summary>
        ''' <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
        ''' <param name="index">The zero-based index of the element to get.</param>
        ''' <returns>The element at the specified index.</returns>
        Protected Overrides Function GetValueAltRep(ByVal index As Integer) As SymbolicExpression
            Return GetValue(index)
        End Function

        ''' <summary>
        ''' Sets the element at the specified index.
        ''' </summary>
        ''' <remarks>Used for pre-R 3.5 </remarks>
        ''' <param name="index">The zero-based index of the element to set.</param>
        ''' <param name="value">The value to set</param>
        Protected Overrides Sub SetValue(ByVal index As Integer, ByVal value As SymbolicExpression)
            Dim offset = GetOffset(index)
            Marshal.WriteIntPtr(DataPointer, offset, If(value, Engine.NilValue).DangerousGetHandle())
        End Sub

        ''' <summary>
        ''' Sets the element at the specified index.
        ''' </summary>
        ''' <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
        ''' <param name="index">The zero-based index of the element to set.</param>
        ''' <param name="value">The value to set</param>
        Protected Overrides Sub SetValueAltRep(ByVal index As Integer, ByVal value As SymbolicExpression)
            SetValue(index, value)
        End Sub

        ''' <summary>
        ''' Efficient conversion from R vector representation to the array equivalent in the CLR
        ''' </summary>
        ''' <returns>Array equivalent</returns>
        Protected Overrides Function GetArrayFast() As SymbolicExpression()
            Dim res = New SymbolicExpression(Length - 1) {}
            Dim useAltRep = Engine.Compatibility = REngine.CompatibilityMode.ALTREP

            For i = 0 To res.Length - 1
                res(i) = If(useAltRep, GetValueAltRep(i), GetValue(i))
            Next

            Return res
        End Function

        ''' <summary>
        ''' Efficient initialisation of R vector values from an array representation in the CLR
        ''' </summary>
        Protected Overrides Sub SetVectorDirect(ByVal values As SymbolicExpression())
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
        ''' Gets the size of each item in this vector
        ''' </summary>
        Protected Overrides ReadOnly Property DataSize As Integer
            Get
                Return Marshal.SizeOf(GetType(IntPtr))
            End Get
        End Property

        ''' <summary>
        ''' Converts into a <see cref="RDotNet.Pairlist"/>.
        ''' </summary>
        ''' <returns>The pairlist.</returns>
        Public Function ToPairlist() As Pairlist
            Return New Pairlist(Engine, GetFunction(Of Rf_VectorToPairList)()(handle))
        End Function

        ''' <summary>
        ''' returns a new ListDynamicMeta for this Generic Vector
        ''' </summary>
        ''' <param name="parameter"></param>
        ''' <returns></returns>
        Public Overrides Function GetMetaObject(ByVal parameter As Expressions.Expression) As DynamicMetaObject
            Return New ListDynamicMeta(parameter, Me)
        End Function

        ''' <summary> Sets the names of the vector. </summary>
        '''
        ''' <param name="names"> A variable-length parameters list containing names.</param>
        Public Sub SetNames(ParamArray names As String())
            Dim cv As CharacterVector = New CharacterVector(Engine, names)
            SetNames(cv)
        End Sub

        ''' <summary> Sets the names of the vector.</summary>
        '''
        ''' <exception cref="ArgumentException"> Incorrect length, not equal to vector length</exception>
        '''
        ''' <param name="names"> A variable-length parameters list containing names.</param>
        Public Sub SetNames(ByVal names As CharacterVector)
            If names.Length <> Length Then
                Throw New ArgumentException("Names vector must be same length as list")
            End If

            Dim namesSymbol = Engine.GetPredefinedSymbol("R_NamesSymbol")
            SetAttribute(namesSymbol, names)
        End Sub
    End Class

