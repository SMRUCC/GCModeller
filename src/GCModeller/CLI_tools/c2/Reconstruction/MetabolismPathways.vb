#Region "Microsoft.VisualBasic::8a58a826825f11b5d2a04ac479c26a84, CLI_tools\c2\Reconstruction\MetabolismPathways.vb"

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

    '     Class MetabolismPathways
    ' 
    '         Properties: ReconstructedEnzrxn
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: LinkEnzymaticCatalyze, Performance
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ConsoleDevice.STDIO
Imports Microsoft.VisualBasic.Extensions

Namespace Reconstruction

    ''' <summary>
    ''' 在重建蛋白质复合物列表后进行代谢网络的重建
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MetabolismPathways : Inherits Operation

        Dim rctCompounds As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Compounds
        Dim rctReactions As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Reactions
        Dim sbjProteins As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Proteins
        Dim rctEnzrxnList As ReconstructedList(Of LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Enzrxn) =
            New Operation.ReconstructedList(Of LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Enzrxn)
        Dim ProteinEquals As c2.Reconstruction.ObjectEquals.Proteins
        Dim ReactionEquals As c2.Reconstruction.ObjectEquals.Reactions
        Dim rctEnzrxns As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Enzrxns

        Public ReadOnly Property ReconstructedEnzrxn As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Enzrxn()
            Get
                Return rctEnzrxnList.ToTable.ToArray
            End Get
        End Property

        Sub New(Session As Operation.OperationSession, ProteinEquals As c2.Reconstruction.ObjectEquals.Proteins, ReactionEquals As c2.Reconstruction.ObjectEquals.Reactions)
            Call MyBase.New(Session)
            rctCompounds = Session.ReconstructedMetaCyc.GetCompounds
            sbjProteins = Session.Subject.GetProteins
            rctReactions = Session.ReconstructedMetaCyc.GetReactions
            Me.ProteinEquals = ProteinEquals
            Me.ReactionEquals = ReactionEquals
            Me.rctEnzrxns = Session.ReconstructedMetaCyc.GetEnzrxns
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns>返回所重建出来的Reaction对象的数目</returns>
        ''' <remarks>
        ''' 扫描Subject数据库中的Reaction数据表
        ''' 若一个Reaction对象中的All Compounds Exists则，重建出一个Reaction对象
        ''' 执行一个Object.Copy即可完成重建工作
        ''' </remarks>
        Public Overrides Function Performance() As Integer
            Dim n As Integer
            For Each sbjReaction As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Reaction In Subject.GetReactions '重建出基本的代谢网络
                If c2.Reconstruction.ObjectEquals.Reactions.AllCompoundsExists(sbjReaction, rctCompounds) AndAlso Not ReactionEquals.Exists(sbjReaction.Identifier) Then
                    Call rctReactions.Add(sbjReaction)
                    n += 1
                    Call Printf(">> '%s'", sbjReaction.Identifier)
                End If
            Next

            Call Me.rctReactions.Indexing()
            Call LinkEnzymaticCatalyze()
            Call Me.rctEnzrxns.Indexing()

            Return n
        End Function

        ''' <summary>
        ''' 重建出酶分子与代谢反应的催化关系
        ''' </summary>
        ''' <remarks>
        ''' 算法描述
        ''' 
        ''' 扫描Subject数据库中的EnzymaticReaction表，取出酶分子对象，
        ''' 使用对象值比较的方法查询Reconstruction数据库中的蛋白质复合物表
        ''' 假若存在，则进入下一步重建工作
        ''' Subject： 获取该EnzymaticReaction对象的Reaction基类型
        ''' 使用对象值比较的方法查询Reconstructed数据库中的Reaction表
        ''' 若查找到对象，则重建出一个EnzymaticReaction对象
        ''' </remarks>
        Private Function LinkEnzymaticCatalyze() As Integer
            Dim sbjProteins As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Proteins = Subject.GetProteins
            Dim rctProteins As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Proteins = Reconstructed.GetProteins
            Dim n As Integer

            For Each EnzymaticReaction In Subject.GetEnzrxns
                'Dim sbjEnzyme = sbjProteins.Select(UniqueId:=EnzymaticReaction.Enzyme)
                'Dim Temp As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein =
                '    New LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein With {.Components = New List(Of String)}

                'If Not Exists(sbjEnzyme, Temp) Then
                '    Continue For
                'End If

                'Dim Querys = rctProteins.Select(Temp, "COMPONENTS")
                Dim Querys = ProteinEquals.SelectFromSubject(EnzymaticReaction.Enzyme)
                If Querys.Count > 0 Then '没有该蛋白质复合物，则不能够进行连接
                    Dim rctEnzrxn As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Enzrxn =
                        New LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Enzrxn With {.Reaction = New List(Of String)}
                    rctEnzrxn.Enzyme = Querys.First
                    rctEnzrxn.Identifier = EnzymaticReaction.Identifier

                    For Each Reaction In rctReactions.Takes(EnzymaticReaction.Reaction) '遍历该酶分子对象所催化的酶促反应
                        Dim Result = rctReactions.Select(Reaction, "LEFT", "RIGHT")
                        If Result.Count > 0 Then
                            Call rctEnzrxn.Reaction.Add(Result.First.Identifier)
                        End If
                    Next

                    If rctEnzrxn.Reaction.Count > 0 Then
                        n += 1
                        Call Printf("ENZRXN >> %s", rctEnzrxn.Identifier)
                        Call rctEnzrxns.Add(rctEnzrxn)
                        Call rctEnzrxnList.Add(EnzymaticReaction.Identifier, rctEnzrxn)
                    End If
                End If
            Next

            Call Printf("Linked %d enzyme and reactions!", n)

            Return n
        End Function
    End Class
End Namespace
