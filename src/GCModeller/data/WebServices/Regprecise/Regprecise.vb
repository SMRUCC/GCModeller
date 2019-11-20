#Region "Microsoft.VisualBasic::b4b599ef21e8331a44740021fd777609, data\WebServices\Regprecise\Regprecise.vb"

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

    '     Module Regprecise
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: [As], __getRequest, __getResponse, genes, genomes
    '                   genomeStats, regulators, regulog, regulogCollections, regulogCollectionStats
    '                   regulogs, regulon, regulons, release, Retrieves
    '                   searchExtRegulons, searchRegulons, sites, (+3 Overloads) wGetDownload
    ' 
    '         Sub: wGetDownload
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO, System.Net, System.Text
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Data.Regprecise.WebServices
Imports SMRUCC.genomics.Data.Regprecise.WebServices.JSON
Imports SMRUCC.genomics.SequenceModel

Namespace Regprecise


    <Package("Regprecise.WebServices.API",
                  Url:="http://regprecise.lbl.gov/RegPrecise/services.jsp",
                  Description:="<strong>Formats</strong>
                  <br /><br />
Currently two response representations are available:<br />
<li>application/json</li>
<li>application/xml</li>
<br />
<br />
Sample requests using curl:<br />
<li>XML : curl -H ""Accept: application/xml"" -i 'http://regprecise.lbl.gov/Services/rest/genomeStats'</li>
<li>JSON: curl -H ""Accept application/json"" -i 'http://regprecise.lbl.gov/Services/rest/genomeStats'	</li>
                  <br />
                  <br />
If not provided defaults to json.
                  <br />
    <br />
<strong>Notes</strong>
                  <br />
                  <br />
<li>In the descriptions below, a parameter between curly brackets, e.g., {parameter}, stands for a value to be supplied by the user (without the curly brackets).</li>
<li>In the descriptions below, among arguments in square brackets, e.g. [regulogId={regulogId},genomeId={genomeId}], only one argument can be submited (in this case either regulogId or genomeId).</li>
 <br />
<br />
   <br />
<strong>Example of the client code</strong>
                  <br />
                  <br />
The template program in perl that can be run to access several of the web services and parse the output data can be dowloaded <a href=""http://regprecise.lbl.gov/RegPrecise/regprecise_api.tar.gz"">here</a>. 
                  The program is organized in two perl scripts: <br />
<li>RegPreciseAdapter.pm - a perl module that provides access to the individual web services</li>
<li>regulons.pl - an example of workflow that can be implemented using a combination of several web services</li>
                  <br />
 Dependecies: <li>JSON.pm</li>",
                  Publisher:="PSNovichkov@lbl.gov; <br />
                        rodionov@burnham.org")>
    Public Module Regprecise

        ReadOnly __DynamicsJSONMapped As Dictionary(Of Type, KeyValuePair(Of Type, Type)) =
            New Dictionary(Of Type, KeyValuePair(Of Type, Type)) From {
 _
            {GetType(gene), ORMapperFactory.Ref(Of gene)},
            {GetType(genome), ORMapperFactory.Ref(Of genome)},
            {GetType(genomeStat), ORMapperFactory.Ref(Of genomeStat)},
            {GetType(JSON.regulator), ORMapperFactory.Ref(Of JSON.regulator)},
            {GetType(regulogCollection), ORMapperFactory.Ref(Of regulogCollection)},
            {GetType(regulogCollectionStat), ORMapperFactory.Ref(Of regulogCollectionStat)},
            {GetType(regulog), ORMapperFactory.Ref(Of regulog)},
            {GetType(JSON.regulon), ORMapperFactory.Ref(Of JSON.regulon)},
            {GetType(regulonRef), ORMapperFactory.Ref(Of regulonRef)},
            {GetType(site), ORMapperFactory.Ref(Of site)}
        }

        ''' <summary>
        ''' 只对集合类型有效
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <returns></returns>
        Private Function Retrieves(Of T As Class)(url As String) As T()
            Dim json As String = __getRequest(url)
            Dim refType As Type = GetType(T)
            Dim ref = __DynamicsJSONMapped(refType)
            ' 首先尝试集合类型的反序列化
            Dim value As Object = json.LoadObject(ref.Key)
            Dim array As T()

            If value Is Nothing Then
                value = json.LoadObject(ref.Value)
                If value Is Nothing Then
                    array = New T() {}
                Else
                    array = New T() {DirectCast(ref.Value.GetValue(value, refType.Name), T)}
                End If
                Call Console.Write("+")
            Else
                array = ref.Key.GetValue(Of T())(value, refType.Name)
            End If

            Return array
        End Function

        ''' <summary>
        ''' + Retrieves a list of genomes that have at least one reconstructed regulon.
        ''' </summary>
        ''' <returns></returns>
        <ExportAPI("genomes",
               Info:="Retrieves a list of genomes that have at least one reconstructed regulon.",
               Example:="http://regprecise.lbl.gov/Services/rest/genomes")>
        Public Function genomes() As <FunctionReturns("Returns a list of genomes with the following data:<br />
<li>genomeId - genome identifier</li>
<li>name - genome name</li>
<li>taxonomyId - NCBI taxonomy id</li>")> SMRUCC.genomics.Data.Regprecise.WebServices.JSON.genome()
            Call "Downloading bacterial genomes entry data from: http://regprecise.lbl.gov/Services/rest/genomes".__DEBUG_ECHO
            Dim retValue As Data.Regprecise.WebServices.JSON.genome() = Retrieves(Of Data.Regprecise.WebServices.JSON.genome)("http://regprecise.lbl.gov/Services/rest/genomes")
            Call $"Retrieve {retValue.Length} genomes entry...".__DEBUG_ECHO
            Return retValue
        End Function

        ''' <summary>
        ''' + Retrieves description of regulog collections of a specific type.
        ''' </summary>
        ''' <param name="type">Type of a collection of regulogs</param>
        ''' <returns></returns>
        <ExportAPI("regulogCollections",
               Info:="Retrieves description of regulog collections of a specific type.",
               Example:="http://regprecise.lbl.gov/Services/rest/regulogCollections?collectionType=taxGroup")>
        Public Function regulogCollections(<Parameter("type", "Type of a collection of regulogs, value can be:
<li>taxGroup - collections by taxonomic groups</li>
<li>tf - collections classified by orthologous transcription factor</li>
<li>tfFam - collections classified by transcription factor family</li>
<li>rnaFam - collections classified by RNA regulatory element family</li>
<li>pathway - collections classified by metabolic pathway or biological process</li>
<li>effector - collections classified by effector molecule or environmental signal</li>")> type As String) As <FunctionReturns("Returns a list of regulog collections. Each regulog collection is provided with the following data: 
<li>collectionType - type of regulog collection</li>
<li>collectionId - identifier of collection</li>
<li>name - collection name</li>
<li>className - name of collection class</li>")> SMRUCC.genomics.Data.Regprecise.WebServices.JSON.regulogCollection()
            Dim url As String = $"http://regprecise.lbl.gov/Services/rest/regulogCollections?collectionType={type}"
            Dim retValue = Retrieves(Of regulogCollection)(url)
            Return retValue
        End Function

        ''' <summary>
        ''' + Retrieves a list of regulogs that belongs to a specific collection
        ''' </summary>
        ''' <param name="collectionType">Type of a collection of regulogs</param>
        ''' <param name="collectionId">Identifier of collection</param>
        ''' <returns></returns>
        <ExportAPI("regulogs",
               Info:="Retrieves a list of regulogs that belongs to a specific collection",
               Example:="http://regprecise.lbl.gov/Services/rest/regulogs?collectionType=taxGroup&collectionId=1")>
        Public Function regulogs(<Parameter("collectionType", "Type of a collection of regulogs, value can be one of:
<li>taxGroup - collections by taxonomic groups</li>
<li>tf - collections classified by orthologous transcription factor</li>
<li>tfFam - collections classified by transcription factor family</li>
<li>rnaFam - collections classified by RNA regulatory element family</li>
<li>pathway - collections classified by metabolic pathway or biological process</li>
<li>effector - collections classified by effector molecule or environmental signal</li>")> collectionType As String,
                             <Parameter("collectionId", "Identifier of collection")> collectionId As Integer) As <FunctionReturns("Returns a list of regulogs. Each regulog is provided with the following data: 
<li>regulogId - identifier of regulog</li>
<li>regulatorName - name of regulator</li>
<li>regulatorFamily - family of regulator</li>
<li>regulationType - type of regulation: either TF (transcription factor) or RNA</li>
<li>taxonName - name of taxonomic group</li>
<li>effector - effector molecule or environmental signal of a regulator</li>
<li>pathway - metabolic pathway or biological process controlled by a regulator </li>
")> SMRUCC.genomics.Data.Regprecise.WebServices.JSON.regulog()
            Dim url As String = $"http://regprecise.lbl.gov/Services/rest/regulogs?collectionType={collectionType}&collectionId={collectionId}"
            Dim retValue = Retrieves(Of regulog)(url)
            Return retValue
        End Function

        ''' <summary>
        ''' Retrieves a regulog
        ''' </summary>
        ''' <param name="regulogId">Identifier of regulog</param>
        ''' <returns></returns>
        <ExportAPI("regulog",
               Info:="Retrieves a regulog",
               Example:="http://regprecise.lbl.gov/Services/rest/regulog?regulogId=621")>
        Public Function regulog(<Parameter("regulogId", "Identifier of regulog")> regulogId As Integer) As <FunctionReturns("Returns a regulog. A regulog is provided with the following data: 
<li>regulogId - identifier of regulog</li>
<li>regulatorName - name of regulator</li>
<li>regulatorFamily - family of regulator</li>
<li>regulationType - type of regulation: either TF (transcription factor) or RNA</li>
<li>taxonName - name of taxonomic group</li>
<li>effector - effector molecule or environmental signal of a regulator</li>
<li>pathway - metabolic pathway or biological process controlled by a regulator </li>
")> SMRUCC.genomics.Data.Regprecise.WebServices.JSON.regulog
            Dim url As String = $"http://regprecise.lbl.gov/Services/rest/regulog?regulogId={regulogId}"
            Dim retData As String = __getRequest(url)
            Dim retValue As regulog = retData.LoadJSON(Of regulog)
            Return retValue
        End Function

        ''' <summary>
        ''' + Retrieves a list of regulons belonging to either a particular regulog or to a particular genome, input Either regulog identifier or genome identifier
        ''' </summary>
        ''' <param name="Id">Either regulog identifier or genome identifier</param>
        ''' <param name="genome"></param>
        ''' <returns></returns>
        <ExportAPI("regulons",
               Info:="Retrieves a list of regulons belonging to either a particular regulog or to a particular genome, input Either regulog identifier or genome identifier",
               Example:="http://regprecise.lbl.gov/Services/rest/regulons?regulogId=621")>
        Public Function regulons(<Parameter("Id", "Either regulog identifier or genome identifier")> Id As Integer,
                             Optional genome As Boolean = True) As <FunctionReturns("Returns a list of regulons. A regulon is provided with the following data: 
<li>regulonId - identifier of a regulon</li>
<li>regulogId - identifier of a regulog</li>
<li>genomeId - identifier of a genome</li>
<li>genomeName - name of a genome</li>
<li>regulatorName - name of a regulator</li>
<li>regulatorFamily - famliy of a regulator</li>
<li>regulationType - type of regulation: either TF (transcription factor) or RNA</li>
<li>effector - effector molecule or environmentla signal of a regulator</li>
<li>pathway - metabolic pathway or biological process controlled by regulator</li>
")> SMRUCC.genomics.Data.Regprecise.WebServices.JSON.regulon()
            Dim url As String = If(genome,
            $"http://regprecise.lbl.gov/Services/rest/regulons?genomeId={Id}",
            $"http://regprecise.lbl.gov/Services/rest/regulons?regulogId={Id}")
            Dim retValue = Retrieves(Of JSON.regulon)(url)
            Return retValue
        End Function

        ''' <summary>
        ''' Retrieves a regulon.
        ''' </summary>
        ''' <param name="regulonId"></param>
        ''' <returns></returns>
        <ExportAPI("regulon",
               Info:="Retrieves a regulon.",
               Example:="http://regprecise.lbl.gov/Services/rest/regulon?regulonId=6423")>
        Public Function regulon(<Parameter("regulonId", "Regulon identifier")> regulonId As Integer) As <FunctionReturns("Returns a regulon with the following data: 
<li>regulonId - identifier of a regulon</li>
<li>regulogId - identifier of a regulog</li>
<li>genomeId - identifier of a genome</li>
<li>genomeName - name of a genome</li>
<li>regulatorName - name of a regulator</li>
<li>regulatorFamily - famliy of a regulator</li>
<li>regulationType - type of regulation: either TF (transcription factor) or RNA</li>
<li>taxonName - name of taxonomic group</li>
<li>effector - effector molecule or environmentla signal of a regulator</li>
<li>pathway - metabolic pathway or biological process controlled by regulator</li>")> JSON.regulon
            Dim url As String = $"http://regprecise.lbl.gov/Services/rest/regulon?regulonId={regulonId}"
            Dim retData As String = __getRequest(url)
            Dim retValue As JSON.regulon = retData.LoadJSON(Of JSON.regulon)
            Return retValue
        End Function

        ''' <summary>
        ''' + Retrieves a list of regulators that belong to either a given regulon or regulog.
        ''' </summary>
        ''' <param name="Id">Regulon or regulog identifier</param>
        ''' <param name="regulon"></param>
        ''' <returns></returns>
        <ExportAPI("regulators",
               Info:="Retrieves a list of regulators that belong to either a given regulon or regulog.",
               Example:="http://regprecise.lbl.gov/Services/rest/regulators?regulonId=6423")>
        Public Function regulators(<Parameter("Id", "Regulon or regulog identifier")> Id As Integer,
                               Optional regulon As Boolean = True) As <FunctionReturns("Returns a list of regulators. A regulator is provided with the following data: 
<li>regulonId - identifier of regulon to which a regulator belongs to</li>
<li>name - name of regulator</li>
<li>locusTag - locus tag of regulator gene in GeneBank</li>
<li>vimssId - identifier of regulator gene in MicrobesOnline database </li>
<li>regulatorFamily - family of a regulator</li>
")> JSON.regulator()
            Dim url As String = If(regulon,
            $"http://regprecise.lbl.gov/Services/rest/regulators?regulonId={Id}",
            $"http://regprecise.lbl.gov/Services/rest/regulators?regulogId={Id}")
            Dim retValue = Retrieves(Of JSON.regulator)(url)
            Return retValue
        End Function

        ''' <summary>
        ''' + Retrieves a list of regulated genes that belongs to either a given regulon or regulog.
        ''' </summary>
        ''' <param name="Id">Regulon or regulog identifier</param>
        ''' <param name="regulon"></param>
        ''' <returns></returns>
        <ExportAPI("genes",
               Info:="Retrieves a list of regulated genes that belongs to either a given regulon or regulog.",
               Example:="http://regprecise.lbl.gov/Services/rest/genes?regulonId=6423")>
        Public Function genes(<Parameter("Regulon or regulog identifier")> Id As Integer,
                          Optional regulon As Boolean = True) As <FunctionReturns("Returns a list of genes with the following data: 
<li>regulonId - identifier of a regulon </li>
<li>name - name of gene</li>
<li>locusTag - locus tag of a gene in GeneBank</li>
<li>vimssId - identifier of gene in MicrobesOnline database </li>
<li>function - gene function</li>
")> gene()
            Dim url As String = If(regulon,
            $"http://regprecise.lbl.gov/Services/rest/genes?regulonId={Id}",
            $"http://regprecise.lbl.gov/Services/rest/genes?regulogId={Id}")
            Dim retValue = Retrieves(Of gene)(url)
            Return retValue
        End Function

        ''' <summary>
        ''' + Retrieves a list of regulatory sites (TF binding sites or RNA regulatory elements) and associated regulated genes that belong to a given regulon or regulog.
        ''' </summary>
        ''' <param name="Id"></param>
        ''' <param name="regulon"></param>
        ''' <returns></returns>
        <ExportAPI("sites",
               Info:="Retrieves a list of regulatory sites (TF binding sites or RNA regulatory elements) and associated regulated genes that belong to a given regulon or regulog.",
               Example:="http://regprecise.lbl.gov/Services/rest/sites?regulonId=6423")>
        Public Function sites(<Parameter("Id", "Regulon or regulog identifier")> Id As Integer,
                          Optional regulon As Boolean = True) As <FunctionReturns("Returns a list of regulatory sites and regulated genes with the following data: 
<li>regulonId - identifier of regulon</li>
<li>sequence - sequence of a regualtory site</li>
<li>score - score of a regualtory site</li>
<li>position - position of a regulatory site relative to the start of a downstream gene </li>
<li>geneLocusTag - locus tag of a downstream gene in GeneBank</li>
<li>geneVIMSSId - identifier of a downstream gene in MicrobesOnline database </li>")> site()
            Dim url As String = If(regulon,
            $"http://regprecise.lbl.gov/Services/rest/sites?regulonId={Id}",
            $"http://regprecise.lbl.gov/Services/rest/sites?regulogId={Id}")
            Dim retValue = Retrieves(Of site)(url)
            Return retValue
        End Function

        ''' <summary>
        ''' + Retrieves general statistics on regulog collections of a particular type.
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        <ExportAPI("regulogCollectionStats",
               Info:="Retrieves general statistics on regulog collections of a particular type.",
               Example:="http://regprecise.lbl.gov/Services/rest/regulogCollectionStats?collectionType=taxGroup")>
        Public Function regulogCollectionStats(<Parameter("type", "Collection type. value can be one of the:
<li>taxGroup - collections by taxonomic groups</li>
<li>tf - collections classified by orthologous transcription factor</li>
<li>tfFam - collections classified by transcription factor family</li>
<li>rnaFam - collections classified by RNA regulatory element family</li>
<li>pathway - collections classified by metabolic pathway or biological process</li>
<li>effector - collections classified by effector molecule or environmental signal</li>")> type As String) As <FunctionReturns("Returns a list of regulog collections with the following data: 
<li>collectionType - type of collection</li>
<li>collectionId - identifier of collection</li>
<li>name - collection name</li>
<li>className - name of collection class</li>
<li>totalGenomeCount - total number of genomes that have at least one regulon in collection</li>
<li>totalRegulogCount - total number of regulogs in collection</li>
<li>tfCount - number of different transcription factors in collection</li>
<li>tfRegulogCount - number of TF-controlled regulogs in collection</li>
<li>tfSiteCount - number of TF binding sites in collection</li>
<li>rnaCount - number of RNA families in collection</li>
<li>rnaRegulogCount - number of RNA-controlled regulogs in collection</li>
<li>rnaSiteCount - number of RNA regulatory sites in collection</li>")> SMRUCC.genomics.Data.Regprecise.WebServices.JSON.regulogCollectionStat()
            Dim url As String = $"http://regprecise.lbl.gov/Services/rest/regulogCollectionStats?collectionType={type}"
            Dim retValue = Retrieves(Of regulogCollectionStat)(url)
            Return retValue
        End Function

        ''' <summary>
        ''' + Retrieves a list of regulon references by the name/locus tag of either regulator or target regulated genes, Object type (either 'gene' or 'regulator') and text value representing either locusTag or name
        ''' </summary>
        ''' <param name="objType">Object type to search, value can be "gene" or "regulator"</param>
        ''' <param name="text">Either locus tag or a name of gene/regulator to search</param>
        ''' <returns></returns>
        <ExportAPI("searchRegulons",
               Info:="Retrieves a list of regulon references by the name/locus tag of either regulator or target regulated genes, Object type (either 'gene' or 'regulator') and text value representing either locusTag or name",
               Example:="http://regprecise.lbl.gov/Services/rest/searchRegulons?objType=regulator&text=argR")>
        Public Function searchRegulons(<Parameter("objType", "Object type to search, value can be ""gene"" or ""regulator""")> objType As String,
                                   <Parameter("text", "Either locus tag or a name of gene/regulator to search")> text As String) As <FunctionReturns("Returns a list of regulon references with the following data: 
<li>regulonId - identifier of regulon</li>
<li>genomeName - name of genome</li>
<li>regulatorName - the name of regulator</li>
<li>foundObjType - found object type (either 'gene' or 'regulator')</li>
<li>foundObjName - found object name (or locusTag)</li>")> SMRUCC.genomics.Data.Regprecise.WebServices.JSON.regulonRef()
            Dim url As String = $"http://regprecise.lbl.gov/Services/rest/searchRegulons?objType={objType}&text={text}"
            Dim retValue = Retrieves(Of regulonRef)(url)
            Return retValue
        End Function

        ''' <summary>
        ''' Retrieves a list of regulon references by genome taxonomy id and comma-delimited list of gene locusTags, inputs the NCBI taxonomy id and comma-delimited list of gene locusTags
        ''' </summary>
        ''' <param name="taxonomyId">NCBI taxonomy id of a genome</param>
        ''' <param name="locusTags">comma-delimited list of gene locusTags</param>
        ''' <returns></returns>
        <ExportAPI("searchExtRegulons",
               Info:="Retrieves a list of regulon references by genome taxonomy id and comma-delimited list of gene locusTags, inputs the NCBI taxonomy id and comma-delimited list of gene locusTags",
               Example:="http://regprecise.lbl.gov/Services/rest/searchExtRegulons?taxonomyId=882&locusTags=DVU0043,DVU0694,DVU3234,DVU3233,DVU0910,DVU3384,DVU0863")>
        Public Function searchExtRegulons(<Parameter("taxonomyId", "NCBI taxonomy id of a genome")> taxonomyId As Integer,
                                     <Parameter("locusTags", "comma-delimited list of gene locusTags")> locusTags As String) As <FunctionReturns("Returns a list of regulon references with the following data: 
<li>regulonId - identifier of regulon</li>
<li>genomeName - name of genome</li>
<li>regulatorName - the name of regulator</li>
<li>foundObjType - found object type </li>
<li>foundObjName - found object name (or locusTag)</li>
")> Object
            Dim url As String = $"http://regprecise.lbl.gov/Services/rest/searchExtRegulons?taxonomyId={taxonomyId}&locusTags={locusTags}"
        End Function

        <Extension> Friend Function [As](Of T, V)(o As T) As V
            Return DirectCast(CObj(o), V)
        End Function

        Private Function __getRequest(strUrl As String) As String
            Dim request As System.Net.HttpWebRequest, timeOut As Integer = 5
            request = WebRequest.Create(strUrl).As(Of System.Net.HttpWebRequest)
            request.Method = "GET"

            Try
RETRY:          Dim json As String = __getResponse(request)
                Return json
            Catch ex As Exception
                Call App.LogException(ex, $"{NameOf(__getRequest)} ==> {NameOf(__getResponse)}")

                If timeOut >= 0 Then
                    timeOut -= 1
                    GoTo RETRY
                Else
                    Call FileIO.FileSystem.WriteAllText($"./request.Exceptions.log", $"[null] {strUrl}" & vbCrLf, append:=True)
                    Return "null"
                End If
            End Try
        End Function

        Private Function __getResponse(request As HttpWebRequest) As String
            Dim response = request.GetResponse.As(Of System.Net.HttpWebResponse)
            Dim s = response.GetResponseStream()
            Dim strDate As New Value(Of String)
            Dim strValue As StringBuilder = New StringBuilder(2048)
            Dim Reader As StreamReader = New StreamReader(s, Encoding.UTF8)

            Do While Not (strDate = Reader.ReadLine()) Is Nothing
                Call strValue.AppendLine(strDate)
            Loop

            Return strValue.ToString
        End Function

        ''' <summary>
        ''' Retrieves version and date of the current release of RegPrecise database
        ''' </summary>
        ''' <returns></returns>
        <ExportAPI("release", Info:="Retrieves version and date of the current release of RegPrecise database",
               Example:="http://regprecise.lbl.gov/Services/rest/release")>
        Public Function release() As <FunctionReturns("Returns the current database release information: 
<li>majorVersion - major version of the current database release</li> 
<li>mionrVersion - minor version of the current database release</li> 
<li>releaseDate - date of the current database release</li>")> String
            Dim verJSON As String = __getRequest("http://regprecise.lbl.gov/Services/rest/release")
            Call $"Current Regprecise release version is: {verJSON}".__DEBUG_ECHO
            Return verJSON
        End Function

        ''' <summary>
        ''' + Retrieves a general statistics on regulons and regulatory sites in genomes
        ''' </summary>
        ''' <returns></returns>
        <ExportAPI("genomeStats",
               Info:="Retrieves a general statistics on regulons and regulatory sites in genomes",
               Example:="http://regprecise.lbl.gov/Services/rest/genomeStats")>
        Public Function genomeStats() As <FunctionReturns("Returns a list of genomes with the following data:
<li>genomeId - genome identifier</li>
<li>name - genome name</li>
<li>taxonomyId - NCBI taxonomy id</li>
<li>tfRegulonCount - total number of TF-controlled regulons reconstructed in genome</li>
<li>tfSiteCount - total number of TF binding sites in genome</li>
<li>rnaRegulonCount - total number of RNA-controlled regulons reconstructed in genome</li>
<li>rnaSiteCount - total number of RNA regulatory sites in genome</li>")> genomeStat()
            Dim url As String = "http://regprecise.lbl.gov/Services/rest/genomeStats"
            Dim retValue = Retrieves(Of genomeStat)(url)
            Return retValue
        End Function

        Sub New()
            Call Settings.Session.Initialize()
        End Sub

        ''' <summary>
        ''' Downloads the entirely Regprecise database.(为了减轻服务器的负担，这里不会使用并行化拓展来进行多线程下载，所以可能时间会比较长)
        ''' </summary>
        ''' <param name="EXPORT">Directory for stores the downloaded database file.</param>
        ''' <returns></returns>
        <ExportAPI("wget.Download", Info:="Downloads the entirely Regprecise database.")>
        Public Function wGetDownload(<Parameter("DIR.EXPORT",
                                                "Directory for stores the downloaded database file. If this directory value is null, then the default location of the GCModeller repository root directory will be used.")>
                                     Optional EXPORT As String = "",
                                     <Parameter("Repository.Updates", "Does the downloader should update the regprecise genome stats database first? if not then the old database will be used.")>
                                     Optional Updates As Boolean = False,
                                     Optional disableRegulatorDownloads As Boolean = False) As Boolean

            If String.IsNullOrEmpty(EXPORT) Then
                EXPORT = Settings.Session.SettingsFile.RepositoryRoot & "/Regprecise/"
            End If

            Dim genomes As Data.Regprecise.WebServices.JSON.genome()
            Dim ErrLog As New LogFile($"{EXPORT}/{NameOf(wGetDownload)}.log")
            Dim path As String = $"{EXPORT}/{NameOf(genomes)}.xml"

            If Updates Then
                genomes = Regprecise.genomes
                Call genomes.GetXml.SaveTo(path)
                Call Regprecise.genomeStats.GetXml.SaveTo($"{EXPORT}/{NameOf(genomeStats)}.xml")
            Else
                genomes = path.LoadXml(Of genome())
            End If

            Call wGetDownload(genomes, EXPORT, ErrLog, disableRegulatorDownloads)

            Call release.SaveTo($"{EXPORT}/regprecise.current.release.json")
            Call ErrLog.Save()

            Return True
        End Function

        <ExportAPI("wget.Download")>
        Public Function wGetDownload(genomes As IEnumerable(Of JSON.genome),
                                     repository As String,
                                     ErrLog As LogFile,
                                     Optional disableRegulatorDownloads As Boolean = False) As Boolean
            Dim path As String = $"{repository}/{NameOf(genomes)}.xml"

            Call "Start to downloads regulons....".__DEBUG_ECHO

            For Each genome As Data.Regprecise.WebServices.JSON.genome In genomes
                Call wGetDownload(genome, repository, ErrLog, disableRegulatorDownloads)
                Call genome.name.__DEBUG_ECHO()
            Next

            Return True
        End Function

        <ExportAPI("wget.Download")>
        Public Function wGetDownload(genome As genome,
                                     repository As String,
                                     ErrLog As LogFile,
                                     Optional disableRegulatorDownloads As Boolean = False) As Boolean
            Dim Path As String = $"{repository}/{NameOf(Regprecise.regulons)}/{genome.genomeId}.{genome.name.NormalizePathString}.xml"
            Dim regulons As JSON.regulon()

            If Path.FileExists Then
                regulons = Path.LoadXml(Of JSON.regulon())
                If regulons.IsNullOrEmpty Then
                    GoTo Download
                End If
            Else
Download:       regulons = Regprecise.regulons(genome.genomeId)
                Call regulons.GetXml.SaveTo(Path)
            End If

            For Each regulon As JSON.regulon In regulons '下载调控因子的数据
                Call wGetDownload(regulon, genome, repository, ErrLog, disableRegulatorDownloads)
                Call Console.Write(".")
            Next

            Return True
        End Function

        ''' <summary>
        ''' 下载得到调控记录的时候同时也会得到从KEGG数据库之中所下载得到的蛋白质序列Fasta数据
        ''' </summary>
        ''' <param name="regulon"></param>
        ''' <param name="genome"></param>
        ''' <param name="repository"></param>
        ''' <param name="ErrLog"></param>
        ''' <param name="disableRegulatorDownloads"></param>
        <ExportAPI("wget.Download")>
        Public Sub wGetDownload(regulon As JSON.regulon,
                                genome As genome,
                                repository As String,
                                ErrLog As LogFile,
                                Optional disableRegulatorDownloads As Boolean = False)

            Dim regulators As JSON.regulator()
            Dim sites As site()
            Dim Path As String = $"{repository}/{NameOf(Regprecise.regulators)}/{genome.name.NormalizePathString}/{regulon.regulogId}.xml"

            If Path.FileExists Then
                regulators = Path.LoadXml(Of JSON.regulator())
                If regulators.IsNullOrEmpty Then
                    GoTo Download
                End If
            Else
Download:       regulators = Regprecise.regulators(regulon.regulonId)
                Call regulators.GetXml.SaveTo(Path)
            End If

            Path = $"{repository}/{NameOf(Regprecise.sites)}/{genome.name.NormalizePathString}/{regulon.regulogId}.xml"
            If Path.FileExists Then
                sites = Path.LoadXml(Of site())
                If sites.IsNullOrEmpty Then
                    GoTo DownloadSites
                End If
            Else
DownloadSites:
                sites = Regprecise.sites(regulon.regulonId)
                Call sites.GetXml.SaveTo(Path)
            End If

            Dim siteTags As String() = sites.Select(Of String)(Function(site) $"{site.geneLocusTag}:{site.geneVIMSSId}:{site.position}").Distinct.ToArray
            Dim sitesFasta = sites.Select(Function(site) FastaReaders.Site.CreateFrom(site, genome.name))

            Path = $"{repository}/Fasta/{NameOf(sites)}/{regulon.regulonId}.{genome.name.NormalizePathString}.fasta"
            Call CType(sitesFasta, FASTA.FastaFile).Save(Path)

            If disableRegulatorDownloads Then
                Return
            End If

            For Each regulator As JSON.regulator In regulators
                If regulator Is Nothing Then
                    Continue For
                End If
                Path = $"{repository}/Fasta/{NameOf(regulators)}/{genome.name.NormalizePathString}/{regulator.locusTag}.fasta"
                If Not Path.FileExists() Then ' 假若蛋白质的序列fasta文件不存在的话，则从KEGG数据库服务器上面进行下载
                    Dim RegulatorFasta = KEGGDownloader.RegulatorDownloads(regulator, ErrLog)
                    If RegulatorFasta Is Nothing Then
                        Continue For
                    Else
                        RegulatorFasta.Sites = siteTags
                    End If

                    Call RegulatorFasta.SaveTo(Path)
                End If
            Next
        End Sub
    End Module
End Namespace
