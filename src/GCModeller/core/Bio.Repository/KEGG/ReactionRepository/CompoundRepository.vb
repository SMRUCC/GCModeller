Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Public Class CompoundRepository : Implements IRepositoryRead(Of String, CompoundIndex)

    Public Property Compounds As CompoundIndex()
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return compoundTable.Values.ToArray
        End Get
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Set(value As CompoundIndex())
            compoundTable = value.ToDictionary(Function(c) c.ID)
        End Set
    End Property

    Dim compoundTable As Dictionary(Of String, CompoundIndex)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Exists(key As String) As Boolean Implements IRepositoryRead(Of String, CompoundIndex).Exists
        Return compoundTable.ContainsKey(key)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetByKey(key As String) As CompoundIndex Implements IRepositoryRead(Of String, CompoundIndex).GetByKey
        If compoundTable.ContainsKey(key) Then
            Return compoundTable(key)
        Else
            Return Nothing
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetWhere(clause As Func(Of CompoundIndex, Boolean)) As IReadOnlyDictionary(Of String, CompoundIndex) Implements IRepositoryRead(Of String, CompoundIndex).GetWhere
        Return compoundTable.Values.Where(clause).ToDictionary(Function(c) c.ID)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetAll() As IReadOnlyDictionary(Of String, CompoundIndex) Implements IRepositoryRead(Of String, CompoundIndex).GetAll
        Return New Dictionary(Of String, CompoundIndex)(compoundTable)
    End Function
End Class

Public Class CompoundIndex
    Implements IKeyIndex(Of Compound)
    Implements INamedValue

    <XmlAttribute>
    Public Property ID As String Implements IKeyedEntity(Of String).Key
    Public Property Entity As Compound Implements IKeyIndex(Of Compound).Entity

    <XmlIgnore>
    Public ReadOnly Property Index As Index(Of String) Implements IKeyIndex(Of Compound).Index

    Dim terms As OrthologyTerms

    Public Property DbTerms As OrthologyTerms
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return terms
        End Get
        Set(value As OrthologyTerms)
            terms = value
            _Index = terms _
                .Terms _
                .Select(Function(term) term.value) _
                .Indexing
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return ID
    End Function
End Class