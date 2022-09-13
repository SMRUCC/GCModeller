#Region "Microsoft.VisualBasic::34d32264bdeea690d7539fdd93e387dc, GCModeller\data\RegulonDatabase\Regtransbase\StructureObjects\ExpArticle.vb"

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

    '   Total Lines: 71
    '    Code Lines: 21
    ' Comment Lines: 49
    '   Blank Lines: 1
    '     File Size: 2.76 KB


    '     Class ExpArticle
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Regtransbase.StructureObjects

    ''' <summary>
    ''' 2. Article (ExpArticle)    
    ''' A separate set of regulatory elements and experiments is created for each article in the package (see below). When work with article is completed, annotator sets the article in one of the following states: Completed, Unrelated or Unclear. “Completed” state means the article contains important information which was entered into database (i.e. annotation includes at least one experiment). Annotator sets “Unrelated” state if there were no important experiments in the article. “Unclear” state used if the annotator can not make a decision about the article.. 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ExpArticle
        ''' <summary>
        ''' pkg_guid: id of the package, containing the article (here and below)
        ''' </summary>
        ''' <remarks></remarks>
        Public pkg_guid
        ''' <summary>
        ''' title: article title
        ''' </summary>
        ''' <remarks></remarks>
        Public title
        Public author
        ''' <summary>
        ''' pmid: article PubMedID
        ''' </summary>
        ''' <remarks></remarks>
        Public pmid
        ''' <summary>
        ''' art_journal, art_year, art_month, art_volume, art_issue, art_pages: article bibliographic data
        ''' </summary>
        ''' <remarks></remarks>
        Public art_journal
        Public art_year
        Public art_month
        Public art_volume
        Public art_issue
        Public art_pages
        ''' <summary>
        ''' art_abstract: article abstract (as in PubMed)
        ''' </summary>
        ''' <remarks></remarks>
        Public art_abstract
        ''' <summary>
        ''' exp_nom: number of experiments in the article 
        ''' </summary>
        ''' <remarks></remarks>
        Public exp_nom
        ''' <summary>
        ''' fl_started: “Article was sent to annotator” flag
        ''' </summary>
        ''' <remarks></remarks>
        Public fl_started
        ''' <summary>
        ''' fl_completed: “Completed” flag
        ''' </summary>
        ''' <remarks></remarks>
        Public fl_completed
        ''' <summary>
        ''' fl_not_by_the_theme: “Unrelated” flag
        ''' </summary>
        ''' <remarks></remarks>
        Public fl_not_by_the_theme
        ''' <summary>
        ''' fl_unclear: “Unclear” flag
        ''' </summary>
        ''' <remarks></remarks>
        Public fl_unclear
        ''' <summary>
        ''' note: Annotator’s comment to the article
        ''' </summary>
        ''' <remarks></remarks>
        Public note
    End Class
End Namespace
