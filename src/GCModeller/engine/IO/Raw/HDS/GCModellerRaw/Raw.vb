#Region "Microsoft.VisualBasic::1f45b03eb0c73a8755aa1c5aecac9cf4, engine\IO\Raw\HDS\GCModellerRaw\Raw.vb"

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


    ' Code Statistics:

    '   Total Lines: 128
    '    Code Lines: 61 (47.66%)
    ' Comment Lines: 51 (39.84%)
    '    - Xml Docs: 72.55%
    ' 
    '   Blank Lines: 16 (12.50%)
    '     File Size: 4.99 KB


    '     Class CellularModules
    ' 
    '         Properties: Metabolites, mRNAId, Polypeptide, Proteins, Reactions
    '                     RNAId
    ' 
    '         Function: GetModuleReader, GetModules
    ' 
    '         Sub: (+2 Overloads) Dispose
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports [Module] = Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps.DataFrameColumnAttribute

Namespace Raw

    ''' <summary>
    ''' The sandbox engine output raw data file format
    ''' </summary>
    Public MustInherit Class CellularModules : Implements IDisposable

        Public Const Magic$ = "GCModeller"

        Protected ReadOnly modules As New Dictionary(Of String, Index(Of String))
        Protected ReadOnly moduleIndex As New Index(Of String)

#Region "Cellular Modules"

        ''' <summary>
        ''' 由基因转录出来的mRNA的编号列表
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>ORF gene id</remarks>
        <[Module]("Message-RNA")>
        Public Property mRNAId As Index(Of String)
        ''' <summary>
        ''' 由基因转录出来的其他的RNA分子的编号列表
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>RNA gene id</remarks>
        <[Module]("Component-RNA")>
        Public Property RNAId As Index(Of String)

        <[Module]("Transfer-RNA")>
        Public Property tRNA As Index(Of String)
        <[Module]("Ribosomal-RNA")>
        Public Property rRNA As Index(Of String)

        ''' <summary>
        ''' 由mRNA翻译出来的多肽链的Id列表
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Usually is the Uniprot protein id</remarks>
        <[Module]("Polypeptide")>
        Public Property Polypeptide As Index(Of String)
        ''' <summary>
        ''' 由一条或者多条多肽链修饰之后得到的最终的蛋白质的编号列表
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Protein complexe</remarks>
        <[Module]("Protein")>
        Public Property Proteins As Index(Of String)
        ''' <summary>
        ''' 代谢物列表
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Usually is the KEGG compounds
        ''' </remarks>
        <[Module]("Metabolite")>
        Public Property Metabolites As Index(Of String)

#Region "Biological Process"

        ''' <summary>
        ''' 反应过程编号列表
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Usually is the KEGG reactions
        ''' </remarks>
        <[Module]("Reaction-Flux")>
        Public Property Reactions As Index(Of String)
        <[Module]("Transcription-Flux")>
        Public Property Transcription As Index(Of String)
        <[Module]("Translation-Flux")>
        Public Property Translation As Index(Of String)
        <[Module]("Protein-Degradation-Flux")>
        Public Property ProteinDegradation As Index(Of String)
        <[Module]("Polypeptide-Degradation-Flux")>
        Public Property PeptideDegradation As Index(Of String)
        <[Module]("RNA-Degradation-Flux")>
        Public Property RNADegradation As Index(Of String)
        <[Module]("tRNA-Charge-Flux")>
        Public Property tRNACharge As Index(Of String)
        <[Module]("ribosome-Assembly-Flux")>
        Public Property ribosomeAssembly As Index(Of String)
        <[Module]("Protein-Mature-Flux")>
        Public Property ProteinMature As Index(Of String)
        <[Module]("Transmembrane-Flux")>
        Public Property TransmembraneFlux As Index(Of String)

#End Region
#End Region

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Function GetModules() As PropertyInfo()
            Return GetType(CellularModules) _
                .GetProperties(PublicProperty) _
                .Where(Function(prop)
                           Return prop.PropertyType Is GetType(Index(Of String))
                       End Function) _
                .ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Function GetModuleReader() As Dictionary(Of String, PropertyInfo)
            Return GetModules _
                .ToDictionary(Function(prop)
                                  Dim modAttr = prop.GetAttribute(Of [Module])

                                  If modAttr Is Nothing OrElse modAttr.Name.StringEmpty Then
                                      Return prop.Name
                                  Else
                                      Return modAttr.Name
                                  End If
                              End Function)
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
End Namespace
