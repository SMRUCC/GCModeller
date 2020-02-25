Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

''' <summary>
''' Sample names helper
''' </summary>
Public Module SampleNames

    ''' <summary>
    ''' Guess all possible sample groups from the given name string collection.
    ''' </summary>
    ''' <param name="allSampleNames"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function GuessPossibleGroups(allSampleNames As IEnumerable(Of String)) As IEnumerable(Of NamedCollection(Of String))

    End Function
End Module
