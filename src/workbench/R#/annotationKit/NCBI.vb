#Region "Microsoft.VisualBasic::cd10aa7e705b0b1e54584aa8f194f0f5, R#\annotationKit\NCBI.vb"

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

    '   Total Lines: 20
    '    Code Lines: 13 (65.00%)
    ' Comment Lines: 4 (20.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 3 (15.00%)
    '     File Size: 627 B


    ' Module NCBI
    ' 
    '     Function: genome_assembly_index
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("NCBI")>
Module NCBI

    ''' <summary>
    ''' read ncbi ftp index of the genome assembly
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("genome_assembly_index")>
    <RApiReturn(GetType(GenBankAssemblyIndex))>
    Public Function genome_assembly_index(file As String, Optional make_accession_index As Boolean = False) As Object
        Dim summary As IEnumerable(Of GenBankAssemblyIndex) = GenBankAssemblyIndex.LoadIndex(file)

        If make_accession_index Then
            Dim asmIndex As New Dictionary(Of String, Object)

            For Each asm As GenBankAssemblyIndex In summary
                asmIndex(asm.assembly_accession.Split("."c).First) = asm
            Next

            Return New list With {.slots = asmIndex}
        Else
            Return pipeline.CreateFromPopulator(summary)
        End If
    End Function

    ''' <summary>
    ''' load the index db
    ''' </summary>
    ''' <param name="repo_dir"></param>
    ''' <param name="qgram"></param>
    ''' <returns></returns>
    <ExportAPI("genbank_assemblyDb")>
    Public Function genbank_assemblyDb(repo_dir As String, Optional qgram As Integer = 6, Optional in_memory As Boolean = True) As AssemblySummaryGenbank
        Return New AssemblySummaryGenbank(qgram, repo_dir, in_memory:=in_memory)
    End Function

    ''' <summary>
    ''' create the index db
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="repo_dir"></param>
    <ExportAPI("create_assemblyDb")>
    Public Sub create_indexdb(file As String, repo_dir As String)
        Call AssemblySummaryGenbank.CreateRepository(file, repo_dir)
    End Sub

    ''' <summary>
    ''' make data subset via a given collection of the acession id
    ''' </summary>
    ''' <param name="repo"></param>
    ''' <param name="accession_ids"></param>
    ''' <returns></returns>
    <ExportAPI("index_subset")>
    Public Function index_subset(repo As AssemblySummaryGenbank, <RRawVectorArgument> accession_ids As Object) As Object
        Return (From asm_id As String
                In CLRVector.safeCharacters(accession_ids)
                Let asm As GenBankAssemblyIndex = repo.GetByAccessionId(asm_id)
                Where Not asm Is Nothing
                Select asm).ToArray
    End Function

    ''' <summary>
    ''' make the in-memory assembly summary database query by the organism name matches
    ''' </summary>
    ''' <param name="db"></param>
    ''' <param name="q"></param>
    ''' <param name="cutoff"></param>
    ''' <returns>
    ''' a vector of <see cref="GenBankAssemblyIndex"/>. and this vector data has the attribute data 
    ''' of query ``index`` result with clr type <see cref="FindResult"/>.
    ''' </returns>
    <ExportAPI("query")>
    <RApiReturn(GetType(GenBankAssemblyIndex))>
    Public Function find(db As AssemblySummaryGenbank, <RRawVectorArgument(TypeCodes.string)> q As Object,
                         Optional cutoff As Double = 0.8,
                         Optional best_match As Boolean = False) As Object

        Dim q_str As String() = CLRVector.asCharacter(q)

        If best_match Then
            Return q_str.SafeQuery _
                .Select(Function(query)
                            Return db.GetBestMatch(query, cutoff)
                        End Function) _
                .ToArray
        Else
            Dim output As New List(Of GenBankAssemblyIndex)
            Dim indexOut As New List(Of FindResult)

            For Each query As String In q_str.SafeQuery
                Dim index = db.Query(q, cutoff).ToArray
                Dim result As GenBankAssemblyIndex() = index _
                    .Select(Function(i)
                                Return db.GetByAccessionId(i.genome.accession_id)
                            End Function) _
                    .ToArray
                Dim matches = index.Select(Function(i) i.match).ToArray

                Call output.AddRange(result)
                Call indexOut.AddRange(matches)
            Next

            Dim vec As New vector(output.ToArray, RType.GetRSharpType(GetType(GenBankAssemblyIndex)))
            Call vec.setAttribute("index", indexOut.ToArray)
            Return vec
        End If
    End Function

End Module
