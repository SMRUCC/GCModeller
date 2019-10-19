Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace OBO

    <HideModuleName>
    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function CreateTermTable(obo As GO_OBO) As Dictionary(Of String, Term)
            Return obo.AsEnumerable.ToDictionary(Function(term) term.id)
        End Function
    End Module
End Namespace