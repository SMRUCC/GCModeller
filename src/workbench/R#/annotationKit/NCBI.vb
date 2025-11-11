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
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("NCBI")>
Module NCBI

    ''' <summary>
    ''' read ncbi ftp index of the genome assembly
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("genome_assembly_index")>
    <RApiReturn(GetType(GenBankAssemblyIndex))>
    Public Function genome_assembly_index(file As String) As Object
        Return pipeline.CreateFromPopulator(GenBankAssemblyIndex.LoadIndex(file))
    End Function

    <ExportAPI("genbank_assemblyDb")>
    Public Function genbank_assemblyDb(file As String, Optional qgram As Integer = 6) As AssemblySummaryGenbank
        Return New AssemblySummaryGenbank(qgram).LoadIntoMemory(file)
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
    Public Function find(db As AssemblySummaryGenbank, q As String, Optional cutoff As Double = 0.8) As Object
        Dim index As FindResult() = db.Query(q, cutoff).ToArray
        Dim result As GenBankAssemblyIndex() = index.Select(Function(i) db(i)).ToArray
        Dim vec As New vector(result, RType.GetRSharpType(GetType(GenBankAssemblyIndex)))
        vec.setAttribute("index", index)
        Return vec
    End Function

End Module
