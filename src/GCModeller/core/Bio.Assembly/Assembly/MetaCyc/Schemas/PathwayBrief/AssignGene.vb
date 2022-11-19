#Region "Microsoft.VisualBasic::e06ee5b9815f1b291fc4ff37896459b5, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\Schemas\PathwayBrief\AssignGene.vb"

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


    ' Code Statistics:

    '   Total Lines: 130
    '    Code Lines: 71
    ' Comment Lines: 40
    '   Blank Lines: 19
    '     File Size: 5.80 KB


    '     Class AssignGene
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: AssignGenes, ConvertId, GetGenes, Performance, Query
    ' 
    '         Sub: (+2 Overloads) Dispose
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.Schema.PathwayBrief

    ''' <summary>
    ''' 将基因与相应的反应过程映射起来
    ''' </summary>
    ''' <remarks></remarks>
    Public Class AssignGene : Implements System.IDisposable

        Dim Proteins As Proteins
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
            Dim EnzAssignedGenes = (From enz As Slots.Enzrxn In MetaCyc.GetEnzrxns Select AssignGenes(enz)).ToArray   '首先先获取所有的酶促反应过程对象所涉及到的基因列表
            Dim LinkList = (From rxn As Slots.Reaction
                            In MetaCyc.GetReactions
                            Where Not rxn.EnzymaticReaction.IsNullOrEmpty
                            Select key = rxn.Identifier,
                                value = Query(rxn.EnzymaticReaction, EnzAssignedGenes)) _
                                  .ToDictionary(Function(x) x.key,
                                                Function(x) x.value)      '获取所有酶促反应对象的基因
            Return LinkList
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="MetaCycData">从<see cref="Performance"></see>函数所得到的结果参数列表</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ConvertId(MetaCycData As Dictionary(Of String, String())) As Dictionary(Of String, String())
            Dim NCBICollection As Dictionary(Of String, String()) = New Dictionary(Of String, String())
            Dim MetaCycGene = MetaCyc.GetGenes

            For Each item As KeyValuePair(Of String, String()) In MetaCycData
                Call NCBICollection.Add(item.Key, (From strid As String In item.Value Select MetaCycGene.Item(strid).Accession1).ToArray)
            Next

            Return NCBICollection
        End Function

        Private Shared Function Query(EnzUniqueIdCollection As List(Of String), EnzAssignedGenes As KeyValuePair(Of String, String())()) As String()
            Dim LQuery = (From link In EnzAssignedGenes
                          Where EnzUniqueIdCollection.IndexOf(link.Key) > -1
                          Select link.Value).ToVector
            Return LQuery
        End Function

        ''' <summary>
        ''' 获取某一个酶促反应中所涉及到的所有基因
        ''' </summary>
        ''' <param name="Enzrxn"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AssignGenes(Enzrxn As Slots.Enzrxn) As KeyValuePair(Of String, String())
            Dim EnzymeProtein As Slots.Protein = Proteins.Item(Enzrxn.Enzyme) '获取酶分子，蛋白质或者蛋白质复合物
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
        Public Shared Function GetGenes(Protein As Slots.Protein, ProteinList As Proteins) As String()
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

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
