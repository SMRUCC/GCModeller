﻿#Region "Microsoft.VisualBasic::a0146d1e83116b19229119d1b5bef975, G:/GCModeller/src/interops/localblast/ParallelTask//Extensions.vb"

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


    ' Code Statistics:

    '   Total Lines: 86
    '    Code Lines: 51
    ' Comment Lines: 15
    '   Blank Lines: 20
    '     File Size: 3.64 KB


    ' Module Extensions
    ' 
    '     Function: (+2 Overloads) BlastpSearch, IsAvailable
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports SMRUCC.genomics.Interops.NCBI.Extensions
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

        Dim tmpQuery As String = TempFileSystem.GetAppSysTempFile
        Dim tmpOut As String = TempFileSystem.GetAppSysTempFile

        If Blastbin Is Nothing Then Blastbin = NCBILocalBlast.CreateSession

        Call Query.SaveTo(tmpQuery)

        Call Blastbin.FormatDb(Subject, Blastbin.MolTypeProtein).Start(True)
        Call Blastbin.Blastp(tmpQuery, Subject, tmpOut, Evalue).Start(True)

        Return Blastbin.GetLastLogFile
    End Function

    <Extension> Public Function BlastpSearch(Query As FastaFile, Subject As String,
                                             Optional evalue As String = "1e-3",
                                             Optional ByRef Blastbin As LocalBLAST.InteropService.InteropService = Nothing) As BlastPlus.v228

        Dim tmpQuery As String = TempFileSystem.GetAppSysTempFile(, App.PID)

        If Blastbin Is Nothing Then Blastbin = NCBILocalBlast.CreateSession

        Call Query.Save(tmpQuery)

        Call Blastbin.FormatDb(Subject, Blastbin.MolTypeProtein).Start(True)
        Call Blastbin.Blastp(tmpQuery, Subject, TempFileSystem.GetAppSysTempFile() & "/blastp.log", evalue).Start(True)

        Return Blastbin.GetLastLogFile
    End Function
End Module
