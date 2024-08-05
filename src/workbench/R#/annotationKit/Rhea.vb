Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.Rhea

''' <summary>
''' Rhea is an expert-curated knowledgebase of chemical and transport reactions of biological interest 
''' and the standard for enzyme and transporter annotation in UniProtKB. Rhea uses the chemical 
''' dictionary ChEBI (Chemical Entities of Biological Interest) to describe reaction participants.
''' </summary>
<Package("rhea")>
Module Rhea

    ''' <summary>
    ''' open the rdf data pack of Rhea database
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' https://ftp.expasy.org/databases/rhea/rdf/rhea.rdf.gz
    ''' </remarks>
    <ExportAPI("open.rdf")>
    Public Function openRDF(file As String) As RheaRDF
        Return file.LoadXml(Of RheaRDF)
    End Function
End Module
