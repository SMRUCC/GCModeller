#Region "Microsoft.VisualBasic::38a006d21e211f39ab1237a76ebcd786, GCModeller\analysis\SequenceToolkit\MotifFinder\SequenceMotif.vb"

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

    '   Total Lines: 22
    '    Code Lines: 14
    ' Comment Lines: 4
    '   Blank Lines: 4
    '     File Size: 554 B


    ' Class SequenceMotif
    ' 
    '     Properties: AverageScore, seeds, width
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Analysis.SequenceTools.MSA

Public Class SequenceMotif : Inherits Probability

    Public Property seeds As MSAOutput

    ''' <summary>
    ''' the length of the MSA alignment
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property width As Integer
        Get
            Return seeds.MSA(Scan0).Length
        End Get
    End Property

    Public ReadOnly Property AverageScore As Double
        Get
            Return score / seeds.MSA.Length
        End Get
    End Property
End Class
