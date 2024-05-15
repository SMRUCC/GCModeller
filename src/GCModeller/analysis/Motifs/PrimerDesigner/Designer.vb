#Region "Microsoft.VisualBasic::daa7e5308ec98c5e309de3f38405493b, analysis\Motifs\PrimerDesigner\Designer.vb"

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
    '    Code Lines: 11
    ' Comment Lines: 16
    '   Blank Lines: 3
    '     File Size: 1.75 KB


    ' Module Designer
    ' 
    '     Function: Ratings, Search
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

''' <summary>
''' Wu, J. S., et al. (2004). "Primer design using genetic algorithm." Bioinformatics 20(11): 1710-1717.
''' 
''' MOTIVATION: Before performing a polymerase chain reaction experiment, a pair of primers to clip 
''' the target DNA subsequence is required. However, this is a tedious task as too many constraints 
''' need to be satisfied. Various kinds of approaches for designing a primer have been proposed in 
''' the last few decades, but most of them do not have restriction sites on the designed primers and 
''' do not satisfy the specificity constraint. RESULTS: The proposed algorithm imitates nature's 
''' process of evolution and genetic operations on chromosomes in order to achieve optimal solutions, 
''' and is a best fit for DNA behavior. Experimental results indicate that the proposed algorithm can 
''' find a pair of primers that not only obeys the design properties but also has a specific 
''' restriction site and specificity. Gel electrophoresis verifies that the proposed method really 
''' can clip out the target sequence. AVAILABILITY: A public version of the software is available on 
''' request from the authors.
''' </summary>
''' <remarks></remarks>
Public Module Designer

    Public Function Search(minL As Integer, maxL As Integer, Optional sense_rs As String = "", Optional antisense_rs As String = "") As Primer()
        Throw New NotImplementedException
    End Function

    Public Function Ratings(Primer As Primer, Profiles As SearchProfile) As Double
        Throw New NotImplementedException
    End Function
End Module
