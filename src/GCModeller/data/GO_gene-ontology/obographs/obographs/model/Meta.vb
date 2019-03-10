#Region "Microsoft.VisualBasic::0ce31608191f839f500c178d2f5f2d28, data\GO_gene-ontology\obographs\obographs\model\Meta.vb"

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

    ' 	Class Meta
    ' 
    ' 	    Properties: BasicPropertyValues, Comments, Definition, Subsets, Synonyms
    '                  Xrefs, XrefsValues
    ' 
    ' 	    Constructor: (+1 Overloads) Sub New
    ' 		Class Builder
    ' 
    ' 		    Function: addSubset, addSynonym, addXref, basicPropertyValues, build
    '                 comments, (+2 Overloads) definition, (+2 Overloads) subsets, synonyms, version
    '                 xrefs
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic

Namespace org.geneontology.obographs.model




	''' <summary>
	''' A holder for metadata
	''' 
	''' The information in a Meta object consists sets of <seealso cref="PropertyValue"/> objects,
	''' which associate the Meta object holder with some value via some property.
	''' 
	''' The set of PropertyValue objects can be partitioned into two subsets:
	''' 
	'''  1. PropertyValues corresponding to a specific explicitly modeled property type (e.g synonym)
	'''  2. generic <seealso cref="BasicPropertyValue"/>s - anything property not explicitly modeled
	'''  
	''' 
	''' 
	''' @author cjm
	''' 
	''' </summary>
	Public Class Meta

		Private Sub New(builder As Builder)
			definition = builder.___definition
			comments = builder.___comments
			subsets = builder.___subsets
			synonyms = builder.___synonyms
			xrefs = builder.___xrefs
			basicPropertyValues = builder.___basicPropertyValues
			version = builder.___version
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private ReadOnly definition As org.geneontology.obographs.model.meta.DefinitionPropertyValue
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private ReadOnly comments As IList(Of String)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private ReadOnly subsets As IList(Of String)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private ReadOnly xrefs As IList(Of org.geneontology.obographs.model.meta.XrefPropertyValue)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private ReadOnly synonyms As IList(Of org.geneontology.obographs.model.meta.SynonymPropertyValue)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private ReadOnly basicPropertyValues As IList(Of org.geneontology.obographs.model.meta.BasicPropertyValue)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private ReadOnly version As String


		''' <returns> the definition </returns>
		Public Overridable Property Definition As org.geneontology.obographs.model.meta.DefinitionPropertyValue
			Get
				Return definition
			End Get
		End Property



		''' <returns> the comments </returns>
		Public Overridable Property Comments As IList(Of String)
			Get
				Return comments
			End Get
		End Property

		''' <returns> the xrefs </returns>
		Public Overridable Property Xrefs As IList(Of org.geneontology.obographs.model.meta.XrefPropertyValue)
			Get
				Return xrefs
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property XrefsValues As IList(Of String)
			Get
				Return xrefs.stream().map(x -> x.Val).collect(java.util.stream.Collectors.toList())
			End Get
		End Property


		''' <returns> the subsets </returns>
		Public Overridable Property Subsets As IList(Of String)
			Get
				Return subsets
			End Get
		End Property









		''' <returns> the synonymPropertyValues </returns>
		Public Overridable Property Synonyms As IList(Of org.geneontology.obographs.model.meta.SynonymPropertyValue)
			Get
				Return synonyms
			End Get
		End Property









		''' <returns> the basicPropertyValues </returns>
		Public Overridable Property BasicPropertyValues As IList(Of org.geneontology.obographs.model.meta.BasicPropertyValue)
			Get
				Return basicPropertyValues
			End Get
		End Property









		Public Class Builder

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public ___subsets As IList(Of String)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public ___basicPropertyValues As IList(Of org.geneontology.obographs.model.meta.BasicPropertyValue)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public ___synonyms As IList(Of org.geneontology.obographs.model.meta.SynonymPropertyValue)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public ___comments As IList(Of String)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public ___definition As org.geneontology.obographs.model.meta.DefinitionPropertyValue
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public ___xrefs As IList(Of org.geneontology.obographs.model.meta.XrefPropertyValue)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public ___version As String


			Public Overridable Function definition(___definition As org.geneontology.obographs.model.meta.DefinitionPropertyValue) As Builder
				Me.___definition = ___definition
				Return Me
			End Function
			Public Overridable Function definition(defval As String) As Builder
				Me.___definition = (New org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder).val(defval).build()
				'((org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder) new DefinitionPropertyValue.Builder().val(defval)).build();
				Return Me
			End Function
			Public Overridable Function subsets(___subsets As IList(Of String)) As Builder
				Me.___subsets = ___subsets
				Return Me
			End Function
			Public Overridable Function subsets(___subsets As String()) As Builder
				Me.___subsets = java.util.Arrays.asList(___subsets)
				Return Me
			End Function
			Public Overridable Function addSubset(subset As String) As Builder
				If Me.___subsets Is Nothing Then Me.subsets(New List(Of String))
				Me.___subsets.Add(subset)
				Return Me
			End Function

			Public Overridable Function comments(___comments As IList(Of String)) As Builder
				Me.___comments = ___comments
				Return Me
			End Function
			Public Overridable Function basicPropertyValues(___basicPropertyValues As IList(Of org.geneontology.obographs.model.meta.BasicPropertyValue)) As Builder
				Me.___basicPropertyValues = ___basicPropertyValues
				Return Me
			End Function
			Public Overridable Function synonyms(___synonyms As IList(Of org.geneontology.obographs.model.meta.SynonymPropertyValue)) As Builder
				Me.___synonyms = ___synonyms
				Return Me
			End Function
			Public Overridable Function addSynonym(syn As org.geneontology.obographs.model.meta.SynonymPropertyValue) As Builder
				If Me.___synonyms Is Nothing Then Me.___synonyms = New List(Of )
				Me.___synonyms.Add(syn)
				Return Me
			End Function
			Public Overridable Function addXref(xref As org.geneontology.obographs.model.meta.XrefPropertyValue) As Builder
				If Me.___xrefs Is Nothing Then Me.___xrefs = New List(Of )
				Me.___xrefs.Add(xref)
				Return Me
			End Function

			Public Overridable Function xrefs(___xrefs As IList(Of org.geneontology.obographs.model.meta.XrefPropertyValue)) As Builder
				Me.___xrefs = ___xrefs
				Return Me
			End Function

			Public Overridable Function version(___version As String) As Builder
				Me.___version = ___version
				Return Me
			End Function


			Public Overridable Function build() As Meta
				Return New Meta(Me)
			End Function

		End Class

	End Class

End Namespace
