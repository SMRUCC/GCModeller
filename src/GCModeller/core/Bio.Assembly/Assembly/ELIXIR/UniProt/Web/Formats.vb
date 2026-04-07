#Region "Microsoft.VisualBasic::9691678882c48a89af9a6424e6f06f66, core\Bio.Assembly\Assembly\ELIXIR\UniProt\Web\Formats.vb"

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
    '    Code Lines: 14 (31.11%)
    ' Comment Lines: 30 (66.67%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 1 (2.22%)
    '     File Size: 963 B


    '     Enum Formats
    ' 
    '         canonical, gff, isoform, list, mappingTable
    '         rdf, tab, txt, xlsx, xml
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.Uniprot.Web

    Public Enum Formats
        ''' <summary>
        ''' FASTA (canonical)
        ''' </summary>
        canonical
        ''' <summary>
        ''' FASTA (canonical &amp; isoform)
        ''' </summary>
        isoform
        ''' <summary>
        ''' Tab-separated
        ''' </summary>
        tab
        ''' <summary>
        ''' Text
        ''' </summary>
        txt
        ''' <summary>
        ''' Excel
        ''' </summary>
        xlsx
        ''' <summary>
        ''' GFF
        ''' </summary>
        gff
        ''' <summary>
        ''' XML
        ''' </summary>
        xml
        ''' <summary>
        ''' Mapping Table
        ''' </summary>         
        mappingTable
        ''' <summary>
        ''' RDF/XML
        ''' </summary>
        rdf
        ''' <summary>
        ''' Target List
        ''' </summary>
        list
    End Enum
End Namespace
