Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Public Class VCellMatrixReader : Implements IDisposable

    ReadOnly s As StreamPack
    ReadOnly fluxList As Dictionary(Of String, String())
    ReadOnly symbolSet As Dictionary(Of String, Dictionary(Of String, String()))
    ReadOnly symbolIndex As New Dictionary(Of String, (compartment As String, [module] As String))
    ReadOnly network As New Dictionary(Of String, FluxEdge)

    Public ReadOnly Property compartmentIds As IReadOnlyCollection(Of String)
    Public ReadOnly Property totalPoints As Integer

    Default Public ReadOnly Property Item(key As String) As FluxEdge
        Get
            Return network(key)
        End Get
    End Property

    Public ReadOnly Property fluxSet As NamedCollection(Of String)()
        Get
            Return fluxList _
                .Select(Function(list)
                            Return New NamedCollection(Of String)(list.Key, list.Value)
                        End Function) _
                .ToArray
        End Get
    End Property

    Private disposedValue As Boolean

    Sub New(filepath As String)
        s = StreamPack.OpenReadOnly(filepath)
        fluxList = s.ReadText("/cellular_flux.json").LoadJSON(Of Dictionary(Of String, String()))
        symbolSet = s.ReadText("/cellular_symbols.json").LoadJSON(Of Dictionary(Of String, Dictionary(Of String, String())))
        compartmentIds = s.ReadText("/compartments.txt") _
            .LineTokens _
            .Where(Function(si) Not si.StringEmpty(, True)) _
            .ToArray
        totalPoints = Strings.Trim(s.ReadText("/ticks.txt")) _
            .Trim(" "c, ASCII.CR, ASCII.LF, ASCII.TAB) _
            .ParseInteger

        Call buildSymbolIndex(symbolSet, symbolIndex)
        Call loadNetwork(s.OpenFile("/cellular_graph.jsonl", FileMode.Open, FileAccess.Read), network)
    End Sub

    Private Shared Sub loadNetwork(s As Stream, network As Dictionary(Of String, FluxEdge))
        Dim reader As New StreamReader(s)
        Dim line As Value(Of String) = ""

        Do While (line = reader.ReadLine) IsNot Nothing
            Dim edge As FluxEdge = CStr(line).LoadJSON(Of FluxEdge)
            Dim id As String = edge.id

            network(id) = edge
        Loop
    End Sub

    Private Shared Sub buildSymbolIndex(symbolSet As Dictionary(Of String, Dictionary(Of String, String())), ByRef symbolIndex As Dictionary(Of String, (compartment As String, [module] As String)))

        For Each compartment In symbolSet
            For Each [module] In compartment.Value
                For Each id As String In [module].Value
                    symbolIndex(id) = (compartment.Key, [module].Key)
                Next
            Next
        Next
    End Sub

    Public Function GetExpression(symbol As String) As Double()
        Dim arg = symbolIndex(symbol)
        Dim path As String = $"/matrix/{arg.compartment}/{arg.[module]}/{symbol}.vec"

        Using buf As New BinaryDataReader(s.OpenBlock(path), byteOrder:=ByteOrder.BigEndian)
            Return buf.ReadDoubles(totalPoints)
        End Using
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call s.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
