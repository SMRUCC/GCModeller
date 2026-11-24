Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2

''' <summary>
''' 由 ``classes.dat`` 构建虚拟细胞模型之中的类(分类)层次定义表
''' </summary>
Public Class ClassCompiler

    ReadOnly biocyc As Workspace

    Sub New(biocyc As Workspace)
        Me.biocyc = biocyc
    End Sub

    Public Function CreateClasses() As ClassTerm()
        Return biocyc.classes.features _
            .Select(Function(c)
                        Return New ClassTerm With {
                            .id = c.uniqueId,
                            .name = c.commonName,
                            .parent = c.parent,
                            .abbreviation = c.abbrevName
                        }
                    End Function) _
            .ToArray
    End Function

End Class
