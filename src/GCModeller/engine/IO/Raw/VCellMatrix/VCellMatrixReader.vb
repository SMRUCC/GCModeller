Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.IO.Raw

Public Class VCellMatrixReader : Implements IDisposable, IStreamContainer

    ReadOnly s As StreamPack
    ReadOnly fluxList As Dictionary(Of String, String())
    ReadOnly symbolSet As Dictionary(Of String, Dictionary(Of String, String()))
    ReadOnly symbolIndex As New Dictionary(Of String, (compartment As String, [module] As String))

    Public ReadOnly Property network As New Dictionary(Of String, FluxEdge)
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

    Sub New(file As Stream)
        Call Me.New(New StreamPack(file, [readonly]:=True))
    End Sub

    Sub New(pack As StreamPack)
        s = pack
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

    Sub New(filepath As String)
        Call Me.New(StreamPack.OpenReadOnly(filepath))
    End Sub

    Public Function GetStream() As StreamPack Implements IStreamContainer.GetStream
        Return s
    End Function

    Public Function ActivityLoads() As Dictionary(Of String, Double())
        Dim folder = s.OpenFolder("/matrix/activityLoads/")
        Dim data As New Dictionary(Of String, Double())

        For Each file In folder.ListFiles(recursive:=False)
            Dim buf = s.OpenBlock(file)
            Dim list As New BinaryDataReader(buf, byteOrder:=ByteOrder.BigEndian)

            Call data.Add(file.fileName.BaseName, list.ReadDoubles(totalPoints))
        Next

        Return data
    End Function

    Public Function ReadMoleculeTree() As Dictionary(Of String, String())
        Dim tree As New Dictionary(Of String, String())

        For Each file In s.OpenFolder("/index/").ListFiles(recursive:=False)
            Dim key As String = file.fileName.BaseName

            If Not key.EndsWith("-Flux") Then
                Call tree.Add(key, Strings.Trim(s.ReadText(file)).Trim(" "c, ASCII.TAB, ASCII.CR, ASCII.LF).LineTokens)
            End If
        Next

        Return tree
    End Function

    Public Iterator Function GetCellularMolecules() As IEnumerable(Of (compartment_id As String, NamedCollection(Of String)()))
        For Each compartment In symbolSet
            Dim moduleSet = compartment.Value _
                .Select(Function(m)
                            Return New NamedCollection(Of String)(m.Key, m.Value)
                        End Function) _
                .ToArray

            Yield (compartment.Key, moduleSet)
        Next
    End Function

    Private Shared Sub loadNetwork(s As Stream, network As Dictionary(Of String, FluxEdge))
        Dim reader As New StreamReader(s)
        Dim line As Value(Of String) = ""

        Do While (line = reader.ReadLine) IsNot Nothing
            If CStr(line) = "" Then
                Continue Do
            End If

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

    Public Function CheckSymbol(symbol As String) As Boolean
        Return symbolIndex.ContainsKey(symbol)
    End Function

    Public Function GetExpression(symbol As String) As Double()
        Dim arg = symbolIndex(symbol)
        Dim path As String = $"/matrix/{arg.compartment}/{arg.[module]}/{symbol}.vec"

        Using buf As New BinaryDataReader(s.OpenBlock(path), byteOrder:=ByteOrder.BigEndian)
            Return buf.ReadDoubles(totalPoints)
        End Using
    End Function

    Public Function FluxExpressionExists(flux_id As String) As Boolean
        Dim path As String = $"/matrix/flux/{flux_id}.vec"
        Dim check As Boolean = s.FileExists(path, True)

        Return check
    End Function

    Public Function GetFluxExpression(symbol As String) As Double()
        Dim path As String = $"/matrix/flux/{symbol}.vec"

        Using buf As New BinaryDataReader(s.OpenBlock(path), byteOrder:=ByteOrder.BigEndian)
            Return buf.ReadDoubles(totalPoints)
        End Using
    End Function

    Public Function GetRegulationExpression(symbol As String, reg As String) As Double()
        Dim path As String = $"/matrix/flux/{reg}/{symbol}.vec"

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
