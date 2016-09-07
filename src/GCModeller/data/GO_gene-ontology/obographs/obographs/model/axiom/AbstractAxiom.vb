Namespace org.geneontology.obographs.model.axiom




	Public MustInherit Class AbstractAxiom
		Implements Axiom

		Protected Friend Sub New(builder As org.geneontology.obographs.model.axiom.EquivalentNodesSet.Builder)
			meta = builder.meta
		End Sub

		Protected Friend ReadOnly meta As org.geneontology.obographs.model.Meta





		''' <returns> the meta </returns>
		Public Overridable Property Meta As org.geneontology.obographs.model.Meta Implements Axiom.getMeta
			Get
				Return meta
			End Get
		End Property


		Public Class Builder

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___meta As org.geneontology.obographs.model.Meta

			Public Overridable Function meta(___meta As org.geneontology.obographs.model.Meta) As org.geneontology.obographs.model.axiom.EquivalentNodesSet.Builder
				Me.___meta = ___meta
				Return Me
			End Function


		End Class

	End Class

End Namespace