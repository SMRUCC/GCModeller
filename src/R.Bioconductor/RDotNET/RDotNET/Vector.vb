Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports RDotNet.Diagnostics
Imports RDotNet.Internals

''' <summary>
''' A vector base.
''' </summary>
''' <typeparam name="T">The element type.</typeparam>
<DebuggerDisplay("Length = {Length}; RObjectType = {Type}")>
<DebuggerTypeProxy(GetType(VectorDebugView(Of)))>
<SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
Public MustInherit Class Vector(Of T)
    Inherits SymbolicExpression
    Implements IEnumerable(Of T)
    ''' <summary>
    ''' Creates a new vector with the specified size.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="type">The element type.</param>
    ''' <param name="length">The length of vector.</param>
    Protected Sub New(ByVal engine As REngine, ByVal type As SymbolicExpressionType, ByVal length As Integer)
        MyBase.New(engine, engine.GetFunction(Of Rf_allocVector)()(type, length))

        If length <= 0 Then
            Throw New ArgumentOutOfRangeException("length")
        End If

        Dim empty = New Byte(length * DataSize - 1) {}
        Marshal.Copy(empty, 0, DataPointer, empty.Length)
    End Sub

    ''' <summary>
    ''' Creates a new vector with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="type">The element type.</param>
    ''' <param name="vector">The elements of vector.</param>
    Protected Sub New(ByVal engine As REngine, ByVal type As SymbolicExpressionType, ByVal vector As IEnumerable(Of T))
        MyBase.New(engine, engine.GetFunction(Of Rf_allocVector)()(type, vector.Count()))
        SetVector(vector.ToArray())
    End Sub

    ''' <summary>
    ''' Creates a new instance for a vector.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="coerced">The pointer to a vector.</param>
    Protected Sub New(ByVal engine As REngine, ByVal coerced As IntPtr)
        MyBase.New(engine, coerced)
    End Sub

    ''' <summary>
    ''' Gets or sets the element at the specified index.
    ''' </summary>
    ''' <param name="index">The zero-based index of the element to get or set.</param>
    ''' <returns>The element at the specified index.</returns>
    Default Public Overridable Property Item(ByVal index As Integer) As T
        Get

            If index < 0 OrElse Length <= index Then
                Throw New ArgumentOutOfRangeException()
            End If

            Using New ProtectedPointer(Me)

                Select Case Engine.Compatibility
                    Case REngine.CompatibilityMode.ALTREP
                        Return GetValueAltRep(index)
                    Case REngine.CompatibilityMode.PreALTREP
                        Return GetValue(index)
                    Case Else
                        Throw New Exception("Unable to access the vector element for this unknown R compatibility mode")
                End Select
            End Using
        End Get
        Set(ByVal value As T)

            If index < 0 OrElse Length <= index Then
                Throw New ArgumentOutOfRangeException()
            End If

            Using New ProtectedPointer(Me)

                Select Case Engine.Compatibility
                    Case REngine.CompatibilityMode.ALTREP
                        SetValueAltRep(index, value)
                    Case REngine.CompatibilityMode.PreALTREP
                        SetValue(index, value)
                    Case Else
                        Throw New Exception("Unable to set the vector element for this unknown R compatibility mode")
                End Select
            End Using
        End Set
    End Property

    ''' <summary>
    ''' Gets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for pre-R 3.5 </remarks>
    ''' <param name="index">The zero-based index of the element to get.</param>
    ''' <returns>The element at the specified index.</returns>
    Protected Overridable Function GetValue(ByVal index As Integer) As T
        Throw New NotSupportedException("GetValue handling not yet supported")
    End Function

    ''' <summary>
    ''' Sets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for pre-R 3.5 </remarks>
    ''' <param name="index">The zero-based index of the element to set.</param>
    ''' <param name="value">The value to set</param>
    Protected Overridable Sub SetValue(ByVal index As Integer, ByVal value As T)
        Throw New NotSupportedException("SetValue handling not yet supported")
    End Sub

    ''' <summary>
    ''' Gets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
    ''' <param name="index">The zero-based index of the element to get.</param>
    ''' <returns>The element at the specified index.</returns>
    Protected Overridable Function GetValueAltRep(ByVal index As Integer) As T
        Throw New NotSupportedException("GetValueAltRep handling not yet supported")
    End Function

    ''' <summary>
    ''' Sets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
    ''' <param name="index">The zero-based index of the element to set.</param>
    ''' <param name="value">The value to set</param>
    Protected Overridable Sub SetValueAltRep(ByVal index As Integer, ByVal value As T)
        Throw New NotSupportedException("SetValueAltRep handling not yet supported")
    End Sub


    ''' <summary>
    ''' Initializes the content of a vector with runtime speed in mind. This method protects the R vector, then call SetVectorDirect.
    ''' </summary>
    ''' <param name="values">The values to put in the vector. Length must match exactly the vector size</param>
    Public Sub SetVector(ByVal values As T())
        If values.Length <> Length Then Throw New ArgumentException("The length of the array provided differs from the vector length")

        Using New ProtectedPointer(Me)
            SetVectorDirect(values)
        End Using
    End Sub

    ''' <summary>
    ''' A method to transfer data from native to .NET managed array equivalents fast.
    ''' </summary>
    ''' <returns>Array of values in the vector</returns>
    Public Function ToArray() As T()
        Using New ProtectedPointer(Me)
            ' 
            '  as of R 3.5 DataPointer may return null if the vector 
            '  is an alternate representation
            '  See section General Vectors in  
            '  https://svn.r-project.org/R/branches/ALTREP/ALTREP.html
            '  
            '  If we are in compatibility mode for pre-ALTREP, we will always
            '  use the fast method.
            ' 
            If Engine.Compatibility = REngine.CompatibilityMode.PreALTREP OrElse DataPointer <> IntPtr.Zero Then
                Return GetArrayFast()
            Else
                Return GetAltRepArray()
            End If
        End Using
    End Function

    ''' <summary> Gets alternate rep array.</summary>
    '''
    ''' <exception cref="NotSupportedException"> Thrown when the requested operation is not supported.</exception>
    '''
    ''' <returns> An array of t.</returns>
    Public Overridable Function GetAltRepArray() As T()
        Throw New NotSupportedException("ALTVEC handling not yet supported")
    End Function

    ''' <summary>
    ''' Gets a representation as a one dimensional array of an R vector, with efficiency in mind for the unmanaged to managed transition, if possible.
    ''' </summary>
    ''' <returns></returns>
    Protected MustOverride Function GetArrayFast() As T()

    ''' <summary>
    ''' Initializes the content of a vector with runtime speed in mind. The vector must already be protected before calling this method.
    ''' </summary>
    ''' <param name="values">The values to put in the vector. Length must match exactly the vector size</param>
    Protected MustOverride Sub SetVectorDirect(ByVal values As T())

    ''' <summary>
    ''' Gets or sets the element at the specified name.
    ''' </summary>
    ''' <param name="name">The name of the element to get or set.</param>
    ''' <returns>The element at the specified name.</returns>
    Default Public Overridable Property Item(ByVal name As String) As T
        Get
            Dim index = getIndex(name)
            Return Me(index)
        End Get
        Set(ByVal value As T)
            Dim index = getIndex(name)
            Me(index) = value
        End Set
    End Property

    Private Function getIndex(ByVal name As String) As Integer
        If Equals(name, Nothing) Then Throw New ArgumentNullException("name", "indexing a vector by name requires a non-null name argument")
        Dim names = Me.Names
        If names Is Nothing Then Throw New NotSupportedException("The vector has no names defined - indexing it by name cannot be supported")
        Dim index = Array.IndexOf(names, name)
        Return index
    End Function

    ''' <summary>
    ''' Gets the number of elements.
    ''' </summary>
    Public ReadOnly Property Length As Integer
        Get
            Return GetFunction(Of Rf_length)()(handle)
        End Get
    End Property

    ''' <summary>
    ''' Gets the names of elements.
    ''' </summary>
    Public ReadOnly Property Names As String()
        Get
            Dim namesSymbol = Engine.GetPredefinedSymbol("R_NamesSymbol")
            Dim lNames = GetAttribute(namesSymbol)

            If lNames Is Nothing Then
                Return Nothing
            End If

            Dim namesVector As CharacterVector = lNames.AsCharacter()

            If namesVector Is Nothing Then
                Return Nothing
            End If

            Dim length = namesVector.Length
            Dim result = New String(length - 1) {}
            namesVector.CopyTo(result, length)
            Return result
        End Get
    End Property

    ''' <summary>
    ''' Gets the pointer for the first element.
    ''' </summary>
    Protected ReadOnly Property DataPointer As IntPtr
        Get

            Select Case Engine.Compatibility
                Case REngine.CompatibilityMode.ALTREP
                    Return GetFunction(Of DATAPTR_OR_NULL)()(DangerousGetHandle())
                Case REngine.CompatibilityMode.PreALTREP
                    Return IntPtr.Add(handle, Marshal.SizeOf(GetType(PreALTREP.VECTOR_SEXPREC)))
                Case Else
                    Throw New MemberAccessException("Unable to translate the DataPointer for this R compatibility mode")
            End Select
        End Get
    End Property

    ''' <summary>
    ''' Gets the size of an element in byte.
    ''' </summary>
    Protected MustOverride ReadOnly Property DataSize As Integer

#Region "IEnumerable<T> Members"

    ''' <summary>
    ''' Gets enumerator
    ''' </summary>
    Public Iterator Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
        For index = 0 To Length - 1
            Yield Me(index)
        Next
    End Function

    Private Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function

#End Region

    ''' <summary>
    ''' Copies the elements to the specified array.
    ''' </summary>
    ''' <param name="destination">The destination array.</param>
    ''' <param name="length">The length to copy.</param>
    ''' <param name="sourceIndex">The first index of the vector.</param>
    ''' <param name="destinationIndex">The first index of the destination array.</param>
    Public Sub CopyTo(ByVal destination As T(), ByVal length As Integer, ByVal Optional sourceIndex As Integer = 0, ByVal Optional destinationIndex As Integer = 0)
        If destination Is Nothing Then
            Throw New ArgumentNullException("destination")
        End If

        If length < 0 Then
            Throw New IndexOutOfRangeException("length")
        End If

        If sourceIndex < 0 OrElse Me.Length < sourceIndex + length Then
            Throw New IndexOutOfRangeException("sourceIndex")
        End If

        If destinationIndex < 0 OrElse destination.Length < destinationIndex + length Then
            Throw New IndexOutOfRangeException("destinationIndex")
        End If

        While Threading.Interlocked.Decrement(length) >= 0
            destination(Math.Min(Threading.Interlocked.Increment(destinationIndex), destinationIndex - 1)) = Me(Math.Min(Threading.Interlocked.Increment(sourceIndex), sourceIndex - 1))
        End While
    End Sub

    ''' <summary>
    ''' Gets the offset for the specified index.
    ''' </summary>
    ''' <param name="index">The index.</param>
    ''' <returns>The offset.</returns>
    Protected Function GetOffset(ByVal index As Integer) As Integer
        Return DataSize * index
    End Function
End Class

