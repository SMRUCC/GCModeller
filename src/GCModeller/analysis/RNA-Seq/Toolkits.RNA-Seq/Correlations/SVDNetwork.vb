Imports LANS.SystemsBiology.Toolkits.RNA_Seq.dataExprMAT
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Mathematical
Imports Microsoft.VisualBasic.Mathematical.Matrix

<[Namespace]("Network.SVD",
             Description:="Try creates a draft gene expression regulation network by using Matrix SVD method.")>
Public Module SVDNetwork

    <ExportAPI("Matrix.Create.From.Chipdata")>
    Public Function CreateMatrix(ChipData As MatrixFrame) As GeneralMatrix
        Dim MAT As Double()() = (From csvLine As DocumentStream.RowObject
                                 In ChipData.GetOriginalMatrix
                                 Select (From col As String
                                         In csvLine.Skip(1)
                                         Select Val(col)).ToArray).ToArray
        Dim Matrix = New GeneralMatrix(MAT)
        Return Matrix
    End Function

    <ExportAPI("Reconstruct")>
    Public Function Reconstruct(MAT As GeneralMatrix) As GeneralMatrix
        Dim SVD = New SingularValueDecomposition(MAT)
        Dim U = SVD.GetU
        Dim V = SVD.GetV.Transpose
        Dim E = SVD.S

        Dim Y = New GeneralMatrix((From Line As Double() In E.Array
                                   Select (From i As Double In Line
                                           Let GetValue As Double = If(i <> 0.0R, 0R, 1.0R)
                                           Select GetValue).ToArray).ToArray)
        E = New GeneralMatrix((From Line As Double() In E.Array
                               Select (From i As Double In Line
                                       Let getValue As Double = If(i = 0.0R, 0R, 1 / i)
                                       Select getValue).ToArray).ToArray)
        Dim YV = Y.ArrayMultiply(V)
        Dim UEV = U.Multiply(E)
        UEV = UEV.Multiply(V)

        Dim W = UEV.Multiply(MAT.Transpose)
        Return W
    End Function

    <ExportAPI("Write.Matrix")>
    Public Function SaveMatrix(MAT As GeneralMatrix, saveto As String) As Boolean
        Dim LQuery = (From line As Double() In MAT.Array
                      Select CType((From col In line Select CStr(col)).ToArray, DocumentStream.RowObject)).ToArray
        Call CType(LQuery, DocumentStream.File).Save(saveto, False)
        Return True
    End Function
End Module
