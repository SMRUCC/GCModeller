#Region "Microsoft.VisualBasic::b3f669d9b8b165e5ce0557964f62018a, sub-system\FBA\FBA_DP\FBA\Models\gcFBA\rFBA.vb"

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

    '     Class rFBAMetabolism
    ' 
    '         Properties: GeneFactors
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: (+2 Overloads) __calFactor, __getBounds, __getBoundValue, __getLowerbound, __getUpbound
    '                   __lowBound, __regImpact, __upBound, getConstraint
    ' 
    '         Sub: ResetStat, SetObjectiveGenes, SetRPKM
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.FBA_DP.DocumentFormat
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools
Imports SMRUCC.genomics.Model.SBML
Imports SMRUCC.genomics.Model.SBML.Specifics.MetaCyc

Namespace Models.rFBA

    ''' <summary>
    ''' Model builder for rFBA(FBA system with gene expression regulation) metabolism system 
    ''' </summary>
    ''' <remarks>
    ''' 关于上下限：
    ''' 非酶促过程使用sbml文件里面的默认参数
    ''' 拥有基因的酶促反应过程则根据调控因子的数量来计算出相应的上下限
    ''' 假若调控因子或者酶分子被缺失，则使用<see cref="rFBA_ARGVS.baseFactor"/>本底表达或者酶促过程最低的速率来进行
    ''' </remarks>
    Public Class rFBAMetabolism : Inherits SBML

        ''' <summary>
        ''' {ORF, regulators}, 假设存在这个列表里面的都是受表达调控的，而不存在的则其表达是自由的
        ''' </summary>
        ReadOnly __regulations As Dictionary(Of String, RegulatesFootprints())
        ReadOnly _params As rFBA_ARGVS

        Sub New(model As Level2.XmlFile, footprints As IEnumerable(Of RegulatesFootprints), param As rFBA_ARGVS)
            Call MyBase.New(model, New String() {}, param.forceEnzymeRev)

            __regulations = (From x As RegulatesFootprints
                             In footprints
                             Where Not String.IsNullOrEmpty(x.Regulator)
                             Select x
                             Group x By x.ORF Into Group).ToDictionary(Function(x) x.ORF,
                                                                       Function(x) x.Group.ToArray)
            _params = param
        End Sub

        ''' <summary>
        ''' 基因的突变设置参数
        ''' 
        ''' 0 缺失突变
        ''' 1 正常表达
        ''' 0-1 低表达
        ''' >1 高表达
        ''' </summary>
        ''' <returns>这个列表之中不存在的基因都是正常表达的</returns>
        Public Property GeneFactors As Dictionary(Of String, Double)

        ''' <summary>
        ''' 找出和基因相关的反应过程
        ''' </summary>
        ''' <param name="locus"></param>
        Public Sub SetObjectiveGenes(locus As IEnumerable(Of String))
            Dim LQuery = (From x As Level2.Elements.Reaction
                          In Me._fluxs.Values.AsParallel
                          Let enzymeLocus As String() = New FluxPropReader(x.Notes).GENE_ASSOCIATION
                          Where Not enzymeLocus.IsNullOrEmpty
                          Select x.id,
                              enzymes = New [Set](enzymeLocus))
            Dim inSet As New [Set](locus)
            Dim targets As String() = (From x In LQuery
                                       Where Not (inSet And x.enzymes).IsEmpty
                                       Select x.id).ToArray
            Call Me.SetObjectiveFunc(targets)
        End Sub

        Public Sub ResetStat()
            GeneFactors = New Dictionary(Of String, Double)
            SetObjectiveFunc(New String() {})
        End Sub

        ''' <summary>
        ''' 假若已经通过<see cref="GeneFactors"/>设置了基因的突变的状态，那么笨函数将不会修改已经设置了的突变状态值
        ''' </summary>
        ''' <param name="rpkm"></param>
        ''' <param name="sample"></param>
        Public Sub SetRPKM(rpkm As IEnumerable(Of DESeq2.ExprStats), sample As String)
            If GeneFactors Is Nothing Then
                GeneFactors = New Dictionary(Of String, Double)
            End If

            For Each gene As DESeq2.ExprStats In rpkm
                If GeneFactors.ContainsKey(gene.locus) Then
                    Continue For ' 已经存在了突变数据了，则忽略掉这个基因
                End If

                Dim stat As Boolean = gene.IsActive(sample)
                If Not stat Then ' 活跃状态，则忽略掉，因为默认设置是活跃状态的
                    Call GeneFactors.Add(gene.locus, 0)     ' 在这里是不表达状态，设置系数为零
                Else
                    Call GeneFactors.Add(gene.locus, gene.GetLevel2(sample))
                End If
            Next
        End Sub

        Protected Friend Overrides Function getConstraint(idx As String) As KeyValuePair(Of String, Double)
            Return rFBAMetabolism._constraint
        End Function

        ''' <summary>
        ''' 计算出目标基因对于反应的流量的上下限的影响大小
        ''' </summary>
        ''' <param name="locus"></param>
        ''' <returns></returns>
        Private Function __calFactor(locus As String, trace As List(Of String)) As Double
            If GeneFactors.ContainsKey(locus) Then  ' 已经设置了突变了
                Dim factor As Double = GeneFactors(locus)
                Return factor '  缺失突变，则调控关系失效，不再计算，过量表达，则基因在质粒载体上表达，同样不受调控影响
            End If

            If Not __regulations.ContainsKey(locus) Then Return 1 ' 没有表达调控左右，则自有表达

            Return __regImpact(locus, trace)
        End Function

        ''' <summary>
        ''' 计算出调控的影响大小
        ''' </summary>
        ''' <param name="locus"></param>
        ''' <param name="trace">
        ''' 因为假若两个调控因子相互之间有调控关系，则在递归的过程之中会出现回路造成死循环，所以使用这个参数来避免这个问题
        ''' </param>
        ''' <returns></returns>
        Private Function __regImpact(locus As String, trace As List(Of String)) As Double
            Dim factor As Double = 0

            For Each regulate In __regulations(locus)
                Dim TF As String = regulate.Regulator
                Dim regImpact As Double =
                    regulate.Pcc * _params.PCCw +
                    regulate.sPcc * _params.sPCCw   ' 直接调控为PCC，间接调控为SPCC，直接调控的权重比较大

                Call trace.Add(TF)

                Dim TFRPKM As Double =
                    If(trace.IndexOf(TF) > -1, 1, __calFactor(TF, trace)) ' 递归计算出调控因子的上一层调控网络所施加的影响

                ' 认为负调控的影响比较大，所以在这里进行负调控的加权
                If regImpact < 0 Then
                    regImpact *= _params.SupressImpact
                End If

                regImpact *= TFRPKM  ' 调控相关性 * 调控因子的活跃度 得到本调控关系的影响度
                factor += regImpact
            Next

            Return factor
        End Function

        Private Function __getBounds(getBound As Func(Of Level2.Elements.Reaction, Double)) As Double()
            Dim list As New List(Of Double)

            For Each fluxId As String In Me.fluxColumns
                Dim flux As Level2.Elements.Reaction = Me._fluxs(fluxId)
                Dim enzymes As String() = New FluxPropReader(flux.Notes).GENE_ASSOCIATION
                Dim bound As Double = getBound(flux)

                ' 查找调控信息，并计算影响因素
                ' 假若酶分子受表达调控，则要考虑到调控因子，反之，则只考虑rpkm以及突变的设置

                If enzymes.IsNullOrEmpty Then     ' 非酶促过程，则使用sbml里面的默认设置
                    Call list.Add(bound)
                Else
                    Dim factors As Double() = enzymes.Select(AddressOf __calFactor)
                    factors = (From x In factors Where x > 0 Select x).ToArray ' 负调控，则没有表达，不再计算该酶分子的影响

                    ' bound 就是base
                    Dim sum As Double = (From x As Double In factors Select bound * x).Sum
                    Call list.Add(sum)
                End If
            Next

            Return list.ToArray
        End Function

        Private Function __calFactor(locus As String) As Double
            Return __calFactor(locus, New List(Of String) From {locus})
        End Function

        Private Function __upBound(flux As Level2.Elements.Reaction) As Double
            If Not _params.FluxOverrides Then
                Return flux.UpperBound
            Else
                Dim n As Double = __getBoundValue(flux, _params)
                If n < 0 Then
                    Return 0R
                Else
                    Return n
                End If
            End If
        End Function

        Private Shared Function __getBoundValue(flux As Level2.Elements.Reaction, params As rFBA_ARGVS) As Double
            Dim enzymes As String() = New FluxPropReader(flux.Notes).GENE_ASSOCIATION
            Dim value As Double

            If enzymes.IsNullOrEmpty Then
                value = params.FluxBoundOverrides
            Else
                value = params.baseFactor
            End If
            If Not flux.reversible Then
                value *= params.DirectedFactor
            End If

            Return value
        End Function

        Private Function __lowBound(flux As Level2.Elements.Reaction) As Double
            If Not flux.reversible Then
                Return 0  ' 反应只有一个方向的，则反方向为零流量
            End If

            If Not _params.FluxOverrides Then
                Return flux.LowerBound
            Else
                Dim n As Double = -1 * __getBoundValue(flux, _params)
                If n > 0 Then
                    Return 0R
                Else
                    Return n
                End If
            End If
        End Function

        Protected Friend Overrides Function __getLowerbound() As Double()
            Return __getBounds(AddressOf __lowBound)
        End Function

        Protected Friend Overrides Function __getUpbound() As Double()
            Return __getBounds(AddressOf __upBound)
        End Function
    End Class
End Namespace
