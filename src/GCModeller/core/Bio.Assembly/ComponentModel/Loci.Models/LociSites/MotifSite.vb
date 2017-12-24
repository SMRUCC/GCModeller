#Region "Microsoft.VisualBasic::0049f36691c69b1dd140fa1536a2770c, ..\GCModeller\core\Bio.Assembly\ComponentModel\Loci.Models\LociSites\MotifSite.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Namespace ComponentModel.Loci

    ''' <summary>
    ''' Motif site model on both DNA/RNA and protein sequence.
    ''' </summary>
    Public Interface IMotifSite

        ''' <summary>
        ''' loci types
        ''' </summary>
        ''' <returns></returns>
        Property Type As String
        ''' <summary>
        ''' loci name
        ''' </summary>
        ''' <returns></returns>
        Property Name As String
        Property Site As Location
    End Interface

    ''' <summary>
    ''' This motif site have the scoring calculation value
    ''' </summary>
    Public Interface IMotifScoredSite : Inherits IMotifSite

        ''' <summary>
        ''' The site score of this SNP site
        ''' </summary>
        ''' <returns></returns>
        Property Score As Double
    End Interface
End Namespace
