#Region "Microsoft.VisualBasic::771f06acd1126ffb469180eac87aac48, Bio.Repository\KEGG\ReactionRepository\ReactionLink.vb"

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

    '     Class ReactionLink
    ' 
    '         Function: FromReactions, (+2 Overloads) FromRepository, PopulateAnyLinks, PopulateConversionLinks, populateLinks
    '                   tuples
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Namespace KEGG.Metabolism

    Public Class ReactionLink : Inherits XmlDataModel

        ''' <summary>
        ''' 只要两种代谢物在同一个代谢过程之中就会建立一个连接
        ''' </summary>
        Dim compoundLinks As Dictionary(Of String, Dictionary(Of String, Reaction()))
        ''' <summary>
        ''' 只有当两种代谢物一个为产物，另外一个为底物，或者反过来的关系的时候才会建立一个连接
        ''' </summary>
        Dim conversionLinks As Dictionary(Of String, Dictionary(Of String, Reaction()))

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function PopulateAnyLinks(cpd1$, cpd2$) As Reaction()
            Return populateLinks(cpd1, cpd2, compoundLinks)
        End Function

        ''' <summary>
        ''' 因为在构造函数之中已经将左右代谢物都合并在一起了
        ''' 所以在这里只需要按照一个方向查找即可，不需要查找两个方向了
        ''' </summary>
        ''' <param name="cpd1$"></param>
        ''' <param name="cpd2$"></param>
        ''' <param name="links"></param>
        ''' <returns></returns>
        Private Shared Function populateLinks(cpd1$, cpd2$, links As Dictionary(Of String, Dictionary(Of String, Reaction()))) As Reaction()
            If Not links.ContainsKey(cpd1) Then
                Return {}
            End If

            With links(cpd1)
                If .ContainsKey(cpd2) Then
                    Return .Item(cpd2)
                Else
                    Return {}
                End If
            End With
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function PopulateConversionLinks(cpd1$, cpd2$) As Reaction()
            Return populateLinks(cpd1, cpd2, conversionLinks)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function FromRepository(repo As ReactionRepository) As ReactionLink
            Return FromReactions(repo.metabolicNetwork)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function FromRepository(repo As String) As ReactionLink
            Return FromReactions((ls - l - r - "*.Xml" <= repo) _
                   .Select(Function(path)
                               Return path.LoadXml(Of Reaction)(
                                   preprocess:=Function(text)
                                                   Return text.Replace("&#x8;", "")
                                               End Function)
                           End Function))
        End Function

        Public Shared Function FromReactions(repo As IEnumerable(Of Reaction)) As ReactionLink
            Dim array As Reaction() = repo.ToArray
            ' compoundLinks
            Dim list = array.SafeQuery _
                            .Select(Function(r)
                                        Dim model = r.ReactionModel
                                        Dim left = tuples(r, Function(null) model.Reactants).AsList
                                        Dim right = tuples(r, Function(null) model.Products)

                                        Return left + right
                                    End Function) _
                            .IteratesALL _
                            .GroupBy(Function(c) c.Name) _
                            .ToDictionary(Function(g) g.Key,
                                          Function(g)
                                              Dim rights = g.Select(Function(r)
                                                                        Dim reaction = r.Value.ReactionModel
                                                                        Return tuples(r.Value, Function(null) reaction.Products).AsList +
                                                                               tuples(r.Value, Function(null) reaction.Reactants)
                                                                    End Function) _
                                                            .IteratesALL _
                                                            .GroupBy(Function(c) c.Name) _
                                                            .ToArray
                                              Return rights.ToDictionary(Function(c) c.Key,
                                                                         Function(group)
                                                                             Return group.GroupBy(Function(r) r.Value.ID) _
                                                                                         .Select(Function(rg) rg.First.Value) _
                                                                                         .ToArray
                                                                         End Function)
                                          End Function)
            Dim conversions As New Dictionary(Of String, Dictionary(Of String, Reaction()))

            For Each compound In list
                Dim cId = compound.Key
                Dim links As Dictionary(Of String, Reaction()) = compound.Value
                Dim reactions = links.Select(Function(tuple As KeyValuePair(Of String, Reaction()))
                                                 Dim cID2$ = tuple.Key
                                                 Dim conversion As Reaction() = tuple _
                                                     .Value _
                                                     .Where(Function(r As Reaction)
                                                                Dim model = r.ReactionModel

                                                                If Not model.Products.Where(Function(c) c.ID = cId).FirstOrDefault Is Nothing AndAlso
                                                                   Not model.Reactants.Where(Function(c) c.ID = cID2).FirstOrDefault Is Nothing Then
                                                                    Return True
                                                                ElseIf Not model.Products.Where(Function(c) c.ID = cID2).FirstOrDefault Is Nothing AndAlso
                                                                       Not model.Reactants.Where(Function(c) c.ID = cId).FirstOrDefault Is Nothing Then
                                                                    Return True
                                                                Else
                                                                    Return False
                                                                End If
                                                            End Function) _
                                                     .ToArray

                                                 Return (cID2, conversion)
                                             End Function) _
                                     .Where(Function(t)
                                                Return Not t.Item2.IsNullOrEmpty
                                            End Function) _
                                     .ToDictionary(Function(t) t.Item1,
                                                   Function(t) t.Item2)

                If reactions.Count > 0 Then
                    conversions.Add(cId, reactions)
                End If
            Next

            Return New ReactionLink With {
                .compoundLinks = list,
                .conversionLinks = conversions
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function tuples(r As Reaction, getIDs As Func(Of Reaction, CompoundSpecieReference())) As IEnumerable(Of NamedValue(Of Reaction))
            Return getIDs(r).Keys.Select(Function(id) New NamedValue(Of Reaction)(id, r))
        End Function
    End Class
End Namespace
