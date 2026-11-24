Namespace Darwinism.NSGA2

    ''' <summary>
    ''' The NSGA-II (Non-dominated Sorting Genetic Algorithm II) multi-objective 
    ''' optimization framework which is built on top of the classic genetic 
    ''' algorithm framework of the <c>Darwinism.GAF</c> namespace.
    ''' </summary>
    ''' <remarks>
    ''' + The <see cref="IRealVector(Of Chr)"/> chromosomes can use the real coded 
    '''   operators <see cref="RealCodedOperators.SBXCrossover(Of Chr)"/> and 
    '''   <see cref="RealCodedOperators.PolynomialMutation(Of Chr)"/>.
    '''
    ''' + All of the objectives are minimization objectives; use the 
    '''   <see cref="SingleObjectiveAdapter(Of Chr)"/> for the classic single 
    '''   objective <see cref="Darwinism.GAF.Fitness(Of Chr)"/> models.
    ''' </remarks>
    Module NamespaceDoc
    End Module
End Namespace
