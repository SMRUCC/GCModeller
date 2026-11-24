Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

''' <summary>
''' The class hierarchy definition of the BioCyc database (``classes.dat``).
''' 
''' 每一个对象描述一个数据库之中的类(class)节点，``TYPES`` 槽位表示当前类的父类，
''' 可以用于为化合物、蛋白质以及代谢通路提供GO分类或者BioCyc的类归属信息。
''' </summary>
<Xref("classes.dat")>
Public Class classes : Inherits Model

    <AttributeField("ABBREV-NAME")> Public Property abbrevName As String
    <AttributeField("STRAIN-NAME")> Public Property strainName As String

    ''' <summary>
    ''' the parent class type of current class node
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property parent As String
        Get
            Return types.SafeQuery.FirstOrDefault
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{uniqueId} -> {parent}"
    End Function

End Class
