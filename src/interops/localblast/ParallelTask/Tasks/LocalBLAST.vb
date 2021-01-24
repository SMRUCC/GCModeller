#Region "Microsoft.VisualBasic::ac9d0c436c61f393cece7a1d71cc22e4, localblast\LocalBLAST\Tasks\LocalBLAST.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Module LocalBLAST
    ' 
    '         Function: BLAST, runBlast
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.InteropService

Namespace Tasks

    Public Module LocalBLAST

        Private Function runBlast(File1 As String,
                                 File2 As String,
                                 Idx As Integer,
                                 logDIR As String,
                                 LocalBlast As InteropService) As String   '匿名函数返回日志文件名

            Dim LogFile As String = VennDataBuilder.BuildFileName(File1, File2, logDIR)

            Call $"[{File1}, {File2}]".__DEBUG_ECHO
            Call LocalBlast.Blastp(File1, File2, LogFile, e:="1").Start(waitForExit:=True) 'performence the BLAST

            Return LogFile
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="paths"></param>
        ''' <param name="logDIR">默认为桌面</param>
        ''' <returns>日志文件列表</returns>
        ''' <remarks></remarks>
        Public Function BLAST(paths As String(), logDIR As String, parms As InitializeParameter) As List(Of QueryPair())
            Dim files As Comb(Of String) = paths
            Dim localBlast As InteropService = CreateInstance(parms)
            Dim DIR_index As i32 = 1
            Dim ReturnedList As New List(Of QueryPair())

            For Each File As String In paths  'formatdb
                Call localBlast.FormatDb(File, "").Start(waitForExit:=True)
            Next

            For Each list As Tuple(Of String, String)() In files.CombList
                Dim DIR As String = String.Format("{0}/{1}/", logDIR, ++DIR_index)
                Dim index As i32 = 1
                Dim logPairList As New List(Of QueryPair)

                Call DIR.MkDIR

                For i As Integer = 0 To list.Count - 1
                    Dim Log1 As String = runBlast(list(i).Item1, list(i).Item2, ++index, logDIR, localBlast)
                    Dim Log2 As String = runBlast(list(i).Item1, list(i).Item2, ++index, logDIR, localBlast)

                    logPairList += New QueryPair With {
                        .Query = Log1,
                        .Target = Log2
                    }
                Next

                Call ReturnedList.Add(logPairList.ToArray)
            Next

            Return ReturnedList
        End Function
    End Module
End Namespace
