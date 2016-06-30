Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<[PackageNamespace]("Phylip",
Cites:="",
Publisher:="evolution.gs.washington.edu/phylip/credits.html",
Url:="http://evolution.genetics.washington.edu/phylip.html",
Description:="PHYLIP is a free package of programs for inferring phylogenies. All of the methods in this namespace required the phylip software was install on your computer.")>
Module PhylipCLI

    Dim _innerInvoker As PhylipInvoker

    <ExportAPI("Init()")>
    Public Function Initialize(bin As String) As Boolean
        _innerInvoker = New PhylipInvoker(bin)
        Return True
    End Function

    <ExportAPI("Gendist")>
    Public Function Gendist(Matrix As MatrixFile.MatrixFile, ResultSaved As String) As Boolean
        Return _innerInvoker.Gendist(Matrix, ResultSaved)
    End Function
End Module