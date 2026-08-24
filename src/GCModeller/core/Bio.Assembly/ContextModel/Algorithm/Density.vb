#Region "Microsoft.VisualBasic::cb6769cf29b4e8e3bba33d4966054e01, core\Bio.Assembly\ContextModel\Algorithm\Density.vb"

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

    '   Total Lines: 45
    '    Code Lines: 25 (55.56%)
    ' Comment Lines: 15 (33.33%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (11.11%)
    '     File Size: 1.49 KB


    '     Class Density
    ' 
    '         Properties: Abundance, Hits, location, loci, locus_tag
    '                     product
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace ContextModel

    ''' <summary>
    ''' Genomics context relative abundance
    ''' </summary>
    Public Class Density : Implements INamedValue

        ''' <summary>
        ''' The gene locus_tag identifier
        ''' </summary>
        ''' <returns></returns>
        Public Property locus_tag As String Implements INamedValue.Key
        Public Property loci As NucleotideLocation
        ''' <summary>
        ''' The specific features on the genome its relative abundance relative to this gene <see cref="locus_tag"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property Abundance As Double
        Public Property Hits As String()
        ''' <summary>
        ''' Current gene object its function annotation.
        ''' </summary>
        ''' <returns></returns>
        Public Property product As String

        <XmlIgnore>
        Public Property location As String
            Get
                Return loci.ToString
            End Get
            Set(value As String)
                loci = LocusExtensions.TryParse(value)
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
