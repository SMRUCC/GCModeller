#Region "Microsoft.VisualBasic::8be1a31fc308062cd2134801af3b2f01, ..\GCModeller\data\STRING\WebAPI\StringAPI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat

Namespace StringDB.WebAPI

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
    <Package("string-db.API", Description:="STRING has an application programming interface (API) which enables you to get the data without using the 
graphical user interface of the web page. The API is convenient if you need to programmatically access some 
information but still do not want to download the entire dataset. There are several scenarios when it is 
practical to use it. For example, you might need to access some interaction from your own scripts or want to 
incorporate some information in STRING to a web page.

We currently provide an implementation using HTTP, where the database information is accessed by HTTP requests. 
Due to implementation and licensing reasons, The API provide methods to query individual items only, similar to 
the web site. If you need access to bulk data, you can download the entire dataset by signing the academic 
license agreement.

http://[database]/[access]/[format]/[request]?[parameter]=[value]", Cites:="", Publisher:="", Url:="http://string-db.org/api/")>
    Public Module StringAPI

        ''' <summary>
        ''' Main entry point of STRING
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property StringDb As New Database With {.strData = "string-db.org/api"}
        ''' <summary>
        ''' Alternative entry point of STRING
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property StringEMBL As New Database With {.strData = "string.embl.de/api"}
        ''' <summary>
        ''' The sister database of STRING
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property StitchEMBL As New Database With {.strData = "stitch.embl.de/api"}

        Public Function CreateURI() As String
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="idList"></param>
        ''' <returns>返回成功的个数</returns>
        ''' <remarks>http://string-db.org/api/psi-mi/interactions?identifier=XC_1184</remarks>
        ''' 
        <ExportAPI("Downloads.Interactions",
                   Info:="http://string-db.org/api/psi-mi/interactions?identifier=gene_id")>
        Public Function DownloadInteractions(<Parameter("lst.LocusId", "The locus_id list of genes' for downloading the interaction data.")>
                                             idList As IEnumerable(Of String),
                                             <Parameter("Dir.Export", "Default directory is current work directory.")>
                                             Optional EXPORT As String = "./") As Integer
            Dim i As Integer = 0

            Call FileIO.FileSystem.CreateDirectory(String.Format("{0}/Images/", EXPORT))

            For Each id As String In idList
                Dim URL As String = String.Format("http://string-db.org/api/psi-mi/interactions?identifier={0}", id)
                Dim XmlFile As String = String.Format("{0}/{1}.xml", EXPORT, id)
                Dim ImageFile As String = String.Format("{0}/Images/{1}.png", EXPORT, id)

                If Not FileIO.FileSystem.FileExists(XmlFile) OrElse FileIO.FileSystem.GetFileInfo(XmlFile).Length = 0 Then
                    Call URL.GET.SaveTo(XmlFile)
                End If

                URL = String.Format("http://string-db.org/api/image/network?identifier={0}", id)

                If Not ImageFile.FileExists Then
                    i += URL.DownloadFile(ImageFile)
                End If
            Next

            Return i
        End Function

        <ExportAPI("Downloads.Interactions",
                   Info:="http://string-db.org/api/psi-mi/interactions?identifier=gene_id")>
        Public Function DownloadInteractions(PTT As PTT,
                                             <Parameter("Dir.Export", "Default directory is current work directory.")>
                                             Optional EXPORT As String = "./") As Integer

            Dim lstID As String() = PTT.GeneIDList.ToArray
            Return StringAPI.DownloadInteractions(lstID, EXPORT)
        End Function
    End Module
End Namespace
