Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork.StoreProcedure

Public Module Extensions

    ''' <summary>
    ''' We usually use this extension method for generates the demo test dataset.
    ''' </summary>
    ''' <param name="samples"></param>
    ''' <param name="inputNames"></param>
    ''' <param name="outputNames"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function SampleSetCreator(samples As IEnumerable(Of Sample),
                                     Optional inputNames As IEnumerable(Of String) = Nothing,
                                     Optional outputNames As IEnumerable(Of String) = Nothing) As DataSet

        Return New SampleList With {
            .items = samples _
                .SafeQuery _
                .ToArray
        }.CreateDataSet(inputNames, outputNames)
    End Function
End Module
