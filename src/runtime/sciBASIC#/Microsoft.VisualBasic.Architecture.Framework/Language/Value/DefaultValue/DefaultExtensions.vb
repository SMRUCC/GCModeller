Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text

Namespace Language.Default

    Public Module DefaultExtensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Split(str As DefaultString, deli$, Optional ignoreCase As Boolean = False) As String()
            Return Splitter.Split(str.DefaultValue, deli, True, compare:=StringHelpers.IgnoreCase(flag:=ignoreCase))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Replace(str As DefaultString, find$, replaceAs$) As String
            Return str.DefaultValue.Replace(find, replaceAs)
        End Function
    End Module
End Namespace