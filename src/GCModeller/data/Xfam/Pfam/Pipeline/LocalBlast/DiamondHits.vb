#Region "Microsoft.VisualBasic::9e376fe0b7f069ec452414e47e2934ba, data\Xfam\Pfam\Pipeline\LocalBlast\DiamondHits.vb"

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

    '   Total Lines: 30
    '    Code Lines: 27 (90.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 3 (10.00%)
    '     File Size: 1.11 KB


    '     Module DiamondHits
    ' 
    '         Function: Parse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Interops.NCBI.Extensions

Namespace Pipeline.LocalBlast

    Public Module DiamondHits

        <Extension>
        Public Iterator Function Parse(blastp As IEnumerable(Of DiamondAnnotation)) As IEnumerable(Of PfamHit)
            For Each hit As DiamondAnnotation In blastp
                Yield New PfamHit With {
                    .description = hit.QseqId,
                    .HitName = hit.QseqId,
                    .QueryName = hit.SseqId,
                    .start = hit.SStart,
                    .ends = hit.SEnd,
                    .evalue = hit.EValue,
                    .identities = hit.Pident,
                    .score = hit.BitScore,
                    .hit_length = hit.Length,
                    .length_hit = hit.Length,
                    .length_hsp = hit.Length,
                    .length_query = hit.Length,
                    .positive = 1,
                    .query_length = hit.Length
                }
            Next
        End Function
    End Module
End Namespace
