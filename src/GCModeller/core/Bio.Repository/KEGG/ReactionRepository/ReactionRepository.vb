Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

''' <summary>
''' KEGG的参考代谢反应模型库，封装了对<see cref="Reaction"/>的对象查询操作
''' </summary>
Public Class ReactionRepository : Implements IRepositoryRead(Of String, Reaction)

    Dim table As Dictionary(Of String, Reaction)

    <XmlNamespaceDeclarations()>
    Public xmlns As New XmlSerializerNamespaces

    Sub New()
        Call xmlns.Add("KEGG", Reaction.Xmlns)
    End Sub

    Public Property MetabolicNetwork As Reaction()
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return table.Values.ToArray
        End Get
        Set(value As Reaction())
            table = value.ToDictionary(Function(r) r.Entry)
        End Set
    End Property

    ''' <summary>
    ''' KEGG代谢反应模型数据之中还包含有非酶促过程
    ''' 使用这个函数将会筛选出所有的酶促过程
    ''' </summary>
    ''' <returns></returns>
    Public Function Enzymetic() As ReactionRepository
        Return New ReactionRepository With {
            .MetabolicNetwork = table _
                .Values _
                .Where(Function(r)
                           Return Not r.Orthology.Terms.IsNullOrEmpty
                       End Function) _
                .ToArray
        }
    End Function

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
                                   .Terms _
                                   .Select(Function(t) t.name) _
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

    Public Shared Function ScanModel(directory As String) As ReactionRepository
        Dim list As New Dictionary(Of String, Reaction)

        For Each Xml As String In ls - l - r - "*.Xml" <= directory
            With Xml.LoadXml(Of Reaction)(
                    preprocess:=Function(text)
                                    Return text.Replace("&#x8;", "")
                                End Function
                )
                If Not list.ContainsKey(.Entry) Then
                    list(.Entry) = .ref
                End If
            End With
        Next

        Return New ReactionRepository With {
            .table = list
        }
    End Function
End Class
