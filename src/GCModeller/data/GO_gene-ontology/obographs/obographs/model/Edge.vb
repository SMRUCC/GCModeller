#Region "Microsoft.VisualBasic::d3a8751aab062cbc2ce0068750708dc6, data\GO_gene-ontology\obographs\obographs\model\Edge.vb"

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

    ' 	Class Edge
    ' 
    ' 	    Properties: [Sub], Meta, Obj, Pred
    ' 
    ' 	    Constructor: (+1 Overloads) Sub New
    ' 		Class Builder
    ' 
    ' 		    Function: [sub], build, meta, obj, pred
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace org.geneontology.obographs.model


	''' <summary>
	''' An edge connects two nodes via a predicate
	''' 
	''' @author cjm
	''' 
	''' </summary>
	Public Class Edge
		Implements NodeOrEdge

		Private Sub New(builder As Builder)
			[sub] = builder.sub
			pred = builder.pred
			obj = builder.obj
			meta = builder.meta
		End Sub

		Private ReadOnly [sub] As String
		Private ReadOnly pred As String
		Private ReadOnly obj As String
		Private ReadOnly meta As Meta



		''' <returns> the subj </returns>
		Public Overridable Property [Sub] As String
			Get
				Return [sub]
			End Get
		End Property



		''' <returns> the pred </returns>
		Public Overridable Property Pred As String
			Get
				Return pred
			End Get
		End Property



		''' <returns> the obj </returns>
		Public Overridable Property Obj As String
			Get
				Return obj
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
			Private ___sub As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___pred As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___obj As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___meta As Meta

			Public Overridable Function [sub](subj As String) As Builder
				Me.___sub = subj
				Return Me
			End Function
			Public Overridable Function obj(___obj As String) As Builder
				Me.___obj = ___obj
				Return Me
			End Function

			Public Overridable Function pred(___pred As String) As Builder
				Me.___pred = ___pred
				Return Me
			End Function

			Public Overridable Function meta(___meta As Meta) As Builder
				Me.___meta = ___meta
				Return Me
			End Function

			Public Overridable Function build() As Edge
				Return New Edge(Me)
			End Function
		End Class

	End Class

End Namespace
