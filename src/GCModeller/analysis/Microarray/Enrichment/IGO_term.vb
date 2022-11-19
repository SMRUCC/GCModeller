#Region "Microsoft.VisualBasic::1cc84eb2a6666854024bb3e43686e828, GCModeller\analysis\Microarray\Enrichment\IGO_term.vb"

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
    '    Code Lines: 14
    ' Comment Lines: 4
    '   Blank Lines: 8
    '     File Size: 477 B


    ' Interface IGoTerm
    ' 
    '     Properties: Go_ID
    ' 
    ' Interface IGoTermEnrichment
    ' 
    '     Properties: CorrectedPvalue, Pvalue
    ' 
    ' Interface IKEGGTerm
    ' 
    '     Properties: ID, Link, ORF, Pvalue, Term
    ' 
    ' /********************************************************************************/

#End Region

Public Interface IGoTerm

    Property Go_ID As String
End Interface

Public Interface IGoTermEnrichment : Inherits IGoTerm

    Property Pvalue As Double
    Property CorrectedPvalue As Double

End Interface

Public Interface IKEGGTerm

    Property ID As String
    Property Term As String
    Property ORF As String()
    Property Pvalue As Double

    ''' <summary>
    ''' KEGG link
    ''' </summary>
    ''' <returns></returns>
    Property Link As String

End Interface
