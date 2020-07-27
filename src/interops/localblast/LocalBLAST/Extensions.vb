#Region "Microsoft.VisualBasic::fe27c4212a2e18dde061145c3af5762e, localblast\LocalBLAST\Extensions.vb"

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

    ' Module Extensions
    ' 
    '     Function: (+2 Overloads) BlastpSearch, IsAvailable, IsNullOrEmpty
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Extensions

    ReadOnly __ends As String() = {"Matrix:", "Gap Penalties:", "Neighboring words threshold:", "Window for multiple hits:"}

    ''' <summary>
    ''' Determine that is this blast result file is completed and integrated based on the ends of the result text file.
    ''' (根据文件末尾的结束标示来判断这个blast操作是否是已经完成了的)
    ''' </summary>
    ''' <param name="path">The file path of the blast output result text file.</param>
    ''' <returns></returns>
    Public Function IsAvailable(path$) As Boolean
        If Not path.FileExists Then
            Return False
        End If

        Dim i As Integer
        Dim last As String = path.Tails(2048)

        For Each word As String In __ends
            If InStr(last, word, CompareMethod.Text) > 0 Then
                i += 1
            End If
            If i >= 2 Then
                Return True
            End If
        Next

        Return i >= 2
    End Function

    ''' <summary>
    ''' Is this collection of the besthit data is empty or nothing?
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    <Extension> Public Function IsNullOrEmpty(data As IEnumerable(Of BestHit)) As Boolean
        If data.IsNullOrEmpty Then
            Return True
        End If

        Dim notNull As BestHit = (From bh As BestHit
                                  In data
                                  Where Not bh.isMatched
                                  Select bh).FirstOrDefault
        Return notNull Is Nothing
    End Function

    Public Const FastaIsNotProtein$ = "Target fasta sequence file is not a protein sequence data file!"

    ''' <summary>
    ''' Invoke the blastp search for the target protein fasta sequence.(对目标蛋白质序列进行Blastp搜索)
    ''' </summary>
    ''' <param name="Query"></param>
    ''' <param name="Subject"></param>
    ''' <param name="evalue"></param>
    ''' <param name="Blastbin">If the services handler is nothing then the function will construct a new handle automatically.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function BlastpSearch(Query As FastaSeq, Subject As String,
                                             Optional Evalue As String = "1e-3",
                                             Optional ByRef Blastbin As LocalBLAST.InteropService.InteropService = Nothing) As BlastPlus.v228

        If Not Query.IsProtSource Then
            Call VBDebugger.PrintException(FastaIsNotProtein)
            Return Nothing
        End If

        Dim tmpQuery As String = App.GetAppSysTempFile
        Dim tmpOut As String = App.GetAppSysTempFile

        If Blastbin Is Nothing Then Blastbin = NCBILocalBlast.CreateSession

        Call Query.SaveTo(tmpQuery)

        Call Blastbin.FormatDb(Subject, Blastbin.MolTypeProtein).Start(True)
        Call Blastbin.Blastp(tmpQuery, Subject, tmpOut, Evalue).Start(True)

        Return Blastbin.GetLastLogFile
    End Function

    <Extension> Public Function BlastpSearch(Query As FastaFile, Subject As String,
                                             Optional evalue As String = "1e-3",
                                             Optional ByRef Blastbin As LocalBLAST.InteropService.InteropService = Nothing) As BlastPlus.v228

        Dim tmpQuery As String = App.GetAppSysTempFile(, App.PID)

        If Blastbin Is Nothing Then Blastbin = NCBILocalBlast.CreateSession

        Call Query.Save(tmpQuery)

        Call Blastbin.FormatDb(Subject, Blastbin.MolTypeProtein).Start(True)
        Call Blastbin.Blastp(tmpQuery, Subject, App.GetAppSysTempFile() & "/blastp.log", evalue).Start(True)

        Return Blastbin.GetLastLogFile
    End Function
End Module
