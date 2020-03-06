
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime
Imports REnv = SMRUCC.Rsharp.Runtime
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

''' <summary>
''' A pipeline collection for proteins' biological function 
''' annotation based on the sequence alignment.
''' </summary>
<Package("annotation.workflow", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
Module workflows

    <ExportAPI("blasthit.sbh")>
    Public Function ExportSBHHits(blasthits As Object, Optional idetities As Double = 0.3, Optional coverage As Double = 0.5) As pipeline

    End Function

    <ExportAPI("open.stream")>
    Public Function openWriter(file As String,
                               Optional type As TableTypes = TableTypes.SBH,
                               Optional encoding As Encodings = Encodings.ASCII,
                               Optional env As Environment = Nothing) As Object
        Select Case type
            Case TableTypes.SBH
                Return New WriteStream(Of BestHit)(file, encoding:=encoding)
            Case TableTypes.BBH
                Return New WriteStream(Of BiDirectionalBesthit)(file, encoding:=encoding)
            Case Else
                Return REnv.Internal.debug.stop($"Invalid stream formatter: {type.ToString}", env)
        End Select
    End Function
End Module

Public Enum TableTypes
    SBH
    BBH
End Enum