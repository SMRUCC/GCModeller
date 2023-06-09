
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Model.SBML

<Package("SBML")>
Module SBMLTools

    ''' <summary>
    ''' Read a sbml model file from a given file path
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read.sbml")>
    Public Function readSBML(file As String) As Object
        Return Level3.LoadSBML(file)
    End Function
End Module
