#Region "Microsoft.VisualBasic::7a65a4653bccc55bf4dc46acda617646, CLI_tools\S.M.A.R.T\ShellScriptAPI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module ShellScriptAPI
    ' 
    '     Function: Convert, CreateArchitectureData, CreatePfamString, CreateVisualizeSession, GetProtein
    '               NewSmartSession, PerfermenceQuery, ReadPfamString, SavePfamString, Visualize
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.CDD
Imports SMRUCC.genomics.Data.Xfam
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Interops.NCBI.ParallelTask
Imports SMRUCC.genomics.ProteinModel

<Package("SMART", Cites:="Letunic, I., et al. (2006). ""SMART 5: domains in the context of genomes And networks."" Nucleic Acids Res 34(Database issue): D257-260.
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
    Public Function CreatePfamString(blastoutput As BlastPlus.v228) As Pfam.PfamString.PfamString()
        Return Pfam.PfamString.CreatePfamString(blastoutput)
    End Function

    <ExportAPI("Pfam-String.Save")>
    Public Function SavePfamString(<Parameter("Pfam-String")> PfamString As Pfam.PfamString.PfamString(),
                                   <Parameter("Save.Csv")> saveCsv As String) As Boolean
        Return PfamString.SaveTo(saveCsv, False)
    End Function

    <ExportAPI("Session.New")>
    Public Function NewSmartSession(<Parameter("cdd.DIR")> cddDIR As String) As CompileDomains
        Dim Cdd_DB As CDDLoader = New CDDLoader(cddDIR)
        Dim LocalBLAST = NCBILocalBlast.CreateSession
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
    Public Function Convert(<Parameter("Db.SMART")> Db As SMARTDB) As IO.File
        Return Db.Export
    End Function

    <ExportAPI("Pfam-String.As.Architectures")>
    Public Function CreateArchitectureData(pfamData As IEnumerable(Of PfamString)) As Protein()
        Dim LQuery = (From pfam As PfamString In pfamData.AsParallel
                      Select New Protein With {
                          .ID = pfam.ProteinId,
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
