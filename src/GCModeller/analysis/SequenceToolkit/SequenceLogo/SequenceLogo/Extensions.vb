#Region "Microsoft.VisualBasic::6a2fac54992df5517fb787df88db6fcd, GCModeller\analysis\SequenceToolkit\SequenceLogo\SequenceLogo\Extensions.vb"

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

    '   Total Lines: 40
    '    Code Lines: 34
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 1.51 KB


    '     Module Extensions
    ' 
    '         Function: CreateDrawingModel
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports ScannerMotif = SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.SequenceMotif

Namespace SequenceLogo

    <HideModuleName>
    Public Module Extensions

        <Extension>
        Public Function CreateDrawingModel(motif As ScannerMotif) As DrawingModel
            Dim n% = motif.seeds.MSA.Length
            Dim E# = Probability.E(n)
            Dim alphas As Residue() = motif _
                .region _
                .Select(Function(r)
                            Dim nt As New Residue With {
                                .Alphabets = r.frequency _
                                    .Select(Function(b)
                                                Return New Alphabet With {
                                                    .Alphabet = b.Key,
                                                    .RelativeFrequency = b.Value
                                                }
                                            End Function) _
                                    .ToArray,
                                .Position = r.index
                            }

                            nt.Bits = Probability.CalculatesBits(nt.Hi, En:=E, NtMol:=True)

                            Return nt
                        End Function) _
                .ToArray

            Return New DrawingModel With {
                .Residues = alphas,
                .En = E
            }
        End Function
    End Module
End Namespace
