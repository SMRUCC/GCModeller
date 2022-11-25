#Region "Microsoft.VisualBasic::4ec01c96a3772b9cd7da07f5d6f7b462, GCModeller\analysis\SequenceToolkit\SequencePatterns.Abstract\ResidueScore.vb"

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

    '   Total Lines: 36
    '    Code Lines: 27
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 982 B


    ' Class ResidueScore
    ' 
    '     Properties: Gene, Protein, residues
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Cos, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra

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
