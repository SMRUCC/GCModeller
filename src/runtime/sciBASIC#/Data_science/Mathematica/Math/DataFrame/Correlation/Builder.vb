Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Module Builder

    <Extension>
    Public Function MatrixBuilder(Of DataSet As {New, INamedValue, DynamicPropertyBase(Of Double)})(data As IEnumerable(Of DataSet), eval As Func(Of Double(), Double(), Double)) As DistanceMatrix
        Dim allData = data.ToArray
        Dim names = allData.PropertyNames
        Dim keys As String() = allData.Keys
        Dim matrix As Double()() = allData _
            .SeqIterator _
            .AsParallel _
            .Select(Function(d)
                        Dim vec As New List(Of Double)
                        Dim sample = d.value.Properties
                        Dim x As Double() = sample.Takes(names).ToArray
                        Dim y As Double()

                        For Each row As DataSet In allData
                            y = names.Select(Function(key) row(key)).ToArray
                            vec += eval(x, y)
                        Next

                        Return (d.i, vec.ToArray)
                    End Function) _
            .OrderBy(Function(d) d.i) _
            .Select(Function(d) d.Item2) _
            .ToArray

        Return New DistanceMatrix(keys.Indexing, matrix)
    End Function
End Module
