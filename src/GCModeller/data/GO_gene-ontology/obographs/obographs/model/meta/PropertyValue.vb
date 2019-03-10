#Region "Microsoft.VisualBasic::ceb431262a646bfa06a91e85ca69f745, data\GO_gene-ontology\obographs\obographs\model\meta\PropertyValue.vb"

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

    ' 	Interface PropertyValue
    ' 
    ' 	    Properties: Meta, Pred, Val, Xrefs
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic

Namespace org.geneontology.obographs.model.meta



	''' <summary>
	''' Associates the container object with a value via a property.
	''' 
	''' For example, a node representing an OWL class may contain a <seealso cref="Meta"/> object
	''' containing a PropertyValue mapping to a textual definition string via a definition property.
	''' 
	''' Broadly, there are two categories of implementing class:
	''' 
	'''  1. PropertyValues corresponding to a specific explicitly modeled property type (e.g synonym)
	'''  2. generic <seealso cref="BasicPropertyValue"/>s - anything property not explicitly modeled
	'''  
	''' A PropertyValue is minimally a tuple `(pred,value)`. However, each sub tuple may also
	''' be "annotated" with additional metadata (this corresponds to an Axiom Annotation in OWL)
	''' 
	'''  - Any tuple can be supported by an array of xrefs.
	'''  - Some implementing classes may choose to model additional explicit annotations (e.g. <seealso cref="SynonymPropertyValue"/>)
	''' 
	''' @author cjm
	''' 
	''' </summary>
	Public Interface PropertyValue

		''' <summary>
		''' Predicates correspond to OWL properties. Like all preds in this datamodel,
		''' a pred is represented as a String which denotes a CURIE
		''' </summary>
		''' <returns> the pred </returns>
		ReadOnly Property Pred As String

		''' <summary>
		''' An array denoting objects that support the property value assertion
		''' </summary>
		''' <returns> the xrefs </returns>
		ReadOnly Property Xrefs As IList(Of String)

		''' <summary>
		''' The value of the property-value
		''' </summary>
		''' <returns> the val </returns>
		ReadOnly Property Val As String



		''' <returns> the meta </returns>
		ReadOnly Property Meta As org.geneontology.obographs.model.Meta

	End Interface

End Namespace
