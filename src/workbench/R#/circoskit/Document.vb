
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Visualize.Circos.Configurations

<Package("document", Category:=APICategories.UtilityTools)>
Module Document

    ''' <summary>
    ''' Creats a new <see cref="Circos"/> plots configuration document.
    ''' </summary>
    <ExportAPI("create_doc")>
    Public Function CreateDataModel() As Circos
        Return Circos.CreateObject
    End Function
End Module
