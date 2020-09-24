#Region "Microsoft.VisualBasic::90a682c66dfb156ed29dcf1842d00e68, engine\AutoCAD\GACircler\DeletionToMinimum.vb"

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

    ' Module DeletionToMinimum
    ' 
    '     Function: DoDeletion
    ' 
    ' Class Encoder
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Decode
    ' 
    ' Class Genome
    ' 
    '     Properties: Size
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: Clone, Crossover, Mutate, RunEvaluation
    ' 
    ' Class Fitness
    ' 
    '     Properties: Cacheable
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Calculate
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF.Helper
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.Models
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports TFReg = SMRUCC.genomics.GCModeller.ModellingEngine.Model.Regulation

''' <summary>
''' 通过删减基因的方法将基因组最小化
''' </summary>
Public Module DeletionToMinimum

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="model">这个数据模型之中仅包含有生物学功能的描述，并没有任何序列信息</param>
    ''' <param name="define"></param>
    ''' <param name="popSize">进化的种群大小</param>
    ''' <param name="eval">计算突变体的对环境的适应度</param>
    ''' <returns>
    ''' 这个函数返回的是目标可能的最优解下的所有的剩余的遗传元件的编号列表，然后下游程序可以根据这个编号列表来进行全基因组序列的装配
    ''' </returns>
    Public Function DoDeletion(model As CellularModule, define As Definition, eval As Func(Of Vessel, Double), Optional popSize% = 500) As String()
        Dim envir As Vessel = New Loader(define).CreateEnvironment(model)
        Dim byteMap As New Encoder(model)
        Dim population As Population(Of Genome) = New Genome(byteMap, envir).InitialPopulation(5000)
        Dim ga As New GeneticAlgorithm(Of Genome)(population, New Fitness(eval, define.status))
        Dim engine As New EnvironmentDriver(Of Genome)(ga) With {
            .Iterations = 10000,
            .Threshold = 0.005
        }

        Call engine.AttachReporter(Sub(i, e, g) EnvironmentDriver(Of Genome).CreateReport(i, e, g).ToString.__DEBUG_ECHO)
        Call engine.Train()

        Dim solutionBytes = ga.Best.chromosome
        Dim components = byteMap.Decode(solutionBytes, inactive:=False).ToArray

        Return components
    End Function

End Module

Public Class Encoder

    Friend ReadOnly index As New Index(Of String)

    Sub New(model As CellularModule)
        For Each gene As CentralDogma In model.Genotype
            Call index.Add(gene.geneID)
        Next
        For Each reg As TFReg In model.Regulations.Where(Function(r) r.type = Processes.Transcription)
            Call index.Add(reg.process)
        Next
    End Sub

    ''' <summary>
    ''' 获取得到目标解所对应的激活的基因组元件或者被删除的基因组元件的编号列表
    ''' </summary>
    ''' <param name="bytes"></param>
    ''' <param name="inactive">是否获取得到未激活的对象编号？</param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Decode(bytes As Integer(), inactive As Boolean) As IEnumerable(Of String)
        Return bytes _
            .SeqIterator _
            .Where(Function(b)
                       If inactive Then
                           Return b.value = 0
                       Else
                           Return b.value > 0
                       End If
                   End Function) _
            .Select(Function(i)
                        Return index(index:=i)
                    End Function)
    End Function
End Class

''' <summary>
''' 基因组是由遗传元件所构成的
''' </summary>
Public Class Genome : Implements Chromosome(Of Genome)

    ''' <summary>
    ''' 染色体上面的基因以及调控位点的构成，1表示存在，0表示缺失
    ''' </summary>
    Friend chromosome As Integer()
    Friend test As Vessel
    Friend byteMap As Encoder

    Shared ReadOnly random As New Random()

    ''' <summary>
    ''' 获取当前的基因组的大小
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Size As Integer
        Get
            Return chromosome.Where(Function(b) b > 0).Sum
        End Get
    End Property

    Sub New()
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="encoder"></param>
    ''' <remarks>
    ''' 在初始状态下所有的遗传元件都是存在的，所以初始序列全部都是1
    ''' </remarks>
    Sub New(encoder As Encoder, model As Vessel)
        chromosome = encoder _
            .index _
            .Select(Function([byte]) 1) _
            .ToArray
        test = model
        byteMap = encoder
    End Sub

    Public Function RunEvaluation(fitness As Func(Of Vessel, Double), init As Dictionary(Of String, Double)) As Double
        ' 首先会需要根据基因组内容进行模板的激活或者缺失
        Dim components = byteMap.Decode(chromosome, inactive:=False).Indexing
        Dim deletes = byteMap.Decode(chromosome, inactive:=True).Indexing

        For Each mass As Factor In test.Mass
            If mass.ID Like components Then
                ' 模板只有一个
                mass.Value = 1
            ElseIf mass.ID Like deletes Then
                ' 当前的这个模板应该是被缺失掉的
                mass.Value = 0
            Else
                ' 对于其他化合物分子，则需要回复到初始值
                mass.Value = init(mass.ID)
            End If
        Next

        ' 对当前得到的这个基因组模型计算模拟计算以及评估
        Return fitness(test)
    End Function

    Private Function Clone() As Genome
        Return New Genome With {
            .chromosome = chromosome.ToArray,
            .test = test.Clone,
            .byteMap = byteMap
        }
    End Function

    Public Iterator Function Crossover(another As Genome) As IEnumerable(Of Genome) Implements Chromosome(Of Genome).Crossover
        Dim thisClone As Genome = Me.Clone()
        Dim otherClone As Genome = another.Clone()

        Call random.Crossover(thisClone.chromosome, another.chromosome)

        Yield thisClone
        Yield otherClone
    End Function

    Public Function Mutate() As Genome Implements Chromosome(Of Genome).Mutate
        Dim result As Genome = Me.Clone()
        Call result.chromosome.ByteMutate(random)
        Return result
    End Function
End Class

Public Class Fitness : Implements Fitness(Of Genome)

    Public ReadOnly Property Cacheable As Boolean Implements Fitness(Of Genome).Cacheable
        Get
            Return False
        End Get
    End Property

    Dim evaluation As Func(Of Vessel, Double)
    Dim reset As Dictionary(Of String, Double)

    Sub New(target As Func(Of Vessel, Double), init As Dictionary(Of String, Double))
        evaluation = target
        reset = init
    End Sub

    ''' <summary>
    ''' 1. 基因组应该尽量小
    ''' 2. 目标函数应该尽量小
    ''' </summary>
    ''' <param name="chromosome"></param>
    ''' <returns></returns>
    Public Function Calculate(chromosome As Genome) As Double Implements Fitness(Of Genome).Calculate
        Dim size = Math.Log(chromosome.Size)
        Dim test = chromosome.RunEvaluation(evaluation, reset)

        Return (size + test) / 2
    End Function
End Class
