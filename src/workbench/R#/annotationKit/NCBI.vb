Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("NCBI")>
Module NCBI

    ''' <summary>
    ''' read ncbi ftp index of the genome assembly
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("genome_assembly_index")>
    <RApiReturn(GetType(FtpIndex))>
    Public Function genome_assembly_index(file As String) As Object
        Return pipeline.CreateFromPopulator(FtpIndex.LoadIndex(file))
    End Function

End Module
