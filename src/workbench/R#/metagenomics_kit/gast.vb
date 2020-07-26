#Region "Microsoft.VisualBasic::5e7f50e9bea5713d08aaadcc6499f575, metagenomics_kit\gast.vb"

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

    ' Module gastTools
    ' 
    '     Function: OTUgreengenesTaxonomy
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome.greengenes
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

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
                                          Optional env As Environment = Nothing) As pipeline

        Dim queries As pipeline = pipeline.TryCreatePipeline(Of Query)(blastn, env)

        If queries.isError Then
            Return queries
        End If

        Dim OTUTable = OTUs.AsGeneric(Of NamedValue(Of Integer))(env)
        Dim taxonomyTable = taxonomy.AsGeneric(Of otu_taxonomy)(env)

        Return queries _
            .populates(Of Query)(env) _
            .OTUgreengenesTaxonomy(OTUTable, taxonomyTable, min_pct) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function
End Module
