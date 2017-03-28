Imports System.Runtime.CompilerServices

Namespace Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv

    Public Module Extensions

        ''' <summary>
        ''' Listing all types in a chebi entity object.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Types(Of T As Tables.Entity)(source As IEnumerable(Of T)) As String()
            Return source.Select(Function(o) o.TYPE).Distinct.ToArray
        End Function
    End Module
End Namespace