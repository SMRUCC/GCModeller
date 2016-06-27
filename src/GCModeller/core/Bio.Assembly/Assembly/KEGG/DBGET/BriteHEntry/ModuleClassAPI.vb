Imports LANS.SystemsBiology.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Language

Namespace Assembly.KEGG.DBGET.BriteHEntry

    Public Class ModuleClassAPI

        ''' <summary>
        ''' A
        ''' </summary>
        Protected ReadOnly __getType As Func(Of PathwayBrief, String)
        ''' <summary>
        ''' B
        ''' </summary>
        Protected ReadOnly __getClass As Func(Of PathwayBrief, String)
        ''' <summary>
        ''' C
        ''' </summary>
        Protected ReadOnly __getCategory As Func(Of PathwayBrief, String)
        Protected ReadOnly _geneHash As Dictionary(Of String, PathwayBrief())
        Protected ReadOnly _modHash As Dictionary(Of PathwayBrief)

        Public ReadOnly Property Genehash As Dictionary(Of String, PathwayBrief())
            Get
                Return _geneHash
            End Get
        End Property

        Public Overloads ReadOnly Property [GetXType] As Func(Of PathwayBrief, String)
            Get
                Return __getType
            End Get
        End Property

        Public ReadOnly Property GetXClass As Func(Of PathwayBrief, String)
            Get
                Return __getClass
            End Get
        End Property

        Public ReadOnly Property GetXCategory As Func(Of PathwayBrief, String)
            Get
                Return __getCategory
            End Get
        End Property

        Public ReadOnly Property Modules As PathwayBrief()

        Sub New([getType] As Func(Of PathwayBrief, String),
            getClass As Func(Of PathwayBrief, String),
            getCategory As Func(Of PathwayBrief, String),
            geneHash As Dictionary(Of String, PathwayBrief()),
            getName As Func(Of PathwayBrief, String))
            __getType = [getType]
            __getClass = getClass
            __getCategory = getCategory
            __getName = getName
            _geneHash = geneHash

            _Modules = _geneHash.Values.MatrixAsIterator.Distinct.ToArray
            _modHash = Modules.ToDictionary
        End Sub

        Protected Sub New()
        End Sub

        ''' <summary>
        ''' Get description
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Function GetModule(name As String) As PathwayBrief
            If _modHash.ContainsKey(name) Then
                Return _modHash(name)
            Else
                Return Nothing
            End If
        End Function

        Public Sub GetBriteInfo(x As PathwayBrief, ByRef A As String, ByRef B As String, ByRef C As String)
            A = __getType(x)
            B = __getClass(x)
            C = __getCategory(x)
        End Sub

        Public Function GetA(x As PathwayBrief) As String
            Return __getType(x)
        End Function

        Public Function GetB(x As PathwayBrief) As String
            Return __getClass(x)
        End Function

        Public Function GetC(x As PathwayBrief) As String
            Return __getCategory(x)
        End Function

        Public Function GetBriteInfo(name As String, ByRef A As String, ByRef B As String, ByRef C As String) As PathwayBrief
            Dim x As PathwayBrief = GetModule(name)

            If x Is Nothing Then
                Return Nothing
            Else
                Call GetBriteInfo(x, A, B, C)
            End If

            Return x
        End Function

        Public Function GetName(x As PathwayBrief) As String
            Return __getName(x)
        End Function

        Protected ReadOnly __getName As Func(Of PathwayBrief, String)

        Private Shared Function __geneHash(mods As PathwayBrief()) As Dictionary(Of String, PathwayBrief())
            Dim LQuery = (From x As PathwayBrief In mods
                          Let genes As String() = x.GetPathwayGenes
                          Where Not StringHelpers.IsNullOrEmpty(genes)
                          Select (From g As String
                                  In genes
                                  Select g,
                                  modX = x)).MatrixAsIterator
            Dim Groups = (From x
                          In LQuery
                          Select x
                          Group x By x.g Into Group) _
                           .ToDictionary(Function(x) x.g,
                                         Function(x) x.Group.ToArray(Function(d) d.modX))
            Return Groups
        End Function

        Public Shared Function FromPathway(DIR As String) As ModuleClassAPI
            Dim brites As New BriteHEntry.ModsBrite(Of bGetObject.Pathway)
            Dim hash = __geneHash(__getFiles(Of bGetObject.Pathway)(DIR))
            Return New ModuleClassAPI(AddressOf brites.GetType,
                                  AddressOf brites.GetClass,
                                  AddressOf brites.GetCategory, hash,
                                  AddressOf __getPathwayName)
        End Function

        Private Shared Function __getFiles(Of T As PathwayBrief)(DIR As String) As PathwayBrief()
            Dim files As IEnumerable(Of String) = ls - l - r - wildcards("*.xml") <= DIR
            Dim LQuery As PathwayBrief() =
                LinqAPI.Exec(Of PathwayBrief) <= From xml As String
                                                 In files.AsParallel
                                                 Select DirectCast(xml.LoadXml(Of T), PathwayBrief)
            Return LQuery
        End Function

        Private Shared Function __getPathwayName(x As PathwayBrief) As String
            Return DirectCast(x, bGetObject.Pathway).Name
        End Function

        Private Shared Function __getModuleName(x As PathwayBrief) As String
            Return DirectCast(x, bGetObject.Module).Name
        End Function

        Public Shared Function FromModules(DIR As String) As ModuleClassAPI
            Dim brites As New BriteHEntry.ModsBrite(Of bGetObject.Module)
            Dim hash = __geneHash(__getFiles(Of bGetObject.Module)(DIR))
            Return New ModuleClassAPI(AddressOf brites.GetType,
                                  AddressOf brites.GetClass,
                                  AddressOf brites.GetCategory, hash,
                                  AddressOf __getModuleName)
        End Function
    End Class
End Namespace