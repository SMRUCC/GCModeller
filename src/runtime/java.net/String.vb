
Namespace Tamir.SharpSsh.java
    ''' <summary>
    ''' Summary description for String.
    ''' </summary>
    Public Class [String]
        Private s As String

        Public Sub New(ByVal s As String)
            Me.s = s
        End Sub

        Public Sub New(ByVal o As Object)
            Me.New(o.ToString())
        End Sub

        Public Sub New(ByVal arr As Byte())
            Me.New(getString(arr))
        End Sub

        Public Sub New(ByVal arr As Byte(), ByVal offset As Integer, ByVal len As Integer)
            Me.New(getString(arr, offset, len))
        End Sub

        Public Shared Widening Operator CType(ByVal s1 As String) As [String]
            If Equals(s1, Nothing) Then Return Nothing
            Return New [String](s1)
        End Operator

        Public Shared Widening Operator CType(ByVal s1 As [String]) As Global.System.String
            If s1 Is Nothing Then Return Nothing
            Return s1.ToStringMethod()
        End Operator

        Public Shared Operator +(ByVal s1 As [String], ByVal s2 As [String]) As [String]
            Return New [String](s1.ToStringMethod() & s2.ToStringMethod())
        End Operator

        Public Function getBytes() As Byte()
            Return [String].getBytes(Me)
        End Function

        Public Overrides Function ToStringMethod() As String
            Return s
        End Function

        Public Function toLowerCase() As [String]
            Return ToStringMethod().ToLower()
        End Function

        Public Function startsWith(ByVal prefix As String) As Boolean
            Return ToStringMethod().StartsWith(prefix)
        End Function

        Public Function indexOf(ByVal [sub] As String) As Integer
            Return ToStringMethod().IndexOf([sub])
        End Function

        Public Function indexOf(ByVal [sub] As Char) As Integer
            Return ToStringMethod().IndexOf([sub])
        End Function

        Public Function indexOf(ByVal [sub] As Char, ByVal i As Integer) As Integer
            Return ToStringMethod().IndexOf([sub], i)
        End Function

        Public Function charAt(ByVal i As Integer) As Char
            Return s(i)
        End Function

        Public Function substring(ByVal start As Integer, ByVal [end] As Integer) As [String]
            Dim len = [end] - start
            Return ToStringMethod().Substring(start, len)
        End Function

        Public Function subStringMethod(ByVal start As Integer, ByVal len As Integer) As [String]
            Return substring(start, len)
        End Function

        Public Function substring(ByVal len As Integer) As [String]
            Return ToStringMethod().Substring(len)
        End Function

        Public Function subStringMethod(ByVal len As Integer) As [String]
            Return substring(len)
        End Function

        Public Function Length() As Integer
            Return ToStringMethod().Length
        End Function

        Public Function lengthMethod() As Integer
            Return Length()
        End Function

        Public Function endsWith(ByVal str As String) As Boolean
            Return s.EndsWith(str)
        End Function

        Public Function lastIndexOf(ByVal str As String) As Integer
            Return s.LastIndexOf(str)
        End Function

        Public Function lastIndexOf(ByVal c As Char) As Integer
            Return s.LastIndexOf(c)
        End Function

        Public Function equals(ByVal o As Object) As Boolean
            Return ToStringMethod().Equals(o.ToString())
        End Function

        Public Overrides Function EqualsMethod(ByVal obj As Object) As Boolean
            Return equals(obj)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return s.GetHashCode()
        End Function

        Public Shared Function getString(ByVal arr As Byte()) As String
            Return getString(arr, 0, arr.Length)
        End Function

        Public Shared Function getString(ByVal arr As Byte(), ByVal offset As Integer, ByVal len As Integer) As String
            Return Global.System.Text.Encoding.Default.GetString(arr, offset, len)
        End Function

        Public Shared Function getStringUTF8(ByVal arr As Byte()) As String
            Return getStringUTF8(arr, 0, arr.Length)
        End Function

        Public Shared Function getStringUTF8(ByVal arr As Byte(), ByVal offset As Integer, ByVal len As Integer) As String
            Return Global.System.Text.Encoding.UTF8.GetString(arr, offset, len)
        End Function

        Public Shared Function getBytes(ByVal str As String) As Byte()
            Return getBytesUTF8(str)
        End Function

        Public Shared Function getBytesUTF8(ByVal str As String) As Byte()
            Return Global.System.Text.Encoding.UTF8.GetBytes(str)
        End Function
    End Class
End Namespace
