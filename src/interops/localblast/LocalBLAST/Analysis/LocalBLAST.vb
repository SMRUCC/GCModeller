#Region "Microsoft.VisualBasic::8002b52cfb14b76c0dd37520d21bdf36, ..\interops\localblast\LocalBLAST\Analysis\LocalBLAST.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.InteropService

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
