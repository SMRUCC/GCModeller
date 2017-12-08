Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

''' <summary>
''' KEGG的参考代谢反应模型库，封装了对<see cref="Reaction"/>的对象查询操作
''' </summary>
Public Class ReactionRepository : Implements IRepositoryRead(Of String, Reaction)

    Dim table As Dictionary(Of String, Reaction)

    Public Property MetabolicNetwork As Reaction()
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return table.Values.ToArray
        End Get
        Set(value As Reaction())
            table = value.ToDictionary(Function(r) r.Entry)
        End Set
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Exists(key As String) As Boolean Implements IRepositoryRead(Of String, Reaction).Exists
        Return table.ContainsKey(key)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetByKey(key As String) As Reaction Implements IRepositoryRead(Of String, Reaction).GetByKey
        Return table(key)
    End Function

    Public Function GetByKOMatch(KO As IEnumerable(Of String)) As IEnumerable(Of Reaction)
        With KO.Distinct.Indexing
            Return table _
                .Values _
                .Where(Function(r)
                           Return r.Orthology _
                                   .Select(Function(t) t.Key) _
                                   .Any(Function(id)
                                            Return .IndexOf(id) > -1
                                        End Function)
                       End Function)
        End With
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetWhere(clause As Func(Of Reaction, Boolean)) As IReadOnlyDictionary(Of String, Reaction) Implements IRepositoryRead(Of String, Reaction).GetWhere
        Return table.Values.Where(clause).ToDictionary
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetAll() As IReadOnlyDictionary(Of String, Reaction) Implements IRepositoryRead(Of String, Reaction).GetAll
        Return New Dictionary(Of String, Reaction)(table)
    End Function
End Class
