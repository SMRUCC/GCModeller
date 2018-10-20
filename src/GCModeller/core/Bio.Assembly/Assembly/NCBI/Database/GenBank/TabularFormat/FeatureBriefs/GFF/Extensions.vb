Imports System.Runtime.CompilerServices

Namespace Assembly.NCBI.GenBank.TabularFormat.GFF

    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Load(path As String) As GFFTable
            Return GFFTable.LoadDocument(path)
        End Function
    End Module
End Namespace