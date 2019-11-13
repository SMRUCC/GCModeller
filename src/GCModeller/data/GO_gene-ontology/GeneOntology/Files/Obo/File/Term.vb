#Region "Microsoft.VisualBasic::1fdca6a18f45eda4ac39baeb489ba297, data\GO_gene-ontology\GeneOntology\Files\Obo\File\Term.vb"

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

    '     Class Term
    ' 
    '         Properties: alt_id, comment, consider, created_by, creation_date
    '                     def, disjoint_from, equivalent_to, intersection_of, is_a
    '                     is_obsolete, property_value, relationship, replaced_by, subset
    '                     synonym, xref
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Reflection

Namespace OBO

    ''' <summary>
    ''' ##### GO as a Graph
    ''' 
    ''' The Structure of GO can be described In terms Of a graph, where Each GO term Is a node, And the relationships between the terms are 
    ''' edges between the nodes. GO Is loosely hierarchical, With 'child' terms being more specialized than their 'parent' terms, but unlike 
    ''' a strict hierarchy, a term may have more than one parent term (note that the parent/child model does not hold true for all types of 
    ''' relation)
    ''' 
    ''' (功能定义,term相当于一个节点，节点之间的继承关系是非严格的，即某一个子节点可能会有多个父节点)
    ''' </summary>
    ''' <remarks>
    ''' #### Sample GO Term
    ''' The following Is a GO term taken from the OBO format file.
    ''' 
    ''' ```
    ''' id: GO : 0016049
    ''' name: cell growth
    ''' Namespace:  biological_process
    ''' def: "The process in which a cell irreversibly increases in size over time by accretion and biosynthetic production of matter similar to that already present." [GOC:ai]
    ''' subset: goslim_generic
    ''' subset: goslim_plant
    ''' subset: gosubset_prok
    ''' synonym: "cell expansion" RELATED []
    ''' synonym: "cellular growth" EXACT []
    ''' synonym: "growth of cell" EXACT []
    ''' is_a: GO:0009987 ! cellular process
    ''' is_a: GO:0040007 ! growth
    ''' relationship: part_of GO : 0008361 ! regulation of cell size
    ''' ```
    ''' </remarks>
    Public Class Term : Inherits base

        ''' <summary>
        ''' A textual description of what the term represents, plus reference(s) to the source of the information. 
        ''' All new terms added to the ontology must have a definition; there remains a very small set of terms 
        ''' from the original ontology that lack definitions, but the vast majority of terms are defined.
        ''' </summary>
        ''' <returns></returns>
        <Field("def")> Public Property def As String
        ''' <summary>
        ''' Alternative words or phrases closely related in meaning to the term name, with indication of the relationship between the name 
        ''' and synonym given by the synonym scope. The scopes for GO synonyms are:
        ''' + exact
        '''   an exact equivalent; interchangeable With the term name
        '''   e.g. ornithine cycle Is an exact synonym of urea cycle
        ''' + broad
        '''   the synonym Is broader than the term name
        '''   e.g. cell division Is a broad synonym of cytokinesis
        ''' + narrow
        '''   the synonym Is narrower Or more precise than the term name
        '''   e.g. pyrimidine-dimer repair by photolyase Is a narrow synonym of photoreactive repair
        ''' + related
        '''   the terms are related In some way Not covered above
        '''   e.g. cytochrome bc1 complex Is a related synonym of ubiquinol-cytochrome-c reductase activity virulence Is a related synonym of pathogenesis
        ''' 
        ''' Custom synonym types are also used In the ontology. For example, a number Of synonyms are designated As systematic synonyms; 
        ''' synonyms Of this type are exact synonyms Of the term name.
        ''' </summary>
        ''' <returns></returns>
        <Field("synonym")> Public Property synonym As String()
        ''' <summary>
        ''' Database cross-references, or dbxrefs, refer to identical or very similar objects in other databases. For instance, 
        ''' the molecular function term retinal isomerase activity is cross-referenced with the Enzyme Commission 
        ''' entry EC:5.2.1.3; the biological process term sulfate assimilation has the cross-reference MetaCyc:PWY-781.
        ''' </summary>
        ''' <returns></returns>
        <Field("xref")> Public Property xref As String()
        <Field("is_a")> Public Property is_a As String()
        ''' <summary>
        ''' Indicates that the term belongs to a designated subset of terms, e.g. one of the GO slims.
        ''' </summary>
        ''' <returns></returns>
        <Field("subset")> Public Property subset As String()
        ''' <summary>
        ''' One or more links that capture how the term relates to other terms in the ontology. All terms 
        ''' (other than the root terms representing each namespace, above) have an is a sub-class relationship 
        ''' to another term; for example, GO:0015758 : glucose transport is a GO:0015749 : monosaccharide transport. 
        ''' The Gene Ontology employs a number of other relations, including part of (e.g. GO:0031966 : 
        ''' mitochondrial membrane part of GO:0005740 : mitochondrial envelope) and regulates (e.g. GO:0006916 : 
        ''' anti-apoptosis regulates GO:0012501 : programmed cell death). The relations documentation has more 
        ''' information on the relations used in the ontology.
        ''' </summary>
        ''' <returns></returns>
        <Field("relationship")> Public Property relationship As String()
        <Field("replaced_by")> Public Property replaced_by As String
        <Field("is_obsolete")> Public Property is_obsolete As String
        <Field("comment")> Public Property comment As String
        <Field("equivalent_to")> Public Property equivalent_to As String()
        <Field("alt_id")> Public Property alt_id As String()
        <Field("intersection_of")> Public Property intersection_of As String()
        <Field("property_value")> Public Property property_value As String()
        <Field("consider")> Public Property consider As String()
        <Field("disjoint_from")> Public Property disjoint_from As String()
        <Field("created_by")> Public Property created_by As String
        <Field("creation_date")> Public Property creation_date As String

        Public Const Term As String = "[Term]"
        Public Const Typedef As String = "[Typedef]"

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}: {2}", [namespace], id, name)
        End Function
    End Class
End Namespace
