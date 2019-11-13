#Region "Microsoft.VisualBasic::828f0bb3d0f8aa9d12e0a91e43695418, core\Bio.Assembly\Assembly\NCBI\Database\GenBank\TabularFormat\FeatureBriefs\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: AsGenes, COGs, ToGeneBrief
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels

Namespace Assembly.NCBI.GenBank.TabularFormat

    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function COGs(genes As IEnumerable(Of GeneBrief)) As IEnumerable(Of String)
            Return From gene In genes Select gene.COG
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function AsGenes(features As IEnumerable(Of GFF.Feature)) As IEnumerable(Of GeneBrief)
            Return features _
                .SafeQuery _
                .Select(Function(feature) feature.ToGeneBrief)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ToGeneBrief(feature As GFF.Feature) As GeneBrief
            Return New GeneBrief With {
                .Code = feature.ID,
                .COG = feature.COG,
                .Gene = feature.attributes.TryGetValue("locus_tag") Or feature.ProteinId.AsDefault,
                .IsORF = True,
                .Length = feature.Length,
                .Location = feature.Location,
                .PID = feature.ProteinId,
                .Product = feature.Product,
                .Synonym = feature.Synonym
            }
        End Function
    End Module
End Namespace
