Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps.DataFrameColumnAttribute
Imports Microsoft.VisualBasic.Linq

Namespace TAB

    Public Module Parser

        <Extension>
        Public Iterator Function LoadMItab(Of T)(path As String) As IEnumerable(Of T)
            Dim schema = LoadMapping(Of T)(mapsAll:=True)
            Dim header As String() = path.ReadFirstLine.Split(Text.ASCII.TAB)
            Dim index As Dictionary(Of String, Integer) = header _
                .SeqIterator _
                .ToDictionary(Function(x) x.obj,
                              Function(x) x.i)

            For Each line As String In path.IterateAllLines.Skip(1)
                Dim tokens As String() = line.Split(Text.ASCII.TAB)
                Dim x As T = Activator.CreateInstance(Of T)

                For Each p In schema.Values
                    Call p.SetValue(x, tokens(index(p.Field.Name)))
                Next

                Yield x
            Next
        End Function
    End Module
End Namespace