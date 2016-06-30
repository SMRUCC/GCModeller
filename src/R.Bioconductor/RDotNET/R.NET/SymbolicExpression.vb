Imports RDotNet.Dynamic
Imports RDotNet.Internals
Imports System.Diagnostics
Imports System.Dynamic
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports System.Text

Imports RDotNET.REngineExtension
Imports RDotNET.SymbolicExpressionExtension

''' <summary>
''' An expression in R environment.
''' </summary>
<DebuggerDisplay("RObjectType = {Type}")> _
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class SymbolicExpression
	Inherits SafeHandle
	Implements IEquatable(Of SymbolicExpression)
	Implements IDynamicMetaObjectProvider
	Private ReadOnly m_engine As REngine
	Private ReadOnly sexp As SEXPREC

	''' <summary>
	''' An object to use to get a lock on if EnableLock is true;
	''' </summary>
	''' <remarks>
	''' Following recommended practices in http://msdn.microsoft.com/en-us/library/vstudio/c5kehkcz(v=vs.120).aspx
	''' </remarks>
	Private Shared ReadOnly lockObject As New [Object]()

	Private m_isProtected As Boolean

	''' <summary>
	''' Creates new instance of SymbolicExpression.
	''' </summary>
	''' <param name="engine">The engine.</param>
	''' <param name="pointer">The pointer.</param>
	Protected Friend Sub New(engine As REngine, pointer As IntPtr)
		MyBase.New(IntPtr.Zero, True)
		Me.m_engine = engine
		Me.sexp = CType(Marshal.PtrToStructure(pointer, GetType(SEXPREC)), SEXPREC)
		SetHandle(pointer)
		Preserve()
	End Sub

	''' <summary>
	''' Is the handle of this SEXP invalid (zero, i.e. null pointer)
	''' </summary>
	Public Overrides ReadOnly Property IsInvalid() As Boolean
		Get
			Return handle = IntPtr.Zero
		End Get
	End Property

	''' <summary>
	''' Gets the <see cref="REngine"/> to which this expression belongs.
	''' </summary>
	Public ReadOnly Property Engine() As REngine
		Get
			Return Me.m_engine
		End Get
	End Property

	''' <summary>
	''' Creates the delegate function for the specified function defined in the DLL.
	''' </summary>
	''' <typeparam name="TDelegate">The type of delegate.</typeparam>
	''' <returns>The delegate.</returns>
	Protected Friend Function GetFunction(Of TDelegate As Class)() As TDelegate
		Return Engine.GetFunction(Of TDelegate)()
	End Function

	''' <summary>
	''' Gets whether this expression is protected from the garbage collection.
	''' </summary>
	Public ReadOnly Property IsProtected() As Boolean
		Get
			Return Me.m_isProtected
		End Get
	End Property

	''' <summary>
	''' Gets the <see cref="SymbolicExpressionType"/>.
	''' </summary>
	Public ReadOnly Property Type() As SymbolicExpressionType
		Get
			Return Me.sexp.sxpinfo.type
		End Get
	End Property

	#Region "IDynamicMetaObjectProvider Members"

	''' <summary>
	''' returns a new SymbolicExpressionDynamicMeta for this SEXP
	''' </summary>
	''' <param name="parameter"></param>
	''' <returns></returns>
	Public Overridable Function GetMetaObject(parameter As System.Linq.Expressions.Expression) As DynamicMetaObject Implements IDynamicMetaObjectProvider.GetMetaObject
		Return New SymbolicExpressionDynamicMeta(parameter, Me)
	End Function

	#End Region

	#Region "IEquatable<SymbolicExpression> Members"

	''' <summary>
	''' Testing the equality of SEXP, based on handle equality.
	''' </summary>
	''' <param name="other">other SEXP</param>
	''' <returns>True if the objects have a handle that is the same, i.e. pointing to the same address in unmanaged memory</returns>
    Public Overloads Function Equals(other As SymbolicExpression) As Boolean Implements IEquatable(Of RDotNet.SymbolicExpression).Equals
        Return other IsNot Nothing AndAlso handle = other.handle
    End Function

	#End Region

	Friend Function GetInternalStructure() As SEXPREC
		Return CType(Marshal.PtrToStructure(handle, GetType(SEXPREC)), SEXPREC)
	End Function

	''' <summary>
	''' Gets all value names.
	''' </summary>
	''' <returns>The names of attributes</returns>
	Public Function GetAttributeNames() As String()
		Dim length As Integer = Me.GetFunction(Of Rf_length)()(Me.sexp.attrib)
		Dim names = New String(length - 1) {}
		Dim pointer As IntPtr = Me.sexp.attrib
		For index As Integer = 0 To length - 1
			Dim node = CType(Marshal.PtrToStructure(pointer, GetType(SEXPREC)), SEXPREC)
			Dim attribute = CType(Marshal.PtrToStructure(node.listsxp.tagval, GetType(SEXPREC)), SEXPREC)
			Dim name As IntPtr = attribute.symsxp.pname
			names(index) = New InternalString(Engine, name)
			pointer = node.listsxp.cdrval
		Next
		Return names
	End Function

	''' <summary>
	''' Gets the value of the specified name.
	''' </summary>
	''' <param name="attributeName">The name of attribute.</param>
	''' <returns>The attribute.</returns>
	Public Function GetAttribute(attributeName As String) As SymbolicExpression
		If attributeName Is Nothing Then
			Throw New ArgumentNullException()
		End If
		If attributeName = String.Empty Then
			Throw New ArgumentException()
		End If

		Dim installedName As IntPtr = Me.GetFunction(Of Rf_install)()(attributeName)
		Dim attribute As IntPtr = Me.GetFunction(Of Rf_getAttrib)()(handle, installedName)
        If CheckNil(Engine, attribute) Then
            Return Nothing
        End If
		Return New SymbolicExpression(Engine, attribute)
	End Function

	Friend Function GetAttribute(symbol As SymbolicExpression) As SymbolicExpression
		If symbol Is Nothing Then
			Throw New ArgumentNullException()
		End If
		If symbol.Type <> SymbolicExpressionType.Symbol Then
			Throw New ArgumentException()
		End If

		Dim attribute As IntPtr = Me.GetFunction(Of Rf_getAttrib)()(handle, symbol.handle)
        If CheckNil(Engine, attribute) Then
            Return Nothing
        End If
		Return New SymbolicExpression(Engine, attribute)
	End Function

	''' <summary>
	''' Sets the new value to the attribute of the specified name.
	''' </summary>
	''' <param name="attributeName">The name of attribute.</param>
	''' <param name="value">The value</param>
	Public Sub SetAttribute(attributeName As String, value As SymbolicExpression)
		If attributeName Is Nothing Then
			Throw New ArgumentNullException()
		End If
		If attributeName = String.Empty Then
			Throw New ArgumentException()
		End If

		If value Is Nothing Then
			value = Engine.NilValue
		End If

		Dim installedName As IntPtr = Me.GetFunction(Of Rf_install)()(attributeName)
		Me.GetFunction(Of Rf_setAttrib)()(handle, installedName, value.handle)
	End Sub

	Friend Sub SetAttribute(symbol As SymbolicExpression, value As SymbolicExpression)
		If symbol Is Nothing Then
			Throw New ArgumentNullException()
		End If
		If symbol.Type <> SymbolicExpressionType.Symbol Then
			Throw New ArgumentException()
		End If

		If value Is Nothing Then
			value = Engine.NilValue
		End If

		Me.GetFunction(Of Rf_setAttrib)()(handle, symbol.handle, value.handle)
	End Sub

	''' <summary>
	''' Protects the expression from R's garbage collector.
	''' </summary>
	''' <seealso cref="Unpreserve"/>
	Public Sub Preserve()
		If Not IsInvalid AndAlso Not m_isProtected Then
			If Engine.EnableLock Then
				SyncLock lockObject
					Me.GetFunction(Of R_PreserveObject)()(handle)
				End SyncLock
			Else
				Me.GetFunction(Of R_PreserveObject)()(handle)
			End If
			Me.m_isProtected = True
		End If
	End Sub

	''' <summary>
	''' Stops protection.
	''' </summary>
	''' <seealso cref="Preserve"/>
	Public Sub Unpreserve()
		If Not IsInvalid AndAlso IsProtected Then
			If Engine.EnableLock Then
				SyncLock lockObject
					Me.GetFunction(Of R_ReleaseObject)()(handle)
					

				End SyncLock
			Else
				Me.GetFunction(Of R_ReleaseObject)()(handle)
			End If
			Me.m_isProtected = False
		End If
	End Sub

	''' <summary>
	''' Release the handle on the symbolic expression, i.e. tells R to decrement the reference count to the expression in unmanaged memory
	''' </summary>
	''' <returns></returns>
	Protected Overrides Function ReleaseHandle() As Boolean
		If IsProtected Then
			Unpreserve()
		End If
		Return True
	End Function

	''' <summary>
	''' Returns the hash code for this instance.
	''' </summary>
	''' <returns>Hash code</returns>
	Public Overrides Function GetHashCode() As Integer
		Return handle.GetHashCode()
	End Function

	''' <summary>
	''' Test the equality of this object with another. If this object is also a SymbolicExpression and points to the same R expression, returns true.
	''' </summary>
	''' <param name="obj">Other object to test for equality</param>
	''' <returns>Returns true if pointing to the same R expression in memory.</returns>
	Public Overrides Function Equals(obj As Object) As Boolean
		Return Equals(TryCast(obj, SymbolicExpression))
	End Function

	''' <summary>
	''' Experimental
	''' </summary>
	''' <typeparam name="K"></typeparam>
	''' <param name="sexp"></param>
	''' <param name="name"></param>
	''' <returns></returns>
	Public Shared Function op_Dynamic(Of K)(sexp As SymbolicExpression, name As String) As SymbolicExpression
		Throw New NotImplementedException()
	End Function

    ' ''' <summary>
    ' ''' Experimental
    ' ''' </summary>
    ' ''' <typeparam name="K"></typeparam>
    ' ''' <param name="sexp"></param>
    ' ''' <param name="name"></param>
    ' ''' <param name="value"></param>
    'Public Shared Sub op_DynamicAssignment(Of K)(sexp As SymbolicExpression, name As String, value As dynamic)

    'End Sub
End Class
