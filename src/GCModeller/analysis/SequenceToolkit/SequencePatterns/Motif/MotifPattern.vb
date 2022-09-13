#Region "Microsoft.VisualBasic::2be667ba660371174088c3571bc07865, GCModeller\analysis\SequenceToolkit\SequencePatterns\Motif\MotifPattern.vb"

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

    '   Total Lines: 26
    '    Code Lines: 18
    ' Comment Lines: 3
    '   Blank Lines: 5
    '     File Size: 880 B


    '     Class MotifPattern
    ' 
    '         Properties: Expression, Id, Motif, Width
    ' 
    '         Function: Scan, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Motif

    ''' <summary>
    ''' Regular expression model for the motifs
    ''' </summary>
    Public Class MotifPattern : Implements INamedValue

        Public Property Id As String Implements INamedValue.Key
        Public Property Expression As String
        Public Property Motif As String
        Public Property Width As Integer

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Function Scan(scanner As Scanner) As SimpleSegment()
            Return scanner.Scan(Expression)
        End Function
    End Class
End Namespace
