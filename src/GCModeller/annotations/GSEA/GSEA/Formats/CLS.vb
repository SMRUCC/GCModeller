Imports Microsoft.VisualBasic.Linq

''' <summary>
''' The CLS file format defines phenotype (class or template) labels and 
''' associates each sample in the expression data with a label. 
''' The CLS file format uses spaces or tabs to separate the fields.
'''
''' The CLS file format differs somewhat depending On whether you are 
''' defining categorical Or continuous phenotypes. Categorical labels 
''' define discrete phenotypes; For example, normal vs tumor.
''' </summary>
''' <remarks>
''' http://software.broadinstitute.org/cancer/software/gsea/wiki/index.php/Data_formats#Phenotype_Data_Formats
''' </remarks>
Public Class CLS

    ''' <summary>
    ''' number of samples
    ''' </summary>
    ''' <returns></returns>
    Public Property numOfSamples As Integer
    ''' <summary>
    ''' number of classes
    ''' </summary>
    ''' <returns></returns>
    Public Property numOfClasses As Integer
    ''' <summary>
    ''' 应用于报告输出的显示名称
    ''' ``[label => name]``
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' The count of this table should be EQUALS to <see cref="numOfClasses"/>
    ''' </remarks>
    Public Property classNameMaps As Dictionary(Of String, String)
    ''' <summary>
    ''' class label of each sample
    ''' </summary>
    ''' <returns></returns>
    Public Property sampleClass As String()

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 1: (number of samples) (space) (number of classes) (space) 1
    ''' 2: # (space) (class 0 name) (space) (class 1 name)
    ''' 3: (sample 1 class) (space) (sample 2 class) (space) ... (sample N class)
    ''' </remarks>
    Public Shared Function ParseFile(path As String) As CLS
        Dim lines$() = path.ReadAllLines
        Dim headers = lines(Scan0).StringSplit("\s+")
        Dim classList = lines(2).StringSplit("(\t|\s)+")
        Dim titles = lines(1).Trim("#"c, " "c).StringSplit("\s+")
        ' Note: The order of the labels determines the association of class names 
        ' and class labels, even if the class labels are the same as the class 
        ' names.
        Dim nameMaps As Dictionary(Of String, String) = classList _
            .Distinct _
            .SeqIterator _
            .ToDictionary(Function(lb) lb.value,
                          Function(i)
                              Return titles(i)
                          End Function)

        Return New CLS With {
            .numOfSamples = headers(Scan0),
            .numOfClasses = headers(1),
            .classNameMaps = nameMaps,
            .sampleClass = classList
        }
    End Function
End Class
