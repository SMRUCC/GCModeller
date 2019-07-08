﻿Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.ComponentModel.Normalizer
Imports Microsoft.VisualBasic.DataMining.DecisionTree.Data
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork.StoreProcedure
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace RandomForests

    ''' <summary>
    ''' 随机森林有一种数据来源是与神经网络的数据集一致的
    ''' 只不过在这里解析样本数据使用的归一化方法始终是
    ''' 区间离散化方法
    ''' </summary>
    Public Module MatrixLoader

        ''' <summary>
        ''' Load training sample dataset of ANN model for build random forest
        ''' </summary>
        ''' <param name="matrix">The output value in the training set only allows 0 and 1.</param>
        ''' <param name="outputName">
        ''' If this argument is nothing, then it means combine all of the output with its name and value as the random forest classify result, 
        ''' otherwise if the output name is specific and exists in matrix output column, then only predicts for the value result of target 
        ''' column. 
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateTable(matrix As DataSet, Optional outputName$ = Nothing) As DataTable
            Dim outputValuer As Func(Of Sample, String)
            Dim headers As String()

            If outputName.StringEmpty Then
                outputValuer = AddressOf matrix.output.combineMultipleOutput
                headers = matrix.NormalizeMatrix.names.AsList + matrix.output.JoinBy("|")
            Else
                Dim outIndex As Integer = matrix.output.IndexOf(outputName)

                If outIndex < 0 Then
                    Throw New InvalidExpressionException($"Result output name '{outputName}' is not exists in trainingSet!")
                End If

                outputValuer = Function(sample) sample.target(outIndex).ToString
                headers = matrix.NormalizeMatrix.names.AsList + outputName
            End If

            Dim trainingSet As Entity() = matrix _
                .PopulateNormalizedSamples(Methods.RangeDiscretizer) _
                .Select(Function(sample)
                            Dim evidence = sample.status _
                                .vector _
                                .AsCharacter(format:="G2") _
                                .AsList

                            Return New Entity With {
                                .entityVector = evidence + outputValuer(sample)
                            }
                        End Function) _
                .ToArray

            Dim table As New DataTable With {
                .headers = headers,
                .rows = trainingSet
            }

            Return table
        End Function

        <Extension>
        Private Function combineMultipleOutput(outputNames As String(), sample As Sample) As String
            Dim positive As New List(Of String)

            For i As Integer = 0 To outputNames.Length - 1
                If sample.target(i) <> 0R Then
                    positive += outputNames(i)
                End If
            Next

            If positive = 0 Then
                Return "*"
            Else
                Return positive.JoinBy("|")
            End If
        End Function
    End Module
End Namespace