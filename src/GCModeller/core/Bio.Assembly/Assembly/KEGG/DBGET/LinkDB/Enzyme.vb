﻿#Region "Microsoft.VisualBasic::990aabfdb4739aba8c86f5711eebebb9, Bio.Assembly\Assembly\KEGG\DBGET\LinkDB\Enzyme.vb"

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

    '     Module Enzyme
    ' 
    '         Function: DoGetEnzymeList, DoGetKEGGGenes, PageUrl
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.DBGET.LinkDB

    Public Module Enzyme

        Public Function PageUrl(RefSeq As String) As String
            Return $"https://www.genome.jp/dbget-bin/get_linkdb?-t+enzyme+rs:{RefSeq}"
        End Function

        Public Function DoGetEnzymeList(refSeq$, Optional cache$ = GenericParser.LinkDbCache) As NamedValue()
            Return GenericParser.LinkDbEntries(PageUrl(refSeq), cache)
        End Function

        Public Function DoGetKEGGGenes(Tcode$, Optional cache$ = GenericParser.LinkDbCache) As NamedValue()
            Return GenericParser.LinkDbEntries($"https://www.genome.jp/dbget-bin/get_linkdb?-t+genes+gn:{Tcode}", cache)
        End Function
    End Module
End Namespace
