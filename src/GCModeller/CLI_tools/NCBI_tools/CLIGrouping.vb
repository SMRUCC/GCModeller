#Region "Microsoft.VisualBasic::4564670756cb119d63ceaf92a87aec23, CLI_tools\NCBI_tools\CLIGrouping.vb"

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

    ' Module CLIGrouping
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Module CLIGrouping

    Public Const TaxonomyTools$ = "NCBI taxonomy tools"
    Public Const ExportTools$ = "NCBI data export tools"
    Public Const NTTools$ = "NCBI ``nt`` database tools"
    Public Const GITools$ = "NCBI GI tools(Obsolete from NCBI, 2016-10-20)"

    Public Const GIWasObsoleted$ =
        "> https://www.ncbi.nlm.nih.gov/news/03-02-2016-phase-out-of-GI-numbers/

###### NCBI is phasing out sequence GIs - use Accession.Version instead!

As of September 2016, the integer sequence identifiers known as ""GIs"" will no longer be included in the GenBank, GenPept, and FASTA formats supported by NCBI for sequence records. The FASTA header will be further simplified to report only the sequence accession.version and record title for accessions managed by the International Sequence Database Collaboration (INSDC) and NCBI’s Reference Sequence (RefSeq) project. As NCBI makes this transition, we encourage any users who have workflows that depend on GI's to begin planning to use accession.version identifiers instead. After September 2016, any processes solely dependent on GIs will no longer function as expected."

End Module
