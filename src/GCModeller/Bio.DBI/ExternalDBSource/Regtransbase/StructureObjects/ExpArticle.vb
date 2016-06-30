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