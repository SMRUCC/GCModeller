#Region "Microsoft.VisualBasic::e5edd192e8a6fc639734f9e4a623c8cf, CLI_tools\c2\Reconstruction\RegulationNetwork.vb"

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

    '     Class RegulationNetwork
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Performance
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.ConsoleDevice.STDIO

Namespace Reconstruction

    ''' <summary>
    ''' 对调控网络的重建过程
    ''' </summary>
    ''' <remarks></remarks>
    Friend Class RegulationNetwork : Inherits Operation

        ''' <summary>
        ''' Subject数据库之中的Regulations表
        ''' </summary>
        ''' <remarks></remarks>
        Dim sbjRegulations As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Regulations
        ''' <summary>
        ''' Subject数据库之中的Proteins表
        ''' </summary>
        ''' <remarks></remarks>
        Dim sbjProteins As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Proteins
        Dim rctCompounds As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Compounds
        Dim _ProteinCPLXEquals As c2.Reconstruction.ObjectEquals.Proteins
        Dim _ReactionsEquals As c2.Reconstruction.ObjectEquals.Reactions
        Dim _PromoterEquals As c2.Reconstruction.ObjectEquals.Promoters
        Dim rctRegulations As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Regulations

        Friend ReconstructList As Operation.ReconstructedList(Of LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Regulation) =
            New Operation.ReconstructedList(Of LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Regulation)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Session"></param>
        ''' <param name="EqualsSession">判断两个蛋白质对象在Subject和Reconstructed数据库之间等价的函数</param>
        ''' <remarks></remarks>
        Sub New(Session As OperationSession, EqualsSession As ObjectEquals.Session)
            Call MyBase.New(Session)
            Me.sbjRegulations = Session.Subject.GetRegulations
            Me.sbjProteins = Session.Subject.GetProteins
            Me.rctCompounds = Session.ReconstructedMetaCyc.GetCompounds
            Me._ProteinCPLXEquals = EqualsSession.ProteinEquals
            Me._ReactionsEquals = EqualsSession.ReactionEquals
            Me._PromoterEquals = EqualsSession.PromoterEquals
            Me.rctRegulations = MyBase.Reconstructed.GetRegulations
        End Sub

        ''' <summary>
        ''' 遍历Subject数据库中的Regulation表，根据Entity类型的不同来重建目标网络
        ''' </summary>
        ''' <remarks>
        ''' 扫描Subject数据库中的Regulation数据表
        ''' 对于每一个Regulation对象，从Regulator属性值在rct数据库之中寻找出等价的对象
        ''' (1) compound对象: UniqueId属性值相等即可
        ''' (2) protein对象：Components列表相等
        ''' 
        ''' 对于每一个Regulation对象，从RegulationEntity属性之中查找出Reconstructed数据库之中等价的目标对象
        ''' (1) Reaction对象：值等价
        ''' (2) Enzyme-Activity对象: Protein等价
        ''' (3) TransUnit对象：Promoter对象等价
        ''' </remarks>
        Public Overrides Function Performance() As Integer
            Dim sbjReactions = MyBase.Subject.GetReactions
            Dim sbjPromoters = MyBase.Subject.GetPromoters

            Dim rctProteins = MyBase.Reconstructed.GetProteins
            Dim RegulatorType As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables

            For Each Regulation As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Regulation In MyBase.Subject.GetRegulations
                Dim RegulatorId As String = Regulation.Regulator

                '第一步，先查询出在两个数据库之间等价的Regulator对象，在查询出等价的RegulationEntity对象
                If rctCompounds.IndexOf(UniqueId:=RegulatorId) = -1 Then    '假若目标对象不存在于Reconstructed数据库之中，则目标调控因子对象可能是一种蛋白质, 否则目标对象是一种小分子化合物，不做任何动作
                    Dim rctProteinList = _ProteinCPLXEquals.SelectFromSubject(RegulatorId) '首先假设目标对象为一个蛋白质，则先从Subject数据库之中查找出目标对象
                    If rctProteinList.IsNullOrEmpty Then '目标对象不是一种蛋白质或者没有查找到等价的对象
                        Call Printf("EXCEPTION::Unknown -> %s", RegulatorId)
                        Continue For '未知，则忽略本对象
                    Else
                        '目标对象是一种蛋白质，则查询出目标蛋白质在Reconstructed数据库中的等价的蛋白质对象
                        '在两个数据库之间有相等价的蛋白质对象
                        RegulatorId = rctProteinList.First     '则复制替换
                        RegulatorType = LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.proteins
                    End If
                Else
                    RegulatorType = LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.compounds
                End If

                '这里是第二步，查询出等价的被调控的对象
                '
                '先查询出Subject数据库中的相应的对象：依次查询Subject数据库中的Reaction，Proteins和Promoters列表
                '再将所查询出的Subject数据库对象转换为与Reconstructed数据库之中的想等价的对象
                '假若可以查询到相应的等价对象，则该等价对象为目标调控对象，反之，则无法构建出一个调控对象
                Dim RegulatedEntity As String = Regulation.RegulatedEntity
                If rctCompounds.IndexOf(UniqueId:=RegulatedEntity) = -1 Then    '假若目标对象不存在于Reconstructed数据库之中，则目标调控因子对象可能是一种蛋白质, 否则目标对象是一种小分子化合物，不做任何动作
                    Dim rctObjectList = _ProteinCPLXEquals.SelectFromSubject(RegulatedEntity)    '首先假设目标对象为一个蛋白质，则先从Subject数据库之中查找出目标对象

                    If rctObjectList.IsNullOrEmpty Then '目标对象不是一种蛋白质，则可能为一个反应或者Promoter
                        rctObjectList = _ReactionsEquals.SelectFromSubject(RegulatedEntity)

                        If rctObjectList.IsNullOrEmpty Then '目标对象不是一种反应过程，则可能是一个启动子或者转录单元
                            rctObjectList = Me._PromoterEquals.SelectFromSubject(RegulatedEntity)
                            If rctObjectList.IsNullOrEmpty Then
                                Continue For '再也不知道怎么办了
                            Else
                                '在两个数据库之间有相等价的Promoter对象
                                RegulatedEntity = rctObjectList.First     '则复制替换
                            End If
                        Else
                            '在两个数据库之间有相等价的Reaction对象
                            RegulatedEntity = rctObjectList.First     '则复制替换
                        End If
                    Else '目标对象是一种蛋白质，则查询出目标蛋白质在Reconstructed数据库中的等价的蛋白质对象
                        '在两个数据库之间有相等价的蛋白质对象
                        RegulatedEntity = rctObjectList.First     '则复制替换
                    End If
                End If

                '第三部，当以上的两个对象都被查询出来的时候，就可以构建出一个Regulation对象了，执行Object.Copy
                Dim rctRegulation As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Regulation = New LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Regulation
                rctRegulation.Regulator = RegulatorId
                rctRegulation.RegulatedEntity = RegulatedEntity
                rctRegulation.Types = Regulation.Types
                rctRegulation.Identifier = String.Format("REG-{0}-{1}", RegulatorId, RegulatedEntity)
                rctRegulation.Mode = Regulation.Mode

                If RegulatorType = LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.proteins Then
                    rctProteins.Select(RegulatorId).Regulates.Add(rctRegulation.Identifier)
                Else
                    rctCompounds.Select(RegulatorId).Regulates.Add(rctRegulation.Identifier)
                End If

                Call Printf("REGULATION >> %s", rctRegulation.Identifier)
                Call rctRegulations.Add(rctRegulation)

                Call ReconstructList.Add(Regulation.Identifier, rctRegulation)
            Next

            Call rctRegulations.Indexing()

            Return 0
        End Function
    End Class
End Namespace
