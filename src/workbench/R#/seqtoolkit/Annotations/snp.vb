#Region "Microsoft.VisualBasic::aa47f0b4e87eb957392a706fbf0a4d2f, R#\seqtoolkit\Annotations\snp.vb"

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

    '   Total Lines: 21
    '    Code Lines: 18 (85.71%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 3 (14.29%)
    '     File Size: 846 B


    ' Module snpTools
    ' 
    '     Function: snp_scan
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SNP
Imports SMRUCC.genomics.Analysis.SequenceTools.SNP.SangerSNPs
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("snp_toolkit")>
<RTypeExport("snp", GetType(SNP))>
Module snpTools

    <ExportAPI("snp_scan")>
    Public Function snp_scan(nt As FastaFile,
                             ref_index$,
                             Optional pureMode As Boolean = False,
                             Optional monomorphic As Boolean = False,
                             Optional ByRef vcf_output_filename$ = Nothing) As SNPsAln

        Return nt.ScanSNPs(ref_index, pureMode, monomorphic, vcf_output_filename)
    End Function
End Module

