#Region "Microsoft.VisualBasic::552982f6babdc249d2c7fec11e1b637f, ..\localblast\LocalBLAST\Analysis\LocalBLAST.vb"

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
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
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
            Dim DIR_index As int = 1
            Dim ReturnedList As New List(Of QueryPair())

            For Each File As String In paths  'formatdb
                Call localBlast.FormatDb(File, "").Start(waitForExit:=True)
            Next

            For Each List In files.CombList
                Dim DIR As String = String.Format("{0}/{1}/", logDIR, ++DIR_index)
                Dim index As int = 1
                Dim logPairList As New List(Of QueryPair)

                Call DIR.MkDIR

                For i As Integer = 0 To List.Count - 1
                    Dim Log1 As String = __blast(List(i).Item1, List(i).Item2, ++index, logDIR, localBlast)
                    Dim Log2 As String = __blast(List(i).Item1, List(i).Item2, ++index, logDIR, localBlast)

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
