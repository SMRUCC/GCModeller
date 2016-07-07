---
title: ExpArticle
---

# ExpArticle
_namespace: [SMRUCC.genomics.Data.Regtransbase.StructureObjects](N-SMRUCC.genomics.Data.Regtransbase.StructureObjects.html)_

2. Article (ExpArticle) 
 A separate set of regulatory elements and experiments is created for each article in the package (see below). When work with article is completed, annotator sets the article in one of the following states: Completed, Unrelated or Unclear. “Completed” state means the article contains important information which was entered into database (i.e. annotation includes at least one experiment). Annotator sets “Unrelated” state if there were no important experiments in the article. “Unclear” state used if the annotator can not make a decision about the article..




### Properties

#### art_abstract
art_abstract: article abstract (as in PubMed)
#### art_journal
art_journal, art_year, art_month, art_volume, art_issue, art_pages: article bibliographic data
#### exp_nom
exp_nom: number of experiments in the article
#### fl_completed
fl_completed: “Completed” flag
#### fl_not_by_the_theme
fl_not_by_the_theme: “Unrelated” flag
#### fl_started
fl_started: “Article was sent to annotator” flag
#### fl_unclear
fl_unclear: “Unclear” flag
#### note
note: Annotator’s comment to the article
#### pkg_guid
pkg_guid: id of the package, containing the article (here and below)
#### pmid
pmid: article PubMedID
#### title
title: article title
