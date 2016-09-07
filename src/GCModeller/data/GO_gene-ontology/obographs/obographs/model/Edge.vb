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