Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.GraphTheory

Public Module Density2D

    <Extension>
    Public Function Density(Of T As INamedValue)(data As IEnumerable(Of T),
                                                 getX As Func(Of T, Integer),
                                                 getY As Func(Of T, Integer),
                                                 gridSize As Size) As IEnumerable(Of NamedValue(Of Double))

        Return data.Density(Function(d) d.Key, getX, getY, gridSize)
    End Function

    <Extension>
    Public Iterator Function Density(Of T)(data As IEnumerable(Of T),
                                           getName As Func(Of T, String),
                                           getX As Func(Of T, Integer),
                                           getY As Func(Of T, Integer),
                                           gridSize As Size) As IEnumerable(Of NamedValue(Of Double))

        Dim grid2 As Grid(Of T) = Grid(Of T).Create(data, getX, getY)
        Dim q As T()
        Dim A As Double = gridSize.Width * gridSize.Height
        Dim d As Double

        For Each x As T In grid2.EnumerateData
            q = grid2.Query(getX(x), getY(x), gridSize).ToArray
            d = q.Length / A

            Yield New NamedValue(Of Double)(getName(x), d)
        Next
    End Function
End Module
