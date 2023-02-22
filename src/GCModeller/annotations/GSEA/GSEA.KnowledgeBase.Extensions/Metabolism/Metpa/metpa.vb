Imports RDotNet.Extensions.VisualBasic
Imports RDotNet.Extensions.VisualBasic.API

Namespace Metabolism.Metpa

    Public Class metpa

        ReadOnly obj As New var(base.list)

        Sub New()

        End Sub

        Public Function write(msetList As msetList) As metpa
            obj("mset.list") = msetList.write
            Return Me
        End Function

        Public Function write(rbcList As rbcList) As metpa
            obj("rbc.list") = rbcList.write
            Return Me
        End Function

        Public Function write(pathIds As pathIds) As metpa
            obj("path.ids") = pathIds.write
            Return Me
        End Function

        Public Function write(uniq_count As Integer) As metpa
            obj("uniq.count") = uniq_count
            Return Me
        End Function

        Public Function write(path_smps As pathSmps) As metpa
            obj("path.smps") = path_smps.write
            Return Me
        End Function

        Public Function write(dgrList As dgrList) As metpa
            obj("dgr.list") = dgrList.write
            Return Me
        End Function

        Public Function write(graphList As graphList) As metpa
            obj("graph.list") = graphList.write
            Return Me
        End Function

        Public Function save(path As String) As Boolean
            SyncLock R
                With R
                    !metpa = obj.name

                    Call base.save({"metpa"}, file:=path)
                End With
            End SyncLock

            Return True
        End Function

    End Class
End Namespace