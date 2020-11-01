Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

''' <summary>
''' HTS expression data simulating for analysis test
''' </summary>
<Package("magnitude", Category:=APICategories.UtilityTools)>
Module magnitude

    <ExportAPI("profiles")>
    Public Function profiles(selector As String, foldchange As Double, Optional base As list = Nothing, Optional env As Environment = Nothing) As Object
        Dim mapNames As String() = KOSelector.SelectMaps(selector)
        Dim data As Dictionary(Of String, Double)

        If base Is Nothing Then
            base = New list With {
                .slots = New Dictionary(Of String, Object)
            }
        End If

        data = base.AsGeneric(Of Double)(env)


    End Function
End Module
