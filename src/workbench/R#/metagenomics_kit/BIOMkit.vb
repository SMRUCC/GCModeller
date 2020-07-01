Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.foundation
Imports SMRUCC.genomics.foundation.BIOM.v10
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' the BIOM file toolkit
''' </summary>
<Package("BIOM_kit")>
Public Module BIOMkit

    ''' <summary>
    ''' read matrix data from a given BIOM file.
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("read.matrix")>
    <RApiReturn(GetType(BIOMDataSet(Of Double)))>
    Public Function readMatrix(file As Object,
                               Optional denseMatrix As Boolean = True,
                               Optional env As Environment = Nothing) As Object

        If file Is Nothing Then
            Return Internal.debug.stop("the given file can not be nothing!", env)
        ElseIf TypeOf file Is String Then
            If DirectCast(file, String).FileExists Then
                Return BIOM.ReadAuto(file, denseMatrix:=denseMatrix)
            Else
                Return Internal.debug.stop("the given file is not found on your filesystem!", env)
            End If
        Else
            Return Internal.debug.stop(Message.InCompatibleType(GetType(String), file.GetType, env), env)
        End If
    End Function
End Module
