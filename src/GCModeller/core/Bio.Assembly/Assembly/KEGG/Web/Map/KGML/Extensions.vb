Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.KEGG.WebServices.KGML

    <HideModuleName> Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function KOlist(kgml As pathway) As String()
            Return kgml.GetIdlistByType("ortholog")
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function GetIdlistByType(kgml As pathway, type$) As String()
            Return kgml.items _
                .Where(Function(entry) entry.type = type) _
                .Select(Function(entry)
                            Return entry.name _
                                .StringSplit("\s+") _
                                .Select(Function(id)
                                            Return id.GetTagValue(":").Value
                                        End Function)
                        End Function) _
                .IteratesALL _
                .Distinct _
                .ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function CompoundList(kgml As pathway) As String()
            Return kgml.GetIdlistByType("compound")
        End Function
    End Module
End Namespace