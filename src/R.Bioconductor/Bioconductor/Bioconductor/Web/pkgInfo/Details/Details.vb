Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace Web.Packages

    Public Class Details

        <Column("biocViews")> Public Property biocViews As String
        <Column("Version")> Public Property Version As String
        <Column("In Bioconductor since")>
        Public Property Since As String
        <Column("License")> Public Property License As String
        <Column("Depends")> Public Property Depends As String
        <Column("[Imports]")> Public Property [Imports] As String
        <Column("LinkingTo")> Public Property LinkingTo As String
        <Column("Suggests")> Public Property Suggests As String
        <Column("SystemRequirements")> Public Property SystemRequirements As String
        <Column("Enhances")> Public Property Enhances As String
        <Column("URL")> Public Property URL As String
        <Column("Depends On Me")> Public Property DependsOnMe As String
        <Column("Imports Me")> Public Property ImportsMe As String
        <Column("Suggests Me")> Public Property SuggestsMe As String

    End Class
End Namespace