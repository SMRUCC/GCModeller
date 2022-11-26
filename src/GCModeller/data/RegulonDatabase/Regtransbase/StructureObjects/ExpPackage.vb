#Region "Microsoft.VisualBasic::0cd33aedef16973ea98062adaba4b35c, GCModeller\data\RegulonDatabase\Regtransbase\StructureObjects\ExpPackage.vb"

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

    '   Total Lines: 207
    '    Code Lines: 40
    ' Comment Lines: 145
    '   Blank Lines: 22
    '     File Size: 9.15 KB


    '     Class ExpPackage
    ' 
    ' 
    ' 
    '     Class Relations_of_regulatory_elements
    ' 
    ' 
    ' 
    '     Class Experminent_object
    ' 
    ' 
    ' 
    '     Class ExpTypes_object
    ' 
    ' 
    ' 
    '     Class ExpSubObject_object
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Regtransbase.StructureObjects

    'Regtransbase structure (1st level)

    'Introduction

    'RegTransBase is a database containing information about regulatory sequences and interactions as well as descriptions of experiments concerning those sequences and interactions.

    'Here we describe the structure of the 1st level of RegTransBase which consists of annotations of literary sources (journal articles). We also propose the creation of the 2nd level of RegTransBase by our experts, which will contain curated data concerning individual regulatory interactions. 

    'Annotation of each article in RegTransBase contains a set of regulatory elements (i. e. “players” in experiments described in the article), links between those elements and information about each experiment (including a list of elements participating in the experiment).

    'Document flow in the 1st level of RegTransBase:
    '- Annotator takes the package of articles (as hard copies) in the office. Also, curator sends him a file containing a “blank annotation” by e-mail. “Blank annotation” contains the title, authors list, abstract and some other information for each article in the package. Curator composes “Blank annotation” file using the curator program.
    '- Annotator imports the “blank annotation” into the RegTransBase annotation program in his computer. Using the annotation program, he enters information about experiments and regulatory elements for all of the articles in the package. When the work is done, the annotator exports the file with annotation and sends it back to the curator (hard copies of the articles also should be returned).
    '- Curator imports the annotation into the database and checks it using the curator program. If annotations are true and accurate, he accepts it (in other cases, he returns package to the annotator for improvement). 

    'Objects of RegTransBase 1st level and their properties

    ''' <summary>
    ''' 1. Package of articles (ExpPackage), Package is the number of articles sent to annotator.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ExpPackage
        ''' <summary>
        ''' topic_guid: identifier of articles’ topic (topics were added for convenience of curator only)
        ''' </summary>
        ''' <remarks></remarks>
        Public topic_guid
        ''' <summary>
        ''' title: package name
        ''' </summary>
        ''' <remarks></remarks>
        Public title
        ''' <summary>
        ''' topic_path: path to the package file
        ''' </summary>
        ''' <remarks></remarks>
        Public topic_path
        ''' <summary>
        ''' master_user_id: name of curator who created the package
        ''' </summary>
        ''' <remarks></remarks>
        Public master_user_id
        ''' <summary>
        ''' annotator_id: name of annotator who will work with the package
        ''' </summary>
        ''' <remarks></remarks>
        Public annotator_id
        ''' <summary>
        ''' fl_ready: “package is ready for export” flag
        ''' </summary>
        ''' <remarks></remarks>
        Public fl_ready
        ''' <summary>
        ''' fl_exported: “package was already exported” flag
        ''' </summary>
        ''' <remarks></remarks>
        Public fl_exported
        ''' <summary>
        ''' master_create_date: date of package creation
        ''' </summary>
        ''' <remarks></remarks>
        Public master_create_date
        ''' <summary>
        ''' master_export_date: date of package export from curator program
        ''' </summary>
        ''' <remarks></remarks>
        Public master_export_date
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public annotator_export_date
        ''' <summary>
        ''' article_date: currently not used 
        ''' </summary>
        ''' <remarks></remarks>
        Public article_date
        ''' <summary>
        ''' fl_can_not_change: “accepted package” flag (i.e. changes are not allowed)
        ''' </summary>
        ''' <remarks></remarks>
        Public fl_can_not_change
        ''' <summary>
        ''' format_version: for service use
        ''' </summary>
        ''' <remarks></remarks>
        Public format_version
    End Class

    Public Class Relations_of_regulatory_elements

        'Possibility to establish relations between regulatory elements is an important feature of RegTransBase. “Child” element in relation of two elements designated as subelement. All regulatory element except Regulator, Effector and Helix can have subelements. Different types of regulatory elements have different sets of possible subelements (see table 1). 

        'Any subelement can have several “parents”. For instance, regulatory site controlling expression of two different genes can be a subelement of those genes. 

        'Table 1. Possible types of subelements


        '                S()
        '                U()
        '                B()
        '                E()
        '                L()
        '                E()
        '                M()
        '                E()
        '                N()
        'T	E  L  E  M  E  N  T
        '		Site	Gene	Operon	Transcr.	Locus	Regulon	SecStr	Regulator
        '	Site	+	+	+	+	+	+	–	–
        '	Gene	–	–	+	+	+	+	–	–
        '	Operon	–	–	–	–	+	+	–	–
        '	Transcr.	–	–	+	–	+	+	–	–
        '	Locus	–	–	–	–	+	+	–	–
        '	Regulon	–	–	–	–	–	–	–	–
        '	SecStr.	+	+	+	+	+	+	–	–
        '	Helix	+	+	+	+	+	+	+	–
        '	Regulator	–	–	–	–	–	–	–	–
        '	Inductor	–	–	–	–	–	–	–	+

        'Links between “parent” and “child” objects are stored in SubObjList table.
        'Properties of 
        'object in SubObjList:

        Public pkg_guid, art_guid
        ''' <summary>
        ''' parent_guid, child_guid: identificators of “parent” and “child” objects
        ''' </summary>
        ''' <remarks></remarks>
        Public parent_guid
        ''' <summary>
        ''' parent_type_id, child_type_id: types of “parent” and “child” objects
        ''' </summary>
        ''' <remarks></remarks>
        Public parent_type_id, child_guid, child_type_id
        ''' <summary>
        ''' child_n: number of current subelement in subelements list of “parental” object (i. e. object defined by parent_guid property)
        ''' </summary>
        ''' <remarks></remarks>
        Public child_n
        ''' <summary>
        ''' strand: defines which DNA strand (direct or complementary) contains the element
        ''' </summary>
        ''' <remarks></remarks>
        Public strand
    End Class

    ''' <summary>
    ''' Experiment
    ''' While working with article, annotator adds experiments to database. Annotation of each experiment includes list of experimental techniques used in the experiment, list of regulatory elements studied in the experiment and description of the experiment (recently, we included additional field describing the aim of the experiment). 
    '''
    ''' Types of experimental techniques are stored in ExpTypes table. 
    ''' Links to regulatory elements studied in the experiment are stored in ExpSubObject table.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Experminent_object

        Public pkg_guid, art_guid, descript
        ''' <summary>
        ''' last_change_date: date of last change of the experiment
        ''' </summary>
        ''' <remarks></remarks>
        Public last_change_date
    End Class

    Public Class ExpTypes_object

        Public pkg_guid, art_guid
        ''' <summary>
        ''' exp_guid: id of experiment linked with experimental technique
        ''' </summary>
        ''' <remarks></remarks>
        Public exp_guid
        ''' <summary>
        ''' exp_type_guid: id of experimental technique type from ExpType table.
        ''' </summary>
        ''' <remarks></remarks>
        Public exp_type_guid
    End Class

    Public Class ExpSubObject_object

        Public pkg_guid, art_guid, exp_guid
        ''' <summary>
        ''' obj_guid: id of regulatory element
        ''' </summary>
        ''' <remarks></remarks>
        Public obj_guid
        ''' <summary>
        ''' obj_type_id: type of regulatory element
        ''' </summary>
        ''' <remarks></remarks>
        Public obj_type_id
        ''' <summary>
        ''' order_num: number of regulatory element in the list of regulatory element for current experiment
        ''' </summary>
        ''' <remarks></remarks>
        Public order_num
        ''' <summary>
        ''' strand: defines which DNA strand (direct or complementary) contains the element
        ''' </summary>
        ''' <remarks></remarks>
        Public strand
    End Class
End Namespace
