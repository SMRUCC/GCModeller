#Region "Microsoft.VisualBasic::906f21b196d0b504dd72fe2c0f0578b6, engine\GCModeller\EngineSystem\Engine\MathematicsModels\SVD.vb"

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

' 

'Namespace EngineSystem.MathematicsModels

'    ''' <summary>
'    ''' 在初始化完毕调控网络之后执行本模块
'    ''' </summary>
'    ''' <remarks></remarks>
'    Public Class SVD : Inherits MathematicsModel

'        Dim ExpressionNetwork As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork
'        Dim ChipData As SMRUCC.genomics.Toolkits.RNASeq.ChipData

'        Sub New(ExpressionNetwork As EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork)
'            Dim ChipDataCsvPath As String = ExpressionNetwork.Get_runtimeContainer.SystemVariables.GetItem(Id:=
'                SMRUCC.genomics.GCMarkupLanguage.SystemVariables._URL_CHIPDATA).Value

'            Call LoggingClient.WriteLine(String.Format("Load ChipData from {0}", ChipDataCsvPath))

'            Me.ChipData = SMRUCC.genomics.Toolkits.RNASeq.ChipData.Load(ChipDataCsvPath)
'            Me.ExpressionNetwork = ExpressionNetwork
'        End Sub

'        ''' <summary>
'        ''' 需要在Disposal系统初始化完成之后进行初始化
'        ''' </summary>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Public Function CreateMatrix() As Integer
'            Call LoggingClient.WriteLine("Start to fill the gap from the SVD calculation result...")

'            Dim PccMatrix As SMRUCC.genomics.Toolkits.RNASeq.PccMatrix = ChipData.CalculatePccMatrix
'            '  Dim MAT = SMRUCC.genomics.Toolkits.RNASeq.SVDNetwork.CreateMatrix(ChipData)
'            '  Dim Weights = SMRUCC.genomics.Toolkits.RNASeq.SVDNetwork.Reconstruct(MAT).Array
'            Dim Index = PccMatrix.GeneIdlist
'            Dim OperonPromoters = (From Model In ExpressionNetwork.NetworkComponents Select New With {.OperonId = Model.TransUnit.FeatureBaseType.UniqueId,
'                                                                                                      .PromoterGene = Model.TransUnit.FeatureBaseType.PromoterGene.AccessionId})
'            Dim LQuery = (From ObjectModel In ExpressionNetwork.NetworkComponents Where ObjectModel.VEC.IsNullOrEmpty Select ObjectModel).ToArray  '筛选出没有调控因子的转录模型
'            Dim Metabolism = ExpressionNetwork._CellSystem.Metabolism.Metabolites
'            Dim PccCutoff As Double = Val(ExpressionNetwork.Get_runtimeContainer.SystemVariables.GetItem(SMRUCC.genomics.GCMarkupLanguage.SystemVariables.PARA_SVD_CUTOFF).Value)

'            Dim Regulators As Dictionary(Of String, EngineSystem.ObjectModels.Entity.Compound()) = GetRegulators()
'            Dim IndexHandles = (From item In Regulators Select Array.IndexOf(Index, item.Key)).ToArray

'            '再根据奇异值分解得到的矩阵结果来赋值调控因子
'            For Each TranscriptionModel In LQuery

'                Dim PromoterGene = TranscriptionModel.TransUnit.FeatureBaseType.PromoterGene.UniqueId
'                Dim idx = Array.IndexOf(Index, PromoterGene) '得到了启动子在芯片数据之中的位置
'                If idx = -1 Then
'                    Call LoggingClient.WriteLine(String.Format("Gene {0} was loss in the Chipdata, regulation model will not build!", PromoterGene), "", Type:=Logging.MSG_TYPES.WRN)
'                    Continue For
'                End If

'                Dim WeightVector = PccMatrix(idx)   '根据位置得到权重向量
'                Dim RegulatorList As List(Of EngineSystem.ObjectModels.Entity.Regulator) = New List(Of ObjectModels.Entity.Regulator)

'                For Each id As Integer In IndexHandles
'                    Dim Weight As Double = WeightVector.SampleValue(id)
'                    If Global.System.Math.Abs(Weight) <= PccCutoff Then
'                        Continue For
'                    End If

'                    Dim RegulatorId As String = Index(id) '得到调控因子的基因编号
'                    Dim RegulatorMetabolites = Regulators(RegulatorId)
'                    Dim RegulatorObject = (From RegulatorMetabolite In RegulatorMetabolites
'                                           Select New EngineSystem.ObjectModels.Entity.Regulator With {
'                                               .RegulatorBaseType = New Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Regulator With {.UniqueId = RegulatorId, .ProteinAssembly = RegulatorId},
'                                               .EntityBaseType = RegulatorMetabolite,
'                                               .ModelBaseElement = RegulatorMetabolite.ModelBaseElement,
'                                               .Weight = Weight}).ToArray

'                    For n As Integer = 0 To RegulatorObject.Count - 1
'                        Call RegulatorObject(n).set_Tag(String.Format("*{0}/r:{1}", RegulatorId, PromoterGene))
'                    Next

'                    Call RegulatorList.AddRange(RegulatorObject)
'                Next

'                If Not RegulatorList.IsNullOrEmpty Then
'                    TranscriptionModel.VEC = RegulatorList.ToArray
'                End If
'            Next

'            Call LoggingClient.WriteLine("End of expression regulation network reconstruction...")

'            Return 0
'        End Function

'        ''' <summary>
'        ''' 获取调控因子的编号
'        ''' </summary>
'        ''' <returns>{GeneId, ProteinComplexityId}，当蛋白质复合物没有记录的时候则Value部分为一个空数组</returns>
'        ''' <remarks></remarks>
'        Private Function GetRegulators() As Dictionary(Of String, EngineSystem.ObjectModels.Entity.Compound())
'            Dim LQuery = (From item In DirectCast(ExpressionNetwork.Get_runtimeContainer, EngineSystem.Engine.Modeller).KernelModule.DataModel.Polypeptides
'                          Where item.ProteinType = Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Polypeptide.ProteinTypes.TranscriptFactor
'                          Select item.UniqueId).ToArray

'            Dim ChunkBuffer As List(Of EngineSystem.ObjectModels.Entity.Compound) = New List(Of ObjectModels.Entity.Compound)

'            For Each Model In ExpressionNetwork.NetworkComponents
'                If Not Model.VEC.IsNullOrEmpty Then
'                    Call ChunkBuffer.AddRange((From item In Model.VEC Select item.EntityBaseType).ToArray)
'                End If
'            Next

'            ChunkBuffer = (From item In ChunkBuffer Select item Distinct).AsList

'            Dim GetValue = Function(GeneId As String) As KeyValuePair(Of String, EngineSystem.ObjectModels.Entity.Compound())
'                               Dim GetCplxLQuery = (From item In ChunkBuffer Where InStr(item.UniqueID, GeneId, CompareMethod.Text) > 0 Select item).ToArray
'                               Return New KeyValuePair(Of String, EngineSystem.ObjectModels.Entity.Compound())(GeneId, GetCplxLQuery)
'                           End Function

'            Dim DictCache = (From ModelBaseId In LQuery Select GetValue(ModelBaseId)).ToArray
'            Dim Data As Dictionary(Of String, EngineSystem.ObjectModels.Entity.Compound()) = New Dictionary(Of String, ObjectModels.Entity.Compound())
'            For Each item In DictCache
'                Call Data.Add(item.Key, item.Value)
'            Next

'            Return Data
'        End Function
'    End Class
'End Namespace
