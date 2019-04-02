Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.Uniprot.XML

''' <summary>
''' Create genetic components from Uniprot database.
''' </summary>
Public Module UniProtExtensions

    <Extension>
    Public Iterator Function CreateDump(uniprot As IEnumerable(Of entry)) As IEnumerable(Of GeneticNode)
        For Each protein As entry In uniprot.Where(Function(g) Not g.sequence Is Nothing)

        Next
    End Function
End Module
