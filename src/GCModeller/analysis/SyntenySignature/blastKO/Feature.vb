#Region "Microsoft.VisualBasic::7dd5517b5d599dcecd38b0fec64d37a2, ..\GCModeller\analysis\SyntenySignature\blastKO\Feature.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports SMRUCC.genomics.ComponentModel.Loci

''' <summary>
''' Feature site on a nucleotide sequence:
''' 
''' + KO: KEGG orthology
''' + motif: KO cluster regulation
''' </summary>
Public Class Feature

    Public Property Type As FeatureTypes
    Public Property Location As NucleotideLocation
    Public Property Name As String

End Class

Public Enum FeatureTypes
    ''' <summary>
    ''' Unknown
    ''' </summary>
    NA
    KO
    Motifs
End Enum
