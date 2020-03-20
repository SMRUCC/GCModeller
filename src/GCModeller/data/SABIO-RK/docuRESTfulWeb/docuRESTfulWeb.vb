Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports sbXML = SMRUCC.genomics.Model.SBML.Level3.XmlFile

Public Module docuRESTfulWeb

    ''' <summary>
    ''' Get a single kinetic law entry by SABIO entry ID
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    Public Function getModelById(id As String, Optional level As Integer = 2, Optional version As Integer = 3, Optional annotation As String = "identifier") As SabiorkSBML
        Dim url = SabiorkSBML.URL_SABIORK_KINETIC_LAWS_QUERY & id
        Dim sbml As String = url.GET

        Return sbml.LoadXml(Of SabiorkSBML)
    End Function

    ''' <summary>
    ''' Search for SABIO kinetic law entries by a query string
    ''' </summary>
    ''' <param name="q"></param>
    ''' <returns></returns>
    Public Function searchKineticLaws(q As Dictionary(Of QueryFields, String), Optional cache$ = "./") As sbXML
        Static api As New Dictionary(Of String, ModelQuery)
        Return api.ComputeIfAbsent(
            key:=cache,
            lazyValue:=Function()
                           Return New ModelQuery(cache)
                       End Function) _
                  .Query(Of sbXML)(q)
    End Function
End Module
