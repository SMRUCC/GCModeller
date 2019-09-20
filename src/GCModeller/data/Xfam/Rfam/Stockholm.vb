#Region "Microsoft.VisualBasic::d7c0b2b93ccec28e25bea876ebc373d3, Xfam\Rfam\Stockholm.vb"

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

    ' Class Stockholm
    ' 
    '     Properties: AC, Alignments, AU, BM, CB
    '                 CC, CL, DC, DE, DR
    '                 FR, GA, ID, KW, MB
    '                 NC, NE, NH, NL, PI
    '                 RA, RC, RF, RL, RM
    '                 RN, RT, SE, SM, SQ
    '                 SS, SS_cons, TC, TN, TP
    '                 WK
    ' 
    '     Function: __fieldsParser, DatabaseParser, Parser, ParseSchema
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions

''' <summary>
''' Rfam.seed
''' 
''' Stockholm format is a Multiple sequence alignment format used by Pfam and Rfam to disseminate protein and RNA sequence alignments. 
''' The alignment editors Ralee and Belvu support Stockholm format as do the probabilistic database search tools, 
''' Infernal and HMMER, and the phylogenetic analysis tool Xrate.
''' </summary>
Public Class Stockholm

#Region "NOTE:  请勿在修改这些域的名称，因为这些事需要用作为字典的键名的"

#Region "Compulsory fields"

    ''' <summary>
    ''' Accession number:           
    ''' Accession number In form PFxxxxx (Pfam) Or RFxxxxx (Rfam).
    ''' </summary>
    ''' <returns></returns>
    <Column("locusId")> Public Property AC As String
    ''' <summary>
    ''' Identification:             
    ''' One word name For family.
    ''' </summary>
    ''' <returns></returns>
    <Column("Identifier")> Public Property ID As String
    ''' <summary>
    ''' Definition 
    ''' Short description of family.
    ''' </summary>
    ''' <returns></returns>
    <Column("Definition")> Public Property DE As String
    ''' <summary>
    ''' Author         
    ''' Authors of the entry.
    ''' </summary>
    ''' <returns></returns>
    <Column("Authors")> Public Property AU As String
    ''' <summary>
    ''' Source Of seed           
    ''' The source suggesting the seed members belong To one family.
    ''' </summary>
    ''' <returns></returns>
    <Column("Source")> Public Property SE As String
    ''' <summary>
    ''' Source Of Structure  
    ''' The source(prediction Or publication) Of the consensus RNA secondary Structure used by Rfam.
    ''' </summary>
    ''' <returns></returns>
    <Column("Structures")> Public Property SS As String
    ''' <summary>
    ''' Build method          
    ''' Command line used To generate the model
    ''' </summary>
    ''' <returns></returns>
    <Column("BuildMethod")> Public Property BM As String
    ''' <summary>
    ''' Search method            
    ''' Command line used To perform the search
    ''' </summary>
    ''' <returns></returns>
    <Column("SearchMethod")> Public Property SM As String
    ''' <summary>
    ''' Gathering threshold    
    ''' Search threshold To build the full alignment.
    ''' </summary>
    ''' <returns></returns>
    <Column("threshold")> Public Property GA As String
    ''' <summary>
    ''' Trusted Cutoff   
    ''' Lowest sequence score (And domain score For Pfam) Of match In the full alignment.
    ''' </summary>
    ''' <returns></returns>
    <Column("cutoff.trusted")> Public Property TC As String
    ''' <summary>
    ''' Noise Cutoff              
    ''' Highest sequence score (And domain score For Pfam) Of match Not In full alignment.
    ''' </summary>
    ''' <returns></returns>
    <Column("cutoff.noise")> Public Property NC As String
    ''' <summary>
    ''' Type                        
    ''' Type of family -- presently Family, Domain, Motif Or Repeat for Pfam. -- a tree with roots Gene, Intron Or Cis-reg for Rfam.
    ''' </summary>
    ''' <returns></returns>
    <Column("type")> Public Property TP As String
    ''' <summary>
    ''' Sequence                          
    ''' Number of sequences in alignment.
    ''' </summary>
    ''' <returns></returns>
    <Ignored> Public Property SQ As String
#End Region

#Region "Optional fields"
    ''' <summary>
    ''' Database Comment:           
    ''' Comment about database reference.
    ''' </summary>
    ''' <returns></returns>
    <Column("Db.Comment")> Public Property DC As String
    ''' <summary>
    ''' Database Reference:         
    ''' Reference to external database.
    ''' </summary>
    ''' <returns></returns>
    <Column("Db.Reference")> Public Property DR As String
    ''' <summary>
    ''' Reference Comment:          
    ''' Comment about literature reference.
    ''' </summary>
    ''' <returns></returns>
    <Column("Ref.Comment")> Public Property RC As String
    ''' <summary>
    ''' Reference Number:           
    ''' Reference Number.
    ''' </summary>
    ''' <returns></returns>
    <Column("Ref.Number")> Public Property RN As String
    ''' <summary>
    ''' Reference Medline:          
    ''' Eight digit medline UI number.
    ''' </summary>
    ''' <returns></returns>
    <Column("Ref.Medline")> Public Property RM As String
    ''' <summary>
    ''' Reference Title:            
    ''' Reference Title.
    ''' </summary>
    ''' <returns></returns>
    <Column("Ref.Title")> Public Property RT As String
    ''' <summary>
    ''' Reference Author:           
    ''' Reference Author
    ''' </summary>
    ''' <returns></returns>
    <Column("Ref.Authors")> Public Property RA As String
    ''' <summary>
    ''' Reference Location:         
    ''' Journal location.
    ''' </summary>
    ''' <returns></returns>
    <Column("Ref.Location")> Public Property RL As String
    ''' <summary>
    ''' Previous identifier:        
    ''' Record of all previous ID lines.
    ''' </summary>
    ''' <returns></returns>
    Public Property PI As String
    ''' <summary>
    ''' Keywords :                      
    ''' Keywords.
    ''' </summary>
    ''' <returns></returns>
    <Column("Keywords")> Public Property KW As String
    ''' <summary>
    ''' Comment :                       
    ''' Comments.
    ''' </summary>
    ''' <returns></returns>
    <Column("Comments")> Public Property CC As String
    ''' <summary>
    ''' Pfam accession:             
    ''' Indicates a nested domain.
    ''' </summary>
    ''' <returns></returns>
    <Column("NestedDomain")> Public Property NE As String
    ''' <summary>
    ''' Location :                      
    ''' Location of nested domains - sequence ID, start And end of insert.
    ''' </summary>
    ''' <returns></returns>
    <Column("NestedLocation")> Public Property NL As String
    ''' <summary>
    ''' Wikipedia link:             
    ''' Wikipedia page
    ''' </summary>
    ''' <returns></returns>
    <Column("wiki")> Public Property WK As String
    ''' <summary>
    ''' Clan:                       
    ''' Clan accession
    ''' </summary>
    ''' <returns></returns>
    <Column("clan")> Public Property CL As String
    ''' <summary>
    ''' Membership :                      
    ''' Used for listing Clan membership
    ''' </summary>
    ''' <returns></returns>
    <Column("membership")> Public Property MB As String
#End Region

#Region "For embedding trees"

    ''' <summary>
    ''' New Hampshire                
    ''' A tree In New Hampshire eXtended format.
    ''' </summary>
    ''' <returns></returns>
    <Column("New Hampshire")> Public Property NH As String
    ''' <summary>
    ''' Tree ID                      
    ''' A unique identifier For the Next tree.
    ''' </summary>
    ''' <returns></returns>
    <Column("Tree.Id")> Public Property TN As String
#End Region

#Region "Other"

    ''' <summary>
    ''' False discovery Rate:         
    ''' A method used To Set the bit score threshold based On the ratio Of expected false positives to true positives. Floating point number between 0 And 1.
    ''' </summary>
    ''' <returns></returns>
    <Column("FalseRate")> Public Property FR As String
    ''' <summary>
    ''' Calibration method:           
    ''' Command line used To calibrate the model (Rfam only, release 12.0 And later)
    ''' </summary>
    ''' <returns></returns>
    <Column("Calibration.Method")> Public Property CB As String
#End Region

    <Ignored> Public Property Alignments As FastaFile

    Public Property SS_cons As String
    Public Property RF As String

#End Region

    Public Shared Function Parser(source As String, schema As PropertyInfo()) As Stockholm
        Dim Tokens As String() = source.LineTokens
        Dim fields As Dictionary(Of String, String) =
            __fieldsParser((From line As String In Tokens
                            Where Not String.IsNullOrWhiteSpace(line) AndAlso
                                line.First = "#"c
                            Select line).ToArray)
        Dim aln = (From line As String In Tokens
                   Where Not String.IsNullOrWhiteSpace(line) AndAlso
                       line.First <> "#"c
                   Let sp As String() = line.Trim.Split
                   Let id As String = sp.First
                   Let seq As String = sp.Last
                   Let fa As SequenceModel.FASTA.FastaSeq =
                       New SequenceModel.FASTA.FastaSeq With {
                          .Headers = {id},
                          .SequenceData = seq
                       }
                   Select fa).ToArray
        Dim family As New Stockholm With {
            .Alignments = New SequenceModel.FASTA.FastaFile(aln)
        }

        For Each prop As PropertyInfo In schema
            Call prop.SetValue(family, fields.TryGetValue(prop.Name))
        Next

        Return family
    End Function

    Public Shared Function ParseSchema() As PropertyInfo()
        Dim typeInfo As Type = GetType(Stockholm)
        Dim props As PropertyInfo() = typeInfo.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
        Dim stringType As Type = GetType(String)
        props = (From prop As PropertyInfo In props
                 Where prop.CanWrite AndAlso
                     prop.PropertyType.Equals(stringType)
                 Select prop).ToArray
        Return props
    End Function

    Private Shared Function __fieldsParser(lines As String()) As Dictionary(Of String, String)
        Dim LQuery = (From x As String In lines
                      Let tokens As String() = Strings.Split(x, "   ")
                      Let name As String = tokens.First.Split.Last
                      Let value As String = tokens.Last
                      Select name, value
                      Group By name Into Group) _
                         .ToDictionary(Function(x) x.name,
                                       Function(x) x.Group.Select(Function(xx) xx.value).JoinBy(" "))
        Return LQuery
    End Function

    Public Shared Function DatabaseParser(path As String) As Dictionary(Of String, Stockholm)
        Dim inText As String = FileIO.FileSystem.ReadAllText(path)
        Dim datas As String() = Regex.Split(inText, "^[/][/]$", RegexOptions.Multiline)
        Dim schema As PropertyInfo() = Stockholm.ParseSchema
        Dim LQuery = (From x As String In datas.AsParallel
                      Let Rfam As Stockholm = Stockholm.Parser(x, schema)
                      Where Not String.IsNullOrEmpty(Rfam.AC)
                      Select Rfam).ToDictionary(Function(x) x.AC)
        Return LQuery
    End Function
End Class
