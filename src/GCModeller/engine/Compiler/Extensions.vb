Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports Excel = Microsoft.VisualBasic.MIME.Office.Excel.File

Public Module Extensions

    <Extension> Public Function ToMarkup(model As CellularModule) As VirtualCell

    End Function

    <Extension> Public Function ToTabular(model As CellularModule) As Excel

    End Function
End Module
