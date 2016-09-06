#Region "Microsoft.VisualBasic::e74e991e60cb534965e90f0886e1d32b, ..\GCModeller\data\GO_gene-ontology\AnnotationFile\Obo\Term.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
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

Imports SMRUCC.genomics.foundation.OBO_Foundry

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
Public Class Term

    ''' <summary>
    ''' Every term has a term name—e.g. mitochondrion, glucose transport, amino acid binding—and a unique zero-padded seven digit 
    ''' identifier (often called the term accession or term accession number) prefixed by GO:, e.g. GO:0005125 or GO:0060092. 
    ''' The numerical portion of the ID has no inherent meaning or relation to the position of the term in the ontologies. 
    ''' Ranges of GO IDs are assigned to individual ontology editors or editing groups, and can thus be used to trace who added the term.
    ''' </summary>
    ''' <returns></returns>
    <Field("id")> Public Property id As String
    <Field("name")> Public Property name As String
    ''' <summary>
    ''' Denotes which of the three sub-ontologies—cellular component, biological process or molecular function—the term belongs to.
    ''' </summary>
    ''' <returns></returns>
    <Field("namespace")> Public Property [namespace] As String
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
    <Field("is_a")> Public Property is_a As String
    ''' <summary>
    ''' Indicates that the term belongs to a designated subset of terms, e.g. one of the GO slims.
    ''' </summary>
    ''' <returns></returns>
    <Field("subset")> Public Property subset As String
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

    Public Const TERM As String = "[Term]"

    Public Overrides Function ToString() As String
        Return String.Format("[{0}] {1}: {2}", [namespace], id, name)
    End Function
End Class
