
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Visualize.Circos
Imports SMRUCC.genomics.Visualize.Circos.Configurations

<Package("circos.model", Category:=APICategories.UtilityTools)>
Module Model

    ''' <summary>
    ''' Invoke set the radius value of the ideogram circle.
    ''' </summary>
    ''' <param name="circos"></param>
    ''' <param name="r"></param>
    ''' <returns></returns>
    <ExportAPI("Set.Ideogram.Radius")>
    Public Function SetIdeogramRadius(circos As Circos, r As Double) As Circos
        Dim idg As Ideogram = circos.GetIdeogram
        Call CircosAPI.SetIdeogramRadius(idg, r)
        Return circos
    End Function
End Module
