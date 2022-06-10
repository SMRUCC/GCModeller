#Region "Microsoft.VisualBasic::ea93b8d10bf0c55326bdfeeb2b1b91ee, analysis\SequenceToolkit\SequencePatterns.Abstract\Probability.vb"

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

' Class Probability
' 
'     Properties: pvalue, region, score
' 
'     Function: patternString, ToString
'     Structure Residue
' 
'         Properties: frequency, index, isEmpty, topChar
' 
'         Function: Cos, GetEmpty, Max, ToString
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra

''' <summary>
''' The PWM model
''' </summary>
Public Class Probability

    Public Property region As Residue()
    Public Property pvalue As Double
    Public Property score As Double

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return patternString() & $" @ {score}, pvalue={pvalue.ToString("G4")}"
    End Function

    Public Function patternString() As String
        Return region _
           .Select(Function(r) r.ToString) _
           .JoinBy("")
    End Function
End Class

Public Class ResidueScore

    Public ReadOnly Property residues As Char()

    Public Shared ReadOnly Property Protein As ResidueScore
        Get
            Return New ResidueScore("")
        End Get
    End Property

    Public Shared ReadOnly Property Gene As ResidueScore
        Get
            Return New ResidueScore("ATGC")
        End Get
    End Property

    Sub New(chars As IEnumerable(Of Char))
        residues = chars.ToArray
    End Sub

    Public Function Cos(a As Residue, b As Residue) As Double
        Dim v1 As New Vector(residues.Select(Function(c) a(c)))
        Dim v2 As New Vector(residues.Select(Function(c) b(c)))

        Return v1.SSM(v2)
    End Function

    Public Overrides Function ToString() As String
        Return residues.CharString
    End Function

End Class
