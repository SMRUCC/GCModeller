#Region "Microsoft.VisualBasic::64eee8ddf4098191aaf608a2b3680e0d, ..\GCModeller\engine\GCMarkupLanguage\GCML_Documents\GCMLDocBuilder\RegulonBuilder.vb"

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

Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Terminal.STDIO

Namespace Builder

    Public Class RegulonBuilder : Inherits IBuilder

        Dim Data As RowObject()

        Sub New(MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As BacterialModel, DataPackage As IO.File, Optional FirstLineTitle As Boolean = True)
            MyBase.New(MetaCyc, Model)
            If FirstLineTitle Then
                Data = DataPackage.Skip(1)
            Else
                Data = DataPackage.ToArray
            End If
        End Sub

        Public Overrides Function Invoke() As BacterialModel
            Dim LQuery = (From row In Data Select GCML_Documents.XmlElements.Bacterial_GENOME.Regulon.Create(row, MyBase.Model)).ToArray
            Model.BacteriaGenome.Regulons = LQuery.ToArray
            Return MyBase.Model
        End Function
    End Class
End Namespace
