Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' A cellular system consist of three features:
''' 
''' + Genotype
''' + Phenotype
''' + Regulations
''' 
''' </summary>
Public Structure CellularModule

    Public Taxonomy As Taxonomy

    ''' <summary>
    ''' Genome
    ''' </summary>
    Public Genotype As Genotype
    ''' <summary>
    ''' Metabolome
    ''' </summary>
    Public Phenotype As Phenotype
    Public Regulations As Regulation()

End Structure
