Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports [Module] = Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps.DataFrameColumnAttribute

''' <summary>
''' The sandbox engine output raw data file format
''' </summary>
Public Class Raw : Implements IDisposable

    Public Const Magic$ = "GCModeller"

#Region "Cellular Modules"

    ''' <summary>
    ''' 由基因转录出来的mRNA的编号列表
    ''' </summary>
    ''' <returns></returns>
    <[Module]("Message-RNA")>
    Public Property mRNAId As Index(Of String)
    ''' <summary>
    ''' 由基因转录出来的其他的RNA分子的编号列表
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <[Module]("Component-RNA")>
    Public Property RNAId As Index(Of String)
    ''' <summary>
    ''' 由mRNA翻译出来的多肽链的Id列表
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <[Module]("Polypeptide")>
    Public Property Polypeptide As Index(Of String)
    ''' <summary>
    ''' 由一条或者多条多肽链修饰之后得到的最终的蛋白质的编号列表
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <[Module]("Protein")>
    Public Property Proteins As Index(Of String)
    ''' <summary>
    ''' 代谢物列表
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <[Module]("Metabolite")>
    Public Property Metabolites As Index(Of String)
    ''' <summary>
    ''' 反应过程编号列表
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <[Module]("Reaction-Flux")>
    Public Property Reactions As Index(Of String)

#End Region

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Protected Function GetModules() As PropertyInfo()
        Return GetType(Raw) _
            .GetProperties(PublicProperty) _
            .Where(Function(prop)
                       Return prop.PropertyType Is GetType(Index(Of String))
                   End Function) _
            .ToArray
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class