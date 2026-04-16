#Region "Microsoft.VisualBasic::ee323e7757ec6d0ea4fe8d2a497e46c0, analysis\SequenceToolkit\SequencePatterns\Motif\MotifPattern.vb"

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

    '   Total Lines: 46
    '    Code Lines: 17 (36.96%)
    ' Comment Lines: 24 (52.17%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (10.87%)
    '     File Size: 1.52 KB


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
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Motif

    ''' <summary>
    ''' Regular expression model for the motifs
    ''' </summary>
    Public Class MotifPattern : Implements INamedValue

        ''' <summary>
        ''' the unique reference id key of current motif pattern
        ''' </summary>
        ''' <returns></returns>
        Public Property Id As String Implements INamedValue.Key
        ''' <summary>
        ''' the regular expression for search the sequence site
        ''' </summary>
        ''' <returns></returns>
        Public Property Expression As String
        ''' <summary>
        ''' the motif name
        ''' </summary>
        ''' <returns></returns>
        Public Property Motif As String
        ''' <summary>
        ''' the width of current motif(site length)
        ''' </summary>
        ''' <returns></returns>
        Public Property Width As Integer

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        ''' <summary>
        ''' scan the motif sites on a given sequence data
        ''' </summary>
        ''' <param name="scanner"></param>
        ''' <returns></returns>
        Public Function Scan(scanner As Scanner) As SimpleSegment()
            Return scanner.Scan(Expression)
        End Function
    End Class
End Namespace
