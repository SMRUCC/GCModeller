#Region "Microsoft.VisualBasic::da61c738dedad1b8304a324a2f48809c, Bio.Repository\KEGG\ReactionRepository\ReactionRepository.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.



' /********************************************************************************/

' Summaries:

' Class ReactionRepository
' 
'     Properties: MetabolicNetwork
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: Enzymetic, Exists, GetAll, GetByKey, GetByKOMatch
'               GetWhere, ScanModel
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Terminal.ProgressBar
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.Annotation

''' <summary>
''' KEGG的参考代谢反应模型库，封装了对<see cref="Reaction"/>的对象查询操作
''' </summary>
Public Class ReactionRepository : Inherits XmlDataModel
    Implements IRepositoryRead(Of String, Reaction)

    ''' <summary>
    ''' ``{rxnID => reaction}``
    ''' </summary>
    Dim table As Dictionary(Of String, Reaction)
    Dim compoundIndex As Dictionary(Of String, String())

    <XmlNamespaceDeclarations()>
    Public xmlns As New XmlSerializerNamespaces

    Sub New()
        Call xmlns.Add("KEGG", Reaction.Xmlns)
    End Sub

    ''' <summary>
    ''' 这个Repository之中的所有的代谢过程的数据都在这里了
    ''' </summary>
    ''' <returns></returns>
    Public Property metabolicNetwork As Reaction()
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return table.Values.ToArray
        End Get
        Set(value As Reaction())
            If value.IsNullOrEmpty Then
                table = New Dictionary(Of String, Reaction)
            Else
                table = value.ToDictionary(Function(r) r.ID)
            End If
        End Set
    End Property

    ''' <summary>
    ''' ``{compound_id => arrayOf(reactionId)}``
    ''' </summary>
    ''' <returns></returns>
    Public Function GetCompoundIndex() As Dictionary(Of String, String())
        If compoundIndex.IsNullOrEmpty Then
            compoundIndex = table.Values _
                .Select(Function(rxn)
                            Return rxn.ReactionModel _
                                .GetMetabolites _
                                .Select(Function(cpd) cpd.ID) _
                                .Select(Function(id)
                                            Return (cpd:=id, rxn:=rxn.ID)
                                        End Function)
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(t) t.cpd) _
                .ToDictionary(Function(cpd) cpd.Key,
                              Function(g)
                                  Return g.Select(Function(t) t.rxn) _
                                      .Distinct _
                                      .ToArray
                              End Function)
        End If

        Return compoundIndex
    End Function

    Public Function Subset(ECNumbers As ECNumber()) As ReactionRepository
        Dim getReactions = Iterator Function() As IEnumerable(Of Reaction)
                               For Each id As ECNumber In ECNumbers
                                   For Each reaction As Reaction In table.Values
                                       If reaction.Enzyme.Any(Function(ecId)
                                                                  Return id.Contains(ecId) OrElse ECNumber.ValueParser(ecId).Contains(id)
                                                              End Function) Then
                                           Yield reaction
                                       End If
                                   Next
                               Next
                           End Function

        Return New ReactionRepository With {
            .table = getReactions() _
                .GroupBy(Function(r) r.ID) _
                .ToDictionary(Function(r) r.Key,
                              Function(g)
                                  Return g.First
                              End Function)
        }
    End Function

    ''' <summary>
    ''' KEGG代谢反应模型数据之中还包含有非酶促过程
    ''' 使用这个函数将会筛选出所有的酶促过程
    ''' </summary>
    ''' <returns></returns>
    Public Function Enzymetic() As ReactionRepository
        Return New ReactionRepository With {
            .metabolicNetwork = table _
                .Values _
                .Where(Function(r)
                           Return Not r.Orthology.Terms.IsNullOrEmpty
                       End Function) _
                .ToArray
        }
    End Function

    ''' <summary>
    ''' Test if target reaction model is exists in current data repository or not. 
    ''' </summary>
    ''' <param name="rxnIdkey"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Exists(rxnIdkey As String) As Boolean Implements IRepositoryRead(Of String, Reaction).Exists
        Return table.ContainsKey(rxnIdkey)
    End Function

    ''' <summary>
    ''' Get a reaction model data by a given reaction id as index key.
    ''' </summary>
    ''' <param name="rxnIdkey"></param>
    ''' <returns>
    ''' If the key is not exists in current repository, then nothing will be returned.
    ''' </returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetByKey(rxnIdkey As String) As Reaction Implements IRepositoryRead(Of String, Reaction).GetByKey
        Return table.TryGetValue(rxnIdkey)
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

    Public Shared Function LoadAuto(handle As String) As ReactionRepository
        If handle.ExtensionSuffix.TextEquals("xml") AndAlso handle.FileExists Then
            Return handle.LoadXml(Of ReactionRepository)
        Else
            Return ScanModel(directory:=handle)
        End If
    End Function

    Public Shared Function ScanModel(directory As String) As ReactionRepository
        Dim list As New Dictionary(Of String, Reaction)
        Dim busy As New SwayBar

        For Each Xml As String In ls - l - r - "*.Xml" <= directory
            With Reaction.LoadXml(handle:=Xml)
                If Not list.ContainsKey(.ID) Then
                    list(.ID) = .ByRef
                    busy.Step()
                End If
            End With
        Next

        Return New ReactionRepository With {
            .table = list
        }
    End Function
End Class
