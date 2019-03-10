﻿#Region "Microsoft.VisualBasic::b2b189fd528bab6c1c65bc17f3885197, analysis\SequenceToolkit\SequencePatterns\SequenceLogo\Extensions.vb"

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
    '         Function: CreateDrawingModel
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports ScannerMotif = SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.Motif

Namespace SequenceLogo

    Public Module Extensions

        <Extension>
        Public Function CreateDrawingModel(motif As ScannerMotif) As DrawingModel
            Dim n% = motif.seeds.MSA.Length
            Dim E# = (1 / Math.Log(2)) * ((4 - 1) / (2 * n))

            Return New DrawingModel With {
                .Residues = motif _
                    .region _
                    .Select(Function(r)
                                Return New Residue With {
                                    .Alphabets = r.frequency _
                                        .Select(Function(b)
                                                    Return New Alphabet With {
                                                        .Alphabet = b.Key,
                                                        .RelativeFrequency = b.Value
                                                    }
                                                End Function) _
                                        .ToArray,
                                    .Position = r.index,
                                    .Bits = Residue.CalculatesBits(.ByRef, E, NtMol:=True).Bits
                                }
                            End Function) _
                    .ToArray,
                .En = E
            }
        End Function
    End Module
End Namespace
