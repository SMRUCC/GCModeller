Imports LANS.SystemsBiology.DatabaseServices.SabiorkKineticLaws.TabularDump
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.ComponentModels
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME
Imports Microsoft.VisualBasic

Namespace GCML_Documents.XmlElements.Metabolism

    Public Class Metabolism

        ''' <summary>
        ''' 实现表达调控过程的底物约束所需要的代谢物
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ConstraintMetaboliteMaps As ConstraintMetaboliteMap()
        Public Property Compartments As List(Of Compartment)
        ''' <summary>
        ''' The collection of all of the metabolites that required in this cell model.
        ''' (本模型中所需求的代谢物的集合)
        ''' </summary>
        ''' <remarks>
        ''' 所有的RNA，蛋白质，小分子化合物都会放置于这个集合之中
        ''' </remarks>
        Public Property Metabolites As List(Of Metabolite)
        ''' <summary>
        ''' The collection of all of the metabolite reaction in this cell model.
        ''' (本模型中的所有代谢反应的集合)
        ''' </summary>
        ''' <remarks></remarks>
        Public Property MetabolismNetwork As List(Of Reaction)
        ''' <summary>
        ''' Metabolism pathways collection.(代谢途径的集合)
        ''' </summary>
        ''' <remarks></remarks>
        Public Property Pathways As Pathway()

        ''' <summary>
        ''' The collection of the enzyme that required in the metabolism reaction.
        ''' (代谢反应所需求的酶分子的集合)
        ''' </summary>
        ''' <remarks></remarks>
        Public Property MetabolismEnzymes As EnzymeCatalystKineticLaw()

        Public Function AppendNewMetabolite(UniqueId As String, CompoundType As Metabolite.MetaboliteTypes) As Metabolite
            Dim LQuery = (From item In Me.Metabolites.AsParallel
                          Where String.Equals(item.Identifier, UniqueId)
                          Select item).ToArray
            If LQuery.IsNullOrEmpty Then
                Dim Metabolite = New Metabolite With {
                    .Identifier = UniqueId,
                    .InitialAmount = 10,
                    .CommonName = UniqueId,
                    .BoundaryCondition = False,
                    .MetaboliteType = CompoundType
                }
                Call Me.Metabolites.Add(Metabolite)
                Return Metabolite
            Else
                Return LQuery.First
            End If
        End Function
    End Class
End Namespace