#Region "Microsoft.VisualBasic::aedac21e12853fbace56f6bcaf64e54d, localblast\ParallelTask\BashShell.vb"

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

    ' Module BashShell
    ' 
    '     Function: Batch, ScriptCallSave, VennBatch
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' 生成用于linux服务器上面批量运行的blast脚本
''' </summary>
''' 
<Package("NCBI.LocalBLAST.BashShell")>
Public Module BashShell

    ''' <summary>
    ''' 2. 保存脚本
    ''' </summary>
    ''' <param name="batch"></param>
    ''' <param name="outDIR"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("Bash.Caller.Save")>
    Public Function ScriptCallSave(batch As Generic.IEnumerable(Of String), outDIR As String) As Boolean
        Dim caller As StringBuilder = New StringBuilder("#!/bin/bash" & vbCrLf)
        Dim i As Integer = 1000

        For Each script As String In batch
            Dim path As String = outDIR & "/" & i & ".sh"
            Dim bash As StringBuilder = New StringBuilder("#!/bin/bash" & vbCrLf)

            Call bash.AppendLine(script)
            Call bash.SaveTo(path)
            Call caller.AppendLine("./" & FileIO.FileSystem.GetFileInfo(path).Name & " &")
        Next

        Return caller.SaveTo(outDIR & "/Invoke.sh")
    End Function

    ''' <summary>
    ''' 1. 生成两两比对的脚本调用
    ''' </summary>
    ''' <param name="inDIR"></param>
    ''' <param name="inRefAs">Linux服务器上面的引用位置</param>
    ''' <param name="outDIR"></param>
    ''' <param name="evalue"></param>
    ''' <param name="blastDIR">这个应该是linux服务器上面的位置，包含blastp+makeblastdb</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("Venn.Batch")>
    Public Function VennBatch(blastDIR As String, inDIR As String, inRefAs As String, outDIR As String, evalue As String) As String()
        Dim fastas As String() =
            FileIO.FileSystem.GetFiles(inDIR,
                                       FileIO.SearchOption.SearchTopLevelOnly,
                                       "*.fasta",
                                       "*.fa",
                                       "*.fsa",
                                       "*.fas").ToArray
        Dim LQuery = (From fa As String
                      In fastas
                      Select Batch(blastDIR,
                          query:=fa,
                          evalue:=evalue,
                          inRefAs:=inRefAs,
                          outDIR:=outDIR,
                          subject:=fastas)).ToArray
        Return LQuery
    End Function

    <ExportAPI("Batch")>
    Public Function Batch(blastDIR As String, query As String, subject As String(), inRefAs As String, outDIR As String, evalue As String) As String
        Dim script As StringBuilder = New StringBuilder
        Dim blastp As String = blastDIR & "/blastp"
        Dim makeblastdb As String = blastDIR & "/makeblastdb"

        For Each sbj As String In subject
            Dim out As String = VennDataBuilder.BuildFileName(query, sbj, outDIR)

            Call script.AppendLine($"{makeblastdb} -dbtype prot -in {sbj.CLIPath}")
            Call script.AppendLine($"{blastp} -in {query.CLIPath} -db {sbj.CLIPath} -evalue {evalue} -out {out.CLIPath}")
            Call script.AppendLine(vbCrLf)
        Next

        Return script.ToString
    End Function
End Module
