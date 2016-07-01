Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.InteropService
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BatchParallel

Namespace Analysis

    Public Module LocalBLAST

        Private Function __blast(File1 As String,
                                 File2 As String,
                                 Idx As Integer,
                                 logDIR As String,
                                 LocalBlast As InteropService) As String   '匿名函数返回日志文件名

            Dim LogFile As String = VennDataBuilder.BuildFileName(File1, File2, logDIR)

            Call $"[{File1}, {File2}]".__DEBUG_ECHO
            Call LocalBlast.Blastp(File1, File2, LogFile, e:="1").Start(WaitForExit:=True) 'performence the BLAST

            Return LogFile
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="lstFiles"></param>
        ''' <param name="LogDIR">默认为桌面</param>
        ''' <returns>日志文件列表</returns>
        ''' <remarks></remarks>
        Public Function BLAST(lstFiles As String(), LogDIR As String, pBlast As InitializeParameter) As List(Of Pair())
            Dim Files As Comb(Of String) = lstFiles
            Dim LocalBlast As InteropService = CreateInstance(pBlast)
            Dim DirIndex As Integer = 1
            Dim ReturnedList As New List(Of Pair())

            For Each File As String In lstFiles  'formatdb
                Call LocalBlast.FormatDb(File, "").Start(WaitForExit:=True)
            Next

            For Each List In Files.CombList
                Dim Dir As String = String.Format("{0}/{1}/", LogDIR, DirIndex)
                Dim Index As Integer = 1
                Dim LogPairList As List(Of Pair) = New List(Of Pair)

                DirIndex += 1
                Call FileIO.FileSystem.CreateDirectory(directory:=Dir)

                For i As Integer = 0 To List.Count - 1
                    Dim Log1 As String = __blast(List(i).Key, List(i).Value, Index, LogDIR, LocalBlast)
                    Index += 1
                    Dim Log2 As String = __blast(List(i).Value, List(i).Key, Index, LogDIR, LocalBlast)
                    Index += 1
                    LogPairList += New Pair With {.File1 = Log1, .File2 = Log2}
                Next

                Call ReturnedList.Add(LogPairList.ToArray)
            Next

            Return ReturnedList
        End Function
    End Module
End Namespace