Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.AnalysisTools.ProteinTools.Sanger.Pfam.PfamString
Imports SMRUCC.genomics.ProteinModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports SMRUCC.genomics.Assembly.NCBI.CDD

<PackageNamespace("SMART", Cites:="Letunic, I., et al. (2006). ""SMART 5: domains in the context of genomes And networks."" Nucleic Acids Res 34(Database issue): D257-260.
	The Simple Modular Architecture Research Tool (SMART) is an online resource (http://smart.embl.de/) used for protein domain identification and the analysis of protein domain architectures. 
Many new features were implemented to make SMART more accessible to scientists from different fields. 
The new 'Genomic' mode in SMART makes it easy to analyze domain architectures in completely sequenced genomes. 
Domain annotation has been updated with a detailed taxonomic breakdown and a prediction of the catalytic activity for 50 SMART domains is now available, 
based on the presence of essential amino acids. Furthermore, intrinsically disordered protein regions can be identified and displayed. 
The network context is now displayed in the results page for more than 350 000 proteins, enabling easy analyses of domain interactions.
",
                  Url:="http://smart.embl.de/",
                  Publisher:="bork@embl.de",
                  Category:=APICategories.ResearchTools)>
Public Module ShellScriptAPI

    <ExportAPI("Pfam-String.Creates")>
    Public Function CreatePfamString(blastoutput As BlastPlus.v228) As Sanger.Pfam.PfamString.PfamString()
        Return Sanger.Pfam.PfamString.CreatePfamString(blastoutput)
    End Function

    <ExportAPI("Pfam-String.Save")>
    Public Function SavePfamString(<Parameter("Pfam-String")> PfamString As Sanger.Pfam.PfamString.PfamString(),
                                   <Parameter("Save.Csv")> saveCsv As String) As Boolean
        Return PfamString.SaveTo(saveCsv, False)
    End Function

    <ExportAPI("Session.New")>
    Public Function NewSmartSession(<Parameter("cdd.DIR")> cddDIR As String) As CompileDomains
        Dim Cdd_DB As CDDLoader = New CDDLoader(cddDIR)
        Dim LocalBLAST = SMRUCC.genomics.NCBI.Extensions.NCBILocalBlast.CreateSession
        Return New CompileDomains(LocalBLAST, Cdd_DB, Settings.TEMP)
    End Function

    <ExportAPI("Read.Csv.Pfam-String")>
    Public Function ReadPfamString(path As String) As PfamString()
        Return path.LoadCsv(Of PfamString)(False).ToArray
    End Function

    <ExportAPI("Domains.Query")>
    Public Function PerfermenceQuery(SMART As CompileDomains,
                                     Query As String,
                                     Optional Grep As String = "",
                                     Optional DB_Name As String = "Pfam") As SMARTDB
        Dim Cache As String = SMART.Performance(Query, Grep, Settings.DataCache, DB_Name)
        Return SMART.GetLastProcessData
    End Function

    <ExportAPI("Result2Csv")>
    Public Function Convert(<Parameter("Db.SMART")> Db As SMARTDB) As DocumentStream.File
        Return Db.Export
    End Function

    <ExportAPI("Pfam-String.As.Architectures")>
    Public Function CreateArchitectureData(pfamData As IEnumerable(Of PfamString)) As Protein()
        Dim LQuery = (From pfam As PfamString In pfamData.AsParallel
                      Select New Protein With {
                          .Identifier = pfam.ProteinId,
                          .Description = pfam.Description,
                          .SequenceData = New String("-"c, pfam.Length),
                          .Domains = pfam.GetDomainData(True)}).ToArray
        Return LQuery
    End Function

    <ExportAPI("Get.Protein")>
    Public Function GetProtein(collection As IEnumerable(Of Protein), id As String) As Protein
        Return collection.GetItem(id)
    End Function

    <ExportAPI("Protein.Visualize")>
    Public Function Visualize([operator] As DomainVisualize, protein As Protein) As Image
        Return [operator].Visualize(protein)
    End Function

    <ExportAPI("Visualizer.Session.New()")>
    Public Function CreateVisualizeSession(<Parameter("cdd.DIR")> cddDIR As String) As DomainVisualize
        Return New DomainVisualize(New CDDLoader(cddDIR))
    End Function
End Module
