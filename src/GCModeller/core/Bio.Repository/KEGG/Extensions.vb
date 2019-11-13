
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

<HideModuleName>
Public Module Extensions

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetCompoundNames(repository As String) As Dictionary(Of String, String)
        Return CompoundRepository.ScanRepository(repository, False) _
            .GroupBy(Function(c) c.entry) _
            .ToDictionary(Function(cpd) cpd.Key,
                          Function(cpd)
                              Return cpd.First _
                                  .commonNames _
                                  .FirstOrDefault Or cpd.Key.AsDefault
                          End Function)
    End Function
End Module
