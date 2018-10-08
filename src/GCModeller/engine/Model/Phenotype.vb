Public Structure Phenotype

    ''' <summary>
    ''' enzyme = protein + RNA
    ''' </summary>
    Public enzymes As String()

    ''' <summary>
    ''' 
    ''' </summary>
    Public fluxes As Reaction()

    ''' <summary>
    ''' Some protein is not an enzyme
    ''' </summary>
    Public proteins As Protein()

End Structure
