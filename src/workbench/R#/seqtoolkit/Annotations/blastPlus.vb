#Region "Microsoft.VisualBasic::ff2e0e9b31319610fcf5f6ce4513772e, R#\seqtoolkit\Annotations\blastPlus.vb"

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

    '   Total Lines: 77
    '    Code Lines: 42 (54.55%)
    ' Comment Lines: 21 (27.27%)
    '    - Xml Docs: 90.48%
    ' 
    '   Blank Lines: 14 (18.18%)
    '     File Size: 2.57 KB


    ' Module blastPlusInterop
    ' 
    '     Function: blastn, blastp, blastx, makeblastdb
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

''' <summary>
''' Basic Local Alignment Search Tool
''' 
''' NCBI blast+ wrapper
''' 
''' BLAST finds regions Of similarity between biological 
''' sequences. The program compares nucleotide Or protein 
''' sequences To sequence databases And calculates the 
''' statistical significance.
''' </summary>
<Package("blast+")>
Module blastPlusInterop

    ''' <summary>
    ''' Application to create BLAST databases
    ''' </summary>
    ''' <param name="[in]">Input file/database name</param>
    ''' <param name="dbtype">Molecule type of target db</param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a character value of the standard output log message of the 
    ''' ``makeblastdb`` program run.
    ''' </returns>
    <ExportAPI("makeblastdb")>
    Public Function makeblastdb([in] As String,
                                <RRawVectorArgument(GetType(String))>
                                Optional dbtype As Object = "nucl|prot",
                                Optional env As Environment = Nothing) As Object

        Dim bin As String = env.globalEnvironment.options.getOption("ncbi_blast")
        Dim seqtype As String = CLRVector.asCharacter(dbtype).First
        Dim localblast = New BLASTPlus(bin).FormatDb(Db:=[in], dbType:=seqtype)
        Dim stdout As String

        localblast.Run()
        stdout = localblast.StandardOutput

        Return stdout
    End Function

    ''' <summary>
    ''' Protein-Protein BLAST
    ''' </summary>
    ''' <param name="query">
    ''' the file path of the query protein fasta sequence file.
    ''' </param>
    ''' <param name="subject">
    ''' the file path of the subject protein sequence database file.
    ''' </param>
    ''' <param name="output">
    ''' the file path of the blastp alignment result output file.
    ''' </param>
    ''' <param name="evalue">
    ''' the e-value threshold of the accepted blastp hits.
    ''' </param>
    ''' <param name="n_threads">
    ''' the thread number for run the blastp program.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a character value of the standard output log message of the ``blastp`` 
    ''' program run, and the alignment result data will be saved into the 
    ''' given output file.
    ''' </returns>
    <ExportAPI("blastp")>
    Public Function blastp(query As String, subject As String, output As String,
                           Optional evalue As Double = 0.001,
                           Optional n_threads As Integer = 2,
                           Optional env As Environment = Nothing) As Object

        Dim bin As String = env.globalEnvironment.options.getOption("ncbi_blast")
        Dim stdout As String
        Dim localblast = New BLASTPlus(bin) With {
            .NumThreads = n_threads
        }.Blastp(query, subject, output, evalue)

        localblast.Run()
        stdout = localblast.StandardOutput

        Return stdout
    End Function

    ''' <summary>
    ''' Nucleotide-Nucleotide BLAST
    ''' </summary>
    ''' <remarks>
    ''' this api is not implemented at this moment.
    ''' </remarks>
    <ExportAPI("blastn")>
    Public Function blastn()

    End Function

    ''' <summary>
    ''' Translated Query Vs. Protein Database
    ''' </summary>
    ''' <remarks>
    ''' this api is not implemented at this moment.
    ''' </remarks>
    <ExportAPI("blastx")>
    Public Function blastx()

    End Function

End Module
