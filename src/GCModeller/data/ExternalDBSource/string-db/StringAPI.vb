#Region "Microsoft.VisualBasic::03fa0975ff852a2373a45146742620f4, ..\GCModeller\data\ExternalDBSource\string-db\StringAPI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.StringDB.StringAPI.Database.Format

Namespace StringDB

    ''' <summary>
    ''' STRING has an application programming interface (API) which enables you to get the data without using the 
    ''' graphical user interface of the web page. The API is convenient if you need to programmatically access some 
    ''' information but still do not want to download the entire dataset. There are several scenarios when it is 
    ''' practical to use it. For example, you might need to access some interaction from your own scripts or want to 
    ''' incorporate some information in STRING to a web page.
    '''
    ''' We currently provide an implementation using HTTP, where the database information is accessed by HTTP requests. 
    ''' Due to implementation and licensing reasons, The API provide methods to query individual items only, similar to 
    ''' the web site. If you need access to bulk data, you can download the entire dataset by signing the academic 
    ''' license agreement.
    ''' 
    ''' http://[database]/[access]/[format]/[request]?[parameter]=[value]
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <Cite(Title:="STRING v9.1: protein-protein interaction networks, with increased coverage and integration",
          Journal:="Nucleic Acids Res",
          Pages:="D808-15",
          Keywords:="Algorithms
Data Interpretation, Statistical
Data Mining
*Databases, Protein
Internet
*Protein Interaction Mapping
Systems Integration
User-Computer Interface",
          DOI:="10.1093/nar/gks1094", ISSN:="1362-4962 (Electronic)
0305-1048 (Linking)",
          Issue:="Database issue",
          Authors:="Franceschini, A.
Szklarczyk, D.
Frankild, S.
Kuhn, M.
Simonovic, M.
Roth, A.
Lin, J.
Minguez, P.
Bork, P.
von Mering, C.
Jensen, L. J.",
          Abstract:="Complete knowledge of all direct and indirect interactions between proteins in a given cell would represent an important milestone towards a comprehensive description of cellular mechanisms and functions. 
Although this goal is still elusive, considerable progress has been made-particularly for certain model organisms and functional systems. Currently, protein interactions and associations are annotated at various levels of detail in online resources, ranging from raw data repositories to highly formalized pathway databases. 
For many applications, a global view of all the available interaction data is desirable, including lower-quality data and/or computational predictions. The STRING database (http://string-db.org/) aims to provide such a global perspective for as many organisms as feasible. 
Known and predicted associations are scored and integrated, resulting in comprehensive protein networks covering >1100 organisms. 
Here, we describe the update to version 9.1 of STRING, introducing several improvements: 
          <p>(i) we extend the automated mining of scientific texts for interaction information, to now also include full-text articles; 
          <p>(ii) we entirely re-designed the algorithm for transferring interactions from one model organism to the other; and 
          <p>(iii) we provide users with statistical information on any functional enrichment observed in their networks.",
          Volume:=41, Year:=2013,
          AuthorAddress:="Institute of Molecular Life Sciences and Swiss Institute of Bioinformatics, University of Zurich, Switzerland.",
          PubMed:=23203871)>
    <[PackageNamespace]("string-db.API", Description:="STRING has an application programming interface (API) which enables you to get the data without using the 
graphical user interface of the web page. The API is convenient if you need to programmatically access some 
information but still do not want to download the entire dataset. There are several scenarios when it is 
practical to use it. For example, you might need to access some interaction from your own scripts or want to 
incorporate some information in STRING to a web page.

We currently provide an implementation using HTTP, where the database information is accessed by HTTP requests. 
Due to implementation and licensing reasons, The API provide methods to query individual items only, similar to 
the web site. If you need access to bulk data, you can download the entire dataset by signing the academic 
license agreement.

http://[database]/[access]/[format]/[request]?[parameter]=[value]", Cites:="", Publisher:="", Url:="http://string-db.org/api/")>
    Public Class StringAPI

        Public MustInherit Class APIToken
            Protected Friend strData As String

            Public MustOverride Function GetToken() As String

            Public Overrides Function ToString() As String
                Return strData
            End Function
        End Class

        ''' <summary>
        ''' Main entry point of STRING
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly StringDb As Database = New Database With {.strData = "string-db.org/api"}
        ''' <summary>
        ''' Alternative entry point of STRING
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly StringEMBL As Database = New Database With {.strData = "string.embl.de/api"}
        ''' <summary>
        ''' The sister database of STRING
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly StitchEMBL As Database = New Database With {.strData = "stitch.embl.de/api"}

        Public Class Database : Inherits StringAPI.APIToken

            ''' <summary>
            ''' JSON format either as a list of hashes/dictionaries, or as a plain list (if there is only one value to be returned per record)
            ''' </summary>
            ''' <remarks></remarks>
            Public ReadOnly Json As Format = New Format With {.strData = "json", .Requests = New Database.Format.Request() {Request.resolve, Request.resolveList, Request.abstracts, Request.abstractsList}}
            ''' <summary>
            ''' Tab separated values, with a header line
            ''' </summary>
            ''' <remarks></remarks>
            Public ReadOnly tsv As Format = New Format With {.strData = "tsv", .Requests = New Request() {Request.resolve, Request.resolveList, Request.abstracts, Request.abstractsList, Request.actions, Request.actionsList, Request.interactors, Request.interactorsList}}
            ''' <summary>
            ''' Tab separated values, without header line
            ''' </summary>
            ''' <remarks></remarks>
            Public ReadOnly tsvNoHeader As Format = New Format With {.strData = "tsv-no-header", .Requests = New Request() {Request.resolve, Request.resolveList, Request.abstracts, Request.abstractsList, Request.actions, Request.actionsList, Request.interactors, Request.interactorsList}}
            ''' <summary>
            ''' The interaction network in PSI-MI 2.5 XML format
            ''' </summary>
            ''' <remarks></remarks>
            Public ReadOnly psiMi As Format = New Format With {.strData = "psi-mi", .Requests = New Request() {Request.interactors, Request.interactorsList, Request.interactions, Request.interactionsList}}
            ''' <summary>
            ''' Tab-delimited form of PSI-MI (similar to tsv, modeled after the IntAct specification. (Easier to parse, but contains less information than the XML format.)
            ''' </summary>
            ''' <remarks></remarks>
            Public ReadOnly psiMiTab As Format = New Format With {.strData = "psi-mi-tab", .Requests = New Request() {Request.interactors, Request.interactorsList, Request.interactions, Request.interactionsList}}
            ''' <summary>
            ''' The network image
            ''' </summary>
            ''' <remarks></remarks>
            Public ReadOnly Image As Format = New Format With {.strData = "image", .Requests = New Request() {Request.network, Request.networkList}}

            Public Class Format : Inherits StringAPI.APIToken

                Public Property Requests As Request()

                Public Class Request : Inherits StringAPI.APIToken

                    Private Sub New()
                    End Sub

                    ''' <summary>
                    ''' List of items that match (in name or identifier) the query item
                    ''' </summary>
                    ''' <remarks></remarks>
                    Public Shared ReadOnly resolve As Request = New Request With {.strData = "resolve"}
                    ''' <summary>
                    ''' List of items that match (in name or identifier) the query items
                    ''' </summary>
                    ''' <remarks></remarks>
                    Public Shared ReadOnly resolveList As Request = New Request With {.strData = "resolveList"}
                    ''' <summary>
                    ''' List of abstracts that contain the query item
                    ''' </summary>
                    ''' <remarks></remarks>
                    Public Shared ReadOnly abstracts As Request = New Request With {.strData = "abstracts"}
                    ''' <summary>
                    ''' List of abstracts that contain any of the query items
                    ''' </summary>
                    ''' <remarks></remarks>
                    Public Shared ReadOnly abstractsList As Request = New Request With {.strData = "abstractsList"}
                    ''' <summary>
                    ''' List of interaction partners for the query item
                    ''' </summary>
                    ''' <remarks></remarks>
                    Public Shared ReadOnly interactors As Request = New Request With {.strData = "interactors"}
                    ''' <summary>
                    ''' List of interaction partners for any of the query items
                    ''' </summary>
                    ''' <remarks></remarks>
                    Public Shared ReadOnly interactorsList As Request = New Request With {.strData = "interactorsList"}
                    ''' <summary>
                    ''' Action partners for the query item
                    ''' </summary>
                    ''' <remarks></remarks>
                    Public Shared ReadOnly actions As Request = New Request With {.strData = "actions"}
                    ''' <summary>
                    ''' Action partners for any of the query items
                    ''' </summary>
                    ''' <remarks></remarks>
                    Public Shared ReadOnly actionsList As Request = New Request With {.strData = "actionsList"}
                    ''' <summary>
                    ''' Interaction network in PSI-MI 2.5 format or PSI-MI-TAB format (similar to tsv)
                    ''' </summary>
                    ''' <remarks></remarks>
                    Public Shared ReadOnly interactions As Request = New Request With {.strData = "interactions"}
                    ''' <summary>
                    ''' Interaction network as above, but for a list of identifiers
                    ''' </summary>
                    ''' <remarks></remarks>
                    Public Shared ReadOnly interactionsList As Request = New Request With {.strData = "interactionsList"}
                    ''' <summary>
                    ''' The network image for the query item
                    ''' </summary>
                    ''' <remarks></remarks>
                    Public Shared ReadOnly network As Request = New Request With {.strData = "network"}
                    ''' <summary>
                    ''' The network image for the query items
                    ''' </summary>
                    ''' <remarks></remarks>
                    Public Shared ReadOnly networkList As Request = New Request With {.strData = "networkList"}

                    Public MustInherit Class Parameter : Inherits StringAPI.APIToken

                        Protected _strValue As String

                        Protected MustOverride ReadOnly Property Name As String

                        ''' <summary>
                        ''' required parameter for single item, e.g. DRD1_HUMAN
                        ''' </summary>
                        ''' <remarks></remarks>
                        Public Class identifier : Inherits Parameter

                            Sub New(Id As String)
                                MyBase._strValue = Id
                            End Sub

                            Protected Overrides ReadOnly Property Name As String
                                Get
                                    Return "identifier"
                                End Get
                            End Property
                        End Class
                        ''' <summary>
                        ''' required parameter for multiple items, e.g.DRD1_HUMAN%0DDRD2_HUMAN
                        ''' </summary>
                        ''' <remarks></remarks>
                        Public Class identifiers : Inherits Parameter

                            Sub New(IdList As String())
                                Dim sBuilder As StringBuilder = New StringBuilder(1024)
                                For Each item In IdList
                                    Call sBuilder.Append(item & "%0D")
                                Next
                                Call sBuilder.Remove(sBuilder.Length - 3, 3)

                                MyBase._strValue = sBuilder.ToString
                            End Sub

                            Protected Overrides ReadOnly Property Name As String
                                Get
                                    Return "identifiers"
                                End Get
                            End Property
                        End Class
                        ''' <summary>
                        ''' For resolve requests: only-ids get the list of only the STRING identifiers (full by default) For abstract requests: use colon pmids for alternative shapes of the pubmed identifier
                        ''' </summary>
                        ''' <remarks></remarks>
                        Public Class format : Inherits Parameter

                            Protected Overrides ReadOnly Property Name As String
                                Get
                                    Return "format"
                                End Get
                            End Property
                        End Class
                        ''' <summary>
                        ''' Taxon identifiers (e.g. Human 9606, see: http://www.uniprot.org/taxonomy)
                        ''' </summary>
                        ''' <remarks></remarks>
                        Public Class species : Inherits Parameter

                            Protected Overrides ReadOnly Property Name As String
                                Get
                                    Return "species"
                                End Get
                            End Property
                        End Class
                        ''' <summary>
                        ''' Maximum number of nodes to return, e.g 10.
                        ''' </summary>
                        ''' <remarks></remarks>
                        Public Class limit : Inherits Parameter


                            Protected Overrides ReadOnly Property Name As String
                                Get
                                    Return "limit"
                                End Get
                            End Property
                        End Class
                        ''' <summary>
                        ''' Threshold of significance to include a interaction, a number between 0 and 1000
                        ''' </summary>
                        ''' <remarks></remarks>
                        Public Class required_score : Inherits Parameter


                            Protected Overrides ReadOnly Property Name As String
                                Get
                                    Return "required_score"
                                End Get
                            End Property
                        End Class
                        ''' <summary>
                        ''' Number of additional nodes in network (ordered by score), e.g./ 10
                        ''' </summary>
                        ''' <remarks></remarks>
                        Public Class additional_network_nodes : Inherits Parameter


                            Protected Overrides ReadOnly Property Name As String
                                Get
                                    Return "additional_network_nodes"
                                End Get
                            End Property
                        End Class
                        ''' <summary>
                        ''' The style of edges in the network. evidence for colored multilines. confidence for singled lines where hue correspond to confidence score. (actions for stitch only)
                        ''' </summary>
                        ''' <remarks></remarks>
                        Public Class network_flavor : Inherits Parameter


                            Protected Overrides ReadOnly Property Name As String
                                Get
                                    Return "network_flavor"
                                End Get
                            End Property
                        End Class
                        ''' <summary>
                        ''' Your identifier for us.
                        ''' </summary>
                        ''' <remarks></remarks>
                        Public Class caller_identity : Inherits Parameter


                            Protected Overrides ReadOnly Property Name As String
                                Get
                                    Return "caller_identity"
                                End Get
                            End Property
                        End Class

                        Public Overrides Function GetToken() As String
                            Return String.Format("{0}={1}", Me.Name, Me._strValue)
                        End Function
                    End Class

                    Public Overrides Function GetToken() As String
                        Return MyBase.strData
                    End Function
                End Class

                Public Overrides Function GetToken() As String
                    Return MyBase.strData
                End Function
            End Class

            Public Overrides Function GetToken() As String
                Return MyBase.strData
            End Function
        End Class

        Public Shared Function CreateURI() As String
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="IdList"></param>
        ''' <returns>返回成功的个数</returns>
        ''' <remarks>http://string-db.org/api/psi-mi/interactions?identifier=XC_1184</remarks>
        ''' 
        <ExportAPI("Downloads.Interactions",
                   Info:="http://string-db.org/api/psi-mi/interactions?identifier=gene_id")>
        Public Shared Function DownloadInteractions(<Parameter("lst.LocusId", "The locus_id list of genes' for downloading the interaction data.")>
                                                    IdList As Generic.IEnumerable(Of String),
                                                    <Parameter("Dir.Export", "Default directory is current work directory.")>
                                                    Optional ExportDir As String = "./") As Integer
            Dim i As Integer = 0

            Call FileIO.FileSystem.CreateDirectory(String.Format("{0}/Images/", ExportDir))

            For Each id As String In IdList
                Dim URL As String = String.Format("http://string-db.org/api/psi-mi/interactions?identifier={0}", id)
                Dim XmlFile As String = String.Format("{0}/{1}.xml", ExportDir, id)
                Dim ImageFile As String = String.Format("{0}/Images/{1}.png", ExportDir, id)

                If Not FileIO.FileSystem.FileExists(XmlFile) OrElse FileIO.FileSystem.GetFileInfo(XmlFile).Length = 0 Then
                    Call URL.GET.SaveTo(XmlFile)
                End If

                URL = String.Format("http://string-db.org/api/image/network?identifier={0}", id)

                If Not FileIO.FileSystem.FileExists(ImageFile) OrElse FileIO.FileSystem.GetFileInfo(ImageFile).Length = 0 Then
                    i += URL.DownloadFile(ImageFile)
                End If
            Next

            Return i
        End Function

        <ExportAPI("Downloads.Interactions",
                   Info:="http://string-db.org/api/psi-mi/interactions?identifier=gene_id")>
        Public Shared Function DownloadInteractions(PTT As SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,
                                                    <Parameter("Dir.Export", "Default directory is current work directory.")>
                                                    Optional ExportDir As String = "./") As Integer
            Dim lstID As String() = PTT.GeneIDList.ToArray
            Return StringAPI.DownloadInteractions(lstID, ExportDir)
        End Function
    End Class
End Namespace
