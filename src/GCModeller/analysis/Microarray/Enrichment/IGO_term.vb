#Region "Microsoft.VisualBasic::f4d70cb9e662b42cf5dce136582d6b70, ..\GCModeller\analysis\Microarray\Enrichment\IGO_term.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
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
