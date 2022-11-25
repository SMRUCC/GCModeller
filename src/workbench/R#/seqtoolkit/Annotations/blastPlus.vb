#Region "Microsoft.VisualBasic::25ad5d126d70ae29a2a06be8463aa98e, R#\seqtoolkit\Annotations\blastPlus.vb"

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
    '    Code Lines: 42
    ' Comment Lines: 21
    '   Blank Lines: 14
    '     File Size: 2.59 KB


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
Imports REnv = SMRUCC.Rsharp.Runtime

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
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("makeblastdb")>
    Public Function makeblastdb([in] As String,
                                <RRawVectorArgument(GetType(String))>
                                Optional dbtype As Object = "nucl|prot",
                                Optional env As Environment = Nothing) As Object

        Dim bin As String = env.globalEnvironment.options.getOption("ncbi_blast")
        Dim seqtype As String = REnv.single(REnv.asVector(Of String)(dbtype), forceSingle:=True)
        Dim localblast = New BLASTPlus(bin).FormatDb(Db:=[in], dbType:=seqtype)
        Dim stdout As String

        localblast.Run()
        stdout = localblast.StandardOutput

        Return stdout
    End Function

    ''' <summary>
    ''' Protein-Protein BLAST
    ''' </summary>
    ''' <returns></returns>
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

    <ExportAPI("blastn")>
    Public Function blastn()

    End Function

    <ExportAPI("blastx")>
    Public Function blastx()

    End Function

End Module
