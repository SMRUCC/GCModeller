#Region "Microsoft.VisualBasic::86b393ae750097db2536bd0fc86c5524, GCModeller\core\Bio.Assembly\ComponentModel\Locus\LociSites\MotifSite.vb"

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

    '   Total Lines: 32
    '    Code Lines: 10
    ' Comment Lines: 18
    '   Blank Lines: 4
    '     File Size: 855 B


    '     Interface IMotifSite
    ' 
    '         Properties: family, name, site
    ' 
    '     Interface IMotifScoredSite
    ' 
    '         Properties: Score
    ' 
    ' 
    ' /********************************************************************************/

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
        Property family As String
        ''' <summary>
        ''' loci name
        ''' </summary>
        ''' <returns></returns>
        Property name As String
        Property site As Location
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
