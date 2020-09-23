#Region "Microsoft.VisualBasic::52e8b165a9e714636ce8def04009e112, sub-system\CellPhenotype\TRN\NetEngine\BinaryNetwork.vb"

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

    '     Class BinaryNetwork
    ' 
    '         Properties: DynamicsExprs, NonRegulationHandles, RuntimeTicks
    ' 
    '         Function: __generateDataChunk, __innerTicks, AllRegulatorInputs, AnalysisMonteCarloTopLevelInput, get_Model
    '                   get_Regulators, GetEnumerator, GetEnumerator1, GetGeneObjects, GetRegulator
    '                   Initialize, SaveNetwork, SetConfigs, SetMutationFactor, ToString
    '                   WriteNodeStates, WriteRegulationValues
    ' 
    '         Sub: Reset, Save, SetKernelLoops
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.genomics.Analysis.CellPhenotype.TRN.KineticsModel
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.DataStorage.FileModel
Imports SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat

Namespace TRN

    ''' <summary>
    ''' 使用逻辑值来模拟计算基因表达调控网络
    ''' </summary>
    ''' <remarks>
    ''' 这里是现实之中的约束条件：
    ''' 
    ''' 1. 合成速度：
    ''' 假设对于每一个碱基而言，其合成的速度是一样的，那么很显然单位时间内，越长的核酸链其合成速度越慢，即每一次迭代循环过程之中，假若该基因的长度越长，则相较于较短的核酸链产生的分子数目越少
    ''' 2. 降解速度：
    ''' 假设酶水解下一个碱基都是以相同的速度，那么水解完一条较长的核酸链很显然相较于较短的核酸链会需要更加长的时间。但是当水解掉最开始的一个碱基之后，我们假设原有的有活性的核酸链将无法再被用于翻译，故而在降解速度方面，较长的核酸链和较短的核酸链都是相同的失活速度的
    ''' 
    ''' </remarks>
    Public Class BinaryNetwork : Inherits ReactorMachine(Of Integer, BinaryExpression)
        Implements IEnumerable(Of BinaryExpression)
        Implements Configs.I_Configurable

        Dim _netStates As DataAdapter(Of Integer, StateEnumerationsSample) = New DataAdapter(Of Integer, StateEnumerationsSample)
        Dim _initCache As Dictionary(Of String, NetworkInput)
        Dim _regStates As DataAdapter(Of Integer, StateEnumerationsSample) = New DataAdapter(Of Integer, StateEnumerationsSample)

        Public ReadOnly Property DynamicsExprs As BinaryExpression()
            Get
                Return MyBase._DynamicsExprs
            End Get
        End Property

        ''' <summary>
        ''' 返回调控网络之中处于表达状态的基因的数目
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function __innerTicks(KernelCycle As Integer) As Integer
            Dim LQuery = (From data0expr As BinaryExpression
                          In _DynamicsExprs.Shuffles
                          Select data0expr.Evaluate).ToArray
            Dim chunkBuffer = (From data0expr As BinaryExpression
                               In _DynamicsExprs
                               Let sample = New StateEnumerationsSample With {
                                   .Handle = data0expr.Handle,
                                   .Value = If(data0expr.Status = True, 1, 0),
                                   .TimeStamp = KernelCycle
                               }
                               Select sample)
            Call _netStates.DataAcquiring(chunkBuffer)
            Call _regStates.DataAcquiring((From data0expr As BinaryExpression
                                                 In _DynamicsExprs
                                           Let sample = New StateEnumerationsSample With {
                                                     .Handle = data0expr.Handle,
                                                     .Value = data0expr.RegulationValue,
                                                     .TimeStamp = KernelCycle
                                                 }
                                           Select sample).ToArray)
            Return (From b As Boolean In LQuery
                    Where b = True
                    Select b).ToArray.Length
        End Function

        Public Sub Reset()
            For Each Expression As BinaryExpression In _DynamicsExprs
                If _initCache.ContainsKey(Expression.Identifier) Then
                    Expression._InternalQuantityValue = _initCache(Expression.Identifier).InitQuantity
                Else
                    Expression._InternalQuantityValue = 0
                End If
            Next

            Call _netStates.Clear()
            Call _regStates.Clear()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="GeneID"></param>
        ''' <param name="Factor">
        ''' 0 - 缺失突变
        ''' 1 - 正常表达
        ''' &gt;1 - 过量表达
        ''' 0-1 - 调控事件以低于平常的概率发生 
        ''' </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SetMutationFactor(GeneID As String, Factor As Double) As Boolean
            Dim Expression = _DynamicsExprs.Take(uniqueId:=GeneID)

            If Expression Is Nothing Then
                Return False
            Else
                Expression.set_Mutation(Factor)
                Return True
            End If
        End Function

        Public Function SaveNetwork(url As String) As Boolean
            Return Me._DynamicsExprs.GetXml.SaveTo(url)
        End Function

        Public Function WriteNodeStates(url As String) As Boolean
            Using CsvAdapter = New MsCsvChunkBuffer(Of Integer)
                Dim Data = _netStates.FetchData((From item In _DynamicsExprs Select item.CreateHandle).ToArray)
                Return CsvAdapter.WriteData(Data, url)
            End Using
        End Function

        Public Function WriteRegulationValues(url As String) As Boolean
            Using CsvAdapter As MsCsvChunkBuffer(Of Integer) = New MsCsvChunkBuffer(Of Integer)

                Dim ChunkBuffer = _regStates.FetchData((From data0expr In _DynamicsExprs Select data0expr.CreateHandle).ToArray)
                Dim LQuery = (From data0expr As DataSerials(Of Integer)
                              In ChunkBuffer.AsParallel
                              Let DataChunk As String() = __generateDataChunk(data0expr)
                              Select data0expr.UniqueId, DataChunk).ToArray
                Dim GenerateCsv = (From row In LQuery.AsParallel
                                   Let ID As String() = {row.UniqueId}
                                   Let Data As String()() = {ID, row.DataChunk}
                                   Select CType(Data.Unlist, RowObject)).ToArray
                Dim Csv As File = CType(GenerateCsv, File)
                Return Csv.Save(url, False)
            End Using
        End Function

        Private Shared Function __generateDataChunk(data0expr As DataSerials(Of Integer)) As String()
            Dim ChunkTemp As String() = New String(data0expr.Samples.Count - 2) {}
            Dim Pre As Integer = data0expr.Samples.First

            For i As Integer = 1 To data0expr.Samples.Length - 1
                Dim n = data0expr.Samples(i)
                ChunkTemp(i - 1) = (n + Pre).ToString
                Pre = n
            Next

            Return ChunkTemp
        End Function

        ''' <summary>
        ''' Gets the genes unique id collection.(获取本网络模型对象之中的基因的编号列表)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetGeneObjects() As String()
            Dim LQuery = (From data0expr In _DynamicsExprs Select data0expr.Identifier Distinct).ToArray
            Return LQuery
        End Function

        Public Function get_Regulators() As String()
            Dim LQuery = (From GeneExpressionEvent As BinaryExpression
                          In _DynamicsExprs
                          Where Not GeneExpressionEvent.RegulatorySites.IsNullOrEmpty
                          Let Regulators = (From site As KineticsModel.SiteInfo
                                            In GeneExpressionEvent.RegulatorySites
                                            Select (From data0expr In site.Regulators Select data0expr.Regulator).ToArray).ToArray.ToVector
                          Select Regulators).ToArray.ToVector.Distinct.ToArray
            Return LQuery
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0} gene objects and regulators are ""{1}"".", GetGeneObjects.Count, String.Join("; ", get_Regulators))
        End Function
        ''' <summary>
        ''' DEBUG
        ''' </summary>
        ''' <param name="dict"></param>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetRegulator(dict As Dictionary(Of String, BinaryExpression), id As String) As BinaryExpression
            If dict.ContainsKey(id) Then
                Return dict(id)
            Else
                MsgBox(id)
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' Monte Carlo experiment input analysis.(分析出最顶层的调控因子作为蒙特卡洛实验的输入之一)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>由于有一些调控因子是找不到任何调控因子的，即该调控因子是位于网络的最上层，则这个调控因子的表达量就使用默认的输入值作为恒定值作为蒙特卡洛实验的输入值</remarks>
        Public Shared Function AnalysisMonteCarloTopLevelInput(footprints As IEnumerable(Of RegulatesFootprints)) As String()
            Dim Regulations As RegulatesFootprints() = (From item As RegulatesFootprints
                                                                       In footprints.AsParallel
                                                        Where Not String.IsNullOrEmpty(item.Regulator)
                                                        Select item).ToArray
            Dim ORFList As String() = (From item In Regulations Select item.ORF Distinct).ToArray
            Dim Regulators As String() = (From item In Regulations Select regEntries = item.Regulator).ToArray.Distinct.ToArray

            Call Console.WriteLine("[DEBUG] regulator elements: {0}", String.Join("; ", Regulators))
            Dim TopLevelRegulators As String() = (From RegulatorId As String In Regulators.AsParallel Where Array.IndexOf(ORFList, RegulatorId) = -1 Select RegulatorId).ToArray

            Call Console.WriteLine("[DEBUG] top level regulators {0}", String.Join("; ", TopLevelRegulators))

            Return TopLevelRegulators
        End Function

        ''' <summary>
        ''' 将所有的调控因子都作为蒙特卡洛实验的输入
        ''' </summary>
        ''' <param name="footprints"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function AllRegulatorInputs(footprints As Generic.IEnumerable(Of RegulatesFootprints)) As String()
            Dim Regulators As String() = (From site As RegulatesFootprints
                                          In footprints.AsParallel
                                          Where Not String.IsNullOrEmpty(site.Regulator)
                                          Select site.Regulator).Distinct.ToArray
            Return Regulators
        End Function

        ''' <summary>
        ''' Setup the internal kernel cycles number of this binary cellular gene expression regulation network model.(对本计算模型设置内核循环的数目。)
        ''' </summary>
        ''' <param name="KelCycles">The kernel cycle vaslue that will be setup as the runtime parameter of the model simulation.</param>
        ''' <remarks></remarks>
        Public Sub SetKernelLoops(KelCycles As Integer)
            Me.IterationLoops = KelCycles
        End Sub

        ''' <summary>
        ''' Gets all of the gene nodes handles collection in this expression regulation network.(获取在网络结构之上没有受到任何调控作用的基因网络节点)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NonRegulationHandles As Long()
            Get
                Dim LQuery = (From dnyExpression As KineticsModel.BinaryExpression
                              In _DynamicsExprs
                              Where dnyExpression.RegulatorySites.IsNullOrEmpty OrElse
                                  dnyExpression.RegulatorCounts = 0
                              Select dnyExpression.Handle).ToArray
                Return LQuery
            End Get
        End Property

        Public Overrides Function Initialize() As Integer
            Call _netStates.SetFiltedHandles(NonRegulationHandles)
            Return 0
        End Function

        Public Sub Save(file As String)
            Call Me.GetXml.SaveTo(file)
        End Sub

        Public Function get_Model() As NetworkModel
            Return New NetworkModel With {
                .GeneObjects = _DynamicsExprs
            }
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of BinaryExpression) Implements IEnumerable(Of BinaryExpression).GetEnumerator
            For Each item As BinaryExpression In _DynamicsExprs
                Yield item
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

        Public Function SetConfigs(conf As Configs) As Integer Implements Configs.I_Configurable.SetConfigs
            Return (From item In Me._DynamicsExprs Select item.SetConfigs(conf)).ToArray.Count
        End Function

        Public Overrides ReadOnly Property RuntimeTicks As Long
            Get
                Return Me.IterationLoops
            End Get
        End Property
    End Class
End Namespace
