#Region "Microsoft.VisualBasic::810a6fc322204ca6732aa34443222f7a, R#\metagenomics_kit\gast.vb"

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

    '   Total Lines: 89
    '    Code Lines: 60
    ' Comment Lines: 21
    '   Blank Lines: 8
    '     File Size: 3.48 KB


    ' Module gastTools
    ' 
    '     Function: OTUgreengenesTaxonomy, ParseGreengenesTaxonomy, ParseMothurOTUs
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Analysis.Metagenome.greengenes
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' gast 16s data analysis tools, combine with the mothur workflow
''' </summary>
<Package("gast")>
Module gastTools

    ''' <summary>
    ''' assign OTU its taxonomy result
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("OTU.taxonomy")>
    Public Function OTUgreengenesTaxonomy(<RRawVectorArgument>
                                          blastn As Object,
                                          OTUs As list,
                                          taxonomy As list,
                                          Optional min_pct# = 0.97,
                                          Optional gast_consensus As Boolean = False,
                                          Optional env As Environment = Nothing) As pipeline

        Dim queries As pipeline = pipeline.TryCreatePipeline(Of Query)(blastn, env)

        If queries.isError Then
            Return queries
        End If

        Dim OTUTable = OTUs.AsGeneric(Of NamedValue(Of Integer))(env)
        Dim taxonomyTable = taxonomy.AsGeneric(Of otu_taxonomy)(env)

        If gast_consensus Then
            Return queries _
                .populates(Of Query)(env) _
                .OTUgreengenesTaxonomy(OTUTable, taxonomyTable, min_pct) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        Else
            Return queries _
                .populates(Of Query)(env) _
                .OTUgreengenesTaxonomyTreeAssign(OTUTable, taxonomyTable, min_pct) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        End If
    End Function

    ''' <summary>
    ''' Parse the greengenes taxonomy file
    ''' </summary>
    ''' <param name="tax">
    ''' the file path of the greengenes taxonomy mapping file.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("parse.greengenes_tax")>
    Public Function ParseGreengenesTaxonomy(tax As String) As list
        Return otu_taxonomy _
            .Load(path:=tax) _
            .ToDictionary(Function(d) d.ID,
                          Function(d)
                              Return CObj(d)
                          End Function)
    End Function

    ''' <summary>
    ''' parse the OTU data file
    ''' </summary>
    ''' <param name="OTU_rep_fasta">
    ''' ``OTU.rep.fasta`` query source comes from the mothur workflow.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("parse.mothur_OTUs")>
    Public Function ParseMothurOTUs(OTU_rep_fasta As String, Optional removes_lt As Double = 0.0001) As list
        Return StreamIterator _
            .SeqSource(OTU_rep_fasta) _
            .ParseOTUrep() _
            .RemovesOTUlt(cutoff:=removes_lt) _
            .ToDictionary(Function(d) d.Key,
                          Function(d)
                              Return CObj(d.Value)
                          End Function)
    End Function
End Module
