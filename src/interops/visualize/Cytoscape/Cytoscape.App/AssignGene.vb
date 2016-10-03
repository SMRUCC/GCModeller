#Region "Microsoft.VisualBasic::fc51d5d543ecf233f0a9cf348775a113, ..\interops\visualize\Cytoscape\Cytoscape.App\AssignGene.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports SMRUCC.genomics.Assembly.MetaCyc.File
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports Microsoft.VisualBasic

''' <summary>
''' 将基因与相应的反应过程映射起来
''' </summary>
''' <remarks></remarks>
Public Class AssignGene

    Dim Proteins As DataFiles.Proteins
    Dim MetaCyc As DatabaseLoadder

    Sub New(MetaCyc As DatabaseLoadder)
        Proteins = MetaCyc.GetProteins
        Me.MetaCyc = MetaCyc
    End Sub

    ''' <summary>
    ''' String() => {Reaction, Associated-Genes}
    ''' </summary>
    ''' <returns>{Reaction, Associated-Genes}</returns>
    ''' <remarks></remarks>
    Public Function Performance() As Dictionary(Of String, String())
        Dim EnzAssignedGenes = (From enz In MetaCyc.GetEnzrxns Select AssignGenes(enz)).ToArray   '首先先获取所有的酶促反应过程对象所涉及到的基因列表
        Dim LinkList As KeyValuePair(Of String, String())() = (From rxn In MetaCyc.GetReactions
                                                               Where Not rxn.EnzymaticReaction.IsNullOrEmpty
                                                               Select New KeyValuePair(Of String, String())(key:=rxn.Identifier, value:=Query(rxn.EnzymaticReaction, EnzAssignedGenes))).ToArray      '获取所有酶促反应对象的基因
        Dim dict As Dictionary(Of String, String()) = New Dictionary(Of String, String())
        For Each link In LinkList
            Call dict.Add(link.Key, link.Value)
        Next

        Return dict
    End Function

    Private Shared Function Query(EnzUniqueIdCollection As List(Of String), EnzAssignedGenes As KeyValuePair(Of String, String())()) As String()
        Dim List As List(Of String) = New List(Of String)
        Dim LQuery = (From link In EnzAssignedGenes Where EnzUniqueIdCollection.IndexOf(link.Key) > -1 Select link.Value).ToArray
        For Each link2 In LQuery
            Call List.AddRange(link2)
        Next
        Return List.ToArray
    End Function

    ''' <summary>
    ''' 获取某一个酶促反应中所涉及到的所有基因
    ''' </summary>
    ''' <param name="Enzrxn"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AssignGenes(Enzrxn As DataFiles.Slots.Enzrxn) As KeyValuePair(Of String, String())
        Dim EnzymeProtein As DataFiles.Slots.Protein = Proteins.Item(Enzrxn.Enzyme) '获取酶分子，蛋白质或者蛋白质复合物
        Dim List = New KeyValuePair(Of String, String())(key:=Enzrxn.Identifier, value:=GetGenes(EnzymeProtein, Proteins))
        Return List
    End Function

    ''' <summary>
    ''' 递归的查找某一个蛋白质复合物的基因
    ''' </summary>
    ''' <param name="Protein"></param>
    ''' <param name="ProteinList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetGenes(Protein As DataFiles.Slots.Protein, ProteinList As DataFiles.Proteins) As String()
        Dim GeneList As List(Of String) = New List(Of String)

        If Protein.Components.IsNullOrEmpty() Then '蛋白质单体，直接获取基因并返回数据
            Return New String() {Protein.Gene}
        Else '蛋白质复合物，则做递归搜索，查询出所有的蛋白质蛋白组件，然后获取基因对象
            For Each ComponentId As String In Protein.Components
                Dim Index As Integer = ProteinList.IndexOf(ComponentId)
                If Index > -1 Then
                    Call GeneList.AddRange(GetGenes(ProteinList(Index), ProteinList))
                End If
            Next
        End If

        Return GeneList.ToArray
    End Function
End Class
