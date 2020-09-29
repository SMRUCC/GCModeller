#Region "Microsoft.VisualBasic::a20c400345f713a48edcc33ea327861a, engine\IO\Raw\vcXML\VcellAdapterDriver.vb"

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

    '     Class VcellAdapterDriver
    ' 
    '         Properties: flux, mass
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: (+2 Overloads) Dispose, FluxSnapshot, MassSnapshot
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Namespace vcXML

    Public Class VcellAdapterDriver : Implements IOmicsDataAdapter, IDisposable

        Public ReadOnly Property mass As OmicsTuple(Of String()) Implements IOmicsDataAdapter.mass
        Public ReadOnly Property flux As OmicsTuple(Of String())

        Dim fs As Writer

        Sub New(file As String, model As CellularModule, args As FluxBaseline)
            mass = OmicsDataAdapter.GetMassTuples(model)
            flux = OmicsDataAdapter.GetFluxTuples(model)

            fs = New Writer(file, New XmlWriterSettings With {.Indent = True, .OmitXmlDeclaration = True})
            fs.writeInit(Me, args)
        End Sub

        Public Sub MassSnapshot(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.MassSnapshot
            Call fs.addFrame(iteration, NameOf(mass.transcriptome), "mass_profile", data.Takes(mass.transcriptome))
            Call fs.addFrame(iteration, NameOf(mass.proteome), "mass_profile", data.Takes(mass.proteome))
            Call fs.addFrame(iteration, NameOf(mass.metabolome), "mass_profile", data.Takes(mass.metabolome))
        End Sub

        Public Sub FluxSnapshot(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.FluxSnapshot
            Call fs.addFrame(iteration, NameOf(flux.transcriptome), "activity", data.Takes(flux.transcriptome))
            Call fs.addFrame(iteration, NameOf(flux.proteome), "activity", data.Takes(flux.proteome))
            Call fs.addFrame(iteration, NameOf(flux.metabolome), "flux_size", data.Takes(flux.metabolome))
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call fs.Dispose()
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
