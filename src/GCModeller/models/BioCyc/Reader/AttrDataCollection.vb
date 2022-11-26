#Region "Microsoft.VisualBasic::0e6eb4bec40dd079b0cc40d1400e22bf, GCModeller\models\BioCyc\Reader\AttrDataCollection.vb"

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

    '   Total Lines: 44
    '    Code Lines: 35
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 1.52 KB


    ' Class AttrDataCollection
    ' 
    '     Properties: features, fileMeta
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: LoadFile, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Linq

Public Class AttrDataCollection(Of T As Model)

    Public ReadOnly Property fileMeta As FileMeta
    Public ReadOnly Property features As IEnumerable(Of T)
        Get
            Return models.Values
        End Get
    End Property

    Protected ReadOnly models As Dictionary(Of String, T)

    Default Public ReadOnly Property getFeature(i As String) As T
        Get
            Return models.TryGetValue(i)
        End Get
    End Property

    Sub New(meta As FileMeta, objects As IEnumerable(Of T))
        fileMeta = meta
        models = objects.ToDictionary(Function(o) o.uniqueId)
    End Sub

    Public Overrides Function ToString() As String
        Return fileMeta.ToString
    End Function

    Public Shared Function LoadFile(file As Stream) As AttrDataCollection(Of T)
        Dim dataFile As AttrValDatFile = AttrValDatFile.ParseFile(New StreamReader(file))
        Dim writer As ObjectWriter = ObjectWriter.LoadSchema(Of T)
        Dim data As T() = (From a As SeqValue(Of FeatureElement)
                           In dataFile.features _
                               .SeqIterator _
                               .AsParallel
                           Let obj As Object = writer.Deserize(a.value)
                           Order By a.i
                           Select DirectCast(obj, T)).ToArray

        Return New AttrDataCollection(Of T)(dataFile.fileMeta, data)
    End Function

End Class
