Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' A cellular system consist of three features:
''' 
''' + Genotype
''' + Phenotype
''' + Regulations
''' 
''' (GCMarkup或者Tabular模型文件加载之后会被转换为这个对象，然后计算核心加载这个对象模型来进行计算分析)
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
