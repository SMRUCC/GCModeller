Imports System.Reflection
Imports System.Runtime.CompilerServices

Namespace Platform.AccessController

    ''' <summary>
    ''' IP白名单
    ''' </summary>
    ''' 
    <AttributeUsage(AttributeTargets.Class Or AttributeTargets.Method, AllowMultiple:=True, Inherited:=True)>
    Public Class Accept : Inherits Attribute

        Public ReadOnly Property remote As String

        Sub New(remoteIP As String)
            Me.remote = remoteIP
        End Sub

        Public Overrides Function ToString() As String
            Return $"Accept from {remote}"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function GetWhitelist(controller As MemberInfo) As IEnumerable(Of String)
            Return controller _
                .GetCustomAttributes(Of Accept) _
                .Select(Function(accept)
                            Return accept.remote
                        End Function)
        End Function
    End Class
End Namespace