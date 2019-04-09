Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace VFDB

    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function BuildVFDIndex(data As IEnumerable(Of FastaHeader)) As Dictionary(Of String, Index(Of String))
            Return data _
                .GroupBy(Function(a) a.organism) _
                .ToDictionary(Function(org) org.Key,
                              Function(genes)
                                  Return genes _
                                     .Select(Function(g) g.geneName) _
                                     .Distinct _
                                     .Indexing
                              End Function)
        End Function
    End Module
End Namespace