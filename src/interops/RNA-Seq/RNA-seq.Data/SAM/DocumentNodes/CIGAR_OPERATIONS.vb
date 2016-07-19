#Region "Microsoft.VisualBasic::66c7ba7d32df7216874a8dab2469e1ef, ..\interops\RNA-Seq\RNA-seq.Data\SAM\DocumentNodes\CIGAR_OPERATIONS.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
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

Imports System.ComponentModel
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace SAM

    ''' <summary>
    ''' 
    ''' </summary>
    Public Enum CIGAR_OPERATIONS As Integer
        ''' <summary>
        ''' alignment match (can be a sequence match Or mismatch)
        ''' </summary>
        M = 0
        ''' <summary>
        ''' insertion To the reference
        ''' </summary>
        I = 1
        ''' <summary>
        ''' deletion from the reference
        ''' </summary>
        D = 2
        ''' <summary>
        ''' skipped region from the reference
        ''' 
        ''' For mRNA -To -genome alignment, an N operation represents an intron. For other types Of alignments, the interpretation of N Is Not defined.
        ''' </summary>
        N = 3
        ''' <summary>
        ''' soft clipping (clipped sequences present In SEQ)
        ''' 
        ''' S may only have H operations between them And the ends Of the CIGAR String.
        ''' </summary>
        S = 4
        ''' <summary>
        ''' hard clipping (clipped sequences Not present In SEQ)
        ''' 
        ''' H can only be present As the first And/Or last operation.
        ''' </summary>
        H = 5
        ''' <summary>
        ''' padding (silent deletion from padded reference)
        ''' </summary>
        P = 6
        ''' <summary>
        ''' sequence match
        ''' </summary>
        EQ = 7
        ''' <summary>
        ''' sequence mismatch
        ''' </summary>
        X = 8
    End Enum
End Namespace
