Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace ModularNetwork.WGCNA

    ''' <summary>
    ''' WGCNA module block extensions
    ''' </summary>
    Public Module WGCNAFiles

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="gene_module_assignment">file path of the csv table file of WGCNA gene module color assignment result</param>
        ''' <returns></returns>
        Public Function ReadModuleAssignment(gene_module_assignment As String) As GeneModuleColor()
            Return gene_module_assignment.LoadCsv(Of GeneModuleColor)(mute:=True)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="module_eigengene_correlation">file path of the csv table file of WGCNA module correlation matrix</param>
        ''' <returns>
        ''' 可以直接通过模块颜色名称进行相关性访问的矩阵对象，例如：
        ''' 
        ''' ```vbnet
        ''' Dim cor As Double = m("black", "white")
        ''' ```
        ''' </returns>
        Public Function ReadModuleEigengeneCorrelation(module_eigengene_correlation As String) As DataMatrix
            Dim df As DataFrameResolver = DataFrameResolver.Load(module_eigengene_correlation)
            Dim colors As New List(Of String)
            Dim mat As New List(Of Double())

            Do While df.Read
                Dim row As String() = df.GetRow

                Call colors.Add(row(0))
                Call mat.Add(row.Skip(1).AsDouble)
            Loop

            Return New DataMatrix(colors, mat)
        End Function
    End Module
End Namespace