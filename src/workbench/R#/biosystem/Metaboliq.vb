Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DeepLearning.LiquidNeuralNetwork
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metaboliq
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

''' <summary>
''' Liquid In-silico Metabolic Network
''' </summary>
<Package("Metaboliq")>
<RTypeExport("config", GetType(MetabolicTrainerConfig))>
Module MetaboliqTool

    <ExportAPI("read_metabolic_graph")>
    Public Function read_graph(file As String, Optional explicit_boundary As Object = Nothing) As MetabolicNetworkGraph
        Return MetabolicNetworkGraph.LoadJson(file, CLRVector.asCharacter(explicit_boundary))
    End Function

    ''' <summary>
    ''' read metabolome/enzyme expression time serials data matrix
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read_timedata")>
    Public Function read_timedata(file As String) As TimeSeriesMatrix
        Return MetabolicDataIO.LoadCsv(file)
    End Function

    <ExportAPI("new")>
    Public Function metaboliq(graph As MetabolicNetworkGraph, Optional mode As LiquidMode = LiquidMode.LTC, Optional solver As String = "rk4") As MetabolicLiquidNetwork
        Return New MetabolicLiquidNetwork(graph, mode, solver)
    End Function

    ''' <summary>
    ''' make liquid network training
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="observed"></param>
    ''' <param name="enzymeSeries"></param>
    ''' <param name="boundarySeries"></param>
    ''' <param name="fluxTruth"></param>
    ''' <returns>
    ''' get training loss <see cref="EpochLoss"/> vector data via attr(x, "loss")
    ''' </returns>
    <ExportAPI("fit")>
    <RApiReturn(GetType(MetabolicTrainer))>
    Public Function fit(model As MetabolicLiquidNetwork, config As MetabolicTrainerConfig, <RRawVectorArgument(TypeCodes.double)> times As Object,
                        observed As Tensor,
                        enzymeSeries As Tensor,
                        boundarySeries As Tensor,
                        fluxTruth As Tensor) As Object

        Dim trainer As New MetabolicTrainer(model, config)
        Dim loss As EpochLoss() = trainer.Fit(CLRVector.asNumeric(times), observed, enzymeSeries, boundarySeries, fluxTruth).ToArray
        Dim value As New vbObject(trainer)
        value.setAttribute("loss", loss)
        Return value
    End Function

    <ExportAPI("predict")>
    Public Function predict(model As MetabolicTrainer, h0 As Tensor, <RRawVectorArgument(TypeCodes.double)> times As Object, enzymeSeries As Tensor,
                            boundarySeries As Tensor) As MetabolicTrajectory

        Return model.Predict(h0, CLRVector.asNumeric(times), enzymeSeries, boundarySeries)
    End Function

End Module
