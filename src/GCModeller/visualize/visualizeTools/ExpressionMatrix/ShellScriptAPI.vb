Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.InteractionModel

Namespace ExpressionMatrix

    <[Namespace]("expression.matrix")>
    Module ShellScriptAPI

        <ExportAPI("mat.invoke_drawing")>
        Public Function DrawingImage(data As SerialsData()) As Image
            Return MatrixDrawing.InvokeDrawing(data, Nothing)
        End Function

        <ExportAPI("mat.load")>
        Public Function LoadData(data As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File) As SerialsData()
            Return DataServicesExtension.LoadCsv(data)
        End Function

        <ExportAPI("mat.invoke_drawing")>
        Public Function DrawingMatrix(MAT As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File) As Image
            Return MatrixDrawing.NormalMatrix(MAT)
        End Function

        <ExportAPI("mat.Triangular_drawing")>
        Public Function DrawingMatrixTr(MAT As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File) As Image
            Return MatrixDrawing.NormalMatrixTriangular(MAT)
        End Function
    End Module
End Namespace