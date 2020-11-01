Imports RDotNet.Internals
Imports System
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports System.Text


    ''' <summary>
    ''' Internal string.
    ''' </summary>
    <DebuggerDisplay("Content = {ToString()}; RObjectType = {Type}")>
    <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
    Public Class InternalString
        Inherits SymbolicExpression
        ''' <summary>
        ''' Convert string to utf8
        ''' </summary>
        ''' <param name="stringToConvert">string to convert</param>

        Public Shared Function NativeUtf8FromString(ByVal stringToConvert As String) As IntPtr
            Dim len = Encoding.UTF8.GetByteCount(stringToConvert)
            Dim buffer = New Byte(len + 1 - 1) {}
            Encoding.UTF8.GetBytes(stringToConvert, 0, stringToConvert.Length, buffer, 0)
            Dim nativeUtf8 = Marshal.AllocHGlobal(buffer.Length)
            Marshal.Copy(buffer, 0, nativeUtf8, buffer.Length)
            Return nativeUtf8
        End Function

        ''' <summary>
        ''' Convert utf8 to string
        ''' </summary>
        ''' <param name="utf8">utf8 to convert</param>

        Public Shared Function StringFromNativeUtf8(ByVal utf8 As IntPtr) As String
            Dim len = 0

            While Marshal.ReadByte(utf8, len) <> 0
                Threading.Interlocked.Increment(len)
            End While

            Dim buffer = New Byte(len - 1) {}
            Marshal.Copy(utf8, buffer, 0, buffer.Length)
            Return Encoding.UTF8.GetString(buffer)
        End Function

        ''' <summary>
        ''' Creates a new instance.
        ''' </summary>
        ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        ''' <param name="pointer">The pointer to a string.</param>
        Public Sub New(ByVal engine As REngine, ByVal pointer As IntPtr)
            MyBase.New(engine, pointer)
        End Sub

        ''' <summary>
        ''' Creates a new instance.
        ''' </summary>
        ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        ''' <param name="s">The string</param>
        Public Sub New(ByVal engine As REngine, ByVal s As String)
            MyBase.New(engine, engine.GetFunction(Of Rf_mkChar)()(s))
        End Sub

        ''' <summary>
        ''' Converts to the string into .NET Framework string.
        ''' </summary>
        ''' <param name="s">The R string.</param>
        ''' <returns>The .NET Framework string.</returns>
        Public Shared Widening Operator CType(ByVal s As InternalString) As String
            Return s.ToString()
        End Operator

        ''' <summary>
        ''' Gets the string representation of the string object.
        ''' This returns <c>"NA"</c> if the value is <c>NA</c>, whereas <see cref="GetInternalValue()"/> returns <c>null</c>.
        ''' </summary>
        ''' <returns>The string representation.</returns>
        ''' <seealso cref="GetInternalValue()"/>
        Public Overrides Function ToString() As String
            Return StringFromNativeUtf8(DataPointer)
        End Function

        ''' <summary>
        ''' Gets the string representation of the string object.
        ''' This returns <c>null</c> if the value is <c>NA</c>, whereas <see cref="ToString()"/> returns <c>"NA"</c>.
        ''' </summary>
        ''' <returns>The string representation.</returns>
        Public Function GetInternalValue() As String
            If handle = Engine.NaStringPointer Then
                Return Nothing
            End If

            Return StringFromNativeUtf8(DataPointer)
        End Function

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
    End Class

