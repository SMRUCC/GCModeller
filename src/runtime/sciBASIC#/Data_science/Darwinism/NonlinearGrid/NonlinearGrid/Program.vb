Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF.Helper
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.NonlinearGridTopology
Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork.StoreProcedure

Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("/summary")>
    <Usage("/summary /in <model.Xml> /data <trainingSet.Xml>")>
    Public Function Summary(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim data$ = args <= "/data"
        Dim model = [in].LoadXml(Of GridMatrix)
        Dim dataset = data.LoadXml(Of DataSet)

    End Function

    <ExportAPI("/training")>
    <Usage("/training /in <trainingSet.Xml> [/out <output_model.Xml>]")>
    Public Function trainGA(args As CommandLine) As Integer
        Dim inFile As String = args <= "/in"
        Dim out$ = args("/out") Or $"{inFile.TrimSuffix}.minError.Xml"

        If Not inFile.FileExists Then
            Call "No input file was found!".PrintException
        Else
            Call runGA(inFile, out)
        End If

        Return 0
    End Function

    Private Sub runGA(inFile$, outFile$)
        Dim trainingSet = inFile.LoadXml(Of DataSet)
        Dim population As Population(Of Genome) = New Genome(Loader.EmptyGridSystem(trainingSet.width)).InitialPopulation(5000)
        Dim fitness As Fitness(Of Genome) = New Environment(trainingSet.DataSamples.AsEnumerable)
        Dim ga As New GeneticAlgorithm(Of Genome)(population, fitness)
        Dim engine As New EnvironmentDriver(Of Genome)(ga) With {
            .Iterations = 10000,
            .Threshold = 0.005
        }

        Call engine.AttachReporter(Sub(i, e, g)
                                       Call EnvironmentDriver(Of Genome).CreateReport(i, e, g).ToString.__DEBUG_ECHO
                                       Call g.Best _
                                             .CreateSnapshot(e) _
                                             .GetXml _
                                             .SaveTo(outFile)
                                   End Sub)
        Call engine.Train()
    End Sub
End Module
