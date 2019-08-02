Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\MEME.exe

' 
'  // 
'  // SMRUCC genomics GCModeller Programs Profiles Manager
'  // 
'  // VERSION:   1.0.0.0
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // 
' 
' 
'  A wrapper tools for the NCBR meme tools, this is a powerfull tools for reconstruct the regulation in the bacterial
'  genome.
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /Copys:                              
'  /Copys.DIR:                          
'  /EXPORT.MotifDraws:                  
'  /Footprints:                         3 - Generates the regulation footprints.
'  /Hits.Context:                       2
'  /LDM.Compares:                       
'  /LDM.Selects:                        
'  /MAST.MotifMatches:                  
'  /MAST.MotifMatchs.Family:            1
'  /mast.Regulations:                   
'  /MEME.Batch:                         Batch meme task by using tmod toolbox.
'  /MEME.LDMs:                          
'  /Motif.BuildRegulons:                
'  /Motif.Info:                         Assign the phenotype information And genomic context Info for
'                                       the motif sites. [SimpleSegment] -> [MotifLog]
'  /Motif.Info.Batch:                   [SimpleSegment] -> [MotifLog]
'  /Motif.Similarity:                   Export of the calculation result from the tomtom program.
'  /MotifHits.Regulation:               
'  /MotifSites.Fasta:                   
'  /Parser.Pathway.Batch:               
'  /Regulator.Motifs:                   
'  /Regulator.Motifs.Test:              
'  /RfamSites:                          
'  /seq.logo:                           
'  /Site.MAST_Scan:                     [MAST.Xml] -> [SimpleSegment]
'  /Site.MAST_Scan.Batch:               [MAST.Xml] -> [SimpleSegment]
'  /Site.RegexScan:                     
'  /site.scan:                          
'  /SiteHits.Footprints:                Generates the regulation information.
'  /SWTOM.Compares:                     
'  /SWTOM.Compares.Batch:               
'  /SWTOM.LDM:                          
'  /SWTOM.Query:                        
'  /SWTOM.Query.Batch:                  
'  /Tom.Query:                          
'  /Tom.Query.Batch:                    
'  /TomTOM:                             
'  /TomTom.LDM:                         
'  /TomTOM.Similarity:                  
'  /TOMTOM.Similarity.Batch:            
'  /TomTom.Sites.Groups:                
'  /Trim.MastSite:                      
'  /Trim.MEME.Dataset:                  Trim meme input data set for duplicated sequence and short seqeucne
'                                       which its min length is smaller than the required min length.
'  --CExpr.WGCNA:                       
'  --family.statics:                    
'  --GetFasta:                          
'  --hits.diff:                         
'  --Intersect.Max:                     
'  --logo.Batch:                        
'  --modules.regulates:                 Exports the Venn diagram model for the module regulations.
'  Motif.Locates:                       
'  MotifScan:                           Scan for the motif site by using fragment similarity.
'  --pathway.regulates:                 Associates of the pathway regulation information for the predicted
'                                       virtual footprint information.
'  --site.Match:                        
'  --site.Matches:                      
'  --site.Matches.text:                 Using this function for processing the meme text output from
'                                       the tmod toolbox.
'  --site.stat:                         Statics of the PCC correlation distribution of the regulation
'  VirtualFootprint.DIP:                Associate the dip information with the Sigma 70 virtual footprints.
' 
' 
' API list that with functional grouping
' 
' 1. MEME analysis sequence parser
' 
' 
'    /Parser.DEGs:                        
'    /Parser.Locus:                       
'    /Parser.Log2:                        
'    /Parser.MAST:                        
'    /Parser.Modules:                     Parsing promoter sequence region for genes in kegg reaction
'                                         modules
'    /Parser.Operon:                      
'    /Parser.Pathway:                     Parsing promoter sequence region for genes in pathways.
'    /Parser.RegPrecise.Operons:          
'    /Parser.Regulon:                     
'    /Parser.Regulon.gb:                  
'    /Parser.Regulon.Merged:              
' 
' 
' 2. MEME tools database utilities
' 
' 
'    /Export.Regprecise.Motifs:           This commandline tool have no argument parameters.
'    /MAST_LDM.Build:                     
'    --Get.Intergenic:                    
' 
' 
' 3. Motif Sites Analysis Tools
' 
' 
'    /Export.MotifSites:                  Motif iteration step 1
'    /Export.Similarity.Hits:             Motif iteration step 2
'    /Similarity.Union:                   Motif iteration step 3
' 
' 
' 4. RegPrecise Analysis Tools
' 
' 
'    /BBH.Select.Regulators:              Select bbh result for the regulators in RegPrecise database
'                                         from the regulon bbh data.
'    /Build.FamilyDb:                     
'    /CORN:                               
'    /LDM.MaxW:                           
'    /regulators.compile:                 Regprecise regulators data source compiler.
'    --build.Regulations:                 Genome wide step 2
'    --build.Regulations.From.Motifs:     
'    --Dump.KEGG.Family:                  
'    --mapped-Back:                       
'    mast.compile:                        
'    mast.compile.bulk:                   Genome wide step 1
'    regulators.bbh:                      Compiles for the regulators in the bacterial genome mapped on
'                                         the regprecise database using bbh method.
'    --TCS.Module.Regulations:            
'    --TCS.Regulations:                   
' 
' 
' 5. Regulon tools
' 
' 
'    /regulon.export:                     
'    /Regulon.Reconstruct:                
'    /Regulon.Reconstruct2:               
'    /Regulon.Reconstructs:               Doing the regulon reconstruction job in batch mode.
'    /Regulon.Test:                       
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
''' A wrapper tools for the NCBR meme tools, this is a powerfull tools for reconstruct the regulation in the bacterial genome.
''' </summary>
'''
Public Class MEME : Inherits InteropService

    Public Const App$ = "MEME.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As MEME
          Return New MEME(App:=directory & "/" & MEME.App)
     End Function

''' <summary>
''' ```
''' /BBH.Select.Regulators /in &lt;bbh.csv> /db &lt;regprecise_downloads.DIR> [/out &lt;out.csv>]
''' ```
''' Select bbh result for the regulators in RegPrecise database from the regulon bbh data.
''' </summary>
'''
Public Function SelectRegulatorsBBH([in] As String, db As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/BBH.Select.Regulators")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/db " & """" & db & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Build.FamilyDb /prot &lt;RegPrecise.prot.fasta> /pfam &lt;pfam-string.csv> [/out &lt;out.Xml>]
''' ```
''' </summary>
'''
Public Function BuildFamilyDb(prot As String, pfam As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Build.FamilyDb")
    Call CLI.Append(" ")
    Call CLI.Append("/prot " & """" & prot & """ ")
    Call CLI.Append("/pfam " & """" & pfam & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Copys /in &lt;inDIR> [/out &lt;outDIR> /file &lt;meme.txt>]
''' ```
''' </summary>
'''
Public Function BatchCopy([in] As String, Optional out As String = "", Optional file As String = "") As Integer
    Dim CLI As New StringBuilder("/Copys")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not file.StringEmpty Then
            Call CLI.Append("/file " & """" & file & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Copys.DIR /in &lt;inDIR> /out &lt;outDIR>
''' ```
''' </summary>
'''
Public Function BatchCopyDIR([in] As String, out As String) As Integer
    Dim CLI As New StringBuilder("/Copys.DIR")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /CORN /in &lt;operons.csv> /mast &lt;mastDIR> /PTT &lt;genome.ptt> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function CORN([in] As String, mast As String, PTT As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/CORN")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/mast " & """" & mast & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /EXPORT.MotifDraws /in &lt;virtualFootprints.csv> /MEME &lt;meme.txt.DIR> /KEGG &lt;KEGG_Modules/Pathways.DIR> [/pathway /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function ExportMotifDraw([in] As String, MEME As String, KEGG As String, Optional out As String = "", Optional pathway As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/EXPORT.MotifDraws")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/MEME " & """" & MEME & """ ")
    Call CLI.Append("/KEGG " & """" & KEGG & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If pathway Then
        Call CLI.Append("/pathway ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Export.MotifSites /in &lt;meme.txt> [/out &lt;outDIR> /batch]
''' ```
''' Motif iteration step 1
''' </summary>
'''
Public Function ExportTestMotifs([in] As String, Optional out As String = "", Optional batch As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Export.MotifSites")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If batch Then
        Call CLI.Append("/batch ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Export.Regprecise.Motifs
''' ```
''' This commandline tool have no argument parameters.
''' </summary>
'''
Public Function ExportRegpreciseMotifs() As Integer
    Dim CLI As New StringBuilder("/Export.Regprecise.Motifs")
    Call CLI.Append(" ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Export.Similarity.Hits /in &lt;inDIR> [/out &lt;out.Csv>]
''' ```
''' Motif iteration step 2
''' </summary>
'''
Public Function LoadSimilarityHits([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Export.Similarity.Hits")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Footprints /footprints &lt;footprints.xml> /coor &lt;name/DIR> /DOOR &lt;genome.opr> /maps &lt;bbhMappings.Csv> [/out &lt;out.csv> /cuts &lt;0.65> /extract]
''' ```
''' 3 - Generates the regulation footprints.
''' </summary>
'''
Public Function ToFootprints(footprints As String, coor As String, DOOR As String, maps As String, Optional out As String = "", Optional cuts As String = "", Optional extract As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Footprints")
    Call CLI.Append(" ")
    Call CLI.Append("/footprints " & """" & footprints & """ ")
    Call CLI.Append("/coor " & """" & coor & """ ")
    Call CLI.Append("/DOOR " & """" & DOOR & """ ")
    Call CLI.Append("/maps " & """" & maps & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not cuts.StringEmpty Then
            Call CLI.Append("/cuts " & """" & cuts & """ ")
    End If
    If extract Then
        Call CLI.Append("/extract ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Hits.Context /footprints &lt;footprints.Xml> /PTT &lt;genome.PTT> [/out &lt;out.Xml> /RegPrecise &lt;RegPrecise.Regulations.Xml>]
''' ```
''' 2
''' </summary>
'''
Public Function HitContext(footprints As String, PTT As String, Optional out As String = "", Optional regprecise As String = "") As Integer
    Dim CLI As New StringBuilder("/Hits.Context")
    Call CLI.Append(" ")
    Call CLI.Append("/footprints " & """" & footprints & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not regprecise.StringEmpty Then
            Call CLI.Append("/regprecise " & """" & regprecise & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /LDM.Compares /query &lt;query.LDM.Xml> /sub &lt;subject.LDM.Xml> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function CompareMotif(query As String, [sub] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/LDM.Compares")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/sub " & """" & [sub] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /LDM.MaxW [/in &lt;sourceDIR>]
''' ```
''' </summary>
'''
Public Function LDMMaxLen(Optional [in] As String = "") As Integer
    Dim CLI As New StringBuilder("/LDM.MaxW")
    Call CLI.Append(" ")
    If Not [in].StringEmpty Then
            Call CLI.Append("/in " & """" & [in] & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /LDM.Selects /trace &lt;footprints.xml> /meme &lt;memeDIR> [/out &lt;outDIR> /named]
''' ```
''' </summary>
'''
Public Function Selectes(trace As String, meme As String, Optional out As String = "", Optional named As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/LDM.Selects")
    Call CLI.Append(" ")
    Call CLI.Append("/trace " & """" & trace & """ ")
    Call CLI.Append("/meme " & """" & meme & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If named Then
        Call CLI.Append("/named ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /MAST.MotifMatches /meme &lt;meme.txt.DIR> /mast &lt;MAST_OUT.DIR> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function MotifMatch2(meme As String, mast As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/MAST.MotifMatches")
    Call CLI.Append(" ")
    Call CLI.Append("/meme " & """" & meme & """ ")
    Call CLI.Append("/mast " & """" & mast & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /MAST.MotifMatchs.Family /meme &lt;meme.txt.DIR> /mast &lt;MAST_OUT.DIR> [/out &lt;out.Xml>]
''' ```
''' 1
''' </summary>
'''
Public Function MotifMatch(meme As String, mast As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/MAST.MotifMatchs.Family")
    Call CLI.Append(" ")
    Call CLI.Append("/meme " & """" & meme & """ ")
    Call CLI.Append("/mast " & """" & mast & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /mast.Regulations /in &lt;mastSites.Csv> /correlation &lt;sp_name/DIR> /DOOR &lt;DOOR.opr> [/out &lt;footprint.csv> /cut &lt;0.65>]
''' ```
''' </summary>
'''
Public Function MastRegulations([in] As String, correlation As String, DOOR As String, Optional out As String = "", Optional cut As String = "") As Integer
    Dim CLI As New StringBuilder("/mast.Regulations")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/correlation " & """" & correlation & """ ")
    Call CLI.Append("/DOOR " & """" & DOOR & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not cut.StringEmpty Then
            Call CLI.Append("/cut " & """" & cut & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /MAST_LDM.Build /source &lt;sourceDIR> [/out &lt;exportDIR:=./> /evalue &lt;1e-3>]
''' ```
''' </summary>
'''
Public Function BuildPWMDb(source As String, Optional out As String = "", Optional evalue As String = "") As Integer
    Dim CLI As New StringBuilder("/MAST_LDM.Build")
    Call CLI.Append(" ")
    Call CLI.Append("/source " & """" & source & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not evalue.StringEmpty Then
            Call CLI.Append("/evalue " & """" & evalue & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /MEME.Batch /in &lt;inDIR> [/out &lt;outDIR> /evalue &lt;1> /nmotifs &lt;30> /mod &lt;zoops> /maxw &lt;100>]
''' ```
''' Batch meme task by using tmod toolbox.
''' </summary>
'''
Public Function MEMEBatch([in] As String, Optional out As String = "", Optional evalue As String = "", Optional nmotifs As String = "", Optional [mod] As String = "", Optional maxw As String = "") As Integer
    Dim CLI As New StringBuilder("/MEME.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not evalue.StringEmpty Then
            Call CLI.Append("/evalue " & """" & evalue & """ ")
    End If
    If Not nmotifs.StringEmpty Then
            Call CLI.Append("/nmotifs " & """" & nmotifs & """ ")
    End If
    If Not [mod].StringEmpty Then
            Call CLI.Append("/mod " & """" & [mod] & """ ")
    End If
    If Not maxw.StringEmpty Then
            Call CLI.Append("/maxw " & """" & maxw & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /MEME.LDMs /in &lt;meme.txt> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function MEME2LDM([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/MEME.LDMs")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Motif.BuildRegulons /meme &lt;meme.txt.DIR> /model &lt;FootprintTrace.xml> /DOOR &lt;DOOR.opr> /maps &lt;bbhmappings.csv> /corrs &lt;name/DIR> [/cut &lt;0.65> /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function BuildRegulons(meme As String, model As String, DOOR As String, maps As String, corrs As String, Optional cut As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Motif.BuildRegulons")
    Call CLI.Append(" ")
    Call CLI.Append("/meme " & """" & meme & """ ")
    Call CLI.Append("/model " & """" & model & """ ")
    Call CLI.Append("/DOOR " & """" & DOOR & """ ")
    Call CLI.Append("/maps " & """" & maps & """ ")
    Call CLI.Append("/corrs " & """" & corrs & """ ")
    If Not cut.StringEmpty Then
            Call CLI.Append("/cut " & """" & cut & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Motif.Info /loci &lt;loci.csv> [/motifs &lt;motifs.DIR> /gff &lt;genome.gff> /atg-dist 250 /out &lt;out.csv>]
''' ```
''' Assign the phenotype information And genomic context Info for the motif sites. [SimpleSegment] -> [MotifLog]
''' </summary>
'''
Public Function MotifInfo(loci As String, Optional motifs As String = "", Optional gff As String = "", Optional atg_dist As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Motif.Info")
    Call CLI.Append(" ")
    Call CLI.Append("/loci " & """" & loci & """ ")
    If Not motifs.StringEmpty Then
            Call CLI.Append("/motifs " & """" & motifs & """ ")
    End If
    If Not gff.StringEmpty Then
            Call CLI.Append("/gff " & """" & gff & """ ")
    End If
    If Not atg_dist.StringEmpty Then
            Call CLI.Append("/atg-dist " & """" & atg_dist & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Motif.Info.Batch /in &lt;sites.csv.inDIR> /gffs &lt;gff.DIR> [/motifs &lt;regulogs.motiflogs.MEME.DIR> /num_threads -1 /atg-dist 350 /out &lt;out.DIR>]
''' ```
''' [SimpleSegment] -> [MotifLog]
''' </summary>
'''
Public Function MotifInfoBatch([in] As String, gffs As String, Optional motifs As String = "", Optional num_threads As String = "", Optional atg_dist As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Motif.Info.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/gffs " & """" & gffs & """ ")
    If Not motifs.StringEmpty Then
            Call CLI.Append("/motifs " & """" & motifs & """ ")
    End If
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
    End If
    If Not atg_dist.StringEmpty Then
            Call CLI.Append("/atg-dist " & """" & atg_dist & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Motif.Similarity /in &lt;tomtom.DIR> /motifs &lt;MEME_OUT.DIR> [/out &lt;out.csv> /bp.var]
''' ```
''' Export of the calculation result from the tomtom program.
''' </summary>
'''
Public Function MEMETOM_MotifSimilarity([in] As String, motifs As String, Optional out As String = "", Optional bp_var As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Motif.Similarity")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/motifs " & """" & motifs & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If bp_var Then
        Call CLI.Append("/bp.var ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /MotifHits.Regulation /hits &lt;motifHits.Csv> /source &lt;meme.txt.DIR> /PTT &lt;genome.PTT> /correlates &lt;sp/DIR> /bbh &lt;bbhh.csv> [/out &lt;out.footprints.Csv>]
''' ```
''' </summary>
'''
Public Function HitsRegulation(hits As String, source As String, PTT As String, correlates As String, bbh As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/MotifHits.Regulation")
    Call CLI.Append(" ")
    Call CLI.Append("/hits " & """" & hits & """ ")
    Call CLI.Append("/source " & """" & source & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    Call CLI.Append("/correlates " & """" & correlates & """ ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /MotifSites.Fasta /in &lt;mast_motifsites.csv> [/out &lt;out.fasta>]
''' ```
''' </summary>
'''
Public Function MotifSites2Fasta([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/MotifSites.Fasta")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Parser.DEGs /degs &lt;deseq2.csv> /PTT &lt;genomePTT.DIR> /door &lt;genome.opr> /out &lt;out.DIR> [/log-fold 2]
''' ```
''' </summary>
'''
Public Function ParserDEGs(degs As String, PTT As String, door As String, out As String, Optional log_fold As String = "") As Integer
    Dim CLI As New StringBuilder("/Parser.DEGs")
    Call CLI.Append(" ")
    Call CLI.Append("/degs " & """" & degs & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    Call CLI.Append("/door " & """" & door & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
    If Not log_fold.StringEmpty Then
            Call CLI.Append("/log-fold " & """" & log_fold & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Parser.Locus /locus &lt;locus.txt> /PTT &lt;genomePTT.DIR> /DOOR &lt;genome.opr> [/out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function ParserLocus(locus As String, PTT As String, DOOR As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Parser.Locus")
    Call CLI.Append(" ")
    Call CLI.Append("/locus " & """" & locus & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    Call CLI.Append("/DOOR " & """" & DOOR & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Parser.Log2 /in &lt;log2.csv> /PTT &lt;genomePTT.DIR> /DOOR &lt;genome.opr> [/factor 1 /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function ParserLog2([in] As String, PTT As String, DOOR As String, Optional factor As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Parser.Log2")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    Call CLI.Append("/DOOR " & """" & DOOR & """ ")
    If Not factor.StringEmpty Then
            Call CLI.Append("/factor " & """" & factor & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Parser.MAST /sites &lt;mastsites.csv> /ptt &lt;genome-context.pttDIR> /door &lt;genome.opr> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function ParserMAST(sites As String, ptt As String, door As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Parser.MAST")
    Call CLI.Append(" ")
    Call CLI.Append("/sites " & """" & sites & """ ")
    Call CLI.Append("/ptt " & """" & ptt & """ ")
    Call CLI.Append("/door " & """" & door & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Parser.Modules /KEGG.Modules &lt;KEGG.modules.DIR> /PTT &lt;genomePTT.DIR> /DOOR &lt;genome.opr> [/locus &lt;union/initx/locus, default:=union> /out &lt;fasta.outDIR>]
''' ```
''' Parsing promoter sequence region for genes in kegg reaction modules
''' </summary>
'''
Public Function ModuleParser(KEGG_Modules As String, PTT As String, DOOR As String, Optional locus As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Parser.Modules")
    Call CLI.Append(" ")
    Call CLI.Append("/KEGG.Modules " & """" & KEGG_Modules & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    Call CLI.Append("/DOOR " & """" & DOOR & """ ")
    If Not locus.StringEmpty Then
            Call CLI.Append("/locus " & """" & locus & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Parser.Operon /in &lt;footprint.csv> /PTT &lt;PTTDIR> [/out &lt;outDIR> /family /offset &lt;50> /all]
''' ```
''' </summary>
'''
Public Function ParserNextIterator([in] As String, PTT As String, Optional out As String = "", Optional offset As String = "", Optional family As Boolean = False, Optional all As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Parser.Operon")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not offset.StringEmpty Then
            Call CLI.Append("/offset " & """" & offset & """ ")
    End If
    If family Then
        Call CLI.Append("/family ")
    End If
    If all Then
        Call CLI.Append("/all ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Parser.Pathway /KEGG.Pathways &lt;KEGG.pathways.DIR/organismModel.Xml> /src &lt;genomePTT.DIR/gbff.txt> [/DOOR &lt;genome.opr> /locus &lt;union/initx/locus, default:=union> /out &lt;fasta.outDIR>]
''' ```
''' Parsing promoter sequence region for genes in pathways.
''' </summary>
'''
Public Function PathwayParser(KEGG_Pathways As String, src As String, Optional door As String = "", Optional locus As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Parser.Pathway")
    Call CLI.Append(" ")
    Call CLI.Append("/KEGG.Pathways " & """" & KEGG_Pathways & """ ")
    Call CLI.Append("/src " & """" & src & """ ")
    If Not door.StringEmpty Then
            Call CLI.Append("/door " & """" & door & """ ")
    End If
    If Not locus.StringEmpty Then
            Call CLI.Append("/locus " & """" & locus & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Parser.Pathway.Batch /in &lt;pathway.directory> /assembly &lt;NCBI_assembly.directory> [/out &lt;out.directory>]
''' ```
''' </summary>
'''
Public Function PathwayParserBatch([in] As String, assembly As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Parser.Pathway.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/assembly " & """" & assembly & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Parser.RegPrecise.Operons /operon &lt;operons.Csv> /PTT &lt;PTT_DIR> [/corn /DOOR &lt;genome.opr> /id &lt;null> /locus &lt;union/initx/locus, default:=union> /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function ParserRegPreciseOperon(operon As String, PTT As String, Optional door As String = "", Optional id As String = "", Optional locus As String = "", Optional out As String = "", Optional corn As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Parser.RegPrecise.Operons")
    Call CLI.Append(" ")
    Call CLI.Append("/operon " & """" & operon & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    If Not door.StringEmpty Then
            Call CLI.Append("/door " & """" & door & """ ")
    End If
    If Not id.StringEmpty Then
            Call CLI.Append("/id " & """" & id & """ ")
    End If
    If Not locus.StringEmpty Then
            Call CLI.Append("/locus " & """" & locus & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If corn Then
        Call CLI.Append("/corn ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Parser.Regulon /inDIR &lt;regulons.inDIR> /out &lt;fasta.outDIR> /PTT &lt;genomePTT.DIR> [/door &lt;genome.opr>]
''' ```
''' </summary>
'''
Public Function RegulonParser(inDIR As String, out As String, PTT As String, Optional door As String = "") As Integer
    Dim CLI As New StringBuilder("/Parser.Regulon")
    Call CLI.Append(" ")
    Call CLI.Append("/inDIR " & """" & inDIR & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    If Not door.StringEmpty Then
            Call CLI.Append("/door " & """" & door & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Parser.Regulon.gb /inDIR &lt;regulons.inDIR> /out &lt;fasta.outDIR> /gb &lt;genbank.gbk> [/door &lt;genome.opr>]
''' ```
''' </summary>
'''
Public Function RegulonParser2(inDIR As String, out As String, gb As String, Optional door As String = "") As Integer
    Dim CLI As New StringBuilder("/Parser.Regulon.gb")
    Call CLI.Append(" ")
    Call CLI.Append("/inDIR " & """" & inDIR & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
    Call CLI.Append("/gb " & """" & gb & """ ")
    If Not door.StringEmpty Then
            Call CLI.Append("/door " & """" & door & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Parser.Regulon.Merged /in &lt;merged.Csv> /out &lt;fasta.outDIR> /PTT &lt;genomePTT.DIR> [/DOOR &lt;genome.opr>]
''' ```
''' </summary>
'''
Public Function RegulonParser3([in] As String, out As String, PTT As String, Optional door As String = "") As Integer
    Dim CLI As New StringBuilder("/Parser.Regulon.Merged")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    If Not door.StringEmpty Then
            Call CLI.Append("/door " & """" & door & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Regulator.Motifs /bbh &lt;bbh.csv> /regprecise &lt;genome.DIR> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function RegulatorMotifs(bbh As String, regprecise As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Regulator.Motifs")
    Call CLI.Append(" ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    Call CLI.Append("/regprecise " & """" & regprecise & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Regulator.Motifs.Test /hits &lt;familyHits.Csv> /motifs &lt;motifHits.Csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function TestRegulatorMotifs(hits As String, motifs As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Regulator.Motifs.Test")
    Call CLI.Append(" ")
    Call CLI.Append("/hits " & """" & hits & """ ")
    Call CLI.Append("/motifs " & """" & motifs & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /regulators.compile
''' ```
''' Regprecise regulators data source compiler.
''' </summary>
'''
Public Function RegulatorsCompile() As Integer
    Dim CLI As New StringBuilder("/regulators.compile")
    Call CLI.Append(" ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /regulon.export /in &lt;sw-tom_out.DIR> /ref &lt;regulon.bbh.xml.DIR> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function ExportRegulon([in] As String, ref As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/regulon.export")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/ref " & """" & ref & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Regulon.Reconstruct /bbh &lt;bbh.csv> /genome &lt;RegPrecise.genome.xml> /door &lt;operon.door> [/out &lt;outfile.csv>]
''' ```
''' </summary>
'''
Public Function RegulonReconstruct(bbh As String, genome As String, door As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Regulon.Reconstruct")
    Call CLI.Append(" ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    Call CLI.Append("/genome " & """" & genome & """ ")
    Call CLI.Append("/door " & """" & door & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Regulon.Reconstruct2 /bbh &lt;bbh.csv> /genome &lt;RegPrecise.genome.DIR> /door &lt;operons.opr> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function RegulonReconstructs2(bbh As String, genome As String, door As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Regulon.Reconstruct2")
    Call CLI.Append(" ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    Call CLI.Append("/genome " & """" & genome & """ ")
    Call CLI.Append("/door " & """" & door & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Regulon.Reconstructs /bbh &lt;bbh_EXPORT_csv.DIR> /genome &lt;RegPrecise.genome.DIR> [/door &lt;operon.door> /out &lt;outDIR>]
''' ```
''' Doing the regulon reconstruction job in batch mode.
''' </summary>
'''
Public Function RegulonReconstructs(bbh As String, genome As String, Optional door As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Regulon.Reconstructs")
    Call CLI.Append(" ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    Call CLI.Append("/genome " & """" & genome & """ ")
    If Not door.StringEmpty Then
            Call CLI.Append("/door " & """" & door & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Regulon.Test /in &lt;meme.txt> /reg &lt;genome.bbh.regulon.xml> /bbh &lt;maps.bbh.Csv>
''' ```
''' </summary>
'''
Public Function RegulonTest([in] As String, reg As String, bbh As String) As Integer
    Dim CLI As New StringBuilder("/Regulon.Test")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/reg " & """" & reg & """ ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /RfamSites /source &lt;sourceDIR> [/out &lt;out.fastaDIR>]
''' ```
''' </summary>
'''
Public Function RfamSites(source As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/RfamSites")
    Call CLI.Append(" ")
    Call CLI.Append("/source " & """" & source & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /seq.logo /in &lt;meme.txt> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function SequenceLogoTask([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/seq.logo")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Similarity.Union /in &lt;preSource.fasta.DIR> /meme &lt;meme.txt.DIR> /hits &lt;similarity_hist.Csv> [/out &lt;out.DIR>]
''' ```
''' Motif iteration step 3
''' </summary>
'''
Public Function UnionSimilarity([in] As String, meme As String, hits As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Similarity.Union")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/meme " & """" & meme & """ ")
    Call CLI.Append("/hits " & """" & hits & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Site.MAST_Scan /mast &lt;mast.xml/DIR> [/batch /out &lt;out.csv>]
''' ```
''' [MAST.Xml] -> [SimpleSegment]
''' </summary>
'''
Public Function SiteMASTScan(mast As String, Optional out As String = "", Optional batch As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Site.MAST_Scan")
    Call CLI.Append(" ")
    Call CLI.Append("/mast " & """" & mast & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If batch Then
        Call CLI.Append("/batch ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Site.MAST_Scan /mast &lt;mast.xml.DIR> [/out &lt;out.csv.DIR> /num_threads &lt;-1>]
''' ```
''' [MAST.Xml] -> [SimpleSegment]
''' </summary>
'''
Public Function SiteMASTScanBatch(mast As String, Optional out As String = "", Optional num_threads As String = "") As Integer
    Dim CLI As New StringBuilder("/Site.MAST_Scan")
    Call CLI.Append(" ")
    Call CLI.Append("/mast " & """" & mast & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Site.RegexScan /meme &lt;meme.txt> /nt &lt;nt.fasta> [/batch /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function SiteRegexScan(meme As String, nt As String, Optional out As String = "", Optional batch As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Site.RegexScan")
    Call CLI.Append(" ")
    Call CLI.Append("/meme " & """" & meme & """ ")
    Call CLI.Append("/nt " & """" & nt & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If batch Then
        Call CLI.Append("/batch ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /site.scan /query &lt;LDM.xml> /subject &lt;subject.fasta> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function SiteScan(query As String, subject As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/site.scan")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/subject " & """" & subject & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /SiteHits.Footprints /sites &lt;MotifSiteHits.Csv> /bbh &lt;bbh.Csv> /meme &lt;meme.txt_DIR> /PTT &lt;genome.PTT> /DOOR &lt;DOOR.opr> [/queryHash /out &lt;out.csv>]
''' ```
''' Generates the regulation information.
''' </summary>
'''
Public Function SiteHitsToFootprints(sites As String, bbh As String, meme As String, PTT As String, DOOR As String, Optional out As String = "", Optional queryhash As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/SiteHits.Footprints")
    Call CLI.Append(" ")
    Call CLI.Append("/sites " & """" & sites & """ ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    Call CLI.Append("/meme " & """" & meme & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    Call CLI.Append("/DOOR " & """" & DOOR & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If queryhash Then
        Call CLI.Append("/queryhash ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /SWTOM.Compares /query &lt;query.meme.txt> /subject &lt;subject.meme.txt> [/out &lt;outDIR> /no-HTML]
''' ```
''' </summary>
'''
Public Function SWTomCompares(query As String, subject As String, Optional out As String = "", Optional no_html As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/SWTOM.Compares")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/subject " & """" & subject & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If no_html Then
        Call CLI.Append("/no-html ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /SWTOM.Compares.Batch /query &lt;query.meme.DIR> /subject &lt;subject.meme.DIR> [/out &lt;outDIR> /no-HTML]
''' ```
''' </summary>
'''
Public Function SWTomComparesBatch(query As String, subject As String, Optional out As String = "", Optional no_html As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/SWTOM.Compares.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/subject " & """" & subject & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If no_html Then
        Call CLI.Append("/no-html ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /SWTOM.LDM /query &lt;ldm.xml> /subject &lt;ldm.xml> [/out &lt;outDIR> /method &lt;pcc/ed/sw; default:=pcc>]
''' ```
''' </summary>
'''
Public Function SWTomLDM(query As String, subject As String, Optional out As String = "", Optional method As String = "") As Integer
    Dim CLI As New StringBuilder("/SWTOM.LDM")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/subject " & """" & subject & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not method.StringEmpty Then
            Call CLI.Append("/method " & """" & method & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /SWTOM.Query /query &lt;meme.txt> [/out &lt;outDIR> /method &lt;pcc> /bits.level 1.6 /minW 6 /no-HTML]
''' ```
''' </summary>
'''
Public Function SWTomQuery(query As String, Optional out As String = "", Optional method As String = "", Optional bits_level As String = "", Optional minw As String = "", Optional no_html As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/SWTOM.Query")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not method.StringEmpty Then
            Call CLI.Append("/method " & """" & method & """ ")
    End If
    If Not bits_level.StringEmpty Then
            Call CLI.Append("/bits.level " & """" & bits_level & """ ")
    End If
    If Not minw.StringEmpty Then
            Call CLI.Append("/minw " & """" & minw & """ ")
    End If
    If no_html Then
        Call CLI.Append("/no-html ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /SWTOM.Query.Batch /query &lt;meme.txt.DIR> [/out &lt;outDIR> /SW-offset 0.6 /method &lt;pcc> /bits.level 1.5 /minW 4 /SW-threshold 0.75 /tom-threshold 0.75 /no-HTML]
''' ```
''' </summary>
'''
Public Function SWTomQueryBatch(query As String, Optional out As String = "", Optional sw_offset As String = "", Optional method As String = "", Optional bits_level As String = "", Optional minw As String = "", Optional sw_threshold As String = "", Optional tom_threshold As String = "", Optional no_html As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/SWTOM.Query.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not sw_offset.StringEmpty Then
            Call CLI.Append("/sw-offset " & """" & sw_offset & """ ")
    End If
    If Not method.StringEmpty Then
            Call CLI.Append("/method " & """" & method & """ ")
    End If
    If Not bits_level.StringEmpty Then
            Call CLI.Append("/bits.level " & """" & bits_level & """ ")
    End If
    If Not minw.StringEmpty Then
            Call CLI.Append("/minw " & """" & minw & """ ")
    End If
    If Not sw_threshold.StringEmpty Then
            Call CLI.Append("/sw-threshold " & """" & sw_threshold & """ ")
    End If
    If Not tom_threshold.StringEmpty Then
            Call CLI.Append("/tom-threshold " & """" & tom_threshold & """ ")
    End If
    If no_html Then
        Call CLI.Append("/no-html ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Tom.Query /query &lt;ldm.xml/meme.txt> [/out &lt;outDIR> /method &lt;pcc/ed; default:=pcc> /cost &lt;0.7> /threshold &lt;0.65> /meme]
''' ```
''' </summary>
'''
Public Function TomQuery(query As String, Optional out As String = "", Optional method As String = "", Optional cost As String = "", Optional threshold As String = "", Optional meme As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Tom.Query")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not method.StringEmpty Then
            Call CLI.Append("/method " & """" & method & """ ")
    End If
    If Not cost.StringEmpty Then
            Call CLI.Append("/cost " & """" & cost & """ ")
    End If
    If Not threshold.StringEmpty Then
            Call CLI.Append("/threshold " & """" & threshold & """ ")
    End If
    If meme Then
        Call CLI.Append("/meme ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Tom.Query.Batch /query &lt;inDIR> [/out &lt;outDIR> /method &lt;pcc/ed; default:=pcc> /cost 0.7 /threshold &lt;0.65>]
''' ```
''' </summary>
'''
Public Function TomQueryBatch(query As String, Optional out As String = "", Optional method As String = "", Optional cost As String = "", Optional threshold As String = "") As Integer
    Dim CLI As New StringBuilder("/Tom.Query.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not method.StringEmpty Then
            Call CLI.Append("/method " & """" & method & """ ")
    End If
    If Not cost.StringEmpty Then
            Call CLI.Append("/cost " & """" & cost & """ ")
    End If
    If Not threshold.StringEmpty Then
            Call CLI.Append("/threshold " & """" & threshold & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /TomTOM /query &lt;meme.txt> /subject &lt;LDM.xml> [/out &lt;outDIR> /method &lt;pcc/ed; default:=pcc> /cost &lt;0.7> /threshold &lt;0.3>]
''' ```
''' </summary>
'''
Public Function TomTOMMethod(query As String, subject As String, Optional out As String = "", Optional method As String = "", Optional cost As String = "", Optional threshold As String = "") As Integer
    Dim CLI As New StringBuilder("/TomTOM")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/subject " & """" & subject & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not method.StringEmpty Then
            Call CLI.Append("/method " & """" & method & """ ")
    End If
    If Not cost.StringEmpty Then
            Call CLI.Append("/cost " & """" & cost & """ ")
    End If
    If Not threshold.StringEmpty Then
            Call CLI.Append("/threshold " & """" & threshold & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /TomTom.LDM /query &lt;ldm.xml> /subject &lt;ldm.xml> [/out &lt;outDIR> /method &lt;pcc/ed/sw; default:=sw> /cost &lt;0.7> /threshold &lt;0.65>]
''' ```
''' </summary>
'''
Public Function LDMTomTom(query As String, subject As String, Optional out As String = "", Optional method As String = "", Optional cost As String = "", Optional threshold As String = "") As Integer
    Dim CLI As New StringBuilder("/TomTom.LDM")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/subject " & """" & subject & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not method.StringEmpty Then
            Call CLI.Append("/method " & """" & method & """ ")
    End If
    If Not cost.StringEmpty Then
            Call CLI.Append("/cost " & """" & cost & """ ")
    End If
    If Not threshold.StringEmpty Then
            Call CLI.Append("/threshold " & """" & threshold & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /TomTOM.Similarity /in &lt;TOM_OUT.DIR> [/out &lt;out.Csv>]
''' ```
''' </summary>
'''
Public Function MEMEPlantSimilarity([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/TomTOM.Similarity")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /TOMTOM.Similarity.Batch /in &lt;inDIR> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function MEMEPlantSimilarityBatch([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/TOMTOM.Similarity.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /TomTom.Sites.Groups /in &lt;similarity.csv> /meme &lt;meme.DIR> [/grep &lt;regex> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function ExportTOMSites([in] As String, meme As String, Optional grep As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/TomTom.Sites.Groups")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/meme " & """" & meme & """ ")
    If Not grep.StringEmpty Then
            Call CLI.Append("/grep " & """" & grep & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Trim.MastSite /in &lt;mastSite.Csv> /locus &lt;locus_tag> /correlations &lt;DIR/name> [/out &lt;out.csv> /cut &lt;0.65>]
''' ```
''' </summary>
'''
Public Function Trim([in] As String, locus As String, correlations As String, Optional out As String = "", Optional cut As String = "") As Integer
    Dim CLI As New StringBuilder("/Trim.MastSite")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/locus " & """" & locus & """ ")
    Call CLI.Append("/correlations " & """" & correlations & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not cut.StringEmpty Then
            Call CLI.Append("/cut " & """" & cut & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Trim.MEME.Dataset /in &lt;seq.fasta> [/out &lt;out.fasta> /minl 8 /distinct]
''' ```
''' Trim meme input data set for duplicated sequence and short seqeucne which its min length is smaller than the required min length.
''' </summary>
'''
Public Function TrimInputs([in] As String, Optional out As String = "", Optional minl As String = "", Optional distinct As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Trim.MEME.Dataset")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not minl.StringEmpty Then
            Call CLI.Append("/minl " & """" & minl & """ ")
    End If
    If distinct Then
        Call CLI.Append("/distinct ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --build.Regulations /bbh &lt;regprecise.bbhMapped.csv> /mast &lt;mastSites.csv> [/cutoff &lt;0.6> /out &lt;out.csv> /sp &lt;spName> /DOOR &lt;genome.opr> /DOOR.extract]
''' ```
''' Genome wide step 2
''' </summary>
'''
Public Function Build(bbh As String, mast As String, Optional cutoff As String = "", Optional out As String = "", Optional sp As String = "", Optional door As String = "", Optional door_extract As Boolean = False) As Integer
    Dim CLI As New StringBuilder("--build.Regulations")
    Call CLI.Append(" ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    Call CLI.Append("/mast " & """" & mast & """ ")
    If Not cutoff.StringEmpty Then
            Call CLI.Append("/cutoff " & """" & cutoff & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not sp.StringEmpty Then
            Call CLI.Append("/sp " & """" & sp & """ ")
    End If
    If Not door.StringEmpty Then
            Call CLI.Append("/door " & """" & door & """ ")
    End If
    If door_extract Then
        Call CLI.Append("/door.extract ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --build.Regulations.From.Motifs /bbh &lt;regprecise.bbhMapped.csv> /motifs &lt;motifSites.csv> [/cutoff &lt;0.6> /sp &lt;spName> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function BuildFromMotifSites(bbh As String, motifs As String, Optional cutoff As String = "", Optional sp As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("--build.Regulations.From.Motifs")
    Call CLI.Append(" ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    Call CLI.Append("/motifs " & """" & motifs & """ ")
    If Not cutoff.StringEmpty Then
            Call CLI.Append("/cutoff " & """" & cutoff & """ ")
    End If
    If Not sp.StringEmpty Then
            Call CLI.Append("/sp " & """" & sp & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --CExpr.WGCNA /mods &lt;CytoscapeNodes.txt> /genome &lt;genome.DIR|*.PTT;*.fna> [/out &lt;DIR.out>]
''' ```
''' </summary>
'''
Public Function WGCNAModsCExpr(mods As String, genome As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("--CExpr.WGCNA")
    Call CLI.Append(" ")
    Call CLI.Append("/mods " & """" & mods & """ ")
    Call CLI.Append("/genome " & """" & genome & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --Dump.KEGG.Family /in &lt;in.fasta> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function KEGGFamilyDump([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("--Dump.KEGG.Family")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --family.statics /sites &lt;motifSites.csv> /mods &lt;directory.kegg_modules>
''' ```
''' </summary>
'''
Public Function FamilyStatics(sites As String, mods As String) As Integer
    Dim CLI As New StringBuilder("--family.statics")
    Call CLI.Append(" ")
    Call CLI.Append("/sites " & """" & sites & """ ")
    Call CLI.Append("/mods " & """" & mods & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --Get.Intergenic /PTT &lt;genome.ptt> /nt &lt;genome.fasta> [/o &lt;out.fasta> /len 100 /strict]
''' ```
''' </summary>
'''
Public Function GetIntergenic(PTT As String, nt As String, Optional o As String = "", Optional len As String = "", Optional strict As Boolean = False) As Integer
    Dim CLI As New StringBuilder("--Get.Intergenic")
    Call CLI.Append(" ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    Call CLI.Append("/nt " & """" & nt & """ ")
    If Not o.StringEmpty Then
            Call CLI.Append("/o " & """" & o & """ ")
    End If
    If Not len.StringEmpty Then
            Call CLI.Append("/len " & """" & len & """ ")
    End If
    If strict Then
        Call CLI.Append("/strict ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --GetFasta /bbh &lt;bbhh.csv> /id &lt;subject_id> /regprecise &lt;regprecise.fasta>
''' ```
''' </summary>
'''
Public Function GetFasta(bbh As String, id As String, regprecise As String) As Integer
    Dim CLI As New StringBuilder("--GetFasta")
    Call CLI.Append(" ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    Call CLI.Append("/id " & """" & id & """ ")
    Call CLI.Append("/regprecise " & """" & regprecise & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --hits.diff /query &lt;bbhh.csv> /subject &lt;bbhh.csv> [/reverse]
''' ```
''' </summary>
'''
Public Function DiffHits(query As String, subject As String, Optional reverse As Boolean = False) As Integer
    Dim CLI As New StringBuilder("--hits.diff")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/subject " & """" & subject & """ ")
    If reverse Then
        Call CLI.Append("/reverse ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --Intersect.Max /query &lt;bbhh.csv> /subject &lt;bbhh.csv>
''' ```
''' </summary>
'''
Public Function MaxIntersection(query As String, subject As String) As Integer
    Dim CLI As New StringBuilder("--Intersect.Max")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/subject " & """" & subject & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --logo.Batch -in &lt;inDIR> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function LogoBatch([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("--logo.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("-in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --mapped-Back /meme &lt;meme.text> /mast &lt;mast.xml> /ptt &lt;genome.ptt> [/out &lt;out.csv> /offset &lt;10> /atg-dist &lt;250>]
''' ```
''' </summary>
'''
Public Function SiteMappedBack(meme As String, mast As String, ptt As String, Optional out As String = "", Optional offset As String = "", Optional atg_dist As String = "") As Integer
    Dim CLI As New StringBuilder("--mapped-Back")
    Call CLI.Append(" ")
    Call CLI.Append("/meme " & """" & meme & """ ")
    Call CLI.Append("/mast " & """" & mast & """ ")
    Call CLI.Append("/ptt " & """" & ptt & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not offset.StringEmpty Then
            Call CLI.Append("/offset " & """" & offset & """ ")
    End If
    If Not atg_dist.StringEmpty Then
            Call CLI.Append("/atg-dist " & """" & atg_dist & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' mast.compile /mast &lt;mast.xml> /ptt &lt;genome.ptt> [/no-meme /no-regInfo /p-value 1e-3 /mast-ldm &lt;DIR default:=GCModeller/Regprecise/MEME/MAST_LDM> /atg-dist 250]
''' ```
''' </summary>
'''
Public Function CompileMast(mast As String, ptt As String, Optional p_value As String = "", Optional mast_ldm As String = "", Optional atg_dist As String = "", Optional no_meme As Boolean = False, Optional no_reginfo As Boolean = False) As Integer
    Dim CLI As New StringBuilder("mast.compile")
    Call CLI.Append(" ")
    Call CLI.Append("/mast " & """" & mast & """ ")
    Call CLI.Append("/ptt " & """" & ptt & """ ")
    If Not p_value.StringEmpty Then
            Call CLI.Append("/p-value " & """" & p_value & """ ")
    End If
    If Not mast_ldm.StringEmpty Then
            Call CLI.Append("/mast-ldm " & """" & mast_ldm & """ ")
    End If
    If Not atg_dist.StringEmpty Then
            Call CLI.Append("/atg-dist " & """" & atg_dist & """ ")
    End If
    If no_meme Then
        Call CLI.Append("/no-meme ")
    End If
    If no_reginfo Then
        Call CLI.Append("/no-reginfo ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' mast.compile.bulk /source &lt;source_dir> [/ptt &lt;genome.ptt> /atg-dist &lt;500> /no-meme /no-regInfo /p-value 1e-3 /mast-ldm &lt;DIR default:=GCModeller/Regprecise/MEME/MAST_LDM> /related.all]
''' ```
''' Genome wide step 1
''' </summary>
'''
Public Function CompileMastBuck(source As String, Optional ptt As String = "", Optional atg_dist As String = "", Optional p_value As String = "", Optional mast_ldm As String = "", Optional no_meme As Boolean = False, Optional no_reginfo As Boolean = False, Optional related_all As Boolean = False) As Integer
    Dim CLI As New StringBuilder("mast.compile.bulk")
    Call CLI.Append(" ")
    Call CLI.Append("/source " & """" & source & """ ")
    If Not ptt.StringEmpty Then
            Call CLI.Append("/ptt " & """" & ptt & """ ")
    End If
    If Not atg_dist.StringEmpty Then
            Call CLI.Append("/atg-dist " & """" & atg_dist & """ ")
    End If
    If Not p_value.StringEmpty Then
            Call CLI.Append("/p-value " & """" & p_value & """ ")
    End If
    If Not mast_ldm.StringEmpty Then
            Call CLI.Append("/mast-ldm " & """" & mast_ldm & """ ")
    End If
    If no_meme Then
        Call CLI.Append("/no-meme ")
    End If
    If no_reginfo Then
        Call CLI.Append("/no-reginfo ")
    End If
    If related_all Then
        Call CLI.Append("/related.all ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --modules.regulates /in &lt;virtualfootprints.csv> [/out &lt;out.DIR> /mods &lt;KEGG_modules.DIR>]
''' ```
''' Exports the Venn diagram model for the module regulations.
''' </summary>
'''
Public Function ModuleRegulates([in] As String, Optional out As String = "", Optional mods As String = "") As Integer
    Dim CLI As New StringBuilder("--modules.regulates")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not mods.StringEmpty Then
            Call CLI.Append("/mods " & """" & mods & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' Motif.Locates -ptt &lt;bacterial_genome.ptt> -meme &lt;meme.txt> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function MotifLocites(ptt As String, meme As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("Motif.Locates")
    Call CLI.Append(" ")
    Call CLI.Append("-ptt " & """" & ptt & """ ")
    Call CLI.Append("-meme " & """" & meme & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' MotifScan -nt &lt;nt.fasta> /motif &lt;motifLDM.xml/LDM_Name/FamilyName> [/delta &lt;default:80> /delta2 &lt;default:70> /offSet &lt;default:5> /out &lt;saved.csv>]
''' ```
''' Scan for the motif site by using fragment similarity.
''' </summary>
'''
Public Function MotifScan(nt As String, motif As String, Optional delta As String = "", Optional delta2 As String = "", Optional offset As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("MotifScan")
    Call CLI.Append(" ")
    Call CLI.Append("-nt " & """" & nt & """ ")
    Call CLI.Append("/motif " & """" & motif & """ ")
    If Not delta.StringEmpty Then
            Call CLI.Append("/delta " & """" & delta & """ ")
    End If
    If Not delta2.StringEmpty Then
            Call CLI.Append("/delta2 " & """" & delta2 & """ ")
    End If
    If Not offset.StringEmpty Then
            Call CLI.Append("/offset " & """" & offset & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --pathway.regulates -footprints &lt;virtualfootprint.csv> /pathway &lt;DIR.KEGG.Pathways> [/out &lt;./PathwayRegulations/>]
''' ```
''' Associates of the pathway regulation information for the predicted virtual footprint information.
''' </summary>
'''
Public Function PathwayRegulations(footprints As String, pathway As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("--pathway.regulates")
    Call CLI.Append(" ")
    Call CLI.Append("-footprints " & """" & footprints & """ ")
    Call CLI.Append("/pathway " & """" & pathway & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' regulators.bbh /bbh &lt;bbhDIR/bbh.index.Csv> [/save &lt;save.csv> /direct /regulons /maps &lt;genome.gb>]
''' ```
''' Compiles for the regulators in the bacterial genome mapped on the regprecise database using bbh method.
''' </summary>
'''
Public Function RegulatorsBBh(bbh As String, Optional save As String = "", Optional maps As String = "", Optional direct As Boolean = False, Optional regulons As Boolean = False) As Integer
    Dim CLI As New StringBuilder("regulators.bbh")
    Call CLI.Append(" ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    If Not save.StringEmpty Then
            Call CLI.Append("/save " & """" & save & """ ")
    End If
    If Not maps.StringEmpty Then
            Call CLI.Append("/maps " & """" & maps & """ ")
    End If
    If direct Then
        Call CLI.Append("/direct ")
    End If
    If regulons Then
        Call CLI.Append("/regulons ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --site.Match /meme &lt;meme.text> /mast &lt;mast.xml> /out &lt;out.csv> [/ptt &lt;genome.ptt> /len &lt;150,200,250,300,350,400,450,500>]
''' ```
''' </summary>
'''
Public Function SiteMatch(meme As String, mast As String, out As String, Optional ptt As String = "", Optional len As String = "") As Integer
    Dim CLI As New StringBuilder("--site.Match")
    Call CLI.Append(" ")
    Call CLI.Append("/meme " & """" & meme & """ ")
    Call CLI.Append("/mast " & """" & mast & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
    If Not ptt.StringEmpty Then
            Call CLI.Append("/ptt " & """" & ptt & """ ")
    End If
    If Not len.StringEmpty Then
            Call CLI.Append("/len " & """" & len & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --site.Matches /meme &lt;DIR.meme.text> /mast &lt;DIR.mast.xml> /out &lt;out.csv> [/ptt &lt;genome.ptt>]
''' ```
''' </summary>
'''
Public Function SiteMatches(meme As String, mast As String, out As String, Optional ptt As String = "") As Integer
    Dim CLI As New StringBuilder("--site.Matches")
    Call CLI.Append(" ")
    Call CLI.Append("/meme " & """" & meme & """ ")
    Call CLI.Append("/mast " & """" & mast & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
    If Not ptt.StringEmpty Then
            Call CLI.Append("/ptt " & """" & ptt & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --site.Matches.text /meme &lt;DIR.meme.text> /mast &lt;DIR.mast.xml> /out &lt;out.csv> [/ptt &lt;genome.ptt> /fasta &lt;original.fasta.DIR>]
''' ```
''' Using this function for processing the meme text output from the tmod toolbox.
''' </summary>
'''
Public Function SiteMatchesText(meme As String, mast As String, out As String, Optional ptt As String = "", Optional fasta As String = "") As Integer
    Dim CLI As New StringBuilder("--site.Matches.text")
    Call CLI.Append(" ")
    Call CLI.Append("/meme " & """" & meme & """ ")
    Call CLI.Append("/mast " & """" & mast & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
    If Not ptt.StringEmpty Then
            Call CLI.Append("/ptt " & """" & ptt & """ ")
    End If
    If Not fasta.StringEmpty Then
            Call CLI.Append("/fasta " & """" & fasta & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --site.stat /in &lt;footprints.csv> [/out &lt;out.csv>]
''' ```
''' Statics of the PCC correlation distribution of the regulation
''' </summary>
'''
Public Function SiteStat([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("--site.stat")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --TCS.Module.Regulations /MiST2 &lt;MiST2.xml> /footprint &lt;footprints.csv> /Pathways &lt;KEGG_Pathways.DIR>
''' ```
''' </summary>
'''
Public Function TCSRegulateModule(MiST2 As String, footprint As String, Pathways As String) As Integer
    Dim CLI As New StringBuilder("--TCS.Module.Regulations")
    Call CLI.Append(" ")
    Call CLI.Append("/MiST2 " & """" & MiST2 & """ ")
    Call CLI.Append("/footprint " & """" & footprint & """ ")
    Call CLI.Append("/Pathways " & """" & Pathways & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --TCS.Regulations /TCS &lt;DIR.TCS.csv> /modules &lt;DIR.mod.xml> /regulations &lt;virtualfootprint.csv>
''' ```
''' </summary>
'''
Public Function TCSRegulations(TCS As String, modules As String, regulations As String) As Integer
    Dim CLI As New StringBuilder("--TCS.Regulations")
    Call CLI.Append(" ")
    Call CLI.Append("/TCS " & """" & TCS & """ ")
    Call CLI.Append("/modules " & """" & modules & """ ")
    Call CLI.Append("/regulations " & """" & regulations & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' VirtualFootprint.DIP vf.csv &lt;csv> dip.csv &lt;csv>
''' ```
''' Associate the dip information with the Sigma 70 virtual footprints.
''' </summary>
'''
Public Function VirtualFootprintDIP(vf_csv As String, dip_csv As String) As Integer
    Dim CLI As New StringBuilder("VirtualFootprint.DIP")
    Call CLI.Append(" ")
    Call CLI.Append("vf.csv " & """" & vf_csv & """ ")
    Call CLI.Append("dip.csv " & """" & dip_csv & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
