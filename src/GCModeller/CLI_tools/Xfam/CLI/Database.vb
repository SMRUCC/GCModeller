#Region "Microsoft.VisualBasic::523ad0abbbc1b10c5a95e874424bdf54, CLI_tools\Xfam\CLI\Database.vb"

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

    ' Module CLI
    ' 
    '     Function: InstallRfam, ParseFastaAsTable
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Data.Xfam.Pfam.Pipeline.Database
Imports SMRUCC.genomics.Data.Xfam.Rfam
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    ''' <summary>
    ''' 包括文件复制以及FormatDb
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--Install.Rfam", Usage:="--Install.Rfam /seed <rfam.seed>")>
    Public Function InstallRfam(args As CommandLine) As Integer
        Dim input As String = args("/seed")
        Dim respo As String = GCModeller.FileSystem.Xfam.Rfam.Rfam
        Dim seedDb As Dictionary(Of String, Stockholm) = ReadDb(input)
        Dim outDIR As String = GCModeller.FileSystem.Xfam.Rfam.RfamFasta
        Dim out As String = respo & "/Rfam.Csv"

        For Each rfam As KeyValuePair(Of String, Stockholm) In seedDb
            Dim path As String = outDIR & $"/{rfam.Key}.fasta"
            Call rfam.Value.Alignments.Save(-1, path, Encodings.ASCII)
        Next

        Return seedDb.Values.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Fasta2Table")>
    <Usage("/Fasta2Table /in <database.fasta> [/out <table.csv>]")>
    <Description("Parse and save the pfam sequence fasta database as csv table file. (Debug used only)")>
    Public Function ParseFastaAsTable(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.table.csv"

        Using reader As New StreamIterator([in]), table As New WriteStream(Of PfamFasta)(out)
            For Each fa As FastaSeq In reader.ReadStream
                Call PfamFasta.CreateObject(fa).DoCall(AddressOf table.Flush)
            Next
        End Using

        Return 0
    End Function
End Module
