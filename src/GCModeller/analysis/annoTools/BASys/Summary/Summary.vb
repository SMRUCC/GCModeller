Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' [index] BASys Annotation Summary
''' </summary>
Public Class Summary : Inherits ClassObject

    ''' <summary>
    ''' Chromosome Id
    ''' </summary>
    ''' <returns></returns>
    Public Property chrId As String
    Public Property Length As Integer
    ''' <summary>
    ''' Gram Stain
    ''' </summary>
    ''' <returns></returns>
    Public Property GramStain As String
    Public Property Topology As String
    Public Property Genus As String
    Public Property Species As String
    Public Property Strain As String
    ''' <summary>
    ''' Number of Genes Identified
    ''' </summary>
    ''' <returns></returns>
    Public Property gIdentified As Integer
    ''' <summary>
    ''' Number of Genes Annotated
    ''' </summary>
    ''' <returns></returns>
    Public Property gAnnotated As Integer

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Function IndexParser(path As String) As Summary
        Dim html As String = path.GET
    End Function
End Class
