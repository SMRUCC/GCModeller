#Region "Microsoft.VisualBasic::2777d8522a96c05eaed340ed0792675b, Bio.Assembly\Assembly\NCBI\Taxonomy\TaxonomyWeb.vb"

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

    '     Module TaxonomyWeb
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.NCBI.Taxonomy

    ''' <summary>
    ''' The Taxonomy Database is a curated classification and nomenclature for all of the organisms in the 
    ''' public sequence databases. This currently represents about 10% of the described species of life 
    ''' on the planet.
    ''' </summary>
    ''' <remarks>
    ''' **Disclaimer**: The NCBI taxonomy database is not an authoritative source for nomenclature or classification - 
    ''' please consult the relevant scientific literature for the most reliable information.
    ''' </remarks>
    Public Module TaxonomyWeb

        ''' <summary>
        ''' http://www.ncbi.nlm.nih.gov/Taxonomy/Browser/wwwtax.cgi
        ''' </summary>
        Public Const TaxonomyWebURL As String = "http://www.ncbi.nlm.nih.gov/Taxonomy/Browser/wwwtax.cgi"
    End Module
End Namespace
