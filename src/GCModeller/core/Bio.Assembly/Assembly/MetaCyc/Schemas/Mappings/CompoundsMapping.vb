#Region "Microsoft.VisualBasic::fbd230379be1db0638c7f8acc0787dac, ..\core\Bio.Assembly\Assembly\MetaCyc\Schemas\Mappings\CompoundsMapping.vb"

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
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Levenshtein
Imports SMRUCC.genomics.Assembly.MetaCyc.File
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports SMRUCC.genomics.ComponentModel.EquaionModel

Namespace Assembly.MetaCyc.Schema

    ''' <summary>
    ''' 查询通用标准名称在MetaCyc数据库Compound之间的等价
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CompoundsMapping : Implements IDisposable

        ''' <summary>
        ''' 用于做等价Mapping的目标数据源
        ''' </summary>
        ''' <remarks></remarks>
        Dim Compounds As ICompoundObject()

        Sub New(MetaCyc As DatabaseLoadder)
            Me.Compounds = MetaCyc.GetCompounds.GetCompoundsAbstract
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="RightSideCompounds">该类型对象是出现在EffectorMap等式的<see cref="EffectorMap.MetaCycId">右边的UniqueId</see></param>
        ''' <remarks></remarks>
        Sub New(RightSideCompounds As IEnumerable(Of ICompoundObject))
            Compounds = RightSideCompounds.ToArray
        End Sub

        Sub New(mets As DataFiles.Compounds)
            Call Me.New(mets.GetCompoundsAbstract)
        End Sub

        Public Function NameQuery(name As String) As ICompoundObject
            Dim LQuery = (From x As ICompoundObject
                          In Compounds
                          Select d = Equals(name, x),
                              x
                          Order By d Descending).First
            Return LQuery.x
        End Function

        Public Overloads Function Equals(name As String, compound As ICompoundObject) As Double
            If String.Equals(name, compound.Key, StringComparison.OrdinalIgnoreCase) Then
                Return 100
            End If
            If String.Equals(name, compound.KEGG_cpd, StringComparison.OrdinalIgnoreCase) Then
                Return 100
            End If

            Dim LQuery = (From s As String
                          In compound.CommonNames
                          Let lev As DistResult = LevenshteinDistance.ComputeDistance(s, name)
                          Where Not lev Is Nothing
                          Select lev.MatchSimilarity).Max
            Return LQuery
        End Function

        ''' <summary>
        ''' 主要的算法思路就是将名称与MetaCyc Compound中的通用名称和同义名进行匹配
        ''' </summary>
        ''' <param name="CompoundSpecies"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EffectorMapping(CompoundSpecies As ICompoundObject()) As EffectorMap()
            Dim Effectors = EffectorMap.GenerateMap(CompoundSpecies)

            For i As Integer = 0 To Effectors.Count - 1
                Dim Effector = Effectors(i)
                Dim LQuery = (From Compound In Compounds Where CompoundsMapping.IsEqual(Effector, Compound) Select Compound).ToArray

                If Not LQuery.IsNullOrEmpty Then '在MetaCyc数据库之中查询到了相对应的记录数据
                    Dim Compound = LQuery.First

                    Effector.MetaCycId = Compound.Key.ToUpper

                    If Not Compound.CommonNames.IsNullOrEmpty Then
                        Effector.CommonName = (From strName As String In Compound.CommonNames Select strName Order By Len(strName) Ascending).First
                        Dim sBuilder As StringBuilder = New StringBuilder(1024)

                        For Each strData As String In Compound.CommonNames
                            Call sBuilder.AppendFormat("[{0}]; ", strData)
                        Next

                        Effector.Synonym = sBuilder.ToString
                    End If
                End If
            Next

            Return Effectors
        End Function

        Public Shared Function IsEqual(Effector As ICompoundObject, Compound As ICompoundObject) As Boolean
            If Not String.IsNullOrEmpty(Effector.PUBCHEM) AndAlso String.Equals(Effector.PUBCHEM, Compound.PUBCHEM) Then
                Return True
            Else
                For Each strEntry As String In Effector.CHEBI
                    If Not String.IsNullOrEmpty(strEntry) AndAlso Array.IndexOf(Compound.CHEBI, strEntry) > -1 Then
                        Return True
                    End If
                Next
            End If

            If IsEqually(Effector.Key, Compound) Then
                Return True
            Else
                If Effector.CommonNames.IsNullOrEmpty Then
                    Return False
                End If
                Dim LQuery = (From Id As String In Effector.CommonNames Where IsEqually(Id, Compound) Select 1).ToArray
                Return Not LQuery.IsNullOrEmpty
            End If
        End Function

        Private Shared Function IsEqually(Effector As String, Compound As ICompoundObject) As Boolean
            If String.Equals(Effector, Compound.Key, StringComparison.OrdinalIgnoreCase) Then
                Return True
            Else
                For Each strName As String In Compound.CommonNames
                    If String.Equals(strName, Effector, StringComparison.OrdinalIgnoreCase) Then
                        Return True
                    End If
                Next
            End If

            Return False
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace
