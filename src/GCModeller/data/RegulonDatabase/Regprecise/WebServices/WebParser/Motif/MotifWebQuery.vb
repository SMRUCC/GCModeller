Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.ComponentModel

Namespace Regprecise

    Public Class MotifWebQuery : Inherits WebQuery(Of String)

        Public Sub New(url As Func(Of String, String), Optional contextGuid As IToString(Of String) = Nothing, Optional parser As IObjectBuilder = Nothing, Optional prefix As Func(Of String, String) = Nothing, <CallerMemberName> Optional cache As String = Nothing, Optional interval As Integer = -1, Optional offline As Boolean = False)
            MyBase.New(url, contextGuid, parser, prefix, cache, interval, offline)
        End Sub


    End Class
End Namespace