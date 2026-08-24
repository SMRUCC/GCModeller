#Region "Microsoft.VisualBasic::586d7e2b6351d2e748a3d66de4780dc3, annotations\Bifrost\MetaEuk\MetaEukConfig.vb"

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

    '   Total Lines: 33
    '    Code Lines: 17 (51.52%)
    ' Comment Lines: 8 (24.24%)
    '    - Xml Docs: 12.50%
    ' 
    '   Blank Lines: 8 (24.24%)
    '     File Size: 1.63 KB


    ' Class MetaEukConfig
    ' 
    '     Properties: AlignmentBandWidth, ContigsFile, EvalueThreshold, GapPenaltyLambda, MaxFragmentLength
    '                 MaxIntronLength, MinExonOverlapFraction, MinExonScore, MinFragmentLength, MinIdentity
    '                 NumThreads, OutputPrefix, OverlapBpThreshold, ReferenceFile, Verbose
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>Algorithm parameters controllable via command-line</summary>
Public Class MetaEukConfig

    ' --- Input/Output ---
    Public Property ContigsFile As String = ""
    Public Property ReferenceFile As String = ""
    Public Property OutputPrefix As String = "metaeuk_out"

    ' --- Fragment Generation ---
    Public Property MinFragmentLength As Integer = 15        ' minimum amino acids per candidate fragment
    Public Property MaxFragmentLength As Integer = 5000      ' maximum amino acids per candidate fragment

    ' --- Homology Search ---
    Public Property EvalueThreshold As Double = 0.001         ' E-value cutoff for significant hits
    Public Property MinIdentity As Double = 0.2              ' minimum sequence identity fraction
    Public Property AlignmentBandWidth As Integer = 32       ' band width for Smith-Waterman

    ' --- Dynamic Programming ---
    Public Property GapPenaltyLambda As Double = 0.5         ' gap penalty coefficient per AA of intron
    Public Property MaxIntronLength As Integer = 50000       ' maximum intron length in bp
    Public Property MinExonScore As Double = 20.0            ' minimum bitscore for an exon to be considered

    ' --- Redundancy Removal ---
    Public Property MinExonOverlapFraction As Double = 0.3   ' fraction overlap to consider exons shared

    ' --- Conflict Resolution ---
    Public Property OverlapBpThreshold As Integer = 10       ' bp overlap to trigger conflict resolution

    ' --- Performance ---
    Public Property Verbose As Boolean = False
    Public Property NumThreads As Integer = 4

End Class
