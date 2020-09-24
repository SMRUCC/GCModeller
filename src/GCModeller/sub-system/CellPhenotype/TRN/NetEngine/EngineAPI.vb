#Region "Microsoft.VisualBasic::10d1083a0de9d2d5f13a6b832d91cfd8, sub-system\CellPhenotype\TRN\NetEngine\EngineAPI.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'#Region "Microsoft.VisualBasic::69c0309a4a8780b753a4ac99856333ae, sub-system\CellPhenotype\TRN\NetEngine\EngineAPI.vb"

'    ' Author:
'    ' 
'    '       asuka (amethyst.asuka@gcmodeller.org)
'    '       xie (genetics@smrucc.org)
'    '       xieguigang (xie.guigang@live.com)
'    ' 
'    ' Copyright (c) 2018 GPL3 Licensed
'    ' 
'    ' 
'    ' GNU GENERAL PUBLIC LICENSE (GPL3)
'    ' 
'    ' 
'    ' This program is free software: you can redistribute it and/or modify
'    ' it under the terms of the GNU General Public License as published by
'    ' the Free Software Foundation, either version 3 of the License, or
'    ' (at your option) any later version.
'    ' 
'    ' This program is distributed in the hope that it will be useful,
'    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
'    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    ' GNU General Public License for more details.
'    ' 
'    ' You should have received a copy of the GNU General Public License
'    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



'    ' /********************************************************************************/

'    ' Summaries:

'    '     Module EngineAPI
'    ' 
'    '         Function: (+2 Overloads) CreateObject
'    '         Class Footprint_INIT
'    ' 
'    '             Properties: data, ORF
'    ' 
'    '             Function: ToString
'    ' 
'    ' 
'    ' 
'    ' 
'    ' /********************************************************************************/

'#End Region

'Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
'Imports SMRUCC.genomics.Analysis.CellPhenotype.TRN.KineticsModel
'Imports SMRUCC.genomics.Analysis.CellPhenotype.TRN.KineticsModel.Regulators
'Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver
'Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.DataStorage.FileModel
'Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Components
'Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream
'Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO
'Imports SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat

'Namespace TRN

'    Public Module EngineAPI

'        Public Function CreateObject(footprints As IEnumerable(Of RegulatesFootprints),
'                                     initStat As IEnumerable(Of NetworkInput),
'                                     cellularNET As XmlresxLoader) As BinaryNetwork
'            Dim LengthMapping = (From trans As Transcript
'                                 In cellularNET.Transcripts
'                                 Select trans
'                                 Group By trans.UniqueId Into Group) _
'                                    .ToDictionary(Function(obj) obj.UniqueId.Split("."c).First,
'                                                  Function(obj) obj.Group.First.TranscriptCompositionVectors.Sum)
'            Dim RawModel = CreateObject(footprints, initStat, LengthMapping)
'            Dim GeneDict = RawModel.DynamicsExprs.ToDictionary(Function(item) item.Identifier)
'            Dim TCS_RR_CACHE = (From id As String
'                                In cellularNET.MisT2.MajorModules.First.TwoComponent.GetRR
'                                Where GeneDict.ContainsKey(id)
'                                Let RR = GeneDict(id)
'                                Let crosstalks = (From ctk As CrossTalks In cellularNET.CrossTalksAnnotation
'                                                  Where String.Equals(ctk.Regulator, id)
'                                                  Select ctk).ToArray
'                                Select CrossTalks_Effect = (From p In crosstalks Where GeneDict.ContainsKey(p.Kinase)
'                                                            Let hk = GeneDict(p.Kinase)
'                                                            Select New KeyValuePairObject(Of BinaryExpression, Double) With {
'                                                                .Key = hk,
'                                                                .Value = p.Probability}).ToArray, RR).ToArray
'            TCS_RR_CACHE = (From regulator In TCS_RR_CACHE
'                            Where Not regulator.CrossTalks_Effect.IsNullOrEmpty
'                            Select regulator).ToArray
'            Dim TCS_RR = (From regulator In TCS_RR_CACHE.AsParallel
'                          Let RR_R = New TCS() With {
'                              .Regulator = regulator.RR.Identifier,
'                              .CrossTalks = regulator.CrossTalks_Effect}.set__Regulator(New BinaryExpression With {.Identifier = regulator.RR.Identifier})
'                          Select DirectCast(RR_R, TCS)).ToDictionary(keySelector:=Function(regulator) regulator.UniqueId)

'            Dim Pathways = cellularNET.KEGG_Pathways.GetAllPathways
'            Dim OCS_CACHE = (From item In (From nr In (From nnn In cellularNET.Regulators Where Not nnn.Effectors.IsNullOrEmpty Select nnn.ProteinId, nnn.Effectors Group By ProteinId Into Group).ToArray Select nr.ProteinId, Effectors = (From ng In nr.Group.ToArray Select ng.Effectors).ToArray.ToVector.Distinct.ToArray).ToArray
'                             Let KEGGCompounds = (From EId As String In item.Effectors Let kid As String = cellularNET.MetabolitesModel(EId).KEGGCompound Where Not String.IsNullOrEmpty(kid) Select kid Distinct).ToArray
'                             Let FindPathways = (From kid As String In KEGGCompounds
'                                                 Let finded = (From pathway In Pathways
'                                                               Where pathway.IsContainsCompound(kid)
'                                                               Select pathway.EntryId, Enzymes = (From gid As String In pathway.GetPathwayGenes Where GeneDict.ContainsKey(gid) Select GeneDict(gid)).ToArray).ToArray
'                                                 Select kid, finded).ToArray
'                             Select item.ProteinId, item.Effectors, KEGGCompounds, FindPathways).ToArray
'            Dim OCS = (From Regulator In OCS_CACHE
'                       Let ocs_r As OCS = New OCS With {
'                           .EffectorPathways = (From p In Regulator.FindPathways
'                                                Select (From item In p.finded
'                                                        Select New KeyValuePairData(Of BinaryExpression()) With {
'                                                            .DataObject = item.Enzymes,
'                                                            .Key = item.EntryId,
'                                                            .Value = p.kid}).ToArray).ToVector,
'                           .Regulator = Regulator.ProteinId}
'                       Select DirectCast(ocs_r.set__Regulator(New BinaryExpression With {.Identifier = Regulator.ProteinId}), OCS)).ToDictionary(Function(regulator) regulator.UniqueId)

'            '进行调控因子的替换
'            For Each Expression In RawModel.DynamicsExprs
'                For Each site In Expression.RegulatorySites
'                    For i As Integer = 0 To site.Regulators.Count - 1
'                        Dim regulator = site.Regulators(i)

'                        '进行TCS的替换
'                        If TCS_RR.ContainsKey(regulator.UniqueId) Then
'                            Dim RR_Source = TCS_RR(regulator.UniqueId)
'                            Dim RR = regulator.CopyTo(Of Regulators.TCS)() '这里必须要进行新构造，因为字典之中的对象是唯一的，但是调控模型中的调控因子却不是唯一的，不进行新构造的话，会覆盖weight的值的，会出错！
'                            RR.CrossTalks = RR_Source.CrossTalks

'                            Call site.set_Regulator(i, RR)
'                        ElseIf OCS.ContainsKey(regulator.UniqueId) Then
'                            Dim OCS_Source = OCS(regulator.UniqueId)
'                            Dim r_OCS = regulator.CopyTo(Of Regulators.OCS)()
'                            r_OCS.EffectorPathways = OCS_Source.EffectorPathways

'                            Call site.set_Regulator(i, r_OCS)
'                        End If
'                    Next
'                Next
'            Next

'            Return RawModel
'        End Function

'        Private Class Footprint_INIT
'            Public Property data As RegulatesFootprints()
'            Public Property ORF As String

'            Public Overrides Function ToString() As String
'                Return String.Format("{0}  -- {1} data(s)", ORF, data.Length)
'            End Function
'        End Class

'        ''' <summary>
'        ''' 从<see cref="RegulatesFootprints">调控网络预测数据</see>之中根据调控关系创建一个逻辑网络（本方法适用于初始化最简单的调控网络模型）
'        ''' </summary>
'        ''' <param name="footprints"></param>
'        ''' <param name="LengthMapping">{基因号，核酸链长度}</param>
'        ''' <returns></returns>
'        ''' <remarks>
'        ''' 由于有一些调控因子是找不到任何调控因子的，即该调控因子是位于网络的最上层，则这个调控因子的表达量就使用默认的输入值作为恒定值作为蒙特卡洛实验的输入值
'        ''' </remarks>
'        Public Function CreateObject(footprints As Generic.IEnumerable(Of RegulatesFootprints),
'                                            InitStatus As Generic.IEnumerable(Of NetworkInput), LengthMapping As Dictionary(Of String, Integer)) As BinaryNetwork

'            '            footprints = {
'            '                     (From item As Simulation.ExpressionRegulationNetwork.NetworkInput
'            '                      In InitStatus.AsParallel
'            '                      Where item.NoneRegulation = True
'            '                      Select New SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint With {.ORF = item.GeneId}).ToArray,
'            ' _
'            '                     footprints.ToArray,
'            ' _
'            '                     (From item As SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint
'            '                      In footprints.AsParallel
'            '                      Select item.OperonRegulationCopies).ToArray.MatrixToUltraLargeVector.ToArray
'            ' _
'            '                   }.MatrixToUltraLargeVector

'            '            Dim LQuery = (From item As SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint
'            '                        In footprints.AsParallel
'            '                        Where Not String.IsNullOrEmpty(item.ORF)
'            '                        Select item
'            '                        Group item By item.ORF Into Group).ToArray '按照ORF进行分组
'            '#Const DEBUG = 1
'            '#If DEBUG Then
'            '            Dim XC_1184 = (From n In LQuery Where String.Equals(n.ORF, "XC_1184") Select n).FirstOrDefault
'            '            If Not XC_1184 Is Nothing Then
'            '                Call Console.WriteLine("[DEBUG] XC_1184 ===> {0}", String.Join("; ", (From n In XC_1184.Group Select n.MotifId).ToArray))
'            '            End If
'            '#End If
'            '            Call Console.WriteLine("[DEBUG] data was grouped into {0} ORF data.", LQuery.Count)

'            '            Dim Footprint_INIT = (From item In LQuery.AsParallel Select New Footprint_INIT With {.data = item.Group.ToArray, .ORF = item.ORF}).ToArray

'            '            Dim Mappings = LengthMapping.Values.GenerateMapping '越长映射得越多，在这里需要反过来，越短映射的越多
'            '            Dim Max = Mappings.Max + 1
'            '            Dim TempMap = LengthMapping.ToArray
'            '            LengthMapping = (From i As Integer In Mappings.Sequence Select ID = TempMap(i).Key, M = Mappings(i)).ToArray.ToDictionary(Function(obj) obj.ID, elementSelector:=Function(obj) Math.Abs(obj.M - Max))

'            '            Call Trace.WriteLine("MAX=" & LengthMapping.Values.Max)
'            '            Call Trace.WriteLine("MIN=" & LengthMapping.Values.Min)


'            '            Dim Inits = InitStatus.ToDictionary
'            '            Dim GeneExpressions = (From GeneId
'            '                                   In Footprint_INIT.AsParallel
'            '                                   Let init_Exists As Boolean = Inits.ContainsKey(GeneId.ORF)
'            '                                   Let initb As Boolean = If(init_Exists, Inits(GeneId.ORF).Level, False)
'            '                                   Let initq As Integer = If(init_Exists, Inits(GeneId.ORF).InitQuantity, 100 * RandomDouble())
'            '                                   Select Expression = New BinaryExpression(init:=initb, d:=LengthMapping(GeneId.ORF)) With
'            '                                                       {
'            '                                                           .UniqueId = GeneId.ORF, ._InternalQuantityValue = initq},
'            '                                          RegulationData = GeneId.data.ToArray).ToArray '构建转录模型

'            '            Dim BinaryNetwork = (From item In GeneExpressions Select item.Expression).ToArray.ToDictionary '创建一个方便与进行调控关系创建的数据检索的字典对象

'            '            Call Console.WriteLine("[DEBUG] {0} lines of data!", BinaryNetwork.Count)

'            '            Dim NetworkInitialization_CACHE = (From Gene In GeneExpressions.AsParallel
'            '                                         Let Regulators = (From item
'            '                                                           In Gene.RegulationData
'            '                                                           Select (From pr
'            '                                                                   In item.Regulators.Sequence
'            '                                                                   Select SiteId = item.Distance, RegulatorId = item.Regulators(pr), PccWeight = Val(item.corr(pr))).ToArray).ToArray.MatrixToVector
'            ' _
'            '                                         Let RegulatorModels = (From item
'            '                                                                In Regulators
'            '                                                                Select SiteId = item.SiteId, Weight = item.PccWeight, RegulatorExpression = BinaryNetwork(item.RegulatorId)).ToArray
'            ' _
'            '                                         Let RegulationModels = (From item
'            '                                                                 In RegulatorModels
'            '                                                                 Let RegulationExpression = New RegulationExpression() With
'            '                                                                                            {
'            '                                                                                                .SitePosition = item.SiteId,
'            '                                                                                                .Weight = item.Weight}.set__Regulator(item.RegulatorExpression)
'            '                                                                 Select RegulationExpression
'            '                                                                 Group RegulationExpression By RegulationExpression.siteposition Into Group).ToArray
'            ' _
'            '                                         Select Gene.Expression, RegulationModels).ToArray

'            '            Dim GenerateSites = (From item In NetworkInitialization_CACHE.AsParallel
'            '                                 Select item.Expression,
'            '                                 rs = (From ittt In item.RegulationModels Let site = New SiteInfo With {.Position = ittt.siteposition} Select site.InvokeSet(NameOf(site.Regulators), ittt.Group.ToArray)).ToArray).ToArray

'            '            Dim NetworkInitialization = (From Gene In GenerateSites.AsParallel
'            '                                         Select Gene.Expression.InvokeSet(NameOf(Gene.Expression.RegulatorySites), Gene.rs)).ToArray

'            '            Dim Model = New BinaryNetwork With {._DynamicsExpressions = NetworkInitialization.OrderBy(Function(GeneObject) GeneObject.UniqueId).ToArray.WriteAddress, .InitCache = Inits}

'            '            Dim RegulatorIdList As String() = (From item In footprints Where Not item.Regulators.IsNullOrEmpty Select item.Regulators).ToArray.MatrixToVector.Distinct.ToArray
'            '            Call Console.WriteLine("[DEBUG] Initialization for the regulators....")
'            '            For Each Expression In Model._DynamicsExpressions
'            '                Expression.Is_RegulatorType = Array.IndexOf(RegulatorIdList, Expression.UniqueId) > -1
'            '            Next

'            '            Return Model
'        End Function

'    End Module
'End Namespace
