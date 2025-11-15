#Region "Microsoft.VisualBasic::ebe9c0ca721ad8d126eece75f266743e, analysis\SequenceToolkit\MotifFinder\SequenceMotif.vb"

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

'   Total Lines: 52
'    Code Lines: 28 (53.85%)
' Comment Lines: 16 (30.77%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 8 (15.38%)
'     File Size: 1.35 KB


' Class SequenceMotif
' 
'     Properties: alignments, AverageScore, RSD, seeds, SignificantSites
'                 width
' 
' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.genomics.Analysis.SequenceAlignment.MSA

''' <summary>
''' motif model
''' </summary>
Public Class SequenceMotif : Inherits Probability

    Public Property seeds As MSAOutput

    ''' <summary>
    ''' the alignment score vector of current motif PWM with the source seeds
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute>
    Public Property alignments As Double()

    Public Property tag As String

    Public ReadOnly Property RSD As Double
        Get
            Return alignments.RSD
        End Get
    End Property

    ''' <summary>
    ''' score / motif with
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property AverageScore As Double
        Get
            Return score / seeds.MSA.Length
        End Get
    End Property

    ''' <summary>
    ''' get number of the conserved sites
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property SignificantSites As Integer
        Get
            Return Aggregate r As Residue
                   In region
                   Where r.isConserved
                   Into Count
        End Get
    End Property

    Public Function CreateModel() As MotifPWM
        Dim alphabets As Char() = New Char() {"A"c, "C"c, "G"c, "T"c}
        Dim n As Integer = 100  ' normalized as 100 sequence input
        Dim E As Double = Probability.E(nsize:=100)

        Return New MotifPWM With {
            .name = name,
            .note = tag,
            .pwm = region _
                .Select(Function(r)
                            Dim hi As Double = Probability.HI(r)

                            Return New ResidueSite(r, alphabets) With {
                                .bits = Probability.CalculatesBits(hi, E, NtMol:=alphabets.Length = 4)
                            }
                        End Function) _
                .ToArray,
            .alphabets = alphabets
        }
    End Function

End Class
