#Region "Microsoft.VisualBasic::5c19126954a68a8c53177c88dd52a94a, data\GO_gene-ontology\obographs\obographs\model\Node.vb"

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

    ' 	Class Node
    ' 
    ' 
    ' 		Enum RDFTYPES
    ' 
    ' 		    			[CLASS], 			[PROPERTY], 			INDIVIDUAL
    ' 
    ' 
    ' 
    ' 		Class Builder
    ' 
    ' 		    Properties: Id, Label, Meta
    ' 
    ' 		    Constructor: (+1 Overloads) Sub New
    ' 		    Function: build, id, label, meta, type
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace org.geneontology.obographs.model


	''' <summary>
	''' A graph node corresponds to a class, individual or property
	''' 
	''' ![Node UML](node-uml.png)
	''' 
	''' @startuml node-uml.png
	''' class Node {
	'''   String id
	''' }
	''' class Meta
	''' 
	''' Node-->Meta : 0..1
	''' @enduml
	''' 
	''' @author cjm
	''' 
	''' </summary>
	Public Class Node
		Implements NodeOrEdge

		Public Enum RDFTYPES
			[CLASS]
			INDIVIDUAL
			[PROPERTY]
		End Enum

		Private Sub New(builder As Builder)
			id = builder.id
			label = builder.label
			meta = builder.meta
			type = builder.type
		End Sub

		Private ReadOnly id As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private ReadOnly label As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private ReadOnly meta As Meta

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private ReadOnly type As RDFTYPES



		''' <returns> the id </returns>
		Public Overridable Property Id As String
			Get
				Return id
			End Get
		End Property



		''' <returns> the lbl </returns>
		Public Overridable Property Label As String
			Get
				Return label
			End Get
		End Property



		''' <returns> the meta </returns>
		Public Overridable Property Meta As Meta Implements NodeOrEdge.getMeta
			Get
				Return meta
			End Get
		End Property



		Public Class Builder

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___id As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___label As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___meta As Meta
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___type As RDFTYPES

			Public Overridable Function id(___id As String) As Builder
				Me.___id = ___id
				Return Me
			End Function

			Public Overridable Function label(___label As String) As Builder
				Me.___label = ___label
				Return Me
			End Function

			Public Overridable Function meta(___meta As Meta) As Builder
				Me.___meta = ___meta
				Return Me
			End Function

			Public Overridable Function type(___type As RDFTYPES) As Builder
				Me.___type = ___type
				Return Me
			End Function

			Public Overridable Function build() As Node
				Return New Node(Me)
			End Function
		End Class

	End Class

End Namespace
