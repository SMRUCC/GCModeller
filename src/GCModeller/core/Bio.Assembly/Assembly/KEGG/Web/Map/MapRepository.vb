Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.WebServices

    ''' <summary>
    ''' The repository xml data of kegg <see cref="Map"/>
    ''' </summary>
    Public Class MapRepository : Inherits XmlDataModel
        Implements IRepositoryRead(Of String, MapIndex)
        Implements Enumeration(Of Map)

        <XmlElement("maps")> Public Property Maps As MapIndex()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return table.Values.ToArray
            End Get
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Set(value As MapIndex())
                table = value.ToDictionary(Function(map) map.id)
            End Set
        End Property

        ''' <summary>
        ''' Get by ID
        ''' </summary>
        Dim table As Dictionary(Of String, MapIndex)

        <XmlNamespaceDeclarations()>
        Public xmlns As XmlSerializerNamespaces

        Sub New()
            xmlns = New XmlSerializerNamespaces
            xmlns.Add("map", Map.XmlNamespace)
        End Sub

        Public Iterator Function QueryMapsByMembers(entity As IEnumerable(Of String)) As IEnumerable(Of MapIndex)
            For Each key As String In entity
                For Each map As MapIndex In table.Values
                    If map.index.IndexOf(key) > -1 Then
                        Yield map
                    End If
                Next
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Exists(key As String) As Boolean Implements IRepositoryRead(Of String, MapIndex).Exists
            Return table.ContainsKey(key)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetByKey(key As String) As MapIndex Implements IRepositoryRead(Of String, MapIndex).GetByKey
            Return table(key)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetWhere(clause As Func(Of MapIndex, Boolean)) As IReadOnlyDictionary(Of String, MapIndex) Implements IRepositoryRead(Of String, MapIndex).GetWhere
            Return table.Values.Where(clause).ToDictionary
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetAll() As IReadOnlyDictionary(Of String, MapIndex) Implements IRepositoryRead(Of String, MapIndex).GetAll
            Return New Dictionary(Of String, MapIndex)(table)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="directory">The reference map download directory</param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function BuildRepository(directory As String) As MapRepository
            Return New MapRepository With {
                .Maps = directory _
                    .DoCall(AddressOf ScanMaps) _
                    .Select(AddressOf CreateIndex) _
                    .ToArray
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ScanMaps(directory As String) As IEnumerable(Of Map)
            Return (ls - l - r - "*.XML" <= directory).Select(AddressOf LoadXml(Of Map))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function CreateIndex(map As Map) As MapIndex
            Call map.Name.__DEBUG_ECHO

            Return New MapIndex With {
                .id = map.id,
                .KeyVector = New TermsVector With {
                    .terms = map _
                        .shapes _
                        .Select(Function(a) a.IDVector) _
                        .IteratesALL _
                        .Distinct _
                        .OrderBy(Function(s) s) _
                        .ToArray
                },
                .shapes = map.shapes,
                .Name = map.Name,
                .PathwayImage = map.PathwayImage,
                .URL = map.URL
            }
        End Function

        Public Shared Function GetMapsAuto(repository As String) As IEnumerable(Of Map)
            If repository.DirectoryExists Then
                Return repository.DoCall(AddressOf ScanMaps)
            Else
                Return repository.LoadXml(Of MapRepository)
            End If
        End Function

        ''' <summary>
        ''' 因为<see cref="MapIndex"/>是直接继承至<see cref="Map"/>对象类型的，所以在这里可以直接返回这个序列
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function GenericEnumerator() As IEnumerator(Of Map) Implements Enumeration(Of Map).GenericEnumerator
            For Each index In Maps
                Yield index
            Next
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of Map).GetEnumerator
            Yield GenericEnumerator()
        End Function
    End Class
End Namespace