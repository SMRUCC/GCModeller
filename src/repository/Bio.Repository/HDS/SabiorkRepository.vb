Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Data.SABIORK
Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports SMRUCC.genomics.Data.SABIORK.TabularDump

Public Class SabiorkRepository : Implements IDisposable

    ReadOnly cache As StreamPack
    ReadOnly webRequest As ModelQuery
    ReadOnly enzyme_class As Dictionary(Of String, String)

    Private disposedValue As Boolean

    Sub New(file As Stream)
        Me.cache = New StreamPack(file, meta_size:=32 * 1024 * 1024, [readonly]:=False)
        Me.webRequest = New ModelQuery(cache)
        Me.enzyme_class = Enums(Of EnzymeClasses)() _
            .ToDictionary(Function(c) CInt(c).ToString,
                          Function(c)
                              Return c & "." & c.Description
                          End Function)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ec_number"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' url = `https://sabiork.h-its.org/sabioRestWebServices/searchKineticLaws/sbml?q=ecnumber:${num}`;
    ''' </remarks>
    Public Function GetByECNumber(ec_number As String) As SbmlDocument
        Dim q As New Dictionary(Of QueryFields, String) From {
            {QueryFields.ECNumber, ec_number}
        }
        Dim str As String = webRequest.QueryCacheText(q)
        Dim result As SbmlDocument = ModelQuery.parseSBML(str)

        ' 20221112 andalso write kinetics model data 
        ' to the repository package
        If Not result Is Nothing Then
            Call saveKineticsModel(ec_number, model:=result)
        End If

        Return result
    End Function

    ' V = Vmax[S] / ( KM + [S])
    ' V = Kcat[E]t[S] / ( KM + [S])
    ' [S] substracte concentration
    ' [E]t enzyme concentration
    ' Vmax = kcat[E]

    Private Sub saveKineticsModel(ec_number As String, model As SbmlDocument)
        Dim mathList = model.mathML.ToDictionary(Function(a) a.Name, Function(a) a.Value)
        Dim numbers As String() = ec_number.Split("."c)
        Dim pathDir As String = $"/{enzyme_class(numbers(Scan0).ToString)}/{numbers.Skip(1).JoinBy("/")}"
        Dim path As String
        Dim math As LambdaExpression
        Dim mathId As String
        Dim json As String
        Dim kineticisModel As EnzymeCatalystKineticLaw
        Dim indexer As New SBMLInternalIndexer(model)

        For Each rxn As SBMLReaction In model.sbml.model.listOfReactions
            path = $"{pathDir}/{rxn.id}.json"
            mathId = "KL_" & rxn.kineticLaw.annotation.sabiork.kineticLawID
            math = mathList(mathId)

            If math.lambda Is Nothing Then
                Continue For
            Else
                kineticisModel = EnzymeCatalystKineticLaw.Create(rxn, math, doc:=indexer)
                json = rxn.GetJson
            End If

            Call cache.Delete(path)
            Call cache.WriteText(json, path)
        Next
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call cache.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
