Imports GACircler
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.Models
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

''' <summary>
''' 通过删减基因的方法将基因组最小化
''' </summary>
Public Module DeletionToMinimum

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="define"></param>
    ''' <param name="popSize">进化的种群大小</param>
    ''' <param name="fitness">计算突变体的对环境的适应度</param>
    ''' <returns></returns>
    Public Function DoDeletion(model As CellularModule, define As Definition, fitness As Func(Of Vessel, Double), Optional popSize% = 500)
        Dim envir As Vessel = New Loader(define).CreateEnvironment(model)

    End Function


End Module

''' <summary>
''' 基因组是由遗传元件所构成的
''' </summary>
Public Class Genome : Implements Chromosome(Of Genome)

    ''' <summary>
    ''' 染色体上面的基因以及调控位点的构成，1表示存在，0表示缺失
    ''' </summary>
    Dim chromosome As Integer()

    Public Function Crossover(another As Genome) As IEnumerable(Of Genome) Implements Chromosome(Of Genome).Crossover
        Throw New NotImplementedException()
    End Function

    Public Function Mutate() As Genome Implements Chromosome(Of Genome).Mutate
        Throw New NotImplementedException()
    End Function
End Class