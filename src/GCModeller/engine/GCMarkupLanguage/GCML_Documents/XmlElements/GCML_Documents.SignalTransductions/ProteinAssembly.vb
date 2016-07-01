Imports System.Text
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace GCML_Documents.XmlElements.SignalTransductions

    ''' <summary>
    ''' 蛋白质相互做用的规则，与化学反应遵守同样的动力学原理
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ProteinAssembly : Inherits Reaction
        Implements sIdEnumerable

#Region "Public Properties"
        ''' <summary>
        ''' 指向代谢组中的底物列表的所生成的蛋白质复合物的UniqueId
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ComplexComponents As String()
#End Region
    End Class
End Namespace