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
'  // VERSION:   3.3277.7609.23646
'  // ASSEMBLY:  Settings, Version=3.3277.7609.23646, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright (c) SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     10/31/2020 1:08:12 PM
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
'  /model.save:                         /model.save /model <meme.txt> [/out <model.json>]
'  /Motif.BuildRegulons:                
'  /Motif.Info:                         Assign the phenotype information And genomic context Info for
'                                       the motif sites. [SimpleSegment] -> [MotifLog]
'  /Motif.Info.Batch:                   [SimpleSegment] -> [MotifLog]
'  /Motif.Similarity:                   Export of the calculation result from the tomtom program.
'  /motif.sites.summary:                
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
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As MEME
          Return New MEME(App:=directory & "/" & MEME.App)
     End Function

''' <summary>
''' ```bash
''' /BBH.Select.Regulators /in &lt;bbh.csv&gt; /db &lt;regprecise_downloads.DIR&gt; [/out &lt;out.csv&gt;]
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
''' ```bash
''' /Build.FamilyDb /prot &lt;RegPrecise.prot.fasta&gt; /pfam &lt;pfam-string.csv&gt; [/out &lt;out.Xml&gt;]
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
''' ```bash
''' /Copys /in &lt;inDIR&gt; [/out &lt;outDIR&gt; /file &lt;meme.txt&gt;]
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
''' ```bash
''' /Copys.DIR /in &lt;inDIR&gt; /out &lt;outDIR&gt;
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
''' ```bash
''' /CORN /in &lt;operons.csv&gt; /mast &lt;mastDIR&gt; /PTT &lt;genome.ptt&gt; [/out &lt;out.csv&gt;]
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
''' ```bash
''' /EXPORT.MotifDraws /in &lt;virtualFootprints.csv&gt; /MEME &lt;meme.txt.DIR&gt; /KEGG &lt;KEGG_Modules/Pathways.DIR&gt; [/pathway /out &lt;outDIR&gt;]
''' ```
''' </summary>
'''

Public Function ExportMotifDraw([in] As String, 
                                   MEME As String, 
                                   KEGG As String, 
                                   Optional out As String = "", 
                                   Optional pathway As Boolean = False) As Integer
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
''' ```bash
''' /Export.MotifSites /in &lt;meme.txt&gt; [/out &lt;outDIR&gt; /batch]
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
''' ```bash
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
''' ```bash
''' /Export.Similarity.Hits /in &lt;inDIR&gt; [/out &lt;out.Csv&gt;]
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
''' ```bash
''' /Footprints /footprints &lt;footprints.xml&gt; /coor &lt;name/DIR&gt; /DOOR &lt;genome.opr&gt; /maps &lt;bbhMappings.Csv&gt; [/out &lt;out.csv&gt; /cuts &lt;0.65&gt; /extract]
''' ```
''' 3 - Generates the regulation footprints.
''' </summary>
'''
''' <param name="extract"> Extract the DOOR operon when the regulated gene is the first gene of the operon.
''' </param>
Public Function ToFootprints(footprints As String, 
                                coor As String, 
                                DOOR As String, 
                                maps As String, 
                                Optional out As String = "", 
                                Optional cuts As String = "", 
                                Optional extract As Boolean = False) As Integer
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
''' ```bash
''' /Hits.Context /footprints &lt;footprints.Xml&gt; /PTT &lt;genome.PTT&gt; [/out &lt;out.Xml&gt; /RegPrecise &lt;RegPrecise.Regulations.Xml&gt;]
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
''' ```bash
''' /LDM.Compares /query &lt;query.LDM.Xml&gt; /sub &lt;subject.LDM.Xml&gt; [/out &lt;outDIR&gt;]
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
''' ```bash
''' /LDM.MaxW [/in &lt;sourceDIR&gt;]
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
''' ```bash
''' /LDM.Selects /trace &lt;footprints.xml&gt; /meme &lt;memeDIR&gt; [/out &lt;outDIR&gt; /named]
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
''' ```bash
''' /MAST.MotifMatches /meme &lt;meme.txt.DIR&gt; /mast &lt;MAST_OUT.DIR&gt; [/out &lt;out.csv&gt;]
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
''' ```bash
''' /MAST.MotifMatchs.Family /meme &lt;meme.txt.DIR&gt; /mast &lt;MAST_OUT.DIR&gt; [/out &lt;out.Xml&gt;]
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
''' ```bash
''' /mast.Regulations /in &lt;mastSites.Csv&gt; /correlation &lt;sp_name/DIR&gt; /DOOR &lt;DOOR.opr&gt; [/out &lt;footprint.csv&gt; /cut &lt;0.65&gt;]
''' ```
''' </summary>
'''

Public Function MastRegulations([in] As String, 
                                   correlation As String, 
                                   DOOR As String, 
                                   Optional out As String = "", 
                                   Optional cut As String = "") As Integer
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
''' ```bash
''' /MAST_LDM.Build /source &lt;sourceDIR&gt; [/out &lt;exportDIR:=./&gt; /evalue &lt;1e-3&gt;]
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
''' ```bash
''' /MEME.Batch /in &lt;inDIR&gt; [/out &lt;outDIR&gt; /evalue &lt;1&gt; /nmotifs &lt;30&gt; /mod &lt;zoops&gt; /maxw &lt;100&gt;]
''' ```
''' Batch meme task by using tmod toolbox.
''' </summary>
'''
''' <param name="[in]"> A directory path which contains the fasta sequence for the meme motifs analysis.
''' </param>
''' <param name="out"> A directory path which outputs the meme.txt data to that directory.
''' </param>
Public Function MEMEBatch([in] As String, 
                             Optional out As String = "", 
                             Optional evalue As String = "", 
                             Optional nmotifs As String = "", 
                             Optional [mod] As String = "", 
                             Optional maxw As String = "") As Integer
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
''' ```bash
''' /MEME.LDMs /in &lt;meme.txt&gt; [/out &lt;outDIR&gt;]
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
''' ```bash
''' 
''' ```
''' /model.save /model &lt;meme.txt&gt; [/out &lt;model.json&gt;]
''' </summary>
'''

Public Function SaveModel() As Integer
    Dim CLI As New StringBuilder("/model.save")
    Call CLI.Append(" ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Motif.BuildRegulons /meme &lt;meme.txt.DIR&gt; /model &lt;FootprintTrace.xml&gt; /DOOR &lt;DOOR.opr&gt; /maps &lt;bbhmappings.csv&gt; /corrs &lt;name/DIR&gt; [/cut &lt;0.65&gt; /out &lt;outDIR&gt;]
''' ```
''' </summary>
'''

Public Function BuildRegulons(meme As String, 
                                 model As String, 
                                 DOOR As String, 
                                 maps As String, 
                                 corrs As String, 
                                 Optional cut As String = "", 
                                 Optional out As String = "") As Integer
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
''' ```bash
''' /Motif.Info /loci &lt;loci.csv&gt; [/motifs &lt;motifs.DIR&gt; /gff &lt;genome.gff&gt; /atg-dist 250 /out &lt;out.csv&gt;]
''' ```
''' Assign the phenotype information And genomic context Info for the motif sites. [SimpleSegment] -&gt; [MotifLog]
''' </summary>
'''
''' <param name="loci"> The motif site info data set, type Is simple segment.
''' </param>
''' <param name="motifs"> A directory which contains the motifsitelog data in the xml file format. Regulogs.Xml source directory
''' </param>
Public Function MotifInfo(loci As String, 
                             Optional motifs As String = "", 
                             Optional gff As String = "", 
                             Optional atg_dist As String = "", 
                             Optional out As String = "") As Integer
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
''' ```bash
''' /Motif.Info.Batch /in &lt;sites.csv.inDIR&gt; /gffs &lt;gff.DIR&gt; [/motifs &lt;regulogs.motiflogs.MEME.DIR&gt; /num_threads -1 /atg-dist 350 /out &lt;out.DIR&gt;]
''' ```
''' [SimpleSegment] -&gt; [MotifLog]
''' </summary>
'''
''' <param name="motifs"> Regulogs.Xml source directory
''' </param>
''' <param name="num_threads"> Default Is -1, means auto config of the threads number.
''' </param>
Public Function MotifInfoBatch([in] As String, 
                                  gffs As String, 
                                  Optional motifs As String = "", 
                                  Optional num_threads As String = "", 
                                  Optional atg_dist As String = "", 
                                  Optional out As String = "") As Integer
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
''' ```bash
''' /Motif.Similarity /in &lt;tomtom.DIR&gt; /motifs &lt;MEME_OUT.DIR&gt; [/out &lt;out.csv&gt; /bp.var]
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
''' ```bash
''' /motif.sites.summary /in &lt;data.directory&gt; [/out &lt;summary.csv&gt;]
''' ```
''' </summary>
'''

Public Function MotifSiteSummary([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/motif.sites.summary")
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
''' ```bash
''' /MotifHits.Regulation /hits &lt;motifHits.Csv&gt; /source &lt;meme.txt.DIR&gt; /PTT &lt;genome.PTT&gt; /correlates &lt;sp/DIR&gt; /bbh &lt;bbhh.csv&gt; [/out &lt;out.footprints.Csv&gt;]
''' ```
''' </summary>
'''

Public Function HitsRegulation(hits As String, 
                                  source As String, 
                                  PTT As String, 
                                  correlates As String, 
                                  bbh As String, 
                                  Optional out As String = "") As Integer
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
''' ```bash
''' /MotifSites.Fasta /in &lt;mast_motifsites.csv&gt; [/out &lt;out.fasta&gt;]
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
''' ```bash
''' /Parser.DEGs /degs &lt;deseq2.csv&gt; /PTT &lt;genomePTT.DIR&gt; /door &lt;genome.opr&gt; /out &lt;out.DIR&gt; [/log-fold 2]
''' ```
''' </summary>
'''

Public Function ParserDEGs(degs As String, 
                              PTT As String, 
                              door As String, 
                              out As String, 
                              Optional log_fold As String = "") As Integer
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
''' ```bash
''' /Parser.Locus /locus &lt;locus.txt&gt; /PTT &lt;genomePTT.DIR&gt; /DOOR &lt;genome.opr&gt; [/out &lt;out.DIR&gt;]
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
''' ```bash
''' /Parser.Log2 /in &lt;log2.csv&gt; /PTT &lt;genomePTT.DIR&gt; /DOOR &lt;genome.opr&gt; [/factor 1 /out &lt;outDIR&gt;]
''' ```
''' </summary>
'''

Public Function ParserLog2([in] As String, 
                              PTT As String, 
                              DOOR As String, 
                              Optional factor As String = "", 
                              Optional out As String = "") As Integer
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
''' ```bash
''' /Parser.MAST /sites &lt;mastsites.csv&gt; /ptt &lt;genome-context.pttDIR&gt; /door &lt;genome.opr&gt; [/out &lt;outDIR&gt;]
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
''' ```bash
''' /Parser.Modules /KEGG.Modules &lt;KEGG.modules.DIR&gt; /PTT &lt;genomePTT.DIR&gt; /DOOR &lt;genome.opr&gt; [/locus &lt;union/initx/locus, default:=union&gt; /out &lt;fasta.outDIR&gt;]
''' ```
''' Parsing promoter sequence region for genes in kegg reaction modules
''' </summary>
'''

Public Function ModuleParser(KEGG_Modules As String, 
                                PTT As String, 
                                DOOR As String, 
                                Optional locus As String = "", 
                                Optional out As String = "") As Integer
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
''' ```bash
''' /Parser.Operon /in &lt;footprint.csv&gt; /PTT &lt;PTTDIR&gt; [/out &lt;outDIR&gt; /family /offset &lt;50&gt; /all]
''' ```
''' </summary>
'''
''' <param name="family"> Group the source by family? Or output the source in one fasta set
''' </param>
Public Function ParserNextIterator([in] As String, 
                                      PTT As String, 
                                      Optional out As String = "", 
                                      Optional offset As String = "", 
                                      Optional family As Boolean = False, 
                                      Optional all As Boolean = False) As Integer
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
''' ```bash
''' /Parser.Pathway /KEGG.Pathways &lt;KEGG.pathways.DIR/organismModel.Xml&gt; /src &lt;genomePTT.DIR/gbff.txt&gt; [/DOOR &lt;genome.opr&gt; /locus &lt;union/initx/locus, default:=union&gt; /out &lt;fasta.outDIR&gt;]
''' ```
''' Parsing promoter sequence region for genes in pathways.
''' </summary>
'''
''' <param name="kegg_pathways"> DBget fetch result from ``kegg_tools``.
''' </param>
''' <param name="src"> The genome proteins gene coordination data file. It can be download from NCBI web site.
''' </param>
''' <param name="locus"> Only works when ``/DOOR`` file was presented.
''' </param>
Public Function PathwayParser(KEGG_Pathways As String, 
                                 src As String, 
                                 Optional door As String = "", 
                                 Optional locus As String = "", 
                                 Optional out As String = "") As Integer
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
''' ```bash
''' /Parser.Pathway.Batch /in &lt;pathway.directory&gt; /assembly &lt;NCBI_assembly.directory&gt; [/out &lt;out.directory&gt;]
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
''' ```bash
''' /Parser.RegPrecise.Operons /operon &lt;operons.Csv&gt; /PTT &lt;PTT_DIR&gt; [/corn /DOOR &lt;genome.opr&gt; /id &lt;null&gt; /locus &lt;union/initx/locus, default:=union&gt; /out &lt;outDIR&gt;]
''' ```
''' </summary>
'''

Public Function ParserRegPreciseOperon(operon As String, 
                                          PTT As String, 
                                          Optional door As String = "", 
                                          Optional id As String = "", 
                                          Optional locus As String = "", 
                                          Optional out As String = "", 
                                          Optional corn As Boolean = False) As Integer
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
''' ```bash
''' /Parser.Regulon /inDIR &lt;regulons.inDIR&gt; /out &lt;fasta.outDIR&gt; /PTT &lt;genomePTT.DIR&gt; [/door &lt;genome.opr&gt;]
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
''' ```bash
''' /Parser.Regulon.gb /inDIR &lt;regulons.inDIR&gt; /out &lt;fasta.outDIR&gt; /gb &lt;genbank.gbk&gt; [/door &lt;genome.opr&gt;]
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
''' ```bash
''' /Parser.Regulon.Merged /in &lt;merged.Csv&gt; /out &lt;fasta.outDIR&gt; /PTT &lt;genomePTT.DIR&gt; [/DOOR &lt;genome.opr&gt;]
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
''' ```bash
''' /Regulator.Motifs /bbh &lt;bbh.csv&gt; /regprecise &lt;genome.DIR&gt; [/out &lt;outDIR&gt;]
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
''' ```bash
''' /Regulator.Motifs.Test /hits &lt;familyHits.Csv&gt; /motifs &lt;motifHits.Csv&gt; [/out &lt;out.csv&gt;]
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
''' ```bash
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
''' ```bash
''' /regulon.export /in &lt;sw-tom_out.DIR&gt; /ref &lt;regulon.bbh.xml.DIR&gt; [/out &lt;out.csv&gt;]
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
''' ```bash
''' /Regulon.Reconstruct /bbh &lt;bbh.csv&gt; /genome &lt;RegPrecise.genome.xml&gt; /door &lt;operon.door&gt; [/out &lt;outfile.csv&gt;]
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
''' ```bash
''' /Regulon.Reconstruct2 /bbh &lt;bbh.csv&gt; /genome &lt;RegPrecise.genome.DIR&gt; /door &lt;operons.opr&gt; [/out &lt;outDIR&gt;]
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
''' ```bash
''' /Regulon.Reconstructs /bbh &lt;bbh_EXPORT_csv.DIR&gt; /genome &lt;RegPrecise.genome.DIR&gt; [/door &lt;operon.door&gt; /out &lt;outDIR&gt;]
''' ```
''' Doing the regulon reconstruction job in batch mode.
''' </summary>
'''
''' <param name="bbh"> A directory which contains the bbh export csv data from the localblast tool.
''' </param>
''' <param name="genome"> The directory which contains the RegPrecise bacterial genome downloads data from the RegPrecise web server.
''' </param>
''' <param name="door"> Door file which is the prediction data of the bacterial operon.
''' </param>
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
''' ```bash
''' /Regulon.Test /in &lt;meme.txt&gt; /reg &lt;genome.bbh.regulon.xml&gt; /bbh &lt;maps.bbh.Csv&gt;
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
''' ```bash
''' /RfamSites /source &lt;sourceDIR&gt; [/out &lt;out.fastaDIR&gt;]
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
''' ```bash
''' /seq.logo /in &lt;meme.txt&gt; [/out &lt;outDIR&gt;]
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
''' ```bash
''' /Similarity.Union /in &lt;preSource.fasta.DIR&gt; /meme &lt;meme.txt.DIR&gt; /hits &lt;similarity_hist.Csv&gt; [/out &lt;out.DIR&gt;]
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
''' ```bash
''' /Site.MAST_Scan /mast &lt;mast.xml/DIR&gt; [/batch /out &lt;out.csv&gt;]
''' ```
''' [MAST.Xml] -&gt; [SimpleSegment]
''' </summary>
'''
''' <param name="batch"> If this parameter presented in the CLI, then the parameter /mast will be used as a DIR.
''' </param>
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
''' ```bash
''' /Site.MAST_Scan /mast &lt;mast.xml.DIR&gt; [/out &lt;out.csv.DIR&gt; /num_threads &lt;-1&gt;]
''' ```
''' [MAST.Xml] -&gt; [SimpleSegment]
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
''' ```bash
''' /Site.RegexScan /meme &lt;meme.txt&gt; /nt &lt;nt.fasta&gt; [/batch /out &lt;out.csv&gt;]
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
''' ```bash
''' /site.scan /query &lt;LDM.xml&gt; /subject &lt;subject.fasta&gt; [/out &lt;outDIR&gt;]
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
''' ```bash
''' /SiteHits.Footprints /sites &lt;MotifSiteHits.Csv&gt; /bbh &lt;bbh.Csv&gt; /meme &lt;meme.txt_DIR&gt; /PTT &lt;genome.PTT&gt; /DOOR &lt;DOOR.opr&gt; [/queryHash /out &lt;out.csv&gt;]
''' ```
''' Generates the regulation information.
''' </summary>
'''

Public Function SiteHitsToFootprints(sites As String, 
                                        bbh As String, 
                                        meme As String, 
                                        PTT As String, 
                                        DOOR As String, 
                                        Optional out As String = "", 
                                        Optional queryhash As Boolean = False) As Integer
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
''' ```bash
''' /SWTOM.Compares /query &lt;query.meme.txt&gt; /subject &lt;subject.meme.txt&gt; [/out &lt;outDIR&gt; /no-HTML]
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
''' ```bash
''' /SWTOM.Compares.Batch /query &lt;query.meme.DIR&gt; /subject &lt;subject.meme.DIR&gt; [/out &lt;outDIR&gt; /no-HTML]
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
''' ```bash
''' /SWTOM.LDM /query &lt;ldm.xml&gt; /subject &lt;ldm.xml&gt; [/out &lt;outDIR&gt; /method &lt;pcc/ed/sw; default:=pcc&gt;]
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
''' ```bash
''' /SWTOM.Query /query &lt;meme.txt&gt; [/out &lt;outDIR&gt; /method &lt;pcc&gt; /bits.level 1.6 /minW 6 /no-HTML]
''' ```
''' </summary>
'''
''' <param name="no_HTML"> If this parameter is true, then only the XML result will be export.
''' </param>
Public Function SWTomQuery(query As String, 
                              Optional out As String = "", 
                              Optional method As String = "", 
                              Optional bits_level As String = "", 
                              Optional minw As String = "", 
                              Optional no_html As Boolean = False) As Integer
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
''' ```bash
''' /SWTOM.Query.Batch /query &lt;meme.txt.DIR&gt; [/out &lt;outDIR&gt; /SW-offset 0.6 /method &lt;pcc&gt; /bits.level 1.5 /minW 4 /SW-threshold 0.75 /tom-threshold 0.75 /no-HTML]
''' ```
''' </summary>
'''
''' <param name="no_HTML"> If this parameter is true, then only the XML result will be export.
''' </param>
Public Function SWTomQueryBatch(query As String, 
                                   Optional out As String = "", 
                                   Optional sw_offset As String = "", 
                                   Optional method As String = "", 
                                   Optional bits_level As String = "", 
                                   Optional minw As String = "", 
                                   Optional sw_threshold As String = "", 
                                   Optional tom_threshold As String = "", 
                                   Optional no_html As Boolean = False) As Integer
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
''' ```bash
''' /Tom.Query /query &lt;ldm.xml/meme.txt&gt; [/out &lt;outDIR&gt; /method &lt;pcc/ed; default:=pcc&gt; /cost &lt;0.7&gt; /threshold &lt;0.65&gt; /meme]
''' ```
''' </summary>
'''

Public Function TomQuery(query As String, 
                            Optional out As String = "", 
                            Optional method As String = "", 
                            Optional cost As String = "", 
                            Optional threshold As String = "", 
                            Optional meme As Boolean = False) As Integer
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
''' ```bash
''' /Tom.Query.Batch /query &lt;inDIR&gt; [/out &lt;outDIR&gt; /method &lt;pcc/ed; default:=pcc&gt; /cost 0.7 /threshold &lt;0.65&gt;]
''' ```
''' </summary>
'''

Public Function TomQueryBatch(query As String, 
                                 Optional out As String = "", 
                                 Optional method As String = "", 
                                 Optional cost As String = "", 
                                 Optional threshold As String = "") As Integer
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
''' ```bash
''' /TomTOM /query &lt;meme.txt&gt; /subject &lt;LDM.xml&gt; [/out &lt;outDIR&gt; /method &lt;pcc/ed; default:=pcc&gt; /cost &lt;0.7&gt; /threshold &lt;0.3&gt;]
''' ```
''' </summary>
'''

Public Function TomTOMMethod(query As String, 
                                subject As String, 
                                Optional out As String = "", 
                                Optional method As String = "", 
                                Optional cost As String = "", 
                                Optional threshold As String = "") As Integer
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
''' ```bash
''' /TomTom.LDM /query &lt;ldm.xml&gt; /subject &lt;ldm.xml&gt; [/out &lt;outDIR&gt; /method &lt;pcc/ed/sw; default:=sw&gt; /cost &lt;0.7&gt; /threshold &lt;0.65&gt;]
''' ```
''' </summary>
'''

Public Function LDMTomTom(query As String, 
                             subject As String, 
                             Optional out As String = "", 
                             Optional method As String = "", 
                             Optional cost As String = "", 
                             Optional threshold As String = "") As Integer
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
''' ```bash
''' /TomTOM.Similarity /in &lt;TOM_OUT.DIR&gt; [/out &lt;out.Csv&gt;]
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
''' ```bash
''' /TOMTOM.Similarity.Batch /in &lt;inDIR&gt; [/out &lt;out.csv&gt;]
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
''' ```bash
''' /TomTom.Sites.Groups /in &lt;similarity.csv&gt; /meme &lt;meme.DIR&gt; [/grep &lt;regex&gt; /out &lt;out.DIR&gt;]
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
''' ```bash
''' /Trim.MastSite /in &lt;mastSite.Csv&gt; /locus &lt;locus_tag&gt; /correlations &lt;DIR/name&gt; [/out &lt;out.csv&gt; /cut &lt;0.65&gt;]
''' ```
''' </summary>
'''

Public Function Trim([in] As String, 
                        locus As String, 
                        correlations As String, 
                        Optional out As String = "", 
                        Optional cut As String = "") As Integer
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
''' ```bash
''' /Trim.MEME.Dataset /in &lt;seq.fasta&gt; [/out &lt;out.fasta&gt; /minl 8 /distinct]
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
''' ```bash
''' --build.Regulations /bbh &lt;regprecise.bbhMapped.csv&gt; /mast &lt;mastSites.csv&gt; [/cutoff &lt;0.6&gt; /out &lt;out.csv&gt; /sp &lt;spName&gt; /DOOR &lt;genome.opr&gt; /DOOR.extract]
''' ```
''' Genome wide step 2
''' </summary>
'''
''' <param name="DOOR_extract"> Extract the operon structure genes after assign the operon information.
''' </param>
Public Function Build(bbh As String, 
                         mast As String, 
                         Optional cutoff As String = "", 
                         Optional out As String = "", 
                         Optional sp As String = "", 
                         Optional door As String = "", 
                         Optional door_extract As Boolean = False) As Integer
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
''' ```bash
''' --build.Regulations.From.Motifs /bbh &lt;regprecise.bbhMapped.csv&gt; /motifs &lt;motifSites.csv&gt; [/cutoff &lt;0.6&gt; /sp &lt;spName&gt; /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function BuildFromMotifSites(bbh As String, 
                                       motifs As String, 
                                       Optional cutoff As String = "", 
                                       Optional sp As String = "", 
                                       Optional out As String = "") As Integer
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
''' ```bash
''' --CExpr.WGCNA /mods &lt;CytoscapeNodes.txt&gt; /genome &lt;genome.DIR|*.PTT;*.fna&gt; [/out &lt;DIR.out&gt;]
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
''' ```bash
''' --Dump.KEGG.Family /in &lt;in.fasta&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''
''' <param name="[in]"> The RegPrecise formated title fasta file.
''' </param>
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
''' ```bash
''' --family.statics /sites &lt;motifSites.csv&gt; /mods &lt;directory.kegg_modules&gt;
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
''' ```bash
''' --Get.Intergenic /PTT &lt;genome.ptt&gt; /nt &lt;genome.fasta&gt; [/o &lt;out.fasta&gt; /len 100 /strict]
''' ```
''' </summary>
'''

Public Function GetIntergenic(PTT As String, 
                                 nt As String, 
                                 Optional o As String = "", 
                                 Optional len As String = "", 
                                 Optional strict As Boolean = False) As Integer
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
''' ```bash
''' --GetFasta /bbh &lt;bbhh.csv&gt; /id &lt;subject_id&gt; /regprecise &lt;regprecise.fasta&gt;
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
''' ```bash
''' --hits.diff /query &lt;bbhh.csv&gt; /subject &lt;bbhh.csv&gt; [/reverse]
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
''' ```bash
''' --Intersect.Max /query &lt;bbhh.csv&gt; /subject &lt;bbhh.csv&gt;
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
''' ```bash
''' --logo.Batch -in &lt;inDIR&gt; [/out &lt;outDIR&gt;]
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
''' ```bash
''' --mapped-Back /meme &lt;meme.text&gt; /mast &lt;mast.xml&gt; /ptt &lt;genome.ptt&gt; [/out &lt;out.csv&gt; /offset &lt;10&gt; /atg-dist &lt;250&gt;]
''' ```
''' </summary>
'''

Public Function SiteMappedBack(meme As String, 
                                  mast As String, 
                                  ptt As String, 
                                  Optional out As String = "", 
                                  Optional offset As String = "", 
                                  Optional atg_dist As String = "") As Integer
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
''' ```bash
''' mast.compile /mast &lt;mast.xml&gt; /ptt &lt;genome.ptt&gt; [/no-meme /no-regInfo /p-value 1e-3 /mast-ldm &lt;DIR default:=GCModeller/Regprecise/MEME/MAST_LDM&gt; /atg-dist 250]
''' ```
''' </summary>
'''

Public Function CompileMast(mast As String, 
                               ptt As String, 
                               Optional p_value As String = "", 
                               Optional mast_ldm As String = "", 
                               Optional atg_dist As String = "", 
                               Optional no_meme As Boolean = False, 
                               Optional no_reginfo As Boolean = False) As Integer
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
''' ```bash
''' mast.compile.bulk /source &lt;source_dir&gt; [/ptt &lt;genome.ptt&gt; /atg-dist &lt;500&gt; /no-meme /no-regInfo /p-value 1e-3 /mast-ldm &lt;DIR default:=GCModeller/Regprecise/MEME/MAST_LDM&gt; /related.all]
''' ```
''' Genome wide step 1
''' </summary>
'''
''' <param name="no_meme"> Specific that the mast site construction will without and meme pwm MAST_LDM model.
''' </param>
Public Function CompileMastBuck(source As String, 
                                   Optional ptt As String = "", 
                                   Optional atg_dist As String = "", 
                                   Optional p_value As String = "", 
                                   Optional mast_ldm As String = "", 
                                   Optional no_meme As Boolean = False, 
                                   Optional no_reginfo As Boolean = False, 
                                   Optional related_all As Boolean = False) As Integer
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
''' ```bash
''' --modules.regulates /in &lt;virtualfootprints.csv&gt; [/out &lt;out.DIR&gt; /mods &lt;KEGG_modules.DIR&gt;]
''' ```
''' Exports the Venn diagram model for the module regulations.
''' </summary>
'''
''' <param name="[in]"> The footprints data required of fill out the pathway Class, category and type information before you call this function.
'''                    If the fields is blank, then your should specify the /mods parameter.
''' </param>
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
''' ```bash
''' Motif.Locates -ptt &lt;bacterial_genome.ptt&gt; -meme &lt;meme.txt&gt; [/out &lt;out.csv&gt;]
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
''' ```bash
''' MotifScan -nt &lt;nt.fasta&gt; /motif &lt;motifLDM.xml/LDM_Name/FamilyName&gt; [/delta &lt;default:80&gt; /delta2 &lt;default:70&gt; /offSet &lt;default:5&gt; /out &lt;saved.csv&gt;]
''' ```
''' Scan for the motif site by using fragment similarity.
''' </summary>
'''

Public Function MotifScan(nt As String, 
                             motif As String, 
                             Optional delta As String = "", 
                             Optional delta2 As String = "", 
                             Optional offset As String = "", 
                             Optional out As String = "") As Integer
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
''' ```bash
''' --pathway.regulates -footprints &lt;virtualfootprint.csv&gt; /pathway &lt;DIR.KEGG.Pathways&gt; [/out &lt;./PathwayRegulations/&gt;]
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
''' ```bash
''' regulators.bbh /bbh &lt;bbhDIR/bbh.index.Csv&gt; [/save &lt;save.csv&gt; /direct /regulons /maps &lt;genome.gb&gt;]
''' ```
''' Compiles for the regulators in the bacterial genome mapped on the regprecise database using bbh method.
''' </summary>
'''
''' <param name="regulons"> The data source of the /bbh parameter is comes from the regulons bbh data.
''' </param>
Public Function RegulatorsBBh(bbh As String, 
                                 Optional save As String = "", 
                                 Optional maps As String = "", 
                                 Optional direct As Boolean = False, 
                                 Optional regulons As Boolean = False) As Integer
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
''' ```bash
''' --site.Match /meme &lt;meme.text&gt; /mast &lt;mast.xml&gt; /out &lt;out.csv&gt; [/ptt &lt;genome.ptt&gt; /len &lt;150,200,250,300,350,400,450,500&gt;]
''' ```
''' </summary>
'''
''' <param name="len"> If not specific this parameter, then the function will trying to parsing the length value from the meme text automatically.
''' </param>
Public Function SiteMatch(meme As String, 
                             mast As String, 
                             out As String, 
                             Optional ptt As String = "", 
                             Optional len As String = "") As Integer
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
''' ```bash
''' --site.Matches /meme &lt;DIR.meme.text&gt; /mast &lt;DIR.mast.xml&gt; /out &lt;out.csv&gt; [/ptt &lt;genome.ptt&gt;]
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
''' ```bash
''' --site.Matches.text /meme &lt;DIR.meme.text&gt; /mast &lt;DIR.mast.xml&gt; /out &lt;out.csv&gt; [/ptt &lt;genome.ptt&gt; /fasta &lt;original.fasta.DIR&gt;]
''' ```
''' Using this function for processing the meme text output from the tmod toolbox.
''' </summary>
'''

Public Function SiteMatchesText(meme As String, 
                                   mast As String, 
                                   out As String, 
                                   Optional ptt As String = "", 
                                   Optional fasta As String = "") As Integer
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
''' ```bash
''' --site.stat /in &lt;footprints.csv&gt; [/out &lt;out.csv&gt;]
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
''' ```bash
''' --TCS.Module.Regulations /MiST2 &lt;MiST2.xml&gt; /footprint &lt;footprints.csv&gt; /Pathways &lt;KEGG_Pathways.DIR&gt;
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
''' ```bash
''' --TCS.Regulations /TCS &lt;DIR.TCS.csv&gt; /modules &lt;DIR.mod.xml&gt; /regulations &lt;virtualfootprint.csv&gt;
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
''' ```bash
''' VirtualFootprint.DIP vf.csv &lt;csv&gt; dip.csv &lt;csv&gt;
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
