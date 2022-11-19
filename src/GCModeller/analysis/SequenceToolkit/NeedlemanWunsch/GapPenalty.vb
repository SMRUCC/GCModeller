#Region "Microsoft.VisualBasic::897bdd47bd1f8f02f277de0fce00c784, GCModeller\analysis\SequenceToolkit\NeedlemanWunsch\GapPenalty.vb"

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

    '   Total Lines: 53
    '    Code Lines: 30
    ' Comment Lines: 18
    '   Blank Lines: 5
    '     File Size: 1.45 KB


    ' Class GapPenalty
    ' 
    '     Properties: GapExtension, GapOpening, PenaltyTyp
    ' 
    '     Function: getGapCost
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' Class to implement linear and affine gap penalties
''' Bioinformatics 1, WS 15/16
''' Jonas Ditz and Benjamin Schroeder
''' </summary>
Public Class GapPenalty

    Dim __affine As Boolean = False

    ''' <summary>
    ''' get gap opening cost </summary>
    ''' <returns> gapOpening </returns>
    Public Property GapOpening As Integer = 1

    ''' <summary>
    ''' get gap extension cost </summary>
    ''' <returns> gapExtension </returns>
    Public Property GapExtension As Integer = 1

    ''' <summary>
    ''' get gap penalty typ </summary>
    ''' <returns> 0 if linear, 1 else </returns>
    Public Property PenaltyTyp As Integer
        Get
            If __affine Then
                Return 1
            Else
                Return 0
            End If
        End Get
        Set(penaltyType As Integer)
            If penaltyType = 0 Then
                Me.__affine = False
            Else
                Me.__affine = True
            End If
        End Set
    End Property

    ''' <summary>
    ''' get gap cost for currend gap </summary>
    ''' <param name="gapOpen"> </param>
    ''' <returns> gapCost </returns>
    Public Function getGapCost(gapOpen As Boolean) As Integer
        If __affine And gapOpen Then
            Return GapOpening
        ElseIf __affine And (Not gapOpen) Then
            Return GapExtension
        Else
            Return GapOpening
        End If
    End Function
End Class
