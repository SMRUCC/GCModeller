Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.foundation.OBO_Foundry.Tree

<HideModuleName> Public Module Extensions

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function TermIndex(terms As IEnumerable(Of GenericTree)) As Index(Of String)
        Return terms.Select(Function(node) node.ID) _
            .Distinct _
            .ToArray
    End Function
End Module
