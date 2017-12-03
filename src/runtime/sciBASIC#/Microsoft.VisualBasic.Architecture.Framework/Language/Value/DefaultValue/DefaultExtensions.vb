Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text

Namespace Language

    Public Module DefaultExtensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Split(str As DefaultString, deli$, Optional ignoreCase As Boolean = False) As String()
            Return Splitter.Split(str.DefaultValue, deli, True, compare:=StringHelpers.IgnoreCase(flag:=ignoreCase))
        End Function
    End Module
End Namespace