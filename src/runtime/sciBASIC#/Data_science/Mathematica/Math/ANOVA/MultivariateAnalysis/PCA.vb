Imports System.Collections.ObjectModel
Imports System.Runtime.CompilerServices
Imports std = System.Math

Public Module PCA

    <Extension>
    Public Function PrincipalComponentAnalysis(statObject As StatisticsObject,
                                               Optional maxPC As Integer = 5,
                                               Optional cutoff As Double = 0.00001) As MultivariateAnalysisResult
        Dim dataArray = statObject.XScaled
        Dim rowSize = dataArray.GetLength(0)
        Dim columnSize = dataArray.GetLength(1)

        If rowSize < maxPC Then
            maxPC = rowSize
        End If

        'PrincipalComponentAnalysisResult pcaBean = new PrincipalComponentAnalysisResult() {
        '    ScoreIdCollection = statObject.YIndexes,
        '    LoadingIdCollection = statObject.XIndexes,
        '    ScoreLabelCollection = statObject.YLabels,
        '    LoadingLabelCollecction = statObject.XLabels,
        '    ScoreBrushCollection = statObject.YColors,
        '    LoadingBrushCollection = statObject.XColors
        '};

        Dim tpMatrix = New Double(rowSize - 1, columnSize - 1) {}
        Dim contributions = New ObservableCollection(Of Double)()
        Dim scores = New ObservableCollection(Of Double())()
        Dim loadings = New ObservableCollection(Of Double())()

        For i As Integer = 0 To maxPC - 1
            Call VBDebugger.EchoLine($"Calculate component {i + 1}...")
            Call dataArray.CalculateComponent(i, columnSize, rowSize, cutoff, tpMatrix, contributions, scores, loadings)
        Next

        Dim maResult = New MultivariateAnalysisResult() With {
            .StatisticsObject = statObject,
            .analysis = GetType(PCA),
            .NFold = 0,
            .Contributions = contributions,
            .TPreds = scores,
            .PPreds = loadings
        }

        Return maResult
    End Function

    <Extension>
    Private Function CalculateComponent(ByRef dataArray As Double(,),
                                        i As Integer,
                                        columnSize As Integer,
                                        rowSize As Integer,
                                        cutoff As Double,
                                        ByRef tpMatrix As Double(,),
                                        ByRef contributions As ObservableCollection(Of Double),
                                        ByRef scores As ObservableCollection(Of Double()),
                                        ByRef loadings As ObservableCollection(Of Double())) As Integer

        Dim mean = New Double(columnSize - 1) {}
        Dim var = New Double(columnSize - 1) {}
        Dim scoreOld = New Double(rowSize - 1) {}
        Dim scoreNew = New Double(rowSize - 1) {}
        Dim loading = New Double(columnSize - 1) {}
        Dim sum As Double
        Dim contributionOriginal As Double = 1

        For j = 0 To columnSize - 1
            sum = 0
            For k = 0 To rowSize - 1
                sum += dataArray(k, j)
            Next
            mean(j) = sum / rowSize
        Next

        For j = 0 To columnSize - 1
            sum = 0
            For k = 0 To rowSize - 1
                sum += std.Pow(dataArray(k, j) - mean(j), 2)
            Next
            var(j) = sum / (rowSize - 1)
        Next

        If i = 0 Then
            contributionOriginal = var.Sum
        End If

        Dim maxVar = var.Max
        Dim maxVarID = Array.IndexOf(var, maxVar)

        For j = 0 To rowSize - 1
            scoreOld(j) = dataArray(j, maxVarID)
        Next

        Dim threshold = Double.MaxValue

        i = 0

        While threshold > cutoff
            Dim scoreScalar = BasicMathematics.SumOfSquare(scoreOld)

            For j = 0 To columnSize - 1
                sum = 0
                For k = 0 To rowSize - 1
                    sum += dataArray(k, j) * scoreOld(k)
                Next
                loading(j) = sum / scoreScalar
            Next

            Dim loadingScalar = BasicMathematics.RootSumOfSquare(loading)

            For j = 0 To columnSize - 1
                loading(j) = loading(j) / loadingScalar
            Next

            For j = 0 To rowSize - 1
                sum = 0
                For k = 0 To columnSize - 1
                    sum += dataArray(j, k) * loading(k)
                Next
                scoreNew(j) = sum
            Next

            threshold = BasicMathematics.RootSumOfSquare(scoreNew, scoreOld)

            For j = 0 To scoreNew.Length - 1
                scoreOld(j) = scoreNew(j)
            Next

            i += 1
        End While

        For j = 0 To columnSize - 1
            For k = 0 To rowSize - 1
                tpMatrix(k, j) = scoreNew(k) * loading(j)
                dataArray(k, j) = dataArray(k, j) - tpMatrix(k, j)
            Next
        Next

        Dim scoreVar = BasicMathematics.Var(scoreNew)

        contributions.Add(scoreVar / contributionOriginal * 100)
        scores.Add(scoreNew)
        loadings.Add(loading)

        Return i
    End Function
End Module
