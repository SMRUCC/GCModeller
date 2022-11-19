#Region "Microsoft.VisualBasic::86bcba77a566377f10c36f123f1285d1, GCModeller\core\Bio.Assembly\ComponentModel\Locus\Nucleotide\NucleotideLocationParser.vb"

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

    '   Total Lines: 23
    '    Code Lines: 16
    ' Comment Lines: 3
    '   Blank Lines: 4
    '     File Size: 691 B


    '     Class NucleotideLocationParser
    ' 
    '         Function: ToString, TryParse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace ComponentModel.Loci

    ''' <summary>
    ''' Custom parser for csv field
    ''' </summary>
    Public Class NucleotideLocationParser
        Implements IParser

        Public Overloads Function ToString(obj As Object) As String Implements IParser.ToString
            If obj Is Nothing Then
                Return ""
            Else
                Return DirectCast(obj, NucleotideLocation).ToString
            End If
        End Function

        Public Function TryParse(cell As String) As Object Implements IParser.TryParse
            Return NucleotideLocation.Parse(cell)
        End Function
    End Class
End Namespace
