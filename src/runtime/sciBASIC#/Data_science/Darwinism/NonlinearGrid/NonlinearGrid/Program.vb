Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF.Helper
Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork.StoreProcedure
Imports TopologyInference

Module Program

    Sub Main()
        Dim trainingSet = "D:\biodeep\biodeepDB\smartnucl_integrative\build_tools\CVD_kb\duke\20190626_12_ANN\singls\trainingSet[Stroke].Xml".LoadXml(Of DataSet)
        Dim population As Population(Of Genome) = New Genome(Loader.EmptyGridSystem(trainingSet.width)).InitialPopulation(5000)
        Dim fitness As Fitness(Of Genome) = New Environment(trainingSet.DataSamples.AsEnumerable)
        Dim ga As New GeneticAlgorithm(Of Genome)(population, fitness)
        Dim engine As New EnvironmentDriver(Of Genome)(ga) With {
            .Iterations = 10000,
            .Threshold = 0.005
        }

        Call engine.AttachReporter(Sub(i, e, g)
                                       EnvironmentDriver(Of Genome).CreateReport(i, e, g).ToString.__DEBUG_ECHO
                                   End Sub)
        Call engine.Train()

        Pause()
    End Sub
End Module
