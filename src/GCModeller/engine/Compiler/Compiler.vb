Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports SMRUCC.genomics.Metagenomics

Public Module Workflow

    ''' <summary>
    ''' 输出Model，然后再从Model写出模型文件
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="KOfunction"></param>
    ''' <param name="repo"></param>
    ''' <returns></returns>
    <Extension>
    Public Function AssemblingModel(genome As GBFF.File, KOfunction As Dictionary(Of String, String), repo As RepositoryArguments) As CellularModule
        Dim taxonomy As Taxonomy = 
    End Function
End Module
