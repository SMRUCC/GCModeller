﻿#Region "Microsoft.VisualBasic::b0c399a21e64ac1da11727c93c8dc7d8, analysis\SequenceToolkit\SequencePatterns\SequenceLogo\DrawingModel.vb"

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

    '     Interface ILogoResidue
    ' 
    '         Properties: Bits
    ' 
    '     Class DrawingModel
    ' 
    '         Properties: Alphabets, En, ModelsId, Residues
    ' 
    '         Function: AAResidue, NTResidue, pwm2ic, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.Patterns

Namespace SequenceLogo

    ''' <summary>
    ''' Abstract model for a residue site in a motif sequence fragment.
    ''' </summary>
    Public Interface ILogoResidue : Inherits IPatternSite
        ''' <summary>
        ''' The information of this site can give us.
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Bits As Double
    End Interface

    ''' <summary>
    ''' Drawing model for the sequence logo visualization.
    ''' </summary>
    Public Class DrawingModel : Inherits BaseClass

        ''' <summary>
        ''' The motif model is consist of a sequence of residue sites.
        ''' </summary>
        ''' <returns></returns>
        Public Property Residues As Residue()
        Public Property En As Double
        ''' <summary>
        ''' This drawing display title.
        ''' </summary>
        ''' <returns></returns>
        Public Property ModelsId As String

        Public Overrides Function ToString() As String
            Return ModelsId & " --> " & Me.GetJson
        End Function

        Public ReadOnly Property Alphabets As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Residues(Scan0).Alphabets.Length
            End Get
        End Property

        ''' <summary>
        ''' Creates the residue model in amino acid profiles
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Shared Function AAResidue(x As ILogoResidue) As Residue
            Dim Residue As New Residue With {
                .Alphabets = ColorSchema _
                    .AA _
                    .Select(Function(r)
                                Return New Alphabet With {
                                    .Alphabet = r,
                                    .RelativeFrequency = x(r)
                                }
                            End Function) _
                    .ToArray,
                .Bits = x.Bits
            }

            Return Residue
        End Function

        ''' <summary>
        ''' Creates the residue model for the nucleotide sequence motif model.
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Shared Function NTResidue(x As ILogoResidue) As Residue
            Dim Residue As New Residue With {
                .Alphabets = {
                    New Alphabet With {.Alphabet = "A"c, .RelativeFrequency = x("A"c)},
                    New Alphabet With {.Alphabet = "T"c, .RelativeFrequency = x("T"c)},
                    New Alphabet With {.Alphabet = "G"c, .RelativeFrequency = x("G"c)},
                    New Alphabet With {.Alphabet = "C"c, .RelativeFrequency = x("C"c)}
                },
                .Bits = x.Bits
            }

            Return Residue
        End Function

        ''' <summary>
        ''' ## Get information content profile from PWM
        ''' </summary>
        ''' <param name="pwm"></param>
        ''' <returns></returns>
        Public Shared Function pwm2ic(pwm As DrawingModel) As Double()
            Dim npos As Integer = pwm.Residues.First.Alphabets.Length
            Dim ic#() = New Double(npos - 1) {}

            For i As Integer = 0 To npos - 1
                Dim idx As Integer = i

                ic(i) = 2 + pwm _
                    .Residues _
                    .Select(Function(x)
                                If (x.Alphabets(idx).RelativeFrequency > 0) Then
                                    Return x.Alphabets(idx).RelativeFrequency * Math.Log(x.Alphabets(idx).RelativeFrequency, 2)
                                Else
                                    Return 0
                                End If
                            End Function) _
                    .Sum
            Next

            Return ic
        End Function
    End Class
End Namespace
