Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' GCModeller id mapping services based on the <see cref="Ptf.ProteinAnnotation"/>
''' </summary>
Public Module IDMapping

    Public Iterator Function Mapping(Of T As INamedValue)(proteins As Ptf.PtfFile, data As IEnumerable(Of T)) As IEnumerable(Of T)

    End Function

End Module
